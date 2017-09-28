﻿using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public class State
    {
        internal string[] AllowedActions { get; set; }
        private View _view;
        private Action<string, Action<string>> _display;
        private Action<Model, Action<string>> _render; //todo _render never used??
        private Dictionary<string, string> _intents;


        public void Init(Action<Model, Action<string>> render, View view, Action<string, Action<string>> display = null, Dictionary<string, string> intents = null)
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

        /// <summary>
        /// Derive the state representation as a function of the system control state
        /// </summary>
        public string[] Representation(Model model, Action<string> next)
        {
            var representation = "Oops... something went wrong, the system";
            if (this.Ready(model))
            {
                representation = this._view.Ready(model, this._intents);
            }
            this._display(representation, next);

            // return allowed actions
            return new string[] { "edit", "save", "delete", "cancel" };
        }

        /// <summary>
        /// Next action predicate, derives whether the system is in a (control) state 
        /// where a new (next) action needs to be invoked
        /// </summary>
        public void NextAction(Model model)
        {

        }

        /// <summary>
        /// Derive the current state of the system
        /// </summary>
        private bool Ready(Model model)
        {
            return true;
        }

        private void DefaultRender(Model model, Action<string> next)
        {
            this.Representation(model, next);
            this.NextAction(model);
        }

    }
}
