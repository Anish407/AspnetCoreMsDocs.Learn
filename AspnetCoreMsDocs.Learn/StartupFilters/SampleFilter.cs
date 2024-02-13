namespace AspnetCoreMsDocs.Learn.StartupFilters;

public class SampleFilter: IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseMiddleware<RequestHandlingMiddleware>();
            next(builder);
        };
    }
}