-- ============================================================================
-- CREACIÓN DE BASE DE DATOS - PIZZERÍA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Motor    : SQL Server / LocalDB
-- Servidor : (localdb)\MSSQLLocalDB
-- Base     : PizzeriaDB
-- Archivos : pizzeriaDB.mdf y pizzeriaDB.ldf en la carpeta BDpizzeria
--            (carpeta padre de comandos). La ruta la pasa el .bat, no va fija aqui.
--
-- Alineado con la app (PedidoDAO.InsertarPedido / CalculadoraPedido), que
-- inserta en dbo.Pedidos las columnas:
--   Cliente, Telefono, TamanoNombre, PrecioBaseTamano, MasaNombre, RecargoMasa,
--   QuesoExtra, Pepperoni, Champinones, Aceitunas, Tocino, EsDelivery,
--   AplicaConvenioEstudiante, CostoIngredientes, SubtotalPizza, MontoDescuento,
--   CostoDelivery, NetoAfecto, MontoIva, TotalPagar
--
-- NOTA: este script crea la base y las tablas dbo.Clientes y dbo.Pedidos
--       (con la FK dbo.Pedidos.IdCliente -> dbo.Clientes.IdCliente).
--       Para una base ya existente, la misma migración se puede aplicar con
--       el script actualizaBaseDatosClientes-01b.sql (idempotente).
--
-- Si la base ya existe pero sus archivos están en otra carpeta, se libera
-- y se vuelve a crear aquí. No se generan archivos .bak.
-- ============================================================================

-- Ruta de los archivos: la calcula creaBaseDatosP-01.bat y la pasa con -v DataDir.
-- No hay rutas absolutas: funciona en cualquier equipo y cualquier carpeta.
DECLARE @DataDir NVARCHAR(400) = N'$(DataDir)';
DECLARE @Origen  NVARCHAR(20)  = N'$(Origen)';
DECLARE @Mdf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.mdf';
DECLARE @Ldf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.ldf';
DECLARE @Actual  NVARCHAR(500);

IF @Origen <> N'bat' OR LEN(@DataDir) < 3
    RAISERROR('Ejecute este script desde creaBaseDatosP-01.bat, que calcula la ruta de los archivos.', 16, 1);
ELSE
BEGIN
    SELECT @Actual = physical_name
    FROM sys.master_files
    WHERE database_id = DB_ID(N'PizzeriaDB')
      AND file_id = 1;

    -- ------------------------------------------------------------------------
    -- 1) Si la base existe en otra ubicación: liberar y recrear aquí
    -- ------------------------------------------------------------------------
    IF DB_ID(N'PizzeriaDB') IS NOT NULL AND @Actual <> @Mdf
    BEGIN
        PRINT 'La base PizzeriaDB existe en otra ubicación: ' + ISNULL(@Actual, '(desconocida)');
        PRINT 'Se liberará y se recreará en la carpeta del proyecto...';

        ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [PizzeriaDB];
    END
END
GO

-- ----------------------------------------------------------------------------
-- 2) Crear la base de datos con archivos en la carpeta del proyecto
-- ----------------------------------------------------------------------------
IF DB_ID(N'PizzeriaDB') IS NULL
BEGIN
    DECLARE @DataDir NVARCHAR(400) = N'$(DataDir)';
    DECLARE @Mdf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.mdf';
    DECLARE @Ldf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.ldf';

    DECLARE @SqlCrear NVARCHAR(MAX) =
        N'CREATE DATABASE [PizzeriaDB]
          ON (NAME = N''PizzeriaDB'',     FILENAME = N''' + @Mdf + N''', SIZE = 8MB, FILEGROWTH = 64MB)
          LOG ON (NAME = N''PizzeriaDB_log'', FILENAME = N''' + @Ldf + N''', SIZE = 8MB, FILEGROWTH = 64MB);';

    EXEC sp_executesql @SqlCrear;
END
GO

USE [PizzeriaDB];
GO

-- ----------------------------------------------------------------------------
-- 3) Crear la tabla Clientes si no existe
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        IdCliente                  INT IDENTITY(1,1) NOT NULL
                                    CONSTRAINT PK_Clientes PRIMARY KEY,

        -- Datos del cliente (mismas características que en dbo.Pedidos)
        Cliente                    NVARCHAR(100)     NOT NULL,
        Telefono                   NVARCHAR(30)      NOT NULL,
        CONSTRAINT UQ_Clientes_Cliente_Telefono UNIQUE (Cliente, Telefono)
    );
END
GO

