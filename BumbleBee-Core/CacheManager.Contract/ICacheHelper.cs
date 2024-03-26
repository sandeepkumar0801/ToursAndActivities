using StackExchange.Redis;

namespace CacheManager.Contract
{
	public interface ICacheHelper
	{
		string GetCacheData(string key);

		IServer GetServer();

		void SetCacheData(string key, string data);

		void LoadCacheData(string freqType);

		void DeleteCacheKeys(RedisKey[] keys);
	}
}
