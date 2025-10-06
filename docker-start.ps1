#!/usr/bin/env pwsh

Write-Host "🐳 Medical Appointment Application - Docker Deployment" -ForegroundColor Green
Write-Host ""

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "⚠️  .env file not found. Creating from .env.example..." -ForegroundColor Yellow
    if (Test-Path ".env.example") {
        Copy-Item ".env.example" ".env"
        Write-Host "✅ Created .env file. Please update it with your actual values." -ForegroundColor Green
        Write-Host "📝 Edit .env file and run this script again." -ForegroundColor Cyan
        exit
    } else {
        Write-Host "❌ .env.example not found. Please create .env manually." -ForegroundColor Red
        exit 1
    }
}

# Build and start containers
Write-Host "🔨 Building Docker images..." -ForegroundColor Cyan
docker-compose build --no-cache

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 Starting containers..." -ForegroundColor Cyan
    docker-compose up -d
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Containers started successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📍 Application URLs:" -ForegroundColor White
        Write-Host "   🔧 Backend API: http://localhost:5236" -ForegroundColor Cyan
        Write-Host "   🌐 Frontend App: http://localhost:4200" -ForegroundColor Cyan
        Write-Host "   📚 API Documentation: http://localhost:5236/swagger" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "📊 View logs:" -ForegroundColor Yellow
        Write-Host "   docker-compose logs -f" -ForegroundColor Gray
        Write-Host ""
        Write-Host "🛑 Stop containers:" -ForegroundColor Yellow
        Write-Host "   docker-compose down" -ForegroundColor Gray
        Write-Host ""
    } else {
        Write-Host "❌ Failed to start containers" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}
