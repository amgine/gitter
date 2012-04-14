@del /Q /A:H *.suo
@del /Q /S *.ncb
@del /Q /S *.cache
@del /Q /S *.user
@del /Q /S *.resharper
@del /Q /S help\*

@rmdir /Q /S output\Release
@rmdir /Q /S output\Debug

@for /D %%i in (*.prj) do @(
	@cd %%i
	@rmdir /Q /S bin
	@rmdir /Q /S obj
	@if exist clear.cmd @call clear
	@cd ..
)
