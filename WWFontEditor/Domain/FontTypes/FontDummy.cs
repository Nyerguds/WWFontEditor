using System;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    class FontDummy : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x00; } }
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        /// <summary>The first symbol that is saved. This hides all symbols before this index from the editor.</summary>
        public override Int32 SymbolsTypeFirst { get { return 0x00; } }
        public override Int32 FontWidthTypeMin { get { return 0x01; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMin { get { return 0x01; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Byte TransparencyColor { get { return 0x00; } }
        /// <summary>Padding at the bottom of the font. Only used for the preview function.</summary>
        public override Int32 FontTypePaddingVertical { get { return 0; } }
        /// <summary>Padding between the characters of the font.</summary>
        public override Int32 FontTypePaddingHorizontal { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 2; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[0]; } }
        public override Boolean CustomSymbolWidthsForType { get { return true; } }
        public override Boolean CustomSymbolHeightsForType { get { return true; } }
        public override String ShortTypeName { get { return "DummyFont"; } }
        public override String ShortTypeDescription { get { return "Dummy font for creating new fonts"; } }
        public override String LongTypeDescription { get { return "This is not a real font type. It is a basic initialisation designed to be as universal as possible for converting to other types."; } }
        public override String[] GamesListForType { get { return new String[0]; } }
        /// <summary>Supported types can always be loaded, but this indicates if save functionality to this type is also available.</summary>
        public override Boolean CanSave { get { return false; } }

        public FontDummy()
        {
            this.m_FontWidth = 8;
            this.m_FontHeight = 8;
            // Byte data inside a FontFileSymbol gets copied, so we only need to define this array once.
            Byte[] byteData = new Byte[this.m_FontWidth * this.m_FontHeight];
            for (Int32 i = 0; i < 0x80; ++i)
                this.m_ImageDataList.Add(new FontFileSymbol(byteData, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor));
        }

        public override void LoadFont(Byte[] fileData)
        {
            throw new NotSupportedException();
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            throw new NotSupportedException();
        }
    }
}
