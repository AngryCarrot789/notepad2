using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.RecyclingBin
{
    /// <summary>
    /// Possible flags for the SHFileOperation method.
    /// </summary>
    [Flags]
    public enum FileOperationFlags : ushort
    {
        /// <summary>
        /// Do not show a dialog during the process
        /// </summary>
        FOF_SILENT = 0x0004,
        /// <summary>
        /// Do not ask the user to confirm selection
        /// </summary>
        FOF_NOCONFIRMATION = 0x0010,
        /// <summary>
        /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
        /// </summary>
        FOF_ALLOWUNDO = 0x0040,
        /// <summary>
        /// Do not show the names of the files or folders that are being recycled.
        /// </summary>
        FOF_SIMPLEPROGRESS = 0x0100,
        /// <summary>
        /// Surpress errors, if any occur during the process.
        /// </summary>
        FOF_NOERRORUI = 0x0400,
        /// <summary>
        /// Warn if files are too big to fit in the recycle bin and will need
        /// to be deleted completely.
        /// </summary>
        FOF_WANTNUKEWARNING = 0x4000,
    }
}
