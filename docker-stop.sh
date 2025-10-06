#!/bin/bash

echo "🛑 Stopping Medical Appointment Application containers..."
docker-compose down

if [ $? -eq 0 ]; then
    echo "✅ Containers stopped successfully!"
else
    echo "❌ Failed to stop containers"
    exit 1
fi
