namespace Laobian.Lib.Extension
{
    public static class TaskExtension
    {
        public static async Task OkForCancel(this Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException) { }
        }
    }
}
