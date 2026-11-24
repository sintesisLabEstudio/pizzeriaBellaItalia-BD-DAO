-- Adjunta PizzeriaDB si no esta adjuntada y la deja en MULTI_USER.
-- Busca PizzeriaDB.mdf y PizzeriaDB_log.ldf en la carpeta de datos por defecto de la instancia
-- LocalDB, que es donde los crea 01_creaBaseDatosP-01.bat. Lo ejecuta 02_cargaBaseDatos-02.bat.
-- Si sus archivos estan en otra carpeta, cambie el valor de @ruta.

USE master;
GO

IF DB_ID(N'PizzeriaDB') IS NULL
BEGIN
    DECLARE @ruta NVARCHAR(260) = CONVERT(NVARCHAR(260), SERVERPROPERTY('InstanceDefaultDataPath'));
    IF RIGHT(@ruta, 1) <> N'\' SET @ruta = @ruta + N'\';

    DECLARE @sql NVARCHAR(MAX) =
        N'CREATE DATABASE [PizzeriaDB] ON (FILENAME = N''' + @ruta + N'PizzeriaDB.mdf''), '
      + N'(FILENAME = N''' + @ruta + N'PizzeriaDB_log.ldf'') FOR ATTACH;';

    EXEC sys.sp_executesql @sql;
    PRINT 'Base PizzeriaDB adjuntada desde: ' + @ruta;
END
ELSE
    PRINT 'La base PizzeriaDB ya estaba adjuntada.';
GO

ALTER DATABASE [PizzeriaDB] SET MULTI_USER WITH ROLLBACK IMMEDIATE;
GO
