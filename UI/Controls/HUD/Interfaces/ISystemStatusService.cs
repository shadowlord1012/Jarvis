using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Interfaces
{
    public interface ISystemStatusService
    {
        SystemStatus GetStatus();
    }
}
