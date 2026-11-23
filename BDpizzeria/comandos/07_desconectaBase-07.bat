@echo off
setlocal
echo Desconectando la base de datos de la instancia local...
sqlcmd -S (localdb)\MSSQLLocalDB -d master -Q "ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db 'PizzeriaDB';"
if %ERRORLEVEL% equ 0 (echo Base desconectada exitosamente.) else (echo Error al desconectar la base.)
endlocal
pause