-- ----------------------------------------------------------------------------
-- 4) Crear la tabla Pedidos si no existe
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Pedidos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedidos
    (
        IdPedido                  INT IDENTITY(1,1) NOT NULL
                                    CONSTRAINT PK_Pedidos PRIMARY KEY,

        -- Relación con el cliente (clave foránea a dbo.Clientes)
        IdCliente                 INT               NOT NULL
                                    CONSTRAINT FK_Pedidos_Clientes
                                    REFERENCES dbo.Clientes (IdCliente)
                                    ON UPDATE CASCADE
                                    ON DELETE NO ACTION,

        -- Datos del cliente
        Cliente                   NVARCHAR(100)     NOT NULL,
        Telefono                  NVARCHAR(30)      NOT NULL,

        -- Configuración de la pizza
        TamanoNombre              NVARCHAR(50)      NOT NULL,
        PrecioBaseTamano          INT               NOT NULL,
        MasaNombre                NVARCHAR(50)      NOT NULL,
        RecargoMasa               INT               NOT NULL,

        -- Ingredientes adicionales (flags)
        QuesoExtra                BIT               NOT NULL CONSTRAINT DF_Pedidos_QuesoExtra   DEFAULT (0),
        Pepperoni                 BIT               NOT NULL CONSTRAINT DF_Pedidos_Pepperoni    DEFAULT (0),
        Champinones               BIT               NOT NULL CONSTRAINT DF_Pedidos_Champinones  DEFAULT (0),
        Aceitunas                 BIT               NOT NULL CONSTRAINT DF_Pedidos_Aceitunas    DEFAULT (0),
        Tocino                    BIT               NOT NULL CONSTRAINT DF_Pedidos_Tocino       DEFAULT (0),

        -- Opciones de entrega y promoción
        EsDelivery                BIT               NOT NULL CONSTRAINT DF_Pedidos_EsDelivery   DEFAULT (0),
        AplicaConvenioEstudiante  BIT               NOT NULL CONSTRAINT DF_Pedidos_Convenio     DEFAULT (0),

        -- Montos calculados por el dominio (PedidoPizza)
        CostoIngredientes         INT               NOT NULL CONSTRAINT DF_Pedidos_CostoIng     DEFAULT (0),
        SubtotalPizza             INT               NOT NULL CONSTRAINT DF_Pedidos_Subtotal     DEFAULT (0),
        MontoDescuento            INT               NOT NULL CONSTRAINT DF_Pedidos_Descuento    DEFAULT (0),
        CostoDelivery             INT               NOT NULL CONSTRAINT DF_Pedidos_CostoDeliv   DEFAULT (0),
        NetoAfecto                INT               NOT NULL CONSTRAINT DF_Pedidos_Neto         DEFAULT (0),
        MontoIva                  INT               NOT NULL CONSTRAINT DF_Pedidos_Iva          DEFAULT (0),
        TotalPagar                INT               NOT NULL CONSTRAINT DF_Pedidos_Total        DEFAULT (0),

        -- Auditoría
        FechaCreacion             DATETIME          NOT NULL CONSTRAINT DF_Pedidos_FechaCreacion DEFAULT (GETDATE())
    );
END
GO

-- ----------------------------------------------------------------------------
-- 5) Unicidad de (Cliente, Telefono) en dbo.Clientes
--    Si la tabla ya existia sin la restriccion, se eliminan duplicados
--    (se conserva el IdCliente menor y se reasignan sus pedidos) y se agrega.
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.objects
               WHERE name = N'UQ_Clientes_Cliente_Telefono' AND type = N'UQ')
BEGIN
    ;WITH ConCanonico AS
    (
        SELECT IdCliente,
               MIN(IdCliente) OVER (PARTITION BY Cliente, Telefono) AS IdCanonico
        FROM dbo.Clientes
    )
    UPDATE p
        SET IdCliente = c.IdCanonico
    FROM dbo.Pedidos p
    JOIN ConCanonico c ON c.IdCliente = p.IdCliente
    WHERE p.IdCliente <> c.IdCanonico;

    ;WITH ConCanonico AS
    (
        SELECT IdCliente,
               MIN(IdCliente) OVER (PARTITION BY Cliente, Telefono) AS IdCanonico
        FROM dbo.Clientes
    )
    DELETE c
    FROM dbo.Clientes c
    JOIN ConCanonico x ON x.IdCliente = c.IdCliente
    WHERE c.IdCliente <> x.IdCanonico;

    ALTER TABLE dbo.Clientes
        ADD CONSTRAINT UQ_Clientes_Cliente_Telefono UNIQUE (Cliente, Telefono);
    PRINT 'Restriccion UQ_Clientes_Cliente_Telefono creada.';
END
GO

-- ----------------------------------------------------------------------------
-- 6) Índices útiles para consultas
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_Cliente' AND object_id = OBJECT_ID(N'dbo.Pedidos'))
    CREATE INDEX IX_Pedidos_Cliente ON dbo.Pedidos(Cliente);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_IdCliente' AND object_id = OBJECT_ID(N'dbo.Pedidos'))
    CREATE INDEX IX_Pedidos_IdCliente ON dbo.Pedidos(IdCliente);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_FechaCreacion' AND object_id = OBJECT_ID(N'dbo.Pedidos'))
    CREATE INDEX IX_Pedidos_FechaCreacion ON dbo.Pedidos(FechaCreacion);
GO

-- ----------------------------------------------------------------------------
-- 7) Verificación: base, tablas y ubicación física de los archivos
-- ----------------------------------------------------------------------------
SELECT
    DB_NAME()        AS BaseDatos,
    f.name           AS ArchivoLogico,
    f.type_desc      AS Tipo,
    f.physical_name  AS RutaFisica
FROM sys.master_files f
WHERE f.database_id = DB_ID();
GO

SELECT
    OBJECT_NAME(OBJECT_ID(N'dbo.Pedidos')) AS Tabla,
    COUNT(*) AS Columnas
FROM sys.columns
WHERE object_id = OBJECT_ID(N'dbo.Pedidos');
GO

SELECT
    OBJECT_NAME(OBJECT_ID(N'dbo.Clientes')) AS Tabla,
    COUNT(*) AS Columnas
FROM sys.columns
WHERE object_id = OBJECT_ID(N'dbo.Clientes');
GO

-- ============================================================================
-- OPCIONAL: recrear la base desde cero (BORRA TODOS LOS DATOS)
-- Descomenta solo si necesitas reiniciar el entorno.
-- ----------------------------------------------------------------------------
-- IF DB_ID(N'PizzeriaDB') IS NOT NULL
-- BEGIN
--     ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--     DROP DATABASE [PizzeriaDB];
-- END
-- GO
-- ============================================================================
