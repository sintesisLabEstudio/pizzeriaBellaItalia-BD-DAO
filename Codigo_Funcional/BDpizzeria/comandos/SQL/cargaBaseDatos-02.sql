-- ============================================================================
-- CARGA / CONEXION DE LA BASE DE DATOS - PIZZERIA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Motor    : SQL Server / LocalDB  (localdb)\MSSQLLocalDB
-- Base     : PizzeriaDB  (nombre que usa Dominio/ConexionBD.cs)
-- Archivos : pizzeriaDB.mdf / pizzeriaDB.ldf
--
-- La ruta NO va escrita aqui: la calcula cargaBaseDatos-02.bat y la pasa con
-- -v DataDir="..." de modo que el script funciona en cualquier equipo y
-- cualquier carpeta (sin rutas absolutas ni numeros de version).
--
-- Ademas de adjuntar, corrige el caso frecuente de una BD ya registrada en la
-- instancia pero apuntando a una ruta vieja que ya no existe: la desadjunta
-- y la vuelve a adjuntar desde la ruta real, conservando los datos.
-- ============================================================================

SET NOCOUNT ON;

DECLARE @DataDir NVARCHAR(400) = N'$(DataDir)';
DECLARE @Origen  NVARCHAR(20)  = N'$(Origen)';
DECLARE @Mdf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.mdf';
DECLARE @Ldf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.ldf';
DECLARE @Actual  NVARCHAR(500);
DECLARE @ExisteMdf INT = 0;
DECLARE @ExisteLdf INT = 0;
DECLARE @Sql NVARCHAR(MAX);

IF @Origen <> N'bat' OR LEN(@DataDir) < 3
    RAISERROR('Ejecute este script desde cargaBaseDatos-02.bat, que calcula la ruta de los archivos.', 16, 1);
ELSE
BEGIN
    -- Ruta fisica con la que la instancia tiene registrada la base (si existe).
    SELECT @Actual = physical_name
    FROM sys.master_files
    WHERE database_id = DB_ID(N'PizzeriaDB') AND file_id = 1;

    -- Si apunta a otra carpeta (por ejemplo una version anterior), se libera y se readjunta aqui.
    IF @Actual IS NOT NULL AND @Actual <> @Mdf
    BEGIN
        PRINT 'PizzeriaDB estaba registrada en otra ruta: ' + @Actual;
        PRINT 'Se desconectara y se adjuntara desde: ' + @Mdf;
        BEGIN TRY
            ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            EXEC master.dbo.sp_detach_db @dbname = N'PizzeriaDB';
        END TRY
        BEGIN CATCH
            PRINT 'No se pudo desconectar con sp_detach_db: ' + ERROR_MESSAGE();
            BEGIN TRY
                DROP DATABASE [PizzeriaDB];
            END TRY
            BEGIN CATCH
                RAISERROR('No se pudo liberar PizzeriaDB. Cierre la aplicacion y vuelva a intentar.', 16, 1);
            END CATCH
        END CATCH
    END

    IF DB_ID(N'PizzeriaDB') IS NULL
    BEGIN
        EXEC master.dbo.xp_fileexist @Mdf, @ExisteMdf OUTPUT;
        EXEC master.dbo.xp_fileexist @Ldf, @ExisteLdf OUTPUT;

        IF @ExisteMdf = 0
            RAISERROR('No existe pizzeriaDB.mdf en la carpeta indicada por el .bat.', 16, 1);
        ELSE
        BEGIN
            IF @ExisteLdf = 1
                SET @Sql = N'CREATE DATABASE [PizzeriaDB] ON (FILENAME = N''' + @Mdf + N'''), (FILENAME = N''' + @Ldf + N''') FOR ATTACH;';
            ELSE
                SET @Sql = N'CREATE DATABASE [PizzeriaDB] ON (FILENAME = N''' + @Mdf + N''') FOR ATTACH_REBUILD_LOG;';

            BEGIN TRY
                EXEC sp_executesql @Sql;
                PRINT 'Base de datos PizzeriaDB adjuntada correctamente.';
            END TRY
            BEGIN CATCH
                PRINT 'Fallo ATTACH con el log existente: ' + ERROR_MESSAGE();
                SET @Sql = N'CREATE DATABASE [PizzeriaDB] ON (FILENAME = N''' + @Mdf + N''') FOR ATTACH_REBUILD_LOG;';
                BEGIN TRY
                    EXEC sp_executesql @Sql;
                    PRINT 'Base de datos PizzeriaDB adjuntada (log reconstruido).';
                END TRY
                BEGIN CATCH
                    RAISERROR('No se pudo adjuntar PizzeriaDB. Ejecute creaBaseDatosP-01.bat.', 16, 1);
                END CATCH
            END CATCH
        END
    END
    ELSE
        PRINT 'PizzeriaDB ya estaba conectada en la ruta correcta.';

    -- La aplicacion abre varias conexiones (pool): nunca dejarla en SINGLE_USER.
    IF DB_ID(N'PizzeriaDB') IS NOT NULL
        ALTER DATABASE [PizzeriaDB] SET MULTI_USER;

    SELECT
        DB_NAME(DB_ID(N'PizzeriaDB')) AS BaseDatos,
        f.name           AS ArchivoLogico,
        f.type_desc      AS Tipo,
        f.physical_name  AS RutaFisica
    FROM sys.master_files f
    WHERE f.database_id = DB_ID(N'PizzeriaDB');

    IF OBJECT_ID(N'PizzeriaDB.dbo.Pedidos', N'U') IS NOT NULL
        SELECT COUNT(*) AS PedidosGuardados FROM PizzeriaDB.dbo.Pedidos;
    IF OBJECT_ID(N'PizzeriaDB.dbo.Clientes', N'U') IS NOT NULL
        SELECT COUNT(*) AS ClientesRegistrados FROM PizzeriaDB.dbo.Clientes;
END
GO
