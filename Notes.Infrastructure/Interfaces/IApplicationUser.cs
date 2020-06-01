using Notes.Database.Entities;
using Notes.Models.ApplicationUser;
using System.Threading.Tasks;

namespace Notes.Infrastructure.Interfaces
{
    public interface IApplicationUser
    {
        Task Add(ApplicationUserUpsertModel model);
        ApplicationUser Get(ApplicationUserGetRequestModel model);
        Task<ApplicationUser> Get(int id);
    }
}
