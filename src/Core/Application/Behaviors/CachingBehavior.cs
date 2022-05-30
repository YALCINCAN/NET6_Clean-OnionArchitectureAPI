using Application.Interfaces;
using Application.Interfaces.Services;
using MediatR;
using Serilog;

namespace Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEasyCacheService _easyCacheService;

        public CachingBehavior(IEasyCacheService easyCacheService)
        {
            _easyCacheService = easyCacheService;
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
                    Log.Information($"Fetched from Cache -> '{cacheableQuery.CacheKey}'.");
                }
                else
                {
                    response = await GetResponseAndAddToCache();
                    Log.Information($"Added to Cache -> '{cacheableQuery.CacheKey}'.");
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