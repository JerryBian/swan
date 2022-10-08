namespace Laobian.Web.Models
{
    public class Quote
    {
        private readonly Random _random;
        private readonly List<Tuple<string, string>> _quotes;

        public Quote()
        {
            _random = new Random();
            _quotes = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("汪曾祺", "四方食事，不过一碗人间烟火。"),
                new Tuple<string, string>("莎士比亚", "我荒废了时间，时间便把我荒废了。"),
                new Tuple<string, string>("余光中", "掉头一去是风吹黑发，回首再来已雪满白头。"),
                new Tuple<string, string>("苏轼", "浮名浮利，虚苦劳神。叹隙中驹，石中火，梦中身。"),
                new Tuple<string, string>("苏轼", "古之立大事者，不惟有超世之才，亦必有坚忍不拔之志。"),
                new Tuple<string, string>("陶铸", "如烟往事俱忘却，心底无私天地宽。"),
                new Tuple<string, string>("卡夫卡", "尽管人群拥挤，每个人都是沉默的，孤独的。"),
                new Tuple<string, string>("毕淑敏", "父母在，人生尚有来处；父母去，人生只剩归途。"),
                new Tuple<string, string>("苏轼", "可惜一溪风月，莫教踏碎琼瑶。"),
                new Tuple<string, string>("马里奥·安德烈蒂", "倘若你一等再等，结果只能是你又老了一岁。"),
                new Tuple<string, string>("罗曼·罗兰", "世界上只有一种真正的英雄主义，就是认清生活的真相后依然热爱生活。")
            };
        }

        public Tuple<string, string> GetOne()
        {
            return _quotes[_random.Next(0, _quotes.Count - 1)];
        }
    }
}
