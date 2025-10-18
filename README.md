# Asisya API - Sistema de Gestión de Productos

API RESTful desarrollada con .NET 8 implementando arquitectura limpia (Clean/Hexagonal), diseñada para gestionar productos y categorías con capacidades de carga masiva y operaciones CRUD completas.

## Stack Tecnológico

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM con soporte PostgreSQL
- **PostgreSQL** - Base de datos relacional
- **JWT** - Autenticación basada en tokens
- **Docker & Docker Compose** - Containerización
- **xUnit, Moq, FluentAssertions** - Suite de testing
- **Testcontainers** - Pruebas de integración

---

## Configuración Inicial

### Prerrequisitos

- .NET 8 SDK instalado
- Docker Desktop (para gestión de contenedores)
- Cliente de base de datos (pgAdmin/DBeaver) - opcional

---

## Guías de Instalación

### Opción 1: Ejecución Local (Recomendada para Desarrollo)

Esta configuración permite depuración directa desde tu IDE preferido.

**Paso 1: Obtener el código fuente**
```bash
git clone <URL_DEL_REPOSITORIO>.git asisya-api
cd asisya-api
```

**Paso 2: Configurar appsettings para desarrollo**

Crear archivo `src/Asisya.Api/appsettings.Development.json`:

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

**Paso 3: Iniciar PostgreSQL en contenedor**
```bash
docker compose up -d db
```
*La base de datos quedará expuesta en el puerto 55432 del host*

**Paso 4: Compilar y ejecutar la API**
```bash
dotnet restore
dotnet build
dotnet run --project src/Asisya.Api
```

**Acceso a la aplicación:**
- Interfaz Swagger: `http://localhost:5000/swagger`
- El sistema inicializará automáticamente las categorías predeterminadas (SERVIDORES, CLOUD)

---

### Opción 2: Despliegue Completo con Docker

Ejecuta toda la infraestructura containerizada (ideal para entornos similares a producción).

```bash
docker compose up --build
```

**Endpoints disponibles:**
- API: `http://localhost:5000/swagger`
- PostgreSQL: `127.0.0.1:55432`

**Credenciales de conexión a BD:**
- Host: `127.0.0.1`
- Puerto: `55432`
- Base de datos: `asisya_db`
- Usuario: `postgres`
- Contraseña: `123`

---

## Guía de Uso de la API

### Autenticación

Obtener token JWT:
```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin"
}
```

Utilizar el token en Swagger mediante el botón **Authorize** o en headers:
```
Authorization: Bearer {tu_token}
```

### Endpoints Principales

**Gestión de Categorías:**
- `POST /api/Category` - Crear nueva categoría
- `GET /api/Category` - Listar todas las categorías

**Gestión de Productos:**
- `POST /api/Products` - Crear producto individual
- `POST /api/Products?count={n}` - Generación masiva de productos aleatorios
- `GET /api/Products` - Listar con filtros, búsqueda y paginación
- `GET /api/Products/{id}` - Obtener detalle (incluye imagen de categoría)
- `PUT /api/Products/{id}` - Actualizar producto
- `DELETE /api/Products/{id}` - Eliminar producto

**Carga masiva optimizada:**
```http
POST /api/Products?count=100000&batchSize=5000
```
*El parámetro `batchSize` controla el tamaño de los lotes de inserción*

---

## Ejecución de Pruebas

```bash
dotnet test
```

**Cobertura de testing:**
- Pruebas unitarias para lógica de negocio
- Pruebas de integración con PostgreSQL real (Testcontainers)

---

## Gestión de Contenedores

### Detener y limpiar recursos del proyecto
```powershell
docker compose down -v
```

### Eliminación completa de imágenes Docker
```powershell
docker images -q | ForEach-Object { docker rmi -f $_ }
```

### Limpieza profunda del sistema Docker
```powershell
docker system prune -a --volumes -f
```

⚠️ **Advertencia**: Esta operación elimina todos los contenedores, imágenes y volúmenes no utilizados

---

## Arquitectura del Sistema

### Patrón Clean Architecture (Hexagonal)

