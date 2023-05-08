using Microsoft.AspNetCore.Mvc;

namespace ServiceGuard.Controllers {
    public class HomeController : Controller {
        
        public string Index() {
            return "Test: This is Home Index";
        }

    }
}
