::prj.cmd
::\prj
::    \sd
::       \prj.cir
::              \drc
::              \ext
::              \log
::              \frame
::              rcpath
::              prj.rfc Relation ../../bd/j1052
::              001.sht
::    \bd
::       prj.pcb
::       prj.rfb    Relation ../sd/j1052.cir
::
@echo off
if not exist $String.Format("%~dp0\\\"{0}\"", PrjName) ( mkdir $String.Format("%~dp0\\\"{0}\"", PrjName) )
pushd $String.Format("%~dp0\\\"{0}\"", PrjName)

if not exist "bd" ( mkdir "bd" )
pushd "bd"
if exist $String.Format("\"..\\..\\{0}.pcb\"", PrjName) (
move /y $String.Format("\"..\\..\\{0}.pcb\"", PrjName) $String.Format("\"{0}.pcb\"", PrjName) )
if not exist $String.Format("\"{0}.rfb\"", PrjName) (
echo $String.Concat("Relation ../sd/", PrjName, ".cir>>") $String.Format("\"{0}.rfb\"", PrjName) )
popd

if not exist "sd" ( mkdir "sd" )
pushd "sd"

if not exist $String.Format("\"{0}.cir\"", PrjName) ( mkdir $String.Format("\"{0}.cir\"", PrjName) )
pushd $String.Format("\"{0}.cir\"", PrjName)

if not exist "drc" ( mkdir "drc" )
if not exist "ext" ( mkdir "ext" )
if not exist "log" ( mkdir "log" )
if not exist "frame" ( mkdir "frame" )
if not exist $String.Format("\"{0}.rfb\"", PrjName) (
echo $String.Concat("Relation ../../bd/", PrjName, ">>") $String.Format("\"{0}.rfc\"", PrjName) )

if not exist "rcpath" (
#foreach(x in RcpathData)
echo ${x}>> "rcpath"
#end
)

#foreach(x in SchList)
if exist $String.Format("\"..\\..\\..\\{0}\"", x) (move /y $String.Format("\"..\\..\\..\\{0}\"", x) $String.Format("\"{0}\"", x) ) 
#end

#if(IsSch)
$String.Format("wscript //e:javascript //nologo \"%ZUKEN_PLM%\\openCADFileManage.js\" \"sch\" \"{0}\" \"%~dp0\\{1}\\sd\\{1}.cir\"", ZukenLocal, PrjName)
#else
$String.Format("Zuken.Command.exe \"%~dp0\\{0}\\bd\\{0}.rul\" \"%~dp0\\{0}.cmd\"", PrjName) 
$String.Format("wscript //e:javascript //nologo \"%ZUKEN_PLM%\\openCADFileManage.js\" \"board\" \"{0}\" \"%~dp0\\{1}\\bd\"", ZukenLocal, PrjName)
#end

popd
popd
popd

exit