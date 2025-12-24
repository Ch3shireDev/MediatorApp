using Microsoft.EntityFrameworkCore;

namespace MediatorTest.Tests.Integration;

public static class DbContextBuilder
{
    public static T CreateDbContext<T>() where T : DbContext
    {
        var options = new DbContextOptionsBuilder<T>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return (T)Activator.CreateInstance(typeof(T), options)!;
    }
}