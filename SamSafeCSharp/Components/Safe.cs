/******************************************************************************************* 
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
using SamSafeCSharp.Helpers;


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





namespace SamSafeCSharp.Components
{
    public class Safe
    {
        private Func<Model, string, int> _saveSnapshot;
        private Actions _actions;
        private Model _model;
        private State _state;
        private View _view;
        private DefaultLogger _logger;
        private Action<string> _errorHandler;
        private DefaultSessionManager _sessionManager;
        private List<Step> _steps;
        private int _stepCount;
        private Step _lastStep;
        private string[] _allowedActions;
        private bool _hangback;
        private Func<int, string> _getSnapshot;
        private Action<PresenterModel, Action<string>> _present;
        private Action<Model, Action<string>> _render;
        private Action<string, Action<string>> _display;
        private bool _blocked;
        private Func<string, string> _displayTimeTravelControls;


        // SAFE Core

        // Insert SAFE middleware and wire SAM components
        public void Init(Actions actions, Model model, State state, View view, DefaultLogger logger = null, Action<string> errorHandler = null, DefaultSessionManager sessionManager = null)
        {
            this._actions = actions;
            this._model = model;
            this._state = state;
            this._view = view;

            this._errorHandler = errorHandler ?? DefaultErrorHandler;
            this._logger = logger ?? new DefaultLogger();

            this._hangback = false;
            this._steps = new List<Step>();
            this._stepCount = 0;
            this._lastStep = this.NewStep(this._stepCount);

            this._sessionManager = sessionManager ?? new DefaultSessionManager();

            this._allowedActions = null;

            if (actions != null && model != null)
            {
                actions.Init(this._present);
            }

            if (state != null)
            {
                if (model != null)
                {
                    model.Init(this.Render);
                    state.Init(this._render, this._view);
                }
            }
            if (view != null)
            {
                if (actions != null)
                {
                    state.Init(null, view, this._display, actions.Intents);
                }
                else
                {
                    state.Init(null, view, this._display);
                }
                this._allowedActions = state.AllowedActions ?? new string[] { };
            }
        }

        public void InitTimeTraveler(DefaultTimeTraveler timeTraveler)
        {
            if (timeTraveler != null)
            {
                this._saveSnapshot = timeTraveler.SaveSnapshot;
                this._getSnapshot = timeTraveler.GetSnapshot;
                this._displayTimeTravelControls = timeTraveler.DisplayTimeTravelControls;
            }
        }


        // The dispatch method decides whether an action can be dispatched
        // based on SAFE's context
        public void Dispatch(string action, PresenterModel data, Action<string> next)
        {
            this._logger.Info("dispatcher received request");
            bool dispatch = false;
            var lastStepActions = this._lastStep.Actions;

            this._logger.Info("dispatcher received request" + JsHelpers.JSON.stringify(data));
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
                    if (lastStep.Action == action)
                    {
                        dispatch = true;
                        // tag the action with the stepid
                        // we want to enforce one action per step
                        // if the step does not match we should not dispatch
                        data.__actionId = lastStep.UId;
                        this._logger.Info("tagging action with            " + lastStep.ToString());

                        this._lastStep.Dispatched = action;
                    }
                }
            }

            if (!dispatch)
            {
                this._errorHandler(new { action = action, error = "not allowed" }.ToString());
            }
            else
            {

                if (this._actions.ActionList.ContainsKey(action))
                {
                    // dispatch action
                    this._logger.Info("invoking action            " + data.ToString());
                    this._actions.ActionList[action](data, next);
                }
                else
                {
                    this._errorHandler(new { action = action, error = "not found" }.ToString());
                }

            }
        }

        public void Present(PresenterModel data, Action<string> next)
        {
            string actionId = data.ActionId ?? null;

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

        public void Block()
        {
            this._lastStep.Actions = new List<StepAction>();
            this._blocked = true;
        }

        public void UnBlock()
        {
            this._blocked = false;
        }

        public Step NewStep(int uId = 0, string[] allowedActions = null)
        {
            _allowedActions = allowedActions ?? new string[] { };
            Step step = new Step(uId, allowedActions);

            this._steps.Add(step);
            this._logger.Info("new step        :" + JsHelpers.JSON.stringify(step));
            return step;
        }

        public void Render(Model model, Action<string> next)
        {
            Render(model, next, true);
        }

        public void Render(Model model, Action<string> next, bool takeSnapshot)
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

        public void Display(string representation, Action<string> next)
        {

            if (this._displayTimeTravelControls != null)
            {
                representation = this._displayTimeTravelControls(representation);
            }
            this._view.Display(representation, next);

        }

        public void DefaultErrorHandler(string message = "")
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
        public StepAction(string uid, string allowedAction)
        {
            this.UId = uid;
            this.Action = allowedAction;
        }
        public string UId { get; }
        public string Action { get; }

        public override string ToString()
        {
            return $"{UId} - {Action}";
        }
    }
}


