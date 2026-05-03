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
        protected override Int32 InternalEditBPP { get { return 4; } }
        public override String ShortTypeName { get { return "IG D2K"; } }
        public override String ShortTypeDescription { get { return "IG Font (Dune 2000)"; } }
        public override String LongTypeDescription { get { return "An 8 BPP font with a fixed set of 256 characters, which allows separate symbols to specify their width and height. It has no Y offset, but instead optimizes the space to the right of all characters."; } }
        public override String[] GamesListForType { get { return new String[] { "Dune 2000" }; } }

        public override void LoadFont(Byte[] fileData)
        {
            // Technically header + first symbol header, but whatev :p
            if (fileData.Length < 0x410)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte fontLoadedFlag = fileData[00];
            Byte spaceSize = fileData[01];
            Byte firstSymbol = fileData[02];
            Byte interval = fileData[03];
            Byte maxHeight = fileData[04];
            Byte empty05 = fileData[05];
            Byte empty06 = fileData[06];
            Byte empty07 = fileData[07];
            //No clue if this is ok as test...
            if (fontLoadedFlag != 1 || empty05 != 0 || empty06 != 0 || empty07 != 0)
                throw new FileTypeLoadException("Identifying bytes in header do not match.");
            this.m_FontHeight = maxHeight;
            // Wlll be increased to the max found in the file.
            this.m_FontWidth = spaceSize;
            for (Int32 i = 0; i < firstSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(this.BitsPerPixel));
            if (firstSymbol > 0x20)
                this.m_ImageDataList[0x20].Width = spaceSize;
            Int32 readOffset = 0x408;
            // Check on "readOffset + 8" because 8 is the byte size of a next symbol header.
            Int32 datalen = fileData.Length;
            Int32 symbolCounter = 0; // Just to be sure and stay below 256
            Byte currentSymbol = firstSymbol;
            while (readOffset + 8 < datalen && symbolCounter < 256)
            {
                Int32 symbolWidth = ArrayUtils.GetLEIntFromByteArray(fileData, readOffset);
                this.m_FontWidth = Math.Max(symbolWidth, this.m_FontWidth);
                readOffset += 4;
                Int32 symbolHeight = ArrayUtils.GetLEIntFromByteArray(fileData, readOffset);
                readOffset += 4;
                Byte[] symbolData = new Byte[symbolWidth*symbolHeight];
                if (readOffset + symbolData.Length > datalen)
                    throw new Exception("File data too short for symbol data of symbol #" + firstSymbol + ".");
                Array.Copy(fileData, readOffset, symbolData, 0, symbolData.Length);
                // should happen after the currentSymbol byte wraps around to 0
                if (m_ImageDataList.Count > currentSymbol)
                {
                    FontFileSymbol ch = this.m_ImageDataList[currentSymbol];
                    ch.ByteData = symbolData;
                    ch.Width = symbolWidth;
                    ch.Height = symbolHeight;
                }
                else
                    this.m_ImageDataList.Add(new FontFileSymbol(symbolData, symbolWidth, symbolHeight, 0, this.BitsPerPixel));
                readOffset += symbolData.Length;
                symbolCounter++;
                currentSymbol++;
            }
            // interval is right-edge X optimization much like WW does Y optimization. Pad it onto the font. The Save will trim it off again.
            if (interval > 0)
            {
                this.m_FontWidth += interval;
                foreach (FontFileSymbol fs in m_ImageDataList)
                    if (fs.Width != 0 || fs.Height != 0)
                        fs.ChangeWidth(fs.Width + interval);
            }
        }

        public override Byte[] SaveFont()
        {
            FontFileSymbol[] baseList = new List<FontFileSymbol>(m_ImageDataList).ToArray();
            FontFileSymbol[] newList = new FontFileSymbol[255];
            Byte spaceWidth = (Byte)this.m_ImageDataList[0x20].Width;
            Byte firstSymbol = 0x21;
            // this is FF and not 100 because the space itself is omitted.
            Int32 remainingSymbols = newList.Length - firstSymbol; // 222 ?
            Array.Copy(baseList, firstSymbol, newList, 0, remainingSymbols);
            Array.Copy(baseList, 0, newList, remainingSymbols, firstSymbol);

            // Code to detect how much space at the right edge is added padding to create space between pixels.
            // This space is trimmed off and added in the header instead.
            // Start from max that can be trimmed off the space, since it's not in the list.
            Int32 globalOpenSpace = spaceWidth;
            foreach (FontFileSymbol fs in m_ImageDataList)
            {
                // ignore completely empty characters; they'd reduce it to 0 for no reason.
                if (fs.Width == 0 && fs.Height == 0)
                    continue;
                Byte[] byteData = fs.ByteData;
                Int32 width = fs.Width;
                Int32 height = fs.Height;
                Int32 minOpenSpace = width;
                for (Int32 y = 0; y < height; y++)
                {
                    Byte[] line = new Byte[width];
                    Array.Copy(byteData, y*width, line, 0, width);
                    minOpenSpace = Math.Min(minOpenSpace, line.Reverse().TakeWhile(x => x == 0).Count());
                }
                globalOpenSpace = Math.Min(globalOpenSpace, minOpenSpace);
                if (globalOpenSpace == 0)
                    break;
            }
            if (globalOpenSpace > 0)
            {
                spaceWidth -= (Byte)globalOpenSpace;
                for (Int32 i = 0; i < newList.Length; i++)
                {
                    // change list to clones with adapted width
                    FontFileSymbol fs = newList[i].Clone();
                    fs.ChangeWidth(fs.Width - globalOpenSpace);
                    newList[i] = fs;
                }
                // font width should not be reduced by globalOpenSpace; it is unused in the saving process.
            }
            // TODO: ADD CODE HERE TO DECREASE FONT WIDTHS BY ABOVE CALCULATED AMOUNT OF PIXELS

            Int32 fileLen = 0x408 + newList.Select(x => x.ByteData.Length + 8).Sum();
            Byte[] fileData = new Byte[fileLen];
            fileData[0] = 0x01;
            fileData[1] = spaceWidth; // space width
            fileData[2] = firstSymbol;
            fileData[3] = (Byte)globalOpenSpace; // space between characters
            fileData[4] = (Byte)m_FontHeight;
            //fileData[5] = 0x00;
            //fileData[6] = 0x00;
            //fileData[7] = 0x00;
            //0x08 => 0x408: giant load of crap. Leave empty, I guess?
            // newlist should contain all except the space
            Int32 writeOffset = 0x408;
            foreach (FontFileSymbol fs in newList)
            {
                ArrayUtils.SetLEIntInByteArray(fileData, writeOffset, fs.Width);
                writeOffset += 4;
                ArrayUtils.SetLEIntInByteArray(fileData, writeOffset, fs.Height);
                writeOffset += 4;
                Byte[] bdata = fs.ByteData;
                Array.Copy(bdata, 0, fileData, writeOffset, bdata.Length);
                writeOffset += bdata.Length;
            }
            return fileData;
        }
    }
}