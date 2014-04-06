Set ws = CreateObject("Wscript.Shell")
'Set args = WScript.Arguments;
ws.run "C:\cr5000\local\zsys\bin\zfmscheme.exe -f C:\cr5000\local\zsys\scm\system\zbms.scc -t cr5sdfm -p:filter shtdata  -directory E:\项目备份\Zuken项目\sample\ll.cir"

'C:\cr5000\local\zsys\bin\zfmscheme.exe -f C:\cr5000\local\zsys\scm\system\zbms.scc -t cr5sdfm -p:filter shtdata  -directory %cd%\ll.cir