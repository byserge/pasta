:moves plugins to host application Pasta.Screenshot
set OLDDIR=%CD%
cd /d %~dp0 

set Configuration=%~1
if "%~1"=="" set Configuration=Debug

set TargetDir=Pasta.Screenshot\bin\%Configuration%\

rmdir %TargetDir%Plugins /S /Q
mkdir %TargetDir%Plugins
xcopy Pasta.BasicEffects\bin\%Configuration% %TargetDir%Plugins\Pasta.BasicEffects /E /I /Y /EXCLUDE:exclude.txt
xcopy Pasta.BasicExport\bin\%Configuration% %TargetDir%Plugins\Pasta.BasicExport /E /I /Y /EXCLUDE:exclude.txt
xcopy Pasta.OcrExport\bin\%Configuration% %TargetDir%Plugins\Pasta.OcrExport /E /I /Y /EXCLUDE:exclude.txt

chdir /d %OLDDIR% &rem restore current directory