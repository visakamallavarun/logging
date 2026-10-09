namespace MiniLogging.Demo;

/// <summary>
/// Application code. It only depends on ILogger&lt;EmployeeService&gt; and never
/// knows which providers (console, file, debug) actually receive the events.
/// </summary>
public sealed class EmployeeService
{
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(ILogger<EmployeeService> logger)
    {
        _logger = logger;
    }

    public void CreateEmployee(int employeeId)
    {
        // Visible only because a category filter lowers this type to Debug.
        _logger.LogDebug("Starting employee creation");

        _logger.LogInformation("Employee {EmployeeId} created", employeeId);
    }

    public void ProcessEmployee(int employeeId)
    {
        using (_logger.BeginScope("Processing EmployeeId: {EmployeeId}", employeeId))
        {
            _logger.LogInformation("Loading employee");
            _logger.LogInformation("Validating employee");
            _logger.LogWarning("Employee {EmployeeId} is missing an email address", employeeId);
            _logger.LogInformation("Saving employee");
        }
    }
}
