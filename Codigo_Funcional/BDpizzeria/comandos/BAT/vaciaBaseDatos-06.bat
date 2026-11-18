@echo off
chcp 65001 >nul
setlocal
title Vaciar base de datos - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "BASE=PizzeriaDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\vaciaBase-06.sql"

color 0c
echo ============================================================
echo  ADVERTENCIA: ESTO BORRA TODOS LOS PEDIDOS Y CLIENTES
echo ============================================================
echo.
set /p "CONFIRMA=Escriba SI para continuar (cualquier otra tecla cancela): "
if /I not "%CONFIRMA%"=="SI" (
    echo Operacion cancelada.
    pause
    exit /b 0
)
color 07

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

echo.
echo Servidor : %SERVIDOR%
echo Base     : %BASE%
echo Vaciando las tablas...
"%SQLCMD%" -S "%SERVIDOR%" -E -d "%BASE%" -b -i "%SCRIPT%"
if errorlevel 1 goto :error

echo.
echo ============================================================
echo  BASE DE DATOS LIMPIA Y REINICIADA A CERO
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  ERROR: no se pudo vaciar la base de datos.
echo  Si la base no esta conectada, ejecute primero cargaBaseDatos-02.bat.
echo ============================================================
pause
exit /b 1
