using System.Net;

namespace SpmcoGateAutomation.Web.ExceptionHandler
{
    public class KeyNotFoundExceptionHandler : ExceptionHandlerBase
    {

        public override bool CanHandle(Exception ex)
        {
            return ex is KeyNotFoundException;
        }

        protected override void CustomManipulateResponse(ApiError apiError, Exception ex)
        {
            apiError.Status = (short)HttpStatusCode.NotFound;
            apiError.Detail = ex.Message;
        }
    }
}
