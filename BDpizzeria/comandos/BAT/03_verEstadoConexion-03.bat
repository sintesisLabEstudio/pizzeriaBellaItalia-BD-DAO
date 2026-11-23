@echo off
setlocal EnableExtensions
set "BASE_DIR=%~dp0"
set "SQL_FILE=%BASE_DIR%03_verEstadoConexion.sql"

where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ERROR: no se encontro sqlcmd en el PATH.
    echo Instale SQL Server Command Line Utilities y vuelva a intentar.
    goto :fin
)

if not exist "%SQL_FILE%" (
    echo ERROR: no se encontro el archivo 03_verEstadoConexion.sql junto a este .bat.
    goto :fin
)

echo Comprobando estado de conexion y diagnostico...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d master -b -i "%SQL_FILE%"

:fin
endlocal
pause
