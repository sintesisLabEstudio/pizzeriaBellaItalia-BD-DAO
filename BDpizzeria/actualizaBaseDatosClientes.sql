-- ============================================================================
-- MIGRACIÓN: TABLA dbo.Clientes Y RELACIÓN CON dbo.Pedidos
-- ----------------------------------------------------------------------------
-- Motor    : SQL Server / LocalDB
-- Servidor : (localdb)\MSSQLLocalDB
-- Base     : PizzeriaDB
--
-- Qué hace (solo DDL, sin triggers ni procedimientos):
--   1) Crea la tabla dbo.Clientes con:
--        - IdCliente  INT IDENTITY(1,1)  -> clave primaria, única y autoincrementable
--        - Cliente    NVARCHAR(100)      -> mismas características que dbo.Pedidos.Cliente
--        - Telefono   NVARCHAR(30)       -> mismas características que dbo.Pedidos.Telefono
--   2) Agrega dbo.Pedidos.IdCliente (INT, NOT NULL) con CLAVE FORÁNEA a dbo.Clientes.
--   3) Migra los datos ya guardados: crea un cliente por cada par
--      (Cliente, Telefono) existente en Pedidos y asigna su IdCliente.
--
-- El script es IDEMPOTENTE: se puede ejecutar varias veces sin error.
-- ============================================================================

USE [PizzeriaDB];
GO

SET NOCOUNT ON;
GO

-- ----------------------------------------------------------------------------
-- 1) Tabla dbo.Clientes
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        IdCliente                  INT IDENTITY(1,1) NOT NULL
                                    CONSTRAINT PK_Clientes PRIMARY KEY,

        -- Datos del cliente (mismas características que en dbo.Pedidos)
        Cliente                    NVARCHAR(100)     NOT NULL,
        Telefono                   NVARCHAR(30)      NOT NULL
    );
    PRINT 'Tabla dbo.Clientes creada.';
END
ELSE
    PRINT 'La tabla dbo.Clientes ya existe.';
GO

-- ----------------------------------------------------------------------------
-- 2) Columna dbo.Pedidos.IdCliente (se agrega nullable para poder migrar)
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'dbo.Pedidos') AND name = N'IdCliente')
BEGIN
    ALTER TABLE dbo.Pedidos ADD IdCliente INT NULL;
    PRINT 'Columna dbo.Pedidos.IdCliente agregada.';
END
ELSE
    PRINT 'La columna dbo.Pedidos.IdCliente ya existe.';
GO

-- ----------------------------------------------------------------------------
-- 3) Migración de los datos existentes
--    Crea un cliente por cada par (Cliente, Telefono) y lo asigna al pedido.
-- ----------------------------------------------------------------------------
INSERT INTO dbo.Clientes (Cliente, Telefono)
SELECT DISTINCT p.Cliente, p.Telefono
FROM dbo.Pedidos p
WHERE NOT EXISTS (SELECT 1 FROM dbo.Clientes c
                  WHERE c.Cliente = p.Cliente AND c.Telefono = p.Telefono);
GO

UPDATE p
SET    p.IdCliente = c.IdCliente
FROM   dbo.Pedidos p
JOIN   dbo.Clientes c ON c.Cliente = p.Cliente AND c.Telefono = p.Telefono
WHERE  p.IdCliente IS NULL;
GO

-- ----------------------------------------------------------------------------
-- 4) IdCliente pasa a ser NOT NULL (obligatorio en cada pedido)
-- ----------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'dbo.Pedidos') AND name = N'IdCliente' AND is_nullable = 1)
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Pedidos WHERE IdCliente IS NULL)
    BEGIN
        -- No debería ocurrir: se inserta un cliente 'SIN CLIENTE ASIGNADO' por si acaso.
        IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE Cliente = N'SIN CLIENTE ASIGNADO')
            INSERT INTO dbo.Clientes (Cliente, Telefono) VALUES (N'SIN CLIENTE ASIGNADO', N'SIN TELEFONO');

        UPDATE p
        SET    p.IdCliente = c.IdCliente
        FROM   dbo.Pedidos p
        CROSS JOIN dbo.Clientes c
        WHERE  p.IdCliente IS NULL AND c.Cliente = N'SIN CLIENTE ASIGNADO';
    END

    ALTER TABLE dbo.Pedidos ALTER COLUMN IdCliente INT NOT NULL;
    PRINT 'dbo.Pedidos.IdCliente ahora es NOT NULL.';
END
GO

-- ----------------------------------------------------------------------------
-- 5) Clave foránea Pedidos.IdCliente -> Clientes.IdCliente
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Pedidos_Clientes')
BEGIN
    ALTER TABLE dbo.Pedidos ADD CONSTRAINT FK_Pedidos_Clientes
        FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente)
        ON UPDATE CASCADE
        ON DELETE NO ACTION;
    PRINT 'Clave foranea FK_Pedidos_Clientes creada.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedidos_IdCliente' AND object_id = OBJECT_ID(N'dbo.Pedidos'))
    CREATE INDEX IX_Pedidos_IdCliente ON dbo.Pedidos(IdCliente);
GO

-- ----------------------------------------------------------------------------
-- 6) Verificación
-- ----------------------------------------------------------------------------
SELECT
    OBJECT_NAME(object_id)                        AS Tabla,
    COUNT(*)                                      AS Columnas
FROM sys.columns
WHERE object_id IN (OBJECT_ID(N'dbo.Clientes'), OBJECT_ID(N'dbo.Pedidos'))
GROUP BY object_id;

SELECT
    fk.name                                   AS ClaveForanea,
    OBJECT_NAME(fk.parent_object_id)          AS TablaHijo,
    cpa.name                                  AS ColumnaHijo,
    OBJECT_NAME(fk.referenced_object_id)      AS TablaPadre,
    cref.name                                 AS ColumnaPadre
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.columns cpa  ON cpa.object_id = fkc.parent_object_id  AND cpa.column_id = fkc.parent_column_id
JOIN sys.columns cref ON cref.object_id = fkc.referenced_object_id AND cref.column_id = fkc.referenced_column_id;

SELECT
    p.IdPedido,
    p.IdCliente,
    p.Cliente,
    p.Telefono,
    p.TotalPagar
FROM dbo.Pedidos p
ORDER BY p.IdPedido;

SELECT
    c.IdCliente,
    c.Cliente,
    c.Telefono,
    COUNT(p.IdPedido) AS PedidosDelCliente
FROM dbo.Clientes c
LEFT JOIN dbo.Pedidos p ON p.IdCliente = c.IdCliente
GROUP BY c.IdCliente, c.Cliente, c.Telefono
ORDER BY c.IdCliente;
GO
