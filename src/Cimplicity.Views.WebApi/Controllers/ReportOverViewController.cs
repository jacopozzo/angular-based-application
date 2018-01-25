using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Cimplicity.Views.Infrastructure.Mapping;
using Cimplicity.Views.WebApi.Models;
using Utils.Data.DatabaseClient;
using Utils.Data.DatabaseClient.Abstractions;
using Utils.Extensions.Data;

namespace Cimplicity.Views.WebApi.Controllers
{
    public class ReportOverViewController
        : ApiController
    {
        private readonly string _connectionString;


        public ReportOverViewController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["soadb"].ConnectionString;
        }


        // GET api/<controller>
        public IEnumerable<ReportOverviewViewModel> Get(string area)
        {
            var list = new List<ReportOverviewViewModel>();

            IQueryOperations storageManager = StorageManagerFactory.CreateDatabaseManager(_connectionString);
            var set = storageManager.ExecuteCommand("sp_VCC_local_reportOverview", new[] {new SqlParameter("@workArea", area)});
            if (set.IsEmpty())
            {
                return list;
            }

            var rows = set.Tables[0].Rows.Cast<DataRow>();
            return rows.MapTo<IEnumerable<ReportOverviewViewModel>>();
            
        }

        
    }
}