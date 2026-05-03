using System;
using System.Linq;
using System.Collections.Generic;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Tiberian Sun format
    /// </summary>
    public class FontFileD2K : FontFile
    {
        // disable removing chars for d2k fonts.
        public override Int32 SymbolsTypeMin { get { return 0x100; } }
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fnt" }; } }
        public override String ShortTypeName { get { return "IG D2K"; } }
        public override String ShortTypeDescription { get { return "IG Font (Dune 2000)"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with a fixed set of 256 characters, which allows separate symbols to specify their width and height. It has no Y offset, but instead optimizes the space to the right of all characters."; } }
        public override String[] GamesListForType { get { return new String[] { "Dune 2000" }; } }

        public override void LoadFont(Byte[] fileData)
        {
            // Technically header + first symbol header, but whatev :p
            if (fileData.Length < 0x410)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte index00 = fileData[00]; // "FontLoadedFlag" according to Siberian GRemlin. No idea why he called it that.
            Byte spaceWidth = fileData[01];
            Byte firstSymbol = fileData[02];
            Byte padding = fileData[03];
            Byte maxHeight = fileData[04];
            Byte empty05 = fileData[05];
            Byte empty06 = fileData[06];
            Byte empty07 = fileData[07];
            //No clue if this is ok as test...
            if (index00 != 1 || empty05 != 0 || empty06 != 0 || empty07 != 0)
                throw new FileTypeLoadException(ERR_BADHEADER);
            this.m_FontHeight = maxHeight;
            // Wlll be increased to the max found in the file.
            this.m_FontWidth = spaceWidth;

            // Initialize symbols array
            FontFileSymbol[] imageDataList = new FontFileSymbol[0x100];
            // Prepare space
            Int32 spacePos = firstSymbol - 1;
            if (spacePos < 0)
                spacePos += 0x100;
            FontFileSymbol space = new FontFileSymbol(this);
            space.ChangeWidth(spaceWidth, this.TransparencyColor);
            // Add space
            imageDataList[spacePos] = space;
            // Read the rest of the symbols.
            Int32 readOffset = 0x408;
            Int32 datalen = fileData.Length;
            Byte currentSymbol = firstSymbol;
            // Check on "readOffset + 8" because 8 is the byte size of a next symbol header.
            // This should ignore the last symbol in the data, since it'll be a 0x0 dummy to be replaced by the space width.
            while (readOffset + 8 < datalen && currentSymbol != spacePos)
            {
                Int32 symbolWidth = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, readOffset, 4, true);
                this.m_FontWidth = Math.Max(symbolWidth, this.m_FontWidth);
                readOffset += 4;
                Int32 symbolHeight = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, readOffset, 4, true);
                readOffset += 4;
                Byte[] symbolData = new Byte[symbolWidth*symbolHeight];
                if (readOffset + symbolData.Length > datalen)
                    throw new Exception("File data too short for symbol data of symbol #" + firstSymbol + ".");
                Array.Copy(fileData, readOffset, symbolData, 0, symbolData.Length);
                FontFileSymbol ffs = new FontFileSymbol(symbolData, symbolWidth, symbolHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                // Add header padding.
                if (padding > 0 && ffs.Width != 0 || ffs.Height != 0)
                    ffs.ChangeWidth(ffs.Width + padding, this.TransparencyColor);
                imageDataList[currentSymbol] = ffs;
                readOffset += symbolData.Length;
                // Byte will wrap around after 0xFF
                currentSymbol++;
            }
            // Probably not needed, but eh, just to be safe.
            for (Int32 i = 0; i < 0x100; ++i)
                if (imageDataList[i] == null)
                    imageDataList[i] = new FontFileSymbol(this);
            // interval is right-edge X optimization much like WW does Y optimization. Pad it onto the font. The Save will trim it off again.
            if (padding > 0)
                this.m_FontWidth += padding;
            this.m_ImageDataList = new List<FontFileSymbol>(imageDataList);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            FontFileSymbol[] baseList = new List<FontFileSymbol>(this.m_ImageDataList).ToArray();
            FontFileSymbol[] newList = new FontFileSymbol[256];
            FontFileSymbol newSpace = baseList[0x20].Clone();
            Byte spaceWidth = (Byte)newSpace.Width;
            // Final saved data does in fact contain a 0x0 dummy entry for the space character... further invalidating the whole optimisation effort.
            newSpace.ChangeWidth(0, this.TransparencyColor);
            newSpace.ChangeHeight(0, this.TransparencyColor);
            baseList[0x20] = newSpace;
            Byte firstSymbol = 0x21;
            // this is FF and not 100 because the space itself is omitted.
            Int32 remainingSymbols = 0x100 - firstSymbol; // 222 ?
            Array.Copy(baseList, firstSymbol, newList, 0, remainingSymbols);
            Array.Copy(baseList, 0, newList, remainingSymbols, firstSymbol);

            // Code to detect how much space at the right edge is added padding to create space between pixels.
            // This space is trimmed off and added in the header instead.
            // Start from max that can be trimmed off the space, since it's not in the list.
            Int32 globalOpenSpace = spaceWidth;
            Int32 images = this.m_ImageDataList.Count;
            for (Int32 i = 0; i < images; ++i)
            {
                FontFileSymbol fs = this.m_ImageDataList[i];
                // ignore completely empty characters; they'd reduce it to 0 for no reason.
                if (fs.Width == 0 && fs.Height == 0)
                    continue;
                Byte[] byteData = fs.ByteData;
                Int32 width = fs.Width;
                Int32 height = fs.Height;
                Int32 minOpenSpace = width;
                for (Int32 y = 0; y < height; ++y)
                {
                    Byte[] line = new Byte[width];
                    Array.Copy(byteData, y * width, line, 0, width);
                    minOpenSpace = Math.Min(minOpenSpace, line.Reverse().TakeWhile(x => x == 0).Count());
                }
                globalOpenSpace = Math.Min(globalOpenSpace, minOpenSpace);
                if (globalOpenSpace == 0)
                    break;
            }
            if (globalOpenSpace > 0)
            {
                spaceWidth -= (Byte)globalOpenSpace;
                for (Int32 i = 0; i < 0x100; ++i)
                {
                    // change list to clones with adapted width
                    FontFileSymbol fs = newList[i].Clone();
                    if (fs.Width > 0 && fs.Height > 0)
                        fs.ChangeWidth(fs.Width - globalOpenSpace, this.TransparencyColor);
                    newList[i] = fs;
                }
                // font width should not be reduced by globalOpenSpace; it is unused in the saving process.
            }
            Int32 fileLen = 0x408 + newList.Select(x => x.ByteData.Length + 8).Sum();
            Byte[] fileData = new Byte[fileLen];
            fileData[0] = 0x01;
            fileData[1] = spaceWidth; // space width
            fileData[2] = firstSymbol;
            fileData[3] = (Byte)globalOpenSpace; // space between characters
            fileData[4] = (Byte) this.m_FontHeight;
            //fileData[5] = 0x00;
            //fileData[6] = 0x00;
            //fileData[7] = 0x00;
            //0x08 => 0x408: giant load of crap. Leave empty, I guess?
            Int32 writeOffset = 0x408;
            for (Int32 i = 0; i < 0x100; ++i)
            {
                FontFileSymbol fs = newList[i];
                ArrayUtils.WriteIntToByteArray(fileData, writeOffset, 4, true, (UInt32) fs.Width);
                writeOffset += 4;
                ArrayUtils.WriteIntToByteArray(fileData, writeOffset, 4, true, (UInt32) fs.Height);
                writeOffset += 4;
                Byte[] bdata = fs.ByteData;
                Array.Copy(bdata, 0, fileData, writeOffset, bdata.Length);
                writeOffset += bdata.Length;
            }
            return fileData;
        }
    }
}