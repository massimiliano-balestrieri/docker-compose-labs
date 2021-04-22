using FriendsApi.Types.Models;
using Microsoft.EntityFrameworkCore;

namespace FriendsApi.Context
{
    public class FriendContext : DbContext
    {
        public FriendContext(DbContextOptions<FriendContext> options) : base(options)
        {
        }

        public virtual DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Friend>(entity =>
            {
                entity.HasKey(x => x.Id);
            });
        }

    }
}
