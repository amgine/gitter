@echo Postbuild
if not exist "..\..\..\output\%1" @mkdir "..\..\..\output\%1"
@xcopy /Y gitter.askpass.exe "..\..\..\output\%1\"
@xcopy /Y *.dll "..\..\..\output\%1\"
@xcopy /Y gitter.askpass.exe.config "..\..\..\output\%1\"
@exit 0
