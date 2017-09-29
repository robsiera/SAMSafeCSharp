using System;
using System.Collections.Generic;

namespace SamSAFE.Interfaces
{
    public interface IView
    {
        string Init(dynamic model);
        void Display(string representation, Action<string> next);
        string Ready(IModel model, Dictionary<string, string> intents);
    }
}
