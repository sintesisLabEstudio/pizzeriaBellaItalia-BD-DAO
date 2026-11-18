@echo off
chcp 65001 >nul
setlocal
title Crear base de datos - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\creaBaseDatosPizzeria-01.sql"
set "SCRIPT_CLIENTES=%SCRIPTDIR%\actualizaBaseDatosClientes-01b.sql"

echo ============================================================
echo  CREACION DE LA BASE DE DATOS - PIZZERIA BELLA ITALIA
echo ============================================================

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en la carpeta SQL.
    goto :error
)
if not exist "%SCRIPT_CLIENTES%" (
    echo [ERROR] No se encontro el script %SCRIPT_CLIENTES% en la carpeta SQL.
    goto :error
)

rem ------------------------------------------------------------------
rem Los .mdf/.ldf se crean en la carpeta padre (BDpizzeria).
rem La ruta se resuelve en runtime: sin rutas absolutas ni numeros de version.
rem ------------------------------------------------------------------
set "DATADIR=%~dp0..\.."
for %%I in ("%DATADIR%") do set "DATADIR=%%~fI"

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
rem LocalDB: crear/iniciar si hace falta.
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
echo.
echo Creando la base de datos y las tablas...
"%SQLCMD%" -S "%SERVIDOR%" -E -b -i "%SCRIPT%" -v DataDir="%DATADIR%" Origen=bat
if errorlevel 1 goto :error

echo.
echo Aplicando la relacion Clientes - Pedidos (IdCliente)...
"%SQLCMD%" -S "%SERVIDOR%" -E -b -i "%SCRIPT_CLIENTES%"
if errorlevel 1 goto :error

echo.
echo ============================================================
echo  BASE DE DATOS CREADA CORRECTAMENTE
echo ============================================================
echo Archivos generados en %DATADIR%:
if exist "%DATADIR%\pizzeriaDB.mdf" (echo   [OK] pizzeriaDB.mdf) else (echo   [X]  pizzeriaDB.mdf NO encontrado)
if exist "%DATADIR%\pizzeriaDB.ldf" (echo   [OK] pizzeriaDB.ldf) else (echo   [X]  pizzeriaDB.ldf NO encontrado)
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  ERROR: no se pudo crear la base de datos.
echo  Revise los mensajes anteriores.
echo ============================================================
pause
exit /b 1
