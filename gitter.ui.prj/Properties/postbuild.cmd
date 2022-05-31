@echo Postbuild
if not exist "..\..\..\..\output\%1\%2\" @mkdir "..\..\..\..\output\%1\%2"
@xcopy /Y gitter.exe "..\..\..\..\output\%1\%2\" /q
@xcopy /Y *.dll "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.exe.config "..\..\..\..\output\%1\%2\" /q
@xcopy /Y gitter.runtimeconfig.json "..\..\..\..\output\%1\%2\" /q
if not exist "..\..\..\..\output\%1\%2\Hunspell\" @mkdir "..\..\..\..\output\%1\%2\Hunspell"
@xcopy /Y Hunspell\*.dll "..\..\..\..\output\%1\%2\Hunspell\" /q
if not exist "..\..\..\..\output\%1\%2\Dictionaries\" @mkdir "..\..\..\..\output\%1\%2\Dictionaries"
@xcopy /Y Dictionaries\*.aff "..\..\..\..\output\%1\%2\Dictionaries\" /q
@xcopy /Y Dictionaries\*.dic "..\..\..\..\output\%1\%2\Dictionaries\" /q
@exit 0
