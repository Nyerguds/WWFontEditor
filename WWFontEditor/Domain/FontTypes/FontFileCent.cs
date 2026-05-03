using System;
using System.Linq;
using System.Text;
using Nyerguds.Util;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileCent : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 FontWidthTypeMax { get { return 0x08; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font. Automatically disables if max and min for both dimensions are the same.</summary>
        public override Boolean CustomSymbYForType { get { return false; } }

        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fnt" }; } }
        public override String ShortTypeName { get { return "CenFont"; } }
        public override String ShortTypeDescription { get { return "Centurion Font"; } }
        public override String LongTypeDescription { get { return "Artificially created format to edit the Centurion fonts."; } }
        public override String[] GamesListForType { get { return new String[] { "Centurion" }; } }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x08)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte[] hdrId = new Byte[4];
            Array.Copy(fileData, 0, hdrId, 0, 4);
            if (!hdrId.SequenceEqual(Encoding.ASCII.GetBytes("CFNT")))
                throw new FileTypeLoadException(ERR_BADHEADER);
            this.m_FontWidth = fileData[4];
            if (this.m_FontWidth > 8)
                throw new FileTypeLoadException("Centurion fonts do not support widths beyond 8 pixels!");
            this.m_FontHeight = fileData[5];
            Int32 startSymbol = fileData[6];
            Int32 nrOfCharacters = fileData[7];
            if (fileData.Length != 8 + nrOfCharacters * (this.m_FontHeight + 2))
                throw new FileTypeLoadException(ERR_SIZECHECK);
            Byte[] characterWidths = new Byte[nrOfCharacters];
            Byte[] characterYOffsets = new Byte[nrOfCharacters];
            Array.Copy(fileData, 8, characterWidths, 0, nrOfCharacters);
            Array.Copy(fileData, 8 + nrOfCharacters, characterYOffsets, 0, nrOfCharacters);
            Byte[][] characterData = new Byte[nrOfCharacters][];
            for (Int32 i = 0; i < nrOfCharacters; i++)
            {
                Byte[] charData = new Byte[this.m_FontHeight];
                Array.Copy(fileData, 8 + nrOfCharacters * 2 + i * this.m_FontHeight, charData, 0, this.m_FontHeight);
                characterData[i] = charData;
            }
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 0, this.m_FontHeight, 0, this.BitsPerPixel));
            for (int i = 0; i < nrOfCharacters; i++)
            {
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(characterData[i], characterWidths[i], this.m_FontHeight,0, this.BitsPerPixel, true);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, characterWidths[i], this.m_FontHeight, characterYOffsets[i], this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {            
            Int32 startSymbol = 0x20;
            Int32 nrOfSymbols = Math.Max(0, m_ImageDataList.Count - startSymbol);
            Byte[] characterWidths = new Byte[nrOfSymbols];
            Byte[] characterYOffsets = new Byte[nrOfSymbols];
            Byte[][] imageData = new Byte[nrOfSymbols][];
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i+startSymbol];
                imageData[i] = ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, this.m_FontHeight, this.BitsPerPixel, true);
                if (imageData[i].Length == 0)
                    imageData[i] = new Byte[this.m_FontHeight];
                characterWidths[i] = (Byte)ffs.Width;
                characterYOffsets[i] = (Byte)ffs.YOffset;
            }
            Byte[] fullData = new Byte[0x08 + nrOfSymbols * (2 + m_FontHeight)];
            Array.Copy(Encoding.ASCII.GetBytes("CFNT"), 0, fullData, 0, 4);
            fullData[0x04] = (Byte)this.m_FontWidth;
            fullData[0x05] = (Byte)this.m_FontHeight;
            fullData[0x06] = (Byte)startSymbol;
            fullData[0x07] = (Byte)nrOfSymbols;
            Array.Copy(characterWidths, 0, fullData, 8, nrOfSymbols);
            Array.Copy(characterYOffsets, 0, fullData, 8 + nrOfSymbols, nrOfSymbols);
            Int32 fontDataOffset = 8 + nrOfSymbols * 2;
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                Array.Copy(imageData[i], 0, fullData, fontDataOffset, this.m_FontHeight);
                fontDataOffset += this.m_FontHeight;
            }
            return fullData;
        }
    }
}