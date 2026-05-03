using System;
using System.Linq;
using System.Collections.Generic;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Tiberian Sun format
    /// </summary>
    public class FontFileD2K : FontFile
    {
        public override Int32 CharactersMax { get { return 0x100; } }
        public override Int32 FontWidthMax { get { return 0xFF; } }
        public override Int32 FontHeightMax { get { return 0xFF; } }
        public override Int32 YOffsetMax { get { return 0x0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        public override String ShortTypeCode { get { return "IG D2K"; } }
        public override String LongTypeCode { get { return "IG Font (Dune 2000)"; } }
        public override String[] GamesList { get { return new String[] { "Dune 2000" }; } }

        public override void LoadFont(Byte[] fileData)
        {
            // Technically header + first symbol header, but whatev :p
            if (fileData.Length < 0x410)
                throw new LoadFailedException(ERR_NOHEADER);

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
                throw new LoadFailedException("Identifying bytes do not match!");

            this.m_FontHeight = maxHeight;
            // Wlll be increased to the max found in the file.
            this.m_FontWidth = spaceSize;
            for (Int32 i = 0; i < firstSymbol; i++)
                this.m_ImageDataList.Add(new FontFileCharacter(this.BitsPerPixel));
            if (firstSymbol > 0x20)
                this.m_ImageDataList[0x20].Width = spaceSize;
            Int32 readOffset = 0x408;
            // Check on "readOffset + 8" because 8 is the byte size of a next character header.
            Int32 datalen = fileData.Length;
            Int32 symbolCounter = 0;
            Byte currentSymbol = firstSymbol;
            while (readOffset + 8 < datalen)// && symbolCounter < 256)
            {
                Int32 symbolWidth = ArrayUtils.GetLEIntFromByteArray(fileData, readOffset);
                this.m_FontWidth = Math.Max(symbolWidth, this.m_FontWidth);
                readOffset += 4;
                Int32 symbolHeight = ArrayUtils.GetLEIntFromByteArray(fileData, readOffset);
                readOffset += 4;
                Byte[] symbolData = new Byte[symbolWidth*symbolHeight];
                if (readOffset + symbolData.Length > datalen)
                    throw new Exception("File data too short for character data of symbol #" + firstSymbol + ".");
                Array.Copy(fileData, readOffset, symbolData, 0, symbolData.Length);
                // should happen after the currentSymbol byte wraps around to 0
                if (m_ImageDataList.Count > currentSymbol)
                {
                    FontFileCharacter ch = this.m_ImageDataList[currentSymbol];
                    ch.ByteData = symbolData;
                    ch.Width = symbolWidth;
                    ch.Height = symbolHeight;
                }
                else
                    this.m_ImageDataList.Add(new FontFileCharacter(symbolData, symbolWidth, symbolHeight, 0, this.BitsPerPixel));
                readOffset += symbolData.Length;
                symbolCounter++;
                currentSymbol++;
            }
        }

        public override Byte[] SaveFont()
        {
            FontFileCharacter[] baseList = new List<FontFileCharacter>(m_ImageDataList).ToArray();
            FontFileCharacter[] newList = new FontFileCharacter[255];
            Byte firstSymbol = 0x21;
            // this is FF and not 100 because the space itself is omitted.
            Int32 remainingSymbols = newList.Length - firstSymbol; // 222 ?
            Array.Copy(baseList, firstSymbol, newList, 0, remainingSymbols);
            Array.Copy(baseList, 0, newList, remainingSymbols, firstSymbol);

            Int32 fileLen = 0x408 + newList.Select(x => x.ByteData.Length + 8).Sum();
            Byte[] fileData = new Byte[fileLen];
            fileData[0] = 0x01;
            fileData[1] = (Byte)this.m_ImageDataList[0x20].Width; // space width
            fileData[2] = firstSymbol;
            fileData[3] = 0x01; // I'unno man. I might width-optimize the fot later to set this, but for not I'm hardcoding it to 1 pixel.
            fileData[4] = (Byte)m_FontHeight;
            //fileData[5] = 0x00;
            //fileData[6] = 0x00;
            //fileData[7] = 0x00;
            //0x08 => 0x408: giant load of crap. Leave empty.
            // newlist should contain all except the space
            Int32 writeOffset = 0x408;
            foreach (FontFileCharacter fc in newList)
            {
                ArrayUtils.SetLEIntInByteArray(fileData, writeOffset, fc.Width);
                writeOffset += 4;
                ArrayUtils.SetLEIntInByteArray(fileData, writeOffset, fc.Height);
                writeOffset += 4;
                Byte[] bdata = fc.ByteData;
                Array.Copy(bdata, 0, fileData, writeOffset, bdata.Length);
                writeOffset += bdata.Length;
            }
            return fileData;
        }
    }
}