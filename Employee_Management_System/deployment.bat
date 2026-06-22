@echo off
setlocal DisableDelayedExpansion

echo =====================================
echo Publishing Project...
echo =====================================

cd /d "%~dp0"

dotnet publish Employee_Management_System.csproj -c Release -o publish

if errorlevel 1 (
echo Publish Failed
pause
exit /b
)

echo =====================================
echo Uploading To MonsterASP...
echo =====================================

"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" ^
-verb:sync ^
-source:contentPath="%cd%\publish" ^
-dest:contentPath="site75728",computerName="https://site75728.siteasp.net:8172/msdeploy.axd?site=site75728",userName="site75728",password="Kd5_6J%%eh3Y=",authType="Basic" ^
-enableRule:AppOffline ^
-allowUntrusted

echo =====================================
echo DONE!
echo =====================================

pause
