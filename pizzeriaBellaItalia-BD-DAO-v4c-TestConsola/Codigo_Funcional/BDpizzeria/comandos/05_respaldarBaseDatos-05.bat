@echo off
setlocal
if not exist respaldo mkdir respaldo
set BACKUP_FILE=%~dp0respaldo\PizzeriaDB_Backup.bak
echo Generando respaldo en %BACKUP_FILE%...
sqlcmd -S (localdb)\MSSQLLocalDB -Q "BACKUP DATABASE [PizzeriaDB] TO DISK='%BACKUP_FILE%' WITH INIT;"
if %ERRORLEVEL% equ 0 (echo Respaldo realizado correctamente.) else (echo Error en el respaldo.)
endlocal
pause
