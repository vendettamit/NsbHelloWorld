using System.Threading;
using System.Web.Mvc;

namespace NsbHelloWorld.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult KickOff()
		{
			//what did you expect to see here? it's a hack
			Thread.Sleep(5000);

			return Content("Hello World");
		}
	}
}