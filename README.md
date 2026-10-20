# Cambio de Moneda – API .NET + Angular

API REST para gestionar tipos de cambio y convertir montos entre monedas, con un front en Angular que la consume. Incluye autenticación JWT, middleware global de excepciones, Docker y CI con GitHub Actions.

## Stack

| Capa | Tecnología |
|---|---|
| Backend | C# · .NET 10 · ASP.NET Core (controllers) |
| Datos | Entity Framework Core + SQLite |
| Seguridad | JWT Bearer |
| Documentación | OpenAPI + Swagger UI |
| Front | Angular (standalone components, signals, Reactive Forms) |
| Contenedores | Docker (multi-stage) + Docker Compose |
| CI/CD | GitHub Actions + GitHub Container Registry |

## Estructura del repositorio

```
├── backend/
│   ├── CambioMoneda.Domain/          # Entidades y excepciones de dominio
│   ├── CambioMoneda.Application/     # DTOs, interfaces y servicios (lógica de negocio)
│   ├── CambioMoneda.Infrastructure/  # EF Core, repositorios, UnitOfWork, JWT
│   ├── CambioMoneda.Api/             # Controllers, middleware, Program.cs
│   └── Dockerfile
├── frontend/                         # App Angular + Dockerfile + nginx.conf
├── postman/                          # Colección de Postman
├── .github/workflows/ci.yml          # Pipeline de CI/CD
└── docker-compose.yml
```

## Arquitectura y patrones

Arquitectura en capas con dependencias hacia adentro: `Api → Application → Domain`, e `Infrastructure` implementa las interfaces definidas en `Application`.

- **Repository**: acceso a datos detrás de `ITipoCambioRepository`.
- **Unit of Work**: `IUnitOfWork.ExecuteInTransactionAsync` envuelve las operaciones de escritura en una transacción.
- **Service layer**: la lógica de negocio vive en `TipoCambioService` y `ConversionService`; los controllers solo orquestan.
- **DTOs**: la API nunca expone las entidades directamente.
- **Dependency Injection**: registro centralizado en `DependencyInjection.cs`.
- **Middleware de excepciones**: traduce excepciones de dominio a respuestas HTTP uniformes.

## Cómo ejecutar

### Con Docker (recomendado)

```bash
docker compose up --build
```

| Servicio | URL |
|---|---|
| Front Angular | http://localhost:4200 |
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |

### Sin Docker

Requisitos: .NET 10 SDK y Node.js 22.

```bash
# Backend (el front espera la API en el puerto 8080)
dotnet run --project backend/CambioMoneda.Api --urls http://localhost:8080

# Frontend
cd frontend
npm install
npm start
```

## Credenciales de prueba

| Usuario | Contraseña |
|---|---|
| `admin` | `admin123` |

La base de datos se crea sola al iniciar y trae dos tipos de cambio de ejemplo: USD→PEN (3.75) y EUR→PEN (4.10).

## Endpoints

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/auth/login` | No | Devuelve un JWT |
| GET | `/api/tipos-cambio` | JWT | Lista los tipos de cambio |
| GET | `/api/tipos-cambio/{id}` | JWT | Obtiene uno por id |
| POST | `/api/tipos-cambio` | JWT | Crea un tipo de cambio |
| PUT | `/api/tipos-cambio/{id}` | JWT | Actualiza un tipo de cambio |
| DELETE | `/api/tipos-cambio/{id}` | JWT | Elimina un tipo de cambio |
| POST | `/api/conversion` | JWT | Aplica un tipo de cambio a un monto |

### Ejemplo de conversión

Request:

```json
{ "monto": 100, "monedaOrigen": "USD", "monedaDestino": "PEN" }
```

Response:

```json
{
  "monto": 100,
  "montoConTipoCambio": 375.00,
  "monedaOrigen": "USD",
  "monedaDestino": "PEN",
  "tipoCambio": 3.75
}
```

### Formato de errores

Los errores de negocio pasan por el middleware y salen siempre con la misma forma:

```json
{ "status": 404, "message": "No hay tipo de cambio registrado para USD->JPY.", "traceId": "0HN..." }
```

| Código | Cuándo |
|---|---|
| 400 | Datos inválidos (monto ≤ 0, monedas mal formadas, tasa ≤ 0, monedas iguales) |
| 401 | Falta el token, es inválido o expiró (lo responde la autenticación JWT, sin cuerpo) |
| 404 | El tipo de cambio o el par de monedas no existe |
| 409 | Ya existe un tipo de cambio para ese par |
| 500 | Error inesperado (se registra en logs; al cliente solo le llega un mensaje genérico) |

## Pruebas

```bash
dotnet test backend
```

Tests unitarios de `ConversionService` (xUnit + NSubstitute): tasa directa, tasa inversa, redondeo, normalización de monedas y validaciones. El repositorio se simula, así que no necesitan base de datos.

## Decisiones de diseño

- **SQLite en lugar de InMemory**: el enunciado permite una base en memoria u otra; SQLite soporta transacciones reales y persiste en un archivo.
- **`EnsureCreated` en lugar de migraciones**: simplifica el arranque para esta prueba. En producción usaría migraciones.
- **Tipo de cambio inverso**: si no existe el par A→B pero sí B→A, se convierte con `1 / tasa` (redondeo a 6 decimales para la tasa y a 2 para el monto).
- **`decimal` para dinero**: evita errores de redondeo de `double`; la tasa se guarda con precisión (18,6).
- **Índice único (origen, destino)**: garantiza a nivel de base de datos que no haya pares duplicados.
- **Transacciones en el CRUD**: crear, actualizar y eliminar se ejecutan dentro de `ExecuteInTransactionAsync`, con rollback si algo falla.
- **Códigos de moneda normalizados**: se normalizan en backend (y el front las pasa a mayúsculas).
- **Usuario fijo en configuración**: el enunciado no pide gestión de usuarios, así que el login valida contra `appsettings.json`.
- **Swagger habilitado en todos los entornos**: facilita la evaluación; en producción lo restringiría.
- **Front**: el interceptor agrega el JWT a cada petición, un guard protege las rutas, y otro interceptor limpia la sesión ante un 401. El token se guarda en `localStorage` junto con su fecha de expiración.

## Postman

Importa `postman/CambioMoneda.postman_collection.json`. Trae las variables `baseUrl` (`http://localhost:8080/api`) y `token`. Al ejecutar **Login**, el token se guarda solo y el resto de peticiones lo usan automáticamente. Incluye una carpeta **Casos de Error** (sin token, par inexistente, monto negativo y duplicado).

## CI/CD

El workflow `.github/workflows/ci.yml` se ejecuta en cada push a main y en cada pull request:

1. **backend**: restaura, compila y ejecuta los tests de .NET.
2. **frontend**: instala dependencias y compila Angular.
3. **docker** (solo en `main`): construye las imágenes de API y front y las publica en GitHub Container Registry.

## Limitaciones conocidas

- La clave JWT y las credenciales de prueba están en `appsettings.json` por simplicidad; en un entorno real irían en variables de entorno o un gestor de secretos (la clave se puede sobrescribir con `Jwt__Key`).
- No hay gestión de usuarios ni roles.
- La base SQLite vive dentro del contenedor, así que se reinicia si se elimina.