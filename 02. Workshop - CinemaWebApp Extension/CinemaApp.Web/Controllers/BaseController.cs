namespace CinemaApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        protected bool IsGuidValid(string? id, ref Guid cinemaGuid)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            bool isGuidValid = Guid.TryParse(id, out cinemaGuid);

            if (!isGuidValid)
            {
                return false;
            }

            return true;
        }
    }
}
