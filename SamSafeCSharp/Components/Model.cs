namespace SamSafeCSharp.Components
{
    public class Model
    {
        public string __token { get; set; }
        public string __session { get; set; }
        public string render { get; set; }
        public string posts { get; set; } // to be implemented into a model
        public string lastDeleted { get; set; }
        public string lastEdited { get; set; }
        public int itemId { get; set; }
        public SafeCSharp.State state { get; set; }
        

        public void init(string render)
        {
            this.render = render; // Method in a method
        }

        public void present(BlogItem data, string next)
        {
            if(data == null)
            {
                data = new BlogItem(); //Implementation
            }

            //Present logic
            int index = -1;
            int d = -1 ;
            foreach (var item in this.posts)
            {
                index += 1;
                if (el.id == data.deletedItemId)
                {
                    d = index;
                }
            }            
            if (d>=0) {
                this.lastDeleted = this.posts[d].ToString();
            }

            if (data?.lastEdited != null)
            {
                this.lastEdited = data.lastEdited;
            }
            else
            {
                this.lastEdited = null;
            }

            if(data?.item != null)
            {
                if(data.item.id > 0)
                {
                    var index = 0;
                    foreach(var el in this.posts)
                    {
                        if(el.id == data.item.id)
                        {
                            this.posts[] = data.item;
                        }
                    }
                }
                else
                {
                    data.item.id = this.itemId++;
                    this.posts += data.item;

                }
            }
            state.Render(this, next);
        }
    }
}
