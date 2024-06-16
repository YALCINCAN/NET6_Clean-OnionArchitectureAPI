using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

public class ExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResponse, new()
    where TException : Exception
{
    private readonly ILogger<ExceptionHandler<TRequest,TResponse,TException>> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        _logger.LogError($"{typeof(TRequest).Name}" + "Exception = {@Exception}", exception);
        var response = new ErrorResponse(500, "Internal Server Error");
        _logger.LogInformation($"{typeof(TRequest).Name}" + "Exception Response = {@Response}", response);
        state.SetHandled(response as TResponse);
        return Task.CompletedTask;
    }
}