namespace Cimplicity.Views.Infrastructure.Configuration
{
    public interface ICimplicityViewsConfiguration
    {
       
        IDataInfo Data { get; }
        int RefreshFrequency { get; }
        string RepositoryAssemblyName { get; }
    }

    public interface IDataInfo
    {
        StorageType StorageType { get; }
        string ConnectionString { get; }
    }
}
