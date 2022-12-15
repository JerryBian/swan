using System.Text.Json.Serialization;

namespace Swan.Areas.Admin.Models
{
    public class StackEditorImageUploadRes
    {
        [JsonPropertyName("UploadedImage")]
        public string UploadedImage { get; set; }
    }
}
