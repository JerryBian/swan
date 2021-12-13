using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Site.Blog;

namespace Laobian.Share.Grpc.Response;

[DataContract]
public class BlogResponse : ResponseBase
{
    [DataMember(Order = 1)] public List<BlogPostRuntime> Posts { get; set; }

    [DataMember(Order = 2)] public BlogPostRuntime PostRuntime { get; set; }

    [DataMember(Order = 3)] public BlogPost Post { get; set; }

    [DataMember(Order = 4)] public List<BlogTag> Tags { get; set; }

    [DataMember(Order = 5)] public BlogTag Tag { get; set; }
}