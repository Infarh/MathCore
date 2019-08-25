@echo off

dotnet build -c RELEASE -v q --force

dotnet test

pause