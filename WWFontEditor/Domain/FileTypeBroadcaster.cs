using System;

namespace Nyerguds.Util
{
    public interface FileTypeBroadcaster
    {
        /// <summary>Very short code name for this type.</summary>
        String ShortTypeName { get; }
        /// <summary>Brief name and description of the overall file type, for the types dropdown in the open file dialog.</summary>
        String ShortTypeDescription { get; }
        /// <summary>Possible file extensions for this file type.</summary>
        String[] FileExtensions { get; }
        /// <summary>Brief name and description of the specific types for all extensions, for the types dropdown in the save file dialog.</summary>
        String[] DescriptionsForExtensions { get; }
        /// <summary>File extension set for this specific file. To be used to identify the item chosen in a Save dialog.</summary>
        String FileExtension { get; set; }
        /// <summary>Supported types can always be loaded, but this indicates if save functionality to this type is also available.</summary>
        Boolean CanSave { get; }
    }

    /// <summary>File Load exceptions. These are typically ignored in favour of checking the next type to try.</summary>
    [Serializable]
    public class FileTypeLoadException : Exception
    {
        /// <summary>USed to store the attempted load type in the Data dictionary to allow serialization.</summary>
        protected const String DATA_AttemptedLoadedTypeString = "AttemptedLoadedType";
        
        public String AttemptedLoadedType
        {
            get { return (String)this.Data[DATA_AttemptedLoadedTypeString]; }
            set { this.Data[DATA_AttemptedLoadedTypeString] = value; }
        }

        public FileTypeLoadException() { }
        public FileTypeLoadException(String message) : base(message) { }
        public FileTypeLoadException(String message, Exception innerException) : base(message, innerException) { }
        public FileTypeLoadException(String message, String attemptedLoadedType)
            : base(message)
        {
            this.AttemptedLoadedType = attemptedLoadedType;
        }
        public FileTypeLoadException(String message, String attemptedLoadedType, Exception innerException)
            : base(message, innerException)
        {
            this.AttemptedLoadedType = attemptedLoadedType;
        }
    }
}
