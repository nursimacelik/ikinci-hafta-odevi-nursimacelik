using Dapper;
using Data.Context;
using Data.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.VehicleRepo
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {

        public VehicleRepository(WasteCollectionDbContext context, ILogger logger) : base(context, logger)
        {

        }


        public async Task<bool> Add(Vehicle entity)
        {
            var sql = "Insert into Vehicle (VehicleName, VehiclePlate) VALUES (@VehicleName, @VehiclePlate)";
            using (var connection = new SqlConnection("Server=LAPTOP-7JP2QLB1\\MSSQLSERVER2; Database=Patika2; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return true;
            }
        }

        public async Task<bool> Delete(long id)
        {
            var sql = "DELETE FROM Vehicle WHERE Id = @Id";
            using (var connection = new SqlConnection("Server=LAPTOP-7JP2QLB1\\MSSQLSERVER2; Database=Patika2; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return true;
            }
        }

        public Task<IEnumerable<Vehicle>> GetAll()
        {
            return base.GetAll();
        }

        public async Task<IEnumerable<Container>> GetContainers(long id)
        {
            var sql = "SELECT Container.Id, ContainerName, Latitude, Longitude, VehicleId FROM Vehicle, Container WHERE Vehicle.Id = Container.VehicleId AND Vehicle.Id = @Id";
            using (var connection = new SqlConnection("Server=LAPTOP-7JP2QLB1\\MSSQLSERVER2; Database=Patika2; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.QueryAsync<Container>(sql, new { Id = id });
                return result;
            }
        }


        public Task<bool> Update(Vehicle entity)
        {
            return base.Update(entity);
        }

        public async Task<Vehicle> GetById(long id)
        {
            var model = await dbSet.FindAsync(id);
            return model;
        }
    }
}
