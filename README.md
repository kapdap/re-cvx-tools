# RDX Web Viewer

> [!WARNING]
> This is an experimental, AI-generated, vibe-slop project.

RDX Web Viewer is a browser-based viewer for Resident Evil/BioHazard: Code: Veronica `.rdx` and `.rdt` files. It displays scene models in a 3D viewport, provides panels for scene data, and exports models as glTF 2.0 binary files.

https://kapdap.github.io/re-cvx-tools/

## Run

There is no install or build step. Open `index.html` in a desktop browser with WebGL support. The app runs offline.

Load files with **File > Open** or drag `.rdx` and `.rdt` files onto the page. You can load multiple files and switch between them in the Files panel.

## Features

- Inspect models, textures, cameras, lights, players, boundaries, AOTs, triggers, and routes.
- Switch between textured, wireframe, and lighting render modes.
- Navigate in Orbit or FPV camera mode.
- Export models as `.glb` files
- Export textures from **File > Export > Textures**.

## Controls

**Orbit mode**

- Middle-drag to orbit; Shift + middle-drag to pan.
- Use the wheel to zoom or right-drag to pan.
- Click a model to select it. Ctrl-click adds to the selection, and double-click opens details.
- Right-click for a context menu. Press Esc to clear the selection.

**FPV mode**

Choose **View > Camera > FPV**, then click the viewport to capture the mouse. Use WASD to move, Space to move up, and Ctrl or C to move down. Hold Shift to move faster; use the wheel to change movement speed.
