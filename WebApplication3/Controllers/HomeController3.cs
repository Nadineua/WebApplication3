using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;
using WebApplication3.Models.WebApplication3.Model; // Import the AzureFunctionCaller class

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly AzureFunctionCaller _azureFunctionCaller;

        // Constructor to inject the AzureFunctionCaller via Dependency Injection
        public FunctionController(AzureFunctionCaller azureFunctionCaller)
        {
            _azureFunctionCaller = azureFunctionCaller;
        }

        // Action to call the Azure Function and get the response
        [HttpGet("call-function")]
        public async Task<IActionResult> CallFunction()
        {
            try
            {
                // Call the Azure Function and get the result
                string result = await _azureFunctionCaller.CallFunctionAsync();

                // Return the result as a JSON response
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                // If something goes wrong, return a BadRequest response with the error message
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
