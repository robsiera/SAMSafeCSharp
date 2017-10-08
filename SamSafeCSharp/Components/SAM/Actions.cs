using System;
using System.Collections.Generic;
using SamSafeCSharp.Components.SAM.Dto;
using SamSAFE.Base;
using SamSAFE.Interfaces;

namespace SamSafeCSharp.Components.SAM
{
    /// <summary>
    /// Actions are responsible for implementing context specific logic. 
    /// If we take the example of a "change of address" action, we might implement some context specific rules, 
    /// such as when there is no country specified in the input dataset, the default country is Australia, 
    /// while the model is responsible for the integrity of a customer address which requires a country value. 
    /// In the context of SAM, actions play another important role with respect to invoking 3rd party APIs. 
    /// For instance, we can define an action which will invoke a 3rd party validation service, which given an address, 
    /// returns the postal address (or an error). It is then the postal address which is presented to the model.
    /// </summary>
    public class Actions : IActions
    {
        private Action<IProposalModel, Action<string>> _present;

        public Dictionary<string, string> Intents { get; set; }

        public Dictionary<string, Action<ActionPayload, Action<string>>> ActionList { get; }

        public Actions()
        {
            Intents = new Dictionary<string, string>
            {
                { "edit", "edit" },
                { "save", "save" },
                { "delete", "delete" },
                { "cancel", "cancel" }
            };

            ActionList = new Dictionary<string, Action<ActionPayload, Action<string>>>
            {
                { "edit", Edit },
                { "save", Save },
                { "delete", Delete },
                { "cancel", Cancel }
            };
        }

        public void Init(Action<IProposalModel, Action<string>> present)
        {
            this._present = present ?? DefaultPresent;
        }

        public bool ActionExists(string actionKey)
        {
            return ActionList.ContainsKey(actionKey);
        }

        public void Handle(ActionInfo actioninfo, Action<string> next)
        {
            ActionList[actioninfo.Name](actioninfo.Data as ActionPayload, next);
        }

        /// <summary>
        /// Default Presenter Method. 
        /// </summary>
        private static void DefaultPresent(IProposalModel data, Action<string> next = null)
        {
            // if this presenter is used, that means we forgot to specify one
            throw new NotImplementedException("Present function not properly initialized?");
        }

        private void Edit(ActionPayload data, Action<string> next)
        {
            var proposal = new ProposalModel
            {
                LastEdited = new BlogPost
                {
                    Title = data.Item.Title,
                    Description = data.Item.Description,
                    Id = data.Item.Id
                }
            };
            _present(proposal, next);
        }

        private void Save(ActionPayload data, Action<string> next)
        {
            var proposal = new ProposalModel
            {
                Item = new BlogPost
                {
                    Title = data.Item.Title,
                    Description = data.Item.Description,
                    Id = data.Item.Id
                }
            };

            if (data.Item.Id > 0)
            {
                // simulate a slow save after editing an item
                /*
                setTimeout(function() {
                    actions.present(data, next);
                }, 9000);
                */

                // slow save simulation not yet implemented in C#
                // save normally
                _present(proposal, next);
            }
            else
            {
                // proceed as normal when created a new item
                _present(proposal, next);
            }
        }

        private void Delete(ActionPayload data, Action<string> next)
        {
            var proposal = new ProposalModel
            {
                DeletedItemId = data.Id
            };
            _present(proposal, next);
        }

        private void Cancel(ActionPayload data, Action<string> next)
        {
            var proposal = new ProposalModel { };
            _present(proposal, next);
        }
    }
}