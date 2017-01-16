﻿using LegnicaIT.BusinessLogic.Actions.Base;
using LegnicaIT.BusinessLogic.Models.App;

namespace LegnicaIT.BusinessLogic.Actions.App.Interfaces
{
    public interface IAddNewApp : IAction
    {
        void Invoke(AppModel app);
    }
}