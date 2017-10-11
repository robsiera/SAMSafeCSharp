namespace SamSAFE.Base
{
  public class ActionContext
    {
        public ActionContext(string actionName)
        {
            this.__action = actionName;
        }

        public string __action { get; }

        /// <summary>
        /// Gets or sets the action Universal Identifier which includes Step information
        /// </summary>
        /// </value>
        public string __actionUId { get; set; }
        public string __token { get; set; }
        public string __session { get; set; }
    }
}
