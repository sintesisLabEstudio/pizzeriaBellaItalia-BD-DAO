@echo off
chcp 65001 >nul
setlocal
title Ver registros de las copias en respaldo - Pizzeria Bella Italia
cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "SCRIPT=verRegistrosTablas.sql"
set "VISORDIR=%CD%\_visor"

echo ============================================================
echo  VER TABLAS Y REGISTROS DE LAS COPIAS EN BD
echo ============================================================

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script %SCRIPT% en esta carpeta.
    goto :error
)
if not exist "%CD%\pizzeriaDB.mdf" (
    echo [ERROR] No hay copia de pizzeriaDB.mdf en esta carpeta.
    echo         Ejecute ..\comandos\BAT\respaldarBaseDatos-05.bat para generarla.
    goto :error
)

set "CONLOG=0"
if exist "%CD%\pizzeriaDB.ldf" set "CONLOG=1"

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

rem ------------------------------------------------------------------
rem Se trabaja sobre COPIAS de las copias para no bloquear ni borrar
rem los archivos originales de la carpeta respaldo.
rem ------------------------------------------------------------------
if exist "%VISORDIR%" rmdir /s /q "%VISORDIR%" >nul 2>&1
mkdir "%VISORDIR%" >nul 2>&1
if not exist "%VISORDIR%" (
    echo [ERROR] No se pudo crear la carpeta temporal:
    echo         %VISORDIR%
    goto :error
)

copy /Y "%CD%\pizzeriaDB.mdf" "%VISORDIR%\visor.mdf" >nul
if errorlevel 1 goto :error_copia
if "%CONLOG%"=="1" (
    copy /Y "%CD%\pizzeriaDB.ldf" "%VISORDIR%\visor.ldf" >nul
    if errorlevel 1 goto :error_copia
)

echo Copias en respaldo: %CD%\pizzeriaDB.mdf
echo BD temporal    : %VISORDIR%
echo Con archivo log: %CONLOG%
echo.
"%SQLCMD%" -S "%SERVIDOR%" -E -b -W -i "%SCRIPT%" -v VisorDir="%VISORDIR%" ConLog=%CONLOG%
set "RC=%errorlevel%"

rmdir /s /q "%VISORDIR%" >nul 2>&1
if exist "%VISORDIR%" echo [AVISO] Quedaron archivos en %VISORDIR%

if not "%RC%"=="0" goto :error

echo.
echo ============================================================
echo  FIN - Las copias en respaldo siguen intactas y la base en vivo
echo        PizzeriaDB no fue modificada.
echo ============================================================
pause
exit /b 0

:error_copia
echo [ERROR] No se pudieron copiar los archivos a la carpeta temporal.
rmdir /s /q "%VISORDIR%" >nul 2>&1
goto :error

:error
echo.
echo ============================================================
echo  ERROR: no se pudo mostrar el contenido de las copias en respaldo.
echo ============================================================
pause
exit /b 1
