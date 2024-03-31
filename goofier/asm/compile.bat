@echo off
echo "%~1"
echo "%~1.bin"
"%~dp0\internal\customasm.exe" "%~1" -o "%~1.bin"
"%~dp0\internal\Microcode gen.exe" "%~1.bin"
mv "%~1.bin.bin" "%~1.bin"
pause