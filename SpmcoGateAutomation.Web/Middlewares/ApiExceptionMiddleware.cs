using Newtonsoft.Json;
using SpmcoGateAutomation.Web.ExceptionHandler;

namespace SpmcoGateAutomation.Web.Middlewares
{
    public class ApiExceptionMiddleware : IMiddleware
    {
        private readonly List<IExceptionHandler> _exceptionHandler = new List<IExceptionHandler>();

        public ApiExceptionMiddleware()
        {
            _exceptionHandler.Add(new KeyNotFoundExceptionHandler());
            _exceptionHandler.Add(new BadRequestExceptionHandler());
            _exceptionHandler.Add(new GeneralExceptionHandler()); // latest record
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            finally
            {

            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.ContentType = "application/json";
            var exceptionhandler = _exceptionHandler.FirstOrDefault(c => c.CanHandle(exception));
            if (exceptionhandler == null)
            {
                exceptionhandler = new GeneralExceptionHandler();
            }
            ApiError errorResponse;
            errorResponse = exceptionhandler.Handle(exception);
            context.Response.StatusCode = errorResponse.Status;
            var result = JsonConvert.SerializeObject(errorResponse);
           // _busControl.SendAsync("logger.queue.hediak", result);
            return context.Response.WriteAsync(result);
        }
        private string GetInnermostExceptionMessage(Exception exception)
        {
            if (exception.InnerException != null)
                return GetInnermostExceptionMessage(exception.InnerException);

            return exception.Message;
        }
    }
}
