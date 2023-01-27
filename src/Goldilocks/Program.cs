using System.Text.Json.Serialization;
using Goldilocks;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders(); // Clearing for benchmark scenario, template has AddConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddContext<AppJsonSerializerContext>();
});

var app = builder.Build();

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => Todos.AllTodos);

// Keeping because it is in the template but not actually benchmarked.
todosApi.MapGet("/{id}", (int id) =>
    Todos.AllTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.Lifetime.ApplicationStarted.Register(() => Console.WriteLine("Application started. Press Ctrl+C to shut down."));
app.Run();

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
