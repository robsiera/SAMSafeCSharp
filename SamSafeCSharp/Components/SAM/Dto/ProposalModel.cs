using SamSAFE.Interfaces;

namespace SamSafeCSharp.Components.SAM.Dto
{
  

    public class ProposalModel : IProposalModel
    {
        public string __action { get; set; }
        public string __actionId { get; set; }
        public string __token { get; set; }
        public string __session { get; set; }

        public int Id { get; set; }
        public BlogPost Item { get; set; }
        public BlogPost LastEdited { get; set; }
        public int DeletedItemId { get; set; } = 0;
    }
}