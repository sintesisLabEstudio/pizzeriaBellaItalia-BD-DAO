@echo off
setlocal EnableExtensions

where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ERROR: no se encontro sqlcmd en el PATH.
    echo Instale SQL Server Command Line Utilities y vuelva a intentar.
    goto :fin
)

echo Desconectando la base de datos de la instancia local...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d master -b -Q "ALTER DATABASE [PizzeriaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db 'PizzeriaDB';"
if errorlevel 1 (
    echo Error al desconectar la base.
) else (
    echo Base desconectada. Los archivos .mdf y .ldf se conservan.
    echo Para volver a usarla ejecute 02_cargaBaseDatos-02.bat.
)

:fin
endlocal
pause
