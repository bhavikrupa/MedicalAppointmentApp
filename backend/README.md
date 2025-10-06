# Medical Appointment Application - Layered Backend Architecture

## 🎯 Architecture Overview

The backend has been completely restructured into a **clean, layered architecture** with clear separation of concerns, following industry best practices and SOLID principles.

## 📁 Project Structure

```
backend/
├── MedicalAppointmentApp.Models/          # Data Models Layer
│   ├── Entities/                          # Database entities
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   ├── Appointment.cs
│   │   ├── Service.cs
│   │   ├── Invoice.cs
│   │   ├── InvoiceItem.cs
│   │   └── DoctorSchedule.cs
│   ├── DTOs/                              # Data Transfer Objects
│   │   ├── CreatePatientDto.cs
│   │   ├── ScheduleAppointmentDto.cs
│   │   ├── CreateInvoiceDto.cs
│   │   ├── CompleteAppointmentDto.cs
│   │   └── ResponseDtos.cs
│   └── Common/                            # Common models
│       └── ApiResponse.cs
│
├── MedicalAppointmentApp.Data/            # Data Access Layer
│   ├── Interfaces/                        # Repository contracts
│   │   ├── IPatientRepository.cs
│   │   ├── IAppointmentRepository.cs
│   │   ├── IDoctorRepository.cs
│   │   ├── IServiceRepository.cs
│   │   └── IInvoiceRepository.cs
│   └── Repositories/                      # Repository implementations
│       ├── PatientRepository.cs
│       ├── AppointmentRepository.cs
│       ├── DoctorRepository.cs
│       ├── ServiceRepository.cs
│       └── InvoiceRepository.cs
│
├── MedicalAppointmentApp.Business/        # Business Logic Layer
│   ├── Interfaces/                        # Service contracts
│   │   ├── IPatientService.cs
│   │   ├── IAppointmentService.cs
│   │   ├── IDoctorService.cs
│   │   ├── IServiceService.cs
│   │   └── IInvoiceService.cs
│   └── Services/                          # Service implementations
│       ├── PatientService.cs
│       ├── AppointmentService.cs
│       ├── DoctorService.cs
│       ├── ServiceService.cs
│       └── InvoiceService.cs
│
└── MedicalAppointmentApi/                 # Presentation Layer (Web API)
    ├── Controllers/                       # API Controllers
    │   ├── PatientsController.cs
    │   ├── AppointmentsController.cs
    │   ├── DoctorsController.cs
    │   ├── ServicesController.cs
    │   └── InvoicesController.cs
    └── Program.cs                         # Application configuration
```

## 🏗️ Architecture Layers

### 1. Models Layer (MedicalAppointmentApp.Models)
**Purpose**: Define data structures and contracts

- **Entities**: Database table representations
- **DTOs**: API request/response objects
- **Common**: Shared models like ApiResponse

**Dependencies**: 
- Supabase
- System.ComponentModel.Annotations

### 2. Data Layer (MedicalAppointmentApp.Data)
**Purpose**: Handle all database operations

- **Repositories**: Implement data access patterns
- **Database Operations**: Execute queries and stored procedures
- **Connection Management**: Handle Npgsql connections

**Responsibilities**:
- ✅ Execute SQL queries and stored procedures
- ✅ Map database results to DTOs
- ✅ Handle database connections and transactions
- ✅ Provide data access abstraction

**Dependencies**:
- Models Layer
- Supabase
- Npgsql
- Microsoft.Extensions.Configuration.Abstractions

### 3. Business Layer (MedicalAppointmentApp.Business)
**Purpose**: Implement business logic and rules

- **Services**: Orchestrate business operations
- **Validation**: Business rule enforcement
- **Coordination**: Manage complex workflows

**Responsibilities**:
- ✅ Business logic processing
- ✅ Transaction coordination
- ✅ Business rule validation
- ✅ Data transformation
- ✅ Service orchestration

**Dependencies**:
- Models Layer
- Data Layer
- Microsoft.Extensions.DependencyInjection.Abstractions

### 4. Presentation Layer (MedicalAppointmentApi)
**Purpose**: Expose HTTP API endpoints

- **Controllers**: Handle HTTP requests/responses
- **Validation**: Input validation
- **Documentation**: Swagger/OpenAPI
- **CORS**: Cross-origin configuration

**Responsibilities**:
- ✅ HTTP request routing
- ✅ Input validation
- ✅ Response formatting
- ✅ Authentication/Authorization
- ✅ API documentation

**Dependencies**:
- All layers (Models, Data, Business)
- Supabase
- Npgsql
- Swashbuckle (Swagger)

