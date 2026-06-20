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


## 📝 Especificación Técnica de Endpoints 

### 1. Registro de Transacciones (Ingresos)
* **HTTP Method:** `POST`
* **Ruta Física:** `/api/transacciones/registratransaccion`
* **Content-Type:** `application/json`
* **Descripción:** Permite un flujo financiero en el sistema. Aplica de manera  las reglas y validaciones.

#### 🛠️ Reglas de Negocio y Validación de Dominio (Criterios de Aceptación)
1. **Validación de Monto:** El monto enviado debe ser estrictamente mayor a cero (`> 0`). En caso contrario, el dominio lanza un `MontoInvalidoException` (HTTP 400).
2. **Campos Requeridos:** La `descripcion`, el `origen` y el usuario `creadoPor` son obligatorios, sanitizándose mediante `Trim()` para evitar strings vacíos o con espacios huérfanos.
3. **Control de Divisas :** Únicamente se aceptan códigos ISO de moneda **`BOB`** (Bolivianos) y **`USD`** (Dólares) Cualquier otra divisa (como `EUR`) es rechazada inmediatamente por el Value Object `Moneda`

---

#### 📥 Estructura de la Petición (Payload Request)

```json
{
  "monto": 5000.00,
  "descripcion": "Sueldo de diciembre",
  "fecha": "2025-12-01T00:00:00Z",
  "origen": "sueldo",
  "moneda": "BOB",
  "tipo": 1,
  "creadoPor": "juan.perez@email.com"
}