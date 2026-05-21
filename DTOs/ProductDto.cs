namespace Getdata1.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public CategoryDto? Category { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string>? GalleryImages { get; set; }

        // Technical Specifications
        public string? Stiffness { get; set; }
        public string? Weight { get; set; }
        public string? Color { get; set; }
        public string? BalancePoint { get; set; }
        public string? Length { get; set; }
        public string? Material { get; set; }
        public string? RecommendedTension { get; set; }
        public string? IncludedString { get; set; }
        public string? GripSize { get; set; }
        public string? Size { get; set; }
        public string? Speed { get; set; }
        public string? Origin { get; set; }
        public string? Warranty { get; set; }

        public List<ProductReviewDto>? Reviews { get; set; } = new List<ProductReviewDto>();

        public double AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : 0;
        public int ReviewCount => Reviews?.Count ?? 0;
        }

        public class ProductReviewDto
        {
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        public int HelpfulCount { get; set; }
        }
        }