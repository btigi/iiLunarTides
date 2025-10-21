using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ii.LunarTides
{
    public class B800Processor
    {
        public Image<Rgba32> Read(string filename)
        {
            var data = File.ReadAllBytes(filename);
            // B800 text mode: 80x25 characters, 2 bytes per character
            // Byte 0: ASCII character
            // Byte 1: Attribute (bits 0-3: foreground, bits 4-6: background, bit 7: blink)

            const int cols = 80;
            const int rows = 25;
            const int charWidth = 8;
            const int charHeight = 16;

            var imageWidth = cols * charWidth;
            var imageHeight = rows * charHeight;

            var image = new Image<Rgba32>(imageWidth, imageHeight);

            // CGA/EGA 16-color palette
            Color[] cgaPalette = new Color[16]
            {
                Color.FromRgb(0, 0, 0),       // 0: Black
                Color.FromRgb(0, 0, 170),     // 1: Blue
                Color.FromRgb(0, 170, 0),     // 2: Green
                Color.FromRgb(0, 170, 170),   // 3: Cyan
                Color.FromRgb(170, 0, 0),     // 4: Red
                Color.FromRgb(170, 0, 170),   // 5: Magenta
                Color.FromRgb(170, 85, 0),    // 6: Brown
                Color.FromRgb(170, 170, 170), // 7: Light Gray
                Color.FromRgb(85, 85, 85),    // 8: Dark Gray
                Color.FromRgb(85, 85, 255),   // 9: Light Blue
                Color.FromRgb(85, 255, 85),   // 10: Light Green
                Color.FromRgb(85, 255, 255),  // 11: Light Cyan
                Color.FromRgb(255, 85, 85),   // 12: Light Red
                Color.FromRgb(255, 85, 255),  // 13: Light Magenta
                Color.FromRgb(255, 255, 85),  // 14: Yellow
                Color.FromRgb(255, 255, 255)  // 15: White
            };

            // Try and find a decent monospace font
            var collection = new FontCollection();
            FontFamily family;
            try
            {
                family = SystemFonts.CreateFont("Consolas", 12, FontStyle.Regular).Family;
            }
            catch
            {
                try
                {
                    family = SystemFonts.CreateFont("Courier New", 12, FontStyle.Regular).Family;
                }
                catch
                {
                    // Give up and use whatever we have
                    family = SystemFonts.Families.First();
                }
            }

            var font = family.CreateFont(12, FontStyle.Regular);

            image.Mutate(ctx =>
            {
                ctx.Fill(cgaPalette[0]);

                for (var row = 0; row < rows; row++)
                {
                    for (var col = 0; col < cols; col++)
                    {
                        var offset = (row * cols + col) * 2;
                        if (offset + 1 >= data.Length)
                            break;

                        var charCode = data[offset];
                        var attribute = data[offset + 1];

                        // Parse attribute byte
                        var fgColor = attribute & 0x0F;
                        var bgColor = (attribute >> 4) & 0x07;

                        // Draw background rectangle
                        var bgRect = new RectangleF(col * charWidth, row * charHeight, charWidth, charHeight);
                        ctx.Fill(cgaPalette[bgColor], bgRect);

                        // Draw character if it's printable
                        if (charCode >= 32 && charCode < 127)
                        {
                            var c = (char)charCode;
                            var position = new PointF(col * charWidth, row * charHeight);
                            
                            ctx.DrawText(c.ToString(), font, cgaPalette[fgColor], position);
                        }
                        else if (charCode > 0)
                        {
                            // Extended ASCII, draw block characters
                            if (charCode == 0xDB || charCode == 0xB0 || charCode == 0xB1 || charCode == 0xB2)
                            {
                                var blockRect = new RectangleF(col * charWidth, row * charHeight, charWidth, charHeight);
                                ctx.Fill(cgaPalette[fgColor], blockRect);
                            }
                        }
                    }
                }
            });

            return image;
        }
    }
}