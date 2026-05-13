using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdventureWorksWeb.Data;
using AdventureWorksWeb.Models;
using System.Diagnostics;

namespace AdventureWorksWeb.Controllers;

public class HomeController : Controller
{
    private readonly AdventureWorksContext _context;

    public HomeController(AdventureWorksContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCount = await _context.Products.CountAsync();
        ViewBag.CustomerCount = await _context.Customers.CountAsync();
        ViewBag.CategoryCount = await _context.ProductCategories.CountAsync();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
