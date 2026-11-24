@echo off
setlocal
set DB_NAME=PizzeriaDB
set MDF_PATH=C:\Users\jvv\Codigo_Funcional\BDpizzeria\pizzeriaDB.mdf
set LDF_PATH=C:\Users\jvv\Codigo_Funcional\BDpizzeria\pizzeriaDB.ldf

echo ========================================================
echo  Reconectando base de datos %DB_NAME% en LocalDB...
echo ========================================================

sqlcmd -S (localdb)\MSSQLLocalDB -d master -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '%DB_NAME%') CREATE DATABASE [%DB_NAME%] ON (FILENAME = '%MDF_PATH%'), (FILENAME = '%LDF_PATH%') FOR ATTACH;"

if %ERRORLEVEL% neq 0 (
    echo [ERROR] No se pudo adjuntar la base de datos. Verifica que las rutas .mdf y .ldf sean correctas.
    pause
    exit /b %ERRORLEVEL%
)

echo Asegurando modo MULTI_USER...
sqlcmd -S (localdb)\MSSQLLocalDB -d master -Q "ALTER DATABASE [%DB_NAME%] SET MULTI_USER WITH ROLLBACK IMMEDIATE;"

echo Verificando estado de conexion...
sqlcmd -S (localdb)\MSSQLLocalDB -d %DB_NAME% -Q "SELECT name AS BaseDatos, state_desc AS Estado, user_access_desc AS Acceso FROM sys.databases WHERE name = '%DB_NAME%';"

echo [OK] Proceso finalizado.
endlocal
pause