using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEasyCacheService _easyCacheService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(IEasyCacheService easyCacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _easyCacheService = easyCacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICacheable cacheableQuery)
            {
                TResponse response;
                if (cacheableQuery.BypassCache) return await next();
                async Task<TResponse> GetResponseAndAddToCache()
                {
                    response = await next();

                    await _easyCacheService.SetAsync<TResponse>((string)cacheableQuery.CacheKey, response);
                    return response;
                }
                var cachedResponse = await _easyCacheService.GetAsync(cacheableQuery.CacheKey, typeof(TResponse));
                if (cachedResponse != null)
                {
                    response = (TResponse)cachedResponse;
                    _logger.LogInformation($"Fetched from Cache -> '{cacheableQuery.CacheKey}'.");
                }
                else
                {
                    response = await GetResponseAndAddToCache();
                    _logger.LogInformation($"Added to Cache -> '{cacheableQuery.CacheKey}'.");
                }
                return response;
            }
            else
            {
                return await next();
            }
        }
    }
}