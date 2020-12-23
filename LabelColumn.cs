using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHLabelPrint
{
	public class LabelColumn
	{
		public string Id { get; set; }
		public string Colcode { get; set; }//--字段编码
		public string Coltitle { get; set; }//--字段标题
		public string Colfunc { get; set; }//-- 字段数据处理方法
		public int Inputtype { get; set; }//--0为只读计算列，1为输入框input,2为下拉框combo,3组装列，4流水号，其他未知
		public string Combosource { get; set; } //-- 下拉框的数据源配置；
		public int State { get; set; }//-- 删除标识
		public int EnableMark { get; set; }//-- 启用标识
		public int LinkWorkcode { get; set; } //与工单关联
		public string MappingTempColCode { get; set; } //映射模板字段
		/// <summary>
		/// 标签唯一标识列
		/// </summary>
		public int LabelPrimaryKey { get; set; }
	}
}
