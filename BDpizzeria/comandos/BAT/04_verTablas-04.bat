@echo off
setlocal EnableExtensions

where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ERROR: no se encontro sqlcmd en el PATH.
    echo Instale SQL Server Command Line Utilities y vuelva a intentar.
    goto :fin
)

echo Listando estructura y contenido de tablas...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d PizzeriaDB -b -Q "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' ORDER BY TABLE_NAME, ORDINAL_POSITION;"
if errorlevel 1 goto :error
sqlcmd -S "(localdb)\MSSQLLocalDB" -d PizzeriaDB -b -Q "SELECT * FROM dbo.Clientes; SELECT * FROM dbo.Pedidos;"
if errorlevel 1 goto :error
goto :fin

:error
echo Error al leer la base. Verifique con 03_verEstadoConexion-03.bat.

:fin
endlocal
pause
