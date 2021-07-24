namespace Laobian.Api.SourceProvider
{
    public interface ISourceProviderFactory
    {
        ISourceProvider Get(SourceMode source);
    }
}