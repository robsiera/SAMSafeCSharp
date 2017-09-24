namespace SamSafeCSharp.Components
{
    public class PresenterModel : BlogPost
    {
        public string __action;
        public string __actionId { get; set; }
        public string __token { get; set; }
        public string __session { get; set; }

        public BlogPost Item { get; set; } // TODO: Check should we need this propery here 
        public BlogPost LastEdited { get; set; }
        public int DeletedItemId { get; set; } = 0;

    }
}