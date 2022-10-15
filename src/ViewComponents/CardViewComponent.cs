using Laobian.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.ViewComponents
{
    public class CardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CardData data)
        {
            return View(data);
        }
    }
}
