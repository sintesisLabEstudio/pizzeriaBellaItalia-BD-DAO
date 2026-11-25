USE PizzeriaDB;
GO

SET IDENTITY_INSERT dbo.Clientes ON;

INSERT INTO dbo.Clientes (IdCliente, Cliente, Telefono)
VALUES 
(1, 'Carlos Mendoza', '987654321'),
(2, 'Ana Gomez', '912345678'),
(3, 'Diego Torres', '955551122');

SET IDENTITY_INSERT dbo.Clientes OFF;
GO

SET IDENTITY_INSERT dbo.Pedidos ON;

INSERT INTO dbo.Pedidos (
    IdPedido, IdCliente, Cliente, Telefono, TamanoNombre, PrecioBaseTamano, 
    MasaNombre, RecargoMasa, QuesoExtra, Pepperoni, Champinones, Aceitunas, 
    Tocino, EsDelivery, AplicaConvenioEstudiante, CostoIngredientes, 
    SubtotalPizza, MontoDescuento, CostoDelivery, NetoAfecto, MontoIva, TotalPagar
)
VALUES 
(
    1, 1, 'Carlos Mendoza', '987654321', 'Mediana', 8500, 
    'Borde de Queso', 1500, 1, 1, 0, 0, 
    0, 0, 1, 1600, 
    11600, 1160, 0, 10440, 1983, 12423
),
(
    2, 2, 'Ana Gomez', '912345678', 'Familiar', 12000, 
    'Tradicional', 0, 0, 0, 1, 1, 
    1, 1, 0, 1600, 
    13600, 0, 2000, 15600, 2964, 18564
),
(
    3, 3, 'Diego Torres', '955551122', 'Personal', 5000, 
    'Delgada', 0, 1, 0, 0, 0, 
    0, 0, 0, 800, 
    5800, 0, 0, 5800, 1102, 6902
);

SET IDENTITY_INSERT dbo.Pedidos OFF;
GO
