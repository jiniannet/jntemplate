#!/usr/bin/env bash
# scripts/fix-target-framework.sh
# AppVeyor: temporarily downgrade net11.0 -> net10.0 before build
set -euo pipefail

# 1) cd to the clone folder (APPVEYOR_BUILD_FOLDER is the clone dir)
cd "${APPVEYOR_BUILD_FOLDER}"
echo "Working dir: $(pwd)"

PROJ1="src/JinianNet.JNTemplate/JinianNet.JNTemplate.csproj"
PROJ2="src/JinianNet.JNTemplate.Test/JinianNet.JNTemplate.Test.csproj"

# 2) JinianNet.JNTemplate.csproj: "net11.0;net10.0" -> "net10.0"
#    Must do this FIRST, otherwise "net11.0" inside the pair would match.
sed -i 's/net11\.0;net10\.0/net10\.0/g' "$PROJ1"
echo "Patched: $PROJ1"

# 3) JinianNet.JNTemplate.Test.csproj: "net11.0" -> "net10.0"
sed -i 's/net11\.0/net10\.0/g' "$PROJ2"
echo "Patched: $PROJ2"

echo "Done. Target frameworks are now .NET 10 only."