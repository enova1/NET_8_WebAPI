namespace WebApi;

/// <summary>
/// Middleware for validating the model state.
/// </summary>
public class ModelStateValidationMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Override the InvokeAsync method.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request path starts with /v1
        if (context.Request.Path.StartsWithSegments("/v1")) // Adjust the path as needed
        {
            var endpoint = context.GetEndpoint();
            // Check if the endpoint has the ApiController attribute
            if (endpoint?.Metadata.GetMetadata<ApiControllerAttribute>() != null)
            {
                // Check if the model state is invalid
                if (context.Items["__MODEL_STATE"] is ModelStateDictionary { IsValid: false } modelState)
                {
                    // Get the errors from the model state
                    var errors = modelState.Values.SelectMany(v => v.Errors);
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new { response = errors });
                    return;
                }
            }
        }
        // Continue to the next middleware
        await next(context).ConfigureAwait(false);
    }
}