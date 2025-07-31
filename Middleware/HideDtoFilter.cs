using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace WebApi.Middleware;


/// <summary>
/// Hide the DTO Object from the Swagger Request Query Schema
/// </summary>
public class HideDtoFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the HTTP method is GET
        if (!context.ApiDescription.HttpMethod!.Equals("GET", StringComparison.OrdinalIgnoreCase)) return;
        // Get the parameter descriptions
        var parameterDescriptions = context.ApiDescription.ParameterDescriptions;

        foreach (var parameterDescription in parameterDescriptions)
        {
            if (parameterDescription.Source != BindingSource.Query) continue;
            // Check if the parameter's type is a complex type
            if (!RemoveType(parameterDescription)) continue;
            // Remove the parameter from the operation's parameters
            var parameterToRemove = operation.Parameters
                .FirstOrDefault(p => p.Name == parameterDescription.Name);

            if (parameterToRemove != null)
            {
                operation.Parameters.Remove(parameterToRemove);
            }
        }
    }

    private static bool RemoveType(ApiParameterDescription parameterType)
    {
        // Check if the type is a class (complex type) and its name contains "DTO"
        if (parameterType.Name.Contains("DTO.", StringComparison.OrdinalIgnoreCase) ||
            parameterType.Name.Contains("AccessDenied", StringComparison.OrdinalIgnoreCase) ||
            parameterType.Name.Contains("Station", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}