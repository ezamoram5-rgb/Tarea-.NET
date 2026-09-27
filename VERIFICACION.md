# Verificación de la entrega

**Autor: Eutimio Zamora**

La solución se verificó en Windows con el SDK de .NET 10.0.400, Entity Framework Core 10.0.11 y una base SQLite temporal real.

La compilación Release finalizó con **0 errores y 0 advertencias**. El script `scripts/Verificar-API.ps1` terminó correctamente con **23 comprobaciones aprobadas**.

| Comprobación | Resultado |
| --- | --- |
| GET inicial devuelve 200 y una lista vacía | Correcto |
| POST devuelve 201 | Correcto |
| POST genera ID y cabecera Location | Correcto |
| GET por ID conserva los campos y las tildes | Correcto |
| GET incluye el cliente registrado | Correcto |
| POST con CUI duplicado devuelve 409 | Correcto |
| PUT devuelve 204 | Correcto |
| GET confirma los cambios de PUT | Correcto |
| Los datos se conservan después de reiniciar la API | Correcto |
| PUT con ID distinto al de la ruta devuelve 400 | Correcto |
| PUT de cliente inexistente devuelve 404 | Correcto |
| POST con ID asignado manualmente devuelve 400 | Correcto |
| PUT con CUI de otro cliente devuelve 409 | Correcto |
| DELETE elimina el segundo cliente de prueba | Correcto |
| CUI inválido devuelve 400 | Correcto |
| Fecha futura devuelve 400 | Correcto |
| Fecha omitida devuelve 400 | Correcto |
| Campos obligatorios ausentes devuelven 400 | Correcto |
| Swagger responde y documenta la ruta de clientes | Correcto |
| DELETE devuelve 204 | Correcto |
| GET del cliente eliminado devuelve 404 | Correcto |
| DELETE del cliente inexistente devuelve 404 | Correcto |
| GET final devuelve una lista vacía | Correcto |

Las pruebas utilizan exclusivamente datos ficticios. La entrega no incluye registros de clientes ni archivos de base de datos. La primera ejecución crea automáticamente la base vacía.
