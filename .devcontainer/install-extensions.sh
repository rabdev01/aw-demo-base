#!/bin/bash
# Installs a pinned version of C# Dev Kit (last .NET 9-compatible 1.x release).
# All failures are non-fatal so the dev container always finishes starting.

set -euo pipefail

PUBLISHER="ms-dotnettools"
EXTENSION="csdevkit"

# Detect platform for the correct VSIX download
ARCH=$(uname -m)
case "$ARCH" in
  aarch64|arm64) PLATFORM="linux-arm64" ;;
  x86_64)        PLATFORM="linux-x64"   ;;
  *)             PLATFORM="linux-x64"   ;;
esac

echo "[ext-install] Querying marketplace for latest 1.x C# Dev Kit version..."
VERSION=$(curl -sf -X POST \
  'https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery' \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json;api-version=3.0-preview.1' \
  -d "{
    \"filters\": [{
      \"criteria\": [{\"filterType\": 7, \"value\": \"${PUBLISHER}.${EXTENSION}\"}],
      \"pageNumber\": 1,
      \"pageSize\": 1,
      \"sortBy\": 0,
      \"sortOrder\": 0
    }],
    \"assetTypes\": [],
    \"flags\": 1
  }" \
  | python3 -c "
import sys, json
data = json.load(sys.stdin)
versions = data['results'][0]['extensions'][0]['versions']
v1x = [v['version'] for v in versions if v['version'].startswith('1.')]
print(v1x[0] if v1x else '')
" 2>/dev/null) || VERSION=""

if [ -z "$VERSION" ]; then
  echo "[ext-install] Could not determine version — skipping pinned install."
  exit 0
fi

echo "[ext-install] Installing C# Dev Kit ${VERSION} (${PLATFORM})..."
VSIX_URL="https://marketplace.visualstudio.com/_apis/public/gallery/publishers/${PUBLISHER}/vsextensions/${EXTENSION}/${VERSION}/vspackage?targetPlatform=${PLATFORM}"

if curl -fsSL "$VSIX_URL" -o /tmp/csdevkit.vsix 2>/dev/null; then
  if code --install-extension /tmp/csdevkit.vsix --force 2>/dev/null; then
    echo "[ext-install] C# Dev Kit ${VERSION} installed successfully."
  else
    echo "[ext-install] code --install-extension failed — skipping."
  fi
else
  echo "[ext-install] Download failed — skipping."
fi

rm -f /tmp/csdevkit.vsix
