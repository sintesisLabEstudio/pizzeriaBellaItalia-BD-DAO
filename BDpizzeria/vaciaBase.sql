-- 1. Borramos los pedidos primero (por si están amarrados a los clientes)
DELETE FROM dbo.Pedidos;
DBCC CHECKIDENT ('dbo.Pedidos', RESEED, 0);

-- 2. Borramos los clientes
DELETE FROM dbo.Clientes;
DBCC CHECKIDENT ('dbo.Clientes', RESEED, 0);