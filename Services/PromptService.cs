using System.Text.Json;
using PromptCenter.API.Models;
using Microsoft.EntityFrameworkCore;
using PromptCenter.API.Data;

namespace PromptCenter.API.Services
{
    public class PromptService : IPromptService
    {
        private readonly IWebHostEnvironment _environment;

        private readonly AppDbContext _context;

        public PromptService(IWebHostEnvironment environment, AppDbContext context)
        {
            _environment = environment;
            _context = context;
        }

        public List<Prompt> GetPrompts()
        {           
            return _context.Prompts
               .AsNoTracking()
               .ToList();
        }
        public Prompt AddPrompt(Prompt prompt)
        {
            _context.Prompts.Add(prompt);
            _context.SaveChanges();

                return prompt;
        }
        public bool deletePrompt(int id)
        {
            var prompt = _context.Prompts.FirstOrDefault(p => p.Id == id);

            if (prompt == null)
            {
                return false;
            }

            _context.Prompts.Remove(prompt);
            _context.SaveChanges();

            return true;
        }
    }
}