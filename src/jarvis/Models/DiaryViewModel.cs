using Laobian.Share.Model.Jarvis;

namespace Laobian.Jarvis.Models;

public class DiaryViewModel
{
    public DiaryRuntime Current { get; set; }

    public DiaryRuntime Prev { get; set; }

    public DiaryRuntime Next { get; set; }
}