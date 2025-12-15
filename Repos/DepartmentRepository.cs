using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MinimalAPIs.Repos
{
    public class DepartmentRepository : IDepartmentRepository
    {
         private readonly string _connStr;  

        public DepartmentRepository(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("Default");
        }
        private IDbConnection CreateConnection() => new SqlConnection(_connStr);
        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = CreateConnection();
            var sql = "DELETE FROM Departments WHERE Id = @Id";
            var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });
            return affectedRows> 0; 
        }


        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            using var conn = CreateConnection();
            var sql = "SELECT * FROM departments";
            return await conn.QueryAsync<Department>(sql);
        }
        public async Task<Department?> GetByIdAsync(int id)
        {
            using var conn = CreateConnection();
            var sql = "SELECT Name, Location   FROM departments WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Department>(sql, new { Id = id });
        }

        public async Task<Department> InsertAsync(Department department)
        {
            using var conn = CreateConnection();
            var sql = @"INSERT INTO Departments (Name, Location) 
                    VALUES (@Name, @Location); 
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            var newId = await conn.ExecuteScalarAsync<int>(sql, new { department.Name, department.Location });
            department.Id = newId;
            return department;
        }

        public async Task<bool> UpdateAsync(int id, Department department)
        {
          using var conn = CreateConnection();  
            var sql = @"UPDATE Departments 
                        SET Name = @Name, Location = @Location 
                        WHERE Id = @Id";
            
            var affectedRows = await conn.ExecuteAsync(sql, new { department.Name, department.Location, Id = id });
            return affectedRows > 0;
        }

    }
}
