@echo Postbuild
if not exist "..\..\..\..\output\%1\" @mkdir "..\..\..\..\output\%1"
@xcopy /Y gitter.uac.exe "..\..\..\..\output\%1\"
@xcopy /Y *.dll "..\..\..\..\output\%1\"
@xcopy /Y gitter.uac.exe.config "..\..\..\..\output\%1\"
@exit 0
