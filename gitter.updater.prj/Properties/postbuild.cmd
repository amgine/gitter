@echo Postbuild
if not exist "..\..\..\output\%1\" @mkdir "..\..\..\output\%1"
@xcopy /Y gitter.updater.exe "..\..\..\output\%1\"
@xcopy /Y *.dll "..\..\..\output\%1\"
@xcopy /Y gitter.updater.exe.config "..\..\..\output\%1\"
@exit 0
