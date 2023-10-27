using DatingApp.Entities;

namespace DatingApp.Interfaces
{
    public interface ITokenServices
    {
        string CreateToken(AppUser user);
    }
}
