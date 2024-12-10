using Microsoft.AspNetCore.Mvc;
using WebApplication3.Data;
using WebApplication3.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace WebApplication24.Controllers
{
    public class TestsController : Controller
    {
        private readonly MyDbContext _context;

        // Constructor: Inject the DbContext for working with the database
        public TestsController(MyDbContext context)
        {
            _context = context;
        }
       
        [HttpGet ("/Tests")]
        public async Task<IActionResult> Index()
        {
            // Fetch and display all Test records from the database
            return View(await _context.Test.ToListAsync());
        }

        [HttpGet("Tests/Create")]
        public IActionResult Create()
        {
            return View(); // Render the Create view
        }

        // POST: /Tests/Create
        [HttpPost("Tests/Create")]
        [ValidateAntiForgeryToken] // CSRF Protection
        public async Task<IActionResult> Create([Bind("Name")] Testdto ts)
        {
            if(string.IsNullOrWhiteSpace(ts.Name))
            {
                ts.Name= "Test";
            }
            var tes = new Test();
            tes.Name = ts.Name;
            _context.Add(tes);
            await _context.SaveChangesAsync();

            return View(); // Render the Create view
        }
    }
}
