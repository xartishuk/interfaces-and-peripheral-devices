//
// FileItem.cs
//
// by Eric Haddan
//
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using IMAPI2.Interop;
using System.ComponentModel;

namespace IMAPI2.MediaItem
{
    /// <summary>
    /// 
    /// </summary>
    class FileItem : IMediaItem
    {        

        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
           [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
           out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);

        private Int64 m_fileLength = 0;

        public FileItem(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The file added to FileItem was not found!", path);
            }

            filePath = path;

            FileInfo fileInfo = new FileInfo(filePath);
            displayName = fileInfo.Name;
            m_fileLength = fileInfo.Length;

            //
            // Get the File icon
            //
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImg = Win32.SHGetFileInfo(filePath, 0, ref shinfo,
                (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);

            if (shinfo.hIcon != null)
            {
                //The icon is returned in the hIcon member of the shinfo struct
                System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                try
                {
                    fileIconImage = (System.Drawing.Image)
                        imageConverter.ConvertTo(icon, typeof(System.Drawing.Image));
                }
                catch (NotSupportedException)
                {
                }

                Win32.DestroyIcon(shinfo.hIcon);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 SizeOnDisc
        {
            get
            {
                if (m_fileLength > 0)
                {
                    FileInfo info = new FileInfo(Path);
                    uint dummy, sectorsPerCluster, bytesPerSector;
                    int result = GetDiskFreeSpaceW(info.Directory.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
                    if (result == 0)
                    {
                        throw new Win32Exception();
                    }
                    uint clusterSize = sectorsPerCluster * bytesPerSector;
                    return ((m_fileLength + clusterSize - 1) / clusterSize) * clusterSize;
                }

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get
            {
                return filePath;
            }
        }
        private string filePath;

        /// <summary>
        /// 
        /// </summary>
        public System.Drawing.Image FileIconImage
        {
            get
            {
                return fileIconImage;
            }
        }
        private System.Drawing.Image fileIconImage = null;


        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            return displayName;
        }
        private string displayName;

        public bool AddToFileSystem(IFsiDirectoryItem rootItem)
        {
            IStream stream = null;

            try
            {
                Win32.SHCreateStreamOnFile(filePath, Win32.STGM_READ | Win32.STGM_SHARE_DENY_WRITE, ref stream);

                if (stream != null)
                {
                    rootItem.AddFile(displayName, stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error adding file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (stream != null)
                {
                    Marshal.FinalReleaseComObject(stream);
                }
            }

            return false;
        }
    }
}
