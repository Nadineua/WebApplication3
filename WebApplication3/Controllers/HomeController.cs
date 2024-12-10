using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
    public class Index1 : PageModel
    {
        private readonly BlobStorageService _blobStorageService;

        public Index1(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        // This method is called when the form is submitted
        public async Task OnPostAsync(IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                await _blobStorageService.UploadFileAsync("mycontainer", file.FileName, stream);
            }
        }
    }
}
