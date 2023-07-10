using update;

// this is the entry point for the migration lambda
// expects connection string in the environment variable "ConnectionString"
// "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;";
new Migration().FunctionHandler();