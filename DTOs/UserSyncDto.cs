namespace PennyMonster.DTOs
{
    public class UserSyncDto
    {
        public Guid InternalId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
