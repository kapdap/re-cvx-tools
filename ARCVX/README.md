# ARCVX

ARCVX is a command line utility for extracting and repacking Resident Evil/Biohazard: Code: Veronica X HD .arc files.

## Releases

Download latest release [here](https://github.com/kapdap/re-cvx-tools/releases/download/arcvx-snapshot-2024.3.20.1/arcvx-snapshot-2024.3.20.1.zip).

## Usage

### Extracting

Simply drag and drop a .arc file or a folder containing .arc files onto **ARCVX.exe**.<br>
Extracted files will be placed in a folder named "**\<file/folder name\>.extract**".

### Repacking

Drag and drop the "**\<file/folder name\>.extract**" folder onto **ARCVX.exe** to repack.<br>
Entries in archives will be replaced if a file exists with the same relative path in the extract folder.

## Features

- Extract HFS containers
- Repack HFS container
- Extract ARC containers
- Repack ARC containers
- Convert textures to .dds
- Convert messages to .txt (US, GB, ES and custom languages supported)
- Rebuild messages from .txt

## TODO

- Convert scripts
- Rebuild scripts
- Rebuild textures from .dds
- Decode/encode JP, DE, FR, IT messages

# Links

[Kuriimu2](https://github.com/FanTranslatorsInternational/Kuriimu2) - Full HFS/ARC read/write support with GUI.
