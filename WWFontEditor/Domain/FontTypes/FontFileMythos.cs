using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Font from Sherlock Holmes: The Case of Serrated Scalpel.
    /// </summary>
    public class FontFileMythos : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x21; } }
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        /// <summary>The first symbol that is saved. This hides all symbols before this index from the editor.</summary>
        public override Int32 SymbolsTypeFirst { get { return 0x21; } }
        public override Int32 FontWidthTypeMin { get { return 0x1; } }
        public override Int32 FontWidthTypeMax { get { return Int32.MaxValue; } }
        public override Int32 FontHeightTypeMin { get { return 0x1; } }
        public override Int32 FontHeightTypeMax { get { return Int32.MaxValue; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Byte TransparencyColor { get { return 0xFF; } }
        /// <summary>Padding at the bottom of the font. Only used for the preview function.</summary>
        public override Int32 FontTypePaddingBottom { get { return 0; } }
        /// <summary>Padding between the characters of the font. Only used for the preview function.</summary>
        public override Int32 FontTypePaddingRight { get { return 1; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "vgs" }; } }
        public override Boolean CustomSymbolWidthsForType { get { return true; } }
        public override Boolean CustomSymbolHeightsForType { get { return true; } }
        public override String ShortTypeName { get { return "MythFont"; } }
        public override String ShortTypeDescription { get { return "Mythos Software font"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with Y-offset support, where FF is used for transparency, and which starts from the first character after the space. The font data does not contain spacing size between the symbols, does not contain the space, and skips symbol #127."; } }
        public override String[] GamesListForType { get { return new String[]
        {
            "The Lost Files of Sherlock Holmes: The Case of Serrated Scalpel",
            "Bodyworks Voyager: Missions in Anatomy",
        }; } }

        public override void LoadFont(Byte[] fileData)
        {
            // 01 00 06 00 00 00 00 01
            // W-1   H-1      CM X? Y
            if (fileData.Length < 0x8)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int32 offset = 0;
            this.m_FontHeight = 1;
            this.m_FontWidth = 1;

            // fill in dummy symbols.
            for (Int32 i = 0; i < 0x20; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[] { 0xFF }, 0, 0, 0, this.BitsPerPixel, this.TransparencyColor));
            // Add space
            this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 4, 0, 0, this.BitsPerPixel, this.TransparencyColor));
            // Read data
            while (offset + 4 < fileData.Length)
            {
                // Dummy symbol after 126
                if (this.m_ImageDataList.Count == 127)
                    this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 0, 0, 0, this.BitsPerPixel, this.TransparencyColor));

                Int32 symbWidth = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset + 0, 2, true) + 1;
                Int32 symbHeight = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset + 2, 2, true) + 1;
                if (symbWidth < 0 || symbHeight < 0)
                    throw new FileTypeLoadException("Bad header data.");
                this.m_FontHeight = Math.Max(symbHeight, this.m_FontHeight);
                this.m_FontWidth = Math.Max(symbWidth, this.m_FontWidth);
                Int32 skipLen;
                Byte comprByte = fileData[offset + 5];
                Boolean compressed = comprByte != 0;
                //    throw new FileTypeLoadException("Bad header data.");
                //Int32 xOffset = fileData[offset + 6];
                Int32 yOffset = fileData[offset + 7];
                offset += 8;
                Int32 dataLen = symbWidth * symbHeight;
                Byte[] imageData = new Byte[dataLen];
                if (compressed)
                {
                    if (comprByte != 1)
                        throw new FileTypeLoadException("Unknown compression type: " + comprByte);
                    skipLen = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, offset, 2, true) - 8;
                    //if (skipLen < 0)
                    //    throw new FileTypeLoadException("Bad compressed size in header.");
                }
                else
                {
                    skipLen = dataLen;
                }
                if (fileData.Length < offset + skipLen)
                    throw new FileTypeLoadException("Header references offset outside file data.");

                if (compressed)
                {
                    // Is compressed. Doesn't actually work...
                    //Array.Copy(fileData, offset, imageData, 0, skipLen);
                    // TODO: LZW MAGIC! ...or not. Bah.

                    // Draw a nice little "Nope" box instead...
                    for (Int32 i = 0; i < imageData.Length; i++)
                        imageData[i] = this.TransparencyColor;
                    Byte drawColor = (Byte)(this.TransparencyColor + 1);
                    Int32 crossDim = Math.Min(symbHeight, symbWidth);
                    Int32 skipW = (symbWidth - crossDim) / 2;
                    Int32 skipH = (symbHeight - crossDim) / 2;
                    for (Int32 y = 0; y < symbHeight; y++)
                    {
                        for (Int32 x = 0; x < symbWidth; x++)
                            if (
                                (x - skipW == y - skipH) || // diagonal '\'
                                (crossDim - x + skipW-1 == y - skipH) || // diagonal '/'
                                (x == 0) || // line left
                                (y == 0) || // line top
                                (x == symbWidth - 1) || // line right
                                (y == symbHeight - 1) // line bottom
                                )
                                imageData[y * symbWidth + x] = drawColor;
                    }
                }
                else
                {
                    Array.Copy(fileData, offset, imageData, 0, dataLen);
                }
                FontFileSymbol fc = new FontFileSymbol(imageData, symbWidth, symbHeight, yOffset, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
                offset += skipLen;
            }
            if (offset != fileData.Length)
                throw new FileTypeLoadException("Font load failed.");
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 actualLen = this.m_ImageDataList.Count - this.SymbolsTypeFirst;
            Byte[][] symbolData = new Byte[actualLen][];
            Int32[] widths = new Int32[actualLen];
            Int32[] heighths = new Int32[actualLen];
            Byte[] yOffsets = new Byte[actualLen];
            for (Int32 i = this.SymbolsTypeFirst; i < this.m_ImageDataList.Count; i++)
            {
                Int32 writeIndex = i - this.SymbolsTypeFirst;
                FontFileSymbol ffs = this.m_ImageDataList[i];
                if (ffs.Width > 0 && ffs.Height > 0)
                {
                    symbolData[writeIndex] = ffs.ByteData;
                    widths[writeIndex] = ffs.Width;
                    heighths[writeIndex] = ffs.Height;
                }
                else
                {
                    symbolData[writeIndex] = new Byte[1];
                    widths[writeIndex] = 1;
                    heighths[writeIndex] = 1;
                }
                yOffsets[writeIndex] = (Byte)ffs.YOffset;
            }
            Byte[] finalData = new Byte[actualLen * 8 + symbolData.Sum(sd => sd.Length)];
            Int32 offset = 0;
            for (Int32 i = 0; i < actualLen; i++)
            {
                // Skip 127. It does not get written to the file.
                if (i == 127 - this.SymbolsTypeFirst)
                    continue;
                ArrayUtils.WriteIntToByteArray(finalData, offset + 0, 2, true, (UInt32)(widths[i] - 1));
                ArrayUtils.WriteIntToByteArray(finalData, offset + 2, 2, true, (UInt32)(heighths[i]-1));
                finalData[offset + 7] = yOffsets[i];
                offset += 8;

                Byte[] curSymbolData = symbolData[i];
                Byte[] saveSymbolData = new Byte[curSymbolData.Length];
                Array.Copy(saveSymbolData, 0, finalData, offset, saveSymbolData.Length);
                offset += saveSymbolData.Length;
            }
            return finalData;
        }
    }
}