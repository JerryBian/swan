using Laobian.Share.Helper;
using Xunit;

namespace Laobian.Share.Test
{
    public class UrlHelperTest
    {
        [Theory]
        [InlineData("https://www.google.com/", "mail", "me")]
        [InlineData("https://www.google.com", "/mail", "/me")]
        public void Test1(string baseAddress, params string[] parts)
        {
            var result = UrlHelper.Combine(baseAddress, parts);
            Assert.Equal("https://www.google.com/mail/me", result);
        }

        [Theory]
        [InlineData("https://www.google.com/", "mail", "me/")]
        public void Test2(string baseAddress, params string[] parts)
        {
            var result = UrlHelper.Combine(baseAddress, parts);
            Assert.Equal("https://www.google.com/mail/me/", result);
        }
    }
}