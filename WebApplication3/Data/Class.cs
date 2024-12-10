namespace WebApplication3.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Xml;
    using WebApplication3.Model;

    public class MyDbContext : DbContext
    {
        // Define DbSets for each table
        public DbSet<Test> Test { get; set; }
      

        // Constructor accepting options, required by EF Core
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    }

}
