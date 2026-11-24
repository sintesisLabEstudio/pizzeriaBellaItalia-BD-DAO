-- Diagnostico de PizzeriaDB: instancia, estado, modo de acceso, archivos y filas por tabla.
-- Equivale a 03_verEstadoConexion-03.bat y a comandos\BAT\03_verEstadoConexion.sql.

SET NOCOUNT ON;

SELECT @@SERVERNAME AS Instancia, SERVERPROPERTY('ProductVersion') AS Version;

DECLARE @estado NVARCHAR(60), @acceso NVARCHAR(60);
SELECT @estado = state_desc, @acceso = user_access_desc
FROM sys.databases
WHERE name = N'PizzeriaDB';

IF @estado IS NULL
BEGIN
    PRINT 'RESULTADO: ERROR, la base PizzeriaDB no existe o esta desadjuntada. Ejecute 01 o 02.';
    RETURN;
END

SELECT name AS Base, state_desc AS Estado, user_access_desc AS Modo
FROM sys.databases
WHERE name = N'PizzeriaDB';

SELECT name AS Archivo, type_desc AS Tipo, physical_name AS Ruta
FROM sys.master_files
WHERE database_id = DB_ID(N'PizzeriaDB');

DECLARE @clientes INT, @pedidos INT;
BEGIN TRY
    EXEC sys.sp_executesql
        N'SELECT @c = COUNT(*) FROM PizzeriaDB.dbo.Clientes; SELECT @p = COUNT(*) FROM PizzeriaDB.dbo.Pedidos;',
        N'@c INT OUTPUT, @p INT OUTPUT',
        @c = @clientes OUTPUT, @p = @pedidos OUTPUT;

    SELECT 'Clientes' AS Tabla, @clientes AS Filas
    UNION ALL
    SELECT 'Pedidos', @pedidos;
END TRY
BEGIN CATCH
    PRINT 'RESULTADO: ERROR, no se pudieron leer las tablas Clientes y Pedidos. Ejecute 01.';
    RETURN;
END CATCH

IF @estado = N'ONLINE' AND @acceso = N'MULTI_USER'
    PRINT 'RESULTADO: TODO OK, base ONLINE y MULTI_USER';
ELSE
    PRINT 'RESULTADO: REVISAR, estado ' + @estado + ' y modo ' + @acceso;
GO
