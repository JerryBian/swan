﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share.Blog
{
    public class BlogPost
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("mdContent")] 
        public string MdContent { get; set; }

        [JsonPropertyName("title")] public string Title { get; set; }

        [JsonPropertyName("createTime")] public DateTime CreateTime { get; set; }

        [JsonPropertyName("publishTime")] public DateTime PublishTime { get; set; }

        [JsonPropertyName("lastUpdateTime")] public DateTime LastUpdateTime { get; set; }

        [JsonPropertyName("isPublished")] public bool IsPublished { get; set; }

        [JsonPropertyName("isTopping")] public bool IsTopping { get; set; }

        [JsonPropertyName("containsMath")] public bool ContainsMath { get; set; }

        [JsonPropertyName("excerpt")] public string Excerpt { get; set; }

        [JsonPropertyName("tag")]
        public List<string> Tag { get; set; } = new();

        public bool IsPostPublished()
        {
            return IsPublished && PublishTime <= DateTime.Now;
        }

        public string GetFullPath(string baseAddress)
        {
            var path =
                $"{baseAddress}/{PublishTime.Year:D4}/{PublishTime.Month:D2}/{Link}.html";
            return path;
        }
    }
}
