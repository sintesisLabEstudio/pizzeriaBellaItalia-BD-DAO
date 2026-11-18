@echo off
chcp 65001 >nul
setlocal
title Ver tablas y datos - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "BASE=PizzeriaDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\verTablas-04.sql"

echo ============================================================
echo  TABLAS Y DATOS - PIZZERIA BELLA ITALIA
echo ============================================================

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en la carpeta SQL.
    goto :error
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
    echo [ERROR] No se encontro sqlcmd. Instale las herramientas de linea de comandos de SQL Server.
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
if defined LOCALDB "%LOCALDB%" start MSSQLLocalDB >nul 2>&1

echo Servidor : %SERVIDOR%
echo Base     : %BASE%
echo.

"%SQLCMD%" -S "%SERVIDOR%" -E -d "%BASE%" -b -W -s "|" -i "%SCRIPT%"
if errorlevel 1 (
    echo.
    echo [ERROR] No se pudo leer la base "%BASE%".
    echo         Ejecute cargaBaseDatos-02.bat para conectar la base de datos.
    goto :error
)

echo.
echo ============================================================
echo  FIN - Tablas y datos mostrados
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  ERROR: no se pudieron mostrar las tablas y sus datos.
echo  Revise los mensajes anteriores.
echo ============================================================
pause
exit /b 1
