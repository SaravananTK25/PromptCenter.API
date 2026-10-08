namespace PromptCenter.API.models
{   
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PromptId { get; set; }
    }
}
