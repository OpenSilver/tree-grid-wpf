@ECHO off

SETLOCAL

IF "%~1" == "--help" (
	GOTO :help
)

IF "%~1" == "-h" (
	GOTO :help
)

SET CFG=Release
SET BUILD_DIR=%~dp0
SET SRC_DIR=%~dp0..\src

REM Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

REM Define the PackageVersion, OpenSilverPkgVersion and WpfCompatPkgVersion variables
SET OpenSilverPkgVersion=
SET WpfCompatPkgVersion=
IF "%~1" == "" (
	SET /P PackageVersion="%ESC%[92mTreeGrid.OpenSilver version:%ESC%[0m "
	SET /P OpenSilverPkgVersion="%ESC%[92mOpenSilver version (empty for the one in OpenSilver.props):%ESC%[0m "
	SET /P WpfCompatPkgVersion="%ESC%[92mOpenSilver.WpfCompat version (empty for the one in OpenSilver.props):%ESC%[0m "
) ELSE (
	SET PackageVersion=%~1
	SET OpenSilverPkgVersion=%~2
	SET WpfCompatPkgVersion=%~3
)

REM An empty version would replace the one in OpenSilver.props, so each is only passed when given.
SET VersionProperties=
IF NOT "%OpenSilverPkgVersion%" == "" SET VersionProperties=%VersionProperties% -p:OpenSilverVersion=%OpenSilverPkgVersion%
IF NOT "%WpfCompatPkgVersion%" == "" SET VersionProperties=%VersionProperties% -p:OpenSilverWpfCompatVersion=%WpfCompatPkgVersion%

ECHO. 
ECHO %ESC%[95mBuilding and packing %ESC%[0mTreeGrid.OpenSilver %ESC%[95min %ESC%[0m%CFG% %ESC%[95mconfiguration%ESC%[0m
ECHO. 
REM The package is built from this fork, so its repository URL points here rather than to the upstream repo.
dotnet pack "%SRC_DIR%\TreeGrid.OpenSilver\TreeGrid.OpenSilver.csproj" -c %CFG% -p:PackageVersion=%PackageVersion%%VersionProperties% -p:RepositoryUrl=https://github.com/OpenSilver/tree-grid-wpf -o "%BUILD_DIR%output" -v minimal --nologo
IF ERRORLEVEL 1 EXIT /b 1

EXIT /b

:help
ECHO [1] TreeGrid.OpenSilver version
ECHO [2] OpenSilver version (default: the one in OpenSilver.props)
ECHO [3] OpenSilver.WpfCompat version (default: the one in OpenSilver.props)

ENDLOCAL
