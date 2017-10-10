namespace SamSAFE.Base
{
    public class ProposalInfo
    {
        public ProposalInfo(ActionContext actionContext, object proposalPayload)
        {
            ActionContext = actionContext;
            ProposalPayload = proposalPayload;
        }

        public ActionContext ActionContext { get; }
        public object ProposalPayload { get; }
    }
}
