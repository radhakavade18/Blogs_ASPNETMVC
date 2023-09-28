namespace Blog.Models.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? DisplayName { get; set; }

        // Tags can have multiple blog posts - relation between blogs and tags
        public ICollection<BlogPost>? BlogPosts { get; set; }
    }
}
