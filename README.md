# Gestión de Inventarios — Sistema Hospitalario

Módulo de gestión de inventarios desarrollado como parte del sistema integral del hospital.
Proyecto académico — Universidad Privada Domingo Savio.

---

## Estado del proyecto

### Parte 1 — Modelado de base de datos ✅
Se realizó el proceso completo de normalización de la base de datos desde la Forma No Normal hasta la Quinta Forma Normal (5FN), documentado en diagramas individuales con su justificación técnica.

**Formas normales completadas:**
- 0FN — Forma No Normal (estructura plana inicial)
- 1FN — Primera Forma Normal
- 2FN — Segunda Forma Normal
- 3FN — Tercera Forma Normal
- BCNF — Forma Normal de Boyce-Codd
- 4FN — Cuarta Forma Normal
- 5FN — Quinta Forma Normal (esquema final)

El diseño final en 5FN incluye las entidades: `Categoria`, `Insumo`, `Proveedor`, `Almacen`, `Usuario`, `Movimiento`, `DetalleMovimiento`, y las tablas de relación binaria `Suministro`, `Distribucion` y `Logistica`.

---

### Parte 2 — Backend en .NET ✅
Web API desarrollada en .NET 8 con Entity Framework Core y PostgreSQL.

**Entidades implementadas:**

Catálogos maestros:
- `Categoria` — clasificación de insumos médicos
- `Insumo` — insumos del inventario, vinculados a una categoría
- `Proveedor` — proveedores autorizados
- `Almacen` — almacenes del hospital con su ubicación
- `Usuario` — usuarios del sistema con su rol

Relaciones 5FN (tablas binarias):
- `Suministro` — vincula insumos con sus proveedores autorizados
- `Distribucion` — vincula insumos con los almacenes donde se guardan
- `Logistica` — vincula proveedores con los almacenes donde entregan

Módulo transaccional:
- `Movimiento` — cabecera de cada entrada o salida de inventario, vinculada a un usuario
- `DetalleMovimiento` — línea de detalle de un movimiento, registra insumo, proveedor, almacén, lote, fecha de vencimiento y cantidad

**Funcionalidades:**
- CRUD completo para las 10 entidades
- Los endpoints trabajan con el campo `Codigo` como identificador — nunca se expone el `Id` interno ni el campo `Estado`
- Eliminación lógica (soft delete) — los registros se marcan como `Inactivo` en lugar de borrarse
- Integridad referencial a nivel de aplicación en todas las relaciones
- Validación de tipo de movimiento (`Entrada` / `Salida`)
- Consultas con JOIN de 2 tablas: insumos con su categoría (`GET /api/Insumos/PorCategoria`)
- Consultas con JOIN de 3 tablas en múltiples controladores: Suministros, Distribuciones, Logísticas y DetalleMovimientos
- Base de datos física generada y actualizada mediante migraciones de EF Core

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
