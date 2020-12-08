@echo Looking for MSBuild...
@for %%f in (
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\amd64\MSBuild.exe"
	"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\MSBuild.exe"
) do @if exist %%f (
	@echo MSBuild found at %%f
	@set msbuild=%%f /m /verbosity:normal /nologo
	@goto msbuildfound
)
@goto msbuildnotfound
@echo MSBuild was not found
@exit /b 1

:msbuildfound
@echo Building gitter...
@if exist "output\Release" @rd /s /q "output\Release"
@%msbuild% master.build /t:Restore %*
@%msbuild% master.build /t:BuildRelease %*
@pause
