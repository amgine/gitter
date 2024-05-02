@echo Postbuild
if not exist "..\..\..\..\output\%1\%2\" @mkdir "..\..\..\..\output\%1\%2"
@xcopy /Y gitter.updater.exe "..\..\..\..\output\%1\%2\" /q
@xcopy /Y *.dll "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.updater.exe.config "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.updater.runtimeconfig.json "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.updater.deps.json "..\..\..\..\output\%1\%2\" /q
@exit 0
