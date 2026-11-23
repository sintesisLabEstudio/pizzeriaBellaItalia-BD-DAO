@echo off
setlocal EnableExtensions
set "BASE_DIR=%~dp0"
set "SQL_FILE=%BASE_DIR%02_cargaBaseDatos.sql"

where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ERROR: no se encontro sqlcmd en el PATH.
    echo Instale SQL Server Command Line Utilities y vuelva a intentar.
    goto :fin
)

if not exist "%SQL_FILE%" (
    echo ERROR: no se encontro el archivo 02_cargaBaseDatos.sql junto a este .bat.
    goto :fin
)

echo Adjuntando PizzeriaDB si hace falta y dejandola en MULTI_USER...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d master -b -i "%SQL_FILE%"
if errorlevel 1 (
    echo Error al cargar la base. Si es un entorno nuevo, ejecute antes 01_creaBaseDatosP-01.bat.
) else (
    echo Base cargada y en MULTI_USER.
)

:fin
endlocal
pause
