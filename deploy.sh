#!/bin/bash

set -e

echo "=================================="
echo "Starting CodeSphere deployment..."
echo "=================================="

cd /home/ubuntu/codesphere-api

echo "Pulling latest code..."
git pull origin main

echo "Publishing application..."
dotnet publish ./codesphere-api.csproj -c Release -o /home/ubuntu/codesphere-api-publish

echo "Restarting API..."
sudo systemctl restart codesphere-api

echo "Checking API status..."
sudo systemctl --no-pager status codesphere-api

echo "=================================="
echo "Deployment completed!"
echo "=================================="
