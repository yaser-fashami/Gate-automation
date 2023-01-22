namespace SpmcoGateAutomation.Web.ExceptionHandler
{
    public interface IExceptionHandler
    {

        bool CanHandle(Exception ex);
        ApiError Handle(Exception ex);
    }

    public abstract class ExceptionHandlerBase : IExceptionHandler
    {
        public abstract bool CanHandle(Exception ex);

        public virtual ApiError Handle(Exception ex)
        {
            var apiError = new ApiError
            {
                Id = Guid.NewGuid().ToString()
            };
            CustomManipulateResponse(apiError, ex);
            return apiError;
        }

        protected abstract void CustomManipulateResponse(ApiError apiError, Exception ex);
    }
}
