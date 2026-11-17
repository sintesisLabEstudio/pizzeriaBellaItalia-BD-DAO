@echo off
color 0c
echo =======================================================
echo    ADVERTENCIA: ESTO BORRARA TODOS LOS DATOS
echo =======================================================
echo Presiona cualquier tecla para continuar o cierra esta ventana para cancelar.
pause >nul

:: Configura aquí el nombre de tu servidor y base de datos
set SERVIDOR=.\SQLEXPRESS
set BASE_DATOS=PizzeriaBellaItalia

:: Variables con los comandos SQL
:: Se borran primero los pedidos (por si tienen llave foránea a clientes) y luego los clientes
set SQL_BORRAR_PEDIDOS="DELETE FROM dbo.Pedidos; DBCC CHECKIDENT ('dbo.Pedidos', RESEED, 0);"
set SQL_BORRAR_CLIENTES="DELETE FROM dbo.Clientes; DBCC CHECKIDENT ('dbo.Clientes', RESEED, 0);"

echo.
echo Vaciando tabla de Pedidos...
sqlcmd -S %SERVIDOR% -d %BASE_DATOS% -E -Q %SQL_BORRAR_PEDIDOS%

echo Vaciando tabla de Clientes...
sqlcmd -S %SERVIDOR% -d %BASE_DATOS% -E -Q %SQL_BORRAR_CLIENTES%

echo.
color 0a
echo =======================================================
echo    BASE DE DATOS LIMPIA Y REINICIADA A CERO.
echo =======================================================
pause