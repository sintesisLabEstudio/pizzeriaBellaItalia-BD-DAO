-- ============================================================================
-- TABLAS, ESTRUCTURA Y DATOS - PIZZERIA BELLA ITALIA
-- Lo invoca verTablas-04.bat con -d PizzeriaDB. Solo lectura.
-- ============================================================================

SET NOCOUNT ON;

-- 1) Listado de tablas con cantidad de filas
SELECT t.name AS Tabla, SUM(p.rows) AS Filas
FROM sys.tables t
LEFT JOIN sys.partitions p
       ON p.object_id = t.object_id AND p.index_id IN (0,1)
WHERE t.is_ms_shipped = 0
GROUP BY t.name
ORDER BY t.name;
GO

-- 2) Estructura y datos de cada tabla
DECLARE @tabla NVARCHAR(200), @objectId INT, @sql NVARCHAR(MAX);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT name, object_id FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY name;
OPEN cur;
FETCH NEXT FROM cur INTO @tabla, @objectId;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '';
    PRINT '=== ' + @tabla + ' ===';
    SET @sql = N'SELECT * FROM dbo.' + QUOTENAME(@tabla) + N';';
    EXEC sp_executesql @sql;
    SET @sql = N'SELECT c.column_id AS Orden, c.name AS Columna, t.name AS Tipo, '
             + N'c.max_length AS Largo, c.is_nullable AS AdmiteNull '
             + N'FROM sys.columns c JOIN sys.types t ON t.user_type_id = c.user_type_id '
             + N'WHERE c.object_id = @objectId;';
    EXEC sp_executesql @sql, N'@objectId INT', @objectId = @objectId;
    FETCH NEXT FROM cur INTO @tabla, @objectId;
END
CLOSE cur;
DEALLOCATE cur;
GO
