@echo off
setlocal
echo Vaciando tablas y reiniciando contadores IDENTITY...
sqlcmd -S (localdb)\MSSQLLocalDB -d PizzeriaDB -Q "ALTER TABLE dbo.Pedidos DROP CONSTRAINT FK_Pedidos_Clientes; TRUNCATE TABLE dbo.Pedidos; TRUNCATE TABLE dbo.Clientes; ALTER TABLE dbo.Pedidos ADD CONSTRAINT FK_Pedidos_Clientes FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente);"
if %ERRORLEVEL% equ 0 (echo Base vaciada y contadores reiniciados.) else (echo Error al vaciar la base.)
endlocal
pause
