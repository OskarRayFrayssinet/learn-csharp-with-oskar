@echo off
rem Bygger och testar din kod och visar din progression (fungerar utan internet).
cd /d "%~dp0"
dotnet run --project verktyg\Dashboard -- %*
pause
