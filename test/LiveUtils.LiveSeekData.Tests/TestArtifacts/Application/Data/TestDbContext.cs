using LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace LiveUtils.LiveSeekData.Tests.TestArtifacts.Application.Data
{
    public class TestDbContext:DbContext
    {
        public DbSet<Person> Persons { get; set; }
    
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
            
        }
    }
}