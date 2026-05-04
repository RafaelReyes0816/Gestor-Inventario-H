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

Catalogos maestros:
- `Categoria` — clasificacion de insumos medicos
- `Insumo` — insumos del inventario, vinculados a una categoria
- `Proveedor` — proveedores autorizados
- `Almacen` — almacenes del hospital con su ubicacion
- `Usuario` — usuarios del sistema con su rol
- `Cama` — gestion de stock de camas hospitalarias por cantidad

Relaciones 5FN (tablas binarias):
- `Suministro` — vincula insumos con sus proveedores autorizados
- `Distribucion` — vincula insumos con los almacenes donde se guardan
- `Logistica` — vincula proveedores con los almacenes donde entregan

Modulo transaccional:
- `Movimiento` — cabecera de cada entrada o salida de inventario, vinculada a un usuario
- `DetalleMovimiento` — linea de detalle de un movimiento, registra insumo, proveedor, almacen, lote, fecha de vencimiento (solo en entradas) y cantidad

**Funcionalidades:**
- CRUD completo para las 11 entidades
- Los endpoints trabajan con el campo `Codigo` como identificador externo; el `Id` interno y el campo `Estado` nunca se exponen
- Eliminacion logica (soft delete) — los registros se marcan como `Inactivo` en lugar de borrarse fisicamente
- Data Transfer Objects (DTOs) en todos los controladores para separar los datos de entrada, actualizacion y respuesta
- Validacion de codigo duplicado en todos los registros nuevos
- `FechaVencimiento` es opcional en `DetalleMovimiento`: obligatoria para Entradas, no requerida para Salidas
- Base de datos fisica generada y actualizada mediante migraciones de EF Core
- CORS configurado con politica abierta para consumo desde la red local

---

### Parte 3 — Diagrama de casos de uso
Se elaboro un unico diagrama de casos de uso con exactamente 10 casos, todos orientados a consultas de informacion (no operaciones CRUD). El diagrama representa el departamento de logistica e inventario hospitalario.

**Actores definidos:**
- Encargado de Logistica / Administrador — actor principal fusionado, accede a la totalidad de las consultas
- Jefe de Farmacia — accede a consultas de disponibilidad, vencimientos y balance de movimientos

**Los 10 casos de uso corresponden a consultas del sistema MIS** y estan documentados con su endpoint y las tablas que involucran.

---

### Parte 4 — Consultas MIS
Se implementaron 16 consultas en total distribuidas en 4 controladores dedicados, separados por area tematica.

**MisGenericas** — 5 consultas genericas obligatorias:
- Listado de insumos con su categoria (JOIN 2 tablas)
- Cantidad de insumos por categoria (GROUP BY + COUNT)
- Cantidad total movida por almacen (GROUP BY + SUM)
- Busqueda de insumo por codigo (filtro)
- Insumos sin proveedor asignado (NOT EXISTS)

**MisInventario** — consultas sobre stock y distribucion:
- Inventario de insumos por almacen (JOIN 3 tablas)
- Insumos proximos a vencer en los proximos 90 dias (JOIN 3 tablas + ORDER BY fecha)
- Almacenes sin insumos asignados (NOT EXISTS)
- Insumos mas solicitados por frecuencia y volumen (GROUP BY + COUNT + SUM)
- Insumos sin distribucion a ningun almacen (NOT EXISTS)
- Stock actual por insumo por almacen — SUM(Entradas) - SUM(Salidas) agrupado
- Alertas de stock bajo — insumos con stock menor o igual a 20 unidades (umbral fijo)

**MisOperaciones** — consultas sobre movimientos y recursos fisicos:
- Historial de movimientos agrupado por usuario (JOIN 2 tablas)
- Entradas y salidas por insumo segun tipo de movimiento (GROUP BY + COUNT + SUM)
- Stock total de camas disponibles (SUM de cantidades activas)

**MisProveedores** — consultas sobre cobertura de proveedores:
- Proveedores autorizados por insumo (JOIN 3 tablas)
- Proveedores con mayor cantidad de insumos suministrados (GROUP BY + COUNT)

Todas las consultas son endpoints `GET` independientes, usan sintaxis LINQ query, incluyen proyeccion y estan orientadas a generar informacion util para la toma de decisiones.

---

### Parte 5 — Frontend en React
Interfaz web desarrollada en React 19 con Vite, consumiendo la API del backend mediante Axios.

**Paginas implementadas:**
- Dashboard — estadisticas generales, alertas de stock bajo, insumos proximos a vencer, movimientos por usuario, cobertura de proveedores y stock de camas
- Stock — registro de entradas y salidas de insumos con tabla de stock actual en tiempo real
- Gestion — administracion CRUD de todas las entidades (catálogos maestros y tablas de relacion 5FN)

**Funcionalidades del frontend:**
- Layout con sidebar fijo de navegacion y topbar con breadcrumb
- Boton "Flujo de trabajo" en la topbar: modal con los 7 pasos en orden para operar el sistema correctamente
- Selects dinamicos que cargan opciones desde la API (sin necesidad de ingresar codigos manualmente)
- En el formulario de stock, al seleccionar un insumo se filtra automaticamente el almacen (segun distribuciones registradas) y el proveedor (segun suministros registrados); si solo hay una opcion se auto-selecciona
- Para movimientos de Salida, los campos Lote y Fecha de vencimiento se ocultan ya que no aplican
- Alertas de stock bajo en el Dashboard con indicador de insumos agotados (stock <= 0)
- Filtro de busqueda en la tabla de stock actual

---

## Tecnologias utilizadas

**Backend:**
- .NET 8 — Web API
- Entity Framework Core 8 — ORM
- PostgreSQL — base de datos
- Swagger / Swashbuckle — documentacion de endpoints

**Frontend:**
- React 19 + Vite
- Axios — cliente HTTP
- CSS-in-JS (estilos en linea)

---

## Como ejecutar

**Backend:**
```bash
dotnet restore
dotnet run
```
La documentacion interactiva estara disponible en `http://localhost:{puerto}/swagger`

**Frontend:**
```bash
npm install
npm run dev
```
La interfaz estara disponible en `http://localhost:5173`
