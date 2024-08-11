
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace FourthPro.Middleware;

public class ErrorHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            context.Request.EnableBuffering();
            await next(context);
        }
        catch (Exception error)
        {
            object lastError = null;
            Console.WriteLine(error);
            var response = context.Response;
            response.ContentType = "application/json";
            CommonResponseDto<object> customResponse = new()
            {
                ErrorMessage = error.Message,
                ErrorMessageDetails = error.Data["custom_message"]
            };
            switch (error)
            {
                case AccessViolationException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                //case SecurityTokenValidationException:
                //    response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    customResponse.ErrorMessage = sharedLocalizer[ErrorMessages.TokenStillWork];
                //    break;
                case InValidExtensionException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    customResponse.ErrorMessage = "Un Supported File";
                    break;
                case AlreadyExistException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    customResponse.ErrorMessage = "Can Not Access";
                    break;
                case InValidSizeException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    customResponse.ErrorMessage = "Invalid Size File";
                    break;
                case ValidationException:
                    // validation application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    customResponse.ErrorMessage = "Internal Server Error";
                    var splitStackTrace = error.StackTrace.Trim().Split("at").Where(s => !string.IsNullOrEmpty(s));
                    StringBuilder errorMessageBuilder = new($"Exception Info: \n Message: {error.Message} \n\n Stack Trace: ");
                    for (int i = 0; i < splitStackTrace.Count(); i++)
                    {
                        if (i == 2)
                            break;
                        errorMessageBuilder.Append($"{splitStackTrace.ElementAt(i)} \n");
                    }
                    var errorMessage = errorMessageBuilder.Append($"Reason: {error.Source}").ToString();
                    break;
            }
            await response.WriteAsJsonAsync(customResponse);
        }
    }
}
