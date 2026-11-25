@echo off
setlocal
set DB_NAME=PizzeriaDB
echo Configurando acceso de la base %DB_NAME%...
sqlcmd -S (localdb)\MSSQLLocalDB -d master -Q "ALTER DATABASE [%DB_NAME%] SET MULTI_USER WITH ROLLBACK IMMEDIATE;"
if %ERRORLEVEL% equ 0 (echo Base configurada en MULTI_USER.) else (echo Error al configurar la base.)
endlocal
pause
