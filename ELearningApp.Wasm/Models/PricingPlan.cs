namespace ELearningApp.Models;

public class PricingPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PriceDescription { get; set; } = string.Empty;
    public bool IsPopular { get; set; }
    public List<string> Features { get; set; } = new();
    public string ButtonText { get; set; } = string.Empty;
    public string ButtonAction { get; set; } = string.Empty;
} 