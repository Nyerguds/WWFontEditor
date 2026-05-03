using System;
using System.Collections.Generic;
using System.Linq;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileEsi : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x21; } }
        public override Int32 SymbolsTypeMax { get { return 0x80; } }
        public override Int32 SymbolsTypeFirst { get { return 0x20; } }
        public override Int32 FontWidthTypeMin { get { return 0x01; } }
        public override Int32 FontWidthTypeMax { get { return 0x800; } }
        public override Int32 FontHeightTypeMin { get { return 0x00; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font.</summary>
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override Int32 FontTypePaddingHorizontal { get { return -1; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "esi", "sms" }; } }
        public override String ShortTypeName { get { return "ESI Font"; } }
        public override String ShortTypeDescription { get { return "ESI Font"; } }
        public override String LongTypeDescription { get { return "A 1-bpp font which only covers ASCII characters after the space. It is encrypred with a rolling XORcypher."; } }
        public override String[] GamesListForType
        {
            get
            {
                return new String[]
                {
                    "Empire: Wargame of the Century",
                    "Family Feud",
                    "Fun House",
                    "The Honeymooners",
                    "Jeopardy!",
                    "Jeopardy!: Junior Edition",
                    "Jeopardy!: First Edition",
                    "Jeopardy!: Second Edition",
                    "Jeopardy!: Third Edition",
                    "Jeopardy!: Sports Edition",
                    "Jim Henson's Muppet Adventure No. 1: Chaos at the Carnival",
                    "Rapcon",
                    "Sesame Street: First Writer",
                    "Tracon: Air Traffic Control Simulator",
                    "Tracon 2"
                };
            }
        }

        private static readonly Byte[] XorKey = new Byte[] { 0xA8, 0xC3, 0xA9, 0xB1, 0xB9, 0xB8, 0xB4, 0xD7, 0xCB, 0xCD, 0xC1, 0xD3, 0xCF, 0xCE };

        private Byte _unknown0x00;
        private Byte _unknown0x03;
        private Byte _unknown0x04;
        private Byte _unknown0x0B;
        //private Byte _unknown0x0C;
        //private Byte _unknown0x6C;

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x6D)
                throw new FileTypeLoadException(ERR_NOHEADER);
            this._unknown0x00 = fileData[0];
            Byte nrOfSymbols = fileData[1];
            if (nrOfSymbols == 0)
                throw new FileTypeLoadException("No symbols in font file.");
            SByte charsShift = (SByte)fileData[2];
            this._unknown0x03 = fileData[3];
            this._unknown0x04 = fileData[4];
            Byte height = fileData[5];
            if (height == 0)
                throw new FileTypeLoadException("Height value is zero.");
            this.FontHeight = height;
            Int32 fontWidthBytes = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 6, 2, true);
            if (fontWidthBytes == 0)
                throw new FileTypeLoadException("Width value is zero.");
            this.FontWidth = fontWidthBytes * 8;
            Int32 bytesPerSymbol = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 8, 2, true);
            if (bytesPerSymbol == 0)
                throw new FileTypeLoadException("Symbol size is zero.");
            Int32 dataSize = bytesPerSymbol * nrOfSymbols;
            // Why is this usually larger than fileData[5]?? Makes no sense.
            Int32 symbolheight = bytesPerSymbol / fontWidthBytes;
            this.BaseLineHeight = fileData[0x0A];
            this._unknown0x0B = fileData[0x0B];
            //this._unknown0x0C = fileData[0x0C];
            // not affected by nrOfSymbols value.
            Byte[] charWidths = new Byte[0x5F];
            Array.Copy(fileData, 0x0D, charWidths, 0, 0x5F);
            //this._unknown0x6C = fileData[0x6C];
            if ((fileData.Length - 0x6D) != dataSize)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            // End of FileTypeLoadExceptions. After this, assume the type is identified.
            Byte[] graphicsData = new Byte[dataSize];
            Array.Copy(fileData, 0x6D, graphicsData, 0, dataSize);
            Int32 xorSize = XorKey.Length;
            for (Int32 i = 0; i < dataSize; i++)
                graphicsData[i] = (Byte)(graphicsData[i] ^ XorKey[i % xorSize]);
            this.m_ImageDataList = new List<FontFileSymbol>();
            for (Int32 i = 0; i < 0x20; ++i)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 0, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor));
            FontFileSymbol space = new FontFileSymbol(this);
            // no known space width value. Put it to the width of the 7F char if available, and if not, take the max width of the font symbols and half it.
            Int32 spaceWidth;
            if (charWidths[0x5E] != 0)
                spaceWidth = charWidths[0x5E];
            else
                spaceWidth = charWidths.Max() / 2;
            space.ChangeWidth(spaceWidth, this.TransparencyColor);
            this.m_ImageDataList.Add(space);
            Int32 bitsLength = this.BitsPerPixel;
            Int32 symbolWidth = fontWidthBytes * 8;
            for (Int32 i = 0; i < nrOfSymbols; ++i)
            {
                Int32 offset = (dataSize + ((charsShift - 1 + i) * bytesPerSymbol)) % dataSize;
                Byte width = charWidths[(charsShift - 1 + i) % 0x5F];
                Byte[] data8Bit;
                try
                {
                    data8Bit = ImageUtils.ConvertTo8Bit(graphicsData, symbolWidth, symbolheight, offset, bitsLength, true);
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i), ex);
                }
                FontFileSymbol fc = new FontFileSymbol(data8Bit, symbolWidth, symbolheight, 0, bitsLength, this.TransparencyColor);
                fc.ChangeWidth(width);
                fc.ChangeHeight(height);
                this.m_ImageDataList.Add(fc);
            }

        }


        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            Int32 lHeight = this.BaseLineHeight;
            if (lHeight == 0)
                lHeight = FontFileDynV4.CalculateLineHeight(this.m_ImageDataList, this.BitsPerPixel, this.FontHeightTypeMax, this.TransparencyColor);

            return new SaveOption[]
            {
                new SaveOption("UN0", SaveOptionType.Number, "Unknown value 0x00", "0,255", this._unknown0x00.ToString()),
                new SaveOption("UN3", SaveOptionType.Number, "Unknown value 0x03", "0,255", this._unknown0x03.ToString()),
                new SaveOption("UN4", SaveOptionType.Number, "Unknown value 0x04", "0,255", this._unknown0x04.ToString()),
                new SaveOption("YOF", SaveOptionType.Number, "Font base line Y-offset (Byte 0x0A)", "0,255", lHeight.ToString()),
                new SaveOption("UNB", SaveOptionType.Number, "Unknown value 0x0B", "0,255", this._unknown0x0B.ToString()),
                //new SaveOption("UNC", SaveOptionType.Number, "Unknown value 0x0C", "0,255", this._unknown0x0C.ToString()),
                //new SaveOption("U6C", SaveOptionType.Number, "Unknown value 0x6C", "0,255", this._unknown0x6C.ToString())
            };
        }
        
        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 unknown0x00;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "UN0"), out unknown0x00);
            Int32 unknown0x03;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "UN3"), out unknown0x03);
            Int32 unknown0x04;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "UN4"), out unknown0x04);
            Int32 lineHeight;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "YOF"), out lineHeight);
            Int32 unknown0x0B;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "UNB"), out unknown0x0B);
            //Int32 unknown0x0C;
            //Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "UNC"), out unknown0x0C);
            //Int32 unknown0x6C;
            //Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "U6C"), out unknown0x6C);
            Int32 nrOfSymbols = m_ImageDataList.Count - 0x21;
            Int32 actualHeight = ((this.FontHeight + 7) / 8) * 8;
            Int32 actualWidthBytes = (this.FontWidth + 7) / 8;
            Int32 actualWidth = actualWidthBytes * 8;
            Int32 bytesPerSymbol = actualWidthBytes * actualHeight;
            Byte[] symbolWidths = new Byte[0x5F];
            Int32 dataSize = bytesPerSymbol * nrOfSymbols;
            Byte[] graphicsData = new Byte[dataSize];
            for (Int32 i = 0; i < nrOfSymbols; ++i)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i + 0x21].Clone();
                symbolWidths[i] = (Byte)ffs.Width;
                ffs.ChangeWidth(actualWidth);
                Byte[] data = ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, ffs.Height, this.BitsPerPixel, true);
                Array.Copy(data, 0, graphicsData, i * bytesPerSymbol, data.Length);
            }
            // Apply xor "encryption".
            Int32 xorSize = XorKey.Length;
            for (Int32 i = 0; i < dataSize; i++)
                graphicsData[i] = (Byte)(graphicsData[i] ^ XorKey[i % xorSize]);
            Byte[] fullFile = new Byte[0x6D + dataSize];
            fullFile[0x00] = (Byte)unknown0x00;
            fullFile[0x01] = (Byte)nrOfSymbols;
            fullFile[0x02] = 1; // I see no point in making this customisable.
            fullFile[0x03] = (Byte)unknown0x03;
            fullFile[0x04] = (Byte)unknown0x04;
            fullFile[0x05] = (Byte) this.FontHeight;
            ArrayUtils.WriteIntToByteArray(fullFile, 6, 2, true, (UInt64) actualWidthBytes);
            ArrayUtils.WriteIntToByteArray(fullFile, 8, 2, true, (UInt64) bytesPerSymbol);
            fullFile[0x0A] = (Byte)lineHeight;
            fullFile[0x0B] = (Byte)unknown0x0B;
            //fullFile[0x0C] = (Byte)unknown0x0C;
            Array.Copy(symbolWidths, 0, fullFile, 0x0D, symbolWidths.Length);
            //fullFile[0x6C] = (Byte)unknown0x6C;
            Array.Copy(graphicsData, 0, fullFile, 0x6D, dataSize);
            return fullFile;
        }
    }
}
