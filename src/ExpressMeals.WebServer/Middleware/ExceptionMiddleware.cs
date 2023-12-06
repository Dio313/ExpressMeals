using System.Net;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.Domains.Exceptions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace ExpressMeals.WebServer.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = null;
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            try
            {
                await _next(context);
            }

            catch (BaseApiException exception)
            {

                httpStatusCode = exception.HttpStatusCode;

                // Generate message
                message = exception.Message;

                // Write to response
                if (exception is BadRequestException)
                {
                    await WriteToResponseAsync(exception.AdditionalData);
                }
                else
                {
                    await WriteToResponseAsync();
                }
            }

            catch (Exception exception)
            {

               await WriteToResponseAsync(exception);
            }

            // Local function
            async Task WriteToResponseAsync(object data = null)
            {
                if (context.Response.HasStarted)
                {
                    throw new InvalidOperationException(
                        "The response has already started, the http status code middleware will not be executed.");
                }

                string json;

                if (data is not null)
                {
                    json = JsonConvert.SerializeObject(new ApiResponse<object>(false, new List<string> {message}, data),
                        jsonSerializerSettings);
                }
                else
                {
                    json = JsonConvert.SerializeObject(new ApiResponse(false, new List<string> {message}), jsonSerializerSettings);
                }

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
        }
    }
}