@echo off
setlocal
echo Comprobando estado de conexion y diagnostico...
sqlcmd -S (localdb)\MSSQLLocalDB -Q "SELECT name, state_desc, user_access_desc FROM sys.databases WHERE name = 'PizzeriaDB';"
sqlcmd -S (localdb)\MSSQLLocalDB -d PizzeriaDB -Q "SELECT 'Clientes' AS Tabla, COUNT(*) AS Filas FROM dbo.Clientes UNION ALL SELECT 'Pedidos', COUNT(*) FROM dbo.Pedidos;"
endlocal
pause
