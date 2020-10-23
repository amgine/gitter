@echo Postbuild
if not exist "..\..\..\..\output\%1\" @mkdir "..\..\..\..\output\%1"
@xcopy /Y gitter.exe "..\..\..\..\output\%1\"
@xcopy /Y *.dll "..\..\..\..\output\%1\"
@xcopy /Y gitter.exe.config "..\..\..\..\output\%1\"
if not exist "..\..\..\..\output\%1\Hunspell\" @mkdir "..\..\..\..\output\%1\Hunspell"
@xcopy /Y Hunspell\*.dll "..\..\..\..\output\%1\Hunspell\"
if not exist "..\..\..\..\output\%1\Dictionaries\" @mkdir "..\..\..\..\output\%1\Dictionaries"
@xcopy /Y Dictionaries\*.aff "..\..\..\..\output\%1\Dictionaries\"
@xcopy /Y Dictionaries\*.dic "..\..\..\..\output\%1\Dictionaries\"
@exit 0
