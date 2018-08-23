using AutoMapper;

namespace WebAPI.Controllers
{
	public static class MappingExtentions
	{
		private static IMapper mapper = new Mapper(new MapperConfiguration(cfg => { }));
		public static TDestination MapTo<TDestination>(this object source) where TDestination : new()
		{
			return mapper.Map<TDestination>(source);
		}

		public static void Map<TSource, TDestination>(this TSource source, out TDestination dest)
		{
			dest = mapper.Map<TDestination>(source);
		}
	}
}