#!/bin/bash

echo "🐳 Medical Appointment Application - Docker Deployment"
echo ""

# Check if .env file exists
if [ ! -f .env ]; then
    echo "⚠️  .env file not found. Creating from .env.example..."
    if [ -f .env.example ]; then
        cp .env.example .env
        echo "✅ Created .env file. Please update it with your actual values."
        echo "📝 Edit .env file and run this script again."
        exit 0
    else
        echo "❌ .env.example not found. Please create .env manually."
        exit 1
    fi
fi

# Build and start containers
echo "🔨 Building Docker images..."
docker-compose build --no-cache

if [ $? -eq 0 ]; then
    echo "✅ Build completed successfully!"
    echo ""
    echo "🚀 Starting containers..."
    docker-compose up -d
    
    if [ $? -eq 0 ]; then
        echo ""
        echo "✅ Containers started successfully!"
        echo ""
        echo "📍 Application URLs:"
        echo "   🔧 Backend API: http://localhost:5236"
        echo "   🌐 Frontend App: http://localhost:4200"
        echo "   📚 API Documentation: http://localhost:5236/swagger"
        echo ""
        echo "📊 View logs:"
        echo "   docker-compose logs -f"
        echo ""
        echo "🛑 Stop containers:"
        echo "   docker-compose down"
        echo ""
    else
        echo "❌ Failed to start containers"
        exit 1
    fi
else
    echo "❌ Build failed"
    exit 1
fi
