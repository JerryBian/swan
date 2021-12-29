using System.IO;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository;

public class NoteFileRepository : GitFileRepository, INoteFileRepository
{
    public NoteFileRepository(IOptions<ApiOptions> options)
    {
        var assetPath = Path.Combine(options.Value.AssetLocation, Constants.AssetDbFolder);
        BasePath = Path.Combine(assetPath, Constants.AssetDbNoteFolder);
    }
}