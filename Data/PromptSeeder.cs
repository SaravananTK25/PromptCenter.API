using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PromptCenter.API.Models;

namespace PromptCenter.API.Data
{
    public class PromptSeeder // if network down will take prompt from json --> for this purpose only.
    {
        public static void Seed(AppDbContext context, IWebHostEnvironment environment)
        {
            // If prompts already exist, don't import again
            if (context != null && context.Prompts != null && context.Prompts.Any())
            {
                return;
            }

            var filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "prompts.json"
            );

            if (!File.Exists(filePath))
            {
                return;
            }

            var json = File.ReadAllText(filePath);

            var jsonPrompts = JsonSerializer.Deserialize<List<Prompt>>(json);

            if (jsonPrompts == null || jsonPrompts.Count == 0)
            {
                return;
            }
            var prompts = jsonPrompts.Select(prompt => new Prompt
            {
                Title = prompt.Title,
                //Content = prompt.Content,
                Category = prompt.Category
            }).ToList();

            context.Prompts.AddRange(prompts);

            context.SaveChanges();
        }
    }
}