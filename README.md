# BancoSol - Sistema de Gestión Financiera Personal

API REST  desarrollada para  que permite a los clientes registrar y analizar sus flujos financieros en múltiples divisas (Bolivianos y Dólares), centralizando reportes y balances consolidados de su situación financiera.

## Arquitectura y Decisiones de Diseño

El proyecto está diseñado bajo los principios de Arquitectura Cebolla para garantizar el desacoplamiento de la infraestructura, una alta mantenibilidad y testabilidad


## 🚀 Instrucciones de Ejecución Local

### Prerrequisitos
* [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior.
* Servidor [MySQL](https://www.mysql.com/) activo (local o en contenedor).

### 1. Configuración de Base de Datos
Abre el archivo `appsettings.json` dentro del proyecto de la API (`src/Presentation/BancoSol.API/appsettings.json`) y actualiza tu cadena de conexión local en la propiedad `DefaultConnection`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=bancosol_db;User=tu_usuario;Password=tu_password;"
}


## Endpoints implementados

| Método | Ruta | Caso de uso |
|--------|------|-------------|
| `POST` | `/api/Transacciones/registraTransaccion` | 1. Registro de Transacción |
| `GET` | `/api/Transacciones/obtenerTransacciones?creadoPor={opcional}` | 2. Consulta de Historial Completo |
| `GET` | `/api/Transacciones/{id}?creadoPor={opcional}` | 3. Consulta de Transacción Específica |
| `GET` | `/api/Reportes/tipoCambio` | 4. Obtención de Tipo de Cambio (HexaRate) |
| `GET` | `/api/Reportes/balance?fechaInicio=&fechaFin=&moneda=&creadoPor={opcional}` | 5. Reporte de Balance Consolidado |

### 1. Registrar transacción
```http
POST /api/Transacciones/registraTransaccion
Content-Type: application/json

{
  "monto": 5000,
  "descripcion": "Sueldo de diciembre",
  "fecha": "2025-12-01",
  "origen": "sueldo",
  "moneda": "BOB",
  "tipo": 1,
  "creadoPor": "juan.perez@email.com"
}
```
`tipo`: `1` = Ingreso, `2` = Egreso. `moneda`: `"BOB"` o `"USD"` (acepta minúsculas).

**Respuesta 201:**
```json
{
  "codigo": 201,
  "mensaje": "Ejecutado exitosamente",
  "data": {
    "id": 1,
    "monto": 5000,
    "descripcion": "Sueldo de diciembre",
    "fecha": "2025-12-01T00:00:00",
    "origen": "sueldo",
    "moneda": "BOB",
    "tipo": 1,
    "creadoPor": "juan.perez@email.com",
    "fechaCreacion": "2026-06-20T10:00:00Z"
  }
}
```

**Respuesta 400 (moneda inválida):**
```json
{
  "codigo": 400,
  "mensaje": "Moneda 'EUR' no soportada. Solo se aceptan: BOB, USD.",
  "data": null
}
```

### 2. Historial completo
```http
GET /api/Transacciones/obtenerTransacciones?creadoPor=juan.perez
```
`creadoPor` es opcional; si se omite, devuelve todas las transacciones de
todos los usuarios.

### 3. Consultar transacción por ID
```http
GET /api/Transacciones/42?creadoPor=juan.perez
```
Si el ID no existe (o pertenece a otro usuario cuando se filtra por
`creadoPor`), responde `404`.

### 4. Tipo de cambio actual
```http
GET /api/Reportes/tipoCambio
```
```json
{
  "codigo": 200,
  "mensaje": "Ejecutado exitosamente",
  "data": {
    "origen": "USD",
    "destino": "BOB",
    "tasa": 6.92,
    "consultadoEn": "2025-12-01T10:00:00Z"
  }
}
```

### 5. Balance consolidado
```http
GET /api/Reportes/balance?fechaInicio=2025-12-01&fechaFin=2025-12-31&moneda=BOB
```
Suma todos los **ingresos** del período (los egresos no se incluyen en este
cálculo), convirtiendo a la moneda solicitada los que estén en moneda
distinta, usando el tipo de cambio vigente de HexaRate. `creadoPor` es un
filtro opcional adicional.

```json
{
  "codigo": 200,
  "mensaje": "Ejecutado exitosamente",
  "data": {
    "fechaInicio": "2025-12-01T00:00:00",
    "fechaFin": "2025-12-31T00:00:00",
    "moneda": "BOB",
    "balanceTotal": 5692.00,
    "cantidadIngresosConsiderados": 3,
    "tipoCambioUsado": 6.92
  }
}
```

> **Nota sobre rangos de fecha:** si `fechaFin` se envía sin hora (ej.
> `"2025-12-31"`), internamente se extiende hasta `23:59:59.999...` de ese
> día antes de consultar la base de datos, para incluir todas las
> transacciones registradas ese día sin importar a qué hora se crearon.

---

## Monedas soportadas

Solo **BOB** (Bolivianos) y **USD** (Dólares). Cualquier otro código es
rechazado con `400 Bad Request`. La validación es insensible a mayúsculas/minúsculas.

## Tipos de transacción

| Valor | Significado |
|---|---|
| `1` | Ingreso |
| `2` | Egreso |

---

## Pendientes / mejoras conocidas

- El filtro `creadoPor` del reporte de balance (Caso de Uso 5) se aplica
  **en memoria** después de traer las transacciones del rango de fechas,
  en lugar de en la consulta SQL — funcional, pero no óptimo para volúmenes
  grandes de datos.
- No hay paginación en `obtenerTransacciones`.
