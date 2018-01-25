using System.Configuration;
using System.Linq;
using Utils.Extensions.Reflection;

namespace Cimplicity.Views.Infrastructure.Configuration.AppProperties {
    public class AppSettingsCimplicityViewsConfiguration : ICimplicityViewsConfiguration {
        public const string ShippingRolesRoleName = "Shipping_Roles";
        public const string ShippingAdminRoleName = "Shipping_Admin";
        public const string ShippingUserRoleName = "Shipping_User";
        public const string ShippingGuiConfiguratorRoleName = "Shipping_Gui_Configurator";


        
        public IDataInfo Data { get; }
        public int RefreshFrequency { get; }
        public string RepositoryAssemblyName { get; }

        public AppSettingsCimplicityViewsConfiguration() {
            var appSetting = ConfigurationManager.AppSettings;

            this.RepositoryAssemblyName = appSetting.AllKeys.Contains(nameof(RepositoryAssemblyName)) ? appSetting[nameof(RepositoryAssemblyName)] : "Default";

            this.Data = new AppSettingsDataInfoConfiguration();


            this.RefreshFrequency = appSetting.AllKeys.Contains(nameof(RefreshFrequency))
                ? appSetting[nameof(RefreshFrequency)].GetValue<int>()
                : 10000;
        }
    }

    public class AppSettingsDataInfoConfiguration : IDataInfo
    {

        public StorageType StorageType { get; }
        public string ConnectionString { get; }


        public AppSettingsDataInfoConfiguration()
        {
            var appSetting = ConfigurationManager.AppSettings;
            this.StorageType = appSetting.AllKeys.Contains(nameof(StorageType))
                ? appSetting[nameof(StorageType)].GetValue<StorageType>()
                : StorageType.Default;

            this.ConnectionString = ConfigurationManager.ConnectionStrings["soadb"].ConnectionString;
        }
    }
}
