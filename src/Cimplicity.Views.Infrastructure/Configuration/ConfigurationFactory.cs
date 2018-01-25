using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Magnum.Extensions;
using Magnum.Reflection;
using Magnum.TypeScanning;

namespace Cimplicity.Views.Infrastructure.Configuration
{
    public class ConfigurationFactory
    {
        private readonly List<Type> _configurationTypes;
        private readonly string _configurationType;

        public ConfigurationFactory()
        {
            _configurationType = ConfigurationManager.AppSettings["ConfigurationType"];
            _configurationTypes = TypeScanner.Scan(cfg =>
            {
                cfg.AssemblyContainingType(typeof(ICimplicityViewsConfiguration));
                cfg.AddAllTypesOf<ICimplicityViewsConfiguration>();
            }).ToList();
        }

        public static ICimplicityViewsConfiguration Configuration => _configuration ?? (_configuration = Instance.Create<ICimplicityViewsConfiguration>());

       

        public ICimplicityViewsConfiguration Create()
        {
            var configurationType = _configurationTypes.FirstOrDefault(type => type.Name == _configurationType);
            return FastActivator.Create(configurationType) as ICimplicityViewsConfiguration;
        }

        public TConfiguration Create<TConfiguration>() where TConfiguration : class
        {
            var configurationType = _configurationTypes.FirstOrDefault(c => c.Implements(typeof(TConfiguration)));
            if (configurationType == null)
            {
                return null;
            }
            return FastActivator.Create(configurationType) as TConfiguration;
        }

        private static ICimplicityViewsConfiguration _configuration;
   
        private static ConfigurationFactory _instance;

        public static ConfigurationFactory Instance => _instance ?? (_instance = new ConfigurationFactory());
    }
}
