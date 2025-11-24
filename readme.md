iiLunarTides
=========

iiLunarTides is a C# library supporting the modification of files relating to Solar Winds, the 1993 space RPG game developed by James Schmalz.

| Name     | Read | Write | Comment
|----------|:----:|-------|--------
| bh_char  | 〰️  |       | RLE
| bh_conv  |      |       | 
| bh_epal  |      |       | 
| bh_epic  |      |       | 
| bh_expl  | 〰️  |       | RLE
| bh_hype  | 〰️  |       | RLE
| bh_imp   |      |       | 
| bh_iord  |      |       | 
| bh_mis   | 〰️  |       | RLE
| bh_mp1   |      |       | 
| bh_mp2   |      |       | 
| bh_mp3   |      |       | 
| bh_ms    |      |       | 
| bh_mu1   |      |       | Music - Creative Media File
| bh_mu2   |      |       | Music - Creative Media File
| bh_mu3   |      |       | Music - Creative Media File
| bh_mu4   |      |       | Music - Creative Media File
| bh_mu5   |      |       | Music - Creative Media File
| bh_mu6   |      |       | Music - Creative Media File
| bh_mu7   |      |       | Music - Creative Media File
| bh_mu8   |      |       | Music - Creative Media File
| bh_mu9   |      |       | Music - Creative Media File
| bh_mu10  |      |       | Music - Creative Media File
| bh_mu11  |      |       | Music - Creative Media File
| bh_mu12  |      |       | Music - Creative Media File
| bh_mu13  |      |       | Music - Creative Media File
| bh_mw1a  | 〰️  |       | RLE
| bh_mw1b  |      |       | 
| bh_mw2a  |      |       | 
| bh_mw2b  |      |       | 
| bh_mw2p  |      |       | 
| bh_mw3a  |      |       | 
| bh_mw3b  |      |       | 
| bh_obj   |      |       | 
| bh_ord   |      |       | 
| bh_pal   |      |       | 
| bh_pan   |      |       | 
| bh_peop  | 〰️  |       | RLE
| bh_pic   |      |       | 
| bh_plan  |      |       | RLE
| bh_scrn  | ✔   |       | B800 Text
| bh_seq   |      |       | 
| bh_seq2  |      |       | RLE
| bh_shd   |      |       | 
| bh_ship  |      |       | 
| bh_sml   | 〰️  |       | RLE
| bh_start |      |       | 
| bh_title |      |       | 
| bh_vfd   |      |       | 
| bh_wr    |      |       | 
| config   |      |       | 
| page     |      |       | 


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