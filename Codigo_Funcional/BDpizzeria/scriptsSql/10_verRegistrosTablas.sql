-- ============================================================================
-- VISOR DE LAS COPIAS DE PIZZERIADB - PIZZERIA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Adjunta como BD TEMPORARIA (PizzeriaDB_Visor) una copia de los archivos
-- pizzeriaDB.mdf/.ldf que estan en esta carpeta respaldo, muestra tablas,
-- estructura y registros, y al final DESADJUNTA esa BD temporal.
--
-- IMPORTANTE: se usa sp_detach_db y NO DROP DATABASE, porque DROP borraria
-- los archivos .mdf/.ldf de la carpeta respaldo (que son el respaldo).
--
-- Lo invoca verRegistrosTablas.bat con:
--   -v VisorDir="ruta\a\_visor" ConLog=1|0
-- ============================================================================

SET NOCOUNT ON;

DECLARE @VisorDir NVARCHAR(500) = N'$(VisorDir)';
DECLARE @ConLog   NVARCHAR(5)   = N'$(ConLog)';
DECLARE @Mdf      NVARCHAR(500) = @VisorDir + N'\visor.mdf';
DECLARE @Ldf      NVARCHAR(500) = @VisorDir + N'\visor.ldf';
DECLARE @Sql      NVARCHAR(MAX);

IF DB_ID(N'PizzeriaDB_Visor') IS NOT NULL
BEGIN
    ALTER DATABASE [PizzeriaDB_Visor] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    EXEC master.dbo.sp_detach_db @dbname = N'PizzeriaDB_Visor';
END

IF @ConLog = N'1'
    SET @Sql = N'CREATE DATABASE [PizzeriaDB_Visor] ON (FILENAME = N''' + @Mdf + N'''), (FILENAME = N''' + @Ldf + N''') FOR ATTACH;'
ELSE
    SET @Sql = N'CREATE DATABASE [PizzeriaDB_Visor] ON (FILENAME = N''' + @Mdf + N''') FOR ATTACH_REBUILD_LOG;';

BEGIN TRY
    EXEC sp_executesql @Sql;
END TRY
BEGIN CATCH
    RAISERROR('No se pudieron adjuntar las copias .mdf/.ldf de la carpeta respaldo.', 16, 1);
END CATCH

PRINT '';
PRINT '=== 1) TABLAS Y CANTIDAD DE FILAS (DE LAS COPIAS EN BD) ===';
SELECT t.name AS Tabla, SUM(p.rows) AS Filas
FROM PizzeriaDB_Visor.sys.tables t
LEFT JOIN PizzeriaDB_Visor.sys.partitions p
       ON p.object_id = t.object_id AND p.index_id IN (0,1)
WHERE t.is_ms_shipped = 0
GROUP BY t.name
ORDER BY t.name;

IF OBJECT_ID(N'PizzeriaDB_Visor.dbo.Clientes', N'U') IS NOT NULL
BEGIN
    PRINT '';
    PRINT '=== 2) ESTRUCTURA DE Clientes ===';
    SELECT c.column_id AS Orden, c.name AS Columna, ty.name AS Tipo,
           c.max_length AS Largo, c.is_nullable AS AdmiteNull
    FROM PizzeriaDB_Visor.sys.columns c
    JOIN PizzeriaDB_Visor.sys.types ty ON ty.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID(N'PizzeriaDB_Visor.dbo.Clientes')
    ORDER BY c.column_id;

    PRINT '';
    PRINT '=== 3) REGISTROS DE Clientes ===';
    SELECT * FROM [PizzeriaDB_Visor].dbo.Clientes;
END

IF OBJECT_ID(N'PizzeriaDB_Visor.dbo.Pedidos', N'U') IS NOT NULL
BEGIN
    PRINT '';
    PRINT '=== 4) ESTRUCTURA DE Pedidos ===';
    SELECT c.column_id AS Orden, c.name AS Columna, ty.name AS Tipo,
           c.max_length AS Largo, c.is_nullable AS AdmiteNull
    FROM PizzeriaDB_Visor.sys.columns c
    JOIN PizzeriaDB_Visor.sys.types ty ON ty.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID(N'PizzeriaDB_Visor.dbo.Pedidos')
    ORDER BY c.column_id;

    PRINT '';
    PRINT '=== 5) REGISTROS DE Pedidos ===';
    SELECT * FROM [PizzeriaDB_Visor].dbo.Pedidos;
END
GO

-- Limpieza: desadjuntar para no borrar los archivos de la carpeta respaldo.
IF DB_ID(N'PizzeriaDB_Visor') IS NOT NULL
BEGIN
    ALTER DATABASE [PizzeriaDB_Visor] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    EXEC master.dbo.sp_detach_db @dbname = N'PizzeriaDB_Visor';
    PRINT '';
    PRINT 'BD temporal desadjuntada. Las copias en respaldo siguen intactas.';
END
GO
