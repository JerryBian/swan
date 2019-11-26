using System.IO;
using System.Text;

namespace Laobian.Share
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}