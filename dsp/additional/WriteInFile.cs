using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsp.additional
{
    public class Writer
    {
        private string str;

        public void WriteInFile()
        {
            Delete();
            string path = "FuncLog.txt";
            if (!File.Exists(path))
            {
                ////create a file to write to
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(str);
                }
            }
            else
            {
                // This text is always added, making the file longer over time
                // if it is not deleted.
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(str);
                }

            }
        }
        public void Write(string str)
        {
            this.str += str;
        }
        private static void Delete()
        {
            string path = "FuncLog.txt";
            try
            {
                // Check if file exists with its full path    
                if (File.Exists(path))
                {
                    // If file found, delete it    
                    File.Delete(path);
                    Console.WriteLine("File deleted.");
                }
                else Console.WriteLine("File not found");
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }

    }
}
