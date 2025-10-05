@echo off
echo Starting Medical Appointment Application...
echo.

echo Starting Backend API...
start "Backend API" cmd /k "cd /d ""c:\Users\NBITBBR\OneDrive - NBFC\Documents\Temp-Personal\Projects\MedicalAppointmentApp\backend\MedicalAppointmentApi"" && dotnet run"

echo Waiting for backend to start...
timeout /t 5 /nobreak > nul

echo Starting Frontend Application...
start "Frontend App" cmd /k "cd /d ""c:\Users\NBITBBR\OneDrive - NBFC\Documents\Temp-Personal\Projects\MedicalAppointmentApp\frontend\medical-appointment-app"" && ng serve --port 4200 --ssr=false"

echo.
echo Applications are starting...
echo Backend API will be available at: http://localhost:5236
echo Frontend App will be available at: http://localhost:4200
echo API Documentation at: http://localhost:5236/swagger
echo.
echo Press any key to continue...
pause > nul