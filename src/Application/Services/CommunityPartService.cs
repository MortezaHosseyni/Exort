using Domain.Repositories;

namespace Application.Services
{
    public interface ICommunityPartService
    {
    }
    public class CommunityPartService(ICommunityPartRepository communityPart) : ICommunityPartService
    {
        private readonly ICommunityPartRepository _communityPart = communityPart;
    }
}
