#!/usr/bin/env pwsh

Write-Host "🛑 Stopping Medical Appointment Application containers..." -ForegroundColor Yellow
docker-compose down

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Containers stopped successfully!" -ForegroundColor Green
} else {
    Write-Host "❌ Failed to stop containers" -ForegroundColor Red
    exit 1
}
