@echo off
setlocal
echo Listando estructura y contenido de tablas...
sqlcmd -S (localdb)\MSSQLLocalDB -d PizzeriaDB -Q "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo';"
sqlcmd -S (localdb)\MSSQLLocalDB -d PizzeriaDB -Q "SELECT * FROM dbo.Clientes; SELECT * FROM dbo.Pedidos;"
endlocal
pause
