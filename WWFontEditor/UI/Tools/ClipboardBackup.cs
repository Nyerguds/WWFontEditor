using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Nyerguds.Util.UI
{
    public class ClipboardBackup: IDisposable
    {
        private readonly Dictionary<String, Object> _clipboard = new Dictionary<String, Object>();
        private readonly Dictionary<String, Boolean> _clipboardIsStream = new Dictionary<String, Boolean>();

        public ClipboardBackup() { }

        public ClipboardBackup(Boolean performBackup)
        {
            if (performBackup)
                this.BackupClipboard();
        }

        public void BackupClipboard()
        {
            this.ClearBackup();
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();
            if (retrievedData == null)
                return;
            String[] formats = retrievedData.GetFormats();
            if (formats.Length <= 0)
                return;
            foreach (String format in formats)
            {
                if (!retrievedData.GetDataPresent(format))
                    continue;
                Object clipObj = retrievedData.GetData(format);
                if (clipObj == null)
                    continue; 
                MemoryStream ms = clipObj as MemoryStream;
                Boolean isStream = ms != null;
                this._clipboard.Add(format, isStream ? ms.ToArray() : clipObj);
                this._clipboardIsStream.Add(format, isStream);
            }
        }

        public void RestoreClipboard()
        {
            if (this._clipboard.Count == 0)
                return;
            List<MemoryStream> streams = new List<MemoryStream>();
            try
            {
                DataObject data = new DataObject();
                foreach (String key in this._clipboard.Keys)
                {
                    if (this._clipboardIsStream.ContainsKey(key) && this._clipboardIsStream[key])
                    {
                        MemoryStream byteData = new MemoryStream((Byte[]) this._clipboard[key]);
                        streams.Add(byteData);
                        data.SetData(key, false, byteData);
                    }
                    else
                    {
                        data.SetData(key, true, this._clipboard[key]);
                    }
                }
                Clipboard.SetDataObject(data, true);
            }
            finally
            {
                foreach (MemoryStream stream in streams)
                {
                    try { stream.Dispose(); }
                    catch
                    {
                        // Will probably never happen anyway since they're just memory streams.
                    }
                }
            }
        }

        public void ClearBackup()
        {
            // Clean up any image objects and other disposables retrieved from the clipboard.
            Object[] values = _clipboard.Values.ToArray();
            this._clipboard.Clear();
            foreach (Object value in values)
            {
                IDisposable disposable = value as IDisposable;
                if (disposable == null)
                    continue;
                try
                {
                    disposable.Dispose();
                }
                catch
                {
                    // Ignore. Can't help it if something is wrong here.
                }
            }
            this._clipboardIsStream.Clear();
        }

        public void Dispose()
        {
            this.ClearBackup();
        }

    }
}