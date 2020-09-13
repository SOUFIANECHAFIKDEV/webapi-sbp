using System;
using System.Collections.Generic;
using System.IO;

namespace InovaFileManager
{
    public class FileManager
    {
        private string path;
        private int START_DIRECTORY = 0;
        private int START_SUB_DIRECTORY = 2;
        private int DIRECTORY_NAME_LENGTH = 2;


        public FileManager()
        {
            path = InovaFileManagerHelper.Path;
        }

        public bool Save(string base64, string FileName)
        {
            try
            {
                // Encoding to hexadecimal
                var fileNameInHex = EncodingWordToHex(FileName);

                // Get Name of Directory and SubDirectory
                var Directory = GetDirectory(fileNameInHex, START_DIRECTORY, DIRECTORY_NAME_LENGTH);
                var SubDirectory = GetDirectory(fileNameInHex, START_SUB_DIRECTORY, DIRECTORY_NAME_LENGTH);

                // Create directory
                CreateDirectory(BuildPath(new string[] { path, Directory }), BuildPath(new string[] { path, Directory, SubDirectory }));

                // Save File
                File.WriteAllText(BuildPath(new string[] { path, Directory, SubDirectory, FileName }), base64);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Get(string FileName)
        {
            try
            {
                // Encoding to hexadecimal
                var fileNameInHex = EncodingWordToHex(FileName);

                // Get Name of Directory and SubDirectory
                var Directory = GetDirectory(fileNameInHex, START_DIRECTORY, DIRECTORY_NAME_LENGTH);
                var SubDirectory = GetDirectory(fileNameInHex, START_SUB_DIRECTORY, DIRECTORY_NAME_LENGTH);

                // Path File
                var Path_File = BuildPath(new string[] { path, Directory, SubDirectory, FileName });

                // Convert file to Base64
                string Base64 = File.ReadAllText(Path_File);
                return Base64;
            }
            catch (DirectoryNotFoundException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
        }

        public bool Delete(string FileName)
        {
            try
            {
                // Encoding to hexadecimal
                var fileNameInHex = EncodingWordToHex(FileName);

                // Get Name of Directory and SubDirectory
                var Directory = GetDirectory(fileNameInHex, START_DIRECTORY, DIRECTORY_NAME_LENGTH);
                var SubDirectory = GetDirectory(fileNameInHex, START_SUB_DIRECTORY, DIRECTORY_NAME_LENGTH);

                // Path File
                var Path_File = BuildPath(new string[] { path, Directory, SubDirectory, FileName });

                if (CheckExistFile(Path_File))
                {
                    File.Delete(Path_File);
                    return true;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //DeleteFiles([FromRoute] List<string> filesnames

        public bool DeleteFiles(List<string> filesnames)
        {
            try 
            {
                foreach (var FileName in filesnames)
                {
                    // Encoding to hexadecimal
                    var fileNameInHex = EncodingWordToHex(FileName);

                    // Get Name of Directory and SubDirectory
                    var Directory = GetDirectory(fileNameInHex, START_DIRECTORY, DIRECTORY_NAME_LENGTH);
                    var SubDirectory = GetDirectory(fileNameInHex, START_SUB_DIRECTORY, DIRECTORY_NAME_LENGTH);

                    // Path File
                    var Path_File = BuildPath(new string[] { path, Directory, SubDirectory, FileName });

                    if (CheckExistFile(Path_File))
                    {
                        File.Delete(Path_File);
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CreateDirectory(string directory, string subDirectory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!Directory.Exists(subDirectory))
            {
                Directory.CreateDirectory(subDirectory);
            }
        }

        public string EncodingWordToHex(string FileName)
        {
            FletcherChecksum fletcher = new FletcherChecksum();
            var Encoding = fletcher.GetChecksum(FileName, 16);
            var EncodingHexa = Encoding.ToString("X").ToString();
            return EncodingHexa;
        }

        public string GetDirectory(string HexFileName, int IndexStart, int Length)
        {
            return HexFileName.Substring(IndexStart, Length);
        }

        public string BuildPath(string[] NameDirectory)
        {
            return string.Join("/", NameDirectory);
        }

        public bool CheckExistFile(string NameDirectory)
        {
            return File.Exists(NameDirectory);
        }

    }
}
