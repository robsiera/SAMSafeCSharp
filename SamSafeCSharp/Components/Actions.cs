using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public class Actions
    {
        public Dictionary<string, string> Intents = new Dictionary<string, string>()
        {
            {"edit","edit"},

            {"save","save"},

            {"delete","delete"},
            
            {"cancel","cancel"},
        };

        public void Init(string present)
        {
            //Method in a method? ~ W
            //this.present = present;
        }

        public bool Present(BlogData data, string next = null)
        {
            return false;
        }


        public bool Edit(BlogData data, string next)
        {
            data.LastEdited = new BlogItem { Title = data.Title, Description = data.Description, Id = data.Id };
            Present(data, next);
            return false;
        }

        public bool Save(BlogData data, string next)
        {
            data.Item = new BlogItem { Title = data.Title, Description = data.Description, Id = data.Id};

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

        public bool Cancel(BlogData data, string next)
        {
            this.Present(data, next);
            return false;
        }


    }
}
