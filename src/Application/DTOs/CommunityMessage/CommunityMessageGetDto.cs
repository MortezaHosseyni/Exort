namespace Application.DTOs.CommunityMessage
{
    public class CommunityMessageGetDto
    {
        public Ulid Id { get; set; }
        
        public string? Text { get; set; }
        public Ulid SenderId { get; set; }
        public Ulid CommunityId { get; set; }
        public Ulid CommunityChannelId { get; set; }
        public Ulid? Reply { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
