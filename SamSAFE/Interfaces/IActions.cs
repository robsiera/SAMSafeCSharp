using System;
using System.Collections.Generic;
using SamSAFE.Base;

namespace SamSAFE.Interfaces
{
    public interface IActions
    {
        Dictionary<string, string> Intents { get; set; }

        //Dictionary<string, Action<IProposalModel, Action<string>>> ActionList { get; set; }

        void Init(Action<ProposalInfo, Action<string>> present);

        bool ActionExists(string actionKey);

        void Handle(ActionInfo actioninfo, Action<string> next);
    }
}