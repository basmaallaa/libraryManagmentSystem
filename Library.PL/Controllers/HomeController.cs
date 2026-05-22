using Library.BLL.Services.Interfaces;
using Library.PL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.PL.Controllers
{
    public class HomeController : Controller
    {
        // NOTE: ILibraryService injection is optional — the homepage works with demo data
        // without it. Once you're ready, inject it and uncomment the ViewBag lines below.
         private readonly ILibraryService _libraryService;

        public HomeController( ILibraryService libraryService )
        {
            _libraryService = libraryService;
        }

        public async Task<IActionResult> Index()
        {
            // Optional: load real books from the database and pass to the view.
            // Uncomment and remove the demo data from Index.cshtml once ready.
            //
            var result = await _libraryService.GetFilteredBooksAsync(null, null, null);
            if (result.IsSuccess)
                ViewBag.FeaturedBooks = result.Data;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
