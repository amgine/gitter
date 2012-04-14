@echo Building gitter...
@set MSBUILD=%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /verbosity:minimal /nologo
@if exist "output\Release" @rd /s /q "output\Release"
@%MSBUILD% master.build /t:BuildRelease %*
@pause
