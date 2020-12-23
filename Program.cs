using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AHLabelPrint
{
	static class Program
	{
		public static string logName = "";
		public static string UserNo = "";
		public static string Cookies = "";
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new lblLogin());
		}
	}
}
