@echo off
chcp 65001 >nul
setlocal
title Respaldar base de datos - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "SCRIPTDIR=%~dp0..\SQL"
for %%I in ("%SCRIPTDIR%") do set "SCRIPTDIR=%%~fI"
set "SCRIPT=%SCRIPTDIR%\respaldarBaseDatos-05.sql"

echo ============================================================
echo  RESPALDO DE LA BASE DE DATOS - PIZZERIA BELLA ITALIA
echo ============================================================

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en la carpeta SQL.
    goto :error
)

rem ------------------------------------------------------------------
rem La BD en vivo esta en la carpeta padre; las copias van en respaldo\.
rem La ruta se calcula aqui: sin rutas absolutas ni numeros de version.
rem ------------------------------------------------------------------
set "DATADIR=%~dp0..\.."
for %%I in ("%DATADIR%") do set "DATADIR=%%~fI"
if not exist "%DATADIR%\pizzeriaDB.mdf" (
    if exist "%~dp0..\pizzeriaDB.mdf" set "DATADIR=%~dp0.."
)
set "BACKUPDIR=%DATADIR%\respaldo"
if not exist "%BACKUPDIR%" mkdir "%BACKUPDIR%" 2>nul
if not exist "%BACKUPDIR%" (
    echo [ERROR] No se pudo crear la carpeta de respaldos:
    echo         %BACKUPDIR%
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
if defined LOCALDB "%LOCALDB%" start MSSQLLocalDB >nul 2>&1

echo Verificando la base de datos antes de copiar...
"%SQLCMD%" -S "%SERVIDOR%" -E -b -W -i "%SCRIPT%"
if errorlevel 1 goto :error

rem ------------------------------------------------------------------
rem Solo se pide confirmacion si YA existen copias (sobrescritura).
rem ------------------------------------------------------------------
set "HAYCOPIAS="
if exist "%BACKUPDIR%\pizzeriaDB.mdf" set "HAYCOPIAS=1"
if exist "%BACKUPDIR%\pizzeriaDB.ldf" set "HAYCOPIAS=1"
if not defined HAYCOPIAS goto :copiar

echo.
echo Ya existen copias en:
echo   %BACKUPDIR%
set "CONFIRMA="
set /p "CONFIRMA=Sobrescribir las copias existentes? (S/N): "
set "CONFIRMA=%CONFIRMA: =%"
if /I not "%CONFIRMA%"=="S" (
    echo Operacion cancelada. Las copias actuales no se modificaron.
    pause
    exit /b 0
)

:copiar
echo.
echo Copiando los archivos originales a la carpeta respaldo...
rem Se reintenta unos segundos: SQL puede mantener el archivo abierto
rem unos instantes despues de la verificacion.
set "COPIA_MDF_OK="
for /L %%N in (1,1,5) do (
    if not defined COPIA_MDF_OK (
        copy /Y "%DATADIR%\pizzeriaDB.mdf" "%BACKUPDIR%\pizzeriaDB.mdf" >nul 2>&1
        if not errorlevel 1 if exist "%BACKUPDIR%\pizzeriaDB.mdf" set "COPIA_MDF_OK=1"
        if not defined COPIA_MDF_OK ping -n 2 127.0.0.1 >nul 2>&1
    )
)
if not defined COPIA_MDF_OK goto :error_copia
echo   [OK] %BACKUPDIR%\pizzeriaDB.mdf

if exist "%DATADIR%\pizzeriaDB.ldf" (
    set "COPIA_LDF_OK="
    for /L %%N in (1,1,5) do (
        if not defined COPIA_LDF_OK (
            copy /Y "%DATADIR%\pizzeriaDB.ldf" "%BACKUPDIR%\pizzeriaDB.ldf" >nul 2>&1
            if not errorlevel 1 if exist "%BACKUPDIR%\pizzeriaDB.ldf" set "COPIA_LDF_OK=1"
            if not defined COPIA_LDF_OK ping -n 2 127.0.0.1 >nul 2>&1
        )
    )
    if not defined COPIA_LDF_OK goto :error_copia
    echo   [OK] %BACKUPDIR%\pizzeriaDB.ldf
) else (
    echo   [-] No habia .ldf en el origen; se omitio.
)

echo.
echo ============================================================
echo  RESPALDO CREADO CORRECTAMENTE
echo ============================================================
echo  Copias de los originales en respaldo:
echo    %BACKUPDIR%\pizzeriaDB.mdf
echo    %BACKUPDIR%\pizzeriaDB.ldf
echo  No se generan archivos .bak.
echo.
echo  Para ver su contenido, abra una CMD en la carpeta respaldo y ejecute:
echo      verRegistrosTablas.bat
echo ============================================================
pause
exit /b 0

:error_copia
echo.
echo ============================================================
echo  ERROR: no se pudieron copiar los archivos originales.
echo  Si la BD tiene los archivos abiertos, ejecute:
echo    1) desconectaBase-07.bat
echo    2) este respaldo otra vez
echo    3) cargaBaseDatos-02.bat
echo ============================================================
pause
exit /b 1

:error
echo.
echo ============================================================
echo  ERROR: no se pudo generar el respaldo.
echo  Si la base no esta conectada, ejecute cargaBaseDatos-02.bat.
echo ============================================================
pause
exit /b 1
