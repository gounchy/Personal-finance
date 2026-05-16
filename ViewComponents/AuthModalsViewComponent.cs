using System.Linq;
using CNPM_Nhom12.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CNPM_Nhom12.ViewComponents
{
    public class AuthModalsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string theme = "public")
        {
            var returnUrl = HttpContext?.Request.Query["returnUrl"].FirstOrDefault();
            var vm = new AuthModalsViewModel
            {
                ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? null : returnUrl,
                Theme = theme
            };
            return View(vm);
        }
    }
}
