using Blog.Models.Domain;

namespace Blog.Repositories
{
    public interface ITagRepository
    {
        // get all tags
        Task<IEnumerable<Tag>> GetAllAsync();

        // get single tag by using ID
        Task<Tag?> GetAsync(Guid id);

        // add new tag
        Task<Tag> AddAsync(Tag tag);

        // Update tag
        Task<Tag?> UpdateAsync(Tag tag);

        // Delete tag
        Task<Tag?> DeleteAsync(Guid id);
    }
}
