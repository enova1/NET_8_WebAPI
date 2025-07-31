using System.Reflection;

namespace WebApi.Middleware;

/// <summary>
/// Include only valid properties for swagger schema definition.
/// </summary>
public class IncludePropertiesSchemaFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Only check the shared object requests and specifically the Auth Security Request.
        if (!context.Type.FullName!.Contains("Request") || context.Type.FullName.Contains("SecurityRequest")) return;
        if (schema.Properties == null) return;
        // Create a list of Properties to remove from the Request Body Swagger Schema
        var excludedProperties = context.Type.GetProperties()
            .Where(p => !IsIncludedProperty(p))
            .Select(p => char.ToLower(p.Name[0]) + p.Name.Substring(1)) // Ensure the first letter is lowercase
            .ToList();
        // Create a list of Properties to Remove from the Schema (minus the excluded)
        var keysToRemove = schema.Properties.Keys
            .Where(key => excludedProperties.Contains(key, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            schema.Properties.Remove(key);
        }
    }
    /// <summary>
    /// These Properties are to be included in the Swagger Request Body Schema.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private static bool IsIncludedProperty(PropertyInfo property)
    {
        return property.Name.Contains("DTO", StringComparison.OrdinalIgnoreCase) ||
               property.Name.Equals("Company", StringComparison.OrdinalIgnoreCase) ||
               property.Name.Equals("Division", StringComparison.OrdinalIgnoreCase);
    }
}