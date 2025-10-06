# Medical Appointment Application

A comprehensive medical appointment and billing management system built with .NET 8 Web API, Angular 20, and Supabase PostgreSQL database.

## 🏥 Overview

This application provides a streamlined solution for small clinics to manage their essential workflows:

- **Patient Management**: Create and manage patient records with comprehensive information
- **Appointment Scheduling**: Book appointments with doctors and manage schedules  
- **Doctor Schedule Management**: Set up and view doctor availability
- **Billing & Invoicing**: Generate invoices for patient visits with transactional integrity

## 🏗️ Architecture

### Backend (.NET 8 Web API)
- **Framework**: .NET Core 8 Web API
- **Database**: Supabase PostgreSQL with stored procedures
- **Authentication**: Supabase Auth integration
- **Architecture**: Clean Architecture with service layer pattern
- **Features**: 
  - RESTful API endpoints
  - Transactional stored procedures with rollback capabilities
  - CORS configuration for frontend integration
  - Swagger/OpenAPI documentation

### Frontend (Angular 20)
- **Framework**: Angular 20 with zoneless change detection
- **Features**: Server-Side Rendering (SSR), Reactive forms, HTTP client integration
- **Styling**: SCSS with responsive design
- **Architecture**: Component-based architecture with services

### Database (Supabase PostgreSQL)
- **Tables**: patients, doctors, appointments, services, invoices, invoice_items, doctor_schedules
- **Stored Procedures**: 6 transactional procedures for complex operations
- **Features**: ACID compliance, foreign key constraints, triggers, sample data

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js (v18+)
- Angular CLI (`npm install -g @angular/cli`)
- Supabase account and project

### Environment Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MedicalAppointmentApp
   ```

2. **Database Setup**
   - Create a Supabase project
   - Run the SQL scripts in order:
     ```sql
     -- Execute database/schema.sql first
     -- Then execute database/stored_procedures.sql
     ```

3. **Backend Configuration**
   - Update `appsettings.json` with your Supabase connection details:
     ```json
     {
       "Supabase": {
         "Url": "your-supabase-url",
         "Key": "your-supabase-anon-key"
       }
     }
     ```

4. **Frontend Configuration**
   - Update API base URL in `src/app/services/api.ts`

### Running the Application

#### Option 1: Docker (Recommended for Production)

**Quick Start with Docker Compose:**
```bash
# Windows
.\docker-start.ps1

# Linux/Mac
./docker-start.sh
```

This will:
- Build both backend and frontend Docker images
- Start all services with proper networking
- Backend: http://localhost:5236
- Frontend: http://localhost:4200

**Prerequisites:**
- Docker Desktop installed
- Create `.env` file from `.env.example` with your Supabase credentials

📖 **See [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) for complete Docker documentation**

#### Option 2: Using VS Code Tasks
1. Open the project in VS Code
2. Press `Ctrl+Shift+P` and run "Tasks: Run Task"
3. Select "Start Full Application" to run both backend and frontend

#### Option 3: Manual Startup

**Backend API:**
```bash
cd backend/MedicalAppointmentApi
dotnet run
```
API will be available at: `http://localhost:5236`

**Frontend App:**
```bash
cd frontend/medical-appointment-app
ng serve
```
Application will be available at: `http://localhost:4200`

### Building for Production

**Backend:**
```bash
cd backend/MedicalAppointmentApi
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

**Frontend:**
```bash
cd frontend/medical-appointment-app
ng build --configuration production
```

## 🚀 Production Deployment

### Option 1: Docker Deployment (Recommended)

**Complete Docker setup is included with:**
- Multi-stage builds for optimized images
- Production and development configurations
- Automated deployment scripts
- Health checks and networking
- Nginx for serving frontend

📖 **See [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) for detailed instructions**

**Quick Deployment:**
```bash
# Windows
.\docker-start.ps1

