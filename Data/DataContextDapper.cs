using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DOTNETAPI.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }
        public IEnumerable<T> LoadData<T>(string sql, object? parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql, parameters);
        }

        public T LoadDataSingle<T>(string sql, object? parameters = null)
        {
            Console.WriteLine(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql, parameters);
        }

        public bool ExecuteSql(string sql, object? parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql, parameters) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql, object? parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql, parameters);
        }

    }
}



























































































































































































































































