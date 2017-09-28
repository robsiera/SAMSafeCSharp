using System;
using System.Collections.Generic;
using System.Linq;
using SamSafeCSharp.Helpers;

namespace SamSafeCSharp.Components
{
    public class Model
    {
        private readonly IJsonStore _jsonStore;

        public Model(IJsonStore jsonStore)
        {
            _jsonStore = jsonStore;
            var sessionPosts = _jsonStore.GetObjectFromJson<List<BlogPost>>("sessionPosts");
            Posts = sessionPosts ?? GetDefaultPosts();
        }

        private static List<BlogPost> GetDefaultPosts()
        {
            return new List<BlogPost>()
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
        }

        public List<BlogPost> Posts { get; }

        public void Init(Action<Model, Action<string>> render)
        {
            this.Render = render;
        }

        public void Present(ProposalModel data, Action<string> next)
        {
            if (data == null)
            {
                data = new ProposalModel(); //Implementation
            }

            if (data.DeletedItemId != 0)
            {
                // delete item but keep reference in LastDeleted
                LastDeleted = Posts.FirstOrDefault(post => post.Id != 0 && post.Id == data.DeletedItemId);
                if (LastDeleted != null)
                {
                    Posts.Remove(LastDeleted);
                }
            }

            if (data.LastEdited != null)
            {
                LastEdited = data.LastEdited;
            }
            else
            {
                LastEdited = null;  // delete model.lastEdited; 
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

            _jsonStore.SetObjectAsJson("sessionPosts", Posts);

            //console.log(model);
            this.Render(this, next);

        }



        public string __token { get; set; }
        public string __session { get; set; }

        private Action<Model, Action<string>> Render { get; set; }

        public int ItemId { get; set; }
        public State State { get; set; }

        public BlogPost LastDeleted { get; set; }

        public BlogPost LastEdited { get; set; }

    }
}
