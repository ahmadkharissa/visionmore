using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Visionmore.Data {
    public class DataContext : IdentityDbContext<User> {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) {
        }

        public DbSet<Product> Products { get; set; }
    }
}