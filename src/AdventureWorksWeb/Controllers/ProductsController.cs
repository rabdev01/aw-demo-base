using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdventureWorksWeb.Data;
using AdventureWorksWeb.Models;

namespace AdventureWorksWeb.Controllers;

public class ProductsController : Controller
{
    private readonly AdventureWorksContext _context;
    private const int PageSize = 20;

    public ProductsController(AdventureWorksContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.ProductNumber.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        ViewBag.TotalCount = totalCount;

        return View(products);
    }
}
