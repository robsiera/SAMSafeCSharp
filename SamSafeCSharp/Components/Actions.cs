namespace SamSafeCSharp.Components
{
    public class Actions
    {
        public string intents { get; set; }

        public void init(string present)
        {
            //Method in a method? ~ W
            //this.present = present;
            var intents = new { edit = "edit", save = "save", delete = "delete", cancel = "cancel" };
        }

        public bool Present(BlogItem data, string next = null)
        {
            return false;
        }


        public bool Edit(BlogItem data, string next)
        {
            data.lastEdited = new BlogItem { title = data.title, description = data.description, id = data.id };
            Present(data, next);
            return false;
        }

        public bool Save(BlogItem data, string next)
        {
            data.item = new BlogItem { title = data.title, description = data.description, id = EmptyifNull(data.id) };
            if (data.item.id != "")
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
                this.Present(data, next);
            }
            return false;
        }

        private static string EmptyifNull(string dataId)
        {
            if (string.IsNullOrEmpty(dataId)) return "";
            return dataId;
        }

        public bool Cancel(BlogItem data, string next)
        {
            this.Present(data, next);
            return false;
        }


    }
}
