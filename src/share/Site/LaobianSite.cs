using System.Runtime.Serialization;

namespace Laobian.Share.Site;

[DataContract]
public enum LaobianSite
{
    [DataMember(Order = 1)] All = 0,

    [DataMember(Order = 2)] Api = 2,

    [DataMember(Order = 3)] Blog = 4,

    [DataMember(Order = 4)] Admin = 8,

    [DataMember(Order = 5)] Jarvis = 16
}