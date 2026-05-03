@echo Postbuild
if not exist "..\..\..\..\output\%1\%2\" @mkdir "..\..\..\..\output\%1\%2"
@xcopy /Y gitter.rebase-editor.exe "..\..\..\..\output\%1\%2\" /q
@xcopy /Y *.dll "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.rebase-editor.exe.config "..\..\..\..\output\%1\%2\" /q 2>nul
@xcopy /Y gitter.rebase-editor.runtimeconfig.json "..\..\..\..\output\%1\%2\" /q 2>nul
@xcopy /Y gitter.rebase-editor.deps.json "..\..\..\..\output\%1\%2\" /q 2>nul
@exit 0
