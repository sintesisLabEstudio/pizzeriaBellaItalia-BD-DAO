@echo off
setlocal EnableExtensions

where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ERROR: no se encontro sqlcmd en el PATH.
    echo Instale SQL Server Command Line Utilities y vuelva a intentar.
    goto :fin
)

choice /c SN /m "Esto BORRA todos los clientes y pedidos. Desea continuar"
if errorlevel 2 (
    echo Operacion cancelada.
    goto :fin
)

echo Vaciando tablas y reiniciando contadores IDENTITY...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d PizzeriaDB -b -Q "SET XACT_ABORT ON; BEGIN TRANSACTION; ALTER TABLE dbo.Pedidos DROP CONSTRAINT FK_Pedidos_Clientes; TRUNCATE TABLE dbo.Pedidos; TRUNCATE TABLE dbo.Clientes; ALTER TABLE dbo.Pedidos ADD CONSTRAINT FK_Pedidos_Clientes FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente); COMMIT TRANSACTION;"
if errorlevel 1 (
    echo Error al vaciar la base. No se aplico ningun cambio.
) else (
    echo Base vaciada y contadores reiniciados.
)

:fin
endlocal
pause
