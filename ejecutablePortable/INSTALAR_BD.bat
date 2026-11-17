@echo off
setlocal
set "DATADIR=%~dp0BD"

echo ============================================================
echo  Pizzeria Bella Italia - Preparar base de datos
echo ============================================================
echo.
echo Instancia : (localdb)\MSSQLLocalDB
echo Base      : PizzeriaDB
echo Datos     : %DATADIR%
echo.

if not exist "%DATADIR%" mkdir "%DATADIR%"

sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -v DataDir="%DATADIR%" -i "%~dp0BD\creaBaseDatosPizzeria.sql"
if errorlevel 1 (
    echo.
    echo ERROR: no se pudo crear la base de datos.
    echo Verifique que LocalDB y sqlcmd esten instalados y que tenga permisos.
    pause
    exit /b 1
)

echo.
echo Base de datos PizzeriaDB lista.
echo Ya puede ejecutar PizzeriaBellaItalia.exe
pause
