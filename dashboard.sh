#!/bin/sh
# Bygger och testar din kod och visar din progression (fungerar utan internet).
cd "$(dirname "$0")"
dotnet run --project verktyg/Dashboard -- "$@"
