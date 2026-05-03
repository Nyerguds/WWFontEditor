using System;
using System.Collections.Generic;
using System.Text;
using Nyerguds.Util;
using Nyerguds.FileData.Epic;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileJazzC : FontFile
    {
        protected const String header = "Digital Dimensions";
        public override Int32 SymbolsTypeMin { get { return 0xA6; } }
        public override Int32 SymbolsTypeMax { get { return 0xA6; } }
        public override Int32 SymbolsTypeFirst { get { return 0x20; } }
        public override Int32 FontWidthTypeMin { get { return 0x00; } }
        public override Int32 FontWidthTypeMax { get { return 0xFFFF; } }
        public override Int32 FontHeightTypeMin { get { return 0x00; } }
        public override Int32 FontHeightTypeMax { get { return 0xFFFF; } }
        public override Int32 FontTypePaddingHorizontal { get { return -1; } }
        public override Int32 FontTypePaddingVertical { get { return 0; } }

        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "0fn" }; } }
        public override String ShortTypeName { get { return "JazzFntC"; } }
        public override String ShortTypeDescription { get { return "Jazz Jackrabbit font (compressed)"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with an unknown text encoding, and variable sizes per symbol. The individual symbols are compressed. The symbols have a peculiar preset order and are rearranged in the editor. ASCII symbols $*/@\\^_`{|} do not exist. Symbols 7F and 9C to 9F likewise won't be saved.\r\n"; } }
        public override String[] GamesListForType { get { return new String[] { "Jazz Jackrabbit" }; } }

        /// <summary>The index on the font that is treated as transparent colour.</summary>
        public override Byte TransparencyColor { get { return 0; } }

        protected Int32 readSpaceWidth = -1;
        protected Int32 baseLineHeight = -1;

        protected readonly Byte[] ReorderTable =
        {
            //_0, 0x_1, 0x_2, 0x_3, 0x_4, 0x_5, 0x_6, 0x_7, 0x_8, 0x_9, 0x_A, 0x_B, 0x_C, 0x_D, 0x_E, 0x_F
            0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50,
            0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66,
            0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76,
            0x77, 0x78, 0x79, 0x7A, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x5B, 0x5D,
            0x3C, 0x3E, 0x81, 0x80, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D,
            0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x9A, 0x97, 0x98, 0x99, 0x9B, 0xA0, 0xA1, 
            0xA2, 0xA3, 0xA4, 0xA5, 0x2C, 0x2E, 0x3F, 0x2D, 0x2B, 0x3D, 0x21, 0x23, 0x25, 0x26, 0x28, 0x29, 
            0x3B, 0x3A, 0x27, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
        };


        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x17)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte[] hdrBytesCheck = Encoding.ASCII.GetBytes(header);
            for (Int32 i = 0; i < hdrBytesCheck.Length; i++)
                if (fileData[i] != hdrBytesCheck[i])
                    throw new FileTypeLoadException(ERR_BADHEADER);
            if (fileData[0x12] != 0x1A || fileData[0x15] != 0x00 || fileData[0x16] != 0x00)
                throw new FileTypeLoadException(ERR_BADHEADER);
            this.readSpaceWidth = fileData[0x13];
            this.baseLineHeight = fileData[0x14];
            Int32 offset = 0x17;
            List<FontFileSymbol> imageDataList = new List<FontFileSymbol>();
            for (Int32 i = 0; i < this.ReorderTable.Length; ++i)
            {
                if (offset == fileData.Length)
                    break;                
                if (offset + 2 > fileData.Length)
                    throw new FileTypeLoadException(ERR_SIZETOOSMALL);
                UInt32 decompSize = (UInt32) ArrayUtils.ReadIntFromByteArray(fileData, offset, 2, true);
                offset += 2;
                if (decompSize == 0)
                {
                    // Dummy item.
                    imageDataList.Add(new FontFileSymbol(this));
                    continue;
                }
                Int32 comprLen = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, offset, 2, true);
                if (offset + 2 + comprLen > fileData.Length)
                    throw new FileTypeLoadException(ERR_SIZETOOSMALL);
                Byte[] decompressedData = JazzRleCompression.RleDecodeJazz(fileData, (UInt32)offset, true, decompSize, true);
                offset += comprLen + 2;
                if (decompressedData == null || decompressedData.Length < 4)
                    throw new FileTypeLoadException(ERR_DECOMPRESS);
                Int32 symbWidth = (Int32) ArrayUtils.ReadIntFromByteArray(decompressedData, 0, 2, true);
                Int32 symbHeight = (Int32)ArrayUtils.ReadIntFromByteArray(decompressedData, 2, 2, true);
                Int32 imgSize = symbWidth * symbHeight;
                if (imgSize != decompSize - 4)
                    throw new FileTypeLoadException("Image dimensions do not match decompressed size!");
                Byte[] symbol = new Byte[imgSize];
                Array.Copy(decompressedData, 4, symbol, 0, imgSize);
                FontFileSymbol ffs = new FontFileSymbol(symbol, symbWidth, symbHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                //symbWidth = (symbWidth + 3) / 4 * 4;
                //ffs.ChangeWidth(symbWidth);
                if (symbWidth > this.m_FontWidth)
                    this.m_FontWidth = symbWidth;
                if (symbHeight > this.m_FontHeight)
                    this.m_FontHeight = symbHeight;
                imageDataList.Add(ffs);
            }
            this.m_ImageDataList = this.RearrangeLoad(imageDataList);
            // If blank, edit it.
            FontFileSymbol space = this.m_ImageDataList[0x20];
            // Present but empty symbol 0x7E does NOT override header space length.
            if (space.Height == 0 && space.Width == 0)
                space.ChangeWidth(this.readSpaceWidth);
        }

        private List<FontFileSymbol> RearrangeLoad(List<FontFileSymbol> imageDataList)
        {
            Int32 length = imageDataList.Count;
            Int32 maxLength = Math.Min(length, this.ReorderTable.Length);
            FontFileSymbol[] symbols = new FontFileSymbol[this.SymbolsTypeMin];
            for (Int32 i = 0; i < maxLength; ++i)
                if (this.ReorderTable[i] >= 0x20)
                    symbols[this.ReorderTable[i]] = imageDataList[i];
            for (Int32 i = 0; i < symbols.Length; ++i)
                if (symbols[i] == null)
                    symbols[i] = new FontFileSymbol(this);
            return new List<FontFileSymbol>(symbols);
        }

        private FontFileSymbol[] RearrangeSave(List<FontFileSymbol> imageDataList, Boolean excludeSpace)
        {
            Int32 sourceLength = imageDataList.Count;
            Int32 length = excludeSpace ? 0x74 : this.ReorderTable.Length;
            FontFileSymbol[] symbols = new FontFileSymbol[length];
            for (Int32 i = 0; i < length; ++i)
            {
                Int32 sourceIndex = this.ReorderTable[i];
                if (sourceIndex < sourceLength && sourceIndex >= 0x20)
                    symbols[i] = imageDataList[sourceIndex];
            }
            return symbols;
        }

        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            Int32 lHeight = this.baseLineHeight;
            if (lHeight < 0)
                lHeight = CalculateLineHeight(this.m_ImageDataList, this.TransparencyColor);
            Boolean widthOnly = this.m_ImageDataList[0x20].Height == 0;
            return new SaveOption[] {
                new SaveOption("SPA", SaveOptionType.Boolean, "Save space as width only", widthOnly ? "1" : "0"),
                new SaveOption("LNH", SaveOptionType.Number, "Line Height", "0,255", lHeight.ToString())
            };
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 lineHeight;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "LNH"), out lineHeight);
            Boolean removeSpace = GeneralUtils.IsTrueValue(SaveOption.GetSaveOptionValue(saveOptions, "SPA"));
            Int32 spaceWidth = 0;
            FontFileSymbol replaceSpace = null;
            if (this.m_ImageDataList.Count > 0x20)
            {
                FontFileSymbol space = this.m_ImageDataList[0x20];
                spaceWidth = space.Width;
                // Game prefers header width over an empty image, so remove it.
                if (spaceWidth == 0 && space.Height == 0)
                    removeSpace = true;
                else if (space.Height == 0 && !removeSpace)
                {
                    replaceSpace = space.Clone();
                    replaceSpace.ChangeHeight(1);
                }
            }
            FontFileSymbol[] symbols = this.RearrangeSave(this.m_ImageDataList, removeSpace);
            if (replaceSpace != null && symbols.Length > 0x7E)
                symbols[0x7E] = replaceSpace;
            Int32 nrOfSymbols = symbols.Length;
            for (Int32 i = symbols.Length; i > 0; --i)
            {
                FontFileSymbol ffs = symbols[i - 1];
                if (ffs != null && ffs.Width != 0 && ffs.Height != 0)
                    break;
                nrOfSymbols = i;
            }
            Byte[][] imageData = new Byte[nrOfSymbols][];
            for (Int32 i = 0; i < nrOfSymbols; ++i)
            {
                FontFileSymbol ffs = symbols[i];
                // It seems the font does not support empty images at all.
                if (ffs == null || ffs.Width == 0 || ffs.Height == 0)
                {
                    // Write dummy entry
                    imageData[i] = new Byte[2];
                    continue;
                }
                Byte[] byteData = ffs.ByteData;
                // Trim off zeroes
                Byte[] writeData = new Byte[byteData.Length + 4];
                ArrayUtils.WriteIntToByteArray(writeData, 0, 2, true, (UInt64)ffs.Width);
                ArrayUtils.WriteIntToByteArray(writeData, 2, 2, true, (UInt64)ffs.Height);
                Array.Copy(byteData, 0, writeData, 4, byteData.Length);
                Byte[] comprData = JazzRleCompression.RleEncodeJazz(writeData);
                // Add uncompressed size in front
                Byte[] comprWriteData = new Byte[comprData.Length + 2];
                ArrayUtils.WriteIntToByteArray(comprWriteData, 0, 2, true, (UInt64)byteData.Length + 4);
                Array.Copy(comprData, 0, comprWriteData, 2, comprData.Length);
                imageData[i] = comprWriteData;
            }
            Int32 bufLen = 0x17;
            for (Int32 i = 0; i < nrOfSymbols; i++)
                bufLen += imageData[i].Length;
            Byte[] fontData = new Byte[bufLen];
            Encoding.ASCII.GetBytes(header, 0, header.Length, fontData, 0);
            fontData[0x12] = 0x1A;
            fontData[0x13] = (Byte)Math.Min(255, spaceWidth);
            fontData[0x14] = (Byte)Math.Min(255, lineHeight);
            // 0x15 & 0x16 are both 00.
            Int32 writeOffs = 0x17;
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                Byte[] curData = imageData[i];
                Int32 curLength = curData.Length;
                Array.Copy(curData, 0, fontData, writeOffs, curLength);
                writeOffs += curLength;
            }
            return fontData;
        }
    }
}