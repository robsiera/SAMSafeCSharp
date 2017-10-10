using SamSAFE.Interfaces;

namespace SamSafeCSharp.Components.SAM.Dto
{
    public class ProposalPayload
    {
        public int Id { get; set; }
        public BlogPost Item { get; set; }
        public BlogPost LastEdited { get; set; }
        public int DeletedItemId { get; set; } = 0;
    }
}