# Linux/Mac
./docker-start.sh
```

### Option 2: Azure Deployment

#### Backend (Azure App Service)
1. Create an Azure App Service for .NET 8
2. Configure Application Settings:
   ```
   Supabase__Url = your-supabase-url
   Supabase__Key = your-supabase-key
   ```
3. Deploy using Azure CLI:
   ```bash
   cd backend/MedicalAppointmentApi
   az webapp up --name your-app-name --resource-group your-rg --runtime "DOTNET:8.0"
   ```

#### Frontend (Azure Static Web Apps)
1. Build the application:
   ```bash
   cd frontend/medical-appointment-app
   ng build --configuration production
   ```
2. Deploy to Azure Static Web Apps:
   ```bash
   az staticwebapp create --name your-app-name --resource-group your-rg
   ```

### Option 3: Traditional Hosting

#### Backend (IIS)
1. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
2. Copy `publish` folder to IIS server
3. Create new website in IIS pointing to publish folder
4. Configure environment variables in web.config

#### Frontend (Static Hosting)
1. Build for production:
   ```bash
   ng build --configuration production
   ```
2. Upload contents of `dist/medical-appointment-app/browser` to web server
3. Configure web server for Angular routing (URL rewriting)

### Environment Variables

**Backend:**
- `Supabase__Url`: Your Supabase project URL
- `Supabase__Key`: Your Supabase anonymous/public key
- `ASPNETCORE_ENVIRONMENT`: Set to "Production"
- `AllowedOrigins`: Comma-separated list of allowed frontend URLs

**Frontend:**
Update `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-url.com/api'
};
```

### Security Checklist
- ✅ Enable HTTPS on both frontend and backend
- ✅ Configure CORS properly for production domains
- ✅ Use environment-specific Supabase keys
- ✅ Enable authentication and authorization
- ✅ Set up rate limiting on API endpoints
- ✅ Configure firewall rules
- ✅ Enable logging and monitoring
- ✅ Regular security updates and patches

## 📋 API Endpoints

### Patients
- `GET /api/patients` - Get all patients
- `POST /api/patients` - Create new patient
- `GET /api/patients/{id}` - Get patient by ID
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient

### Appointments
- `GET /api/appointments` - Get all appointments
- `POST /api/appointments` - Schedule new appointment
- `GET /api/appointments/{id}` - Get appointment by ID
- `PUT /api/appointments/{id}` - Update appointment
- `DELETE /api/appointments/{id}` - Cancel appointment

### Doctors
- `GET /api/doctors` - Get all doctors
- `GET /api/doctors/{id}/schedule` - Get doctor schedule
- `GET /api/doctors/{id}/available-slots` - Get available time slots

### Invoices
- `GET /api/invoices` - Get all invoices
- `POST /api/invoices` - Create new invoice
- `GET /api/invoices/{id}` - Get invoice by ID

### Services
- `GET /api/services` - Get all services
- `POST /api/services` - Create new service

## 🗃️ Database Schema

### Main Tables
- **patients**: Patient information and medical history
- **doctors**: Doctor profiles and specializations
- **appointments**: Scheduled appointments
- **services**: Medical services and pricing
- **invoices**: Billing information
- **invoice_items**: Invoice line items
- **doctor_schedules**: Doctor availability

### Stored Procedures
- `create_patient()`: Create patient with validation
- `schedule_appointment()`: Book appointment with conflict checking
- `create_invoice_with_services()`: Generate invoice with line items
- `complete_appointment_with_billing()`: Complete appointment and create invoice
- `get_doctor_schedule()`: Retrieve doctor schedule with appointments
- `get_available_time_slots()`: Find available appointment slots

## 🔧 Development

### Backend Development
- Built with Clean Architecture principles
- Service layer handles business logic
- Controllers provide RESTful endpoints
- Models use Supabase attributes for ORM mapping
- Comprehensive error handling and logging

### Frontend Development
- Angular 20 with latest features
- Zoneless change detection for performance
- Component-based architecture
- Reactive forms for data input
- HTTP interceptors for error handling
- Responsive design with SCSS

### Key Features
- **Transactional Integrity**: All complex operations use stored procedures with rollback
- **Error Handling**: Comprehensive error handling on both frontend and backend
- **Validation**: Input validation on both client and server side
- **CORS Support**: Properly configured for cross-origin requests
- **Logging**: Structured logging for debugging and monitoring

## 📝 Project Status

### ✅ Completed
- ✅ Database schema and stored procedures
- ✅ Backend API with all endpoints (Layered architecture: API → Business → Data → Models)
- ✅ Frontend services and routing
- ✅ Authentication integration with Supabase
- ✅ Build configuration and tasks
- ✅ Development environment setup
- ✅ Patient Management interface (complete with forms, validation, CRUD operations)
- ✅ Appointment Scheduling interface (with doctor/patient selection, time slots)
- ✅ Billing & Invoice Management (invoice creation with multiple items)
- ✅ Doctor Schedule Management (weekly and daily schedule views)
- ✅ Form validation and error handling (reactive forms throughout)
- ✅ Responsive SCSS styling for all components
- ✅ Docker containerization (Backend + Frontend with Nginx)
- ✅ Docker Compose orchestration (Production + Development)
- ✅ Automated deployment scripts (Windows PowerShell + Linux/Mac Bash)
- ✅ Complete Docker documentation

### 🚧 TODO
- ⏳ Integration/E2E testing
- ⏳ Production deployment configuration
- ⏳ User authentication UI
- ⏳ Advanced search and filtering
- ⏳ Reports and analytics dashboard

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Check the API documentation at `http://localhost:5236/swagger`
- Review the database schema in `database/schema.sql`
- Check the stored procedures in `database/stored_procedures.sql`

---

**Built with ❤️ for efficient clinic management**