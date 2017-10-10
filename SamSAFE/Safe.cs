﻿/******************************************************************************************* 
 * The MIT License (MIT)
 * -----------------------------------------------------------------------------------------
 * Copyright (c) 2016 Convergence Modeling LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in the 
 * Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 *  
 * https://opensource.org/licenses/MIT
 * 
 */

using System;
using System.Collections.Generic;
using SamSAFE.Base;
using SamSAFE.Interfaces;

// /////////////////////////////////////////////////////////////////
// State-Action Fabric Element 
// SAFE is a micro-container for server-side SAM implementations
// 
// SAFE implements the following services
//  - action dispatcher 
//  - action hang back
//  - session dehydration / hydration
//  - enforces allowed actions and action "hang back"
//  - logging
//
//  TODO
//  - server side time travel
//  - caching (idempotent actions)
// import { saveSnapshot, getSnapshot } from './timeTravelStore.js'  ;





namespace SamSAFE
{
    public class Safe
    {
        private Func<IModel, string, int> _saveSnapshot;
        private IActions _actions;
        private IModel _model;
        private IState _state;
        private IView _view;
        private ILogger _logger;
        private Action<string> _errorHandler;
        private ISessionManager _sessionManager;
        private List<Step> _steps;
        private int _stepCount;
        private Step _lastStep;
        private string[] _allowedActions;
        private Func<int, string> _getSnapshot; //todo why isn't is being used?
        private bool _blocked;
        private Func<string, string> _displayTimeTravelControls;


        // SAFE Core

        /// <summary>
        /// Insert SAFE middleware and wire SAM components
        /// </summary>
        public void Init(IActions actions, IModel model, IState state, IView view, ILogger logger = null, Action<string> errorHandler = null, ISessionManager sessionManager = null)
        {
            this._actions = actions;
            this._model = model;
            this._state = state;
            this._view = view;

            this._errorHandler = errorHandler ?? DefaultErrorHandler;
            this._logger = logger ?? new DefaultLogger();

            this._steps = new List<Step>();
            this._stepCount = 0;
            this._lastStep = this.NewStep(this._stepCount);

            this._sessionManager = sessionManager ?? new DefaultSessionManager();

            this._allowedActions = null;

            if (actions != null && model != null)
            {
                actions.Init(this.Present);
            }

            if (state != null)
            {
                if (model != null)
                {
                    model.Init(this.Render);
                    state.Init(this.Render, this._view);
                }
            }
            if (view != null && state != null)
            {
                if (actions != null)
                {
                    state.Init(null, view, this.Display, actions.Intents);
                }
                else
                {
                    state.Init(null, view, this.Display);
                }
                this._allowedActions = state.AllowedActions ?? new string[] { };
            }
        }

        public void InitTimeTraveler(ITimeTraveler timeTraveler)
        {
            if (timeTraveler != null)
            {
                this._saveSnapshot = timeTraveler.SaveSnapshot;
                this._getSnapshot = timeTraveler.GetSnapshot;
                this._displayTimeTravelControls = timeTraveler.DisplayTimeTravelControls;
            }
        }


        /// <summary>
        /// The dispatch method decides whether an action can be dispatched
        /// based on SAFE's context
        /// </summary>
        public void Dispatch(string actionname, object actionPayload, Action<string> next)
        {
            var actionInfo = new ActionInfo(actionname, actionPayload);

            this._logger.Info("dispatcher received request");
            bool dispatch = false;
            var lastStepActions = this._lastStep.Actions;

            this._logger.Info("dispatcher received request" + JsHelpers.JSON.stringify(actionInfo.Data));
            this._logger.Info("lastStepActions            " + JsHelpers.JSON.stringify(lastStepActions));


            if (lastStepActions.Count == 0)
            {
                // action validation is disabled
                dispatch = true;
            }
            else
            {
                foreach (var lastStep in lastStepActions)
                {
                    this._logger.Info(lastStep.ToString());
                    if (lastStep.ActionName == actionInfo.Name)
                    {
                        dispatch = true;
                        // tag the action with the stepid
                        // we want to enforce one action per step
                        // if the step does not match we should not dispatch
                        actionInfo.__actionId = lastStep.UId;
                        this._logger.Info("tagging action with            " + lastStep.ToString());

                        this._lastStep.Dispatched = actionInfo.Name;
                    }
                }
            }

            if (!dispatch)
            {
                this._errorHandler(new { action = actionInfo.Name, error = "not allowed" }.ToString());
            }
            else
            {

                if (this._actions.ActionExists(actionInfo.Name))
                {
                    // dispatch action
                    this._logger.Info("invoking action            " + actionInfo.Data.ToString());
                    this._actions.Handle(actionInfo, next);
                }
                else
                {
                    this._errorHandler(new { action = actionInfo.Name, error = "not found" }.ToString());
                }

            }
        }