**Domain Layer (Núcleo)**
- Entidades de dominio
- Interfaces de puertos (contratos de repositorios)
- Reglas de negocio independientes

**Application Layer (Casos de Uso)**
- Servicios de aplicación
- DTOs para transferencia de datos
- Orquestación de lógica de negocio

**Infrastructure Layer (Adaptadores)**
- Implementación de repositorios con EF Core
- Contexto de base de datos (`AppDbContext`)
- Integración con Npgsql

**API Layer (Presentación)**
- Controllers REST
- Configuración de inyección de dependencias
- Middleware y filtros

### Estrategias de Rendimiento

**Optimización de Base de Datos:**
- Índices compuestos: `(categoryId, price)` para filtros frecuentes
- Índice de búsqueda: `products(name)` para queries de texto
- Paginación nativa en todas las consultas

**Carga Masiva:**
- Procesamiento por lotes (batch inserts)
- Tamaño de lote configurable
- Transacciones optimizadas

**Inicialización:**
- Método `EnsureCreated()` para bootstrap automático
- Datos semilla (seed data) para categorías base

### Seguridad

- Autenticación JWT con claves de 256+ bits
- Endpoints protegidos con autorización basada en tokens
- Validación de entrada en DTOs

---

## Estrategia de Escalabilidad Cloud

### Arquitectura para Alta Disponibilidad

**1. Capa de Aplicación**
- API stateless compatible con escalado horizontal
- Load Balancer (ALB, Azure App Gateway, GCP HTTPS LB)
- Múltiples instancias detrás del balanceador

**2. Orquestación de Contenedores**
```yaml
# Ejemplo de HorizontalPodAutoscaler para Kubernetes
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: asisya-api-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: asisya-api
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

**3. Capa de Datos**
- **Connection Pooling**: PgBouncer para gestión eficiente de conexiones
- **Read Replicas**: Separación de lecturas/escrituras
- **Escalado vertical**: Aumentar recursos de instancia principal
- **Índices optimizados**: Revisión continua de query performance

**4. Caché y Procesamiento Asíncrono**
- **Redis/Memcached**: Cache de listados y filtros frecuentes
- **Message Queues**: SQS/RabbitMQ/Azure Service Bus para operaciones masivas
- Patrón `202 Accepted` para operaciones de larga duración

**5. Observabilidad**
- Logs estructurados con Serilog
- Distributed tracing (OpenTelemetry)
- Métricas clave: P95 latency, requests/segundo, tasa de error
- Health checks (`/health`, `/ready`)

**6. CI/CD y Despliegue**
- Pipeline automatizado (GitHub Actions, GitLab CI)
- Build → Test → Push a registry
- Estrategias: Blue-Green o Canary deployments
- Rollback automático ante fallos

**7. Gestión de Configuración**
- Variables de entorno para configuración por ambiente
- Secrets Manager para credenciales (AWS Secrets Manager, Azure Key Vault)
- Separación estricta de configuración y código

---

## Variables de Entorno

### Conexión a Base de Datos

**API dentro de Docker:**
```
ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=asisya_db;Username=postgres;Password=123
```

**API local con DB en Docker:**
```
ConnectionStrings__DefaultConnection=Host=localhost;Port=55432;Database=asisya_db;Username=postgres;Password=123
```

### Configuración JWT

```
Jwt__Key=<clave_minimo_32_caracteres>
Jwt__Issuer=Asisya
Jwt__Audience=AsisyaClients
```

---

## Checklist de Verificación

- [ ] Levantar stack completo: `docker compose up --build`
- [ ] Verificar Swagger en `http://localhost:5000/swagger`
- [ ] Autenticarse: `POST /api/Auth/login` con credenciales admin/admin
- [ ] Probar carga masiva: `POST /api/Products?count=10000`
- [ ] Validar filtros: `GET /api/Products?categoryId=1&minPrice=100`
- [ ] Conectar pgAdmin a `127.0.0.1:55432` para inspección de datos
- [ ] Ejecutar suite de pruebas: `dotnet test`

---

## Soporte y Contribuciones

Para reportar problemas o sugerir mejoras, por favor abre un issue en el repositorio del proyecto.