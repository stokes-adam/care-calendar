using System.Reflection;
using DbUp;

// switch statement
var connectionString = args.Length switch
{
    1 => args.FirstOrDefault(),
    > 1 => $"Server=localhost;Port=5432;Database=care_calendar;User Id={args[0]};Password={args[1]};",
    _ => "Server=localhost;Port=5432;Database=care_calendar;User Id=postgres;Password=password;"
};

var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif                
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;