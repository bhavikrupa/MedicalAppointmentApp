FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["MedicalAppointmentApi/MedicalAppointmentApi.csproj", "MedicalAppointmentApi/"]
COPY ["MedicalAppointmentApp.Business/MedicalAppointmentApp.Business.csproj", "MedicalAppointmentApp.Business/"]
COPY ["MedicalAppointmentApp.Data/MedicalAppointmentApp.Data.csproj", "MedicalAppointmentApp.Data/"]
COPY ["MedicalAppointmentApp.Models/MedicalAppointmentApp.Models.csproj", "MedicalAppointmentApp.Models/"]

# Restore dependencies
RUN dotnet restore "MedicalAppointmentApi/MedicalAppointmentApi.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/MedicalAppointmentApi"
RUN dotnet build "MedicalAppointmentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MedicalAppointmentApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MedicalAppointmentApi.dll"]
