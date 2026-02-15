using System.Net;
using System.Text.Json;
using Singularity.Models;

namespace Singularity.Middlewares;

public class ExceptionMiddleware{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next){
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context){
        try{
            await _next(context);
        }
        catch (Exception ex){
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception){
        context.Response.ContentType = "application/json";
        
        var statusCode = exception switch{
            AppException appEx => appEx.StatusCode,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var response = new{
            status = statusCode,
            message = exception.Message,
            detail = exception.InnerException?.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}