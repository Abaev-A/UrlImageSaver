using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Data;
using UrlImageSaver.Application.Models;

namespace UrlImageSaver.Application.Storage
{
    class PostgreStorage : IStorage
    {
        #region Fields
        private readonly PostgreStorageOptions _options;
        private readonly ILogger<PostgreStorage> _logger;
        #endregion

        #region ctor
        public PostgreStorage(IOptions<PostgreStorageOptions> dbStoreOptions, ILoggerFactory loggerFactory)
        {
            _options = dbStoreOptions.Value;
            _logger = loggerFactory.CreateLogger<PostgreStorage>();
        }
        #endregion

        #region IStorage
        public void Store(string name, byte[] content)
        {
            try
            {
                if (_options.UseProcedure)
                    ExecuteProcedure(name, content);
                else
                    SendQuery(name, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during database storing");
            }
        }
        #endregion

        #region private
        private void SendQuery(string name, byte[] content)
        {
            string sql = $"INSERT INTO {_options.TableName} (name, content) VALUES (\'{name}\', @Content)";

            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    NpgsqlParameter param = command.CreateParameter();
                    param.ParameterName = "@Content";
                    param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                    param.Value = content;
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();

                    _logger.LogDebug($"File stored successfully {name}");
                }
            }
        }

        private void ExecuteProcedure(string name, byte[] content)
        {
            string sql = _options.ProcedureName;

            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue(":p_name", name);
                    command.Parameters.AddWithValue(":p_content", content);
                    command.ExecuteNonQuery();

                    _logger.LogDebug($"File stored successfully {name}");
                }
            }
        }
        #endregion
    }
}
