@echo Postbuild
if not exist "..\..\..\..\output\%1\%2\" @mkdir "..\..\..\..\output\%1\%2"
@xcopy /Y gitter.askpass.exe "..\..\..\..\output\%1\%2\" /q
@xcopy /Y *.dll "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.askpass.exe.config "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.askpass.runtimeconfig.json "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.askpass.deps.json "..\..\..\..\output\%1\%2\" /q
@exit 0
