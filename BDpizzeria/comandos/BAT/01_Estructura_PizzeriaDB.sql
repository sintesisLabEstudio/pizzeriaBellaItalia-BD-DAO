-- Crea la base PizzeriaDB y sus tablas (Clientes y Pedidos).
-- Es idempotente: se puede ejecutar varias veces sin error ni perdida de datos.
-- Lo ejecuta 01_creaBaseDatosP-01.bat con sqlcmd.

IF DB_ID(N'PizzeriaDB') IS NULL
    CREATE DATABASE PizzeriaDB;
GO

USE PizzeriaDB;
GO

IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes (
        IdCliente INT IDENTITY(1,1) NOT NULL,
        Cliente NVARCHAR(100) NOT NULL,
        Telefono NVARCHAR(30) NOT NULL,
        CONSTRAINT PK_Clientes PRIMARY KEY CLUSTERED (IdCliente ASC),
        CONSTRAINT UQ_Clientes_Cliente_Telefono UNIQUE NONCLUSTERED (Cliente ASC, Telefono ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.Pedidos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedidos (
        IdPedido INT IDENTITY(1,1) NOT NULL,
        IdCliente INT NOT NULL,
        Cliente NVARCHAR(100) NOT NULL,
        Telefono NVARCHAR(30) NOT NULL,
        TamanoNombre NVARCHAR(50) NOT NULL,
        PrecioBaseTamano INT NOT NULL,
        MasaNombre NVARCHAR(50) NOT NULL,
        RecargoMasa INT NOT NULL,
        QuesoExtra BIT NOT NULL,
        Pepperoni BIT NOT NULL,
        Champinones BIT NOT NULL,
        Aceitunas BIT NOT NULL,
        Tocino BIT NOT NULL,
        EsDelivery BIT NOT NULL,
        AplicaConvenioEstudiante BIT NOT NULL,
        CostoIngredientes INT NOT NULL,
        SubtotalPizza INT NOT NULL,
        MontoDescuento INT NOT NULL,
        CostoDelivery INT NOT NULL,
        NetoAfecto INT NOT NULL,
        MontoIva INT NOT NULL,
        TotalPagar INT NOT NULL,
        CONSTRAINT PK_Pedidos PRIMARY KEY CLUSTERED (IdPedido ASC),
        CONSTRAINT FK_Pedidos_Clientes FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente)
    );
END
GO
