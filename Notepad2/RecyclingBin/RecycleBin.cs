using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.RecyclingBin
{
    public static class RecycleBin
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        /// <summary>
        /// Send file to recycle bin
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        /// <param name="flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
        public static bool Send(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags
                };
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool Send(string path)
        {
            return Send(
                path, 
                FileOperationFlags.FOF_NOCONFIRMATION | 
                FileOperationFlags.FOF_WANTNUKEWARNING);
        }

        /// <summary>
        /// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool SilentSend(string path)
        {
            return Send(
                path, 
                FileOperationFlags.FOF_NOCONFIRMATION | 
                FileOperationFlags.FOF_NOERRORUI | 
                FileOperationFlags.FOF_SILENT);
        }

        private static bool DeleteFile(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = flags
                };
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DeleteCompletelySilent(string path)
        {
            return DeleteFile(
                path,
                FileOperationFlags.FOF_NOCONFIRMATION | 
                FileOperationFlags.FOF_NOERRORUI |
                FileOperationFlags.FOF_SILENT);
        }
    }
}
