namespace SamSAFE.Base
{
  public class ActionContext
    {
        public ActionContext(string intentName)
        {
            this.__intentName = intentName;
        }

        public string __intentName { get; }

        /// <summary>
        /// Gets or sets the action Universal Identifier which includes Step information
        /// </summary>
        /// </value>
        public string __actionUId { get; set; }
        public string __token { get; set; }
        public string __session { get; set; }
    }
}
