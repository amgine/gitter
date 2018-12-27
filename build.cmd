@echo Looking for MSBuild...
@for %%f in (
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\MSBuild\14.0\Bin\amd64\MSBuild.exe" 
	"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
	"%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
	"%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
) do @if exist %%f (
	@echo MSBuild found at %%f
	@set msbuild=%%f /m /verbosity:normal /nologo
	@goto msbuildfound
)
@echo MSBuild was not found
@exit /b 1

:msbuildfound
@echo Building gitter...
@if exist "output\Release" @rd /s /q "output\Release"
@%msbuild% master.build /t:BuildRelease %*
@pause
