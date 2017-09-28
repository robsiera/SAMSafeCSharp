using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public interface IModel
    {
        string __session { get; set; }

        string __token { get; set; }

        void Init(Action<IModel, Action<string>> render);

        void Present(IProposalModel data, Action<string> next);

        List<IItem> Posts { get; }

        IItem LastEdited { get; set; }
    }
}
