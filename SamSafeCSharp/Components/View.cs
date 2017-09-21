using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;

namespace SamSafeCSharp.Components
{
    public class View
    {
        //view.intents = { edit: 'edit', save: 'save', delete: 'delete', cancel: 'cancel' } ;
        private Dictionary<string, string> Intents = new Dictionary<string, string>()
        {
            {"edit","edit"},

            {"save","save"},

            {"delete","delete"},

            {"cancel","cancel"},
        };
            
        public string Init(dynamic model)
        {
            return Ready(model);
        }

        public void Display(string representation, string next)
        {
            //TODO
        }

        public string Ready(dynamic model, Dictionary<string, string> intents = null)
        {
            string output = string.Empty;
            if (intents == null)
                intents = Intents;

            foreach (var post in model.Posts)
            {
                output = output + "<br><br>" +
                         @"<div class=""mdl -cell mdl-cell--6-col"" >" +
                         @"<h3 id=""title""> " +
                         post.Title +
                         "</h3>"
                         + "<br>" +
                         @"<p id=""description""> "
                          + post.Description + 
                          "</p>"
                           + "<br>" +
                         @"</div><br><br>";
            }
            

            return output;
        }
    }
}