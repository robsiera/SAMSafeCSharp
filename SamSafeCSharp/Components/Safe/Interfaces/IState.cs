using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public interface IState
    {
        string[] AllowedActions { get; set; }

        void Init(Action<IModel, Action<string>> render, IView view, Action<string, Action<string>> display = null, Dictionary<string, string> intents = null);

        string[] Representation(IModel model, Action<string> next);

        void NextAction(IModel model);
    }
}
