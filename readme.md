iiLunarTides
=========

iiLunarTides is a C# library supporting the modification of files relating to Solar Winds, the 1993 space RPG game developed by James Schmalz.

| Name   | Read | Write | Comment
|--------|:----:|-------|--------
| DAT    | 〰️   |   ✗   | Multiple different formats all using the same extension

## Usage

Instantiate the relevant class and call the `Process` method passing the filename.

```csharp
var datProcessor = new DatProcessor();

foreach (var file in Directory.GetFiles(@"D:\Games\Solar-Winds\ep1-the-escape", "*.dat"))
{
    Console.WriteLine($"Processing {file}");
    var images = datProcessor.Process(file, @"D:\Games\Solar-Winds\ep1-the-escape\bh_mp1.dat");

    foreach (var image in images)
    {
        var f = Path.GetFileNameWithoutExtension(file);
        var fileName = @$"D:\data\solarwinds\\{f}_{images.IndexOf(image)}.png";
        image.SaveAsPng(fileName);
        Console.WriteLine($"Saved {fileName}");
    }
}
```


## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/iiLunarTides

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```

## Licencing

iiLunarTides is licenced under the MIT License. Full licence details are available in licence.md