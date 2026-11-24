@echo off
setlocal EnableExtensions
set "BASE_DIR=%~dp0"
set "SQL_FILE=%BASE_DIR%01_Estructura_PizzeriaDB.sql"

where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ERROR: no se encontro sqlcmd en el PATH.
    echo Instale SQL Server Command Line Utilities y vuelva a intentar.
    goto :fin
)

if not exist "%SQL_FILE%" (
    echo ERROR: no se encontro el archivo 01_Estructura_PizzeriaDB.sql junto a este .bat.
    goto :fin
)

echo Creando la base PizzeriaDB y sus tablas si no existen...
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -i "%SQL_FILE%"
if errorlevel 1 (
    echo Error al crear la base o las tablas.
) else (
    echo Base y tablas listas.
)

:fin
endlocal
pause