        private void Present(IProposalModel data, Action<string> next)
        {
            string actionId = data.__actionId ?? null;

            if (!this._blocked)
            {
                var lastStepActions = this._lastStep.Actions;
                this._logger.Info(lastStepActions.ToString());
                bool presentData = (lastStepActions.Count == 0);

                if (!presentData)
                {
                    // are we expecting that action?
                    foreach (var item in lastStepActions)
                    {
                        if (item.UId == actionId)
                        {
                            presentData = true;
                        }
                    }
                }
                if (presentData)
                {
                    Block();
                    if (!string.IsNullOrEmpty(data.__token))
                    {
                        this._model.__session = this._sessionManager.RehydrateSession(data.__token);
                        this._model.__token = data.__token;
                    }
                    if (this._saveSnapshot != null)
                    {
                        // Store snapshot in TimeTravel
                        this._saveSnapshot(null, data.ToString());
                    }
                    this._model.Present(data, next);
                }
                else
                {
                    // unexpected actions
                    // possibly an action from previous step that needs to be blocked
                }
            }
            else
            {
                // ignore action's effect
                // this.logger({ blocked: true,data}) ; //todo
            }
        }

        private void Block()
        {
            this._lastStep.Actions = new List<StepAction>();
            this._blocked = true;
        }

        private void UnBlock()
        {
            this._blocked = false;
        }

        private Step NewStep(int uId = 0, string[] allowedActions = null)
        {
            _allowedActions = allowedActions ?? new string[] { };
            Step step = new Step(uId, allowedActions);

            this._steps.Add(step);
            this._logger.Info("new step        :" + JsHelpers.JSON.stringify(step));
            return step;
        }

        private void Render(IModel model, Action<string> next)
        {
            Render(model, next, true);
        }

        private void Render(IModel model, Action<string> next, bool takeSnapshot)
        {
            //todo check next lines
            /*takeSnapShot = takeSnapShot || true;
            if (takeSnapShot && this.saveSnapshot)
            {
                // Store snapshot in TimeTravel
                this.saveSnapshot(model, null);
            }*/

            this._allowedActions = this._state.Representation(model, next);
            this.UnBlock();
            this._lastStep = this.NewStep(this._stepCount++, this._allowedActions);
            this._sessionManager.DehydrateSession(model);
            this._state.NextAction(model);
        }

        private void Display(string representation, Action<string> next)
        {

            if (this._displayTimeTravelControls != null)
            {
                representation = this._displayTimeTravelControls(representation);
            }
            this._view.Display(representation, next);

        }

        private void DefaultErrorHandler(string message = "")
        {

            this._logger.Error(message);
        }
    }


    // minimal service implementations

    // session manager 
    // logger

    // default in memory store for time travel 
    public class DefaultSnapshotStore
    {
        readonly string[] _store = new string[] { };

        public int Store(int i, string s)
        {
            _store[i] = s;
            return _store.Length;
        }

        public string Retrieve(int i)
        {
            if (i >= 0 && i < _store.Length)
                return _store[i];
            return null;
        }

        public string[] RetrieveAll()
        {
            return _store;
        }
    }

    // time traveler
    public class Step
    {
        public Step(int uId, string[] allowedActions)
        {
            this.StepId = uId;
            this.Actions = new List<StepAction>();
            if (allowedActions != null)
            {
                var k = 0;
                foreach (var allowedAction in allowedActions)
                {
                    this.Actions.Add(new StepAction(uId + "_" + k++, allowedAction));
                }
            }
        }
        public int StepId { get; set; }
        public List<StepAction> Actions { get; set; }

        public string Dispatched;
    }

    public class StepAction
    {
        public StepAction(string uid, string allowedActionName)
        {
            this.UId = uid;
            this.ActionName = allowedActionName;
        }
        public string UId { get; }
        public string ActionName { get; }

        public override string ToString()
        {
            return $"{UId} - {ActionName}";
        }
    }
}


