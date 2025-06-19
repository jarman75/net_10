#:package Marten@8.*

using Marten;
using Marten.Events;


// Program

Console.WriteLine("Event Sourcing with Marten Example!");

var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

// Configurar DocumentStore con soporte para eventos
var store = DocumentStore.For(_ =>
{
    _.Connection(connectionString);
    
    // Registrar los tipos de eventos
    _.Events.AddEventType<AccountCreated>();
    _.Events.AddEventType<MoneyDeposited>();
    _.Events.AddEventType<MoneyWithdrawn>();
    
    // Habilitar control de concurrencia optimista
    //_.Events.OptimisticConcurrency = true;
});

// Ejemplo de uso del Event Store
using var session = store.LightweightSession();
// Crear una nueva cuenta con saldo inicial
var accountId = Guid.NewGuid();
Console.WriteLine($"Creating account {accountId}");

var created = new AccountCreated(accountId, 1000m);
session.Events.Append(accountId, created);

// Realizar algunas transacciones
session.Events.Append(accountId, new MoneyDeposited(accountId, 500m, "Initial deposit"));
session.Events.Append(accountId, new MoneyWithdrawn(accountId, 200m, "ATM withdrawal"));

await session.SaveChangesAsync();

// Cargar la cuenta desde los eventos
var account = await session.Events.AggregateStreamAsync<BankAccount>(accountId);
Console.WriteLine($"\nCuenta reconstruida desde eventos:");
Console.WriteLine($"Balance actual: ${account?.Balance}"); // Debería mostrar $1300

// Obtener todos los eventos de la cuenta
var events = await session.Events.FetchStreamAsync(accountId);
Console.WriteLine("\nHistorial de eventos:");
foreach (var @event in events)
{
    Console.WriteLine($"- {@event.Data}");
}


#region Entities
public class BankAccount
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }

    public BankAccount()
    {
        Id = Guid.Empty;
        Balance = 0m;
    }

    public void Apply(AccountCreated @event)
    {
        Id = @event.AccountId;
        Balance = @event.InitialBalance;
    }

    public void Apply(MoneyDeposited @event)
    {
        if (@event.Amount <= 0)
            throw new InvalidOperationException("Deposit amount must be positive");
            
        Balance += @event.Amount;
    }

    public void Apply(MoneyWithdrawn @event)
    {
        if (@event.Amount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be positive");
            
        if (Balance < @event.Amount)
            throw new InvalidOperationException("Insufficient funds");
            
        Balance -= @event.Amount;
    }
}
#endregion

#region Event records
public record AccountCreated(Guid AccountId, decimal InitialBalance);
public record MoneyDeposited(Guid AccountId, decimal Amount, string? Description = null);
public record MoneyWithdrawn(Guid AccountId, decimal Amount, string? Description = null);
#endregion