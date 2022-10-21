namespace Laobian.Lib.Provider
{
    public interface IAssetFileProvider
    {
        string GetLogBaseDir();

        string LogExtension { get; }

        string JsonExtension { get; }
    }
}
