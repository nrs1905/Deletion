using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
// Author: Nathaniel Shah

namespace Deletion
{ 
    class Program
    {
        //help is the help message to print whenever there is an invalid argument
        static string help = "Usage: Deletion [-e][-s] [-r?] [-t?] < path > < Extension/Name >" + Environment.NewLine +
            "Delete FILES with the speicfied extension, or containingg the given substring." + Environment.NewLine +
            "You MUST specify one of the parameters, -e or -s" + Environment.NewLine +
            "-e\tLook for extension to remove" + Environment.NewLine +
            "-s\tRun in parallel mode (uses all available processors)" + Environment.NewLine +
            "-r\t(Optional) Run the program recursively to include all subdirectories" + Environment.NewLine +
            "-t\t(Optional) Run the program in test mode to see what files are flagged by the program for deletion" + Environment.NewLine + 
            "Reminder: Extension names require a '.' before hand. ex. '.png'" + Environment.NewLine;
        static int fcount = 0;
        static bool recursive = false;
        static bool ext = false;
        static bool test = false;
        static void Main(string[] args)
        {
            bool exists; //whether the provided directory exists
            var use = ""; //instantializes use flag
            var path = ""; //instantializes path 
            string sub;
            try
            {
                var len = args.Length;
                use = args[0];
                string rec;
                string tes;
                string unknown;
                if (len == 5)
                {
                    rec = args[1];
                    tes = args[2];
                    path = args[3];
                    sub = args[4];
                    if (rec != "-r" | tes != "-t")
                    {
                        printHelp();
                        return;
                    }
                    recursive = true;
                    test = true;
                }
                else if (len == 4)
                {
                    unknown = args[1];
                    path = args[2];
                    sub = args[3];
                    if (unknown == "-r")
                        recursive = true;
                    else if (unknown == "-t")
                        test = true;
                    else
                    {
                        printHelp();
                        return;
                    }
                }
                else
                {
                    path = args[1];
                    sub = args[2];
                } 
                exists = Directory.Exists(path); //Check if the directory exists
            }
            //If there is any issue with accessing the path, print the help message then exit gracefully
            catch (Exception)
            {
                printHelp();
                return;
            }
            //If the provided flag is not recognized, or the path does not exist, print the help message then exit gracefully
            if (use == "-e") {
                ext = true;
            }
            else if (use != "-s")
            {
                printHelp();
                return;
            }
            if (!exists)
            {
                Console.WriteLine("The path '"+ path +"' is invalid" + Environment.NewLine);
                printHelp();
                return;
            }
            Console.WriteLine("Directory '" + path + "':" + Environment.NewLine);
            searchStart(path, sub);
        }
        /*
         * printHelp
         * Prints the help message to the console
         */
        static private void printHelp()
        {
            Console.WriteLine(help);
        }
        /*
         * searchStart
         * Calls the search method, then prints out the number of files deleted
         * Args:    path    the path of the directory to search
         * Args:    sub     the extension type/substring to search for
         */
        
        static private void searchStart(string path, string sub)
        {
            search(path, sub);
            if (test)
                Console.WriteLine("No files were deleted. You are in test mode");
            else
                Console.WriteLine("done. " + fcount + " files deleted");
        }
        /*
         * search
         * A parallel thread based method that recursively searches through every directory in a parent directory (if enabled) then checks its name or extension. 
         * If it matches the provided sub, then it will delete that file, and increment fcount, a count of all files deleted
         */
        static private void search(string path, string sub)
        {
            if (recursive)
            {
                string[] folders = Directory.GetDirectories(path);
                // Recursively go thorugh all of the subdirectories
                Parallel.ForEach(folders, folder =>
                {
                    try
                    {
                        search(folder, sub);
                    }
                    catch
                    {
                        Console.WriteLine("Folder " + folder + " could not be loaded. Skipped from search" + Environment.NewLine);
                    }
                }
                );
            }
            string[] files = Directory.GetFiles(path);
            // Parallel search of all files, check if each file is an image or not, then add its size to the respective count
            Parallel.ForEach(files, file =>
            {
                try
                {
                    var extension = Path.GetExtension(file);
                    {
                        if (ext) {
                            if (extension == sub)
                            {
                                // Atomic increment
                                Interlocked.Increment(ref fcount);
                                if (!test)
                                    File.Delete(file);
                                Console.WriteLine(file);
                            }
                        }
                        else if (Path.GetFileName(file).Contains(sub))
                        {
                            Interlocked.Increment(ref fcount);
                            if (!test)
                                File.Delete(file);
                            Console.WriteLine(file);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("File " + file + " could not be deleted. Skipped." + Environment.NewLine);
                }
            }
            );
        }
    }
}