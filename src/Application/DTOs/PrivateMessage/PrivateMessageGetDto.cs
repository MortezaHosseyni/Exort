namespace Application.DTOs.PrivateMessage
{
    public class PrivateMessageGetDto
    {
        public Ulid Id { get; set; }
        
        public string? Text { get; set; }
        public Ulid SenderId { get; set; }
        public Ulid ReceiverId { get; set; }
        public Ulid? Reply { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
