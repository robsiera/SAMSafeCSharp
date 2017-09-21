namespace SamSafeCSharp.Components
{
    public class PresenterModel : BlogPost
    {
        public BlogPost Item { get; set; } // TODO: Check should we need this propery here 
        public BlogPost LastEdited { get; set; }
        public int DeletedItemId { get; set; } = 0;

        public string __token { get; set; }
        public string __session { get; set; }

        public string ActionId { get; set; }
        
    }
}