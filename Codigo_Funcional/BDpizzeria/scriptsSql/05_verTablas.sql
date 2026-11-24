-- Muestra la estructura de las tablas dbo y su contenido.
-- Equivale a 04_verTablas-04.bat.

USE PizzeriaDB;
GO

SET NOCOUNT ON;

PRINT '=== ESTRUCTURA DE LAS TABLAS dbo ===';
SELECT TABLE_NAME AS Tabla, COLUMN_NAME AS Columna, DATA_TYPE AS Tipo,
       ORDINAL_POSITION AS Orden, IS_NULLABLE AS AdmiteNull
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME, ORDINAL_POSITION;

PRINT '=== CLIENTES ===';
SELECT IdCliente, Cliente, Telefono
FROM dbo.Clientes
ORDER BY IdCliente DESC;

PRINT '=== PEDIDOS ===';
SELECT IdPedido, IdCliente, Cliente, Telefono, TamanoNombre,
       MasaNombre, EsDelivery, AplicaConvenioEstudiante, SubtotalPizza,
       MontoDescuento, CostoDelivery, MontoIva, TotalPagar
FROM dbo.Pedidos
ORDER BY IdPedido DESC;
GO
