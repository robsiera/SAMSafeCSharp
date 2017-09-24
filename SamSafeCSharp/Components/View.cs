using System;
using System.Collections.Generic;
using System.Diagnostics;
using SamSafeCSharp.Helpers;

namespace SamSafeCSharp.Components
{
    public class View
    {
        public View()
        {
        }

        //view.intents = { edit: 'edit', save: 'save', delete: 'delete', cancel: 'cancel' } ;
        private readonly Dictionary<string, string> _intents = new Dictionary<string, string>()
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

        public string Ready(Model model, Dictionary<string, string> intents = null)
        {
            if (intents == null)
                intents = _intents;

            var titleValue = model?.LastEdited?.Title.orDefault("Title");
            var descriptionValue = model?.LastEdited?.Description.orDefault("Description");
            var id = model?.LastEdited?.Id.ToString().orDefault("");
            var cancelButton = $@"<button id=""cancel"" onclick=""JavaScript:return actions.cancel({{}});\"">Cancel</button>{Environment.NewLine}";
            var valAttr = "value";
            var actionLabel = "Save";
            var idElement = $@", 'id':'{id}'";
            if (id.IsNullOrEmpty())
            {
                cancelButton = "";
                valAttr = "placeholder";
                idElement = "";
                actionLabel = "Add";
            }
            var output = (
                $@"<br><br><div class=""blog-post"">{Environment.NewLine}" + model.Posts.map((post) =>
                {
                    return TemplateRenderingService.Instance.RenderHbs("postitem", post);

                }).@join($"{Environment.NewLine}") + $@"{Environment.NewLine}</div>{Environment.NewLine}
                <br><br>{Environment.NewLine}
                <div class=""mdl-cell mdl-cell--6-col"">{Environment.NewLine}
                <input id=""title"" type=""text"" class=""form-control""  {valAttr}=""{titleValue}""><br>{Environment.NewLine}
                <input id=""description"" type=""textarea"" class=""form-control"" {valAttr}=""{descriptionValue}""><br>{Environment.NewLine}
                <button id=""save"" onclick=""JavaScript:return actions.save({{'title':document.getElementById('title').value, 'description': document.getElementById('description').value{idElement}}});"">{actionLabel}</button>
                {Environment.NewLine}{cancelButton}{Environment.NewLine}</div>
                <br><br>{Environment.NewLine}");
            return output;

        }

        public void Display(string representation, Action<string> next)
        {
            if (next != null)
            {
                next(representation);
            }
            else
            {
                Debugger.Break();
                //todo uncomment and fix
                //var stateRepresentation = document.getElementById("representation");
                //stateRepresentation.innerHTML = representation;
            }
        }
    }
}