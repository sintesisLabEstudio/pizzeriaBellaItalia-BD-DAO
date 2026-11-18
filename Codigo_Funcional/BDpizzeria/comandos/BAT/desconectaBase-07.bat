@echo off
chcp 65001 >nul
setlocal
title Desconectar base de datos - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\desconectaBase-07.sql"

echo ============================================================
echo  DESCONEXION DE LA BASE DE DATOS - PIZZERIA BELLA ITALIA
echo ============================================================

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en la carpeta SQL.
    goto :error
)

rem Los .mdf/.ldf estan en la carpeta padre (BDpizzeria).
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
echo Desconectando la base de datos...
echo.

"%SQLCMD%" -S "%SERVIDOR%" -E -b -i "%SCRIPT%"
if errorlevel 1 goto :error

echo.
echo ============================================================
echo  BASE DE DATOS DESCONECTADA
echo ============================================================
echo Los archivos se conservan en %DATADIR%:
if exist "%DATADIR%\pizzeriaDB.mdf" (echo   [OK] pizzeriaDB.mdf) else (echo   [X]  pizzeriaDB.mdf NO encontrado)
if exist "%DATADIR%\pizzeriaDB.ldf" (echo   [OK] pizzeriaDB.ldf) else (echo   [X]  pizzeriaDB.ldf NO encontrado)
echo.
echo Para volver a conectarla ejecute: cargaBaseDatos-02.bat
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  ERROR: no se pudo desconectar la base de datos.
echo  Revise los mensajes anteriores.
echo ============================================================
pause
exit /b 1
