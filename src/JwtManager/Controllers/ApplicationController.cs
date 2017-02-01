﻿using LegnicaIT.BusinessLogic.Actions.App.Interfaces;
using LegnicaIT.BusinessLogic.Helpers;
using LegnicaIT.JwtManager.Configuration;
using Microsoft.Extensions.Options;
using LegnicaIT.JwtManager.Authorization;
using LegnicaIT.BusinessLogic.Enums;
using LegnicaIT.BusinessLogic.Models;
using LegnicaIT.JwtManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LegnicaIT.BusinessLogic.Actions.UserApp.Interfaces;
using LegnicaIT.BusinessLogic.Actions.User.Interfaces;

namespace LegnicaIT.JwtManager.Controllers
{
    [AuthorizeFilter(UserRole.Manager)]
    public class ApplicationController : BaseController
    {
        private readonly IGetAppUserRole getUserRole;
        private readonly IRevokeRole revokeRole;
        private readonly IGrantRole grantRole;
        private readonly IDeleteUserApp deleteUserApp;
        private readonly IGetUserApps getUserApps;
        private readonly IGetApp getApp;
        private readonly IAddNewApp addNewApp;
        private readonly IEditApp editApp;
        private readonly IAddNewUserApp addUserApp;

        public ApplicationController(
            IGetAppUserRole getUserRole,
            IRevokeRole revokeRole,
            IGrantRole grantRole,
            IDeleteUserApp deleteUserApp,
            IAddNewUserApp addUserApp,
            IGetUserApps getUserApps,
            IOptions<ManagerSettings> managerSettings,
            IOptions<LoggerConfig> loggerSettings,
            IGetApp getApp,
            IAddNewApp addNewApp,
            IEditApp editApp)
            : base(managerSettings, loggerSettings)
        {
            this.getUserRole = getUserRole;
            this.revokeRole = revokeRole;
            this.grantRole = grantRole;
            this.deleteUserApp = deleteUserApp;
            this.addUserApp = addUserApp;
            this.getApp = getApp;
            this.getUserApps = getUserApps;
            this.addNewApp = addNewApp;
            this.editApp = editApp;
        }

        public IActionResult Index()
        {
            var userApps = getUserApps.Invoke(LoggedUser.UserModel.Id);
            List<AppViewModel> listOfApps = new List<AppViewModel>();

            foreach (var appFromDb in userApps)
            {
                var model = new AppViewModel
                {
                    Id = appFromDb.Id,
                    Name = appFromDb.Name
                };

                listOfApps.Add(model);
            }

            ViewData["apps"] = listOfApps;

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(AppUserViewModel appuser)
        {
            if (!ModelState.IsValid)
            {
                //TODO error info
            }
            var newAppuser = new LegnicaIT.BusinessLogic.Models.UserAppModel
            {
                AppId = appuser.AppId,
                UserId = appuser.UserId,
                Role = appuser.Role
            };

            addUserApp.Invoke(newAppuser);

            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult AddUser(int id)
        {
            //TODO Adduser View with action AddUser
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(AppUserViewModel appuser)
        {
            if (!ModelState.IsValid)
            {
                //TODO error info
            }

            deleteUserApp.Invoke(appuser.UserId);
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult ListUsers(int appId) // Based on selected app?
        {
            return View("ListUsers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RevokeUserRole(AppUserViewModel appuser)
        {
            if (!ModelState.IsValid)
            {
                //TODO error info
            }

            revokeRole.Invoke(appuser.AppId, appuser.UserId, appuser.Role);
            return RedirectToAction("ListUsers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GrantUserRole(AppUserViewModel appuser)
        {
            if (!ModelState.IsValid)
            {
                //TODO error info
            }

            grantRole.Invoke(appuser.AppId, appuser.UserId, appuser.Role);
            return RedirectToAction("ListUsers");
        }

        [ValidateAntiForgeryToken]
        public IActionResult ChangeUserRole(int appId, int userId)
        {
            var userRole = getUserRole.Invoke(appId, userId);

            ViewData["userRole"] = userRole;

            return View("ChangeUserRole");
        }

        /*
         *  Show/add/edit applications
         */

        [AuthorizeFilter(UserRole.User)]
        public IActionResult Details(int id)
        {
            var app = getApp.Invoke(id);
            var model = new AppViewModel { Id = app.Id, Name = app.Name };

            return View(new FormModel<AppViewModel>(model));
        }

        [AuthorizeFilter(UserRole.SuperAdmin)]
        public IActionResult Add()
        {
            var model = new AppViewModel();

            return View(new FormModel<AppViewModel>(model, true));
        }

        [AuthorizeFilter(UserRole.SuperAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AppViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Alert danger
                return View(new FormModel<AppViewModel>(model, true));
            }

            var newModel = new AppModel { Id = model.Id, Name = model.Name };
            addNewApp.Invoke(newModel);

            // TODO: Alert success
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var app = getApp.Invoke(id);

            var model = new AppViewModel { Id = app.Id, Name = app.Name };

            return View(new FormModel<AppViewModel>(model, true));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Alert danger
                return View(new FormModel<AppViewModel>(model, true));
            }

            var newModel = new AppModel { Id = model.Id, Name = model.Name };
            editApp.Invoke(newModel);

            // TODO: Alert success
            return RedirectToAction("Details", new { id = newModel.Id });
        }
    }
}
