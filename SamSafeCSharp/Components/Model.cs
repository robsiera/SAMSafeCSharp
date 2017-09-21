using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SamSafeCSharp.Components
{
    public class Model
    {
        public string __token { get; set; }
        public string __session { get; set; }

        //public string render { get; set; }
        //public int itemId { get; set; }
        //public State state { get; set; }

        public List<BlogItem> Posts = new List<BlogItem>()
        {
            new BlogItem()
            {
                Id = 1,
                Title = "The SAM Pattern",
                Description = "SAM is a new reactive/functional pattern that simplifies Front-End architectures by clearly separating the business logic from the view and, in particular, strictly decoupling back-end APIs from the Front-End. SAM is technology independent and as such can be used to build Web Apps or Native Apps"
            },
            new BlogItem()
            {
                Id = 2,
                Title = "Why I no longer use MVC Frameworks",
                Description = "The worst part of my job these days is designing APIs for front-end developers."
            }
        };

        public BlogItem LastDeleted { get; set; }

        public BlogItem LastEdited { get; set; }

        public void Init(string render)
        {
            //this.render = render; // Method in a method
        }

        public void Present (BlogData data, string next)
        {
            if ( data == null )
            {
                data = new BlogData(); //Implementation
            }

            if (data.DeletedItemId != 0)
            {
                var d = -1;

                foreach (var post in Posts)
                {
                    if (post.Id != 0 && post.Id == data.DeletedItemId)
                        d = post.Id;
                }

                if(d > 0)
                    LastDeleted = Posts.ElementAt(d);
            }


            if (data.LastEdited != null)
            {
                LastEdited = data.LastEdited;
            }
            else
            {
                // delete model.lastEdited; TODO check 
            }

            if (data.Item != null)
            {

                if (data.Item.Id > 0)
                {
                    // item has been edited
                    var indexer = 0;
                    foreach (var post in Posts)
                    {
                        if (post.Id > 0 && post.Id == data.Item.Id)
                            Posts[indexer] = data.Item;

                        indexer = indexer + 1;
                    }
                }
                else
                {
                    // new item
                    data.Item.Id = Posts.Max(x => x.Id) + 1; 
                    Posts.Add(data.Item);
                }
            }

            // TODO: Check how to render model...
            //console.log(model);

            //model.render(model, next);

        }
    }
}
