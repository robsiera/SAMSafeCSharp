﻿namespace SamSafeCSharp.Components
{
    public class BlogPost : IItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}