using Domain.Repositories;

namespace Application.Services
{
    public interface IUserService
    {
    }
    public class UserService(IUserRepository user) : IUserService
    {
        private readonly IUserRepository _user = user;
    }
}
