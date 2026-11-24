-- Adjunta PizzeriaDB si no esta adjuntada y la deja en MULTI_USER.
-- Busca pizzeriaDB.mdf y pizzeriaDB.ldf en la carpeta real del proyecto (v4B).
-- Lo ejecuta 02_cargaBaseDatos-02.bat. Si el proyecto se mueve, cambie el valor de @ruta.

USE master;
GO

IF DB_ID(N'PizzeriaDB') IS NULL
BEGIN
    DECLARE @ruta NVARCHAR(260) = N'C:\desarrollo\Csoft\pizzeriaBellaItalia-BD-DAO-v4B\Codigo_Funcional\BDpizzeria';
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
