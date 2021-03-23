using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHLabelPrint
{
	/// <summary>
	/// 标签打印返回消息
	/// </summary>
	public class PrintMessage
	{
		private List<string> imagesPathList = new List<string>();
		private List<PackageLabel> templateColList = new List<PackageLabel>();
		public double CostTimes { get; set; }
		public bool Result { get; set; }
		public string Message { get; set; }
		public List<string> ImagesPathList { get => imagesPathList; set => imagesPathList = value; }
		public List<PackageLabel> TemplateColList { get => templateColList; set => templateColList = value; }
	}
}
