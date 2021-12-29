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

namespace Data.BookRepo
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {

        public BookRepository(BookStoreDbContext context, ILogger logger) : base(context, logger)
        {

        }


        public async Task<bool> Add(Book entity)
        {
            entity.PublishDate = DateTime.Now.AddMonths(-30);
            var sql = "Insert into Book (Title,GenreId,PageCount,PublishDate, AuthorId) VALUES (@Title,@GenreId,@PageCount,@PublishDate,@AuthorId)";
            using (var connection = new SqlConnection("Server=LAPTOP-7JP2QLB1\\MSSQLSERVER2; Database=Patika2; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return true;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var sql = "DELETE FROM Book WHERE Id = @Id";
            using (var connection = new SqlConnection("Server=LAPTOP-7JP2QLB1\\MSSQLSERVER2; Database=Patika2; Trusted_Connection=True;"))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return true;
            }
        }

        public Task<IEnumerable<Book>> GetAll()
        {
            return base.GetAll();
        }



        public Task<bool> Update(Book entity)
        {
            throw new NotImplementedException();
        }
    }
}
