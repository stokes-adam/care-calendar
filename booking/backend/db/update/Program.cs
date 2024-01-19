using update;

// this is the entry point for the migration lambda
Console.WriteLine("Starting migration");
new Migration().FunctionHandler();