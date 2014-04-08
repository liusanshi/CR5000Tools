@echo off

rem 获取插件安装路径
rem HKEY_LOCAL_MACHINE\SOFTWARE\Kingdee\金蝶 K3 WISE PLM客户端组件集
rem Path : c:\Program Files\Kingdee\K3PLM\Integration

rem web站点安装目录
rem HKEY_LOCAL_MACHINE\SOFTWARE\Kingdee\KDMIDDLEWARE\K3PLM
rem 64位
rem HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Kingdee\KDMIDDLEWARE\K3PLM
rem Path : D:\Program Files\kingdee\K3PLM\web

setlocal 

echo 开始服务器部署
if /i "%PROCESSOR_IDENTIFIER:~0,3%" == "X86" ( 
goto x86
) else (
goto x64
)

:x86
for /f "tokens=1,2,* " %%i in ('REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\Kingdee\KDMIDDLEWARE\K3PLM" ^| find /i "Path"') do set "PLMPath=%%k"
goto copyoperate

:x64
for /f "tokens=1,2,* " %%i in ('REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Kingdee\KDMIDDLEWARE\K3PLM" ^| find /i "Path"') do set "PLMPath=%%k"
goto copyoperate

:copyoperate

copy /y "Zuken.Server.dll" "%PLMPath%\bin\Zuken.Server.dll"
copy /y "ZKSCHsht.gif" "%PLMPath%\skins\ObjectIcon\ZKSCHsht.gif"
copy /y "ZKPCBpcb.gif" "%PLMPath%\skins\ObjectIcon\ZKPCBpcb.gif"
copy /y "ADPRJcmd.gif" "%PLMPath%\skins\ObjectIcon\ADPRJcmd.gif"
copy /y "%PLMPath%\Integration.config.xml" "%PLMPath%\Integration_back.config.xml"
copy /y "Integration.config.xml" "%PLMPath%\Integration.config.xml"
copy /y "ZSKIntegration.js" "%PLMPath%\Javascript\Integration\ZSKIntegration.js"

ModifyConfig.exe "%PLMPath%\Web.config" configuration/appSettings add _license_ALLEGROCIS PROE
ModifyConfig.exe "%PLMPath%\Web.config" configuration/appSettings add _license_ALLEGROPCB PROE

ModifyConfig.exe "%PLMPath%\Web.config" configuration/appSettings add _EFT_ZKSCH SCH
ModifyConfig.exe "%PLMPath%\Web.config" configuration/appSettings add _EFT_ZKPCB PCB
ModifyConfig.exe "%PLMPath%\Web.config" configuration/appSettings add _EFT_ZKPRJ PRJ

ModifyConfig.exe "%PLMPath%\Integration.config.xml" IntegrationConfiguration/appSettings update ALLEGROCIS "Zuken.Server.ZukenSchOperate, Zuken.Server"
ModifyConfig.exe "%PLMPath%\Integration.config.xml" IntegrationConfiguration/appSettings update ALLEGROPCB "Zuken.Server.ZukenPcbOperate, Zuken.Server"

ModifyConfig.exe "%PLMPath%\Document\ElectronIntegration.aspx" IntegrationConfiguration/appSettings replace "var isimport = false;" "if(!bomtips()) {return false;};var isimport = false;"

ModifyConfig.exe "%PLMPath%\Document\Class\Document.js" IntegrationConfiguration/appSettings append "/***DocumentDownload-SetFileWriteAttributes(LocalFileName)***/" ";function DocumentDownload(FtpFileName, LocalFileName) {if (!CheckFtp()) return false;if (FtpFileName == \"\" || LocalFileName == \"\") return false;result = fileControler.Controler.DownloadFile(FtpFileName, LocalFileName);/***DocumentDownload-SetFileWriteAttributes(LocalFileName)***/SetFileWriteAttributes(LocalFileName);}"

echo 服务器部署完成
echo.
echo.
echo 开始插件部署

set Integration=%PLMPath%\..\Integration\Integration Setup

copy /y "Kingdee.PLM.Integration.Setup.Zuken.dll" "%Integration%\KDSetup\Dll\Kingdee.PLM.Integration.Setup.Zuken.dll"
copy /y "Kingdee.PLM.Integration.Client.Zuken.dll" "%Integration%\Resources\Common\Dll\Kingdee.PLM.Integration.Client.Zuken.dll"
copy /y "Zuken.Common.dll" "%Integration%\Resources\Common\Dll\Zuken.Common.dll"

if not exist "%Integration%\Resources\Zuken" ( md "%Integration%\Resources\Zuken" )
copy /y "openCADFileManage.js" "%Integration%\Resources\Zuken\openCADFileManage.js"
copy /y "openCADFileManage.vbs" "%Integration%\Resources\Zuken\openCADFileManage.vbs"
copy /y "proj.tpl" "%Integration%\Resources\Zuken\proj.tpl"
copy /y "PCBColumnConfig.txt" "%Integration%\Resources\Zuken\PCBColumnConfig.txt"
copy /y "Vici.Core.dll" "%Integration%\Resources\Zuken\Vici.Core.dll"
copy /y "Zuken.Command.exe" "%Integration%\Resources\Zuken\Zuken.Command.exe"
copy /y "Zuken.Common.dll" "%Integration%\Resources\Zuken\Zuken.Common.dll"
copy /y "component.ini" "%Integration%\Resources\Zuken\component.ini"
copy /y "log.jpg" "%Integration%\Resources\Zuken\log.jpg"
copy /y "icon.png" "%Integration%\Resources\Zuken\icon.png"



echo 插件部署完成
echo.
echo 在客户端安装插件即可使用
echo.
pause