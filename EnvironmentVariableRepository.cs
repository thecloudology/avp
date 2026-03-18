using Dapper;
using Microsoft.Data.SqlClient;
using CSOTeamApp.Models;

namespace CSOTeamApp.Repositories
{
    public class EnvironmentVariableRepository
    {
        private readonly string _connectionString;

        public EnvironmentVariableRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<EnvironmentVariable>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<EnvironmentVariable>(
                @"SELECT e.Id, e.ProcessId, e.ElementName, e.ElementTypeId, 
                         t.TypeName, e.LastUpdated
                  FROM rpa_Environment_Variables e
                  INNER JOIN rpa_Environment_Variable_Types t ON e.ElementTypeId = t.Id");
        }

        public async Task<IEnumerable<EnvironmentVariable>> GetByProcessIdAsync(string processId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<EnvironmentVariable>(
                @"SELECT e.Id, e.ProcessId, e.ElementName, e.ElementTypeId, 
                         t.TypeName, e.LastUpdated
                  FROM rpa_Environment_Variables e
                  INNER JOIN rpa_Environment_Variable_Types t ON e.ElementTypeId = t.Id
                  WHERE e.ProcessId = @ProcessId",
                new { ProcessId = processId });
        }

        public async Task<EnvironmentVariable> GetByElementNameAsync(string processId, string elementName)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<EnvironmentVariable>(
                @"SELECT e.Id, e.ProcessId, e.ElementName, e.ElementTypeId, 
                         t.TypeName, e.LastUpdated
                  FROM rpa_Environment_Variables e
                  INNER JOIN rpa_Environment_Variable_Types t ON e.ElementTypeId = t.Id
                  WHERE e.ProcessId = @ProcessId AND e.ElementName = @ElementName",
                new { ProcessId = processId, ElementName = elementName });
        }

        public async Task<IEnumerable<EnvironmentVariableType>> GetTypesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<EnvironmentVariableType>(
                "SELECT Id, TypeName FROM rpa_Environment_Variable_Types");
        }

        public async Task<int> AddAsync(EnvironmentVariable envVar)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                @"INSERT INTO rpa_Environment_Variables (ProcessId, ElementName, ElementTypeId)
                  VALUES (@ProcessId, @ElementName, @ElementTypeId)",
                envVar);
        }

        public async Task<int> UpdateAsync(EnvironmentVariable envVar)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                @"UPDATE rpa_Environment_Variables 
                  SET ElementName = @ElementName, ElementTypeId = @ElementTypeId
                  WHERE Id = @Id AND ProcessId = @ProcessId",
                envVar);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                "DELETE FROM rpa_Environment_Variables WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<bool> ExistsAsync(string processId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM rpa_Environment_Variables WHERE ProcessId = @ProcessId",
                new { ProcessId = processId });
        }
    }
}
