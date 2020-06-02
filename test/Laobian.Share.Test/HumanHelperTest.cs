using System;
using Laobian.Share.Helper;
using Xunit;

namespace Laobian.Share.Test
{
    public class HumanHelperTest
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal("123", HumanHelper.DisplayInt(123));
        }

        [Fact]
        public void Test2()
        {
            Assert.Equal("1.2k", HumanHelper.DisplayInt(1234));
        }

        [Fact]
        public void Test3()
        {
            Assert.Equal("12.6k", HumanHelper.DisplayInt(12553));
        }

        [Fact]
        public void Test4()
        {
            Assert.Equal("1.3m", HumanHelper.DisplayInt(1_255_123));
        }

        [Fact]
        public void Test5()
        {
            Assert.Equal("125B", HumanHelper.DisplayBytes(125));
        }

        [Fact]
        public void Test6()
        {
            Assert.Equal("10.3k", HumanHelper.DisplayBytes(Convert.ToInt64(1024 * 10.25)));
        }

        [Fact]
        public void Test7()
        {
            Assert.Equal("1.3M", HumanHelper.DisplayBytes(Convert.ToInt64(1024 * 1024 * 1.25)));
        }

        [Fact]
        public void Test8()
        {
            Assert.Equal("1.3G", HumanHelper.DisplayBytes(Convert.ToInt64(1024 * 1024 * 1024 * 1.25)));
        }

        [Fact]
        public void Test9()
        {
            Assert.Equal("1.3T", HumanHelper.DisplayBytes(Convert.ToInt64((long) 1024 * 1024 * 1024 * 1024 * 1.25)));
        }

        [Fact]
        public void Test10()
        {
            Assert.Equal("109k", HumanHelper.DisplayInt(109009));
        }

        [Fact]
        public void Test11()
        {
            Assert.Equal("12小时01分钟", HumanHelper.DisplayTimeSpan(new TimeSpan(0, 12, 1, 2)));
        }

        [Fact]
        public void Test12()
        {
            Assert.Equal("10天02小时", HumanHelper.DisplayTimeSpan(new TimeSpan(10, 2, 1, 2)));
        }

        [Fact]
        public void Test13()
        {
            Assert.Equal("2年256天12小时", HumanHelper.DisplayTimeSpan(new TimeSpan(986, 12, 1, 2)));
        }

        [Fact]
        public void Test14()
        {
            Assert.Equal("365天12小时", HumanHelper.DisplayTimeSpan(new TimeSpan(365, 12, 1, 2)));
        }

        [Fact]
        public void Test15()
        {
            Assert.Equal("59秒", HumanHelper.DisplayTimeSpan(new TimeSpan(0, 0, 0, 59)));
        }
    }
}