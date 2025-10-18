# Asisya API (Clean/Hexagonal, .NET 8)

Cumple con la prueba técnica. Arquitectura limpia/hexagonal, EF Core (PostgreSQL), JWT, Docker, CI y **semilla automática** de categorías `SERVIDORES` y `CLOUD`.

---

## 📦 Requisitos
- **.NET 8 SDK**
- **Docker Desktop** (recomendado para DB)
- (Opcional) **pgAdmin 4** o **DBeaver** para inspeccionar la base

---

## 🚀 Clonar, construir y ejecutar **localmente** (API fuera de Docker)

> Ideal para depurar desde Visual Studio/VS Code.

1) **Clonar el repo**
```bash
git clone <TU-REPO-GIT>.git asisya-api
cd asisya-api
```

2) **Configurar conexión local (Development)**
Crea `src/Asisya.Api/appsettings.Development.json` con tu conexión. Si usas la DB en Docker mapeada al puerto `55432`, usa:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=55432;Database=asisya_db;Username=postgres;Password=123"
  },
  "Jwt": {
    "Key": "this_is_a_dev_key_with_more_than_32_chars_123456",
    "Issuer": "Asisya",
    "Audience": "AsisyaClients"
  }
}
```

3) **Levantar la base en Docker (solo DB)**
> Si ya tienes PostgreSQL local, puedes saltar este paso y ajustar la cadena.
```bash
docker compose up -d db
```
> El `docker-compose.yml` expone la DB en el **host 55432** → contenedor 5432.

4) **Restaurar, compilar y ejecutar la API en Development**
```bash
dotnet restore
dotnet build
dotnet run --project src/Asisya.Api
```
- Swagger: http://localhost:5000/swagger
- En el primer arranque se crea el esquema y se siembran categorías `SERVIDORES` y `CLOUD`.

5) **Probar autenticación (JWT)**
```http
POST /api/Auth/login
{
  "username": "admin",
  "password": "admin"
}
```
Copia el token y pulsa **Authorize** en Swagger (Bearer).

6) **Probar endpoints clave**
- `POST /api/Category` → crear categoría
- `GET /api/Category` → listar
- `POST /api/Products` → crear **un** producto
- `POST /api/Products?count=100000` → **generar y guardar productos aleatorios** (carga masiva)
- `GET /api/Products` → paginar/filtrar/buscar
- `GET /api/Products/{id}` → detalle con foto de la categoría
- `PUT /api/Products/{id}` / `DELETE /api/Products/{id}`

> **Nota de carga masiva**: puedes ajustar el tamaño de lote con el parámetro opcional `batchSize`  
> Ejemplo: `/api/Products?count=100000&batchSize=5000`

---

## 🐳 Ejecutar **todo con Docker** (API + DB)

1) **Levantar todo**
```bash
docker compose up --build
```
- API: http://localhost:5000/swagger
- DB (host): **127.0.0.1:55432**

2) **Conectar pgAdmin**
- Host: `127.0.0.1`
- Port: `55432`
- Maintenance DB: `postgres`
- User: `postgres`
- Password: `123`

> Si no ves `asisya_db`, pulsa *Refresh* en **Databases** o vuelve a registrar la conexión.

---

## 🧪 Pruebas

```bash
dotnet test
```
- **Unitarias**: xUnit + Moq + FluentAssertions.  
- **Integración**: Testcontainers (requiere Docker en ejecución).

---

## 🧰 Limpieza de Docker (reset total)

> **Windows PowerShell**

- Bajar el stack de este proyecto y borrar volúmenes:
```powershell
docker compose down -v
```

- Borrar **todas** las imágenes (builds):
```powershell
docker images -q | ForEach-Object { docker rmi -f $_ }
```

- Borrar contenedores e imágenes/volúmenes/redes no usados:
```powershell
docker system prune -a --volumes -f
```

> ⚠️ Esto deja Docker como recién instalado (se eliminan datos persistidos).

---

## 🏗️ Decisiones arquitectónicas

- **Clean/Hexagonal**:  
  - **Domain**: entidades + puertos (interfaces de repos).
  - **Application**: casos de uso, **DTOs** y lógica de orquestación (no exponemos entidades).
  - **Infrastructure**: adaptadores (EF Core + Npgsql), repositorios y `AppDbContext`.
  - **Api**: controllers y wiring DI.
- **Persistencia**: PostgreSQL con EF Core. En arranque se usa `EnsureCreated()` para bootstrap simple en DEV/containers.  
- **Semilla**: crea `SERVIDORES` y `CLOUD` si no existen.
- **Seguridad**: JWT (clave ≥ 32 chars), endpoints críticos protegidos.
- **Performance**:
  - Inserción **por lotes** en carga masiva.
  - Índices `products(name)` y `(categoryId, price)`.
  - Query params para filtros (paginación, categoría, rango de precios, texto).
- **Pruebas**: unitarias (servicios) + integración (repositorios con Postgres real vía Testcontainers).

---

## ☁️ Escalado horizontal en cloud

**Objetivo**: aumentar réplicas y soportar picos (p. ej., 100k+ inserts) manteniendo latencia estable.

1. **API stateless** con JWT → detrás de **Load Balancer** (ALB/NLB, App Gateway, GCP HTTPS LB).  
2. **Contenedores** → **Kubernetes/ECS/App Service** con **autoscaling** (HPA o equivalente).  
3. **DB**: **PgBouncer** (pooling), **read replicas** para lecturas, índices y escalado vertical inicial.  
4. **Caching y colas**: Redis para listados/filtros, colas (SQS/RabbitMQ/Service Bus) para cargas masivas asíncronas (`202 Accepted`).  
5. **Observabilidad**: logs estructurados (Serilog), tracing (OpenTelemetry), métricas (P95, RPS), health/ready checks.  
6. **CI/CD**: GitHub Actions → build/test/push imagen → despliegue blue/green o canary.  
7. **Config/secretos**: variables de entorno + Secret Manager (JWT/DB).

Ejemplo HPA (K8s):
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
spec:
  minReplicas: 2
  maxReplicas: 20
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 60
```

---

## 🔐 Variables de entorno más comunes

- **ConnectionStrings:DefaultConnection**  
  - Docker (API dentro de Docker): `Host=db;Port=5432;Database=asisya_db;Username=postgres;Password=123`  
  - Local (API fuera de Docker, DB en Docker 55432): `Host=localhost;Port=55432;Database=asisya_db;Username=postgres;Password=123`

- **JWT**  
  - `Jwt:Key` (≥ 32 chars), `Jwt:Issuer`, `Jwt:Audience`

---

## ✅ Checklist rápido
- `docker compose up --build` levanta API+DB.  
- Swagger en `http://localhost:5000/swagger`.  
- `POST /api/Auth/login` (admin/admin) → token.  
- `POST /api/Products?count=N` → carga masiva (lotes).  
- `GET /api/Products` → filtros/paginación.  
- pgAdmin a `127.0.0.1:55432` para ver `asisya_db`.
