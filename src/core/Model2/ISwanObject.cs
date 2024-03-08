using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Swan.Core.Model2
{
    public interface ISwanObject
    {
        static abstract string ObjectName { get; }

        static abstract List<string> GetObjectProperties();

        long Id { get; set; }

        DateTime LastModifiedAt { get; set; }

        DateTime CreatedAt { get; set; }
    }
}
