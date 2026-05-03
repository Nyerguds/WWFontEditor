using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Old 1bpp Westwood Studios font format
    /// </summary>
    public class FontFileMythos : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x1; } }
        public override Int32 SymbolsTypeMax { get { return 0xFF; } }
        /// <summary>The first symbol that is saved. This hides all symbols before this index from the editor.</summary>
        public override Int32 SymbolsTypeFirst { get { return 0x21; } }
        public override Int32 FontWidthTypeMin { get { return 0x1; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMin { get { return 0x1; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        /// <summary>Padding at the bottom of the font. Only used for the preview function.</summary>
        public override Int32 FontTypePaddingBottom { get { return 0; } }
        /// <summary>Padding between the characters of the font. Only used for the preview function.</summary>
        public override Int32 FontTypePaddingRight { get { return 1; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "vgs" }; } }
        public override Boolean CustomSymbolWidthsForType { get { return true; } }
        public override Boolean CustomSymbolHeightsForType { get { return true; } }
        public override String ShortTypeName { get { return "MythFont"; } }
        public override String ShortTypeDescription { get { return "Mythos font"; } }
        public override String LongTypeDescription { get { return "A 1bpp font with Y-offset support which is saved as 8-bit data with the values reversed (00 for painted pixels, FF for clear ones). It does not include a space character, and it is unknown if the games can handle reading more than 94 symbols from it."; } }
        public override String[] GamesListForType { get { return new String[]
        {
            "Sherlock Holmes: The Case of Serrated Scalpel"
        }; } }

        public override void LoadFont(Byte[] fileData)
        {
            // 01 00 06 00 00 00 00 01
            // W-1   H-1             Y
            if (fileData.Length < 0x8)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int32 offset = 0;
            this.m_FontHeight = 1;
            this.m_FontWidth = 1;

            // fill in dummy symbols.
            for (Int32 i = 0; i < 0x20; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[1], 1, 1, 0, this.BitsPerPixel));
            // Add space
            this.m_ImageDataList.Add(new FontFileSymbol(new Byte[4], 4, 1, 0, this.BitsPerPixel));
            // Read data
            while (offset < fileData.Length)
            {
                Int32 symbWidth = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset + 0, 2, true) + 1;
                Int32 symbHeight = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset + 2, 2, true) + 1;
                if (symbWidth < 0 || symbHeight < 0)
                    throw new FileTypeLoadException("Bad header data.");
                this.m_FontHeight = Math.Max(symbHeight, this.m_FontHeight);
                this.m_FontWidth = Math.Max(symbWidth, this.m_FontWidth);
                if (fileData[offset + 4] != 0 || fileData[offset + 6] != 0 || fileData[offset + 6] != 0)
                    throw new FileTypeLoadException("Bad header data.");
                Int32 yOffset = fileData[offset + 7];
                offset += 8;
                Int32 dataLen = symbWidth * symbHeight;
                Byte[] imageData = new Byte[dataLen];
                if (fileData.Length < offset + dataLen)
                    throw new FileTypeLoadException("header references offset outside file data.");
                Array.Copy(fileData, offset, imageData, 0, dataLen);
                for (Int32 i = 0; i < dataLen; i++)
                {
                    if (imageData[i] == 0)
                        imageData[i] = 1;
                    else if (imageData[i] == 0xFF)
                        imageData[i] = 0;
                    else
                        throw new FileTypeLoadException("Bad value.");
                }
                FontFileSymbol fc = new FontFileSymbol(imageData, symbWidth, symbHeight, yOffset, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
                offset += dataLen;
            }
            if (offset != fileData.Length)
                throw new FileTypeLoadException("Font load failed.");
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            Int32 actualLen = m_ImageDataList.Count - SymbolsTypeFirst;
            Byte[][] symbolData = new Byte[actualLen][];
            Int32[] widths = new Int32[actualLen];
            Int32[] heighths = new Int32[actualLen];
            Byte[] yOffsets = new Byte[actualLen];
            for (Int32 i = SymbolsTypeFirst; i < m_ImageDataList.Count; i++)
            {
                Int32 writeIndex = i - SymbolsTypeFirst;
                FontFileSymbol ffs = m_ImageDataList[i];
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
                ArrayUtils.WriteIntToByteArray(finalData, offset + 0, 2, true, (UInt32)(widths[i] - 1));
                ArrayUtils.WriteIntToByteArray(finalData, offset + 2, 2, true, (UInt32)(heighths[i]-1));
                finalData[offset + 7] = yOffsets[i];
                offset += 8;

                Byte[] curSymbolData = symbolData[i];
                Byte[] saveSymbolData = new Byte[curSymbolData.Length];

                for (Int32 o = 0; o < saveSymbolData.Length; o++)
                {
                    if (curSymbolData[o] == 0)
                        saveSymbolData[o] = 0xFF;
                    else
                        saveSymbolData[o] = 0;
                }
                Array.Copy(saveSymbolData, 0, finalData, offset, saveSymbolData.Length);
                offset += saveSymbolData.Length;
            }
            return finalData;
        }
    }
}