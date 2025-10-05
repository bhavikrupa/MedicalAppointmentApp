# Development Guide

## 🛠️ Development Environment Setup

### Required Tools
- **Visual Studio Code** with extensions:
  - C# Dev Kit
  - Angular Language Service
  - Thunder Client (for API testing)
  - GitLens
  - Prettier
  - ESLint

### Database Development
- **Supabase Dashboard**: For database management and monitoring
- **SQL Editor**: Built into Supabase or use external tools like pgAdmin

## 🏗️ Project Structure

```
MedicalAppointmentApp/
├── .vscode/                    # VS Code configuration
│   └── tasks.json             # Build and run tasks
├── backend/                   # .NET Web API
│   └── MedicalAppointmentApi/
│       ├── Controllers/       # API controllers
│       ├── Models/           # Entity models
│       ├── DTOs/             # Data transfer objects
│       ├── Services/         # Business logic layer
│       └── Program.cs        # Application entry point
├── frontend/                 # Angular application
│   └── medical-appointment-app/
│       ├── src/app/
│       │   ├── components/   # UI components
│       │   ├── services/     # HTTP services
│       │   ├── models/       # TypeScript interfaces
│       │   └── app.ts        # Main app component
│       └── angular.json      # Angular configuration
├── database/                 # Database scripts
│   ├── schema.sql           # Table definitions
│   └── stored_procedures.sql # Stored procedures
└── README.md                # Project documentation
```

## 🔄 Development Workflow

### 1. Backend Development

**Starting Development:**
```bash
cd backend/MedicalAppointmentApi
dotnet watch run
```

**Testing API:**
- Use Swagger UI: `http://localhost:5236/swagger`
- Use Thunder Client extension in VS Code
- Test endpoints with sample data

**Adding New Features:**
1. Create/update models in `Models/`
2. Add DTOs in `DTOs/`
3. Implement business logic in `Services/`
4. Create controller endpoints in `Controllers/`
5. Test endpoints thoroughly

### 2. Frontend Development

**Starting Development:**
```bash
cd frontend/medical-appointment-app
ng serve
```

**Adding New Features:**
1. Create Angular components: `ng generate component component-name`
2. Add services for API communication: `ng generate service service-name`
3. Define TypeScript interfaces in `models/`
4. Implement routing in `app.routes.ts`
5. Add styling with SCSS

### 3. Database Development

**Making Schema Changes:**
1. Update `database/schema.sql`
2. Create migration scripts if needed
3. Test changes in development environment
4. Update stored procedures if necessary

**Adding Stored Procedures:**
1. Add new procedures to `database/stored_procedures.sql`
2. Update backend service methods to call new procedures
3. Test transactional behavior thoroughly

## 🧪 Testing Strategy

### Backend Testing
```bash
# Unit tests
cd backend/MedicalAppointmentApi
dotnet test

# Integration tests (to be implemented)
# Test API endpoints with real database
```

### Frontend Testing
```bash
# Unit tests
cd frontend/medical-appointment-app
ng test

# E2E tests
ng e2e
```

### Database Testing
- Test stored procedures with sample data
- Verify transactional rollback behavior
- Check constraint enforcement

## 🐛 Debugging

### Backend Debugging
- Use VS Code debugger with .NET configuration
- Check logs in console output
- Use Swagger UI for API testing
- Monitor Supabase dashboard for database queries

### Frontend Debugging
- Use Chrome DevTools
- Check Network tab for API calls
- Use Angular DevTools extension
- Monitor console for errors

### Database Debugging
- Use Supabase SQL Editor for query testing
- Check database logs in Supabase dashboard
- Use pgAdmin for advanced database management

## 📊 Performance Monitoring

### Backend Performance
- Monitor API response times
- Check database query performance
- Use .NET performance counters
- Monitor memory usage

### Frontend Performance
- Use Chrome DevTools Performance tab
- Monitor bundle sizes with Angular CLI
- Check Core Web Vitals
- Use Angular DevTools profiler

## 🔐 Security Considerations

### Backend Security
- Input validation on all endpoints
- SQL injection prevention (using parameterized queries)
- Authentication with Supabase
- CORS configuration
- Error handling without exposing sensitive data

### Frontend Security
- Input sanitization
- XSS prevention
- Secure HTTP headers
- Authentication token management

### Database Security
- Row Level Security (RLS) policies in Supabase
- Proper user permissions
- Connection string security
- Regular security updates

## 🚀 Deployment

### Development Deployment
- Backend: `dotnet run` (automatic reload with `dotnet watch run`)
- Frontend: `ng serve` (automatic reload)
- Database: Supabase cloud instance

### Production Deployment (Future)
- Backend: Docker container or Azure App Service
- Frontend: Static hosting (Netlify, Vercel, or Azure Static Web Apps)
- Database: Supabase production instance

## 📋 Code Standards

### Backend (.NET)
- Follow Microsoft coding conventions
- Use meaningful variable and method names
- Implement proper error handling
- Add XML documentation comments
- Use async/await pattern consistently

### Frontend (TypeScript/Angular)
- Follow Angular style guide
- Use TypeScript strict mode
- Implement proper component lifecycle
- Use reactive programming patterns (RxJS)
- Follow consistent naming conventions

### Database (SQL)
- Use snake_case for table and column names
- Add appropriate indexes
- Include foreign key constraints
- Document complex queries
- Use transactions for data integrity

## 🔧 Common Issues and Solutions

### Compilation Errors
1. **Supabase Model Issues**: Ensure all models inherit from `BaseModel`
2. **Namespace Issues**: Add proper using statements
3. **Package Restore**: Run `dotnet restore` if packages are missing

### Runtime Errors
1. **CORS Issues**: Check CORS configuration in Program.cs
2. **Database Connection**: Verify Supabase connection string
3. **Authentication**: Check Supabase keys and permissions

### Build Issues
1. **Angular Build**: Clear node_modules and reinstall if needed
2. **TypeScript Errors**: Check interface definitions match API models
3. **Missing Dependencies**: Run `npm install` or `dotnet restore`

## 📈 Monitoring and Logging

### Application Logging
- .NET built-in logging framework
- Structured logging with Serilog (future enhancement)
- Console output for development
- File logging for production

### Database Monitoring
- Supabase dashboard metrics
- Query performance monitoring
- Connection pool monitoring
- Storage usage tracking

### Error Tracking
- Console logging for development
- Application Insights (future enhancement)
- Error boundaries in Angular
- Centralized error handling

---

**Happy Coding! 🎉**