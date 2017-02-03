using LegnicaIT.BusinessLogic.Actions.User.Interfaces;
using LegnicaIT.BusinessLogic.Enums;
using LegnicaIT.BusinessLogic.Helpers;
using LegnicaIT.BusinessLogic.Models.Common;
using LegnicaIT.JwtManager.Authorization;
using LegnicaIT.JwtManager.Configuration;
using LegnicaIT.JwtManager.Helpers;
using LegnicaIT.JwtManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LegnicaIT.JwtManager.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IGetUserDetails getUserDetails;
        private readonly BreadcrumbHelper Breadcrumb = new BreadcrumbHelper();

        public AuthController(IOptions<ManagerSettings> managerSettings,
            IGetUserDetails getUserDetails,
            IOptions<LoggerConfig> loggerSettings)
            : base(managerSettings, loggerSettings)
        {
            this.getUserDetails = getUserDetails;
            Breadcrumb.Add("Authorization", "Index", "Auth");
        }

        [AuthorizeFilter(UserRole.Manager)]
        public ActionResult Index()
        {
            var result = new ResultModel<string>("User have Manager permission");

            return Json(result);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            Breadcrumb.Add("Login", "Login", "Auth");

            ViewData.Add("breadcrumbItems", Breadcrumb.GetBreadcrumbItems());

            var LoginModel = new LoginModel();
            return View(LoginModel);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Email", "Invalid email or password");
                logger.Information("Model is not valid");

                return View(model);
            }

            var handler = new ApiHelper(Settings.ApiReference);
            var resultString = handler.AcquireToken(model.Email, model.Password, model.AppId);
            var result = JsonConvert.DeserializeObject<ResultModel<object>>(resultString.ResponseMessage);

            if (result.Status.Code == ResultCode.Error)
            {
                logger.Information("Token is not valid");

                return View(model);
            }

            HttpContext.Session.SetString("token", result.Value.ToString());

            var userDetails = getUserDetails.Invoke(model.Email);
            HttpContext.Session.SetString("UserDetails", JsonConvert.SerializeObject(userDetails));

            ViewData["Message"] = model.Email;

            return RedirectToActionPermanent("Index", "Home");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Logout()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("token")))
            {
                HttpContext.Session.Remove("token");
            }
            else
            {
                logger.Information("Something went wrong during logout");
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
