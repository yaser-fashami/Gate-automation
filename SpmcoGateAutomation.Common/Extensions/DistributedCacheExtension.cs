using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SpmcoGateAutomation.Common.Extensions
{
    public static class DistributedCacheExtension
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recodeId, T data)
        {
            var options = new DistributedCacheEntryOptions();
            //options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(36000);
            //options.SlidingExpiration = slidingExpirationTime;
            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recodeId, data.ToString());
        }
        public static async Task<string> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);
            if (jsonData is null)
            {
                return "";
            }
            return jsonData.ToString();
        }
    }
}
