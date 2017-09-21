using System.Collections.Generic;

namespace SamSafeCSharp.Components
{
    public class State
    {
        public string allowedActions { get; set; }
        public View view { get; set; }
        public string display { get; set; }
        public Dictionary<string, string> intents { get; set; }


        public void init(string render, View view, string display = null, Dictionary<string, string> intents = null)
        {
            if(render != null)
            {
                this.view = view;
                //this.display = display ?? view.Display; // Method in a method

            }

            if(intents != null)
            {
                this.intents = intents;
            }
            //if (render != null)
                //this.Render = render; // Method in a method
        }

        // Derive the state representation as a function of the systen
        // control state
        public string[] Representation(BlogData model, string next)
        {
            var representation = "Oops... something went wrong, the system";
            if (this.Ready(model))
            {
                representation = this.view.Ready(model, this.intents);
            }
            //this.display(representation, next); 
            view.Display(representation, next);
            
            
            // return allowed actions
            return new string[] {"edit", "save", "delete", "cancel" };
        }

        // Derive the current state of the system
        public bool Ready(BlogData model)
        {
            return true;
        }

        // Next action predicate, derives whether
        // the system is in a (control) state where
        // a new (next) action needs to be invoked
        public void NextAction(BlogData model){}


        public void Render(BlogData model, string next)
        {
            this.Representation(model, next);
            this.NextAction(model);
        }

    }
}
