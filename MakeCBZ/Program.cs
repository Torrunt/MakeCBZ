using System;
using Ionic.Zip;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MakeCBZ
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ZipFile zip = new ZipFile();
            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
            zip.CompressionMethod = CompressionMethod.None;

            string output = "";
            int s;
            bool convertingZips = false;

            for (int i = 0; i < args.Length; i++)
            {
                output = args[i].Substring(0, args[i].LastIndexOf('\\'));

                s = args[i].LastIndexOf('\\') + 1;
                string name = args[i].Substring(s, args[i].Length - s);
                Console.WriteLine(name);

                if (args[i].Contains(".cbz") || args[i].Contains(".zip") || args[i].Contains(".cbr") || args[i].Contains(".rar"))
                {
                    // Convert zip
                    convertingZips = true;

                    Console.WriteLine("\nreading...\n");

                    zip = new ZipFile();
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
                    zip.CompressionMethod = CompressionMethod.None;

                    string tempFolder = output + "\\temp";
                    Directory.CreateDirectory(tempFolder);
                    List<string> directories = new List<string>();

                    var archive = SharpCompress.Archive.ArchiveFactory.Open(args[i]);
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            string dir = "";
                            if (entry.FilePath.IndexOf("/") != -1)
                            {
                                dir = entry.FilePath.Substring(0, entry.FilePath.IndexOf("/"));
                                if (!directories.Contains(dir))
                                {
                                    Console.WriteLine(dir);
                                    directories.Add(dir);
                                    Directory.CreateDirectory(tempFolder + "\\" + dir);
                                    zip.AddDirectory(tempFolder + "\\" + dir, dir);
                                }
                            }
                            Console.WriteLine(entry.FilePath);

                            string filePath = tempFolder + "\\" + entry.FilePath.Substring(entry.FilePath.IndexOf("\\") + 1, entry.FilePath.Length - (entry.FilePath.IndexOf("\\") + 1));
                            FileStream file = File.Create(filePath);
                            entry.WriteTo(file);
                            file.Flush();
                            file.Dispose();

                            zip.AddFile(filePath, dir);
                        }
                    }
                    archive.Dispose();

                    Console.WriteLine("\nsaving...");

                    output = args[i].Replace(".cbr", ".cbz").Replace(".rar", "cbz").Replace(".zip", ".cbz").Replace(".rar", ".cbz");

                    zip.Save(output);

                    Console.WriteLine("\nsaved " + output);

                    Console.WriteLine("\ndeleting temp folder...");
                    Directory.Delete(tempFolder, true);
                    zip.Dispose();
                    zip = null;

                    Console.WriteLine("\ncompleted " + name + "\n\n");
                }
                else if (!convertingZips)
                {
                    // Add files/folders to cbz
                    Console.WriteLine(args[i]);

                    if (args[i].Contains(".jpg") || args[i].Contains(".png") || args[i].Contains(".jpeg"))
                        zip.AddFile(args[i], ""); // file
                    else
                        zip.AddDirectory(args[i], name.ToLower().IndexOf("volume") == -1 ? name : ""); // folder
                }
            }

            if (convertingZips)
                return;

            // Make CBZ from files/folders
            Console.WriteLine();

            if (output.ToLower().IndexOf("volume") != -1 || args.Length == 1)
            {
                // files are already in a volume folder, don't ask for vol number
                if (args.Length == 1)
                    output = args[0];

                s = output.LastIndexOf('\\') + 1;
                string vol = output.Substring(s, output.Length - s);
                output = output.Substring(0, s - 1);

                string series = output.Substring(0, s - 1);
                s = series.LastIndexOf('\\') + 1;
                series = series.Substring(s, series.Length - s);

                Console.WriteLine("Series:" + series);
                Console.WriteLine("Volume:" + vol);

                output += "\\" + series + " - " + vol + ".cbz";
            }
            else
            {
                // ask for volume number (if nothing, will just use series/folder name)
                Console.Write("Volume: ");
                string input = Console.ReadLine();

                string series = output.Substring(output.LastIndexOf('\\') + 1, output.Length - (output.LastIndexOf('\\') + 1));

                if (input.Trim() == "")
                    output += "\\" + series + ".cbz";
                else
                    output += "\\" + series + " - Volume " + input + ".cbz";
            }

            Console.WriteLine("\nsaving...");

            zip.Save(output);

            Console.WriteLine("\n" + output + " saved");

            // cut straight to clipboard
            DataObject data = new DataObject();
            data.SetFileDropList(new System.Collections.Specialized.StringCollection() { output });
            data.SetData("Preferred DropEffect", DragDropEffects.Move);
            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);

            Console.WriteLine("\nCut to clipboard");
        }
    }
}
