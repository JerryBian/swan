using System.Text.Json.Serialization;

namespace Laobian.Web.Areas.Admin.Models
{
    public class StackEditorImageUploadRes
    {
        [JsonPropertyName("UploadedImage")]
        public string UploadedImage { get; set; }
    }
}
