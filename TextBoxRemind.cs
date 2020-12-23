using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AHLabelPrint
{
    class TextBoxRemind
    {
        private string[] array = null;
        private static string workPath = Directory.GetCurrentDirectory();
        private static string textPath = null;

        public void InitAutoCompleteCustomSource(TextBox text,String boxname)
        {
            if (false == System.IO.Directory.Exists(workPath + "/TextRemind/"))
            {
                //创建文件夹
                System.IO.Directory.CreateDirectory(workPath + "/TextRemind/");
            }
            textPath = Path.Combine(workPath, "TextRemind/"+ boxname + ".txt");
            array = ReadText();
            if(array!=null && array.Length > 0)
            {
                AutoCompleteStringCollection ACSC = new AutoCompleteStringCollection();
                for(int i =0; i < array.Length; i++)
                {
                    ACSC.Add(array[i]);
                }
                text.AutoCompleteCustomSource = ACSC;
            }
        }

        string[] ReadText()
        {
            try
            {
                if (!File.Exists(textPath))
                {
                    FileStream fileStream = File.Create(textPath);
                    fileStream.Close();
                    fileStream = null;
                }
                return File.ReadAllLines(textPath, Encoding.Default);
            }
            catch
            {
                return null;
            }
        }

        public void Remind(string str,String boxname)
        {
            StreamWriter streamWriter = null;
            if (false == System.IO.Directory.Exists(workPath+ "/TextRemind/"))
            {
                //创建文件夹
                System.IO.Directory.CreateDirectory(workPath + "/TextRemind/");
            }
            textPath = Path.Combine(workPath, "TextRemind/" + boxname + ".txt");
            try
            {
                if(array!=null && !array.Contains(str))
                {
                    streamWriter = new StreamWriter(textPath, true, Encoding.Default);
                    streamWriter.WriteLine(str);
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }
            }
        }
    }
}
