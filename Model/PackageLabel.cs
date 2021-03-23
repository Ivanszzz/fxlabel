using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHLabelPrint
{
	/// <summary>
	/// 单个标签打印的参数信息
	/// </summary>
	public class PackageLabel
	{
		private Dictionary<string, string> paramValues = new Dictionary<string, string>();

		/// <summary>
		/// 用于模板烧录的标签参数和值；
		/// 需要设计模板的人提供参数格式；
		/// </summary>
		public Dictionary<string, string> ParamValues { get => paramValues; set => paramValues = value; }
	}
}
