using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.FileData.Mythos;
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

        private Boolean _skip127 = false;

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
            for (Int32 i = 0; i < 0x20; ++i)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[] { 0xFF }, 0, 0, 0, this.BitsPerPixel, this.TransparencyColor));
            // Add space
            this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 4, 0, 0, this.BitsPerPixel, this.TransparencyColor));
            // Read data
            while (offset + 4 < fileData.Length)
            {
                // Dummy symbol after 126
                if (this._skip127 && this.m_ImageDataList.Count == 127)
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
                    UInt32 endOffset = (UInt32)(offset + skipLen);
                    try
                    {
                        MythosCompression mc = new MythosCompression();
                        imageData = mc.FlagRleDecode(fileData, (UInt32)offset, endOffset, dataLen, true);
                        if (imageData == null)
                            imageData = mc.CollapsedTransparencyDecode(fileData, (UInt32)offset, endOffset, dataLen, symbWidth, this.TransparencyColor, true);
                        
                    }
                    catch (Exception e)
                    {
                        throw new FileTypeLoadException("Cannot decompress VGS file!", e);
                    }
                    if (imageData == null)
                        throw new FileTypeLoadException("Cannot decompress VGS file!");
                }
                else
                {
                    Array.Copy(fileData, offset, imageData, 0, dataLen);
                }
                /*/
                if (symbWidth == 1 && symbHeight == 1 && yOffset == 0 && imageData.Length == 1 && imageData[0] == this.TransparencyColor)
                {
                    imageData = new Byte[] {this.TransparencyColor, 0, 0, this.TransparencyColor };
                    symbWidth = 2;
                    symbHeight = 2;
                }
                //*/
                FontFileSymbol fc = new FontFileSymbol(imageData, symbWidth, symbHeight, yOffset, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
                offset += skipLen;
            }
            if (offset != fileData.Length)
                throw new FileTypeLoadException("Font load failed.");
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 len = this.m_ImageDataList.Count;
            Int32 actualLen = len - this.SymbolsTypeFirst;
            Int32 saveLen = this._skip127 ? actualLen - 1 : actualLen;
            Byte[][] symbolData = new Byte[actualLen][];
            Int32[] widths = new Int32[actualLen];
            Int32[] heighths = new Int32[actualLen];
            Byte[] yOffsets = new Byte[actualLen];
            for (Int32 i = this.SymbolsTypeFirst; i < len; ++i)
            {
                Int32 writeIndex = i - this.SymbolsTypeFirst;
                FontFileSymbol ffs = this.m_ImageDataList[i];

                if (this._skip127 && i == 127)
                {
                    symbolData[writeIndex] = new Byte[0];
                    widths[writeIndex] = 0;
                    heighths[writeIndex] = 0;
                    yOffsets[writeIndex] = 0;
                    continue;
                }
                if (ffs.Width > 0 && ffs.Height > 0)
                {
                    symbolData[writeIndex] = ffs.ByteData;
                    widths[writeIndex] = ffs.Width;
                    heighths[writeIndex] = ffs.Height;
                }
                else
                {
                    symbolData[writeIndex] = new Byte[] { this.TransparencyColor };
                    widths[writeIndex] = 1;
                    heighths[writeIndex] = 1;
                }
                yOffsets[writeIndex] = (Byte)ffs.YOffset;
            }
            Byte[] finalData = new Byte[(saveLen) * 8 + symbolData.Sum(sd => sd.Length)];
            Int32 offset = 0;
            Int32 skipIndex = 127 - this.SymbolsTypeFirst;
            for (Int32 i = 0; i < actualLen; ++i)
            {
                // Skip 127. It does not get written to the file.
                if (this._skip127 && i == skipIndex)
                    continue;
                ArrayUtils.WriteIntToByteArray(finalData, offset + 0, 2, true, (UInt32)(widths[i] - 1));
                ArrayUtils.WriteIntToByteArray(finalData, offset + 2, 2, true, (UInt32)(heighths[i]-1));
                finalData[offset + 7] = yOffsets[i];
                offset += 8;
                Byte[] curSymbolData = symbolData[i];
                Array.Copy(curSymbolData, 0, finalData, offset, curSymbolData.Length);
                offset += curSymbolData.Length;
            }
            return finalData;
        }
    }
}