﻿using System.Linq;
using LegnicaIT.BusinessLogic.Actions.User.Interfaces;
using LegnicaIT.BusinessLogic.Enums;
using LegnicaIT.BusinessLogic.Helpers;
using LegnicaIT.DataAccess.Repositories.Interfaces;

namespace LegnicaIT.BusinessLogic.Actions.User.Implementation
{
    public class GrantRole : IGrantRole
    {
        private readonly IUserAppRepository userAppRepository;
        private readonly IUserRepository userRepository;

        public GrantRole(IUserAppRepository userAppRepository, IUserRepository userRepository)
        {
            this.userAppRepository = userAppRepository;
            this.userRepository = userRepository;
        }

        public bool Invoke(int appId, int user, UserRole newRole)
        {
            var userFromDb = userRepository.GetById(user);

            if (userFromDb == null || userFromDb.IsSuperAdmin)
            {
                return false;
            }

            if (newRole == UserRole.SuperAdmin)
            {
                userFromDb.IsSuperAdmin = true;
                userRepository.Edit(userFromDb);
                userRepository.Save();

                return true;
            }

            var userApp = userAppRepository.FindBy(m => m.User.Id == user && m.App.Id == appId).FirstOrDefault();

            if (userApp == null)
            {
                return false;
            }

            var userRole = (UserRole)userApp.Role;

            if (userRole.HasRole(newRole))
            {
                return false;
            }

            userApp.Role = (DataAccess.Enums.UserRole)newRole;
            userAppRepository.Edit(userApp);
            userAppRepository.Save();

            return true;
        }
    }
}
