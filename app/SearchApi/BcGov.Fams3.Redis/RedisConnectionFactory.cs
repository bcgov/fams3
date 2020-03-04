using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.Redis
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer OpenConnection();
        IDatabase GetDatabase();
    }

    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private static Lazy<ConnectionMultiplexer> _connection;
        private string _redisConnectionStr;

        public RedisConnectionFactory(string redisConnectionStr)
        {
            _connection = null;
            this._redisConnectionStr = redisConnectionStr;
        }

        public ConnectionMultiplexer OpenConnection()
        {
            try
            {
                if (_connection == null)
                {
                    var options = ConfigurationOptions.Parse(_redisConnectionStr);
                    _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
                }
                return _connection.Value;
            }catch(RedisException redisExp)
            {
                throw redisExp;
            }catch(Exception e)
            {
                throw e;
            }
        }

        public IDatabase GetDatabase()
        {
            return _connection.Value.GetDatabase();
        }
    }
}
