using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class ReadModel
    {
        public ReadModel(ReadObject obj)
        {
            Object = obj;
        }

        public ReadObject Object { get; init; }

        public string Metadata { get; set; }

        public string CommentHtml { get; set; }
    }
}
