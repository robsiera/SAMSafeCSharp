using System;
using System.Collections.Generic;
using System.Diagnostics;
using SamSafeCSharp.Helpers;
using SamSAFE.Interfaces;

namespace SamSafeCSharp.Components.Sam
{
    public class View : IView
    {
        public View()
        {
        }

        public string Ready(IModel iModel, Dictionary<string, string> intents = null)
        {
            var model = iModel as Model;

            if (intents == null)
                intents = Actions.AllIntents; // todo: intents never used??

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

        #region SAM boilerplate code

        public string Init(dynamic model)
        {
            return Ready(model);
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

        #endregion
    }
}