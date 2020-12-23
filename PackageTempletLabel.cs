using Newtonsoft.Json;
using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AHLabelPrint
{
	public class PackageTempletLabel
	{
		/// <summary>
		/// 宽度
		/// </summary>
		public int Width { get; set; }
		/// <summary>
		/// 高度
		/// </summary>
		public int Height { get; set; }
		/// <summary>
		/// 生成新的标签路径
		/// </summary>
		public string LabelImageFolderPath { get; set; }
		/// <summary>
		/// 模板路径
		/// </summary>
		public string TempletUrl { get; set; }
		public string PrinterName { get; set; }
		/// <summary>
		/// 打印张数
		/// </summary>
		public int PrintNumber { get; set; }
		/// <summary>
		/// 生成图片文件名关键标识,可以用工单号赋值
		/// </summary>
		public string ImageFileCode { get; set; }

		private List<PackageLabel> printLabels = new List<PackageLabel>();
		public List<PackageLabel> PrintLabels { get => printLabels; set => printLabels = value; }

		public Engine btEngine { get; set; }

		//public int MillimeterToPixel(int dpi, int length) //length是毫米，1厘米=10毫米
		//{
		//	double millimererTopixel = 25.4;
		//	//1英寸=25.4mm=96DPI，那么1mm=96/25.4DPI
		//	return (((dpi / millimererTopixel) * length)).ToInt();
		//}

		/// <summary>
		/// 标签打印
		/// </summary>
		/// <returns></returns>
		public PrintMessage Print()
		{
			PrintMessage prtMsg = new PrintMessage();
			prtMsg.Result = false;
			string imageFilePath = string.Empty;
			string imageFileName = string.Empty;
			if (string.IsNullOrEmpty(this.ImageFileCode))
			{
				this.ImageFileCode = DateTime.Now.ToString("yyyyMMddHHmissfff");
			}

			if (!this.LabelImageFolderPath.EndsWith("\\"))
			{
				this.LabelImageFolderPath = this.LabelImageFolderPath + "\\";
			}
			string expMsg = "";
			try
			{
				Stopwatch watch = new Stopwatch();
				watch.Reset();
				watch.Start();
				Seagull.BarTender.Print.Messages m;
				LabelFormatDocument labelFormat = btEngine.Documents.Open(this.TempletUrl);
				int printNumber = this.PrintLabels.Count;
				Result r;
				List<Result> list = new List<Result>();
				bool existsLabelCol = false;
				bool checkLabelDataOK = true;
				bool printLabelOK = true;
				
				for (int i = 0; i < printNumber; i++)
				{
					checkLabelDataOK = true;
					imageFileName = this.ImageFileCode + "_" + Guid.NewGuid().ToString() + ".jpg";
					imageFilePath = this.LabelImageFolderPath + imageFileName;

					foreach (SubString subStr in labelFormat.SubStrings)
					{
						existsLabelCol = false;
						foreach (KeyValuePair<string, string> item in PrintLabels[i].ParamValues)
						{
							if (subStr.Name.Equals(item.Key)&&!string.IsNullOrEmpty(item.Value))
							{
								existsLabelCol = true;
								labelFormat.SubStrings.SetSubString(item.Key, item.Value);
								break;
							}
						}

						if (!existsLabelCol)
						{
							expMsg = subStr.Name + "为空";
							checkLabelDataOK = false;
							break;
						}
					}

					if (!checkLabelDataOK)
					{
						printLabelOK = false;
						break;
					}
					

					if (labelFormat != null)
					{
						//labelFormat.ExportPrintPreviewToFile(this.LabelImageFolderPath, @"\exp.bmp", ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(300, 300), System.Drawing.Color.White, OverwriteOptions.Overwrite, true, true, out m);
						//if (!m.HasError)
						//{
						//	labelFormat.ExportImageToFile(imageFilePath, ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(600, 600), OverwriteOptions.Overwrite);

						//	Image image = Image.FromFile(imageFilePath);
						//	Bitmap NmpImage = new Bitmap(image, new Size((int)MillimetersToPixelsWidth(100), (int)MillimetersToPixelsWidth(50)));//
						//}

						//Generate a thumbnail for it.


						labelFormat.ExportImageToFile(imageFilePath, ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(300), OverwriteOptions.Overwrite);

						//Image image = Image.FromFile(imageFilePath);
						//Bitmap NmpImage = new Bitmap(image);
						//NmpImage=GetThumbnail(NmpImage, 430, 290);
						//NmpImage.Save(imageFilePath.Replace(".jpg","1.jpg"));
						//labelFormat.Print("labelprint", 10000, out m);
						//labelFormat.ExportImageToFile(imageFilePath, ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(ImageResolution.Printer), OverwriteOptions.Overwrite);

						//labelFormat.ExportImageToFile(imageFilePath, ImageType.JPEG, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, new Resolution(430, 214),OverwriteOptions.Overwrite);

						//prtMsg.ImagesPathList.Add(imageFileName);

						//labelFormat.PageSetup.PaperWidth = this.Width;
						//labelFormat.PageSetup.PaperHeight = this.Height;
						labelFormat.PageSetup.LabelWidth = this.Width;
						labelFormat.PageSetup.LabelHeight = this.Height;
						labelFormat.PrintSetup.PrinterName = PrinterName;
						r = labelFormat.Print("lblprint_"+ this.Width+"*"+ this.Height,out m);
						if (Result.Success == r)
						{
							list.Add(r);
						}
						else
						{
							Logger.GetLogger().WriteLog(JsonConvert.SerializeObject(m));
							printLabelOK = false;
							break;
						}
					}
				}

				if (!printLabelOK)
				{
					prtMsg.Result = false;
					prtMsg.Message = "打印失败"+ expMsg;
				}
				else
				{
					prtMsg.CostTimes = watch.ElapsedMilliseconds;
					prtMsg.Result = true;
					prtMsg.Message = "打印成功" + string.Join(",", list);
				}
				watch.Stop();
			}
			catch (Exception)
			{
				prtMsg.Message = "打印失败"+ expMsg;
				throw;
			}
			return prtMsg;
		}


		/// <summary>
		/// 修改图片的大小
		/// </summary>
		/// <param name="b"></param>
		/// <param name="destHeight"></param>
		/// <param name="destWidth"></param>
		/// <returns></returns>
		//public Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
		//{
		//	System.Drawing.Image imgSource = b;
		//	System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
		//	int sW = 0, sH = 0;
		//	// 按比例缩放           
		//	int sWidth = imgSource.Width;
		//	int sHeight = imgSource.Height;
		//	if (sHeight > destHeight || sWidth > destWidth)
		//	{
		//		if ((sWidth * destHeight) > (sHeight * destWidth))
		//		{
		//			sW = destWidth;
		//			sH = (destWidth * sHeight) / sWidth;
		//		}
		//		else
		//		{
		//			sH = destHeight;
		//			sW = (sWidth * destHeight) / sHeight;
		//		}
		//	}
		//	else
		//	{
		//		sW = sWidth;
		//		sH = sHeight;
		//	}
		//	Bitmap outBmp = new Bitmap(destWidth, destHeight);
		//	Graphics g = Graphics.FromImage(outBmp);
		//	g.Clear(Color.Transparent);
		//	// 设置画布的描绘质量         
		//	g.CompositingQuality = CompositingQuality.HighQuality;
		//	g.SmoothingMode = SmoothingMode.HighQuality;
		//	g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		//	g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
		//	g.Dispose();
		//	// 以下代码为保存图片时，设置压缩质量     
		//	EncoderParameters encoderParams = new EncoderParameters();
		//	long[] quality = new long[1];
		//	quality[0] = 100;
		//	EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
		//	encoderParams.Param[0] = encoderParam;
		//	imgSource.Dispose();
		//	return outBmp;
		//}

		/// <summary>
		/// 取得模版里的参数
		/// </summary>
		public List<LabelParam> GetPrintTemplateParams()
		{
			List<LabelParam> prams = new List<LabelParam>();
			//using (Engine btEngine = new Engine(true))
			//{
				LabelFormatDocument labelFormat = this.btEngine.Documents.Open(this.TempletUrl);
				XmlDocument xmldoc = new XmlDocument();
				//<?xml version="1.0" encoding="utf-16"?><SubStrings><SubString Name="name"><Value>张老师</Value></SubString><SubString Name="age"><Value>23</Value></SubString><SubString Name="code"><Value>SPZ1TM681E11P25RAXXX</Value></SubString><SubString Name="ID"><Value /></SubString></SubStrings>
				xmldoc.LoadXml(labelFormat.SubStrings.XML);
				XmlNodeList nodeList = xmldoc.SelectNodes("SubStrings/SubString");
				LabelParam labelPram = null;
				foreach (XmlNode item in nodeList)
				{
					labelPram = new LabelParam();
					labelPram.Name = item.Attributes["Name"].Value;
					labelPram.Value = item.InnerText;
					prams.Add(labelPram);
				}
			//}
			return prams;
		}
	}
}
