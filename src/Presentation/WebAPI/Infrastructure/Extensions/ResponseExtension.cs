using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace WebAPI.Infrastructure.Extensions
{
    public static class ResponseExtension
    {
        public static ActionResult FromResponse<T>(this ControllerBase controller, IResponse response)
        {
            switch (response.StatusCode)
            {
                case StatusCodes.Status200OK:
                    if (response is IDataResponse<T> dataResponse)
                        return controller.Ok(dataResponse);
                    else if (response is IPagedDataResponse<T> pagedDataResponse)
                        return controller.Ok(pagedDataResponse);
                    else if (response is ISuccessResponse successResponse)
                        return controller.Ok(successResponse);
                    else
                    {
                        return controller.Ok();
                    }
                case StatusCodes.Status404NotFound:
                    if (response is IErrorResponse errorResponse)
                        return controller.NotFound(errorResponse);
                    else
                    {
                        return controller.NotFound();
                    }
                case StatusCodes.Status400BadRequest:
                    if (response is IErrorResponse errorResponseBadRequest)
                        return controller.BadRequest(errorResponseBadRequest);
                    else
                    {
                        return controller.BadRequest();
                    }
                default:
                    throw new Exception("Internal Server Error");
            }
        }
    }
}
