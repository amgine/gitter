@echo Postbuild
if not exist "..\..\..\..\output\%1\%2\" @mkdir "..\..\..\..\output\%1\%2"
@xcopy /Y gitter.uac.exe "..\..\..\..\output\%1\%2\" /q
@xcopy /Y *.dll "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.uac.exe.config "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.uac.runtimeconfig.json "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.uac.deps.json "..\..\..\..\output\%1\%2\" /q
@exit 0
