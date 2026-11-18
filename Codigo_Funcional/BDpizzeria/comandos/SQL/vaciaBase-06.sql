-- ============================================================================
-- VACIAR DATOS - PIZZERIA BELLA ITALIA
-- ----------------------------------------------------------------------------
-- Borra primero los pedidos (tienen FK a Clientes) y luego los clientes.
-- Reinicia los IDENTITY para que el proximo pedido/cliente empiece en 1.
-- No borra la base ni las tablas: solo los registros.
-- La ejecuta vaciaBaseDatos-06.bat (no depende de rutas ni de equipos).
-- ============================================================================
USE [PizzeriaDB];
GO
SET NOCOUNT ON;

DELETE FROM dbo.Pedidos;
DBCC CHECKIDENT ('dbo.Pedidos', RESEED, 0);

DELETE FROM dbo.Clientes;
DBCC CHECKIDENT ('dbo.Clientes', RESEED, 0);
GO
