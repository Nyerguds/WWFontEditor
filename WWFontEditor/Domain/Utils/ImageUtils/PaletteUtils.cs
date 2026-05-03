using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Nyerguds.ImageManipulation
{
    /// <summary>
    /// Class containing palette generating utilities.
    /// </summary>
    public static class PaletteUtils
    {
        public static ColorPalette GetColorPalette(Color[] colors, PixelFormat pf)
        {
            ColorPalette cp;
            using (Bitmap bm = new Bitmap(1, 1, pf))
                cp = bm.Palette;
            for (Int32 i = 0; i < colors.Length && i < cp.Entries.Length; i++)
                cp.Entries[i] = colors[i];
            return cp;
        }

        /// <summary>
        /// The standard EGA/CGA palette.
        /// </summary>
        private static readonly Color[] EgaPalette =
        {
            Color.FromArgb(0x00, 0x00, 0x00), // black
            Color.FromArgb(0x00, 0x00, 0xAA), // blue
            Color.FromArgb(0x00, 0xAA, 0x00), // green
            Color.FromArgb(0x00, 0xAA, 0xAA), // cyan
            Color.FromArgb(0xAA, 0x00, 0x00), // red
            Color.FromArgb(0xAA, 0x00, 0xAA), // magenta
            Color.FromArgb(0xAA, 0x55, 0x00), // yellow / brown
            Color.FromArgb(0xAA, 0xAA, 0xAA), // white / light gray
            Color.FromArgb(0x55, 0x55, 0x55), // dark gray / bright black
            Color.FromArgb(0x55, 0x55, 0xFF), // bright blue
            Color.FromArgb(0x55, 0xFF, 0x55), // bright green
            Color.FromArgb(0x55, 0xFF, 0xFF), // bright cyan
            Color.FromArgb(0xFF, 0x55, 0x55), // bright red
            Color.FromArgb(0xFF, 0x55, 0xFF), // bright magenta
            Color.FromArgb(0xFF, 0xFF, 0x55), // bright yellow
            Color.FromArgb(0xFF, 0xFF, 0xFF), // bright white
        };

        private static readonly Byte[][] CgaPalettes =
        {
            new Byte[] {2, 4, 6}, // Mode 4, palette 0
            new Byte[] {3, 5, 7}, // Mode 4, palette 1
            new Byte[] {3, 4, 7}, // Mode 5 palette
        };

        public static Color[] GetEgaPalette()
        {
            return EgaPalette.ToArray();
        }

        public static Color[] GetEgaPalette(Int32 bitsPerPixel)
        {
            Int32 colours = 1 << bitsPerPixel;
            if (colours > 16)
                throw new NotSupportedException("EGA palette can not contain more than 16 colours!");
            if (colours == 16)
                return EgaPalette.ToArray();
            Color[] pal = new Color[colours];
            Array.Copy(EgaPalette, 0, pal, 0, colours);
            return pal;
        }

        public static Color[] GetCgaPalette(Byte backgroundColor, Boolean colorBurst, Boolean palette, Boolean intensity, Int32 bitsPerPixel)
        {
            if (backgroundColor > 15)
                throw new NotSupportedException("CGA palette values only go up to 15!");
            Color[] pal = new Color[1 << bitsPerPixel];
            pal[0] = EgaPalette[backgroundColor];
            if (bitsPerPixel == 1)
            {
                pal[1] = EgaPalette[0];
                return pal;
            }
            if (bitsPerPixel != 2)
                throw new NotSupportedException("CGA palette can only be 1bpp or 2bpp!");
            Int32 paletteNr = colorBurst ? (palette ? 1 : 0) : 2;
            Byte[] colors = CgaPalettes[paletteNr];
            Int32 intensityAdd = intensity ? 8 : 0;
            for (Int32 i = 0; i < 3; i++)
            {
                Int32 cgacol = colors[i] | intensityAdd;
                pal[i + 1] = EgaPalette[cgacol];
            }
            return pal;
        }

        public static Boolean DetectCgaPalette(Color[] palEntries, out Byte backgroundColor, out Boolean colorBurst, out Boolean palette, out Boolean intensity)
        {
            Int32 colors = palEntries.Length;
            colorBurst = false;
            palette = false;
            intensity = false;
            backgroundColor = GetEgaIndex(palEntries[0]);
            if (backgroundColor == 0xFF)
            {
                backgroundColor = 0;
                return false;
            }
            if (colors == 2)
                return palEntries[1].R == 0 && palEntries[1].G == 0 && palEntries[1].B == 0;
            if (colors != 4)
                return false;
            for (Int32 opts = 0; opts < 6; opts++)
            {
                Boolean palMatch = true;
                // Switched colorburst so it would come last.
                Color[] cgaPal = GetCgaPalette(backgroundColor, (opts & 4) == 0, (opts & 2) == 1, (opts & 1) == 1, 2);
                for (Int32 i = 1; i < 4; i++)
                {
                    if (Color.FromArgb(palEntries[i].R, palEntries[i].G, palEntries[i].B) == cgaPal[i])
                        continue;
                    palMatch = false;
                    break;
                }
                if (!palMatch)
                    continue;
                colorBurst = (opts & 4) == 0;
                palette = (opts & 2) == 1;
                intensity = (opts & 1) == 1;
                return true;
            }
            return false;
        }

        public static Byte GetEgaIndex(Color col)
        {
            Color c = Color.FromArgb(col.R, col.G, col.B);
            for (Byte i = 0; i < 16; i++)
            {
                if (EgaPalette[i].R != c.R || EgaPalette[i].G != c.G || EgaPalette[i].B != c.B)
                    continue;
                return i;
            }
            return 0xFF;
        }

        public static Boolean[] MakeTransparencyGuide(Int32 bpp, Int32[] transparentIndices)
        {
            Int32 palLen = bpp > 8 ? 0 : 1 << bpp;
            Boolean[] tranGuide = new Boolean[palLen];
            foreach (Int32 b in transparentIndices)
                if (b < tranGuide.Length)
                    tranGuide[b] = true;
            return tranGuide;
        }

        public static Boolean[] MakeTransparencyGuide(Int32 bpp, Int32 transparentColor)
        {
            Int32 palLen = bpp > 8 ? 0 : 1 << bpp;
            Boolean[] tranGuide = new Boolean[palLen];
            if (transparentColor < tranGuide.Length)
                tranGuide[transparentColor] = true;
            return tranGuide;
        }

        public static Boolean[] MakeTransparencyGuide(Int32 bpp, Color[] palette)
        {
            Int32 palLen = bpp > 8 ? 0 : 1 << bpp;
            Boolean[] tranGuide = new Boolean[palLen];
            if (palette == null)
                return tranGuide;
            Int32 len = Math.Min(palLen, palette.Length);
            for (Int32 i = 0; i < len; i++)
                tranGuide[i] = palette[i].A < 128;
            return tranGuide;
        }

        private static Boolean[] PrepareTransparencyGuide(Boolean[] transparencyGuide, Int32 targetPalLen)
        {
            Boolean[] newTransparencyGuide = new Boolean[targetPalLen];
            if (transparencyGuide != null)
                Array.Copy(transparencyGuide, 0, newTransparencyGuide, 0, Math.Min(transparencyGuide.Length, targetPalLen));
            return newTransparencyGuide;
        }

        public static Color[] ApplyTransparencyGuide(Color[] palette, Boolean[] transparencyGuide)
        {
            transparencyGuide = PrepareTransparencyGuide(transparencyGuide, palette.Length);
            for (Int32 i = 0; i < palette.Length; i++)
                palette[i] = Color.FromArgb(transparencyGuide[i] ? 0x00 : 0xFF, palette[i]);
            return palette;
        }

        /// <summary>
        /// Creates a new palette with the full amount of colour for the given bits per pixel value, and pours the given colours into it.
        /// </summary>
        /// <param name="sourcePalette">Source colours.</param>
        /// <param name="pixelFormat">Pixel format for which to generate the new palette.</param>
        /// <param name="transparencyGuide">Array of booleans specifying which indices to make transparent.</param>
        /// <returns>The new palette.</returns>
        public static Color[] MakePalette(Color[] sourcePalette, PixelFormat pixelFormat, Boolean[] transparencyGuide)
        {
            return MakePalette(sourcePalette, pixelFormat, transparencyGuide, null);
        }

        /// <summary>
        /// Creates a new palette with the full amount of colour for the given bits per pixel value, and pours the given colours into it.
        /// </summary>
        /// <param name="sourcePalette">Source colours.</param>
        /// <param name="pixelFormat">Pixel format for which to generate the new palette.</param>
        /// <param name="transparencyGuide">Array of booleans specifying which indices to make transparent.</param>
        /// <param name="defaultColor">Default colour if the source palette is smaller than the returned palette. If not filled in, leftover colors will be Color.Empty.</param>
        /// <returns>The new palette.</returns>
        public static Color[] MakePalette(Color[] sourcePalette, PixelFormat pixelFormat, Boolean[] transparencyGuide, Color? defaultColor)
        {
            Int32 bpp = Image.GetPixelFormatSize(pixelFormat);
            return MakePalette(sourcePalette, bpp, transparencyGuide, defaultColor);
        }

        /// <summary>
        /// Creates a new palette with the full amount of colour for the given bits per pixel value, and pours the given colours into it.
        /// </summary>
        /// <param name="sourcePalette">Source colours.</param>
        /// <param name="bpp">Bits per pixel for which to generate the new palette.</param>
        /// <param name="transparencyGuide">Array of booleans specifying which indices to make transparent.</param>
        /// <returns>The new palette.</returns>
        public static Color[] MakePalette(Color[] sourcePalette, Int32 bpp, Boolean[] transparencyGuide)
        {
            return MakePalette(sourcePalette, bpp, transparencyGuide, null);
        }

        /// <summary>
        /// Creates a new palette with the full amount of colour for the given bits per pixel value, and pours the given colours into it.
        /// </summary>
        /// <param name="sourcePalette">Source colours.</param>
        /// <param name="bpp">Bits per pixel for which to generate the new palette.</param>
        /// <param name="transparencyGuide">Array of booleans specifying which indices to make transparent.</param>
        /// <param name="defaultColor">Default colour if the source palette is smaller than the returned palette. If not filled in, leftover colors will be Color.Empty.</param>
        /// <returns>The new palette.</returns>
        public static Color[] MakePalette(Color[] sourcePalette, Int32 bpp, Boolean[] transparencyGuide, Color? defaultColor)
        {
            Int32 palLen = bpp > 8 ? 0 : 1 << bpp;
            Color[] pal = new Color[palLen];
            transparencyGuide = PrepareTransparencyGuide(transparencyGuide, palLen);
            for (Int32 i = 0; i < palLen; i++)
            {
                Color col;
                if (sourcePalette != null && i < sourcePalette.Length)
                    col = sourcePalette[i];
                else if (defaultColor.HasValue)
                    col = defaultColor.Value;
                else
                    col = Color.Empty;
                pal[i] = Color.FromArgb(transparencyGuide[i] ? 0x00 : 0xFF, col);
            }
            return pal;
        }

        public static Color[] GenerateGrayPalette(Int32 bpp, Boolean[] transparencyGuide, Boolean reverseGenerated)
        {
            Int32 palLen = 1 << bpp;
            Color[] pal = new Color[palLen];
            transparencyGuide = PrepareTransparencyGuide(transparencyGuide, palLen);
            // generate greyscale palette.
            Int32 steps = 255 / (palLen - 1);
            for (Int32 i = 0; i < pal.Length; i++)
            {
                Double curval = reverseGenerated ? pal.Length - 1 - i : i;
                Byte grayval = (Byte)Math.Min(255, Math.Round(curval * steps, MidpointRounding.AwayFromZero));
                pal[i] = Color.FromArgb(transparencyGuide == null ? 255 : transparencyGuide[i] ? 0x00 : 0xFF, grayval, grayval, grayval);
            }
            return pal;
        }

        public static Color[] GenerateDefWindowsPalette(Int32 bpp, Boolean[] transparencyGuide, Boolean reverseGenerated)
        {
            Color[] pal;
            using (Bitmap bm = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                pal = bm.Palette.Entries;
            for (Int32 i = 0; i < pal.Length; i++)
                if (pal[i].A < 0xFF)
                    pal[i] = Color.FromArgb(0xFF, pal[i]);
            // Cut down to requested size
            pal = MakePalette(pal, bpp, null, Color.Black);
            // Reverse after cutting since otherwise we won't get the default 16 color palette.
            if (reverseGenerated)
            {
                Color[] entries = pal.Reverse().ToArray();
                for (Int32 i = 0; i < pal.Length; i++)
                    pal[i] = entries[i];
            }
            // Apply transparency and return
            return ApplyTransparencyGuide(pal, transparencyGuide);
        }

        public static Color[] GenerateDoubleRainbow(Int32 blackIndex, Boolean[] transparencyGuide, Boolean reverseGenerated)
        {
            Color[] smallPal = GenerateRainbowPalette(4, blackIndex, null, reverseGenerated);
            Color[] bigPal = GenerateRainbowPalette(8, blackIndex, null, reverseGenerated);
            Array.Copy(smallPal, 0, bigPal, 0, smallPal.Length);
            return ApplyTransparencyGuide(bigPal, transparencyGuide);
        }

        public static Color[] GenerateRainbowPalette(Int32 bpp, Int32 blackIndex, Boolean[] transparencyGuide, Boolean reverseGenerated)
        {
            return GenerateRainbowPalette(bpp, blackIndex, transparencyGuide, reverseGenerated, 0, (Int32)ColorHSL.SCALE, false);
        }

        /// <summary>
        /// Generates a colour palette of the given bits per pixel containing a hue rotation of the given range.
        /// </summary>
        /// <param name="bpp">Bits per pixel of the image the palette is for.</param>
        /// <param name="blackIndex">Index on the palette to replace with black.</param>
        /// <param name="transparencyGuide">Array with booleans indicating which indices should become transparent.</param>
        /// <param name="reverseGenerated">Reverse the generated range. This happens after the generating, and before the operations on the first index/.</param>
        /// <param name="startHue">Start hue range. Value from 0 to 240.</param>
        /// <param name="endHue">End hue range. Value from 0 to 240. Must be higher then startHue.</param>
        /// <param name="inclusiveEnd">True to include the end hue in the palette. If you generate a full hue range, this can be set to False to avoid getting a duplicate red colour on it.</param>
        /// <returns>The generated palette, as array of System.Drawing.Color objects.</returns>
        public static Color[] GenerateRainbowPalette(Int32 bpp, Int32 blackIndex, Boolean[] transparencyGuide, Boolean reverseGenerated, Int32 startHue, Int32 endHue, Boolean inclusiveEnd)
        {
            Int32 colors = 1 << bpp;
            Color[] pal = new Color[colors];
            Double step = (Double)(endHue - startHue) / (inclusiveEnd ? colors - 1 : colors);
            Double start = startHue;
            Double satValue = ColorHSL.SCALE;
            Double lumValue = 0.5 * ColorHSL.SCALE;
            for (Int32 i = 0; i < colors; i++)
            {
                if (i + 1 == colors)
                {
                    i++;
                    i--;
                }
                Double curStep = start + step * i;
                pal[i] = new ColorHSL(curStep, satValue, lumValue);
            }
            if (reverseGenerated)
            {
                Color[] entries = pal.Reverse().ToArray();
                for (Int32 i = 0; i < pal.Length; i++)
                    pal[i] = entries[i];
            }
            if (blackIndex >= 0 && blackIndex < colors)
                pal[blackIndex] = Color.Black;
            // Apply transparency
            return ApplyTransparencyGuide(pal, transparencyGuide);
        }
    }
}
