using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AHLabelPrint
{
    public class Logger
    {
        private static string logdir = AppDomain.CurrentDomain.BaseDirectory + "/log/";
        private static string logfilename = logdir + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        private static Logger logger = null;
        private static object _lock = new object();

        private Logger() { }
        public static Logger GetLogger()
        {
            if (logger == null)
            {
                if (!Directory.Exists(logdir))
                    Directory.CreateDirectory(logdir);
                logger = new Logger();
            }
            return logger;
        }

        public static int i = 0;
        public void WriteLog(string content)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    FileStream _filestream;
                    if (!File.Exists(logfilename))
                        _filestream = new FileStream(logfilename, FileMode.Create);
                    else
                        _filestream = new FileStream(logfilename, FileMode.Append, FileAccess.Write);

                    using (_filestream)
                    {
                        using (var sw = new StreamWriter(_filestream, Encoding.Default))
                        {
                            sw.WriteLine("[{0}]:{1}_{2}", DateTime.Now.ToString(), content, i++);
                        }
                    }
                }
            });

        }
    }
}
