using AudioMerger___Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace AudioMerger___Backend.DataBase
{
    public class DataBaseModel : DbContext
    {
        public DataBaseModel(DbContextOptions<DataBaseModel> options) : base(options)
        {
        }

        public DbSet<UserDbModel> Usuarios { get; set; }
    }
}
