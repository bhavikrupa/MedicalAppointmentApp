# Docker Deployment Guide

## 🐳 Medical Appointment Application - Docker Setup

This guide provides complete instructions for deploying the Medical Appointment Application using Docker and Docker Compose.

## 📋 Prerequisites

- Docker Desktop installed (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v3.8 or higher
- Supabase account with database credentials
- 8GB RAM minimum
- 10GB free disk space

## 🚀 Quick Start

### 1. Clone and Navigate to Project

```powershell
cd MedicalAppointmentApp
```

### 2. Configure Environment Variables

Copy the example environment file and update with your credentials:

```powershell
# Windows
Copy-Item .env.example .env

# Linux/Mac
cp .env.example .env
```

Edit `.env` file with your Supabase credentials:

```env
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-anon-key
SUPABASE_CONNECTION_STRING=User Id=postgres.xxxxx;Password=xxxxx;Server=xxxxx.supabase.com;Port=6543;Database=postgres
```

### 3. Build and Run

**Windows (PowerShell):**
```powershell
.\docker-start.ps1
```

**Linux/Mac:**
```bash
chmod +x docker-start.sh
./docker-start.sh
```

### 4. Access Applications

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5236
- **Swagger Documentation**: http://localhost:5236/swagger
- **Health Check**: http://localhost:5236/health

## 📦 What Gets Deployed

### Backend Container
- .NET 8 Web API
- Layered architecture (Models, Data, Business, API)
- Swagger documentation
- Health check endpoint
- Auto-restart on failure

### Frontend Container
- Angular 20 application
- Nginx web server
- Gzip compression enabled
- Static asset caching
- SPA routing support

### Network
- Custom bridge network `medical-app-network`
- Container-to-container communication
- Isolated from host network

## 🔧 Docker Commands

### View Container Status
```bash
docker-compose ps
```

### View Logs
```bash
# All containers
docker-compose logs -f

# Backend only
docker-compose logs -f backend

# Frontend only
docker-compose logs -f frontend

# Last 100 lines
docker-compose logs --tail=100 backend
```

### Stop Containers
```powershell
# Windows
.\docker-stop.ps1

# Linux/Mac
./docker-stop.sh

# Or directly
docker-compose down
```

### Restart Containers
```bash
docker-compose restart
```

### Rebuild Containers
```bash
# Rebuild without cache
docker-compose build --no-cache

# Rebuild and start
docker-compose up -d --build
```

### Remove Everything (including volumes)
```bash
docker-compose down -v
```

## 🔍 Troubleshooting

### Container Won't Start

**Check logs:**
```bash
docker-compose logs backend
docker-compose logs frontend
```

**Common issues:**
- Missing or invalid `.env` file
- Port already in use (5236 or 4200)
- Insufficient disk space
- Docker daemon not running

### Backend Health Check Failing

```bash
# Check backend health
curl http://localhost:5236/health

# Or in PowerShell
Invoke-WebRequest -Uri http://localhost:5236/health
```

### Frontend Not Loading

1. Check if backend is healthy
2. Verify CORS settings in backend
3. Check nginx logs:
   ```bash
   docker-compose logs frontend
   ```

### Database Connection Issues

1. Verify `.env` credentials
2. Check Supabase connection string
3. Test connection from backend container:
   ```bash
   docker exec -it medical-appointment-api /bin/bash
   ```

### Port Conflicts

If ports 4200 or 5236 are already in use, modify `docker-compose.yml`:

```yaml
services:
  backend:
    ports:
      - "5237:80"  # Change 5236 to 5237
  
  frontend:
    ports:
      - "4201:80"  # Change 4200 to 4201
```

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│          Docker Host Machine                │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │     medical-app-network (bridge)     │  │
│  │                                      │  │
│  │  ┌────────────────┐  ┌────────────┐ │  │
│  │  │   Backend      │  │  Frontend  │ │  │
│  │  │   (.NET 8)     │  │  (Angular) │ │  │
│  │  │   Port: 80     │  │  (Nginx)   │ │  │
│  │  └────────┬───────┘  │  Port: 80  │ │  │
│  │           │          └─────────────┘ │  │
│  │           │                          │  │
│  └───────────┼──────────────────────────┘  │
│              │                              │
│              ↓                              │
│    ┌─────────────────────┐                 │
│    │  External Services  │                 │
│    │  (Supabase DB)     │                 │
│    └─────────────────────┘                 │
└─────────────────────────────────────────────┘
         ↑                    ↑
      Port 5236           Port 4200
      (Backend)          (Frontend)
```

## 📁 File Structure

```
MedicalAppointmentApp/
├── docker-compose.yml              # Production compose file
├── docker-compose.dev.yml          # Development compose file
├── .env.example                    # Environment template
├── .env                           # Your environment variables (gitignored)
├── docker-start.ps1               # Windows start script
├── docker-start.sh                # Linux/Mac start script
├── docker-stop.ps1                # Windows stop script
├── docker-stop.sh                 # Linux/Mac stop script
│
├── backend/
│   ├── .dockerignore              # Docker ignore patterns
│   └── MedicalAppointmentApi/
│       └── Dockerfile             # Backend Docker image
│
└── frontend/
    └── medical-appointment-app/
        ├── Dockerfile             # Frontend Docker image
        ├── nginx.conf             # Nginx configuration
        └── .dockerignore          # Docker ignore patterns
```

## 🔐 Security Considerations

### Production Deployment

1. **Use secrets management:**
   ```bash
   docker secret create supabase_key your-key
   ```

2. **Enable HTTPS:**
   - Use reverse proxy (Nginx/Traefik)
   - Configure SSL certificates
   - Update CORS settings

3. **Restrict network access:**
   ```yaml
   networks:
     medical-app-network:
       internal: true  # No external access
   ```

4. **Use non-root user in containers**
5. **Regular security updates**
6. **Monitor container logs**

## 🚀 Production Deployment

### Cloud Deployment Options

#### Docker Hub
```bash
# Tag images
docker tag medical-appointment-api:latest yourusername/medical-api:latest
docker tag medical-appointment-frontend:latest yourusername/medical-frontend:latest

# Push to Docker Hub
docker push yourusername/medical-api:latest
docker push yourusername/medical-frontend:latest
```

#### Azure Container Instances
```bash
az container create \
  --resource-group medical-app \
  --name medical-backend \
  --image yourusername/medical-api:latest \
  --ports 80
```

#### AWS ECS
```bash
# Create task definition
# Deploy using ECS service
```

#### Kubernetes
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: medical-backend
spec:
  replicas: 3
  selector:
    matchLabels:
      app: medical-backend
  template:
    metadata:
      labels:
        app: medical-backend
    spec:
      containers:
      - name: backend
        image: yourusername/medical-api:latest
        ports:
        - containerPort: 80
```

## 📊 Monitoring

### Container Metrics
```bash
# Resource usage
docker stats

# Inspect container
docker inspect medical-appointment-api
```

### Application Logs
```bash
# Follow logs with timestamps
docker-compose logs -f -t

# Export logs
docker-compose logs > application.log
```

## 🔄 CI/CD Integration

### GitHub Actions Example
```yaml
name: Build and Deploy

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Build Docker images
        run: docker-compose build
      
      - name: Push to registry
        run: |
          docker-compose push
```

## 💾 Backup and Restore

### Backup Database
```bash
# Backup from Supabase
# Use Supabase dashboard or pg_dump
```

### Backup Volumes
```bash
docker run --rm \
  -v medical-app-data:/data \
  -v $(pwd):/backup \
  alpine tar czf /backup/backup.tar.gz /data
```

## 🆘 Support

For issues or questions:
- Check logs: `docker-compose logs -f`
- Review [DEVELOPMENT.md](DEVELOPMENT.md)
- Check [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md)

## 📝 License

Medical Appointment Application - 2025
