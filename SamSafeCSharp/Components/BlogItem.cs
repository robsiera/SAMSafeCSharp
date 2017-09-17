namespace SamSafeCSharp.Components
{
    public class BlogItem
    {
        public string actionId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public BlogItem lastEdited { get; set; }
        public BlogItem item { get; set; }
        public string deletedItemId { get; set; }
    }
}