using DevHabit.Api.Database;
using DevHabit.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DevHabit.Api.Services;

public sealed class UserContext(
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext dbContext,
    IMemoryCache memoryCache)
{
    private const string CacheKeyPrefix = "user:id";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<string?> GetUserAsync(CancellationToken cancellationToken = default)
    {
        // This our extension method
        // we used httpContextAccessor to get the current HttpContext cuz  HttpContext cannot be accessed directly outside controller or middleware
        var identityId = httpContextAccessor.HttpContext?.User.GetIdentityId();
        if (string.IsNullOrEmpty(identityId)) return null;
        var cacheKey = $"{CacheKeyPrefix}{identityId}";
        //caching the userId
        var userId = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(CacheDuration);
            var userId = await dbContext.Users
                .Where(u => u.IdentityId == identityId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
            return userId;
        });
        return userId;
    }
}