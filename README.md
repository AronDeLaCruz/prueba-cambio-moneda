\# Cambio de Moneda – API + Angular



Descripción corta: API REST para gestionar tipos de cambio y convertir montos.



\## Stack

.NET 10 (ASP.NET Core), EF Core + SQLite, JWT, Angular, Docker, GitHub Actions



\## Arquitectura

Capas: Domain / Application / Infrastructure / Api. Patrones: Repository, Unit of Work,

Service, DTO, Dependency Injection, Middleware de excepciones.

(Agrega un diagrama sencillo o la estructura de carpetas)



\## Cómo ejecutar

\### Con Docker

docker compose up --build

\- API: http://localhost:8080 · Swagger: /swagger

\- Front: http://localhost:4200

\### Sin Docker

cd backend \&\& dotnet run --project CambioMoneda.Api

cd frontend \&\& npm install \&\& npm start



\## Credenciales de prueba

usuario: admin · password: admin123



\## Endpoints

Tabla con método, ruta, auth y descripción.



\## Ejemplo de conversión

Request/response JSON.



\## Decisiones de diseño

\- SQLite en vez de InMemory: permite transacciones reales.

\- EnsureCreated en vez de migraciones: simplicidad (en producción usaría migraciones).

\- Tipo de cambio inverso: si no existe A->B pero sí B->A, se usa 1/tasa.

\- decimal para montos.

\- Transacciones en las operaciones de escritura del CRUD.

\- Middleware global de excepciones con respuesta uniforme.

\- Usuario fijo en configuración: el enunciado no pide gestión de usuarios.



\## Pruebas

dotnet test · colección Postman en /postman



\## CI/CD

Descripción del pipeline.

