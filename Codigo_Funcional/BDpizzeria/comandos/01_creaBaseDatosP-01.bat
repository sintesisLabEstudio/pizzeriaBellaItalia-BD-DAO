@echo off
setlocal
set DB_NAME=PizzeriaDB
echo Creando base de datos %DB_NAME%...
sqlcmd -S (localdb)\MSSQLLocalDB -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '%DB_NAME%') CREATE DATABASE [%DB_NAME%];"
if %ERRORLEVEL% equ 0 (echo Base creada con exito.) else (echo Error al crear la base.)
endlocal
pause
