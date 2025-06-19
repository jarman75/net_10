#:package Marten@8.*

using Marten;
using Marten.Linq.SoftDeletes;

Console.WriteLine("Hello, Marten!");

//database connection string
var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

// Create a DocumentStore
var store = DocumentStore.For(_ =>
{
    _.Connection(connectionString);
    _.Schema.For<User>().SoftDeleted();
});

//await CreateUser();

//await DeleteUser();

await RecoverUser();

#region User
async Task CreateUser()
{
    using var session = store.LightweightSession();
    var user = new User(Guid.NewGuid(), "John Doe", "john.doe@example.com");
    session.Store(user);
    await session.SaveChangesAsync();
    Console.WriteLine("User created successfully.");
}

async Task DeleteUser()
{
    using var session = store.LightweightSession();
    session.DeleteWhere<User>(u => u.Name == "John Doe");
    await session.SaveChangesAsync();
    Console.WriteLine("User deleted successfully.");
}
//recover soft-deleted user
async Task RecoverUser()
{
    using var session = store.LightweightSession();
    //session.UndoDeleteWhere<User>(u => u.Name == "John Doe");
    var user = await session.LoadAsync<User>(Guid.Parse("7d778d57-879b-47d3-8679-a9d46c2f5773"));
    user = new User(user!.Id, user.Name, "john.doe@update.com");
    

    await session.SaveChangesAsync();
    Console.WriteLine("User recovered successfully.");
    
}

public record User(Guid Id, string Name, string? Email);
#endregion

