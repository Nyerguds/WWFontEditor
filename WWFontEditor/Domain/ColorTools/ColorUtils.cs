using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ColorManipulation
{
    public static class ColorUtils
    {

        public static Color ColorFromUInt(UInt32 argb)
        {
            return Color.FromArgb((Byte)((argb & 0xff000000) >> 0x18), (Byte)((argb & 0xff0000) >> 0x10), (Byte)((argb & 0xff00) >> 0x08), (Byte)(argb & 0xff));
        }
                
        public static Color[] GetEightBitColorPalette(SixBitColor[] sixbitpalette)
        {
            Color[] eightbitpalette = new Color[sixbitpalette.Length];
            for (Int32 i = 0; i < sixbitpalette.Length; i++)
                eightbitpalette[i] = sixbitpalette[i].getAsColor();
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
            FileStream fs = new FileStream(palfilename, FileMode.Create, FileAccess.Write);
            BinaryWriter Writer = new BinaryWriter(fs);
            Writer.Write(pal);
            Writer.Close();
        }

        public static SixBitColor[] ReadSixBitPaletteFile(String palfilename)
        {
            const String invalid = "This is not a valid six-bit palette file.";
            if (new FileInfo(palfilename).Length != 768)
                throw new ArgumentException(invalid);

            Byte[] readBytes;
            FileStream fs = new FileStream(palfilename, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(fs))
            {
                readBytes = reader.ReadBytes(768);
            }            
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
        
    }
}
