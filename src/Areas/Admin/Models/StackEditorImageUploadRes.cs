using System.Text.Json.Serialization;

namespace Laobian.Areas.Admin.Models
{
    public class StackEditorImageUploadRes
    {
        [JsonPropertyName("UploadedImage")]
        public string UploadedImage { get; set; }
    }
}
