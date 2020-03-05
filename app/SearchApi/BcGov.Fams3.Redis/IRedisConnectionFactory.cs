using Microsoft.Extensions.Logging;
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
        private ILogger<RedisConnectionFactory> _logger;

        public RedisConnectionFactory(string redisConnectionStr, ILogger<RedisConnectionFactory> logger)
        {
            _connection = null;
            this._redisConnectionStr = redisConnectionStr;
            this._logger = logger;
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
                _logger.LogError(redisExp.Message);
                throw redisExp;
            }catch(Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
        }

        public IDatabase GetDatabase()
        {
            if (_connection == null) OpenConnection();
            return _connection.Value.GetDatabase();
        }
    }
}
