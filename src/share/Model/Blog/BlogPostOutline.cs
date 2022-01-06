using System.Runtime.Serialization;

namespace Laobian.Share.Model.Blog;

[DataContract]
public class BlogPostOutline
{
    [DataMember(Order = 1)] public string Link { get; set; }

    [DataMember(Order = 2)] public string Title { get; set; }
}