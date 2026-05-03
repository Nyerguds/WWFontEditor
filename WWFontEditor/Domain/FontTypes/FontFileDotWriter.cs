using System;
using System.Collections.Generic;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileDotWriter : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x20; } }
        public override Int32 SymbolsTypeMax { get { return 0x255; } }
        /// <summary>The first symbol that is saved. This hides all symbols before this index from the editor.</summary>
        public override Int32 SymbolsTypeFirst { get { return 0x20; } }
        public override Int32 FontWidthTypeMin { get { return 0x0A; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMin { get { return 0x00; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Byte TransparencyColor { get { return 0x00; } }
        /// <summary>Padding at the bottom of the font. Only used for the preview function.</summary>
        public override Int32 FontTypePaddingVertical { get { return 0; } }
        /// <summary>Padding between the characters of the font.</summary>
        public override Int32 FontTypePaddingHorizontal { get { return this.m_padding; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "pr", "pri", "" }; } }
        public override Boolean CustomSymbolWidthsForType { get { return true; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DotWriter font"; } }
        public override String ShortTypeDescription { get { return "DotWriter Font"; } }
        public override String LongTypeDescription { get { return "Font of the DotWriter application."; } }
        public override String[] GamesListForType { get { return new String[0]; } }
        /// <summary>Supported types can always be loaded, but this indicates if save functionality to this type is also available.</summary>
        public override Boolean CanSave { get { return false; } }

        protected Int32 m_padding = -1;

        public Boolean LittleEndianBits { get; set; }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x0A)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int32 symbolSize = fileData[0] + (fileData[1] << 8);
            Int32 symbolWidth = fileData[2];
            if (symbolSize % symbolWidth != 0)
                throw new FileTypeLoadException(ERR_BADHEADERDATA + " Length is not divisible by width.");
            Int32 symbolHeightHeader = fileData[4];
            Int32 version = fileData[5];
            // Amount of 8-pixel blocks to form one symbol.
            Int32 symbolStack = fileData[6];
            if (symbolSize / symbolWidth != symbolStack)
                throw new FileTypeLoadException(ERR_BADHEADERDATA + " Symbol height doesn't match size.");
            Int32 nrOfSymbols = fileData[8];
            Int32 heightDataOffset = symbolSize*nrOfSymbols;
            if (fileData.Length < heightDataOffset + nrOfSymbols)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            // If absolutely ALL highest bits in the entire font are true, assume little-endian.
            Boolean littleEndian = true;
            for (Int32 i = symbolSize; i < heightDataOffset; ++i)
            {
                if ((fileData[i] & 0x80) != 0)
                    continue;
                littleEndian = false;
            }
            Int32 blockHeight = littleEndian ? 7 : 8;
            Int32 symbolHeight = blockHeight * symbolStack;
            Int32 usableHeight = symbolHeightHeader;
            if (version >= 3)
            {
                if (littleEndian && (symbolHeightHeader & 0x80) != 0)
                    usableHeight = symbolHeight - (symbolHeightHeader ^ 0x80);
                else
                    usableHeight = symbolHeight - symbolHeightHeader;
            }
            Boolean viableHeaderHeight = usableHeight < symbolHeight && usableHeight > 0;
            Int32 spaceWidth = fileData[heightDataOffset];
            if (spaceWidth > symbolWidth)
                spaceWidth = symbolWidth;
            FontFileSymbol[] imageDataList = new FontFileSymbol[0x20 + nrOfSymbols];
            Int32 symbolSize8Bit = symbolWidth*symbolHeight;
            Int32 offset = symbolSize;
            Int32 blockSize8Bit = blockHeight * symbolWidth;
            Int32 maxYWithData = 0;
            // Loop over symbols
            for (Int32 s = 1; s < nrOfSymbols; ++s)
            {
                Byte[] symbolData = new Byte[symbolSize8Bit];
                // Build image
                for (Int32 h = 0; h < symbolStack; ++h)
                {
                    Byte[] eightLines = new Byte[symbolWidth];
                    Array.Copy(fileData, offset + symbolWidth * h, eightLines, 0, symbolWidth);
                    Int32 stride = 1;
                    // This resuls in a flipped image, that is 8 (or 7) wide and the height of the symbol width.
                    Byte[] eightLinesEightBit = ImageUtils.ConvertTo8Bit(eightLines, blockHeight, symbolWidth, 0, 1, !littleEndian, ref stride);
                    Int32 writeOffs = blockSize8Bit*h;
                    // write data into the final array at the right offset, in the right orientation.
                    for (Int32 y = 0; y < blockHeight; y++)
                        for (Int32 x = 0; x < symbolWidth; x++)
                            symbolData[writeOffs + (y * symbolWidth + x)] = eightLinesEightBit[x * blockHeight + y];
                }
                // If the height value in the header is a sane value, check lowest pixel data to see if it is usable.
                if (viableHeaderHeight)
                {
                    Boolean foundPixel = false;
                    for (Int32 y = symbolHeight - 1; y >= 0; --y)
                    {
                        Int32 checkOffset = symbolWidth*y;
                        for (Int32 x = 0; x < symbolWidth; ++x)
                        {
                            if (symbolData[checkOffset + x] == 0)
                                continue;
                            if (y > maxYWithData)
                                maxYWithData = y;
                            foundPixel = true;
                            break;
                        }
                        if (foundPixel)
                            break;
                    }
                }
                FontFileSymbol curSymbol = new FontFileSymbol(symbolData, symbolWidth, symbolHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                Int32 curSymbolWidth = fileData[heightDataOffset+s];
                //if (symbolHeightActual != symbolHeight)
                //    curSymbol.ChangeHeight(symbolHeightActual);
                if (curSymbolWidth < symbolWidth)
                curSymbol.ChangeWidth(curSymbolWidth);
                imageDataList[0x20 + s] = curSymbol;
                offset += symbolSize;
            }
            for (Int32 i = 0; i < 0x20; ++i)
                imageDataList[i] = new FontFileSymbol(new Byte[0], 0, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
            FontFileSymbol ffs = new FontFileSymbol(new Byte[spaceWidth * symbolHeight], spaceWidth, symbolHeight, 0, this.BitsPerPixel, this.TransparencyColor);
            imageDataList[0x20] = ffs;
            this.m_ImageDataList = new List<FontFileSymbol>(imageDataList);
            this.m_FontHeight = symbolHeight;
            this.m_FontWidth = symbolWidth;
            if (viableHeaderHeight && usableHeight > maxYWithData)
                this.SetFontHeight(usableHeight);
            else if (version >= 3)
            {
            }
            // For saving.
            this.LittleEndianBits = littleEndian;
            this.ExtraInfo = "Height in header: " + symbolHeightHeader + "\nBit order: " + (littleEndian ? "little-" : "big-") + "endian";
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            throw new NotSupportedException();
        }
    }
}
