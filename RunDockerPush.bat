@echo off
For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%a%%b)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME%") do (set mytime=%%a%%b)
SET datetime=%mydate%%mytime%
echo %datetime%
DockerImageBuild.bat dev-%datetime%