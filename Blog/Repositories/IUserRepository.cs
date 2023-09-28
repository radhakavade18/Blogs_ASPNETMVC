using Microsoft.AspNetCore.Identity;

namespace Blog.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetAll();
    }
}
