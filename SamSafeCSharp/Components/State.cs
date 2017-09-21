using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public class State
    {
        internal string AllowedActions { get; set; }
        private View _view;
        private Action<string, Action<string>> _display;
        private Action<PresenterModel, Action<string>> _render;
        private Dictionary<string, string> _intents;


        public void Init(Action<PresenterModel, Action<string>> render, View view, Action<string, Action<string>> display = null, Dictionary<string, string> intents = null)
        {
            if (render != null)
            {
                this._view = view;
                this._display = display ?? view.Display;
            }

            if (intents != null)
            {
                this._intents = intents;
            }

            this._render = render ?? this.DefaultRender;
        }

        // Derive the state representation as a function of the systen
        // control state
        public string[] Representation(PresenterModel model, Action<string> next)
        {
            var representation = "Oops... something went wrong, the system";
            if (this.Ready(model))
            {
                representation = this._view.Ready(model, this._intents);
            }
            //this.display(representation, next); 
            _view.Display(representation, next);


            // return allowed actions
            return new string[] { "edit", "save", "delete", "cancel" };
        }

        // Derive the current state of the system
        public bool Ready(PresenterModel model)
        {
            return true;
        }

        // Next action predicate, derives whether the system is in a (control) state 
        // where a new (next) action needs to be invoked
        public void NextAction(PresenterModel model) { }


        public void DefaultRender(PresenterModel model, Action<string> next)
        {
            this.Representation(model, next);
            this.NextAction(model);
        }

    }
}
