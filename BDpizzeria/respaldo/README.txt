============================================================================
CARPETA DE RESPALDOS - PIZZERIA BELLA ITALIA
============================================================================

Aqui quedan las COPIAS DE LOS ORIGINALES de la base de datos:

    pizzeriaDB.mdf      copia del archivo de datos original
    pizzeriaDB.ldf      copia del archivo de log original

No se generan archivos .bak.

Se generan ejecutando desde la carpeta comandos\BAT:

    respaldarBaseDatos-05.bat

Si ya existen copias, el comando pide confirmacion antes de sobrescribirlas;
si se responde N, las copias actuales quedan intactas.

Para ver tablas, estructura y registros de esas copias sin tocar la base
en vivo, ejecutar desde ESTA carpeta:

    verRegistrosTablas.bat

Ese .bat adjunta una copia temporal de los archivos de aqui como
PizzeriaDB_Visor (en respaldo\_visor), muestra el contenido y al final la
desadjunta. Las copias en respaldo quedan intactas y la base en vivo
PizzeriaDB no se modifica.

No hay rutas absolutas: funciona aunque se mueva la carpeta del proyecto
o se cambie de equipo. Si no hay copias, el .bat indica como generarlas.
============================================================================
