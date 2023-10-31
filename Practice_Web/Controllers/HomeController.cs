using Practice_Web.DAL;
using Practice_Web.Models;
using Practice_Web.Models.Entity;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Practice_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserContext context = new UserContext();
        private readonly OperationsByADO operations = new OperationsByADO();
        private readonly string SLU = Constants.SESSION_LOGGED_USER;

        public HomeController()
        {
            //Default constructor
        }

        // GET: Home
        public ActionResult Index() => Session[SLU] == null ? View() : Index((User)Session[SLU], true);

        [HttpPost]
        public ActionResult Index(User user, bool throughSession = false)
        {
            if (user != null && ModelState.IsValid && (throughSession || operations.IsLoginSuccess(user)))
            {
                ViewBag.ServerMSG = "Login success!";
                ViewBag.IsLoginSuccess = "true";
                operations.HandleLoggedSession(true, user);
                return View(user);
            }

            ViewBag.ServerMSG = "Login failed!";
            ViewBag.IsLoginSuccess = "false";
            operations.HandleLoggedSession(false, null);
            return View();
        }

        public ActionResult Register() => View();

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (user == null || user.HasNullInProperties) return View();

            if (operations.IsAccountExist(user.Account))
            {
                ViewBag.ServerMSG = "Account is already existed!";
                return View();
            }

            if (ModelState.IsValid)
            {
                if (operations.HandleRegister(user) != 1) return View();

                operations.HandleLoggedSession(true, user);
                return RedirectToAction("Index");
            }

            return View(user.Password = "");
        }

        public ActionResult ChangePassword() => Session[SLU] != null ? View() : RedirectToAction("Index") as ActionResult;

        [HttpPost]
        public ActionResult ChangePassword(User user)
        {
            if (user == null || user.Password == null || user.Password.Length < 8 || user.Password.Length > 32)
            {
                return View();
            }
            else if (Session[SLU] == null)
            {
                return RedirectToAction("Index");
            }

            var userSession = Session[SLU] as User;
            userSession.Password = user.Password;

            if (operations.HandleChangePassword(userSession) == 0)
            {
                ViewBag.ServerMSG = "Operation failed, please try again.";
                return View();
            }

            operations.HandleLoggedSession(true, userSession);
            return RedirectToAction("Index");
        }

        //private void HandleCookie(User user)
        //{
        //    var cookieName = "LoggedUserCookie";
        //    if (Response.Cookies.AllKeys.Contains(cookieName))
        //    {
        //        HttpContext.Application.Remove(Response.Cookies[cookieName].Value);
        //        Response.Cookies.Remove(cookieName);
        //    }

        //    var token = Guid.NewGuid().ToString();
        //    HttpContext.Application[token] = DateTime.UtcNow.AddHours(1);

        //    Response.Cookies.Add(
        //        new HttpCookie(cookieName, token)
        //        {
        //            Expires = DateTime.Now.AddHours(1),
        //            HttpOnly = true
        //        }
        //    );
        //}

        public ActionResult Logout()
        {
            operations.HandleLoggedSession(false, null);
            return RedirectToAction("Index");
        }

        public ActionResult Test_1() => Json($"Deleted {operations.DeleteUser(4)} from Database.", JsonRequestBehavior.AllowGet);
    }
}
