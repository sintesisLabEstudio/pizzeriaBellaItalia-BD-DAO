@echo off
chcp 65001 >nul
setlocal

title Crear base de datos - Pizzeria Bella Italia

cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "SCRIPT=creaBaseDatosPizzeria.sql"

echo ============================================================
echo  CREACION DE LA BASE DE DATOS - PIZZERIA BELLA ITALIA
echo ============================================================
echo  Servidor : %SERVIDOR%
echo  Script   : %SCRIPT%
echo  Carpeta  : %~dp0
echo ============================================================
echo.

if not exist "%SCRIPT%" (
    echo [ERROR] No se encontro el script "%SCRIPT%" en esta carpeta.
    goto :error
)

where sqlcmd >nul 2>&1
if errorlevel 1 (
    set "SQLCMD=C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"
    if not exist "%SQLCMD%" (
        echo [ERROR] No se encontro sqlcmd en el PATH ni en la ruta habitual.
        echo         Instale las herramientas de linea de comandos de SQL Server
        echo         o ejecute el script manualmente desde SQL Server Management Studio.
        goto :error
    )
) else (
    set "SQLCMD=sqlcmd"
)

echo Ejecutando script SQL...
echo.
"%SQLCMD%" -S "%SERVIDOR%" -E -b -i "%SCRIPT%"
if errorlevel 1 goto :error

echo.
echo ============================================================
echo  BASE DE DATOS CREADA CORRECTAMENTE
echo ============================================================
echo.
echo Archivos en la carpeta del proyecto:
if exist "%~dp0pizzeriaDB.mdf" (echo   [OK] pizzeriaDB.mdf) else (echo   [X]  pizzeriaDB.mdf NO encontrado)
if exist "%~dp0pizzeriaDB.ldf" (echo   [OK] pizzeriaDB.ldf) else (echo   [X]  pizzeriaDB.ldf NO encontrado)
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
