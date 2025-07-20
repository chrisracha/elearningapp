namespace ELearningApp.Api.Models;

public class Testimonial
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string JobTitle { get; set; } = string.Empty;
}
 