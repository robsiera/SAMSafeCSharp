using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public class Actions
    {
        public readonly Dictionary<string, Action<ProposalModel, Action<string>>> ActionList = new Dictionary<string, Action<ProposalModel, Action<string>>>();

        public Actions()
        {
            ActionList.Add("edit", Edit);
            ActionList.Add("save", Save);
            ActionList.Add("delete", Delete);
            ActionList.Add("cancel", Cancel);
        }

        public readonly Dictionary<string, string> Intents = new Dictionary<string, string>()
        {
            {"edit","edit"},
            {"save","save"},
            {"delete","delete"},
            {"cancel","cancel"},
        };

        private Action<ProposalModel, Action<string>> _present;

        public void Init(Action<ProposalModel, Action<string>> present)
        {
            this._present = present ?? DefaultPresent;
        }

        /// <summary>
        /// Default Presenter Method. 
        /// </summary>
        private static void DefaultPresent(ProposalModel data, Action<string> next = null)
        {
            // if this presenter is used, that means we forgot to specify one
            throw new NotImplementedException("Present function not properly initialized?");
        }

        public void Edit(ProposalModel data, Action<string> next)
        {
            data.LastEdited = new BlogPost { Title = data.Item.Title, Description = data.Item.Description, Id = data.Item.Id };
            _present(data, next);
        }

        public void Save(ProposalModel data, Action<string> next)
        {
            data.Item = new BlogPost { Title = data.Item.Title, Description = data.Item.Description, Id = data.Item.Id };

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
                _present(data, next);
            }
            else
            {
                // proceed as normal when created a new item
                _present(data, next);
            }
        }

        public void Delete(ProposalModel data, Action<string> next)
        {
            data.DeletedItemId = data.Id;
            _present(data, next);
        }

        public void Cancel(ProposalModel data, Action<string> next)
        {
            _present(data, next);
        }
    }
}