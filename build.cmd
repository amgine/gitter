@set DOTNET_CLI_UI_LANGUAGE=en
@echo Building gitter...
@if exist "output\Release" @rd /s /q "output\Release"
@dotnet build --configuration Release gitter.sln
@pause
