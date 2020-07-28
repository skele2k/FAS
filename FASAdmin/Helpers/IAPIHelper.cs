using FASAdmin.Models;
using System.Threading.Tasks;

namespace FASAdmin.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}