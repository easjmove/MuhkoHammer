using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MuhkoHammer.ModelClasses;
using System.Data.SqlClient;

namespace MuhkoHammer.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(ILogger<IndexModel> logger)
        {
        }

        public void OnGet()
        {
        }
        public void OnPost()
        {
        }
    }
}