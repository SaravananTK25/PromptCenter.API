using Microsoft.EntityFrameworkCore;
using PromptCenter.API.models;
using PromptCenter.API.Models;

namespace PromptCenter.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Prompt> Prompts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}