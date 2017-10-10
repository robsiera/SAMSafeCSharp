using SamSAFE.Interfaces;

namespace SamSafeCSharp.Components.SAM.Dto
{
    public class Proposal : IProposal
    {
        public string __actionId { get; set; }
        public string __token { get; set; }

        public int Id { get; set; }
        public BlogPost Item { get; set; }
        public BlogPost LastEdited { get; set; }
        public int DeletedItemId { get; set; } = 0;
    }
}