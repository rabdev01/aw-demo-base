using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdventureWorksWeb.Data;
using AdventureWorksWeb.Models;

namespace AdventureWorksWeb.Controllers;

public class CustomersController : Controller
{
    private readonly AdventureWorksContext _context;
    private const int PageSize = 20;

    public CustomersController(AdventureWorksContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                (c.CompanyName != null && c.CompanyName.Contains(search)) ||
                (c.EmailAddress != null && c.EmailAddress.Contains(search)));
        }

        var totalCount = await query.CountAsync();
        var customers = await query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        ViewBag.TotalCount = totalCount;

        return View(customers);
    }
}
