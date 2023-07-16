using update;

// this is the entry point for the migration lambda
// expects connection string in the environment variable "ConnectionString"
// "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres;";
new Migration().FunctionHandler();