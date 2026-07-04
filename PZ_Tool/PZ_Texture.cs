using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

public static class PZ_Texture
{
    private static Dictionary<string, Bitmap> TEXTURES;
    private static Dictionary<string, List<int>> PIXELS;
    private static List<string> KEYS;

    public static void InitVRAM()
    {
        TEXTURES = new Dictionary<string, Bitmap>();
        PIXELS = new Dictionary<string, List<int>>();
        KEYS = new List<string>();
    }

    public static Bitmap skyboxData;

    public static void LoadImage4Bit(BufferedBinaryReader r, int vramX, int vramY, int imgW, int imgH)
    {
        int tX = (vramX / 64) * 64;
        int tY = (vramY / 256) * 256;

        for (int y = 0; y < imgH; y++)
        {
            for (int x = 0; x < imgW / 4; x++)
            {
                var b = r.ReadUInt16();

                var p1 = (b & 0xF);
                var p2 = (b & 0xF0) >> 4;
                var p3 = (b & 0xF00) >> 8;
                var p4 = (b & 0xF000) >> 12;

                int oX = tX + (x / 64) * 64;
                int oY = tY + (y / 256) * 256;

                int k = oX / 64;

                if (oY > 255)
                    k += 16;

                string key = $"{k.ToString("X2")}";

                if (!KEYS.Contains(key))
                {
                    KEYS.Add(key);
                    TEXTURES.Add(key, new Bitmap(256, 256));
                    PIXELS.Add(key, new List<int>());
                }

                PIXELS[key].Add(p1);
                PIXELS[key].Add(p2);
                PIXELS[key].Add(p3);
                PIXELS[key].Add(p4);
            }
        }

        foreach (var key in KEYS)
        {
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    var p = PIXELS[key][(256 * y) + x];

                    TEXTURES[key].SetPixel(x, y, Color.FromArgb(p << 3, p << 3, p << 3));
                }
            }
        }
    }

    public static void LoadImage16Bit(BufferedBinaryReader r, int vramX, int vramY, int imgW, int imgH)
    {
        int tX = (vramX / 64) * 64;
        int tY = (vramY / 256) * 256;

        for (int y = 0; y < imgH; y++)
        {
            for (int x = 0; x < imgW; x++)
            {
                var color16 = r.ReadUInt16();

                var r0 = (color16 & 0x1F);
                var g0 = (color16 & 0x3E0) >> 5;
                var b0 = (color16 & 0x7C00) >> 10;

                var r8 = r0 << 3;
                var g8 = g0 << 3;
                var b8 = b0 << 3;
                var a = 255;

                if (color16 >> 15 == 0)
                {
                    if (r8 == 0 && g8 == 0 && b8 == 0)
                        a = 0;
                    else
                        a = 255;
                }
                else
                {
                    if (r8 == 0 && g8 == 0 && b8 == 0)
                        a = 127;
                    else
                        a = 127;
                }

                var c = Color.FromArgb(a, r8, g8, b8);

                int k = tX / 64;

                if (tY > 255)
                    k += 16;

                string key = $"{k.ToString("X2")}";

                if (!KEYS.Contains(key))
                {
                    KEYS.Add(key);
                    TEXTURES.Add(key, new Bitmap(256, 256));                  
                }

                TEXTURES[key].SetPixel(x + (vramX - tX), y + (vramY - tY), c);                
            }
        }
    }

    public static void ReColorImageByObject(int tPage, int cPage, byte[] uvs)
    {
        int tX = ((tPage >> 0) & 0xF) * 64;
        int tY = ((tPage >> 4) & 0x1) * 256;

        int cX = ((cPage >> 0) & 0x3F) * 16;
        int cY = ((cPage >> 6) & 0x1FF);

        int tKey = tX / 64;

        if (tY > 255)
            tKey += 16;

        int cKey = cX / 256;

        if (cY > 255)
        {
            cKey += 16;
            cY -= 256;
        }

        Color[] palette = new Color[16];

        for (int p = 0; p < 16; p++)
        {
            Color c = TEXTURES[$"{cKey.ToString("X2")}"].GetPixel(cX + p, cY);

            palette[p] = c;
        }

        List<int> w = new List<int>();
        List<int> h = new List<int>();

        for (int i = 0; i < 4; i++)
        {

            if (!w.Contains(uvs[i]))
                w.Add(uvs[i]);

            if (!h.Contains(uvs[i + 4]))
                h.Add(uvs[i + 4]);
        }

        int wSize = w.Max() - w.Min();
        int hSize = h.Max() - h.Min();
        int wPtr = w.Min();
        int hPtr = h.Min();

        for (int i = 0; i < hSize + 1; i++)
        {
            for (int j = 0; j < wSize + 1; j++)
            {
                var pj = j + wPtr;
                var pi = i + hPtr;
                var px = (256 * pi) + pj;

                TEXTURES[$"{tKey.ToString("X2")}"].SetPixel(pj, pi, palette[PIXELS[$"{tKey.ToString("X2")}"][px]]);
            }
        }
    }

    public static void ReColorImageByTerrain(int tPage, int cPage, int x, int y, int w, int h)
    {
        int tX = ((tPage >> 0) & 0xF) * 64;
        int tY = ((tPage >> 4) & 0x1) * 256;

        int cX = ((cPage >> 0) & 0x3F) * 16;
        int cY = ((cPage >> 6) & 0x1FF);

        int tKey = tX / 64;

        if (tY > 255)
            tKey += 16;

        int cKey = cX / 256;

        if (cY > 255)
        {
            cKey += 16;
            cY -= 256;
        }

        Color[] palette = new Color[16];

        for (int p = 0; p < 16; p++)
        {
            Color c = TEXTURES[$"{cKey.ToString("X2")}"].GetPixel(cX + p, cY);

            palette[p] = c;
        }

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                var pj = j + x;
                var pi = i + y;
                var px = (256 * pi) + pj;
        
                TEXTURES[$"{tKey.ToString("X2")}"].SetPixel(pj, pi, palette[PIXELS[$"{tKey.ToString("X2")}"][px]]);             
            }
        }
    }

    public static void SaveVRAM(string path)
    {
        foreach (var key in KEYS)
        {
            TEXTURES[key].Save($"{path + Path.DirectorySeparatorChar}TEX_{key}.png");
        }

        skyboxData.Save($"{path + Path.DirectorySeparatorChar}SKYBOX.png");
    }

    public static Color[] LoadPalette16Bit(BufferedBinaryReader r, int count)
    {
        Color[] result = new Color[count];

        for (int c = 0; c < count; c++)
        {
            var color16 = r.ReadUInt16();

            var r0 = (color16 & 0x1F);
            var g0 = (color16 & 0x3E0) >> 5;
            var b0 = (color16 & 0x7C00) >> 10;

            var r8 = r0 << 3;
            var g8 = g0 << 3;
            var b8 = b0 << 3;
            var a = 255;

            if (color16 >> 15 == 0)
            {
                if (r8 == 0 && g8 == 0 && b8 == 0)
                    a = 0;
                else
                    a = 255;
            }
            else
            {
                if (r8 == 0 && g8 == 0 && b8 == 0)
                    a = 127;
                else
                    a = 127;
            }

            result[c] = Color.FromArgb(a, r8, g8, b8);
        }

        return result;
    }

    public static Bitmap LoadImage(BufferedBinaryReader r, int imgW, int imgH, int mode, Color[] palette = null)
    {
        Bitmap t = new Bitmap(imgW, imgH);

        switch (mode)
        {
            case 0: //4-bit
                for (int y = 0; y < imgH; y++)
                {
                    for (int x = 0; x < imgW / 4; x++)
                    {
                        var b = r.ReadUInt16();

                        var p1 = (b & 0xF);
                        var p2 = (b & 0xF0) >> 4;
                        var p3 = (b & 0xF00) >> 8;
                        var p4 = (b & 0xF000) >> 12;

                        if (palette == null)
                        {
                            t.SetPixel(x * 4 + 0, y, Color.FromArgb(p1 << 3, p1 << 3, p1 << 3));
                            t.SetPixel(x * 4 + 1, y, Color.FromArgb(p2 << 3, p2 << 3, p2 << 3));
                            t.SetPixel(x * 4 + 2, y, Color.FromArgb(p3 << 3, p3 << 3, p3 << 3));
                            t.SetPixel(x * 4 + 3, y, Color.FromArgb(p4 << 3, p4 << 3, p4 << 3));
                        }
                        else
                        {
                            t.SetPixel(x * 4 + 0, y, palette[p1]);
                            t.SetPixel(x * 4 + 1, y, palette[p2]);
                            t.SetPixel(x * 4 + 2, y, palette[p3]);
                            t.SetPixel(x * 4 + 3, y, palette[p4]);
                        }
                    }
                }
                break;
            case 1: //8-bit
                for (int y = 0; y < imgH; y++)
                {
                    for (int x = 0; x < imgW / 2; x++)
                    {
                        var b = r.ReadUInt16();

                        var p1 = (b & 0xFF);
                        var p2 = (b & 0xFF00) >> 8;

                        if (palette == null)
                        {
                            t.SetPixel(x * 2 + 0, y, Color.FromArgb(p1 >> 1, p1 >> 1, p1 >> 1));
                            t.SetPixel(x * 2 + 1, y, Color.FromArgb(p2 >> 1, p2 >> 1, p2 >> 1));
                        }
                        else
                        {
                            t.SetPixel(x * 2 + 0, y, palette[p1]);
                            t.SetPixel(x * 2 + 1, y, palette[p2]);
                        }
                    }
                }
                break;
            case 2: //16-bit
                for (int y = 0; y < imgH; y++)
                {
                    for (int x = 0; x < imgW; x++)
                    {
                        var color16 = r.ReadUInt16();

                        var r0 = (color16 & 0x1F);
                        var g0 = (color16 & 0x3E0) >> 5;
                        var b0 = (color16 & 0x7C00) >> 10;

                        var r8 = (byte)(r0 << 3);
                        var g8 = (byte)(g0 << 3);
                        var b8 = (byte)(b0 << 3);
                        var a = 255;

                        if (color16 >> 15 == 0)
                        {
                            if (r8 == 0 && g8 == 0 && b8 == 0)
                                a = 0;
                            else
                                a = 255;
                        }
                        else
                        {
                            if (r8 == 0 && g8 == 0 && b8 == 0)
                                a = 127;
                            else
                                a = 127;
                        }

                        t.SetPixel(x, y, Color.FromArgb(a, r8, g8, b8));
                    }
                }
                break;
        }

        return t;
    }

    public static Bitmap LoadTIM(byte[] data, int offset)
    {
        Bitmap image = new Bitmap(20, 20);

        using (BufferedBinaryReader r = new BufferedBinaryReader(data))
        {
            r.Seek(offset, SeekOrigin.Begin);

            Color[] palette = new Color[] { };

            long clutRectOfs = 0;
            long clutColorsOfs = 0;
            long imgRectOfs = 0;
            long imgIndicesOfs = 0;
            long imgIndices2Ofs = 0;

            short clutWidth = 0;
            short clutHeight = 0;
            short imageWidth = 0;
            short imageHeight = 0;

            if (r.ReadInt32() == 0x10)
            {
                uint flag = r.ReadUInt32();
                bool hasPalette = (flag & 0x8) != 0;

                if (hasPalette)
                {
                    clutRectOfs = r.Position + 0x4;
                    clutColorsOfs = r.Position + 0xC;

                    r.Seek(r.ReadInt32() - 0x4, SeekOrigin.Current);
                }
                else
                {
                    clutRectOfs = 0;
                    clutColorsOfs = 0;
                }

                imgRectOfs = r.Position + 0x04;
                imgIndicesOfs = r.Position + 0x0C;
                imgIndices2Ofs = (r.Position - 0x0C + r.ReadInt32() + 0x0B) & -4;

                if (clutRectOfs != 0)
                {
                    if ((flag & 0x20) == 0)
                    {
                        r.Seek(clutRectOfs + 0x04, SeekOrigin.Begin);

                        clutWidth = r.ReadInt16();
                        clutHeight = r.ReadInt16();
                    }
                    else
                    {
                        clutWidth = 16;
                        if (16 < r.ReadInt16())
                            clutWidth = 256;
                    }

                    palette = new Color[clutHeight * clutWidth];

                    r.Seek(clutColorsOfs, SeekOrigin.Begin);

                    for (int i = 0; i < palette.Length; i++)
                    {
                        ushort color16 = r.ReadUInt16();

                        var r0 = (color16 & 0x1F);
                        var g0 = (color16 & 0x3E0) >> 5;
                        var b0 = (color16 & 0x7C00) >> 10;

                        byte r8 = (byte)(r0 << 3);
                        byte g8 = (byte)(g0 << 3);
                        byte b8 = (byte)(b0 << 3);
                        byte a = 255;

                        if (i == 0)
                        {
                            if (color16 >> 15 == 0)
                            {
                                if (r8 == 0 && g8 == 0 && b8 == 0)
                                    a = 0;
                                else
                                    a = 255;
                            }
                            else
                            {
                                if (r8 == 0 && g8 == 0 && b8 == 0)
                                    a = 127;
                                else
                                    a = 127;
                            }
                        }

                        palette[i] = Color.FromArgb(a, r8, g8, b8);
                    }

                    r.Seek(imgRectOfs + 0x04, SeekOrigin.Begin);

                    imageWidth = r.ReadInt16();
                    imageHeight = r.ReadInt16();

                    image = new Bitmap(imageWidth * 4, imageHeight);

                    for (int i = 0; i < imageHeight; i++)
                    {
                        for (int j = 0; j < imageWidth; j++)
                        {
                            var b = r.ReadUInt16();
                            var p1 = (b & 0xF);
                            var p2 = (b & 0xF0) >> 4;
                            var p3 = (b & 0xF00) >> 8;
                            var p4 = (b & 0xF000) >> 12;

                            image.SetPixel(j * 4, i, palette[p1]);
                            image.SetPixel((j * 4) + 1, i, palette[p2]);
                            image.SetPixel((j * 4) + 2, i, palette[p3]);
                            image.SetPixel((j * 4) + 3, i, palette[p4]);
                        }
                    }
                }
            }
        }

        return image;
    }

    public static void GetTankTextures(string mDir, BufferedBinaryReader r, Color[] pallette)
    {
        Bitmap texture = new Bitmap(PZ.TANK_TEXTURE_W, PZ.TANK_TEXTURE_H);

        using (var g = Graphics.FromImage(texture))
            g.Clear(Color.Black);

        for (int y = 0; y < PZ.TANK_TEXTURE_H; y++)
        {
            for (int x = 0; x < PZ.TANK_TEXTURE_W; x++)
            {
                var p = r.ReadByte();
                
                texture.SetPixel(x, y, pallette[p]);
            }
        }

        texture.Save($"{mDir}TEX_00.png");
    }

    public static Color[] GetTankPalette(BufferedBinaryReader r)
    {
        Color[] result = new Color[PZ.TANK_PAL_SZ];
        
        for (int c = 0; c < PZ.TANK_PAL_SZ; c++)
        {
            var color16 = r.ReadUInt16();

            var r0 = (color16 & 0x1F);
            var g0 = (color16 & 0x3E0) >> 5;
            var b0 = (color16 & 0x7C00) >> 10;
            
            var r8 = r0 << 3;
            var g8 = g0 << 3;
            var b8 = b0 << 3;
            var a = 255;

            if (color16 >> 15 == 0)
            {
                if (r8 == 0 && g8 == 0 && b8 == 0)
                    a = 0;
                else
                    a = 255;
            }
            else
            {
                if (r8 == 0 && g8 == 0 && b8 == 0)
                    a = 127;
                else
                    a = 127;
            }

            result[c] = Color.FromArgb(a, r8, g8, b8);
        }

        return result;
    }
}

