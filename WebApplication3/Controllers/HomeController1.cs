using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication3.Model;
using System.Threading.Tasks;
using System;

namespace WebApplication2.Controllers
{
    public class BlobController : Controller
    {
        private readonly FileService _fileService;

        // Constructor: Inject FileService (used for working with Azure Blob Storage)
        public BlobController(FileService fileService)
        {
            _fileService = fileService;
        }

        // GET: Blob/Upload
        [HttpGet("Blob/Upload")]
        public IActionResult Upload()
        {
            return View(); // This will render the Upload.cshtml view
        }

        // POST: Blob/Upload
        [HttpPost("Blob/Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewData["UploadStatus"] = "Please select a file to upload.";
                return View(); // Return to the same page with a message
            }

            try
            {
                var result = await _fileService.UploadAsync(file);

                if (result.error)
                {
                    ViewData["UploadStatus"] = $"Error: {result.Status}";
                    return View(); // Return to the same page with error message
                }

                ViewData["UploadStatus"] = $"File '{file.FileName}' uploaded successfully.";
                return View(); // Return to the same page with success message
            }
            catch (Exception ex)
            {
                ViewData["UploadStatus"] = $"Error: {ex.Message}";
                return View(); // Return to the same page with error message
            }
        }

        // GET: Blob/List
        [HttpGet("Blob/List")]
        public async Task<IActionResult> ListBlobs()
        {
            try
            {
                var blobs = await _fileService.ListAsync();
                return View(blobs); // This will render a List.cshtml view showing the blobs
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(); // Return with error message
            }
        }

        // GET: Blob/Download/{filename}
        [HttpGet("Blob/Download/{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                var blob = await _fileService.DownloadAsync(filename);

                if (blob == null)
                {
                    return NotFound("File not found.");
                }

                return File(blob.Content, blob.ContentType, blob.Name); // Download the file
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // DELETE: Blob/Delete/{filename}
        [HttpDelete("Blob/Delete/{filename}")]
        public async Task<IActionResult> Delete(string filename)
        {
            try
            {
                var result = await _fileService.DeleteAsync(filename);
                return Ok(result.Status); // Return success message
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
