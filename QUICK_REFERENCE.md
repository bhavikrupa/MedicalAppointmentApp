# Quick Reference Guide

## 🚀 Getting Started in 5 Minutes

### 1. Prerequisites
- [ ] Docker Desktop installed and running
- [ ] Supabase project created
- [ ] Git repository cloned

### 2. Environment Setup
```bash
# Copy environment template
cp .env.example .env

# Edit .env and add your Supabase credentials
```

Required variables:
```env
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-anon-key
POSTGRES_CONNECTION_STRING=postgresql://...
```

### 3. Deploy

**Windows:**
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
- **Backend API**: http://localhost:5236/swagger
- **Health Check**: http://localhost:5236/health

### 5. Stop Services

**Windows:**
```powershell
.\docker-stop.ps1
```

**Linux/Mac:**
```bash
./docker-stop.sh
```

---

## 📋 Common Commands

### Docker Operations
```bash
# View running containers
docker ps

# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f backend
docker-compose logs -f frontend

# Restart a service
docker-compose restart backend
docker-compose restart frontend

# Rebuild and restart
docker-compose up -d --build

# Stop all services
docker-compose down

# Remove everything (including volumes)
docker-compose down -v
```

### Development Operations
```bash
# Build backend
cd backend/MedicalAppointmentApi
dotnet build

# Run backend
dotnet run

# Build frontend
cd frontend/medical-appointment-app
npm install
ng build

# Run frontend
ng serve
```

### Database Operations
```bash
# Connect to Supabase
# Use Supabase Dashboard or psql client
psql "postgresql://postgres:[password]@db.[project].supabase.co:5432/postgres"

# Run schema
psql -f database/schema.sql

# Run stored procedures
psql -f database/stored_procedures.sql

# Insert demo data
psql -f database/demodata.sql
```

---

## 🔧 VS Code Tasks

Press `Ctrl+Shift+P` → "Tasks: Run Task" → Select:

- **Start Full Application** - Run both backend and frontend
- **Start Backend API** - Run only backend
- **Start Frontend App** - Run only frontend
- **Build Backend** - Build .NET project
- **Build Frontend** - Build Angular project

---

## 📂 Project Structure

```
MedicalAppointmentApp/
├── backend/
│   └── MedicalAppointmentApi/         # .NET 8 Web API
│       ├── Controllers/               # REST endpoints
│       ├── Models/                    # Entity models
│       ├── DTOs/                      # Data transfer objects
│       ├── Services/                  # Business logic
│       ├── Dockerfile                 # Backend container
│       └── appsettings.json          # Configuration
├── frontend/
│   └── medical-appointment-app/       # Angular 20
│       ├── src/app/
│       │   ├── components/           # UI components
│       │   └── services/             # API services
│       ├── Dockerfile                # Frontend container
│       └── nginx.conf                # Web server config
├── database/
│   ├── schema.sql                    # Database schema
│   ├── stored_procedures.sql         # Stored procedures
│   └── demodata.sql                  # Sample data
├── docker-compose.yml                # Production config
├── docker-compose.dev.yml            # Development config
├── .env.example                      # Environment template
├── docker-start.ps1                  # Windows deploy
├── docker-start.sh                   # Linux/Mac deploy
└── Documentation files               # READMEs and guides
```

---

## 🌐 API Endpoints

### Patients
```http
GET    /api/patients           # List all patients
POST   /api/patients           # Create patient
GET    /api/patients/{id}      # Get patient by ID
PUT    /api/patients/{id}      # Update patient
DELETE /api/patients/{id}      # Delete patient
```

### Appointments
```http
GET    /api/appointments        # List all appointments
POST   /api/appointments        # Schedule appointment
GET    /api/appointments/{id}   # Get appointment
PUT    /api/appointments/{id}   # Update appointment
DELETE /api/appointments/{id}   # Cancel appointment
```

### Doctors
```http
GET    /api/doctors                      # List all doctors
GET    /api/doctors/{id}/schedule        # Get doctor schedule
GET    /api/doctors/{id}/available-slots # Get available slots
```

### Invoices
```http
GET    /api/invoices        # List all invoices
POST   /api/invoices        # Create invoice
GET    /api/invoices/{id}   # Get invoice by ID
```

### Services
```http
GET    /api/services        # List all services
POST   /api/services        # Create service
```

---

## 🐛 Troubleshooting

### Docker Issues

**Containers won't start:**
```bash
# Check logs
docker-compose logs

# Rebuild without cache
docker-compose build --no-cache

# Clean everything and restart
docker-compose down -v
docker-compose up -d
```

**Port conflicts:**
```bash
# Windows - Find process on port
netstat -ano | findstr :4200
taskkill /PID <process_id> /F

# Linux/Mac - Find and kill process
lsof -ti:4200 | xargs kill -9
```

### Backend Issues

**Can't connect to database:**
- Verify `.env` file has correct Supabase credentials
- Check Supabase connection string is valid
- Ensure database is accessible from your network

**API returns 500 errors:**
```bash
# Check backend logs
docker-compose logs backend

# Or if running locally
dotnet run
```

### Frontend Issues

**Can't reach backend API:**
- Verify backend is running: http://localhost:5236/health
- Check CORS configuration in backend
- Ensure API URL is correct in frontend service

**Build fails:**
```bash
# Clear cache and reinstall
cd frontend/medical-appointment-app
rm -rf node_modules
rm package-lock.json
npm install
```

---

## 📊 Health Checks

### Backend Health
```bash
curl http://localhost:5236/health
```

Should return:
```json
{
  "status": "Healthy",
  "timestamp": "2024-12-..."
}
```

### Frontend Health
```bash
curl http://localhost:4200
```

Should return the Angular index.html

### Container Health
```bash
docker-compose ps
```

Should show all services as "healthy"

---

## 🔐 Security Checklist

Before deployment:
- [ ] Update `.env` with production credentials
- [ ] Never commit `.env` file to Git
- [ ] Use strong passwords for database
- [ ] Enable HTTPS in production
- [ ] Configure CORS for production domains only
- [ ] Review Nginx security headers
- [ ] Enable rate limiting
- [ ] Set up monitoring and alerts
- [ ] Regular security updates

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| [README.md](README.md) | Main project overview |
| [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) | Complete Docker guide |
| [DEPLOYMENT_SUMMARY.md](DEPLOYMENT_SUMMARY.md) | Quick deployment reference |
| [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) | Backend architecture |
| [DEVELOPMENT.md](DEVELOPMENT.md) | Development guide |
| [TESTING.md](TESTING.md) | Testing procedures |
| [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) | Project completion status |

---

## 🆘 Getting Help

1. Check documentation files above
2. Review Swagger API docs: http://localhost:5236/swagger
3. Check application logs: `docker-compose logs -f`
4. Verify environment variables: `docker-compose config`
5. Review Docker status: `docker-compose ps`

---

## ✅ Pre-Deployment Checklist

- [ ] All tests passing
- [ ] Documentation reviewed
- [ ] `.env` configured with production values
- [ ] Database schema deployed
- [ ] Stored procedures created
- [ ] Demo data loaded (optional)
- [ ] Docker images built successfully
- [ ] Health checks working
- [ ] API endpoints tested
- [ ] Frontend can reach backend
- [ ] CORS configured correctly
- [ ] Security review completed
- [ ] Monitoring configured
- [ ] Backup strategy defined

---

**Quick Start Time: 5 minutes** ⚡  
**First Deployment: 15 minutes** 🚀  
**Full Setup with Data: 30 minutes** 📊

---

*Keep this guide handy for quick reference!*
