using System;
using System.Linq;
using Cimplicity.Views.Data.Repository;
using Cimplicity.Views.Infrastructure.Configuration;
using Magnum.Extensions;
using Magnum.Reflection;
using Magnum.TypeScanning;

namespace Cimplicity.Views.Data.Context
{
    public class RepositoryFactory
    {

        private static RepositoryFactory _instance;

        private readonly Type[] _repositoryTypes;

        public RepositoryFactory(ICimplicityViewsConfiguration configuration)
        {

            _repositoryTypes = TypeScanner.Scan(cfg =>
            {
                var baseType = Type.GetType(configuration.RepositoryAssemblyName);
                cfg.AssemblyContainingType(baseType);
                cfg.AddAllTypesOf<IRepository>();
                
            });
        }

        public static RepositoryFactory Instance => _instance ?? (_instance = new RepositoryFactory(ConfigurationFactory.Configuration));

        public TRepository Create<TRepository>() where TRepository : class
        {
            var repositoryType = _repositoryTypes.FirstOrDefault(type => type.Implements(typeof(TRepository)));
            return FastActivator.Create(repositoryType) as TRepository;
        }
    }
}