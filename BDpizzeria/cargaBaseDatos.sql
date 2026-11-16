-- ============================================================================
-- CARGA / CONEXIÓN DE LA BASE DE DATOS - PIZZERÍA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Motor    : SQL Server / LocalDB
-- Servidor : (localdb)\MSSQLLocalDB
-- Base     : PizzeriaDB
-- Archivos : pizzeriaDB.mdf y pizzeriaDB.ldf en esta misma carpeta
--
-- Este script NO crea la base desde cero: adjunta (attaches) los archivos
-- existentes a la instancia de LocalDB, para que la aplicación pueda conectarse
-- con su cadena de conexión:
--
--   Server=(localdb)\MSSQLLocalDB;Database=PizzeriaDB;Integrated Security=True;
--
-- Uso:
--   - Si la base ya está conectada, solo verifica su estado.
--   - Si no está conectada, la adjunta desde esta carpeta.
--   - Si los archivos no existen, indica ejecutar creaBaseDatosP.bat.
-- ============================================================================

SET NOCOUNT ON;

DECLARE @DataDir NVARCHAR(400) = N'C:\desarrollo\Csoft\App_WinForms_Pizzeria_Bella_Italia_BD\Codigo_Funcional\BDpizzeria';
DECLARE @Mdf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.mdf';
DECLARE @Ldf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.ldf';

DECLARE @ExisteMdf INT = 0;
DECLARE @ExisteLdf INT = 0;
EXEC master.dbo.xp_fileexist @Mdf, @ExisteMdf OUTPUT;
EXEC master.dbo.xp_fileexist @Ldf, @ExisteLdf OUTPUT;

-- ----------------------------------------------------------------------------
-- 1) Si la base ya está conectada, no se hace nada
-- ----------------------------------------------------------------------------
IF DB_ID(N'PizzeriaDB') IS NOT NULL
BEGIN
    PRINT 'La base PizzeriaDB ya esta conectada en la instancia.';
END
-- ----------------------------------------------------------------------------
-- 2) Si no está conectada, se adjunta desde esta carpeta
-- ----------------------------------------------------------------------------
ELSE
BEGIN
    IF @ExisteMdf = 0
    BEGIN
        PRINT 'ERROR: No se encontro el archivo: ' + @Mdf;
        PRINT 'Ejecute primero creaBaseDatosP.bat para crear la base de datos.';
        ;THROW 50001, 'No existe pizzeriaDB.mdf en la carpeta del proyecto.', 1;
    END

    DECLARE @Sql NVARCHAR(MAX);

    IF @ExisteLdf = 1
    BEGIN
        SET @Sql =
            N'CREATE DATABASE [PizzeriaDB]
              ON (FILENAME = N''' + @Mdf + N'''),
                 (FILENAME = N''' + @Ldf + N''')
              FOR ATTACH;';
    END
    ELSE
    BEGIN
        -- Sin archivo de log: SQL Server lo reconstruye
        SET @Sql =
            N'CREATE DATABASE [PizzeriaDB]
              ON (FILENAME = N''' + @Mdf + N''')
              FOR ATTACH_REBUILD_LOG;';
    END

    BEGIN TRY
        EXEC sp_executesql @Sql;
        PRINT 'Base de datos PizzeriaDB conectada correctamente.';
    END TRY
    BEGIN CATCH
        PRINT 'No se pudo adjuntar con el log existente. Detalle: ' + ERROR_MESSAGE();

        -- Segundo intento: reconstruir el log
        BEGIN TRY
            SET @Sql =
                N'CREATE DATABASE [PizzeriaDB]
                  ON (FILENAME = N''' + @Mdf + N''')
                  FOR ATTACH_REBUILD_LOG;';
            EXEC sp_executesql @Sql;
            PRINT 'Base de datos PizzeriaDB conectada (log reconstruido).';
        END TRY
        BEGIN CATCH
            PRINT 'ERROR: No se pudo conectar la base de datos.';
            PRINT 'Si los archivos estan danados, ejecute creaBaseDatosP.bat.';
            ;THROW;
        END CATCH
    END CATCH
END

-- Asegurar que la base quede disponible para varios usuarios
IF DB_ID(N'PizzeriaDB') IS NOT NULL
    ALTER DATABASE [PizzeriaDB] SET MULTI_USER;
GO

-- ----------------------------------------------------------------------------
-- 3) Verificación de la conexión
-- ----------------------------------------------------------------------------
IF DB_ID(N'PizzeriaDB') IS NOT NULL
    EXEC(N'
        USE [PizzeriaDB];

        SELECT
            DB_NAME()        AS BaseDatos,
            f.name           AS ArchivoLogico,
            f.type_desc      AS Tipo,
            f.physical_name  AS RutaFisica
        FROM sys.master_files f
        WHERE f.database_id = DB_ID();

        SELECT
            OBJECT_NAME(OBJECT_ID(N''dbo.Pedidos'')) AS Tabla,
            COUNT(*) AS Columnas
        FROM sys.columns
        WHERE object_id = OBJECT_ID(N''dbo.Pedidos'');

        SELECT COUNT(*) AS PedidosGuardados FROM dbo.Pedidos;
    ');
ELSE
    PRINT 'La base PizzeriaDB no esta disponible.';
GO
