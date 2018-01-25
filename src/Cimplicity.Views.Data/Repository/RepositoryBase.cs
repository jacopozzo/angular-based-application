using Cimplicity.Views.Data.Context;

namespace Cimplicity.Views.Data.Repository
{
    public abstract class RepositoryBase
    {

        protected IDataContext Context { get; set; }
    }
}