using Cimplicity.Views.Infrastructure.Configuration;

namespace Cimplicity.Views.Data.Sql.Repository
{
    public class RepositoryBase
    {
        public string ConnectionString { get; set; }

        public RepositoryBase(ICimplicityViewsConfiguration configuration)
        {
            
        }
    }
}