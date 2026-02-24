using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRUD.Web.Models;
using EnterpriseCRUD.Application.Common.Interfaces;

namespace EnterpriseCRUD.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMetadataStore _metadataStore;

    public HomeController(IMetadataStore metadataStore)
    {
        _metadataStore = metadataStore;
    }

    public async Task<IActionResult> Index()
    {
        var modules = await _metadataStore.GetAllAsync();
        ViewBag.ModuleCount = modules.Count;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
