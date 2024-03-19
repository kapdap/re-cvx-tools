# ARCVX

ARCVX is a command line utility for extracting and rebuilding Resident Evil/Biohazard: Code: Veronica X HD .arc files.

## Usage

### Extracting

Simply drag and drop a .arc file or a folder containing .arc files onto **ARCVX.exe**.<br>
Extracted files will be placed in a folder named "**\<file/folder name\>.extract**".

### Repacking

Drag and drop the "**\<file/folder name\>.extract**" folder onto **ARCVX.exe** to repack.<br>
Entries in archives will be replaced if a file exists with the same relative path in the extract folder.

## Features

- Extract HFS containers
- Extract ARC containers
- Repack HFS containers
- Repack ARC containers
- Convert textures to .dds
- Convert messages to .txt (US-only)
- Rebuild messages from .txt (US-only)

## TODO

- Convert scripts
- Rebuild scripts
- Rebuild textures
- Convert non-US messages
- Custom language tables

# Links

[Kuriimu2](https://github.com/FanTranslatorsInternational/Kuriimu2) - Full HFS/ARC read/write support with GUI.
