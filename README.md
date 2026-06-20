# BancoSol API — Plantilla Onion Architecture (.NET 8)

Proyecto base vacío listo para empezar a desarrollar.

## Estructura

```
BancoSol/
├── src/
│   ├── Core/
│   │   ├── BancoSol.Domain/          ← Centro del onion. Entidades, interfaces, enums.
│   │   │                                NO depende de nada.
│   │   └── BancoSol.Application/     ← Casos de uso, DTOs, servicios.
│   │                                    Depende solo de Domain.
│   ├── Infrastructure/
│   │   └── BancoSol.Infrastructure/  ← EF Core, repositorios, APIs externas.
│   │                                    Depende de Domain + Application.
│   └── Presentation/
│       └── BancoSol.API/             ← Controllers, Program.cs, Swagger.
│                                        Conecta todo (capa más externa).
└── tests/
    └── BancoSol.Tests/               ← Pruebas unitarias (xUnit + Moq)
```

## Regla de oro del Onion

**Las dependencias siempre apuntan hacia adentro.**

```
API → Infrastructure → Application → Domain
```

Domain nunca sabe que existe Infrastructure o API. Si en algún momento un
archivo de `BancoSol.Domain` necesita un `using BancoSol.Infrastructure...`,
algo está mal estructurado.

## Cómo está organizado por ahora

Incluye una entidad y servicio de **ejemplo** (`SampleEntity`, `SampleService`,
`SampleController`) solo para que el proyecto compile y corra desde el día 1.
Bórralos cuando agregues tus entidades reales.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL 8.x

## Cómo correr el proyecto

```bash
# Restaurar paquetes
dotnet restore

# Configurar la cadena de conexión en src/Presentation/BancoSol.API/appsettings.json

# Crear la primera migración
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/BancoSol.Infrastructure \
  --startup-project src/Presentation/BancoSol.API

# Ejecutar (las migraciones se aplican automáticamente al iniciar)
dotnet run --project src/Presentation/BancoSol.API
```

Swagger disponible en: `http://localhost:5000/swagger`

## Correr tests

```bash
dotnet test
```

## Siguiente paso sugerido

1. Crea tu primera entidad real en `BancoSol.Domain/Entities/`
2. Agrega su interfaz de repositorio si necesitas algo más que el CRUD genérico
3. Crea el DTO y el servicio en `BancoSol.Application/`
4. Mapea la entidad en `AppDbContext` (Infrastructure)
5. Crea el controller en `BancoSol.API/Controllers/`
6. Genera la migración y prueba en Swagger
