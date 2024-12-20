﻿using DokanNet;
using System.Security.AccessControl;

namespace FSDokan
{
    public class SimpleFS : IDokanOperations
    {
        private readonly Dictionary<string, File> inputFiles = new Dictionary<string, File>();
        private readonly Dictionary<string, File> outputFiles = new Dictionary<string, File>();

        private readonly static int CAPACITY = 128 * 1024 * 1024; // 128 MB
        private readonly int totalNumberOfBytes = CAPACITY;
        private int totalNumberOfFreeBytes = CAPACITY;

        public void Cleanup(string fileName, IDokanFileInfo info)
        {
            if (info.DeleteOnClose)
            {
                if (fileName.StartsWith(Path.DirectorySeparatorChar + "input" + Path.DirectorySeparatorChar) && inputFiles.ContainsKey(fileName))
                {
                    totalNumberOfFreeBytes += inputFiles[fileName].Data.Length;
                    inputFiles.Remove(fileName);
                }
                else if (fileName.StartsWith(Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar) && outputFiles.ContainsKey(fileName))
                {
                    totalNumberOfFreeBytes += outputFiles[fileName].Data.Length;
                    outputFiles.Remove(fileName);
                }
            }
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {

        }

        public NtStatus CreateFile(string fileName, DokanNet.FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, IDokanFileInfo info)
        {
            if (mode == FileMode.CreateNew)
            {
                if (fileName.StartsWith(Path.DirectorySeparatorChar + "input" + Path.DirectorySeparatorChar) && !inputFiles.ContainsKey(fileName))
                {
                    inputFiles.Add(fileName, new File(fileName, DateTime.Now));
                }
                else if (fileName.StartsWith(Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar) && !outputFiles.ContainsKey(fileName))
                {
                    outputFiles.Add(fileName, new File(fileName,DateTime.Now));
                }
            }
            return NtStatus.Success;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            if (inputFiles.ContainsKey(fileName) || outputFiles.ContainsKey(fileName))
            {
                info.DeleteOnClose = true;
                return NtStatus.Success;
            }
            else
            {
                return NtStatus.Error;
            }
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            if (inputFiles.ContainsKey(fileName) || outputFiles.ContainsKey(fileName))
            {
                info.DeleteOnClose = true;
                return NtStatus.Success;
            }else
            {
                return NtStatus.ObjectNameNotFound;
            }
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            throw new NotImplementedException();
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new List<FileInformation>();
            if (fileName == Path.DirectorySeparatorChar.ToString())
            {
                files.Add(new FileInformation()
                {
                    FileName = "input",
                    Attributes = FileAttributes.Directory
                });
                files.Add(new FileInformation()
                {
                    FileName = "output",
                    Attributes = FileAttributes.Directory
                });
            } 
            else if (fileName == (Path.DirectorySeparatorChar + "input"))
            {
                foreach(var file  in inputFiles.Values)
                {
                    files.Add(new FileInformation()
                    {
                        FileName = Path.GetFileName(file.Name),
                        Length = file.Data.Length,
                        Attributes = FileAttributes.Normal,
                        CreationTime = file.Created
                    });
                }
            }
            else if (fileName == (Path.DirectorySeparatorChar + "output"))
            {
                foreach (var file in outputFiles.Values)
                {
                    files.Add(new FileInformation()
                    {
                        FileName = Path.GetFileName(file.Name),
                        Length = file.Data.Length,
                        Attributes = FileAttributes.Normal,
                        CreationTime = file.Created
                    });
                }
            }
            return NtStatus.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            throw new NotImplementedException();
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            throw new NotImplementedException();
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            totalNumberOfFreeBytes = this.totalNumberOfFreeBytes;
            totalNumberOfBytes = this.totalNumberOfBytes;
            freeBytesAvailable = this.totalNumberOfFreeBytes;
            return NtStatus.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, IDokanFileInfo info)
        {
            if (fileName == Path.DirectorySeparatorChar.ToString())
            {
                fileInfo = new()
                {
                    FileName = fileName,
                    Attributes = FileAttributes.Directory
                };
            }else if (fileName == (Path.DirectorySeparatorChar + "input"))
            {
                fileInfo = new()
                {
                    FileName = "input",
                    Attributes = FileAttributes.Directory
                };
            }else if (fileName == (Path.DirectorySeparatorChar + "output"))
            {
                fileInfo = new()
                {
                    FileName = "output",
                    Attributes = FileAttributes.Directory
                };
            }
            else if (fileName.StartsWith(Path.DirectorySeparatorChar + "input") && inputFiles.ContainsKey(fileName))
            {
                fileInfo = new()
                {
                    FileName = fileName,
                    Length = inputFiles[fileName].Data.Length,
                    Attributes = FileAttributes.Normal,
                    CreationTime = inputFiles[fileName].Created
                };
            }
            else if (fileName.StartsWith(Path.DirectorySeparatorChar + "output") && outputFiles.ContainsKey(fileName))
            {
                fileInfo = new()
                {
                    FileName = fileName,
                    Length = outputFiles[fileName].Data.Length,
                    Attributes = FileAttributes.Normal,
                    CreationTime = outputFiles[fileName].Created
                };
            }
            else
            {
                fileInfo = default;
                return NtStatus.Error;
            }
            return NtStatus.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            if (info.IsDirectory)
            {
                DirectorySecurity directorySecurity = new();
                directorySecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        Environment.UserName,
                        FileSystemRights.FullControl,
                        AccessControlType.Allow));
                security = directorySecurity;
            }
            else
            {
                FileSecurity fileSecurity = new();
                fileSecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        Environment.UserName,
                        FileSystemRights.FullControl,
                        AccessControlType.Allow));
                security = fileSecurity;
            }
            return DokanResult.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "AleksandarFS";
            features = FileSystemFeatures.None;
            fileSystemName = "UserFS";
            maximumComponentLength = 256;
            return NtStatus.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            throw new NotImplementedException();
        }

        public NtStatus Mounted(IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            throw new NotImplementedException();
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            File? file = null;
            if (fileName.StartsWith(Path.DirectorySeparatorChar + "input"))
                file = inputFiles[fileName];
            else if (fileName.StartsWith(Path.DirectorySeparatorChar + "output"))
                file = outputFiles[fileName];
            file.Data.Skip((int)offset).Take(buffer.Length).ToArray().CopyTo(buffer, 0);
            int diff = file.Data.Length - (int)offset;
            bytesRead = buffer.Length > diff ? diff : buffer.Length;
            return NtStatus.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            File? file = null;
            if (fileName.StartsWith(Path.DirectorySeparatorChar + "input"))
            {
                if (!inputFiles.ContainsKey(fileName))
                    inputFiles.Add(fileName, new File(fileName, DateTime.Now));
                file = inputFiles[fileName];
            }
            else if (fileName.StartsWith(Path.DirectorySeparatorChar + "output"))
            {
                if (!outputFiles.ContainsKey(fileName))
                    outputFiles.Add(fileName, new File(fileName, DateTime.Now));
                file = outputFiles[fileName];
            }
            if (info.WriteToEndOfFile)
            {
                file.Data = file.Data.Concat(buffer).ToArray();
                bytesWritten = buffer.Length;
            }
            else
            {
                int difference = file.Data.Length - (int)offset;
                totalNumberOfFreeBytes += difference;
                file.Data = file.Data.Take((int)offset).Concat(buffer).ToArray();
                bytesWritten = buffer.Length;
            }
            totalNumberOfFreeBytes -= bytesWritten;
            return NtStatus.Success;
        }
    }
}
