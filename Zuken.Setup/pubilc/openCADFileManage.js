var ws = new ActiveXObject("Wscript.Shell");
var args = WScript.Arguments;
var openType = args(0),
zfmschemePath = args(1) + "\\zsys\\bin\\zfmscheme.exe",
zbmsPath = args(1) + "\\zsys\\scm\\system\\zbms.scc",
filePath = args(2),
cmd = "";

cmd = zfmschemePath + " -f " + zbmsPath;
if(openType == "sch"){
    cmd += " -t cr5sdfm -p:filter shtdata "
}else{
    cmd += " -t cr5bdfm -p:filter pcbdata "
}
cmd += " -directory " + filePath;

ws.run(cmd);