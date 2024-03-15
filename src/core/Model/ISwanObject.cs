using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public interface ISwanObject
    {
        string Id { get; set; }

        DateTime LastModifiedAt { get; set; }

        DateTime CreatedAt { get; set; }

        static abstract string GitPath { get; }
    }
}
