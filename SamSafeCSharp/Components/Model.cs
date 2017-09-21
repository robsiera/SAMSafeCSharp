using System;
using System.Collections.Generic;
using System.Linq;

namespace SamSafeCSharp.Components
{
    public class Model
    {
        public readonly List<BlogPost> Posts = new List<BlogPost>()
        {
            new BlogPost()
            {
                Id = 1,
                Title = "The SAM Pattern",
                Description = "SAM is a new reactive/functional pattern that simplifies Front-End architectures by clearly separating the business logic from the view and, in particular, strictly decoupling back-end APIs from the Front-End. SAM is technology independent and as such can be used to build Web Apps or Native Apps"
            },
            new BlogPost()
            {
                Id = 2,
                Title = "Why I no longer use MVC Frameworks",
                Description = "The worst part of my job these days is designing APIs for front-end developers."
            }
        };

        public void Init(Action<PresenterModel, Action<string>, bool> render)
        {
            this.Render = render; 
        }

        public void Present(PresenterModel data, string next)
        {
            if (data == null)
            {
                data = new PresenterModel(); //Implementation
            }

            if (data.DeletedItemId != 0)
            {
                var d = -1;

                foreach (var post in Posts)
                {
                    if (post.Id != 0 && post.Id == data.DeletedItemId)
                        d = post.Id;
                }

                if (d > 0)
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



        public string __token { get; set; }
        public string __session { get; set; }

        public Action<PresenterModel, Action<string>, bool> Render { get; set; }
        public int ItemId { get; set; }
        public State State { get; set; }
        
        public BlogPost LastDeleted { get; set; }

        public BlogPost LastEdited { get; set; }

}
}
