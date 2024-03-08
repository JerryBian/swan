using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swan.Core.Model2
{
    public class SwanConfig
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }
}
