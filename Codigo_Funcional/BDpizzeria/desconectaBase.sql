-- ============================================================================
-- DESCONEXIÓN (DETACH) DE LA BASE DE DATOS - PIZZERÍA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Motor    : SQL Server / LocalDB
-- Servidor : (localdb)\MSSQLLocalDB
-- Base     : PizzeriaDB
--
-- Este script DESADJUNTA la base de la instancia, pero NO borra los archivos:
--   pizzeriaDB.mdf y pizzeriaDB.ldf se mantienen en la carpeta del proyecto.
--
-- Después de ejecutarlo, la aplicación no podrá conectarse hasta que la base
-- se vuelva a cargar con cargaBaseDatos.bat (o hasta que la app la adjunte
-- automáticamente al guardar un pedido).
-- ============================================================================

SET NOCOUNT ON;

IF DB_ID(N'PizzeriaDB') IS NULL
BEGIN
    PRINT 'La base PizzeriaDB no esta conectada. Nada que desconectar.';
END
ELSE
BEGIN
    BEGIN TRY
        -- Cierra conexiones abiertas y deja la base en modo de un solo usuario
        ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

        -- Desadjunta la base conservando los archivos .mdf y .ldf
        EXEC master.dbo.sp_detach_db @dbname = N'PizzeriaDB';

        PRINT 'Base de datos PizzeriaDB desconectada correctamente.';
        PRINT 'Los archivos pizzeriaDB.mdf y pizzeriaDB.ldf se conservan en la carpeta.';
    END TRY
    BEGIN CATCH
        PRINT 'ERROR al desconectar la base de datos: ' + ERROR_MESSAGE();
        ;THROW;
    END CATCH
END
GO

-- ----------------------------------------------------------------------------
-- Verificación: NULL = desconectada / numero = conectada
-- ----------------------------------------------------------------------------
SELECT DB_ID(N'PizzeriaDB') AS IdBaseConectada;
GO
