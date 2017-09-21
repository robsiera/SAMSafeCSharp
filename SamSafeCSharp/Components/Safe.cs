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
        private bool saveSnapshot;

        public Actions actions { get; set; }
        public Model model { get; set; }
        public State state { get; set; }
        public View view { get; set; }
        public DefaultLogger logger { get; set; }
        public defaultErrorHandler errorHandler { get; set; }
        public defaultSessionManager sessionManager { get; set; }
        public bool hangback { get; set; }
        public List<Step> steps { get; set; }
        public int stepCount { get; set; }
        public Step lastStep { get; set; }
        public string allowedActions { get; set; }
        public string present { get; set; }
        public string render { get; set; }
        public string display { get; set; }
        public bool blocked { get; set; }



        // SAFE Core

        // Insert SAFE middleware and wire SAM components
        public void init(Actions actions, Model model, State state, View view, DefaultLogger logger = null, defaultErrorHandler errorHandler = null, defaultSessionManager sessionManager = null)
        {
            this.actions = actions;
            this.model = model;
            this.state = state;
            this.view = view;

            this.errorHandler = errorHandler ?? new defaultErrorHandler();
            this.logger = logger ?? new DefaultLogger();

            this.hangback = false;
            this.steps = new List<Step>();
            this.stepCount = 0;
            this.lastStep = this.newStep(this.stepCount);

            this.sessionManager = sessionManager ?? new defaultSessionManager();

            this.allowedActions = null;

            if (actions != null && model != null)
            {
                actions.Init(this.present);
            }

            if (state != null)
            {
                if (model != null)
                {
                    model.Init(this.render);
                    state.init(this.render, this.view);
                }
            }
            if (view != null)
            {
                if (actions != null)
                {
                    state.init(null, view, this.display, actions.Intents);
                }
                else
                {
                    state.init(null, view, this.display);
                }
                this.allowedActions = state.allowedActions ?? "";
            }
        }

        public void initTimeTraveler(DefaultTimeTraveler timeTraveler)
        {
            if (timeTraveler != null)
            {
                /*
                this.saveSnapshot = timeTraveler.saveSnapshot ?? null;
                this.getSnapshot = timeTraveler.getSnapshot ?? null;
                this.displayTimeTravelControls = timeTraveler.displayTimeTravelControls ?? null;
                */
            }
        }

        // The Dispatcher is an optional component which 
        // exposes a dispatch API 
        public void dispatcher(App app, string path, string next)
        {
            // assumes express cookie-parser middleware
            app.post(path);

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
            this.logger.Info("dispatcher received request");
            bool dispatch = false;
            var lastStepActions = this.lastStep.Actions;

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

        public void Present(BlogData data, string next)
        {
            string actionId = data.ActionId ?? null;

            if (!this.blocked)
            {
                string lastStepActions = this.lastStep.Actions;
                this.logger.Info(lastStepActions);
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
                        this.model.__token = data.__token;
                    }
                    if (this.saveSnapshot)
                    {
                        // Store snapshot in TimeTravel
                        //this.saveSnapshot(null, data); TODO
                    }
                    this.model.Present(data, next);
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
            this.lastStep.Actions = "";
            this.blocked = true;
        }

        public void unBlock()
        {
            this.blocked = false;
        }

        public Step newStep(int uId = 0, string allowedActions = null)
        {
            allowedActions = allowedActions ?? "";
            int k = 0;
            Step step = new Step(uId, allowedActions);

            this.steps.Add(step);
            //this.logger.Info('new step        :' + JSON.stringify(step));
            return step;
        }

        public void Render(BlogData model, string next, bool takeSnapshot)
        {
            /*takeSnapShot = takeSnapShot || true;
            if (takeSnapShot && safe.saveSnapshot)
            {
                // Store snapshot in TimeTravel
                safe.saveSnapshot(model, null);
            }*/

            //this.allowedActions = this.state.Representation(model, next) ?? ""; TODO
            this.unBlock();
            this.lastStep = this.newStep(this.stepCount++, this.allowedActions);
            this.sessionManager.dehydrateSession(model);
            this.state.NextAction(model);
        }

        public void Display(string representation, string next)
        {
            /*
            if (this.displayTimeTravelControls)
            {
                representation = safe.displayTimeTravelControls(representation);
            }*/
            this.view.Display(representation, next);

        }

        public void DeepCopy(string x)
        {
            //return JSON.parse(JSON.stringify(x));
        }
        
    }
    // minimal service implementations

    // session manager 
    public class defaultSessionManager
    {
        public void dehydrateSession(object model)
        {
            /*if (model.__token)
            {
                safe.defaultSessionManager[model.__token] = model.__session;
            }*/
        }

        public object rehydrateSession(object token)
        {
            object session = token;
            return session;
        }
    };
    // logger
    public class DefaultLogger
    {
        public int loggingLevel { get; set; }

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
            if (this.loggingLevel <= 1)
            {
                this.Output("[WARNING]", message);
            }
        }
        public void Info(string message)
        {
            if (this.loggingLevel <= 0)
            {
                this.Output("[INFO]", message);
            }
        }
        public void Error(string message)
        {
            if (this.loggingLevel <= 0)
            {
                this.Output("[ERROR]", message);
            }
        }

    }
    // error handler
    public class defaultErrorHandler
    {
        public void DefaultErrorHandler(string message)
        {
            //Default logger = logger, this = safe volgens js
            //this.logger.Error(message);
        }
    }
    // default in memory store for time travel 
    public class DefaultSnapshotStore
    {
        string[] store;

        public int Store(int i, string s)
        {
            store[i] = s;
            return store.Length;
        }

        public string Retrieve(int i)
        {
            if (i >= 0 && i < store.Length)
                return store[i];
            return null;
        }

        public string[] RetrieveAll()
        {
            return store;
        }


    }
    // time traveler
    public class DefaultTimeTraveler
    {
        public DefaultSnapshotStore snapshotStore { get; set; }
        public int cursor { get; set; }

        /* JS code
         *  let display =(res,representation) => {
                res.status(200).send(representation) ;
            } ; 
         * */

        public DefaultTimeTraveler(DefaultSnapshotStore store = null)
        {
            snapshotStore = store ?? new DefaultSnapshotStore();
            cursor = -1;
        }


        public void Init(App app, string path, string next)
        {
            /*display = next || display;

            // API : returns a given snapshot or all if index is negative 
            app.get(path + '/:snapshot', function(req, res) {

                let index = req.params.snapshot;
                res.setHeader("Content-Type", "application/json");
                display(res, JSON.stringify(safe.getSnapshot(index)));

            }) ;

            app.get(path, function(req, res) {
                res.setHeader("Content-Type", "application/json");
                display(res, JSON.stringify(safe.getSnapshot()));

            }) ;

            // travel back
            app.post(path + '/:snapshot', function(req, res) {

                let dis = (representation) => {
                    let resp = res;
                    display(resp, representation);
                }


                let index = req.params.snapshot;

                if (index >= 0)
                {
                    let snapshot = snapshotStore.retrieve(index);
                    if ((index >= 0) && (index < snapshotStore.length()))
                    {
                        cursor = index;
                    }
                    let m = safe.deepCopy(snapshot.store);
                    m.__token = req.cookies['safe_token'] || '';

                    let modelProperties = Object.getOwnPropertyNames(safe.model).filter(function(p) {
                        return typeof safe.model[p] !== 'function';
                    }) ;

                    let snapShotProperties = Object.getOwnPropertyNames(m);

                    modelProperties.forEach(function(p) {
                        delete safe.model[p];
                    });

                    snapShotProperties.forEach(function(p) {
                        safe.model[p] = m[p];
                    });

                    safe.render(safe.model, dis, false);
                }
            }) ;*/
        }

        public int SaveSnapshot(Model model, string dataset)
        {
            // TODO check this
            string snapshot = snapshotStore.Retrieve(cursor);
            //if (dataset != null)
            //{
            //    cursor++;
            //    snapshot = null;
            //    snapshot = this.DeepCopy(dataset);
            //}

            //if (model != null)
            //    snapshot.store = this.DeepCopy(model);

            return snapshotStore.Store(cursor, snapshot);
        }

        public string GetSnapshot(int i)
        {
            if (i == 0)
                i = -1;

            if (i >= 0)
            {
                return snapshotStore.Retrieve(i);
            }
            else
            {
                return snapshotStore.RetrieveAll().ToString();
            }
        }

        public string DisplayTimeTravelControls(string representation)
        {
            /*JS CODE
             * 
             * return (representation + 
        '          <br>\n<br>\n<hr>\n<div class="mdl-cell mdl-cell--6-col">\n'+
        '                      <input id="__snapshot" type="text" class="form-control"><br>\n'+
        '                      <button id="__travel" onclick="JavaScript:return travel({\'snapshot\':document.getElementById(\'__snapshot\').value});"> TimeTravel </button>\n'+
        '          </div><br><br>\n') ;
             * 
             * 
             * */
            return null;
        }
    }

    public class Step
    {
        public int uId { get; set; }
        public string allowedActions { get; set; }

        public Step(int uId, string allowedActions)
        {
            this.uId = uId;
            this.allowedActions = allowedActions;
        }
        public string Actions { get; set; }
    }
}


