# Gestión de Inventarios — Sistema Hospitalario

Módulo de gestión de inventarios desarrollado como parte del sistema integral del hospital.
Proyecto académico — Universidad Privada Domingo Savio.

---

## Estado del proyecto

### Parte 1 — Modelado de base de datos
Se realizó el proceso completo de normalización de la base de datos desde la Forma No Normal hasta la Quinta Forma Normal (5FN), documentado en diagramas individuales con su justificación técnica.

**Formas normales completadas:**
- 0FN — Forma No Normal (estructura plana inicial)
- 1FN — Primera Forma Normal
- 2FN — Segunda Forma Normal
- 3FN — Tercera Forma Normal
- BCNF — Forma Normal de Boyce-Codd
- 4FN — Cuarta Forma Normal
- 5FN — Quinta Forma Normal (esquema final)

El diseño final en 5FN incluye las entidades: `Categoria`, `Insumo`, `Proveedor`, `Almacen`, `Usuario`, `Movimiento`, `DetalleMovimiento`, `Cama`, y las tablas de relación binaria `Suministro`, `Distribucion` y `Logistica`.

---

### Parte 2 — Backend en .NET
Web API desarrollada en .NET 8 con Entity Framework Core y PostgreSQL.

**Entidades implementadas:**

Catálogos maestros:
- `Categoria` — clasificación de insumos médicos
- `Insumo` — insumos del inventario, vinculados a una categoría
- `Proveedor` — proveedores autorizados
- `Almacen` — almacenes del hospital con su ubicación
- `Usuario` — usuarios del sistema con su rol
- `Cama` — camas del hospital organizadas por sala y tipo

Relaciones 5FN (tablas binarias):
- `Suministro` — vincula insumos con sus proveedores autorizados
- `Distribucion` — vincula insumos con los almacenes donde se guardan
- `Logistica` — vincula proveedores con los almacenes donde entregan

Módulo transaccional:
- `Movimiento` — cabecera de cada entrada o salida de inventario, vinculada a un usuario
- `DetalleMovimiento` — línea de detalle de un movimiento, registra insumo, proveedor, almacén, lote, fecha de vencimiento y cantidad

**Funcionalidades:**
- CRUD completo para las 11 entidades
- Los endpoints trabajan con el campo `Codigo` como identificador — nunca se expone el `Id` interno ni el campo `Estado`
- Eliminación lógica (soft delete) — los registros se marcan como `Inactivo` en lugar de borrarse físicamente
- Data Transfer Objects (DTOs) en todos los controladores para separar los datos de entrada, actualización y respuesta
- Validación de código duplicado en todos los registros nuevos
- Base de datos física generada y actualizada mediante migraciones de EF Core

---

### Parte 3 — Diagrama de casos de uso
Se elaboró un único diagrama de casos de uso con exactamente 10 casos, todos orientados a consultas de información (no operaciones CRUD). El diagrama representa el departamento de logística e inventario hospitalario.

**Actores definidos:**
- Encargado de Logística — actor principal, accede a la mayoría de las consultas operativas
- Jefe de Farmacia — accede a consultas de disponibilidad, vencimientos y balance de movimientos
- Administrador — accede a la totalidad de las consultas del sistema

**Los 10 casos de uso corresponden a consultas del sistema MIS** y están documentados con su endpoint y las tablas que involucran.

---

### Parte 4 — Consultas MIS
Se implementaron 15 consultas en total distribuidas en 4 controladores dedicados, separados por área temática.

**MisGenericas** — 5 consultas genéricas obligatorias:
- Listado de insumos con su categoría (JOIN 2 tablas)
- Cantidad de insumos por categoría (GROUP BY + COUNT)
- Cantidad total movida por almacén (GROUP BY + SUM)
- Búsqueda de insumo por código (filtro)
- Insumos sin proveedor asignado (NOT EXISTS)

**MisInventario** — consultas sobre stock y distribución:
- Inventario de insumos por almacén (JOIN 3 tablas)
- Insumos próximos a vencer en los próximos 90 días (JOIN 3 tablas + ORDER BY fecha)
- Almacenes sin insumos asignados (NOT EXISTS)
- Insumos más solicitados por frecuencia y volumen (GROUP BY + COUNT + SUM)
- Insumos sin distribución a ningún almacén (NOT EXISTS)

**MisOperaciones** — consultas sobre movimientos y recursos físicos:
- Historial de movimientos agrupado por usuario (JOIN 2 tablas)
- Entradas y salidas por insumo según tipo de movimiento (GROUP BY + COUNT + SUM)
- Camas activas agrupadas por sala (GROUP BY + COUNT)

**MisProveedores** — consultas sobre cobertura de proveedores:
- Proveedores autorizados por insumo (JOIN 3 tablas)
- Proveedores con mayor cantidad de insumos suministrados (GROUP BY + COUNT)

Todas las consultas son endpoints `GET` independientes, usan sintaxis LINQ query (`from ... in ... join ... where ... select new {}`), incluyen proyección y están orientadas a generar información útil para la toma de decisiones.

---

## Tecnologías utilizadas
- .NET 8 — Web API
- Entity Framework Core 8 — ORM
- PostgreSQL — base de datos
- Swagger / Swashbuckle — documentación de endpoints

## Cómo ejecutar
```bash
dotnet restore
```
```bash
dotnet run
```
La documentación interactiva estará disponible en `http://localhost:{puerto}/swagger`
