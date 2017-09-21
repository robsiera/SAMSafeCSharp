namespace SamSafeCSharp.Components
{
    public class BlogData
    {
        public BlogItem LastEdited { get; set; }
        public BlogItem Item { get; set; } // TODO: Check should we need this propery here 
        public int DeletedItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }

        public string __token { get; set; }

        public string __session { get; set; }

        public string ActionId { get; set; }
        
        
    }
}