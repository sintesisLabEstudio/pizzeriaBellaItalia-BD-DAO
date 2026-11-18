-- ============================================================================
-- VERIFICACION PREVIA AL RESPALDO - PIZZERIA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- No genera archivos .bak. Solo:
--   1) comprueba que PizzeriaDB este conectada,
--   2) hace CHECKPOINT para volcar a disco las paginas pendientes (asi la
--      copia posterior de los .mdf/.ldf es mas consistente),
--   3) muestra las rutas fisicas reales de los archivos.
-- Lo invoca respaldarBaseDatos-05.bat antes de copiar.
-- ============================================================================

SET NOCOUNT ON;

IF DB_ID(N'PizzeriaDB') IS NULL
    RAISERROR('PizzeriaDB no esta conectada. Ejecute primero cargaBaseDatos-02.bat.', 16, 1);
ELSE
BEGIN
    USE [PizzeriaDB];
    CHECKPOINT;

    PRINT 'Base verificada. Se pueden copiar los archivos originales.';
    SELECT f.name AS ArchivoLogico,
           f.type_desc AS Tipo,
           f.physical_name AS RutaFisica
    FROM sys.master_files f
    WHERE f.database_id = DB_ID(N'PizzeriaDB')
    ORDER BY f.file_id;
END
GO
