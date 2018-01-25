using AutoMapper;

namespace Cimplicity.Views.Infrastructure.Mapping
{
    public static class MapperExtensions
    {
        public static TDestination MapTo<TDestination>(this object source)
            where TDestination : class
        {
            
            return source != null ? Mapper.Map<TDestination>(source) : null;
        }
    }
}