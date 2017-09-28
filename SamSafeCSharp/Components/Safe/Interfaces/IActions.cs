﻿using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public interface IActions
    {
        Dictionary<string, string> Intents { get; set; }

        Dictionary<string, Action<IProposalModel, Action<string>>> ActionList { get; set; }

        void Init(Action<IProposalModel, Action<string>> present);
    }
}