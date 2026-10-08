namespace PromptCenter.API.Models
{
    public class Prompt
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string PromptText { get; set; } = string.Empty;

        public string NegativePrompt { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
    }
}