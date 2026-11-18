============================================================================
COMANDOS DE BASE DE DATOS - PIZZERIA BELLA ITALIA (DAO / .NET 10 WinForms)
============================================================================

ESTRUCTURA
----------
comandos\
    BAT\     todos los .bat ejecutables (numerados por orden de uso)
    SQL\     todos los .sql que invocan los .bat (mismo numero)
    cfg.txt  cadena de conexion usada por Dominio/ConexionBD.cs
    README.txt  este archivo

Cada .bat calcula la carpeta SQL con %~dp0..\SQL, por lo que funciona
aunque se mueva la carpeta del proyecto a otro equipo.

ORDEN DE USO (carpeta BAT)
--------------------------
01) creaBaseDatosP-01.bat      -> crea la BD y las tablas desde cero;
                                  usa creaBaseDatosPizzeria-01.sql y
                                  actualizaBaseDatosClientes-01b.sql.
02) cargaBaseDatos-02.bat      -> adjunta PizzeriaDB a LocalDB y deja MULTI_USER.
                                  Es el paso obligatorio antes de abrir la app.
03) verEstadoConexion-03.bat   -> diagnostico: LocalDB, estado/modo de la BD,
                                  ruta del .mdf y cantidad de filas.
04) verTablas-04.bat           -> tablas, estructura y datos.
05) respaldarBaseDatos-05.bat  -> copia pizzeriaDB.mdf/.ldf a la carpeta respaldo\.
                                  Si ya hay copias, pide confirmacion antes de
                                  sobrescribirlas. No genera archivos .bak.
06) (en carpeta respaldo) verRegistrosTablas.bat
                              -> muestra tablas/registros de las copias en respaldo.
07) vaciaBaseDatos-06.bat      -> borra pedidos/clientes y reinicia IDENTITY.
08) desconectaBase-07.bat      -> solo DESADJUNTA (no borra archivos).
    cargaBaseDatos-02.bat      -> para volver a conectarla.

QUE HACE CADA ARCHIVO
---------------------
creaBaseDatosP-01.bat       llama a creaBaseDatosPizzeria-01.sql (DDL + UQ) y
                            despues a actualizaBaseDatosClientes-01b.sql.
cargaBaseDatos-02.bat       llama a cargaBaseDatos-02.sql (attach + MULTI_USER
                            y correccion de rutas antiguas).
verEstadoConexion-03.bat    llama a verEstadoConexion-03.sql (solo lectura).
verTablas-04.bat            llama a verTablas-04.sql (solo lectura).
respaldarBaseDatos-05.bat   llama a respaldarBaseDatos-05.sql (verifica y hace
                            CHECKPOINT) y copia los originales .mdf/.ldf en
                            respaldo\, previa confirmacion si ya existen.
vaciaBaseDatos-06.bat       llama a vaciaBase-06.sql (DELETE + RESEED).
desconectaBase-07.bat       llama a desconectaBase-07.sql (sp_detach_db).

PORTABILIDAD (cualquier equipo)
-------------------------------
- No hay rutas absolutas en los .sql. La ruta de los archivos se calcula
  como dos niveles por encima de BAT\ (es decir, BDpizzeria).
- sqlcmd se busca en el PATH y luego en ODBC 140 a 180.
- sqllocaldb se busca en el PATH y en versiones habituales; si la instancia
  no existe se crea y si esta detenida se inicia.
- Los .bat usan chcp 65001 y mensajes sin tildes para evitar mojibake.
- La app se conecta por NOMBRE de BD (PizzeriaDB), no por ruta: basta con
  ejecutar cargaBaseDatos-02.bat en el equipo nuevo.

UBICACION DE ARCHIVOS
---------------------
BDpizzeria\                  pizzeriaDB.mdf, pizzeriaDB.ldf (base en vivo)
BDpizzeria\comandos\BAT\     los 7 .bat numerados
BDpizzeria\comandos\SQL\     los 8 .sql numerados
BDpizzeria\respaldo\               copias .mdf/.ldf y verRegistrosTablas.bat (sin .bak)

============================================================================
