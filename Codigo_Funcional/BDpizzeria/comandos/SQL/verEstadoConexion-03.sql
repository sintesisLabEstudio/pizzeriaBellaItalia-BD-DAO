-- ============================================================================
-- DIAGNOSTICO DE CONEXION - PIZZERIA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Comprueba: instancia, si PizzeriaDB esta adjunta, su estado (state_desc),
-- modo de acceso ( MULTI_USER / SINGLE_USER ), ruta fisica del .mdf y filas.
-- Lo invoca verEstadoConexion-03.bat. No modifica nada.
-- ============================================================================

SET NOCOUNT ON;

SELECT CAST(@@SERVERNAME AS NVARCHAR(40)) AS Servidor,
       CAST(COALESCE(SERVERPROPERTY('InstanceName'), N'(default)') AS NVARCHAR(25)) AS Instancia;

SELECT CASE WHEN DB_ID(N'PizzeriaDB') IS NULL
            THEN 'NO CONECTADA' ELSE 'CONECTADA' END AS EstadoBD;

IF DB_ID(N'PizzeriaDB') IS NOT NULL
BEGIN
    SELECT d.name        AS BaseDatos,
           d.state_desc  AS Estado,
           d.user_access_desc AS Acceso,
           f.name        AS ArchivoLogico,
           f.type_desc   AS Tipo,
           f.physical_name AS RutaFisica
    FROM sys.databases d
    LEFT JOIN sys.master_files f
           ON f.database_id = d.database_id AND f.file_id = 1
    WHERE d.name = N'PizzeriaDB';

    IF OBJECT_ID(N'PizzeriaDB.dbo.Pedidos', N'U') IS NOT NULL
        SELECT COUNT(*) AS Pedidos FROM PizzeriaDB.dbo.Pedidos;
    IF OBJECT_ID(N'PizzeriaDB.dbo.Clientes', N'U') IS NOT NULL
        SELECT COUNT(*) AS Clientes FROM PizzeriaDB.dbo.Clientes;
END

IF DB_ID(N'PizzeriaDB') IS NULL
    RAISERROR('PizzeriaDB NO esta conectada. Ejecute cargaBaseDatos-02.bat.', 16, 1);
GO
