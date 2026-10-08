using PromptCenter.API.Models;

namespace PromptCenter.API.models
{
    public class UserFavorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PromptId { get; set; }

        public User User { get; set; } = null!;

        public Prompt Prompt { get; set; } = null!;
    }
}
