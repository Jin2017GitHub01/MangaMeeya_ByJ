# MangaMeeya by Jin - Manga ZIP Viewer

A manga ZIP file viewer application built with WPF (Windows Presentation Foundation).

## Features

- **ZIP File Open**: Open a manga ZIP file and automatically extract images
- **Image Display**: Display manga pages with high quality and ratio preservation
- **Page Navigation**: Browse pages using Prev/Next buttons
- **Mouse Gestures & Wheel**:
  - **Wheel Scroll Up / Drag Right**: Previous page
  - **Wheel Scroll Down / Drag Left**: Next page
  - **Drag Up**: Toggle fullscreen mode
  - **Drag Down**: Exit fullscreen mode
- **Multi-language Support**: Select dynamically between English, Korean, Chinese, and Japanese. Language preference is saved.
- **Page Info**: Real-time display of current page and total pages
- **Supported Formats**: JPG, JPEG, PNG, BMP, GIF, WebP
- **Modern UI**: Clean and responsive WPF-based user interface

## Setup & Running

### Requirements
- .NET 6.0 or higher
- Windows OS

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

Or run the executable directly:
```bash
bin/Debug/net6.0-windows/MangaMeeya_by_Jin.exe
```

## How to Use

1. Click the **📁 Open ZIP File** button.
2. Select a manga ZIP file.
3. Images are automatically extracted, and the first page is shown.
4. Use the **◀ Prev** / **Next ▶** buttons or mouse gestures (drag/wheel) to navigate.
5. Use the **🖥 Fullscreen** button or drag up to toggle fullscreen mode.

## Project Structure

```
MangaMeeya_by_Jin/
├── Program.cs              # Application entry point
├── App.xaml                # Application resources
├── App.xaml.cs             # Application code-behind
├── AppSettings.cs          # Application settings (persists ZIP paths and language)
├── LanguageManager.cs      # Localization manager containing translations
├── MainWindow.xaml         # Main Window UI layout
├── MainWindow.xaml.cs      # Main Window logic
├── SplashWindow.xaml       # Splash Screen UI layout
├── SplashWindow.xaml.cs    # Splash Screen logic
├── MangaMeeya_by_Jin.csproj # Project file
└── README.md               # Korean README file
```

## Technical Stack

- **Framework**: .NET 6.0 WPF (Windows Presentation Foundation)
- **Library**: SharpZipLib (ZIP file processing)
- **UI**: XAML-based declarative UI
- **Language**: C#

## Key Implementation Details

### ZIP File Processing
- Extracts ZIP files using SharpZipLib
- Decompresses contents to a temporary directory automatically
- Automatically cleans up the temporary directory on exit

### Image Rendering
- Efficient image loading using BitmapImage
- Uniform stretching to maintain aspect ratios
- Optimal reading experience with black background

### Navigation & Gestures
- Intuitive button-based page navigation
- Swipe-like mouse gestures for seamless navigation (drag/wheel)
- Handling of start and end boundary pages

## Contributing

Bug reports, feature requests, and pull requests are always welcome!

## License

MIT License - See the [LICENSE](LICENSE) file for details.
