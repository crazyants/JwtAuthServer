using LegnicaIT.BusinessLogic.Actions.User.Interfaces;
using LegnicaIT.BusinessLogic.Enums;
using LegnicaIT.BusinessLogic.Helpers;
using LegnicaIT.BusinessLogic.Models.Common;
using LegnicaIT.JwtManager.Authorization;
using LegnicaIT.JwtManager.Configuration;
using LegnicaIT.JwtManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace LegnicaIT.JwtManager.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IGetUserDetails getUserDetails;

        public AuthController(IOptions<ManagerSettings> managerSettings,
            IGetUserDetails getUserDetails,
            IOptions<LoggerConfig> loggerSettings)
            : base(managerSettings, loggerSettings)
        {
            this.getUserDetails = getUserDetails;
        }

        [HttpGet("/auth")]
        [AuthorizeFilter(UserRole.Manager)]
        public ActionResult Index()
        {
            var result = new ResultModel<string>("User have Manager permission");

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet("/auth/login")]
        public ActionResult Login()
        {
            var LoginModel = new LoginModel();
            return View(LoginModel);
        }

        [AllowAnonymous]
        [HttpPost("/auth/login")]
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

            var user = getUserDetails.Invoke(model.Email);
            HttpContext.Session.SetString("UserDetails", JsonConvert.SerializeObject(user));

            ViewData["Message"] = model.Email;
            return RedirectToActionPermanent("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("/auth/logout")]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove("token");
                HttpContext.Session.Clear();
            }
            catch (Exception e)
            {
                logger.Information($"Something went wrong during logout : {e}");
            }

            return RedirectToAction("Login");
        }
    }
}
