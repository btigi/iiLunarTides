using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace ii.LunarTides
{
    public class DatProcessor
    {
        public List<Image<Rgba32>> Process(string filePath, string? paletteFilePath = null)
        {
            var images = new List<Image<Rgba32>>();

            var palette = paletteFilePath != null ? LoadPalette(paletteFilePath) : CreateDefaultPalette();

            using var br = new BinaryReader(File.OpenRead(filePath));
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var width = br.ReadByte();
                var height = br.ReadByte();

                if (height == 0 || width == 0)
                    continue;
                    
                var indexData = br.ReadBytes(height * width);
                
                var image = new Image<Rgba32>(width, height);
                
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int dataIndex = y * width + x;
                        if (dataIndex < indexData.Length)
                        {
                            byte paletteIndex = indexData[dataIndex];
                            image[x, y] = palette[paletteIndex];
                        }
                    }
                }
                
                images.Add(image);
            }

            return images;
        }

        private Rgba32[] LoadPalette(string paletteFilePath)
        {
            var palette = new Rgba32[256];
            
            using var br = new BinaryReader(File.OpenRead(paletteFilePath));
            for (int i = 0; i < 256; i++)
            {
                // Read 6-bit RGB values and convert to 8-bit
                byte r = (byte)(br.ReadByte() * 4);
                byte g = (byte)(br.ReadByte() * 4);
                byte b = (byte)(br.ReadByte() * 4);
                
                palette[i] = new Rgba32(r, g, b, 255);
            }
            
            return palette;
        }

        private Rgba32[] CreateDefaultPalette()
        {
            var palette = new Rgba32[256];
            
            for (int i = 0; i < 256; i++)
            {
                if (i < 16)
                {
                    // Standard 16 colors
                    var standardColors = new Rgba32[]
                    {
                        new(0, 0, 0, 255),       // Black
                        new(128, 0, 0, 255),     // Dark Red
                        new(0, 128, 0, 255),     // Dark Green
                        new(128, 128, 0, 255),   // Dark Yellow
                        new(0, 0, 128, 255),     // Dark Blue
                        new(128, 0, 128, 255),   // Dark Magenta
                        new(0, 128, 128, 255),   // Dark Cyan
                        new(192, 192, 192, 255), // Light Gray
                        new(128, 128, 128, 255), // Dark Gray
                        new(255, 0, 0, 255),     // Red
                        new(0, 255, 0, 255),     // Green
                        new(255, 255, 0, 255),   // Yellow
                        new(0, 0, 255, 255),     // Blue
                        new(255, 0, 255, 255),   // Magenta
                        new(0, 255, 255, 255),   // Cyan
                        new(255, 255, 255, 255)  // White
                    };
                    palette[i] = standardColors[i];
                }
                else
                {
                    // Gradient colours
                    byte value = (byte)(i - 16);
                    palette[i] = new Rgba32(value, value, value, 255);
                }
            }
            
            return palette;
        }
    }
}