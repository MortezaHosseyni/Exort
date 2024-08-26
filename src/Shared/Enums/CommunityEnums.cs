namespace Shared.Enums.Community
{
    public enum CommunityStatus
    {
        Deleted = 0,
        Active = 1,
        Banned = 2,
        Restricted = 3
    }
    public enum CommunityType
    {
        Private = 1,
        Public = 2
    }
    public enum CommunityChannelType
    {
        Text = 1,
        Voice = 2
    }

    public enum CommunityChannelStatus
    {
        Deleted = 0,
        Active = 1,
        Inactive = 2
    }

    public enum UserCommunityStatus
    {
        Active = 1,
        Banned = 2,
    }
}
