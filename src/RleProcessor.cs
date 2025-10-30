using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ii.LunarTides
{
    public class RleProcessor
    {
        public List<Image<Rgba32>> Read(string filePath, Rgba32[] palette)
        {
            if (!File.Exists(filePath))
                return [];

            var data = File.ReadAllBytes(filePath);
            var images = new List<Image<Rgba32>>();
            int offset = 0;

            while (offset < data.Length - 4)
            {
                // Try to read potential dimensions
                if (offset + 2 >= data.Length)
                    break;

                int width = data[offset];
                int height = data[offset + 1];

                // Sanity check for sprite dimensions (1-256 pixels)
                if (width > 0 && width <= 256 && height > 0 && height <= 256)
                {
                    try
                    {
                        byte[]? decompressed = DecompressRLE(data, offset + 2, width * height, out int bytesRead);

                        if (decompressed != null && decompressed.Length == width * height && bytesRead > 0)
                        {
                            // Skip if the image is completely transparent/empty (all zeros)
                            bool hasContent = decompressed.Any(b => b != 0);

                            if (hasContent)
                            {
                                var image = new Image<Rgba32>(width, height);
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        int dataIndex = y * width + x;
                                        if (dataIndex < decompressed.Length)
                                        {
                                            byte paletteIndex = decompressed[dataIndex];
                                            image[x, y] = palette[paletteIndex];
                                        }
                                    }
                                }

                                images.Add(image);

                                offset += 2 + bytesRead;
                                continue;
                            }
                        }
                    }
                    catch { }
                }

                offset++;
            }

            return images;
        }

        private byte[]? DecompressRLE(byte[] data, int startOffset, int expectedSize, out int bytesRead)
        {
            List<byte> output = new List<byte>(expectedSize);
            int offset = startOffset;
            bytesRead = 0;

            try
            {
                while (output.Count < expectedSize && offset < data.Length)
                {
                    byte value = data[offset++];

                    if (value == 0xFF && offset < data.Length)
                    {
                        // RLE marker: next two bytes are count and color
                        if (offset + 1 < data.Length)
                        {
                            byte count = data[offset++];
                            byte color = data[offset++];

                            for (int i = 0; i < count && output.Count < expectedSize; i++)
                            {
                                output.Add(color);
                            }
                        }
                    }
                    else
                    {
                        output.Add(value);
                    }
                }

                bytesRead = offset - startOffset;

                if (output.Count == expectedSize)
                    return output.ToArray();
            }
            catch { }

            return null;
        }

        public Rgba32[]? LoadVGAPalette(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            byte[] data = File.ReadAllBytes(filePath);

            // VGA palette is 768 bytes (256 colors * 3 RGB bytes)
            if (data.Length != 768)
                return null;

            Rgba32[] palette = new Rgba32[256];

            // Detect if palette is 0-63 (VGA) or 0-255 format
            int maxValue = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > maxValue)
                    maxValue = data[i];
            }

            bool isVGAFormat = maxValue <= 63;

            for (int i = 0; i < 256; i++)
            {
                int offset = i * 3;
                int r, g, b;

                if (isVGAFormat)
                {
                    // VGA palette values are 0-63, scale to 0-255
                    r = (data[offset] * 255) / 63;
                    g = (data[offset + 1] * 255) / 63;
                    b = (data[offset + 2] * 255) / 63;
                }
                else
                {
                    // Already in 0-255 format
                    r = data[offset];
                    g = data[offset + 1];
                    b = data[offset + 2];
                }

                // Clamp values to valid range just in case
                r = Math.Clamp(r, 0, 255);
                g = Math.Clamp(g, 0, 255);
                b = Math.Clamp(b, 0, 255);

                palette[i] = new Rgba32((byte)r, (byte)g, (byte)b, 255);
            }

            return palette;
        }
    }
}
