#!/bin/bash
set -euo pipefail

echo "📦 Installing EMIBurn mod for RimWorld..."

MOD_NAME="EMIBurn"
DLL_PATH="Assemblies/net472/$MOD_NAME.dll"
TARGET_MOD_DIR="/Users/tuch/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Mods/$MOD_NAME"

# Make sure the build exists
if [ ! -f "$DLL_PATH" ]; then
    echo "❌ $DLL_PATH not found. Run ./build.sh first"
    exit 1
fi

# Safety guard: never rm -rf an empty/unexpected path
case "$TARGET_MOD_DIR" in
    */Mods/"$MOD_NAME") ;;
    *) echo "❌ Refusing to touch unexpected target: $TARGET_MOD_DIR"; exit 1 ;;
esac

# Clean any previous install (removes stale/duplicate DLLs and junk)
rm -rf "$TARGET_MOD_DIR"
mkdir -p "$TARGET_MOD_DIR/Assemblies"

# Exactly one copy of the assembly, at the top level of Assemblies/.
# RimWorld loads Assemblies/ recursively, so a second copy in net472/ would
# load the mod twice (duplicate Harmony patches / type clashes) — avoid that.
cp "$DLL_PATH" "$TARGET_MOD_DIR/Assemblies/$MOD_NAME.dll"

# Copy the shippable mod content, excluding source, build output, VCS and junk.
echo "📁 Copying mod files..."
rsync -a \
    --exclude 'Source' \
    --exclude 'Assemblies' \
    --exclude 'obj' \
    --exclude 'bin' \
    --exclude '.git' \
    --exclude '.gitignore' \
    --exclude '.DS_Store' \
    --exclude 'a.txt' \
    --exclude '*.sh' \
    --exclude '*.md' \
    --exclude '/emiBURN.png' \
    ./ "$TARGET_MOD_DIR/"

echo "✅ Mod installed to:"
echo "$TARGET_MOD_DIR"
echo "   Restart RimWorld and enable EMIBurn (below Harmony) in the mod list."
