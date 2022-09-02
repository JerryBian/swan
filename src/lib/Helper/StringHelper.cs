namespace Laobian.Lib.Helper
{
    public static class StringHelper
    {
        public static string Random()
        {
            return "b" + Guid.NewGuid().ToString("N");
        }
    }
}
