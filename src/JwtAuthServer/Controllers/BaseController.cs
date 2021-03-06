﻿using LegnicaIT.BusinessLogic.Configuration.Helpers;
using LegnicaIT.BusinessLogic.Configuration;
using LegnicaIT.JwtAuthServer.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace LegnicaIT.JwtAuthServer.Controllers
{
    [EnableCors("AnyOrigin")]
    public class BaseController : Controller
    {
        protected UserAppModel LoggedUser { get; set; }
        public Logger logger { get; set; }

        public BaseController(IOptions<LoggerConfig> loggerSettings)
        {
            logger = new Logger(GetType(), loggerSettings);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // we will have User authenticated by app.UseJwtBearerAuthentication(...)
            var user = ((ControllerBase)context.Controller).User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                LoggedUser = new UserAppModel();
                // convert security claims to our custom user data
                LoggedUser.FillFromClaims(user.Claims);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            if (!context.ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];

                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        logger.Warning($"Key: {key}, Error: {errorMessage}");
                    }
                }
            }
        }
    }
}
