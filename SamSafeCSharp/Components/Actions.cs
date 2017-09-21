using System;
using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public class Actions
    {
        public readonly Dictionary<string, string> Intents = new Dictionary<string, string>()
        {
            {"edit","edit"},
            {"save","save"},
            {"delete","delete"},
            {"cancel","cancel"},
        };

        private Func<PresenterModel, string, bool> _present;

        public void Init(Func<PresenterModel, string,bool> present)
        {
            this._present = present ?? Present;
        }

        public static bool Present(PresenterModel data, string next = null)
        {
            return false;
        }


        public bool Edit(PresenterModel data, string next)
        {
            data.LastEdited = new BlogPost { Title = data.Title, Description = data.Description, Id = data.Id };
            Present(data, next);
            return false;
        }

        public bool Save(PresenterModel data, string next)
        {
            data.Item = new BlogPost { Title = data.Title, Description = data.Description, Id = data.Id };

            if (data.Item.Id > 0)
            {
                // simulate a slow save after
                // editing an item
                /*
                setTimeout(function() {
                    actions.present(data, next);
                }, 9000);
                */
            }
            else
            {
                // proceed as normal when created a new item
                Present(data, next);
            }
            return false;
        }

        private static string EmptyifNull(string dataId)
        {
            if (string.IsNullOrEmpty(dataId)) return "";
            return dataId;
        }

        public bool Cancel(PresenterModel data, string next)
        {
            Present(data, next);
            return false;
        }


    }
}
