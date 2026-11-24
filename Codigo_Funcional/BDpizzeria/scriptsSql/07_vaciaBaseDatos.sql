-- Vacia Clientes y Pedidos y reinicia los contadores IDENTITY.
-- Equivale a 06_vaciaBaseDatos-06.bat (que ademas pide confirmacion con choice).
-- Se elimina y se vuelve a crear la llave foranea porque TRUNCATE no puede
-- ejecutarse mientras exista una FK que referencie la tabla.

SET XACT_ABORT ON;
BEGIN TRANSACTION;

ALTER TABLE dbo.Pedidos DROP CONSTRAINT FK_Pedidos_Clientes;

TRUNCATE TABLE dbo.Pedidos;
TRUNCATE TABLE dbo.Clientes;

DBCC CHECKIDENT ('dbo.Pedidos', RESEED, 0);
DBCC CHECKIDENT ('dbo.Clientes', RESEED, 0);

ALTER TABLE dbo.Pedidos ADD CONSTRAINT FK_Pedidos_Clientes
    FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente);

COMMIT TRANSACTION;
GO
