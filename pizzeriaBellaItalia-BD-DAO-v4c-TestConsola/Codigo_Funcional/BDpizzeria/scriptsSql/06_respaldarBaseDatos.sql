-- Genera un respaldo .bak de PizzeriaDB.
-- Equivale a 05_respaldarBaseDatos-05.bat.
-- Si la ruta no existe, crear antes la carpeta (el .bat lo hace con mkdir).

DECLARE @BackupFile NVARCHAR(260) =
    N'C:\desarrollo\Csoft\pizzeriaBellaItalia-BD-DAO-v4B\Codigo_Funcional\BDpizzeria\respaldo\PizzeriaDB_Backup.bak';

BACKUP DATABASE [PizzeriaDB] TO DISK = @BackupFile WITH INIT;
GO
