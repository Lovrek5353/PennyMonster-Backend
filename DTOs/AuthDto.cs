namespace PennyMonster.DTOs
{
    public class TokenRequestDto
    {
        public string FirebaseUid { get; set; } = "mock-user-123";
        public string Email { get; set; } = "test@pennymonster.com";
        public string FirstName { get; set; } = "Test";
        public string LastName { get; set; } = "User";
    }

    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
    }
}
