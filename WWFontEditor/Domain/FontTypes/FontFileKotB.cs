using System;
using System.Linq;
using System.Text;
using Nyerguds.Util;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileKotB : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return NrOfSymbols; } }
        public override Int32 SymbolsTypeMax { get { return NrOfSymbols; } }
        public override Int32 SymbolsTypeFirst { get { return 0x00; } }
        public override Int32 FontHeightTypeMax { get { return 0x6; } }
        public override Int32 FontHeightTypeMin { get { return 0x6; } }
        public override Int32 FontWidthTypeMin { get { return 0x08; } }
        public override Int32 FontWidthTypeMax { get { return 0x08; } }
        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font.</summary>
        public override Boolean CustomSymbolWidthsForType { get { return true; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font.</summary>
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fnt" }; } }
        public override String ShortTypeName { get { return "KotBFont"; } }
        public override String ShortTypeDescription { get { return "Kings of the Beach Font"; } }
        public override String LongTypeDescription { get { return "An 8x6 pixel 1-bit font that supports custom character widths, but lacks a real header."; } }
        public override String[] GamesListForType { get { return new String[] { "Kings of the Beach" }; } }

        protected const Int32 NrOfSymbols = 0x5B;

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length != NrOfSymbols * 7)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            this.m_FontWidth = 8;
            this.m_FontHeight = 6;

            Byte[] characterWidths = new Byte[NrOfSymbols];
            Array.Copy(fileData, 0, characterWidths, 0, NrOfSymbols);
            if (characterWidths.Any(w => w > 8))
                throw new FileTypeLoadException("Character widths data exceeds maximum width.");
            Byte[][] characterData = new Byte[NrOfSymbols][];
            for (Int32 i = 0; i < NrOfSymbols; i++)
            {
                Byte[] charData = new Byte[this.m_FontHeight];
                Array.Copy(fileData, NrOfSymbols + i * this.m_FontHeight, charData, 0, this.m_FontHeight);
                characterData[i] = charData;
            }
            for (Int32 i = 0; i < NrOfSymbols; i++)
            {
                Int32 charWidth = characterWidths[i];
                Byte[] curData8bit = ImageUtils.ConvertTo8Bit(characterData[i], charWidth, this.m_FontHeight, 0, this.BitsPerPixel, true);
                FontFileSymbol fc = new FontFileSymbol(curData8bit, charWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Byte[] characterWidths = new Byte[NrOfSymbols];
            Byte[][] imageData = new Byte[NrOfSymbols][];
            for (Int32 i = 0; i < NrOfSymbols; i++)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i];
                imageData[i] = ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, this.m_FontHeight, this.BitsPerPixel, true);
                if (imageData[i].Length == 0)
                    imageData[i] = new Byte[this.m_FontHeight];
                characterWidths[i] = (Byte)ffs.Width;
            }
            Byte[] fullData = new Byte[NrOfSymbols * (1 + this.m_FontHeight)];
            Int32 fontDataOffset = 0;
            Array.Copy(characterWidths, 0, fullData, fontDataOffset, NrOfSymbols);
            fontDataOffset += NrOfSymbols;
            for (Int32 i = 0; i < NrOfSymbols; i++)
            {
                Array.Copy(imageData[i], 0, fullData, fontDataOffset, this.m_FontHeight);
                fontDataOffset += this.m_FontHeight;
            }
            return fullData;
        }
    }
}