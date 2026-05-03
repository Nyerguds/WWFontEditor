using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Nyerguds.ImageManipulation
{
    public static class ColorUtils
    {

        public static Color ColorFromUInt(UInt32 argb)
        {
            return Color.FromArgb((Byte)((argb >> 0x18) & 0xFF), (Byte)((argb >> 0x10) & 0xFF), (Byte)((argb >> 0x08) & 0xFF), (Byte)(argb & 0xFF));
        }
                
        public static Color[] GetEightBitColorPalette(SixBitColor[] sixbitpalette)
        {
            Color[] eightbitpalette = new Color[sixbitpalette.Length];
            for (Int32 i = 0; i < sixbitpalette.Length; i++)
                eightbitpalette[i] = sixbitpalette[i].GetAsColor();
            return eightbitpalette;
        }
        
        public static SixBitColor[] GetSixBitColorPalette(Color[] eightbitpalette)
        {
            SixBitColor[] sixbitpalette = new SixBitColor[eightbitpalette.Length];
            for (Int32 i = 0; i < eightbitpalette.Length; i++)
                sixbitpalette[i] = new SixBitColor(eightbitpalette[i]);
            return sixbitpalette;
        }
        
        public static void WriteSixBitPaletteFile(Color[] palette, String palfilename)
        {
            SixBitColor[] newpal = GetSixBitColorPalette(palette);
            WriteSixBitPaletteFile(newpal, palfilename);
        }

        public static void WriteSixBitPaletteFile(SixBitColor[] palette, String palfilename)
        {
            Byte[] pal = new Byte[palette.Length * 3];
            for (Int32 i = 0; i < palette.Length; i++)
            {
                Int32 index = i * 3;
                pal[index] = palette[i].R;
                pal[index + 1] = palette[i].G;
                pal[index + 2] = palette[i].B;
            }
            File.WriteAllBytes(palfilename, pal);
        }

        public static SixBitColor[] ReadSixBitPaletteFile(String palfilename)
        {
            const String invalid = "This is not a valid six-bit palette file.";
            if (new FileInfo(palfilename).Length != 768)
                throw new ArgumentException(invalid);

            Byte[] readBytes = File.ReadAllBytes(palfilename);
            SixBitColor[] pal = new SixBitColor[256];
            try
            {
                for (Int32 i = 0; i < pal.Length; i++)
                {
                    Int32 index = i * 3;
                    pal[i] = new SixBitColor(readBytes[index], readBytes[index + 1], readBytes[index + 2]);
                }
                return pal;
            }
            catch (ArgumentException e)
            {
                // ArgumentException means some of the values exceeded 63
                throw new NotSupportedException(invalid, e);
            }
        }

        public static Int32 GetClosestPaletteIndexMatch(Color col, Color[] colorPalette, List<Int32> excludedindexes)
        {
            Int32 colorMatch = 0;
            Int32 leastDistance = int.MaxValue;
            Int32 red = col.R;
            Int32 green = col.G;
            Int32 blue = col.B;
            for (Int32 i = 0; i < colorPalette.Length; i++)
            {
                if (excludedindexes == null || !excludedindexes.Contains(i))
                {
                    Color paletteColor = colorPalette[i];
                    Int32 redDistance = paletteColor.R - red;
                    Int32 greenDistance = paletteColor.G - green;
                    Int32 blueDistance = paletteColor.B - blue;
                    Int32 distance = (redDistance * redDistance) + (greenDistance * greenDistance) + (blueDistance * blueDistance);
                    if (distance < leastDistance)
                    {
                        colorMatch = i;
                        leastDistance = distance;
                        if (distance == 0)
                            return i;
                    }
                }
            }
            return colorMatch;
        }
        
    }
}
