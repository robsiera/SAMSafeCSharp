using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamSafeCSharp.Components
{
    public interface IView
    {
        string Init(dynamic model);
        void Display(string representation, Action<string> next);
        string Ready(IModel model, Dictionary<string, string> intents);
    }
}
