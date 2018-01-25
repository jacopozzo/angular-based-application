using System.Collections.Generic;
using Cimplicity.Views.Domain.Model;

namespace Cimplicity.Views.Data.Repository
{
    public interface IReportRepository : IRepository
    {
        IEnumerable<ReportOverview> GetReportOverview(string area,int page = 1, int pageSize = 20);

        IEnumerable<ReportOverview> GetReportOverviews(string area, IEnumerable<string> productionLines,
            IEnumerable<string> workCells = null, IEnumerable<string> materials = null, int page = 1, int pageSize = 20);
    }
}