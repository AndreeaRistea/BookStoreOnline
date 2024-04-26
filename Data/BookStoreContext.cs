using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proiect.Models;
using Proiect_CE.Models;

namespace Proiect_CE.Data
{
    public class BookStoreContext : IdentityDbContext<IdentityUser>
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<PublishingHouse> PublishingHouses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WishList>()
                 .HasOne(e => e.User)
                 .WithOne(e => e.WishList)
                 .HasForeignKey<User>();

            modelBuilder.Entity<Cart>()
                .HasOne(e => e.User)
                .WithOne(e => e.Cart)
                .HasForeignKey<User>();

            modelBuilder.Entity<CartItem>()
                .HasOne(e => e.Book)
                .WithOne(e => e.CartItem)
                .HasForeignKey<Book>();
        }
    }
}
