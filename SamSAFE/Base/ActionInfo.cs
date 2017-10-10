namespace SamSAFE.Base
{
    public class ActionInfo
    {
        public ActionInfo(string actionName, object payload)
        {
            ActionContext = new ActionContext(actionName);
            Data = payload;
        }

        public ActionContext ActionContext { get; }
        public object Data { get; }
    }
}
