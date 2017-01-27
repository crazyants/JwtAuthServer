﻿using LegnicaIT.BusinessLogic.Actions.Base;
using LegnicaIT.BusinessLogic.Models;

namespace LegnicaIT.BusinessLogic.Actions.UserApp.Interfaces
{
    public interface IAddNewUserApp : IAction
    {
        void Invoke(UserAppModel model);
    }
}
