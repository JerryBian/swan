﻿using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public class BlogTagObject : FileObjectBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public override string GetFileName()
        {
            return "tag.json";
        }
    }
}