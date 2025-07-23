#!/bin/bash

echo "🔨 Building EMIBurn mod..."

MOD_NAME="EMIBurn"
SOURCE_DIR="Source"
BUILD_OUTPUT="Assemblies"
DLL_NAME="$MOD_NAME.dll"

# Build .dll
dotnet build "$SOURCE_DIR/$MOD_NAME.csproj" --configuration Release

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

# Make sure Assemblies directory exists
mkdir -p "$BUILD_OUTPUT"

echo "✅ Build successful: $BUILD_OUTPUT/$DLL_NAME"
