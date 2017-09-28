using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using SamSafeCSharp.Helpers;

namespace SamSafeCSharp.Components
{
    public class View : IView
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

        public string Ready(IModel model, Dictionary<string, string> intents = null)
        {
            if (intents == null)
                intents = _intents; // todo: intents never used??

            var id = JsHelpers.orDefault(model?.LastEdited?.Id.ToString(), "");
            dynamic viewModel = new
            {
                titleValue = model?.LastEdited?.Title.orDefault("Title"),
                descriptionValue = model?.LastEdited?.Description.orDefault("Description"),
                id = id,
                valAttr = id == "" ? "placeholder" : "value",
                actionLabel = id == "" ? "Add" : "Save",
                idElement = id == "" ? "" : $@", 'id':'{id}'",
                showCancel = id != "",
                posts = model.Posts,
                intents,
            };

            //TemplateRenderingService.Instance.RegisterPartial("post", "postitem");
            //return TemplateRenderingService.Instance.RenderHbs("mainview", viewModel);

            return JsHelpers.JSON.stringify(viewModel);
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