using Microsoft.AspNetCore.Mvc;
using PromptCenter.API.Models;
using PromptCenter.API.Services;

namespace PromptCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromptsController : ControllerBase
    {
        private readonly IPromptService _promptService;

        public PromptsController(IPromptService promptService)
        {
            _promptService = promptService;
        }
        
        [HttpGet]
        public IActionResult GetPrompts()
        {
            var prompts = _promptService.GetPrompts();

            return Ok(prompts);
        }
        [HttpPost]
        public IActionResult AddPrompt([FromBody] Prompt prompt)
        {
            if (prompt == null)
            {
                return BadRequest("Prompt data is required.");
            }

            var createdPrompt = _promptService.AddPrompt(prompt);

            return Ok(createdPrompt);
        }
        [HttpDelete("{id}")]
        public IActionResult DeletePrompt(int id)
        {
            var deleted = _promptService.deletePrompt(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Prompt not found."
                });
            }

            return Ok(new
            {
                message = "Prompt deleted successfully."
            });
        }
    }
}