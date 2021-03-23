using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHLabelPrint
{
	public class LabelCustomer
	{
		public string Id { get; set; }
		public string MappingChildId { get; set; } //关联子标签
		public int MappingProgramId { get; set; } //映射工序，按此工序取数量；
		public string PackageQtyJson { get; set; } //包装数量枚举数据集合
		public string CustCode { get; set; } //客户编码
		public string CustTitle { get; set; } //客户标题
		public string FactoryCode { get; set; } //工厂编码
		public string FactoryTitle { get; set; } //工厂名称
		public string TemplateType { get; set; } //模板类型,内中外箱
		public string LabelTemplateUrl { get; set; } //客户标签模板路径
		public string LabelTemplateColString { get; set; } //客户标签模板的字段内容
		public int LabelHeight { get; set; } //客户标签高度
		public int LabelWidth { get; set; } //客户标签宽度
		public string Supplier { get; set; } //产品制造商
		public string SupplierCode { get; set; } //制造商编号
		public string ManufacturePN { get; set; } //厂家型号
		public string CountryOfOrigin { get; set; } //原产地
		public int State { get; set; } //状态标识，0删除1启用
		public string Creator { get; set; } //
		public DateTime CreateTime { get; set; } //
		public string Updatator { get; set; } //
		public DateTime UpdateTime { get; set; } //
		public string SerialCode { get; set; }
		public int WorkcodeRelaceAllG { get; set; } //客户标签去掉工单号里的‘-’
		public int LabelShowOneWorkcode { get; set; } //客户标签仅显示任意一个工单号；
		public string PreWorkCodeList { get; set; }//工单前缀符号
	}
}
