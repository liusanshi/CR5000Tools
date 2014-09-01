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
        /// <summary>
        /// 登录
        /// </summary>
        LOGIN = 0,
        /// <summary>
        /// 登出
        /// </summary>
        LOGOUT,
        /// <summary>
        /// 打开plm
        /// </summary>
        OPENPLM,
        /// <summary>
        /// 查找文档
        /// </summary>
        DOCFIND,
        /// <summary>
        /// 查找物料
        /// </summary>
        MATFIND,
        /// <summary>
        /// 签出文档
        /// </summary>
        CHECKOUT,
        /// <summary>
        /// 签入文件
        /// </summary>
        CHECKIN,
        /// <summary>
        /// 导入工作区
        /// </summary>
        WORKSPACE,
        /// <summary>
        /// 导入项目输出
        /// </summary>
        PROJECT,
        /// <summary>
        /// 导入任务输出
        /// </summary>
        TASK,
        /// <summary>
        /// 物料标准化
        /// </summary>
        GM,
        /// <summary>
        /// 无用
        /// </summary>
        SETRIGHT,
        /// <summary>
        /// 配置
        /// </summary>
        CONFIG,
        /// <summary>
        /// 无用
        /// </summary>
        USERDEFINED
    }
}
