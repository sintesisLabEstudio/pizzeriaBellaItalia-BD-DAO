-- ============================================================================
-- CREACIÓN DE BASE DE DATOS - PIZZERÍA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Motor    : SQL Server / LocalDB
-- Servidor : (localdb)\MSSQLLocalDB
-- Base     : PizzeriaDB
-- Archivos : pizzeriaDB.mdf y pizzeriaDB.ldf EN ESTA MISMA CARPETA
--
-- Alineado con la app (PedidoDAO.InsertarPedido / CalculadoraPedido), que
-- inserta en dbo.Pedidos las columnas:
--   Cliente, Telefono, TamanoNombre, PrecioBaseTamano, MasaNombre, RecargoMasa,
--   QuesoExtra, Pepperoni, Champinones, Aceitunas, Tocino, EsDelivery,
--   AplicaConvenioEstudiante, CostoIngredientes, SubtotalPizza, MontoDescuento,
--   CostoDelivery, NetoAfecto, MontoIva, TotalPagar
--
-- Si la base ya existe pero sus archivos están en otra carpeta, se respalda
-- (respaldo_pizzeriaDB.bak) y se vuelve a crear aquí.
-- ============================================================================

-- Ruta donde se crearán los archivos de datos.
-- Si mueves el proyecto de carpeta, actualiza esta ruta y vuelve a ejecutar.
DECLARE @DataDir NVARCHAR(400) = N'C:\desarrollo\Csoft\App_WinForms_Pizzeria_Bella_Italia_BD\Codigo_Funcional\BDpizzeria';
DECLARE @Mdf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.mdf';
DECLARE @Ldf     NVARCHAR(500) = @DataDir + N'\pizzeriaDB.ldf';
DECLARE @Bak     NVARCHAR(500) = @DataDir + N'\respaldo_pizzeriaDB.bak';
DECLARE @Actual  NVARCHAR(500);

SELECT @Actual = physical_name
FROM sys.master_files
WHERE database_id = DB_ID(N'PizzeriaDB')
  AND file_id = 1;

-- ----------------------------------------------------------------------------
-- 1) Si la base existe en otra ubicación: respaldar y recrear en esta carpeta
-- ----------------------------------------------------------------------------
IF DB_ID(N'PizzeriaDB') IS NOT NULL AND @Actual <> @Mdf
BEGIN
    PRINT 'La base PizzeriaDB existe en otra ubicación: ' + ISNULL(@Actual, '(desconocida)');
    PRINT 'Se creará un respaldo y se recreará en la carpeta del proyecto...';

    BACKUP DATABASE [PizzeriaDB]
        TO DISK = @Bak
        WITH INIT,
             NAME = N'PizzeriaDB - respaldo antes de mover archivos';

    ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [PizzeriaDB];
END
GO

-- ----------------------------------------------------------------------------
-- 2) Crear la base de datos con archivos en la carpeta del proyecto
-- ----------------------------------------------------------------------------
IF DB_ID(N'PizzeriaDB') IS NULL
BEGIN
    DECLARE @DataDir NVARCHAR(400) = N'C:\desarrollo\Csoft\App_WinForms_Pizzeria_Bella_Italia_BD\Codigo_Funcional\BDpizzeria';
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
-- 3) Crear la tabla Pedidos si no existe
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Pedidos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedidos
    (
        IdPedido                  INT IDENTITY(1,1) NOT NULL
                                    CONSTRAINT PK_Pedidos PRIMARY KEY,

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
-- 4) Índices útiles para consultas
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_Cliente' AND object_id = OBJECT_ID(N'dbo.Pedidos'))
    CREATE INDEX IX_Pedidos_Cliente ON dbo.Pedidos(Cliente);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_FechaCreacion' AND object_id = OBJECT_ID(N'dbo.Pedidos'))
    CREATE INDEX IX_Pedidos_FechaCreacion ON dbo.Pedidos(FechaCreacion);
GO

-- ----------------------------------------------------------------------------
-- 5) Verificación: base, tabla y ubicación física de los archivos
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
