using System;
using System.Collections.Generic;
using SamSAFE.Base;

namespace SamSAFE.Interfaces
{
    public abstract class BaseActions<TActionModel, TProposalModel>
    {
        public static readonly Dictionary<string, string> AllIntents = new Dictionary<string, string>();

        public Dictionary<string, string> Intents { get; protected set; }

        private Dictionary<string, Action<ActionContext, TActionModel, Action<string>>> ActionList { get; } = new Dictionary<string, Action<ActionContext, TActionModel, Action<string>>>();

        protected Action<ActionContext, TProposalModel, Action<string>> Present;

        public void Init(Action<ActionContext, TProposalModel, Action<string>> present)
        {
            this.Present = present ?? DefaultPresent;
        }

        public bool IntentExists(string intentKey)
        {
            return AllIntents.ContainsKey(intentKey);
        }

        public static string GetActionByIntent(string intentKey)
        {
            return AllIntents[intentKey];
        }

        /// <summary>
        /// Default Presenter Method. 
        /// </summary>
        private static void DefaultPresent(ActionContext actionContext, TProposalModel data, Action<string> next = null)
        {
            // if this presenter is used, that means we forgot to specify one
            throw new NotImplementedException("Present function not properly initialized?");
        }

        public void Handle(ActionContext actionContext, TActionModel actionPayload, Action<string> next)
        {
            ActionList[AllIntents[actionContext.__intentName]](actionContext, actionPayload, next);
        }

        protected void RegisterActionHandler(string actionName, Action<ActionContext, TActionModel, Action<string>> actionHandler)
        {
            ActionList.Add(actionName, actionHandler);
        }

        protected void RegisterIntent(string intentName, string actionName)
        {
            AllIntents.Add(intentName, actionName);
        }
    }
}