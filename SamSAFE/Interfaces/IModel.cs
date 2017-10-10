using System;
using SamSAFE.Base;

namespace SamSAFE.Interfaces
{
    public interface IModel
    {
        string __session { get; set; }
        string __token { get; set; }


        void Init(Action<IModel, Action<string>> render);

        void Present(ProposalInfo proposalInfo, Action<string> next);

    }
}
