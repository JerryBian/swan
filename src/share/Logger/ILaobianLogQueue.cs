namespace Laobian.Share.Logger
{
    public interface ILaobianLogQueue
    {
        void Add(LaobianLog log);

        bool TryDequeue(out LaobianLog log);
    }
}