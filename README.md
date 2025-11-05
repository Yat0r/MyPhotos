# MyPhotos

Small sample photo album application implemented in C# for .NET Framework 4.8.
This repository contains two main parts:
- `MyPhotoAlbum` — core data model types (e.g. `PhotoAlbum`, `Photograph`, `AlbumManager`, `AlbumStorage`).
- `MyPhotoControls` — WinForms UI controls / dialogs that use the album types.

## Features
- Load/save simple text-format photo albums via `AlbumStorage`.
- Edit photo metadata (caption, date taken, photographer, notes).
- Basic UI dialogs and controls for viewing and editing photos.

## Requirements
- Visual Studio (2017/2019/2022) or compatible IDE that targets .NET Framework 4.8.
- .NET Framework 4.8 developer targeting pack.
## Third‑party libraries
- `Bunifu.UI.WinForms` — optional third‑party WinForms UI library used by the project (v9.1.0).
  - This project uses NuGet package `Bunifu.UI.WinForms`. If your UI projects (for example `MyPhotoControls` and `MyPhotos`) use Bunifu controls, install the package into those projects as well.

## Installing Bunifu
- In Visual Studio use __Manage NuGet Packages__ on the project and search for `Bunifu.UI.WinForms`, or open the Package Manager Console:
  - Open __Tools > NuGet Package Manager > Package Manager Console__ and run:
    `Install-Package Bunifu.UI.WinForms -Version 9.1.0`
- After installation, rebuild the solution and restart Visual Studio if design-time toolbox items do not appear.


## Build & Run
1. Open the solution in Visual Studio.
2. Ensure the solution's projects target `.NET Framework 4.8` (open __Project Properties__ → __Application__ → Target framework).
3. Confirm project references:
   - `MyPhotoControls` should reference the `MyPhotoAlbum` project (right-click `References` → __Add Reference__ → Projects).
4. Build: use __Build > Rebuild Solution__.
5. Run the startup project (F5).

## Project structure (important files)
- `MyPhotoAlbum/PhotoAlbum.cs` — album collection and descriptors.
- `MyPhotoAlbum/Photograph.cs` — photo metadata and image handling.
- `MyPhotoAlbum/AlbumStorage.cs` — read/write album file format.
- `MyPhotoAlbum/AlbumManager.cs` — manager for the current album.
- `MyPhotoControls/PhotoEditDiialog.cs` — photo properties dialog (UI).
- `MyPhotoControls/PixelDialog.cs` — pixel inspector dialog used by viewer (WIP).

## Usage
- Open or create an album from the UI.
- Select a photo to view/edit metadata in `Photo Properties`.
- Save album to persist changes (text format).

## Tests
- If there are unit tests, run them using Visual Studio's __Test Explorer__.

## Contributing
- Fork → create a feature branch → submit a pull request.
- Keep changes small; include build and test steps.

## License
This project does not include a license file. Add a license (for example, `MIT`) if you intend to share the code publicly.
