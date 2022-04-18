using System;
using System.Collections.Generic;
using System.IO;


namespace DirectoryScans
{
    class FolderFiles
    {
        public List<object> GetFoldersAndFiles(string path)
        {
            EnumerationOptions enumerationOptions = new EnumerationOptions();
            enumerationOptions.IgnoreInaccessible = true;
            enumerationOptions.RecurseSubdirectories = true;
            List<string> allName = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            IEnumerable<FileSystemInfo> findAllFolderAndFile = directoryInfo.EnumerateFileSystemInfos("*", enumerationOptions);
            
            //sort by name
            allName.Add(directoryInfo.FullName);
            foreach (FileSystemInfo file in findAllFolderAndFile)
            {
                allName.Add(file.FullName);
            }
            allName.Sort();

            List<object> foldersAndFiles = new List<object>();
            foreach (string fullname in allName)
            {
                if (Directory.Exists(fullname)) //folder
                {
                    foldersAndFiles.Add(new DirectoryInfo(fullname));
                }

                if(File.Exists(fullname)) //file
                {
                    foldersAndFiles.Add(new FileInfo(fullname));
                }
            }
            return foldersAndFiles;
        }

        public long GetSize(DirectoryInfo folder)
        {
            EnumerationOptions enumerationOptions = new EnumerationOptions();
            enumerationOptions.IgnoreInaccessible = true;
            enumerationOptions.RecurseSubdirectories = true;
            string[] path_folders = Directory.GetFiles(folder.FullName, "*", enumerationOptions);
            long bytes = 0;
            foreach (string filename in path_folders)
            {
                FileInfo file = new FileInfo(filename);
                bytes += file.Length;
            }
            return bytes;
        }

        public long GetSize(FileInfo file)
        {
            return file.Length;
        }

        public int GetLevelPath(string path)
        {
            int first = path.Split('/').Length - 1;
            int second = path.Split('\\').Length - 1;
            return first + second;
        }
    }


    internal class Command
    {
        private string command = "";
        private string path = "";
        private string output_file = "";
        private bool quiet_print = false;
        private bool human_read = false;

        public Command()
        {
            path = Directory.GetCurrentDirectory();
            DateTime date = DateTime.Now;
            string output_name = "sizes-" + date.ToString("yyyy-MM-dd");
            output_file = path + "\\" + output_name + ".txt";
        }

        private string GetTreeByPath(string path)
        {
            FolderFiles folderFiles = new FolderFiles();
            List<object> files_folder = folderFiles.GetFoldersAndFiles(path);
            int dest_level = folderFiles.GetLevelPath(path) - 2;
            string output = "";
            foreach (var folder_file in files_folder)
            {
                string name = "";
                string type_size = "bytes";
                double size = 0;
                int level = 0;
                if (folder_file.GetType() == typeof(DirectoryInfo)) //folder
                {
                    DirectoryInfo dir = (DirectoryInfo)folder_file;
                    name = dir.Name;
                    size = 1000;
                    size = folderFiles.GetSize(dir);
                    level = folderFiles.GetLevelPath(dir.FullName) - dest_level;
                }
                if (folder_file.GetType() == typeof(FileInfo)) //file
                {
                    FileInfo file = (FileInfo)folder_file;
                    name = file.Name;
                    size = folderFiles.GetSize(file);
                    level = folderFiles.GetLevelPath(file.FullName) - dest_level;
                }

                if (human_read)
                {
                    if (size > 1024) //mb
                    {
                        size /= 1024;
                        size = Math.Round(size, 0);
                        type_size = "kb";
                    }
                    if (size > 1024) //gb
                    {
                        size /= 1024;
                        type_size = "mb";
                    }
                    if (size > 1024) //tb
                    {
                        size /= 1024;
                        type_size = "gb";
                    }
                    size = Math.Round(size, 2);
                }
                output += " ".PadLeft(level, '-') + name + " (" + size + " " + type_size + ")\n";
            }
            return output;
        }

        public void Input()
        {
            command = Console.ReadLine();
            if (command == null) { command = ""; }
            command = command.Replace(" ", "");

            if (command == "-o")
            {
                Console.WriteLine("Введите новый путь выходного txt файла: ");
                string new_output = Console.ReadLine();
                if (Directory.Exists(new_output))
                {
                    output_file = new_output+".txt";
                }
                else
                {
                    Console.WriteLine("Не правильный ввод пути");
                }
            }
            else if (command == "-p")
            {
                Console.WriteLine("Введите новый путь для сканирования: ");
                string new_path = Console.ReadLine();
                if (Directory.Exists(new_path))
                {
                    path = new_path;
                }
                else
                {
                    Console.WriteLine("Не правильный ввод пути");
                }
            }
            else if (command == "-q")
            {
                quiet_print = !quiet_print;
            }
            else if (command == "-h")
            {
                human_read = !human_read;
            }
        }


        public void Run()
        {
            if (command == "")
            {
                Console.WriteLine("Идёт сканирование...");
                string output = GetTreeByPath(path);

                if (!quiet_print) //print
                {
                    Console.WriteLine(output);
                }

                Console.WriteLine("Сканирование завершено");
                //Write file
                StreamWriter stream = new StreamWriter(output_file);
                stream.Write(output);
                stream.Close();
            }
        }
    }
}
