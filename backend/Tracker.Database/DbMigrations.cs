using DbUp;

namespace Tracker.Database;
public static class DbMigrations
{
    public static void Initialize(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);
        var names = typeof(DbMigrations).Assembly.GetManifestResourceNames();

        foreach (var name in names.OrderBy(x => x))
        {
            Console.WriteLine(name);
        }
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DbMigrations).Assembly)
            .LogToConsole()
            .Build();

        if (!upgrader.IsUpgradeRequired())
        {
            return;
        }

        var result = upgrader.PerformUpgrade();
        
        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            throw new Exception("Database migration failed");
        }
    }
}
