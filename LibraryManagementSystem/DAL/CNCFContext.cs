using LibraryManagementSystem.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LibraryManagementSystem.DAL
{
    public class CNCFContext : DbContext
    {
        //The name of the connection string (which you'll add to the Web.config file later) 
        //is passed in to the constructor.
        //If you don't specify a connection string or the name of one explicitly
        //Entity Framework assumes that the connection string name is the same as the class name.

        public CNCFContext() : base("CNCFConnection")
        {
        }

        // creates a DbSet property for each entity set
        // entity set typically corresponds to a database table
        // and an entity corresponds to a row in the table.
        public DbSet<HocSinh> HocSinh { get; set; }
        public DbSet<ChuDe> ChuDe { get; set; }
        public DbSet<MuonTraSach> MuonTraSach { get; set; }
        public DbSet<Sach> Sach { get; set; }
        public DbSet<ThanhLy>ThanhLy { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // modelBuilder.Conventions.Remove prevents table names from being pluralized (diễn tả ở số nhiều)
            // If you didn't do this, the generated tables in the database
            // would be named Students, Courses, and Enrollments
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}