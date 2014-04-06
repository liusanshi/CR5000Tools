1. 安装包 文件清单：
    拷入 插件安装目录
    Zuken.Common.dll
    Zuken.Command.exe
    openCADFileManage.js
    openCADFileManage.vbs
    proj.tpl
    Vici.Core.dll
    IntegrationLogin.exe
    IntegrationLogin.exe.config
    
    Kingdee.PLM.Integration.Client.Zuken.dll
    Zuken.Common.dll
    
    菜单文件
    
2. 安装说明：dfg
    2.1. 将安装目录加入 path环境变量
    2.2. 添加环境变量：ZUKEN_PLM 指向安装目录
    2.3. 所有文件放入安装目录
    2.4. 卸载的时候还原环境变量
    2.5. 菜单文件 
    
3. web 站点 部署说明
    3.1  Integration.config.xml 添加节点：
    <add key="ZKSCH" value="Zuken.Server.ZukenSchOperate, Zuken.Server"/>
    <add key="ZKPCB" value="Zuken.Server.ZukenPcbOperate, Zuken.Server"/>
    
    3.2  Zuken.Server.dll 拷入 web的bin目录
    
    3.3  将图标文件考入 web\skins\ObjectIcon
    ZKSCHsht.gif
    ZKPCBpcb.gif
    ZKPRJcmd.gif
    
    3.4 在数据库中设置 zksch、zkpcb 的文件类型、业务类型
