using System.Runtime.Serialization;
using Laobian.Share.Site.Blog;

namespace Laobian.Share.Grpc.Request;

[DataContract]
public class BlogRequest
{
    [DataMember(Order = 1)] public bool ExtractRuntime { get; set; }

    [DataMember(Order = 2)] public string Link { get; set; }

    [DataMember(Order = 3)] public BlogPost Post { get; set; }

    [DataMember(Order = 4)] public string ReplacedPostLink { get; set; }

    [DataMember(Order = 5)] public string TagId { get; set; }

    [DataMember(Order = 6)] public BlogTag Tag { get; set; }
}