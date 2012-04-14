@echo Postbuild
if not exist "..\..\..\output\%1" @mkdir "..\..\..\output\%1"
if not exist "..\..\..\output\%1\Hunspell" @mkdir "..\..\..\output\%1\Hunspell"
if not exist "..\..\..\output\%1\Dictionaries" @mkdir "..\..\..\output\%1\Dictionaries"
@copy /Y Hunspell\Hunspell*.dll "..\..\..\output\%1\Hunspell"
@copy /Y Dictionaries\*.* "..\..\..\output\%1\Dictionaries"
@exit 0
