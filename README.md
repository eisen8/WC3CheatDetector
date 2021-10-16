# WC3CheatTools
WC3CheatTools is a set of tools for Warcraft III for interacting with cheated maps.

## WC3CheatDetector
A tool for detecting injected cheats within a Warcraft III Map File. Configuration is defined in the AppConfig file.
### Settings:
* **InGameMode**: Whether or not to use InGameMode. If true, attempts to find the current map used by the game and checks for cheats on that file. If false, looks for map files in the directory specified by InputDir.
* **InputDir**: Path to the input directory to check for maps in. Not used InGameMode.
* **MapFilter**: A standard file filter to filter on the maps in the InputDir.
* **CheckSubfolders**: Whether to check subfolders in the InputDir or just the top level directory.
* **OutputDir**: Path to the output directory to put the output files.
* **LogLevel**: The level of logs to display to the console. Values: DEBUG, INFO, WARN, ERROR, OFF.

### Usage
#### InGameMode
1. Modify the AppConfig settings as needed.
2. Join a Warcraft III game.
3. Run WC3CheatDetector.exe.
4. Cheat warnings will be logged to the console. The raw JASS file and a JSus file (a subset of the suspicious lines from the JASS file) will be created in the output. Inspect the console output and the output JASS files to see if cheats are present.

**Notes: This finds the InGame map file by checking which .w3x file the Warcraft III process has locked. Usually, WC3 locks only the map file you are currently playing. However, there are issues with the Warcraft III client where sometimes it locks the wrong file or doesn't lock any file at all. So this method will not always work. Restarting the Warcraft process or leaving/rejoining a game can fix this issue.**

#### InputDirMode
* Modify the AppConfig settings as needed.
* Put Warcraft 3 map files (.w3x) into the Input directory.
* Run WC3CheatDetector.exe
* Cheat warnings will be logged to the console. The raw JASS file and a JSus file (a subset of the suspicious lines from the JASS file) will be created in the output. Inspect the console output and the output JASS files to see if cheats are present.

## WC3CheatInjector
A tool for injecting a cheat pack into a Warcraft III Map File. Configuration is defined in the AppConfig file.
### Settings:
* **InputDir**: Path to the input directory to check for maps in.
* **MapFilter**: A standard file filter to filter on the maps in the InputDir.
* **CheckSubfolders**: Whether to check subfolders in the InputDir or just the top level directory.
* **OutputDir**: Path to the output directory to put the output files.
* **CheatPackDir**: Path to the directory storing the cheat packs for injection. The cheatpack should be split into 3 text files: endglobals.txt, globals.txt, and main.txt. 
* **CreateVerificationFiles**: Whether or not to create JASS files in the output (for verification). If false, just creates the cheated map file. Recommended to be true.
* **LogLevel**: The level of logs to display to the console. Values: DEBUG, INFO, WARN, ERROR, OFF.

### Usage
1. Modify the AppConfig settings as needed.
2. Put uncheated Warcraft 3 map files (.w3x) into the Input directory.
3. Put a CheatPack (endglobals.txt, globals.txt, and main.txt.) into the CheatPackDir. For CheatPack examples see http://forum.wc3edit.net/tutorials-cheatpacks-f80/cheat-packs-available-on-our-site-t5134.html .
4. Run WC3CheatInjector.exe.
5. Cheated maps will be placed in the OutputDir. Verify the map cheats work in Warcraft III.