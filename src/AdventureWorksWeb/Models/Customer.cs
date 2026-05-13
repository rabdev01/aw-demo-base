namespace AdventureWorksWeb.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string? Title { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? SalesPerson { get; set; }
    public string? EmailAddress { get; set; }
    public string? Phone { get; set; }
}
