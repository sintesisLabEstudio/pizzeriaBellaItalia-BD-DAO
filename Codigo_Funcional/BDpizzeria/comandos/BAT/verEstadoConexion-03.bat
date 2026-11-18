@echo off
chcp 65001 >nul
setlocal
title Estado de la conexion - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\verEstadoConexion-03.sql"

echo ============================================================
echo  ESTADO DE LA CONEXION - PIZZERIA BELLA ITALIA
echo ============================================================

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en la carpeta SQL.
    goto :error
)

rem ------------------------------------------------------------------
rem Ruta de los archivos: carpeta padre (BDpizzeria) o esta misma.
rem ------------------------------------------------------------------
set "DATADIR=%~dp0..\.."
for %%I in ("%DATADIR%") do set "DATADIR=%%~fI"
if not exist "%DATADIR%\pizzeriaDB.mdf" (
    if exist "%~dp0..\pizzeriaDB.mdf" set "DATADIR=%~dp0.."
)

set "SQLCMD="
where sqlcmd >nul 2>&1
if not errorlevel 1 set "SQLCMD=sqlcmd"
if not defined SQLCMD (
    for %%P in (
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn\SQLCMD.EXE"
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\160\Tools\Binn\SQLCMD.EXE"
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\150\Tools\Binn\SQLCMD.EXE"
        "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\140\Tools\Binn\SQLCMD.EXE"
    ) do if exist "%%~P" set "SQLCMD=%%~P"
)
if not defined SQLCMD (
    echo [ERROR] No se encontro sqlcmd.
    goto :error
)

set "LOCALDB="
where sqllocaldb >nul 2>&1
if not errorlevel 1 set "LOCALDB=sqllocaldb"
if not defined LOCALDB (
    for %%P in (
        "C:\Program Files\Microsoft SQL Server\150\Tools\Binn\SqlLocalDB.exe"
        "C:\Program Files\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe"
        "C:\Program Files\Microsoft SQL Server\170\Tools\Binn\SqlLocalDB.exe"
    ) do if exist "%%~P" set "LOCALDB=%%~P"
)

echo.
echo 1) Instancia LocalDB
if defined LOCALDB (
    "%LOCALDB%" info MSSQLLocalDB 2>nul
    if errorlevel 1 echo    La instancia MSSQLLocalDB no existe o esta detenida.
) else (
    echo    sqllocaldb no disponible en este equipo.
)

echo.
echo 2) Archivos esperados en %DATADIR%
if exist "%DATADIR%\pizzeriaDB.mdf" (echo    [OK] pizzeriaDB.mdf) else (echo    [X] pizzeriaDB.mdf NO encontrado)
if exist "%DATADIR%\pizzeriaDB.ldf" (echo    [OK] pizzeriaDB.ldf) else (echo    [X] pizzeriaDB.ldf NO encontrado)
if exist "%DATADIR%\respaldo" (echo    [OK] carpeta respaldo presente) else (echo    [-] carpeta respaldo aun no creada)

rem Se inicia la instancia si estaba detenida (LocalDB se apaga solo).
if defined LOCALDB "%LOCALDB%" start MSSQLLocalDB >nul 2>&1

echo.
echo 3) Base de datos en la instancia
"%SQLCMD%" -S "%SERVIDOR%" -E -b -W -i "%SCRIPT%"
if errorlevel 1 (
    echo.
    echo RESULTADO: REVISAR - vea el mensaje anterior.
    echo Si la base no esta conectada, ejecute cargaBaseDatos-02.bat
    goto :error
)

echo.
echo ============================================================
echo  RESULTADO: TODO OK
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  RESULTADO: REVISAR
echo ============================================================
pause
exit /b 1
