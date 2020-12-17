using System;

namespace Ke_Blog.Models
{
    public class Post
    {
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public DateTime Creates { get; set; } = DateTime.Now;
    }
}
