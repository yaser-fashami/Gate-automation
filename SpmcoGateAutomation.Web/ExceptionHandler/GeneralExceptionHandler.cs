using System.Net;


namespace SpmcoGateAutomation.Web.ExceptionHandler
{
    public class GeneralExceptionHandler : ExceptionHandlerBase
    {
        public override bool CanHandle(Exception ex)
        {
            return ex is Exception;
        }

        protected override void CustomManipulateResponse(ApiError apiError, Exception ex)
        {
            apiError.Status = (short)HttpStatusCode.InternalServerError;
            apiError.Title = "هنگام پردازش درخواست خطایی اتفاق افتاد. لطفا با شناسه دریافتی " +
               "جهت بررسی علت بروز مشکل ببا پشتیبانی سیستم تماس حاصل نمایید ";
        }
    }
}
