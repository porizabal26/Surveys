using Microsoft.EntityFrameworkCore;
using Surveys.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Surveys.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Usr> Usr { get; set; }
        public DbSet<Survey> Survey { get; set;}
        public DbSet<SurveyDetail> SurveyDetail { get; set; }
        public DbSet<DataType> DataType { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<SurveyDetailResponse> SurveyDetailResponse { get; set; }
    }
}
