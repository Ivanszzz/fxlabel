using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHLabelPrint
{
	class ClientResponseMsg
	{
		public bool IsSuccess { get; set; }
		public string Messaage { get; set; }
		public object Data { get; set; }
		public int totalRows { get; set; }
	}
}
