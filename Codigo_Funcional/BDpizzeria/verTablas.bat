@echo off
chcp 65001 >nul
setlocal

title Ver tablas y datos - Pizzeria Bella Italia

cd /d "%~dp0"

set "SERVIDOR=(localdb)\MSSQLLocalDB"
set "BASE=PizzeriaDB"

echo ============================================================
echo  TABLAS Y DATOS - PIZZERIA BELLA ITALIA
echo ============================================================
echo  Servidor : %SERVIDOR%
echo  Base     : %BASE%
echo  Carpeta  : %~dp0
echo ============================================================
echo.

where sqlcmd >nul 2>&1
if errorlevel 1 (
    set "SQLCMD=C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"
    if not exist "%SQLCMD%" (
        echo [ERROR] No se encontro sqlcmd en el PATH ni en la ruta habitual.
        echo         Instale las herramientas de linea de comandos de SQL Server.
        goto :error
    )
) else (
    set "SQLCMD=sqlcmd"
)

echo 1) Listado de tablas con cantidad de filas
echo ------------------------------------------
"%SQLCMD%" -S "%SERVIDOR%" -E -d "%BASE%" -b -W -s "|" -Q "SET NOCOUNT ON; SELECT t.name AS Tabla, SUM(p.rows) AS Filas FROM sys.tables t LEFT JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1) WHERE t.is_ms_shipped = 0 GROUP BY t.name ORDER BY t.name;"
if errorlevel 1 (
    echo.
    echo [ERROR] No se pudo leer la base "%BASE%".
    echo         Ejecute cargaBaseDatos.bat para conectar la base de datos.
    goto :error
)

echo.
echo 2) Estructura y datos de cada tabla
echo ------------------------------------
"%SQLCMD%" -S "%SERVIDOR%" -E -d "%BASE%" -b -W -s "|" -Q "SET NOCOUNT ON; DECLARE @tabla NVARCHAR(200), @objectId INT, @sql NVARCHAR(MAX); DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT name, object_id FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY name; OPEN cur; FETCH NEXT FROM cur INTO @tabla, @objectId; WHILE @@FETCH_STATUS = 0 BEGIN PRINT ''; PRINT '=== ' + @tabla + ' ==='; SET @sql = N'SELECT * FROM dbo.' + QUOTENAME(@tabla) + N';'; EXEC sp_executesql @sql; SET @sql = N'SELECT c.column_id AS Orden, c.name AS Columna, t.name AS Tipo, c.max_length AS Largo, c.is_nullable AS AdmiteNull FROM sys.columns c JOIN sys.types t ON t.user_type_id = c.user_type_id WHERE c.object_id = @objectId;'; EXEC sp_executesql @sql, N'@objectId INT', @objectId = @objectId; FETCH NEXT FROM cur INTO @tabla, @objectId; END CLOSE cur; DEALLOCATE cur;"
if errorlevel 1 (
    echo.
    echo [ERROR] Fallo al mostrar el contenido de las tablas.
    goto :error
)

echo.
echo ============================================================
echo  FIN - Tablas y datos mostrados
echo ============================================================
pause
exit /b 0

:error
echo.
echo ============================================================
echo  ERROR: no se pudieron mostrar las tablas y sus datos.
echo  Revise los mensajes anteriores.
echo ============================================================
pause
exit /b 1
