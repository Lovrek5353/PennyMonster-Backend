namespace PennyMonster.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.MinValue;

        public DateTime LastModified {  get; set; } = DateTime.MinValue;
    }

    public class CategoryCreateDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Color { get; set; }
    }

    public class CategoryUpdateDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Color { get; set; }
    }
}
