@echo off
chcp 65001 >nul
setlocal
title Cargar base de datos - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "BASE=PizzeriaDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\cargaBaseDatos-02.sql"

echo ============================================================
echo  CARGA / CONEXION DE LA BASE DE DATOS - PIZZERIA BELLA ITALIA
echo ============================================================

rem ------------------------------------------------------------------
rem Los .mdf/.ldf viven en la carpeta padre (BDpizzeria), no aqui.
rem La ruta se resuelve en runtime: funciona aunque se mueva el proyecto.
rem ------------------------------------------------------------------
set "DATADIR=%~dp0..\.."
for %%I in ("%DATADIR%") do set "DATADIR=%%~fI"
if not exist "%DATADIR%\pizzeriaDB.mdf" (
    if exist "%~dp0..\pizzeriaDB.mdf" set "DATADIR=%~dp0.."
)
if not exist "%DATADIR%\pizzeriaDB.mdf" (
    echo [ERROR] No se encontro pizzeriaDB.mdf en:
    echo         %DATADIR%
    echo         Coloque el archivo en BDpizzeria o en la carpeta comandos.
    goto :error
)
if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en la carpeta SQL.
    goto :error
)

rem ------------------------------------------------------------------
rem sqlcmd: primero el PATH, luego las versiones habituales del ODBC.
rem ------------------------------------------------------------------
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

rem ------------------------------------------------------------------
rem LocalDB: crear/iniciar si hace falta (recupera instancias detenidas).
rem ------------------------------------------------------------------
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
if defined LOCALDB (
    "%LOCALDB%" info MSSQLLocalDB >nul 2>&1
    if errorlevel 1 "%LOCALDB%" create MSSQLLocalDB -s >nul 2>&1
    "%LOCALDB%" start MSSQLLocalDB >nul 2>&1
)

echo Servidor : %SERVIDOR%
echo Archivos : %DATADIR%
echo Adjuntando la base de datos...
echo.

"%SQLCMD%" -S "%SERVIDOR%" -E -b -i "%SCRIPT%" -v DataDir="%DATADIR%" Origen=bat
if errorlevel 1 goto :error

echo.
echo ============================================================
echo  BASE DE DATOS CONECTADA CORRECTAMENTE
echo ============================================================
echo  Cadena que usa la aplicacion (Dominio\ConexionBD.cs):
echo  Server=(localdb)\MSSQLLocalDB;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=True;
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  ERROR: no se pudo cargar/conectar la base de datos.
echo  Revise los mensajes anteriores.
echo ============================================================
pause
exit /b 1
