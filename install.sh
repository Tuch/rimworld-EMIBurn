#!/bin/bash

echo "📦 Installing EMIBurn mod for RimWorld..."

MOD_NAME="EMIBurn"
DLL_NAME="$MOD_NAME.dll"
BUILD_OUTPUT="Assemblies"

# Path to RimWorld Mods folder
TARGET_MOD_DIR="/Users/tuch/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Mods/$MOD_NAME"

# Make sure the build exists
if [ ! -f "$BUILD_OUTPUT/net472/$DLL_NAME" ]; then
    echo "❌ File $BUILD_OUTPUT/$DLL_NAME not found. Run ./build.sh first"
    exit 1
fi

# Create destination folder and subfolders
mkdir -p "$TARGET_MOD_DIR/Assemblies"

# Copy .dll
cp "$BUILD_OUTPUT/net472/$DLL_NAME" "$TARGET_MOD_DIR/Assemblies/"

# Copy all mod files except source code and build artifacts
echo "📁 Copying mod files..."
rsync -av --exclude 'Source' --exclude 'bin' --exclude 'obj' --exclude 'build.sh' --exclude 'install.sh' . "$TARGET_MOD_DIR/"

echo "✅ Mod installed to:"
echo "$TARGET_MOD_DIR"
