using PromptCenter.API.Models;

namespace PromptCenter.API.Services
{
    public interface IPromptService
    {
        List<Prompt> GetPrompts();
        Prompt AddPrompt(Prompt prompt);
        bool deletePrompt(int id);
    }
}