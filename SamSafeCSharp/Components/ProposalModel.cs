namespace SamSafeCSharp.Components
{
    public class ProposalModel 
    {
        public string __action { get; set; }
        public string __actionId { get; set; }
        public string __token { get; set; }
        public string __session { get; set; }


        public int Id { get; set; }
        public BlogPost Item { get; set; } // TODO: Check should we need this propery here 
        public BlogPost LastEdited { get; set; }
        public int DeletedItemId { get; set; } = 0;
    }
}