namespace Swan.Lib.Model
{
    public class ReadItemView
    {
        public ReadItemView(ReadItem raw)
        {
            Raw = raw;
            Posts = new List<Tuple<string, string, string>>();
        }

        public ReadItem Raw { get; init; }

        public string CommentHtml { get; set; }

        public List<Tuple<string, string, string>> Posts { get; init; }

        public string Metadata { get; set; }
    }
}
