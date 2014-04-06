using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zuken.Command.Entity
{
    /// <summary>
    /// 命令
    /// </summary>
    public enum IntegrationOrder
    {
        LOGIN = 0,
        LOGOUT,
        OPENPLM,
        DOCFIND,
        MATFIND,
        CHECKOUT,
        CHECKIN,
        WORKSPACE,
        PROJECT,
        TASK,
        GM,
        SETRIGHT,
        CONFIG,
        USERDEFINED
    }
}
