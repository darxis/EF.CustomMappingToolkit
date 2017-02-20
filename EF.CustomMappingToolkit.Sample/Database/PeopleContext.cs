using EF.CustomMappingToolkit.Sample.Model;
using System.Data.Entity;

namespace EF.CustomMappingToolkit.Sample.Database
{
    public class PeopleContext : DbContext
    {
        public DbSet<Person> People { get; set; }
    }
}
