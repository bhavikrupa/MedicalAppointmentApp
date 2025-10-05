#!/usr/bin/env pwsh

Write-Host "🏥 Starting Medical Appointment Application..." -ForegroundColor Green
Write-Host ""

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $scriptDir "backend\MedicalAppointmentApi"
$frontendPath = Join-Path $scriptDir "frontend\medical-appointment-app"

# Function to start backend
function Start-Backend {
    Write-Host "🚀 Starting Backend API..." -ForegroundColor Yellow
    Start-Process -FilePath "pwsh" -ArgumentList "-NoExit", "-Command", "cd '$backendPath'; dotnet run" -WindowStyle Normal
}

# Function to start frontend  
function Start-Frontend {
    Write-Host "🌐 Starting Frontend Application..." -ForegroundColor Yellow
    Start-Process -FilePath "pwsh" -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; ng serve --port 4200 --ssr=false" -WindowStyle Normal
}

try {
    # Start backend
    Start-Backend
    
    # Wait a moment for backend to initialize
    Write-Host "⏳ Waiting for backend to start..." -ForegroundColor Cyan
    Start-Sleep -Seconds 5
    
    # Start frontend
    Start-Frontend
    
    Write-Host ""
    Write-Host "✅ Applications are starting up!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📍 Application URLs:" -ForegroundColor White
    Write-Host "   🔧 Backend API: http://localhost:5236" -ForegroundColor Cyan
    Write-Host "   🌐 Frontend App: http://localhost:4200" -ForegroundColor Cyan  
    Write-Host "   📚 API Documentation: http://localhost:5236/swagger" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "💡 Tip: Both applications will open in separate terminal windows" -ForegroundColor Yellow
    Write-Host "💡 Press Ctrl+C in each terminal window to stop the applications" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Press any key to continue..." -ForegroundColor White
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}
catch {
    Write-Error "Failed to start applications: $($_.Exception.Message)"
    exit 1
}