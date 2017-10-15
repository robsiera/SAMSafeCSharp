using System;
using System.Collections.Generic;
using SamSafeCSharp.Components.Sam.Dto;
using SamSAFE.Base;
using SamSAFE.Interfaces;

namespace SamSafeCSharp.Components.Sam
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
    public class Actions : BaseActions<ActionPayload, ProposalPayload>
    {
        #region Action Names Constants

        public static string Edit => "edit";
        public static string Save => "save";
        public static string Delete => "delete";
        public static string Cancel => "cancel";

        #endregion

        public Actions()
        {
            RegisterActionHandler(Actions.Edit, EditHandler);
            RegisterActionHandler(Actions.Save, SaveHandler);
            RegisterActionHandler(Actions.Delete, DeleteHandler);
            RegisterActionHandler(Actions.Cancel, CancelHandler);

            RegisterIntent("edit", Actions.Edit);
            RegisterIntent("save", Actions.Save);
            RegisterIntent("delete", Actions.Delete);
            RegisterIntent("cancel", Actions.Cancel);

            Intents = AllIntents;
        }

        #region Action Handlers / Proposal Creators

        private void EditHandler(ActionContext actionContext, ActionPayload data, Action<string> next)
        {
            var proposalPayload = new ProposalPayload
            {
                LastEdited = new BlogPost
                {
                    Title = data.Item.Title,
                    Description = data.Item.Description,
                    Id = data.Item.Id
                }
            };
            Present(actionContext, proposalPayload, next);
        }

        private void SaveHandler(ActionContext actionContext, ActionPayload data, Action<string> next)
        {
            var proposalPayload = new ProposalPayload
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
                Present(actionContext, proposalPayload, next);
            }
            else
            {
                // proceed as normal when created a new item
                Present(actionContext, proposalPayload, next);
            }
        }

        private void DeleteHandler(ActionContext actionContext, ActionPayload data, Action<string> next)
        {
            var proposalPayload = new ProposalPayload
            {
                DeletedItemId = data.Id
            };
            Present(actionContext, proposalPayload, next);
        }

        private void CancelHandler(ActionContext actionContext, ActionPayload actionPayload, Action<string> next)
        {
            var proposalPayload = new ProposalPayload { };
            Present(actionContext, proposalPayload, next);
        }

        #endregion

    }
}