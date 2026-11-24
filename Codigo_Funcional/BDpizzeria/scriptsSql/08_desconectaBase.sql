-- Desadjunta PizzeriaDB de LocalDB conservando los archivos .mdf y .ldf.
-- Equivale a 07_desconectaBase-07.bat.
-- SINGLE_USER WITH ROLLBACK IMMEDIATE cierra las conexiones activas.

USE master;
GO

ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
EXEC sp_detach_db 'PizzeriaDB';
GO
