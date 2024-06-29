# WC3CheatDetector
A console app tool for detecting injected cheatpacks in Warcraft III map files. It can be used while in a game or on standalone w3x files. The purpose of this tool is to avoid joining or accidently hosting cheated maps.

## Requirements
* Requires .NET core 3.1. https://dotnet.microsoft.com/download
* Currently only Windows 10 x64 is supported. Other versions of Windows may work as well but are not actively tested on.
* For building the solution I recommend Visual Studio 2019 or 2022.

## Settings

### App Config
The App Config file (WC3CheatDetector.dll.config in the output build folder or App.config in the solution) contains settings for the program. Here are the definitions for the setting:
* **InGameMode**: Whether or not to use InGameMode. If true (InGameMode), attempts to find the current map used by the game and checks for cheats on that file. If false (InputDirMode), looks for map files in the directory specified by InputDir.
* **InputDir**: Path to the input directory to check for maps in. Only used in InputDirMode.
* **MapFilter**: A standard file filter to filter on the maps in the InputDir. Only used in InputDirMode.
* **CheckSubfolders**: Whether to check subfolders in the InputDir or just the top level directory. Only used in InputDirMode.
* **OutputDir**: Path to the output directory to put the output files.
* **LogLevel**: The level of logs to display to the console. Values: DEBUG, INFO, WARN, ERROR, OFF.

### WhiteList/BlackList
The WhiteList.json and BlackList.json contains a list of WhiteListed and BlackListed maps. If a scanned map's MD5 hash matches an entry in the whitelist or the blacklist it will be noted as such in the log output.

## Usage

### InGameMode
1. Modify the AppConfig setting InGameMode setting to be true.
2. Join a Warcraft III game.
3. Run WC3CheatDetector.exe.
4. The map file will be scanned and potential cheat warnings will be logged to the console. The JASS script file and a JSus file (a subset of the suspicious lines from the JASS file) will be created in the output.

**Note: This finds the InGame map file by checking which .w3x file the Warcraft III process has locked.

### InputDirMode
1. Modify the AppConfig setting InGameMode setting to be false.
2. Put Warcraft 3 map files (.w3x) into the Input directory.
3. Run WC3CheatDetector.exe
4. Each map file will be scanned and potential cheat warnings will be logged to the console. The JASS script file and a JSus file (a subset of the suspicious lines from the JASS file) will be created in the output for each map file.


### FAQ
TBD