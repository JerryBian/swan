using System.IO;
using System.Text;

namespace Laobian.Share.Core
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
