# BancoSol API — Sistema de Gestión Financiera Personal

API REST que permite registrar transacciones financieras (ingresos y egresos) en Bolivianos (BOB) y Dólares (USD), consultar el historial, y obtener un balance consolidado convertido a la moneda que el usuario elija.

**Stack:** .NET 8 · ASP.NET Core Web API · Entity Framework Core · MySQL · xUnit + Moq · Swagger/OpenAPI

## Arquitectura

Onion Architecture, con la regla de dependencia en una sola dirección — Domain no conoce a nadie:

```
src/
├── Core/
│   ├── BancoSol.Domain          # Entidades, Value Objects, excepciones y reglas de negocio puras
│   └── BancoSol.Application     # Casos de uso (servicios), DTOs y mapeo
├── Infrastructure/
│   └── BancoSol.Infrastructure  # EF Core + MySQL, cliente HTTP a la API externa HexaRate
└── Presentation/
    └── BancoSol.API             # Controllers, Swagger, middleware de excepciones
tests/
└── BancoSol.Tests               # xUnit + Moq, por capa (Dominio / Aplicación)
```

## Decisiones de diseño

| Decisión | Razón |
|---|---|
| **Identificación de usuario sin autenticación.** `creadoPor` (capturado al registrar) se usa como filtro opcional en los endpoints de lectura. | El documento no incluye un caso de uso de login, pero los Casos 1 y 2 hablan de "mis ingresos" / "sus ingresos". Construir JWT completo no estaba en el alcance pedido; ver [Mejoras futuras](#mejoras-futuras). |
| **El balance consolidado suma solo Ingresos**, aunque el dominio modela también Egresos. | Los criterios de aceptación y los 3 ejemplos numéricos del Caso 5/7 son exclusivamente sobre ingresos; filtrar por `TipoTransaccion.Ingreso` es lo que reproduce esos resultados exactos. |
| **URL de HexaRate y cadena de conexión externalizadas** (patrón `IOptions<T>`, variables de entorno). | Permite cambiar de proveedor o de base de datos sin recompilar — necesario para desplegar en Railway sin hardcodear credenciales. |
| **404 (no 403) al consultar un ingreso de otro usuario.** | Evita revelar la existencia de IDs ajenos (seguridad por oscuridad, mínimo viable sin auth real). |

## Ejecución local

**Prerrequisitos:** [.NET SDK 8.0](https://dotnet.microsoft.com/download), MySQL activo (local o en contenedor).

```bash
# 1. Configurar la cadena de conexión en src/Presentation/BancoSol.API/appsettings.json
#    "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=bancosol_db;User=tu_usuario;Password=tu_password;" }

# 2. Restaurar, compilar y correr (las migraciones se aplican solas al iniciar)
dotnet restore
dotnet build
dotnet run --project src/Presentation/BancoSol.API

# 3. Correr las pruebas
dotnet test
```

La API redirige `/` a `/swagger`, donde se puede explorar y probar cada endpoint ("Try it out").

## Endpoints

| Método | Ruta | Caso de uso |
|---|---|---|
| `POST` | `/api/Transacciones/registraTransaccion` | 1. Registro de transacción |
| `GET` | `/api/Transacciones/obtenerTransacciones` | 2. Historial completo |
| `GET` | `/api/Transacciones/{id}` | 3. Consulta por ID |
| `GET` | `/api/Reportes/tipoCambio` | 4. Tipo de cambio USD/BOB (HexaRate) |
| `GET` | `/api/Reportes/balance` | 5. Balance consolidado |

**Query params opcionales/requeridos por endpoint:**
- `obtenerTransacciones` y `{id}` → `?creadoPor=` (opcional, filtra por usuario).
- `balance` → `?fechaInicio=&fechaFin=&moneda=` (requeridos) `&creadoPor=` (opcional).

Detalles completos de parámetros, ejemplos y respuestas: ver Swagger en `/swagger` — es la fuente de verdad interactiva, por lo que no se duplica aquí.

**Notas rápidas:**
- `moneda`: solo `BOB` o `USD` (insensible a mayúsculas) → `400` en cualquier otro caso.
- `tipo`: `1` = Ingreso, `2` = Egreso.
- Balance: si `fechaFin` se envía sin hora, se extiende a `23:59:59.999` de ese día para incluir todo lo registrado ese día.

## Pruebas

```bash
dotnet test
```

Cubre, por capa:
- **Dominio:** `Moneda`, `Transaccion`, `CalculadoraBalance` (reproduce los 3 ejemplos numéricos exactos del documento de la prueba: 5692 BOB, 1350 USD, 1200 USD).
- **Aplicación:** `ServicioTransacciones` (registro, validaciones, ownership) y `ServicioReportes` (tipo de cambio, balance, validación de moneda y rango de fechas).

## Despliegue

Desplegado en Railway (API + MySQL). Repositorio incluye `Dockerfile` para build reproducible. La cadena de conexión y la URL de HexaRate se inyectan por variables de entorno, no están en el código.

- **Repositorio:** _completar_
- **API:** _completar_
- **Swagger:** _completar_

## Mejoras futuras

1. **Autenticación (JWT/OAuth2).** Es la mejora de mayor impacto pendiente: hoy `creadoPor` se recibe del cliente sin verificar identidad, lo cual es aceptable para esta prueba técnica pero no para producción. Con JWT, `creadoPor` se derivaría del token validado en el servidor, no del payload — cerrando la brecha de seguridad entre lo pedido por el documento ("mis ingresos") y cómo se garantiza hoy.
2. **Filtro de `creadoPor` en el balance a nivel SQL**, en lugar de en memoria tras traer todo el rango de fechas — mejor rendimiento con volúmenes grandes.
3. **Paginación** en `obtenerTransacciones` para historiales extensos.
