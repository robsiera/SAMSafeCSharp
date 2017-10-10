namespace SamSAFE.Base
{
  public class ActionContext
    {
        public ActionContext(string actionName)
        {
            this.__action = actionName;
        }

        public string __action { get; }
        public string __actionId { get; set; }
        public string __token { get; set; }
        public string __session { get; set; }
    }
}
