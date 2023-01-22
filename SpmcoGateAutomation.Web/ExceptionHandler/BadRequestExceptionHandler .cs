using System.Net;

namespace SpmcoGateAutomation.Web.ExceptionHandler
{
    public class BadRequestExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception ex)
        {
            return ex is ApplicationException;
        }

        public ApiError Handle(Exception ex)
        {
            return new ApiError
            {
                Id = Guid.NewGuid().ToString(),
                Status = (short)HttpStatusCode.BadRequest,
                Detail = ex.Message
            };
            
        }
    }
}
