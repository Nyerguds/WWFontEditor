using Nyerguds.Util;
using System;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Raw 8x15 font.
    /// THIS IS A QUICK IMPLEMENTATION TO OPEN THE MKSFONT.BIN FILE FROM THE FLOPPY VERSION OF MORTAL KOMBAT 1!
    /// DO NOT ENABLE THIS IN RELEASE VERSIONS, SINCE IT HAS NO FAIL CONDITIONS!
    /// </summary>
    public class FontFileMK : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMin { get { return 8; } }
        public override Int32 FontWidthTypeMax { get { return 8; } }
        public override Int32 FontHeightTypeMin { get { return 15; } }
        public override Int32 FontHeightTypeMax { get { return 15; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "bin" }; } }
        public override String ShortTypeName { get { return "MKFont"; } }
        public override String ShortTypeDescription { get { return "MKFont"; } }
        public override String LongTypeDescription { get { return "A simple 8 BPP font without header data."; } }
        public override String[] GamesListForType { get { return new String[] { "Mortal Kombat 1" }; } }
        public override Boolean CanSave { get { return false; } }

        public override void LoadFont(Byte[] fileData)
        {
            m_FontWidth = 8;
            m_FontHeight = 15;
            Int32 dataSize = this.m_FontWidth * this.m_FontHeight;
            for (Int32 i = 0; i < 0x20; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[m_FontHeight * m_FontWidth], this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel));
            for (Int32 i = 0; i * dataSize < fileData.Length; i++)
            {
                Byte[] curData8bit = new Byte[dataSize];
                try
                {
                    Array.Copy(fileData, i * dataSize, curData8bit, 0, dataSize);
                }
                catch (IndexOutOfRangeException)
                {
                    return;
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont()
        {
            throw new NotSupportedException();
        }
    }
}