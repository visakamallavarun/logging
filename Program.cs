using MiniLogging;
using MiniLogging.Demo;
using MiniLogging.DependencyInjection;

// ---------------------------------------------------------------------------
// SETUP TIME: register services, providers (destinations) and filtering rules.
// ---------------------------------------------------------------------------
var services = new ServiceCollection();

services.AddLogging(logging =>
{
    logging.ClearProviders();                 // remove any default providers
    logging.AddConsole();                     // destination 1: console
    logging.AddFile("logs/app.log");          // destination 2: file
    logging.AddDebug();                       // destination 3: debugger output
    logging.SetMinimumLevel(LogLevel.Information);

    // Category-specific rule: this one type logs down to Debug.
    logging.AddFilter("MiniLogging.Demo.EmployeeService", LogLevel.Debug);
});

services.AddTransient(typeof(EmployeeService));

using var provider = services.BuildServiceProvider();

// ---------------------------------------------------------------------------
// LOGGING TIME: resolve a service; DI injects ILogger<EmployeeService>.
// ---------------------------------------------------------------------------
var employeeService = provider.GetRequiredService<EmployeeService>();

employeeService.CreateEmployee(101);
employeeService.ProcessEmployee(202);

Console.WriteLine();
Console.WriteLine("Done. File destination written to: logs/app.log");
// Disposing the provider disposes the factory -> providers (flushes the file).
