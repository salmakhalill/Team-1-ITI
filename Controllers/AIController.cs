using Microsoft.AspNetCore.Mvc;
using Team_1_ITI.Services.AI;

namespace Team_1_ITI.Controllers
{
    public class AIController : Controller
    {
        private readonly AIService _aiService;

        public AIController(AIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Chat()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ask(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return BadRequest("Question is required.");
            }

            string answer = await _aiService.AskAsync(question);

            return Json(new
            {
                answer
            });
        }
    }
}