namespace Laobian.Lib.Model
{
    public class ReadItemView
    {
        public ReadItemView(ReadItem raw)
        {
            Raw = raw;
        }

        public ReadItem Raw { get; init; }

        public string CommentHtml { get; set; }

        public string PostCommentTitle { get; set; }

        public string PostCommentUrl { get; set; }

        public string Metadata { get; set; }
    }
}
