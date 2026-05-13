namespace AdventureWorksWeb.Models;

public class ProductCategory
{
    public int ProductCategoryID { get; set; }
    public int? ParentProductCategoryID { get; set; }
    public string Name { get; set; } = string.Empty;

    public ProductCategory? ParentCategory { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