## 🔄 Data Flow

```
HTTP Request
    ↓
Controller (Validation)
    ↓
Business Service (Business Logic)
    ↓
Repository (Data Access)
    ↓
Database (Supabase/PostgreSQL)
    ↓
Entity Models
    ↓
Response DTOs
    ↓
HTTP Response
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL/Supabase database
- Visual Studio 2022 or VS Code

### Build the Solution

```powershell
# Navigate to backend directory
cd backend

# Restore packages
dotnet restore

# Build all projects
dotnet build

# Run the API
cd MedicalAppointmentApi
dotnet run
```

### API Endpoints

The API runs on `http://localhost:5236` (or configured port)

**Swagger Documentation**: `http://localhost:5236`

#### Patients
- `GET /api/patients` - Get all patients
- `POST /api/patients` - Create new patient

#### Appointments
- `GET /api/appointments` - Get all appointments
- `POST /api/appointments` - Schedule appointment
- `POST /api/appointments/{id}/complete` - Complete appointment with billing

#### Doctors
- `GET /api/doctors` - Get all doctors
- `GET /api/doctors/{id}/schedule` - Get doctor schedule
- `GET /api/doctors/{id}/available-slots` - Get available time slots

#### Services
- `GET /api/services` - Get all medical services

#### Invoices
- `GET /api/invoices` - Get all invoices
- `POST /api/invoices` - Create new invoice

## 📦 Package Dependencies

### MedicalAppointmentApp.Models
```xml
<PackageReference Include="Supabase" Version="1.0.0" />
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
```

### MedicalAppointmentApp.Data
```xml
<PackageReference Include="Supabase" Version="1.0.0" />
<PackageReference Include="Npgsql" Version="9.0.4" />
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
```

### MedicalAppointmentApp.Business
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.2" />
```

### MedicalAppointmentApi
```xml
<PackageReference Include="Supabase" Version="1.0.0" />
<PackageReference Include="Npgsql" Version="9.0.4" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.9.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
```

## ⚙️ Configuration

### appsettings.json
```json
{
  "Supabase": {
    "Url": "your-supabase-url",
    "Key": "your-supabase-key"
  },
  "ConnectionStrings": {
    "Supabase": "your-postgresql-connection-string"
  }
}
```

## 🧪 Testing

### Unit Testing Strategy

1. **Repository Tests**: Mock Npgsql connections
2. **Service Tests**: Mock repository interfaces
3. **Controller Tests**: Mock service interfaces
4. **Integration Tests**: Use test database

## 🎨 Design Patterns Used

1. **Repository Pattern**: Data access abstraction
2. **Dependency Injection**: Loose coupling
3. **Service Layer Pattern**: Business logic separation
4. **DTO Pattern**: Data transfer optimization
5. **Interface Segregation**: SOLID principles

## ✨ Benefits

### Maintainability
- ✅ Clear separation of concerns
- ✅ Easy to locate and modify code
- ✅ Single responsibility per class

### Testability
- ✅ Each layer independently testable
- ✅ Easy to mock dependencies
- ✅ Isolated unit tests

### Scalability
- ✅ Easy to add new features
- ✅ Horizontal scaling ready
- ✅ Microservices ready

### Flexibility
- ✅ Swap implementations easily
- ✅ Multiple presentation layers possible
- ✅ Database-agnostic design

## 🔐 Security Considerations

- Input validation at controller level
- Parameterized queries prevent SQL injection
- Environment-based configuration
- CORS policy configured for Angular frontend

## 📊 Performance Optimizations

- Async/await throughout
- Connection pooling with Npgsql
- DTO projections reduce data transfer
- Scoped dependency injection

## 🚀 Deployment

### Docker Support (Future)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "MedicalAppointmentApi.dll"]
```

### Environment Variables
```
SUPABASE_URL=<your-url>
SUPABASE_KEY=<your-key>
CONNECTION_STRING=<your-connection>
```

## 📚 Additional Resources

- [BACKEND_ARCHITECTURE.md](../BACKEND_ARCHITECTURE.md) - Detailed architecture documentation
- [DEVELOPMENT.md](../DEVELOPMENT.md) - Development guide
- [TESTING.md](../TESTING.md) - Testing strategies

## 🤝 Contributing

1. Follow the layered architecture
2. Add interfaces before implementations
3. Write unit tests for new features
4. Update documentation

## 📝 License

This project is part of the Medical Appointment Application system.

---

**Built with ❤️ using .NET 8, Clean Architecture, and best practices**
