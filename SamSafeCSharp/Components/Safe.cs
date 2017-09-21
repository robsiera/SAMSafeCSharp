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
        private bool _saveSnapshot;
        private Actions _actions;
        private Model _model;
        private State _state;
        private View _view;
        private DefaultLogger _logger;
        private DefaultErrorHandler _errorHandler;
        private DefaultSessionManager _sessionManager;
        private bool _hangback;
        private List<Step> _steps;
        private int _stepCount;
        private Step _lastStep;
        private string _allowedActions;
        private Func<PresenterModel, string, bool> _present;
        private Action<PresenterModel, Action<string>> _render;
        private Action<string, Action<string>> _display;
        private bool _blocked;
        private Func<Model, string, int> saveSnapshot;
        private Func<int, string> getSnapshot;
        private Func<string, string> displayTimeTravelControls;


        // SAFE Core

        // Insert SAFE middleware and wire SAM components
        public void Init(Actions actions, Model model, State state, View view, DefaultLogger logger = null, DefaultErrorHandler errorHandler = null, DefaultSessionManager sessionManager = null)
        {
            this._actions = actions;
            this._model = model;
            this._state = state;
            this._view = view;

            this._errorHandler = errorHandler ?? new DefaultErrorHandler();
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
                this._allowedActions = state.AllowedActions ?? "";
            }
        }

        public void InitTimeTraveler(DefaultTimeTraveler timeTraveler)
        {
            if (timeTraveler != null)
            {
                this.saveSnapshot = timeTraveler.SaveSnapshot;
                this.getSnapshot = timeTraveler.GetSnapshot;
                this.displayTimeTravelControls = timeTraveler.DisplayTimeTravelControls;
            }
        }

        // The Dispatcher is an optional component which 
        // exposes a dispatch API 
        public static void Dispatcher(App app, string path, string next)
        {
            // assumes express cookie-parser middleware
            app.Post(path);

            /*JS CODE
            app.post(path, function(req, res) {
                var ret = (representation) => {
                    res.status(200).send(representation);
                };
                var data = req.body;

                data.__token = req.cookies['safe_token'] || '';

                safe.dispatch(data.__action, data, next || ret);
            }) ;
            */
        }

        // The dispatch method decides whether an action can be dispatched
        // based on SAFE's context
        public void Dispatch(Actions action, string data, string next)
        {
            this._logger.Info("dispatcher received request");
            bool dispatch = false;
            var lastStepActions = this._lastStep.Actions;

            /*
             * safe.logger.info('dispatcher received request'+JSON.stringify(data)) ;
                safe.logger.info('lastStepActions            '+JSON.stringify(lastStepActions)) ;
             * 
             * */

            if (lastStepActions.Length == 0)
            {
                // action validation is disabled
                dispatch = true;
            }
            else
            {
                /*
                 lastStepActions.forEach( (a) => {
                safe.logger.info(a) ;
                if (a.action === action) {
                dispatch = true ;
                // tag the action with the stepid
                // we want to enforce one action per step
                // if the step does not match we should not dispatch
                data.__actionId = a.uid ;
                safe.logger.info('tagging action with            '+JSON.stringify(a)) ;
            
                safe.lastStep.dispatched = action ;
            }
             
             */
            }
        }

        public void Present(PresenterModel data, string next)
        {
            string actionId = data.ActionId ?? null;

            if (!this._blocked)
            {
                string lastStepActions = this._lastStep.Actions;
                this._logger.Info(lastStepActions);
                bool presentData = (lastStepActions.Length == 0);

                if (!presentData)
                {
                    // are we expecting that action?
                    foreach (var item in lastStepActions)
                    {
                        //if (item.Id == actionId) TODO
                        //{
                        //    presentData = true;
                        //}
                    }
                }
                if (presentData)
                {
                    Block();
                    if (!string.IsNullOrEmpty(data.__token))
                    {
                        //this.model.__session = this.sessionManager.rehydrateSession(data.__token); TODO
                        this._model.__token = data.__token;
                    }
                    if (this._saveSnapshot)
                    {
                        // Store snapshot in TimeTravel
                        //this.saveSnapshot(null, data); TODO
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
                // this.logger({ blocked: true,data}) ;
            }
        }

        public void Block()
        {
            this._lastStep.Actions = "";
            this._blocked = true;
        }

        public void UnBlock()
        {
            this._blocked = false;
        }

        public Step NewStep(int uId = 0, string allowedActions = null)
        {
            allowedActions = allowedActions ?? "";
            int k = 0;
            Step step = new Step(uId, allowedActions);

            this._steps.Add(step);
            //this.logger.Info('new step        :' + JSON.stringify(step));
            return step;
        }

        public void Render(PresenterModel model, Action<string> next, bool takeSnapshot)
        {
            /*takeSnapShot = takeSnapShot || true;
            if (takeSnapShot && safe.saveSnapshot)
            {
                // Store snapshot in TimeTravel
                safe.saveSnapshot(model, null);
            }*/

            //this.allowedActions = this.state.Representation(model, next) ?? ""; TODO
            this.UnBlock();
            this._lastStep = this.NewStep(this._stepCount++, this._allowedActions);
            this._sessionManager.DehydrateSession(model);
            this._state.NextAction(model);
        }

        public void Display(string representation, Action<string> next)
        {

            if (this.displayTimeTravelControls != null)
            {
                representation = this.displayTimeTravelControls(representation);
            }
            this._view.Display(representation, next);

        }

        public void DeepCopy(string x)
        {
            //return JSON.parse(JSON.stringify(x));
        }

    }
    // minimal service implementations

    // session manager 
    // logger
    public class DefaultLogger
    {
        public int LoggingLevel { get; set; }

        public string Output(string level, string message)
        {
            DateTime time = DateTime.Now;
            string m = message;
            if (message is string)
            {
                // m = JSON.stringify(message) ;
            }

            return String.Format("%s: %s", level, message);
        }

        public void Warning(string message)
        {
            if (this.LoggingLevel <= 1)
            {
                this.Output("[WARNING]", message);
            }
        }
        public void Info(string message)
        {
            if (this.LoggingLevel <= 0)
            {
                this.Output("[INFO]", message);
            }
        }
        public void Error(string message)
        {
            if (this.LoggingLevel <= 0)
            {
                this.Output("[ERROR]", message);
            }
        }

    }
    // error handler
    public class DefaultErrorHandler
    {
        public DefaultErrorHandler(string message = "")
        {
            //Default logger = logger, this = safe volgens js
            //this.logger.Error(message);
        }
    }
    // default in memory store for time travel 
    public class DefaultSnapshotStore
    {
        string[] _store = new string[] { };

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
        public int UId { get; set; }
        public string AllowedActions { get; set; }

        public Step(int uId, string allowedActions)
        {
            this.UId = uId;
            this.AllowedActions = allowedActions;
        }
        public string Actions { get; set; }
    }
}


