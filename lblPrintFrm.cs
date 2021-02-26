using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seagull.BarTender.Print;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Windows.Forms;
using static System.Windows.Forms.CheckedListBox;

namespace AHLabelPrint
{
    public partial class lblPrintFrm : Form
    {
        private string CustomerId = string.Empty;
        private string OutterRecordId = string.Empty;
        private string MappedChildId = string.Empty;
        private LabelCustomer custObj = new LabelCustomer();
        private Dictionary<string, JObject> enablePrintWorkcode = new Dictionary<string, JObject>();
        private List<LabelColumn> colList = new List<LabelColumn>();
        private TextBoxRemind remind = null;
        private ArrayList selectRowIndexList = new ArrayList();
        private int selectRowIndex = 0;
        Boolean isBeforeOperation = true;
        DateTime _dt = DateTime.Now;
        public lblPrintFrm()
        {
            InitializeComponent();
            skinEngine1.SkinFile = System.Environment.CurrentDirectory + "\\Skins\\DiamondBlue.ssk";
        }

        private void lblPrintFrm_Load(object sender, EventArgs e)
        {
            loadUserInfo();
            loadPackageTypeComboxData(this.comb_lblcategory);
            //loadPackageQtyComboxData(this.comb_packageQty);
            JObject searCondition = new JObject();
            loadLabelCustomerData(searCondition);
        }

        private void loadUserInfo()
        {
            this.lbl_username.Text =Program.logName;
        }

        private void loadPackageQtyComboxData(ComboBox ctrlObj, LabelCustomer custTempObj)
        {
            ctrlObj.Items.Clear();
            JArray jarray = JsonConvert.DeserializeObject<JArray>(custTempObj.PackageQtyJson);
            foreach (var item in jarray)
            {
                ctrlObj.Items.Add(item.ToString());
            }
            ctrlObj.SelectedItem = "2000";
        }

        
        private void loadMappingProgramIdComboxData(ComboBox ctrlObj)
        {
            ArrayList mylist = new ArrayList();
            mylist.Add(new DictionaryEntry("19", "奈印"));
            mylist.Add(new DictionaryEntry("26", "包装"));
            ctrlObj.DataSource = mylist;
            ctrlObj.DisplayMember = "Value";
            ctrlObj.ValueMember = "Key";
        }
        private void loadPrintRecordStateComboxData(ComboBox ctrlObj)
        {
            ArrayList mylist = new ArrayList();
            mylist.Add(new DictionaryEntry("", "全部"));
            mylist.Add(new DictionaryEntry("0", "未绑定"));
            mylist.Add(new DictionaryEntry("1", "已绑定"));
            ctrlObj.DataSource = mylist;
            ctrlObj.DisplayMember = "Value";
            ctrlObj.ValueMember = "Key";
        }
        private void loadPrintStateComboxData(ComboBox ctrlObj)
        {
            ArrayList mylist = new ArrayList();
            mylist.Add(new DictionaryEntry("", "全部"));
            mylist.Add(new DictionaryEntry("0", "未打印"));
            mylist.Add(new DictionaryEntry("1", "已打印"));
            ctrlObj.DataSource = mylist;
            ctrlObj.DisplayMember = "Value";
            ctrlObj.ValueMember = "Key";
        }
        
        private void loadPackageTypeComboxData(ComboBox ctrlObj)
        {
            ArrayList mylist = new ArrayList();
            mylist.Add(new DictionaryEntry("", "全部"));
            mylist.Add(new DictionaryEntry("box", "盒装"));
            mylist.Add(new DictionaryEntry("inner", "中箱"));
            mylist.Add(new DictionaryEntry("outter", "外箱"));
            ctrlObj.DataSource = mylist;
            ctrlObj.DisplayMember = "Value";
            ctrlObj.ValueMember = "Key";
        }

        /// <summary>
        /// 加载标签主界面查询树
        /// </summary>
        private void loadLabelCustomerSelectTree(List<LabelCustomer> list)
        {
            ArrayList factoryName = new ArrayList();
            foreach(LabelCustomer item in list)
            {
                if(!factoryName.Contains(item.FactoryTitle))
                    factoryName.Add(item.FactoryTitle);
            }

            for(int i = 0; i < factoryName.Count; i++)
            {
                TreeNode node = new TreeNode(factoryName[i].ToString());
                foreach(LabelCustomer item in list)
                {
                    if (item.FactoryTitle == factoryName[i].ToString())
                        node.Nodes.Add(GetTemplateType(item.TemplateType));
                }
                this.client_treeView.Nodes.Add(node);
            }
            this.client_treeView.Focus();
        }

        private void loadLabelCustomerData(JObject searCondition)
        {
            string returnMsg = string.Empty;
            string url = "/LabelCustomer/GetCustomerList";
            string body = JsonConvert.SerializeObject(searCondition);
            returnMsg = AjaxHelper.ClientRequest(url, body);
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);

            //{ "IsSuccess":true,"totalRows":1,"Data":[{"Id":"AFEB4C8C-CA9D-432E-891D-E8FED98A8F15","CustCode":"test","CustTitle":"test","FactoryCode":"test","FactoryTitle":"test","TemplateType":"box","LabelTemplateUrl":"/Label/btw/boshuo.btw","LabelTemplateColString":"","LabelHeight":50,"LabelWidth":75,"Supplier":"","SupplierCode":"","ManufacturePN":"","CountryOfOrigin":"","State":1,"Creator":"","CreateTime":"\/Date(-62135596800000)\/","Updatator":"","UpdateTime":"\/Date(-62135596800000)\/"}],"Messaage":""}
            if (msgObj.IsSuccess)
            {
                gdv_customer.Rows.Clear();
                List<LabelCustomer> custList = JsonConvert.DeserializeObject<List<LabelCustomer>>(msgObj.Data.ToString());
                if(this.client_treeView.Nodes.Count == 0)
                    loadLabelCustomerSelectTree(custList);
                DataGridViewRow viewRow = new DataGridViewRow();
                int cellIndex = 0;
                foreach (LabelCustomer item in custList)
                {
                    cellIndex = 0;
                    viewRow = new DataGridViewRow();
                    viewRow.CreateCells(this.gdv_customer);

                    viewRow.Cells[cellIndex++].Value = item.Id;
                    viewRow.Cells[cellIndex++].Value = item.MappingChildId; 
                    viewRow.Cells[cellIndex++].Value = item.CustCode;
                    viewRow.Cells[cellIndex++].Value = item.CustTitle;
                    viewRow.Cells[cellIndex++].Value = item.FactoryCode;
                    viewRow.Cells[cellIndex++].Value = item.FactoryTitle;
                    viewRow.Cells[cellIndex++].Value = GetTemplateType(item.TemplateType);
                    viewRow.Cells[cellIndex++].Value = item.TemplateType;
                    viewRow.Cells[cellIndex++].Value = item.LabelWidth;
                    viewRow.Cells[cellIndex++].Value = item.LabelHeight;
                    viewRow.Cells[cellIndex++].Value = item.SupplierCode;
                    viewRow.Cells[cellIndex++].Value = item.Supplier;
                    gdv_customer.Rows.Add(viewRow);
                }
            }
        }

        private string GetTemplateType(string templateType)
        {
            string title = string.Empty;
            switch (templateType)
            {
                case "box":
                    title = "盒装";
                    break;
                case "inner":
                    title = "中箱";
                    break;
                case "outter":
                    title = "外箱";
                    break;
            }
            return title;
        }

        private string DisGetTemplateType(string templateName)
        {
            string title = string.Empty;
            switch (templateName)
            {
                case "盒装":
                    title = "box";
                    break;
                case "中箱":
                    title = "inner";
                    break;
                case "外箱":
                    title = "outter"; 
                    break;
                case "":
                    title = "";
                    break;
            }
            return title;
        }

        private void loadExInputControl(string CustomerId,ComboBox combObj,Label lblObj,GroupBox grpbox)
        {
            //根据customerId 获取模板；
            GetCustomerInfo(CustomerId, combObj, lblObj);// , this.lbl_lblsize);
            string returnMsg = string.Empty;
            LoadExFormControl(CustomerId, grpbox);
        }

        private void LoadExFormControl(string CustomerId, GroupBox grpbox)
        {
            grpbox.Controls.Clear();
            ClientResponseMsg msgObj = GetCustomerLabelForm(CustomerId);
            if (msgObj.IsSuccess)
            {
                colList = JsonConvert.DeserializeObject<List<LabelColumn>>(JsonConvert.SerializeObject(msgObj.Data));
                Label lbl = new Label();
                TextBox ttt = new TextBox();
                TextBox tt = new TextBox();
                ComboBox comb = new ComboBox();
                int i = 0;
                foreach (LabelColumn item in colList)
                {
                    //--0为只读，1为输入框input,2为下拉框combo,3 字段组装模板，4 自动生成流水号,5为记忆输出框input
                    if (item.Inputtype == 1)
                    {
                        lbl = new Label();
                        lbl.Name = item.Colcode;
                        lbl.Text = item.Coltitle;
                        lbl.AutoSize = true;
                        lbl.Size = new System.Drawing.Size(59, 12);

                        grpbox.Controls.Add(lbl);
                        lbl.Location = new Point(20, 21 + i * 27);

                        ttt = new TextBox();
                        ttt.Name = item.MappingTempColCode;
                        ttt.AutoSize = true;
                        ttt.Size = new System.Drawing.Size(151, 21);
                        grpbox.Controls.Add(ttt);
                        ttt.Location = new Point(100, 21 + i * 27);
                        i++;
                    }
                    else if (item.Inputtype == 2)
                    {
                        lbl = new Label();
                        lbl.Name = item.Colcode;
                        lbl.Text = item.Coltitle;
                        lbl.AutoSize = true;
                        lbl.Size = new System.Drawing.Size(59, 12);

                        grpbox.Controls.Add(lbl);
                        lbl.Location = new Point(20, 21 + i * 27);

                        comb = new ComboBox();
                        comb.Name = item.MappingTempColCode;
                        comb.AutoSize = true;
                        comb.Size = new System.Drawing.Size(151, 21);
                        this.grpbox_otherInput.Controls.Add(comb);
                        comb.Location = new Point(100, 21 + i * 27);
                        i++;
                    }
                    //增加输入框的记忆功能
                    else if(item.Inputtype == 5)
                    {
                        lbl = new Label();
                        lbl.Name = item.Colcode;
                        lbl.Text = item.Coltitle;
                        lbl.AutoSize = true;
                        lbl.Size = new System.Drawing.Size(59, 12);

                        grpbox.Controls.Add(lbl);
                        lbl.Location = new Point(20, 21 + i * 27);

                        tt = new TextBox();
                        tt.Name = item.MappingTempColCode;
                        tt.AutoSize = true;
                        tt.Size = new System.Drawing.Size(151, 21);
                        tt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        tt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        grpbox.Controls.Add(tt);

                        remind = new TextBoxRemind();
                        remind.InitAutoCompleteCustomSource(tt, item.MappingTempColCode);

                        tt.Location = new Point(100, 21 + i * 27);
                        i++;
                    }
                }
            }
        }

        private ClientResponseMsg GetCustomerLabelForm(string CustomerId )
        {
            string body = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("CustomerId", CustomerId);
            body = JsonConvert.SerializeObject(reqData);
            string returnMsg = AjaxHelper.ClientRequest("/LabelPrint/GetPrintForm", body);
            //{"IsSuccess":true,"Data":[{"Id":"6752986A-89CD-4260-B394-D9DCCDC5EA8D","Colcode":"po","Coltitle":"采购订单","Colfunc":"","Inputtype":1,"Combosource":"","State":0,"EnableMark":0,"LinkWorkcode":0},{"Id":"A57D0FF4-8D80-4A8D-8E0F-58A255EEF09F","Colcode":"clientcode","Coltitle":"客户料号","Colfunc":"","Inputtype":1,"Combosource":"","State":0,"EnableMark":0,"LinkWorkcode":0}],"Messaage":""}
           return JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
        }

        private ClientResponseMsg GetCustomerInfo(string CustomerId,ComboBox ctrlObj,Label lblObj)
        {
            JObject reqData = new JObject();

            ClientResponseMsg msgObj;
            string body = string.Empty;
            reqData.Add("CustomerId", CustomerId);
            body = JsonConvert.SerializeObject(reqData);
            string customerInfo = AjaxHelper.ClientRequest("/LabelCustomer/GetCustomerInfo", body);
            msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(customerInfo);
            if (msgObj.IsSuccess)
            {
                custObj = JsonConvert.DeserializeObject<LabelCustomer>(msgObj.Data.ToString());
                ctrlObj.SelectedItem = custObj.TemplateType.ToLower();
                lblObj.Text = custObj.LabelWidth + "mm * " + custObj.LabelHeight + " mm";
            }
            return msgObj;
        }

        /// <summary>
        /// 获取到客户端打印机列表
        /// </summary>
        private void loadPrinter(ComboBox ctrlObj)
        {
            ctrlObj.Items.Clear();
            Printers printers = new Printers();
            foreach (Printer printer in printers)
            {
                ctrlObj.Items.Add(printer.PrinterName);
            }

            if (printers.Count > 0)
            {
                // Automatically select the default printer.
                ctrlObj.SelectedItem = printers.Default.PrinterName;
            }
        }

        private void txt_workcode_KeyUp(object sender, KeyEventArgs e)
        {
            DateTime _tempt = DateTime.Now;
            TimeSpan ts = _tempt.Subtract(_dt);
            if (ts.Milliseconds > 50)
            {
                if (txt_workcode.Text != "")
                    MessageBox.Show("禁止手输!");
                txt_workcode.Text = "";
            }
        }

        private void comb_packageQty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //包装数量改变，就触发重新计算最大打印张数
            CalcMaxPrintNumer();
        }

        public DialogResult ConfirmYesNo(string prompt)
        {
            return MessageBox.Show(prompt, "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //打印处理，先获取或者控件输入值；
            object printNameTemp = this.comb_printlist.SelectedItem;//打印机
            object packageTypeTemp = comb_packagetype.SelectedItem;//包装类型
            object packageQtyTemp = comb_packageQty.SelectedItem;//包装数量
            bool mergin = chk_merginprint.Checked;//合批勾选
            string printNum = txt_printNum.Text;//打印数量

            string selectedItem = string.Empty;
            string selectItemTemp;
            for (int i = 0; i < chklist_workcode.Items.Count; i++)
            {
                if (chklist_workcode.GetItemChecked(i))
                {
                    selectItemTemp = chklist_workcode.Items[i].ToString();
                    //补上工单前缀符号
                    if (!string.IsNullOrEmpty(custObj.PreWorkCodeList))
                    {
                        selectItemTemp = custObj.PreWorkCodeList+ selectItemTemp;
                    }

                    //替换工单号里的-
                    if (custObj.WorkcodeRelaceAllG == 1)
                    {
                        selectedItem = selectedItem + selectItemTemp.Replace("-","") + ",";
                    }
                    else
                    {
                        selectedItem = selectedItem + selectItemTemp + ",";
                    }
                }
            }
            selectedItem = selectedItem.TrimEnd(',');

            #region 数据检查
            if (printNameTemp == null)
            {
                MessageBox.Show("请选择打印机");
                return;
            }
            if (packageTypeTemp == null)
            {
                MessageBox.Show("请选择包装类型");
                return;
            }
            if (packageQtyTemp == null)
            {
                MessageBox.Show("请选择包装数量");
                return;
            }
            if (selectedItem == null || string.IsNullOrEmpty(selectedItem))
            {
                MessageBox.Show("请选择显示在标签上的工单");
                return;
            }
            else
            {
                if (custObj.LabelShowOneWorkcode == 1)
                {
                    if (selectedItem.Split(',').Length > 1)
                    {
                        MessageBox.Show("该标签上只能显示一个工单号");
                        return;
                    }
                }
            }

            if (printNum == null || string.IsNullOrEmpty(printNum) || Convert.ToInt32(printNum)==0)
            {
                MessageBox.Show("请输入正确的打印数量");
                return;
            }

            if (Convert.ToInt32(printNum) > Convert.ToInt32(string.IsNullOrEmpty(this.txt_maxPrintNum.Text) ? "0" : this.txt_maxPrintNum.Text))
            {
                MessageBox.Show("打印张数不能大于最大允许打印张数");
                return;
            }

            if (this.chk_merginprint.Checked)
            {
                if (chklist_workcode.Items.Count < 2)
                {
                    MessageBox.Show("需要2张工单以上才能合批");
                    return;
                }
                if (Convert.ToInt32(printNum) > 1)
                {
                    MessageBox.Show("合批打印最大只能打印1张标签");
                    return;
                }
            }
            else
            {
                if (chklist_workcode.Items.Count > 1)
                {
                    MessageBox.Show("存在多张工单，请选择合批打印");
                    return;
                }
            }

            if (custObj.LabelTemplateColString == null || custObj.LabelTemplateColString == "null")
            {
                MessageBox.Show("请点击更新模板");
                return;
            }
            #endregion

            #region 组装标签数据参数输入
            JObject printParamData = new JObject();
            //组装其他输入的参数
            bool otherCtrlCheck = GetOtherInput(printParamData, this.grpbox_otherInput);
            if (!otherCtrlCheck)
            {
                return;
            }
            //组装打印页面需要的系统必要参数
            printParamData.Add("UserNo", Program.UserNo);
            printParamData.Add("merginPrint", this.chk_merginprint.Checked);
            JArray merginData = GetMerginData(packageQtyTemp);
            printParamData.Add("merginData", merginData);
            DictionaryEntry packageType = (DictionaryEntry)packageTypeTemp;
            printParamData.Add("packageType", packageType.Key.ToString());
            printParamData.Add("printNum", printNum.ToString());
            printParamData.Add("lbl_PackageQty", packageQtyTemp.ToString());
            //当开单即打标签时 工单取一级工单
            printParamData.Add("lbl_WorkCodeList",isBeforeOperation ? this.dgv_workprintdetail.SelectedRows[0].Cells["workcode"].Value.ToString():selectedItem.ToString());
            printParamData.Add("CustomerId", CustomerId);
            printParamData.Add("lbl_SupplyCode", custObj.SupplierCode);

            //组装扫码工单里的参数：包含计算值
            //若为合批，则取当中一批工单的数据作为标签的数据；
            JObject lblMainWorkInfoObj = null;
            foreach (KeyValuePair<string, JObject> kvp in enablePrintWorkcode)
            {
                lblMainWorkInfoObj = kvp.Value;
                break;
            }

            foreach (var item in lblMainWorkInfoObj)
            {
                if (",id,".IndexOf("," + item.Key.ToLower() + ",") == -1)
                {
                    printParamData.Add(item.Key, item.Value);
                }
            }

            //组装标签客户信息里的字段

            //List<string> ignoreColumnList = new List<string>() { "Id", "TemplateType", "LabelTemplateUrl", "LabelTemplateColString", "LabelHeight", "LabelWidth", "State", "Creator", "CreateTime", "Updatator", " UpdateTime" };
            //Type tp = custObj.GetType();
            //foreach (PropertyInfo item in tp.GetProperties())
            //{
            //    //该属性不可写，直接跳出，或者该字段允许忽略；
            //    if (!item.CanWrite || ignoreColumnList.Contains(item.Name)) continue;
            //    printParamData.Add(item.Name, item.GetValue(custObj, null).ToString());
            //}

            //获取模板里的默认值
            List<LabelParam> labelParamList= GetDefaultValueFromTemplate(printParamData, custObj);

            //组装配置的动态组装数据和流水号计算数据，需要连接服务器生成流水号或者序列号，可能需要包含在二维码里；
            int printCount = Convert.ToInt32(printNum);
            bool checkSerialParams = GetSerialNumber(printParamData, printCount, packageType.Key.ToString());
            if (!checkSerialParams)
            {
                MessageBox.Show("流水号规则配置必要参数为空");
                return;
            }
            //组装字段
            GetLabelCalcColumnData(printParamData, printCount, packageType.Key.ToString());
            //给组装字段补上bar_标签；
            InitBarColumnData(printParamData);
            #endregion

            DialogResult result = ConfirmYesNo("选择打印机为：" + printNameTemp.ToString() + ",请确认打印机设备纸张尺寸为:" + custObj.LabelWidth + "*" + custObj.LabelHeight);
            if (result == DialogResult.No)
            {
                return;
            }
            PrintMessage prtMsg = PrintBar(printNameTemp.ToString(), labelParamList, custObj, printParamData, false);
            if (!prtMsg.Result)
            {
                MessageBox.Show(prtMsg.Message);
            }
            else
            {
                //保存打印记录
                string url = "/LabelPrint/SavePrintRecord";
                string returnMsg = string.Empty;
                JObject postData = new JObject();
                postData.Add("LabelData", JsonConvert.SerializeObject(prtMsg.TemplateColList));
                postData.Add("OtherData", JsonConvert.SerializeObject(printParamData));
                string body = JsonConvert.SerializeObject(postData);
                returnMsg = AjaxHelper.ClientRequest(url, body);
                ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
                if (!msgObj.IsSuccess)
                {
                    MessageBox.Show(msgObj.Messaage);
                }
                else
                {
                    MessageBox.Show("打印成功，打印记录已生成。");
                    enablePrintWorkcode.Clear();
                    chklist_workcode.Items.Clear();
                    this.dgv_workprintdetail.Rows.Clear();
                    CalcMaxPrintNumer();
                    if (isBeforeOperation)
                    {
                        this.txt_workcode.Text = selectedItem.ToString();
                        selectRowIndexList.Clear();
                        this.txt_workcode_KeyDown(sender, new KeyEventArgs(Keys.Enter));
                        CalcMaxPrintNumer();
                    }
                    //加载打印记录
                    this.btn_SearchRecord_Click(sender, e);
                }
            }
        }

        private void InitBarColumnData(JObject printParamData)
        {
            JObject printParamDataTemp = printParamData.DeepClone().ToObject<JObject>();
            string repString = string.Empty;
            foreach (var item in printParamDataTemp.Properties())
            {
                repString = string.Empty;
                if (item.Name.IndexOf("lbl_") > -1)
                {
                    repString = item.Name.Replace("lbl_", "bar_");
                    if (printParamData.Property(repString) == null)
                    {
                        printParamData.Add(repString, item.Value);
                    }
                    else
                    {
                        printParamData[repString] = item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 模板里的前缀和后缀在这里无法实现，只能实现读取默认值
        /// </summary>
        /// <param name="printParamData"></param>
        /// <param name="custObj"></param>
        /// <returns></returns>
        private List<LabelParam> GetDefaultValueFromTemplate(JObject printParamData, LabelCustomer custObj)
        {
            List<LabelParam> templateObjList = JsonConvert.DeserializeObject<List<LabelParam>>(custObj.LabelTemplateColString);
            foreach (LabelParam item in templateObjList)
            {
                if(!string.IsNullOrEmpty(item.Name)&& !string.IsNullOrEmpty(item.Value))
                {
                    //if(printParamData.Property(item.Name) ==null)
                        printParamData[item.Name] = item.Value;
                }
            }
            return templateObjList;
        }
        public string GetMd5Hash(string strInput)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(strInput, "MD5");
        }
        private void GetLabelCalcColumnData(JObject printParamData, int printCount, string boxType)
        {
            List<LabelColumn> colListTemp = colList.FindAll(col => col.Inputtype == 3);
            List<string> listArray;
            List<string> labelPrimaryKeyArray;
            MatchCollection matchList = null;
            List<string> recordIdList = new List<string>();
            string recordId = string.Empty;
            string CombosourceTemp = string.Empty;

            if (boxType == "box")
            {
                for (int i = 0; i < printCount; i++)
                {
                    //生成打印记录ID；
                    recordId = Guid.NewGuid().ToString();
                    recordIdList.Add(recordId);
                }
                printParamData.Add("sysRecordId", string.Join("●", recordIdList));
            }
            else
            {
                recordIdList.Add(OutterRecordId);
                printParamData["sysRecordId"] = string.Join("●", recordIdList);
            }

            //循环要组装的列；
            foreach (LabelColumn col in colListTemp)
            {
                labelPrimaryKeyArray = new List<string>();
                listArray = new List<string>();
                for (int i = 0; i < printCount; i++)
                {
                    CombosourceTemp = col.Combosource;
                    //需要正则匹配函数截取出二维码的拼凑规则
                    matchList = GetRegexMapString("[", "]", CombosourceTemp);

                    foreach (Match item in matchList)
                    {
                        if (item.Value.IndexOf("sys_") > -1)
                        {
                            CombosourceTemp = CombosourceTemp.Replace(item.Value, recordIdList[i]);
                        }
                        else if (item.Value.IndexOf("srl_") > -1)
                        {
                            CombosourceTemp = CombosourceTemp.Replace(item.Value, printParamData[item.Value.Replace("[", "").Replace("]", "")].ToString().Split('●')[i]);
                        }
                        else
                        {
                            CombosourceTemp = CombosourceTemp.Replace(item.Value, printParamData[item.Value.Replace("[", "").Replace("]", "")].ToString());
                        }
                    }

                    listArray.Add(CombosourceTemp);

                    //col.Combosource
                    //获取标签唯一标识值
                    if (col.LabelPrimaryKey == 1)
                    {
                        if (!labelPrimaryKeyArray.Contains(CombosourceTemp))
                            labelPrimaryKeyArray.Add(CombosourceTemp);
                    }
                }

                //获取标签唯一标识值
                if (col.LabelPrimaryKey == 1)
                {
                    if (printParamData.Property("md5ValPrimary") == null)
                    {
                        printParamData.Add("md5ValPrimary", string.Join("●", labelPrimaryKeyArray));
                    }
                    else
                    {
                        printParamData["md5ValPrimary"]=string.Join("●", labelPrimaryKeyArray);
                    }
                }

                if (boxType == "box")
                {
                    if (printParamData.Property(col.MappingTempColCode) == null) {
                        printParamData.Add(col.MappingTempColCode, string.Join("●", listArray));
                    }
                }
                else
                {
                    //if (printParamData.Property(col.MappingTempColCode) != null)
                    {
                        printParamData[col.MappingTempColCode] = string.Join("●", listArray);
                    }
                }
            }
        }

        private bool GetSerialNumber(JObject printParamData, int printCount,string boxType)
        {
            List<string> listArray = null;
            bool checkSerialParams=false;
            //流水号字段
            List<LabelColumn> colListSerialNumberTemp = colList.FindAll(col => col.Inputtype == 4);
            JObject serialReqParams = null;
            JObject serialTemplate = null;
            checkSerialParams = true;
            foreach (LabelColumn col in colListSerialNumberTemp)
            {
                serialReqParams = new JObject();
                listArray = new List<string>();
                #region 生成流水码参数： 注释
                //生成流水码参数：
                //serialCode：客户编码
                //clientcode：客户料号
                //cycle：奈印周期
                //date：发货周期
                //structure：组成格式
                //count：生成流水码数量
                //exec proc_Sel_SysSerialNumber @p_serialCode,@p_clientcode,@p_cycle,@p_date,@p_structure,@p_count,@p_currentVal output
                //select @p_currentVal
                #endregion
                //{"p_clientcode":"lbl_ClientCode","p_cycle":"lbl_Cycle","p_date":"lbl_CycleDate","p_structure":"clientcode,cycle"}
                serialTemplate = JsonConvert.DeserializeObject<JObject>(col.Combosource);
                checkSerialParams = CheckSerialCondition(printParamData, col);
                if (!checkSerialParams)
                {
                    break;
                }

                serialReqParams.Add("p_serialCode", custObj.SerialCode);
                serialReqParams.Add("p_clientcode", printParamData[serialTemplate["p_clientcode"].ToString()]);
                serialReqParams.Add("p_cycle", printParamData[serialTemplate["p_cycle"].ToString()]);
                serialReqParams.Add("p_date", printParamData[serialTemplate["p_date"].ToString()]);
                serialReqParams.Add("p_structure", serialTemplate["p_structure"].ToString());
                serialReqParams.Add("p_count", printCount);
                serialReqParams.Add("customerId", custObj.Id);

                string body = JsonConvert.SerializeObject(serialReqParams);
                string customerInfo = AjaxHelper.ClientRequest("/LabelPrint/GetSysSerialNumber", body);
                ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(customerInfo);
                if (msgObj.IsSuccess)
                {
                    JObject serivalObj = JsonConvert.DeserializeObject<JObject>(msgObj.Data.ToString());
                    if (boxType == "box")
                    {
                        printParamData.Add(col.MappingTempColCode, serivalObj["serialNumber"].ToString().Replace(",", "●"));
                    }
                    else
                    {
                        printParamData[col.MappingTempColCode]= serivalObj["serialNumber"].ToString().Replace(",", "●");
                    }
                }
            }
            return checkSerialParams;
        }

        private bool GetOtherInput(JObject printParamData, GroupBox grpBox)
        {
            bool otherCtrlCheck = true;
            foreach (Control item in grpBox.Controls)
            {
                if (item is TextBox)
                {
                    TextBox t = (TextBox)item;
                    //20201009zyong   加入字段类型为5的输入框的记忆功能
                    ClientResponseMsg msgObj = GetCustomerLabelForm(CustomerId);
                    if (msgObj.IsSuccess)
                    {
                        colList = JsonConvert.DeserializeObject<List<LabelColumn>>(JsonConvert.SerializeObject(msgObj.Data));
                        foreach (LabelColumn item_ in colList)
                        {
                            if (item_.Inputtype == 5)
                            {
                                if (t.Text.Trim() != "" && item_.MappingTempColCode== t.Name)
                                {
                                    remind.Remind(t.Text.Trim(), t.Name);
                                    remind.InitAutoCompleteCustomSource(t, t.Name);
                                }
                            }
                        }
                    }
                    //-----------------------------------
                    if (string.IsNullOrEmpty(t.Text))
                    {
                        otherCtrlCheck = false;
                        MessageBox.Show("请补全其他输入下的内容");
                        break;
                    }
                    if (printParamData.Property(t.Name) != null)
                    {
                        printParamData[t.Name] = t.Text;
                    }
                    else
                    {
                        printParamData.Add(t.Name, t.Text);
                    }
                }
            }

            return otherCtrlCheck;
        }

        /// <summary>
        /// 获取待打印的标签数据
        /// </summary>
        /// <param name="packageQtyTemp"></param>
        /// <returns></returns>
        private JArray GetMerginData(object packageQtyTemp)
        {
            JArray merginData = new JArray();
            JObject merginItem = null;
            int consumeQty = 0;
            int enablePrintQtyTemp = 0;
            int packageQty = 0;
            if (isBeforeOperation)
            {
                merginItem = new JObject();
                enablePrintQtyTemp = Convert.ToInt32(this.dgv_workprintdetail.SelectedRows[0].Cells["enablePrintQty"].Value.ToString());
                packageQty = Convert.ToInt32(packageQtyTemp.ToString());
                merginItem.Add("orderDetailId", this.dgv_workprintdetail.SelectedRows[0].Cells["orderDetailId"].Value.ToString());
                merginItem.Add("workcode", this.dgv_workprintdetail.SelectedRows[0].Cells["workcode"].Value.ToString());
                merginItem.Add("qty", packageQty.ToString());
                merginData.Add(merginItem);
            }
            else
            {
                //升序排序 则优先消耗数量小的工单
                dgv_workprintdetail.Sort(dgv_workprintdetail.Columns["enablePrintQty"], ListSortDirection.Ascending);
                foreach (DataGridViewRow item in dgv_workprintdetail.Rows)
                {
                    merginItem = new JObject();
                    enablePrintQtyTemp = Convert.ToInt32(item.Cells["enablePrintQty"].Value.ToString());
                    packageQty = Convert.ToInt32(packageQtyTemp.ToString());
                    merginItem.Add("orderDetailId", item.Cells["orderDetailId"].Value.ToString());
                    merginItem.Add("workcode", item.Cells["workcode"].Value.ToString());
                    //合批工单分别的打印数量
                    //若标签工单数量>合批工单的数量
                    //累计消耗打印数量小于标签数量时
                    if (this.chk_merginprint.Checked)
                    {
                        if (consumeQty + enablePrintQtyTemp < packageQty && packageQty < enablePrintQtyTemp)
                        {
                            consumeQty = consumeQty + Convert.ToInt32(item.Cells["enablePrintQty"].Value.ToString());
                            merginItem.Add("qty", item.Cells["enablePrintQty"].Value.ToString());
                        }
                        else
                        {
                            merginItem.Add("qty", (enablePrintQtyTemp - consumeQty).ToString());
                        }
                    }
                    else
                    {
                        if (packageQty <= enablePrintQtyTemp)
                        {
                            merginItem.Add("qty", packageQty.ToString());
                        }
                    }
                    merginData.Add(merginItem);
                }
            }

            return merginData;
        }

        private MatchCollection GetRegexMapString(string start,string end,string textString)
        {
            string pattern = Regex.Escape(start) + "(?<content>[^\\" + start + "\\" + end + "].*?)" + Regex.Escape(end);
            return Regex.Matches(textString, pattern);
        }

        private bool CheckSerialCondition(JObject printParamData, LabelColumn col)
        {
            bool checkSerialParams = true;
            JObject serialTemplate = JsonConvert.DeserializeObject<JObject>(col.Combosource);
            if (serialTemplate["p_structure"].ToString().ToLower().Equals("clientcode,cycle"))
            {
                if (printParamData[serialTemplate["p_clientcode"].ToString()] == null
                    || string.IsNullOrEmpty(printParamData[serialTemplate["p_clientcode"].ToString()].ToString())
                    || printParamData[serialTemplate["p_cycle"].ToString()] == null
                    || string.IsNullOrEmpty(printParamData[serialTemplate["p_cycle"].ToString()].ToString())
                    )
                {
                    checkSerialParams = false;
                }
            }
            else if (serialTemplate["p_structure"].ToString().ToLower().Equals("clientcode,date"))
            {
                if (printParamData[serialTemplate["p_clientcode"].ToString()] == null
                     || string.IsNullOrEmpty(printParamData[serialTemplate["p_clientcode"].ToString()].ToString())
                     || printParamData[serialTemplate["p_date"].ToString()] == null
                     || string.IsNullOrEmpty(printParamData[serialTemplate["p_date"].ToString()].ToString())
                     )
                {
                    checkSerialParams = false;
                }
            }
            else if (serialTemplate["p_structure"].ToString().ToLower().Equals("cycle,date"))
            {
                if (printParamData[serialTemplate["p_cycle"].ToString()] == null
                     || string.IsNullOrEmpty(printParamData[serialTemplate["p_cycle"].ToString()].ToString())
                     || printParamData[serialTemplate["p_date"].ToString()] == null
                     || string.IsNullOrEmpty(printParamData[serialTemplate["p_date"].ToString()].ToString())
                     )
                {
                    checkSerialParams = false;
                }
            }
            else if (serialTemplate["p_structure"].ToString().ToLower().Equals("clientcode,cycle,date"))
            {
                if (printParamData[serialTemplate["p_clientcode"].ToString()] == null
                    || string.IsNullOrEmpty(printParamData[serialTemplate["p_clientcode"].ToString()].ToString())
                    || printParamData[serialTemplate["p_cycle"].ToString()] == null
                    || string.IsNullOrEmpty(printParamData[serialTemplate["p_cycle"].ToString()].ToString())
                    || printParamData[serialTemplate["p_date"].ToString()] == null
                    || string.IsNullOrEmpty(printParamData[serialTemplate["p_date"].ToString()].ToString())
                    )
                {
                    checkSerialParams = false;
                }
            }
            else
            {
                if (serialTemplate["p_structure"].ToString() == null
                    || string.IsNullOrEmpty(serialTemplate["p_structure"].ToString().ToString()))
                {
                    checkSerialParams = false;
                }
            }

            return checkSerialParams;
        }

        public PrintMessage ReFreshTemplate(LabelCustomer customer)
        {
            PrintMessage prtMsg = new PrintMessage();
            string localBtwPath = Application.StartupPath + @"\Label\btw\";
            string downTemplateFileName = HttpMethods.HttpDownload(ConfigurationManager.AppSettings["MESServerUrl"] + customer.LabelTemplateUrl, localBtwPath);
            if (string.IsNullOrEmpty(downTemplateFileName))
            {
                prtMsg.Result = false;
                prtMsg.Message = "模板文件不存在";
                return prtMsg;
            }
            PackageTempletLabel pkgTmpLabel = new PackageTempletLabel();
            pkgTmpLabel.LabelImageFolderPath = Application.StartupPath + @"\Label\exp\"; //Server.MapPath("~/Label/exp/"); //标签存放本地文件夹路径
            pkgTmpLabel.TempletUrl = localBtwPath + downTemplateFileName; //Server.MapPath("~" + customer.LabelTemplateUrl);// + "/Label/btw/" + "boshuo.btw";//标签本地模块路径

            List<LabelParam> labelList = null;
            using (Engine btEngine = new Engine(true))
            {
                pkgTmpLabel.btEngine = btEngine;
                labelList = pkgTmpLabel.GetPrintTemplateParams();
            }

            //保存打印记录
            string url = "/LabelCustomer/RefreshTemplate";
            string returnMsg = string.Empty;
            JObject postBody = new JObject();
            postBody.Add("CustomerId", customer.Id);
            postBody.Add("TemplateContent", JsonConvert.SerializeObject(labelList));
            string body = JsonConvert.SerializeObject(postBody);
            returnMsg = AjaxHelper.ClientRequest(url, body);
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (!msgObj.IsSuccess)
            {
                prtMsg.Result = false;
                prtMsg.Message = "刷新失败";
                return prtMsg;
            }
            prtMsg.Result = true;
            prtMsg.Message = "刷新成功";
            return prtMsg;
        }

        public PrintMessage RePrintBar(string printName, LabelCustomer customer, List<PackageLabel> labelDataDetail, bool isPreView = false)
        {
            PrintMessage prtMsg = new PrintMessage();
            string localBtwPath = Application.StartupPath + @"\Label\btw\";
            string downTemplateFileName = HttpMethods.HttpDownload(ConfigurationManager.AppSettings["MESServerUrl"] + customer.LabelTemplateUrl, localBtwPath);
            if (string.IsNullOrEmpty(downTemplateFileName))
            {
                prtMsg.Result = false;
                prtMsg.Message = "模板文件不存在";
                return prtMsg;
            }

            PackageTempletLabel pkgTmpLabel = new PackageTempletLabel();
            pkgTmpLabel.Width = customer.LabelWidth;//标签宽度
            pkgTmpLabel.Height = customer.LabelHeight;//标签高度
            pkgTmpLabel.LabelImageFolderPath = Application.StartupPath + @"\Label\exp\"; //Server.MapPath("~/Label/exp/"); //标签存放本地文件夹路径
            pkgTmpLabel.TempletUrl = localBtwPath + downTemplateFileName; //Server.MapPath("~" + customer.LabelTemplateUrl);// + "/Label/btw/" + "boshuo.btw";//标签本地模块路径
            pkgTmpLabel.PrintNumber = 1;//本次答应张数
            pkgTmpLabel.ImageFileCode = "";//标签关键编码，一般存工单号；
            pkgTmpLabel.PrinterName = printName;

            //组装标签模板的烧录内容
            using (Engine btEngine = new Engine(true))
            {
                pkgTmpLabel.btEngine = btEngine;
                pkgTmpLabel.PrintLabels= labelDataDetail;
                prtMsg = pkgTmpLabel.Print();
                prtMsg.TemplateColList = pkgTmpLabel.PrintLabels;
            }
            return prtMsg;
        }

        public PrintMessage PrintBarFixData(string printName, LabelCustomer customer, JObject dataObj,string serialNumber)
        {
            PrintMessage prtMsg = new PrintMessage();
            int printCount = Convert.ToInt32(dataObj["printNum"].ToString());
            string Workcode = dataObj["lbl_WorkCodeList"].ToString();

            string localBtwPath = Application.StartupPath + @"\Label\btw\";
            string downTemplateFileName = HttpMethods.HttpDownload(ConfigurationManager.AppSettings["MESServerUrl"] + customer.LabelTemplateUrl, localBtwPath);
            if (string.IsNullOrEmpty(downTemplateFileName))
            {
                prtMsg.Result = false;
                prtMsg.Message = "模板文件不存在";
                return prtMsg;
            }
            PackageTempletLabel pkgTmpLabel = new PackageTempletLabel();
            pkgTmpLabel.Width = customer.LabelWidth;//标签宽度
            pkgTmpLabel.Height = customer.LabelHeight;//标签高度
            pkgTmpLabel.LabelImageFolderPath = Application.StartupPath + @"\Label\exp\"; //Server.MapPath("~/Label/exp/"); //标签存放本地文件夹路径
            pkgTmpLabel.TempletUrl = localBtwPath + downTemplateFileName; //Server.MapPath("~" + customer.LabelTemplateUrl);// + "/Label/btw/" + "boshuo.btw";//标签本地模块路径
            pkgTmpLabel.PrintNumber = printCount;//本次答应张数
            pkgTmpLabel.ImageFileCode = Workcode;//标签关键编码，一般存工单号；
            pkgTmpLabel.PrinterName = printName;

            PackageLabel pkgLabel = null;
            LabelParam lblParam = null;

            List<LabelParam> labelList = null;
            //组装标签模板的烧录内容
            using (Engine btEngine = new Engine(true))
            {
                pkgTmpLabel.btEngine = btEngine;

                labelList = pkgTmpLabel.GetPrintTemplateParams();
                int labelParamCount = labelList.Count;

                pkgLabel = new PackageLabel();
                int reprintIndex = 0;
                for (int k = 0; k < labelParamCount; k++)
                {
                    lblParam = labelList[k];
                    if (lblParam.Name.StartsWith("srl_") || lblParam.Name.StartsWith("qr_"))
                    {
                        string[] serialArray = dataObj[lblParam.Name].ToString().Split('●');
                        if (lblParam.Name.StartsWith("srl_"))
                        {
                            for (int i = 0; i < serialArray.Length; i++)
                            {
                                if (serialNumber == serialArray[i])
                                {
                                    reprintIndex = i;
                                    break;
                                }
                            }
                        }
                        pkgLabel.ParamValues.Add(lblParam.Name, dataObj[lblParam.Name] == null ? "" : serialArray[reprintIndex]);
                        //lblParam.Value = dataObj[lblParam.Name] == null ? "" : serialArray[i];
                    }
                    else
                    {
                        pkgLabel.ParamValues.Add(lblParam.Name, dataObj[lblParam.Name] == null ? "" : dataObj[lblParam.Name].ToString());
                        //lblParam.Value = dataObj[lblParam.Name].ToString();
                    }
                }

                pkgTmpLabel.PrintLabels.Add(pkgLabel);
                //打印生成标签图片；
                prtMsg = pkgTmpLabel.Print();
                prtMsg.TemplateColList = pkgTmpLabel.PrintLabels;
            }
            return prtMsg;
        }

        public PrintMessage PrintBar(string printName, List<LabelParam> labelParamList, LabelCustomer customer, JObject dataObj, bool isPreView = false)
        {
            PrintMessage prtMsg = new PrintMessage();
            int printCount = Convert.ToInt32(dataObj["printNum"].ToString());
            string Workcode = dataObj["lbl_WorkCodeList"].ToString();

            string localBtwPath = Application.StartupPath + @"\Label\btw\";
            string downTemplateFileName = HttpMethods.HttpDownload(ConfigurationManager.AppSettings["MESServerUrl"] + customer.LabelTemplateUrl, localBtwPath);
            if (string.IsNullOrEmpty(downTemplateFileName))
            {
                prtMsg.Result = false;
                prtMsg.Message = "模板文件不存在";
                return prtMsg;
            }
            PackageTempletLabel pkgTmpLabel = new PackageTempletLabel();
            pkgTmpLabel.Width = customer.LabelWidth;//标签宽度
            pkgTmpLabel.Height = customer.LabelHeight;//标签高度
            pkgTmpLabel.LabelImageFolderPath = Application.StartupPath + @"\Label\exp\"; //Server.MapPath("~/Label/exp/"); //标签存放本地文件夹路径
            pkgTmpLabel.TempletUrl = localBtwPath + downTemplateFileName; //Server.MapPath("~" + customer.LabelTemplateUrl);// + "/Label/btw/" + "boshuo.btw";//标签本地模块路径
            pkgTmpLabel.PrintNumber = printCount;//本次打印张数
            pkgTmpLabel.ImageFileCode = Workcode;//标签关键编码，一般存工单号；
            pkgTmpLabel.PrinterName = printName;

            PackageLabel pkgLabel = null;
            LabelParam lblParam = null;

            //组装标签模板的烧录内容
            using (Engine btEngine = new Engine(true))
            {
                pkgTmpLabel.btEngine = btEngine;
                int labelParamCount = labelParamList.Count;
                for (int i = 0; i < printCount; i++)
                {
                    pkgLabel = new PackageLabel();
                    for (int k = 0; k < labelParamCount; k++)
                    {
                        lblParam = labelParamList[k];
                        //模板里自带默认值，直接采用，无需再设定值；
                        if (!string.IsNullOrEmpty(lblParam.Value)) continue;

                        if (dataObj[lblParam.Name].ToString().IndexOf('●') > -1)
                        {
                            string[] serialArray = dataObj[lblParam.Name].ToString().Split('●');
                            pkgLabel.ParamValues.Add(lblParam.Name, dataObj[lblParam.Name] == null ? "" : serialArray[i]);
                        }
                        //if (lblParam.Name.StartsWith("srl_")|| lblParam.Name.StartsWith("qr_"))
                        //{
                        //    string [] serialArray=dataObj[lblParam.Name].ToString().Split('●');
                        //    pkgLabel.ParamValues.Add(lblParam.Name, dataObj[lblParam.Name] == null ? "" : serialArray[i]);
                            
                        //}
                        else
                        {
                            
                            pkgLabel.ParamValues.Add(lblParam.Name, dataObj[lblParam.Name] == null ? "" : dataObj[lblParam.Name].ToString());
                        }
                    }
                    pkgTmpLabel.PrintLabels.Add(pkgLabel);
                }

                //打印生成标签图片；
                prtMsg = pkgTmpLabel.Print();
                prtMsg.TemplateColList = pkgTmpLabel.PrintLabels;
            }
            return prtMsg;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Search_Click(object sender, EventArgs e)
        {
            JObject searCondition = new JObject(); 
            searCondition.Add("PageSize", "100");
            searCondition.Add("PageNum", 1);
            searCondition.Add("CustCode", this.txt_CustCode.Text);
            searCondition.Add("CustTitle", this.txt_CustTitle.Text);
            DictionaryEntry selectItem =(DictionaryEntry) this.comb_lblcategory.SelectedItem;
            searCondition.Add("TemplateType", selectItem.Key.ToString());
            searCondition.Add("TreeSelect", "");
            loadLabelCustomerData(searCondition);
        }

        private void gdv_customer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //当选择的打印标签类型为 box 盒装标签时
            string templateType= this.gdv_customer.Rows[e.RowIndex].Cells["TemplateTypeCode"].Value.ToString();
            string keyColumn = this.gdv_customer.Rows[e.RowIndex].Cells["Id"].Value.ToString();
            string mappingChildId= this.gdv_customer.Rows[e.RowIndex].Cells["MappingChildId"].Value.ToString();
            if ("box".Equals(templateType))
            {
                CustomerId = keyColumn;
                tabControl1.SelectedIndex = 1;
                MappedChildId = "";
            }
            else if ("inner".Equals(templateType))
            {
                //if (string.IsNullOrEmpty(mappingChildId))
                //{
                //    MessageBox.Show("请关联子标签");
                //    return;
                //}
                CustomerId = keyColumn;
                MappedChildId = "inner";
                tabControl1.SelectedIndex = 2;
            }
            else if ("outter".Equals(templateType))
            {
                //if (string.IsNullOrEmpty(mappingChildId))
                //{
                //    MessageBox.Show("请关联子标签");
                //    return;
                //}
                CustomerId = keyColumn;
                MappedChildId = "outter";
                tabControl1.SelectedIndex = 2;
            }

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex==1)
            {
                if (string.IsNullOrEmpty(CustomerId))
                {
                    MessageBox.Show("请双击标签客户信息进入该TAB页");
                    tabControl1.SelectedIndex = 0;
                    return;
                }
                if (!string.IsNullOrEmpty(MappedChildId))
                {
                    MessageBox.Show("请客户标签只支持外箱打印");
                    tabControl1.SelectedIndex = 0;
                    MappedChildId = "";
                    return;
                }
                this.dtpk_begindate.Value =Convert.ToDateTime( DateTime.Now.ToString("yyyy-MM")+"-01");

                //开始加载标签数据
                this.txt_workcode.Focus();
                loadPrinter(this.comb_printlist);
                loadMappingProgramIdComboxData(this.comb_MappingProgramId);
                loadExInputControl(CustomerId, this.comb_packagetype,this.lbl_lblsize, this.grpbox_otherInput);
                loadPackageQtyComboxData(this.comb_packageQty,custObj);
                loadPackageTypeComboxData(this.comb_packagetype);
                loadPrintRecordStateComboxData(this.comb_recordState);
                loadPrintStateComboxData(this.comb_printState); 
                //加载下拉框的默认值
                this.comb_packagetype.SelectedItem = new DictionaryEntry(custObj.TemplateType, GetTemplateType(custObj.TemplateType));

                DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_recordState.SelectedItem;
                DictionaryEntry printStateSelItem = (DictionaryEntry)this.comb_printState.SelectedItem;
                //加载打印记录
                loadPrintRecord(this.gdvLblRecord, CustomerId, this.txt_workcode.Text, recordStateSelItem.Key.ToString(), printStateSelItem.Key.ToString(),this.dtpk_begindate.Value, this.dtpk_enddate.Value, 0);
            }
            else if (e.TabPageIndex == 2)
            {
                if (string.IsNullOrEmpty(MappedChildId))
                {
                    MessageBox.Show("请双击标签客户信息进入该TAB页");
                    tabControl1.SelectedIndex = 0;
                    return;
                }
                this.dtpk_outter_begindate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM") + "-01");
                //开始加载标签数据
                loadPrinter(this.comb_outter_printer);
                loadExInputControl(CustomerId, this.combo_outter_packageType, this.lbl_outter_lblSize, this.grpbox_outter);
                loadPackageTypeComboxData(this.combo_outter_packageType);
                loadPackageQtyComboxData(this.combo_outter_packQty,custObj);

                InitOuterDGV();
                loadPrintRecordStateComboxData(this.comb_outter_printrecordState);
                loadPrintStateComboxData(this.comb_outter_printState); 
                //加载下拉框的默认值
                this.combo_outter_packageType.SelectedItem = new DictionaryEntry(custObj.TemplateType, GetTemplateType(custObj.TemplateType));
                DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_outter_printrecordState.SelectedItem;
                DictionaryEntry printStateSelItem = (DictionaryEntry)this.comb_outter_printState.SelectedItem;
                //初始化dgv
                //this.dgv_outter_printRecordList
                //加载打印记录
                loadPrintRecord(this.dgv_outter_printRecordList, custObj.Id, this.txt_outter_search_workcode.Text, recordStateSelItem.Key.ToString(), printStateSelItem.Key.ToString(),this.dtpk_outter_begindate.Value, this.dtpk_outter_enddate.Value, 1,true);
            }
            else 
            {
                CustomerId = string.Empty;
                MappedChildId = string.Empty;
                custObj = new LabelCustomer();
            }
        }

        private void InitOuterDGV()
        {
            dgv_outter_printdetail.Rows.Clear();
            DataGridViewColumn dgvCol = new DataGridViewColumn();
            DataGridViewCell dgvCell = new DataGridViewTextBoxCell();
            dgvCol.Name = "Id";
            dgvCol.HeaderText = "Id";
            dgvCol.Visible = false;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "Cycle";
            dgvCol.HeaderText = "周期";
            dgvCol.Visible = false;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "Workcode";
            dgvCol.HeaderText = "工单";
            dgvCol.Visible = true;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "PackageQty";
            dgvCol.Width = 90;
            dgvCol.HeaderText = "标签数量";
            dgvCol.Visible = true;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "PackageType";
            dgvCol.HeaderText = "包装类型";
            dgvCol.Visible = true;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "SerialNumber";
            dgvCol.HeaderText = "流水号";
            dgvCol.Visible = true;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "PrintState";
            dgvCol.HeaderText = "打印状态";
            dgvCol.Visible = true;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
            dgvCol = new DataGridViewColumn();
            dgvCol.Name = "Descriplion";
            dgvCol.HeaderText = "子标签数据";
            dgvCol.Visible = false;
            dgvCol.CellTemplate = dgvCell;
            dgv_outter_printdetail.Columns.Add(dgvCol);
        }

        private void loadPrintRecord(DataGridView dgvObj, string customerId,string workcode,string recordState,string printedState,DateTime beginDate, DateTime endDate,int OnlyParent,bool IsOutter=false)
        {
            dgvObj.Rows.Clear();

            //根据输入单号，获取工单信息
            string url = "/LabelPrint/GetPrintRecord";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("CustomerId", customerId);
            reqData.Add("Workcode", workcode);
            reqData.Add("IsMapped", recordState);
            reqData.Add("IsPrinted", printedState);
            reqData.Add("BeginDate", beginDate);
            reqData.Add("EndDate", endDate);
            reqData.Add("OnlyParent", OnlyParent);//只显示父标签；未打印的；
            reqData.Add("PageSize", 100);
            reqData.Add("PageNum", 1);
            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);

            //{"IsSuccess":true,"Data":"{\"id\":\"726243\",\"cycle\":\"945Z\",\"workcode\":\"W-19111693-01-02\",\"enableprintqty\":\"2000\",\"printedqty\":\"0\",\"printedpages\":\"0\",\"goodqty\":\"2000\"}","Messaage":""}
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (msgObj.IsSuccess)
            {
                JArray printRecordList = JsonConvert.DeserializeObject<JArray>(msgObj.Data.ToString());
                JObject itemRecord = null;
                int cellIndex = 0;
                int rowIndex = 0;
                foreach (var item in printRecordList)
                {
                    cellIndex = 0;
                    itemRecord = (JObject)item;
                    DataGridViewRow viewRow = new DataGridViewRow();
                    viewRow.CreateCells(dgvObj);
                    viewRow.Cells[cellIndex++].Value = (rowIndex+1);
                    viewRow.Cells[cellIndex++].Value = itemRecord["PrintRecordId"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRCustomerId"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRWorkCode"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRTagQty"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRCycle"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRClientCode"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRSerialNumber"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRIsMapped"].ToString()=="0"?"未绑定":"已绑定";
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRIsPrinted"].ToString() == "0" ? "未打印" : "已打印";
                    viewRow.Cells[cellIndex++].Value = GetPRAuditOper(itemRecord["PRAuditOper"].ToString()); 
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRCreator"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRCreateTime"];
                    if (IsOutter)
                    {
                        viewRow.Cells[cellIndex++].Value = itemRecord["PRChecked"].ToString()=="0"?"未核对":"已核对";
                        viewRow.Cells[cellIndex++].Value = itemRecord["PRChecker"];
                        viewRow.Cells[cellIndex++].Value = itemRecord["PRCheckTime"];
                    }
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRQRCodeMd5Val"];
                    rowIndex++;
                    dgvObj.Rows.Add(viewRow);
                }
            }

        }

        private object GetPRAuditOper(string v)
        {
            string tempString = string.Empty;
            switch(v)
            {
                case "checking":
                    tempString = "申请中";
                    break;
                case "reprint":
                    tempString = "可补打";
                    break;
                default:
                    tempString = "";
                    break;
            }
            return tempString;
        }

        private void btn_SearchRecord_Click(object sender, EventArgs e)
        {
            DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_recordState.SelectedItem;
            DictionaryEntry printStateSelItem = (DictionaryEntry)this.comb_printState.SelectedItem;
            //加载打印记录
            loadPrintRecord(this.gdvLblRecord, CustomerId, this.txt_search_workcode.Text, recordStateSelItem.Key.ToString(), printStateSelItem.Key.ToString(),this.dtpk_begindate.Value, this.dtpk_enddate.Value, 0) ;
        }

        private void chk_merginprint_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.chk_merginprint.Checked)
            {
                this.txt_printNum.Text = "1";
                //this.txt_maxPrintNum.Text = "1";
            }
        }

        /// <summary>
        /// 工单显示列表 删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_workprintdetail_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string workcode = this.dgv_workprintdetail.Rows[e.Row.Index].Cells[1].Value.ToString();
            if (isBeforeOperation)
            {
                enablePrintWorkcode.Clear();
                chklist_workcode.Items.Clear();
            }
            else
            {
                enablePrintWorkcode.Remove(workcode);
                chklist_workcode.Items.Remove(workcode);
            }
            CalcMaxPrintNumer();
        }

        /// <summary>
        /// 计算可打印数量
        /// </summary>
        private void CalcMaxPrintNumer()
        {
            int maxPrintNum = 0;
            int totalEnableQty = 0;
            JObject workcodeObj = null;

            if (isBeforeOperation)
            {
                //var selected = this.dgv_workprintdetail.SelectedRows;
                //if (selected.Count > 0)
                //{
                //    for (var i = 0; i < this.dgv_workprintdetail.Rows.Count; i++)
                //    {
                //        totalEnableQty = totalEnableQty + Convert.ToInt32(this.dgv_workprintdetail.Rows[i].Cells["enableprintqty"].Value.ToString());

                //        if (selected[0].Cells["workcode"].Value.ToString().Equals(this.dgv_workprintdetail.Rows[i].Cells["workcode"].Value.ToString()))
                //        {
                //            break;
                //        }
                //    }
                //}

                for (var i = 0; i < this.dgv_workprintdetail.Rows.Count; i++)
                {
                    totalEnableQty = totalEnableQty + Convert.ToInt32(this.dgv_workprintdetail.Rows[i].Cells["enableprintqty"].Value.ToString());

                    if (i==selectRowIndex)
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in enablePrintWorkcode)
                {
                    workcodeObj = item.Value;
                    totalEnableQty = totalEnableQty + Convert.ToInt32(string.IsNullOrEmpty(workcodeObj["enablePrintQty"].ToString()) ? "0" : workcodeObj["enablePrintQty"].ToString());
                }
            }

            maxPrintNum = totalEnableQty / Convert.ToInt32(this.comb_packageQty.SelectedItem.ToString());
            this.txt_maxPrintNum.Text = maxPrintNum.ToString();
            if (isBeforeOperation)
                this.txt_printNum.Text = maxPrintNum.ToString();
        }

        /// <summary>
        /// 工单显示列表 行点击事件 自动定位到当前该打印的工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_workprintdetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var selected = this.dgv_workprintdetail.SelectedRows;
                if (selected.Count > 0)
                {
                    for (var i = 0; i < this.dgv_workprintdetail.Rows.Count; i++)
                    {
                        if (selected[0].Cells["workcode"].Value.ToString().Equals(this.dgv_workprintdetail.Rows[i].Cells["workcode"].Value.ToString()))
                        {
                            if (i != selectRowIndex)
                            {
                                this.dgv_workprintdetail.ClearSelection();
                                this.dgv_workprintdetail.Rows[selectRowIndex].Selected = true;
                                this.dgv_workprintdetail.CurrentCell = this.dgv_workprintdetail.Rows[selectRowIndex].Cells[1];
                            }
                        }
                    }
                }
                CalcMaxPrintNumer();
            }
        }
        
        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_workprintdetail_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var selected = this.dgv_workprintdetail.SelectedRows;
                if (selected.Count > 0)
                {
                    for (var i = 0; i < this.dgv_workprintdetail.Rows.Count; i++)
                    {
                        if (selected[0].Cells["workcode"].Value.ToString().Equals(this.dgv_workprintdetail.Rows[i].Cells["workcode"].Value.ToString()))
                        {
                            if (i != selectRowIndex)
                            {
                                this.dgv_workprintdetail.ClearSelection();
                                this.dgv_workprintdetail.Rows[selectRowIndex].Selected = true;
                                this.dgv_workprintdetail.CurrentCell = this.dgv_workprintdetail.Rows[selectRowIndex].Cells[1];
                            }
                        }
                    }
                }
                CalcMaxPrintNumer();
            }
        }

        /// <summary>
        /// 外箱标签选取指定的盒装标签记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_selectedOK_Click(object sender, EventArgs e)
        {
            chkworkcodelist.Items.Clear();
            dgv_outter_printdetail.Rows.Clear();

            DataGridViewSelectedRowCollection selectRowCol = this.dgv_outter_printRecordList.SelectedRows;
            if (selectRowCol.Count <= 0|| selectRowCol.Count>1)
            {
                MessageBox.Show("请选择单张外箱标签");
                return;
            }
            DataGridViewRow selRow = selectRowCol[0];
            this.btn_outter_print.Enabled = true;
            if (selRow.Cells[10].Value.ToString() == "申请中")
            {
                MessageBox.Show("该标签存在未处理的申请，不能打印。");
                this.btn_outter_print.Enabled = false;
            }

            //当前选中外箱若已打印，则不能重复打印，只能选择破损补打；
            string PrintState = selRow.Cells[9].Value.ToString();//打印状态
            string PackageQty= selRow.Cells[4].Value.ToString();//外箱包装数量
            if (!this.combo_outter_packQty.Items.Contains(PackageQty))
            {
                this.combo_outter_packQty.Items.Add(PackageQty);
            }
            this.combo_outter_packQty.SelectedItem = PackageQty;

            if (PrintState == "已打印")
            {
                this.btn_outter_print.Enabled = false;
            }

           
            OutterRecordId = selRow.Cells[1].Value.ToString();
            //显示工单里加入选中的标签；查询外箱关联的内箱标签；
            string url = "/LabelPrint/GetParentPrintRecord";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            //reqData.Add("CustomerId", custObj.MappingChildId);
            //reqData.Add("LabelRecordId", selRow.Cells[1].Value.ToString());
            reqData.Add("ParentId", selRow.Cells[1].Value.ToString());//只显示子标签；未打印的；
            //reqData.Add("BoxType", selRow.Cells[1].Value.ToString());

            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);

            //{"IsSuccess":true,"Data":"{\"id\":\"726243\",\"cycle\":\"945Z\",\"workcode\":\"W-19111693-01-02\",\"enableprintqty\":\"2000\",\"printedqty\":\"0\",\"printedpages\":\"0\",\"goodqty\":\"2000\"}","Messaage":""}
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (msgObj.IsSuccess)
            {
                this.txt_outter_printNum.Text = "1";
                this.txt_outter_maxPrintNum.Text = "1"; 

                int cellIndex = 0;
                JArray childRecordArray= JsonConvert.DeserializeObject<JArray>(msgObj.Data.ToString());
                DataGridViewRow viewRow = null;
                string PRWorkCode = string.Empty;
                string tempString = string.Empty;
                string[] workcodeArray;
                string[] cycleArray;
                foreach (var itemRecord in childRecordArray)
                {
                    cellIndex = 0;
                    //this.chk_outter_workcodeList.Items.Add(itemRecord["PRWorkCode"].ToString(), true);

                    viewRow = new DataGridViewRow();
                    viewRow.CreateCells(dgv_outter_printdetail);
                    //viewRow.Cells[cellIndex++].Value = itemRecord["PRCustomerId"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PrintRecordId"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRCycle"];
                    PRWorkCode = itemRecord["PRWorkCode"].ToString();
                    viewRow.Cells[cellIndex++].Value = PRWorkCode;
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRTagQty"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRBoxType"];

                    //viewRow.Cells[cellIndex++].Value = itemRecord["PRClientCode"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRSerialNumber"];
                    //viewRow.Cells[cellIndex++].Value = itemRecord["PRIsMapped"].ToString() == "0" ? "未绑定" : "已绑定";
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRIsPrinted"].ToString() == "0" ? "未打印" : "已打印";
                    //viewRow.Cells[cellIndex++].Value = itemRecord["PRCreator"];
                    //viewRow.Cells[cellIndex++].Value = itemRecord["PRCreateTime"];
                    viewRow.Cells[cellIndex++].Value = itemRecord["PRDescriplion"];
                    //viewRow.Cells[cellIndex++].Value = itemRecord["PRQRCodeMd5Val"];

                    workcodeArray = PRWorkCode.Split(',');
                    cycleArray = itemRecord["PRCycle"].ToString().Split(',');
                    tempString = string.Empty;
                    for (int i = 0; i < workcodeArray.Length; i++)
                    {
                        tempString = workcodeArray[i] + "," + cycleArray[0];
                        if (!chkworkcodelist.Items.Contains(tempString))
                        {
                            if (custObj.WorkcodeRelaceAllG == 1)
                            {
                                chkworkcodelist.Items.Add(tempString.Replace("-", ""));
                            }
                            else
                            {
                                chkworkcodelist.Items.Add(tempString);
                            }
                        }
                    }
                    
                    dgv_outter_printdetail.Rows.Add(viewRow);
                }

                //显示子标签其他输入的参数内容
                //GroupBox grpBox = grpbox_outter;
                //bool otherCtrlCheck = true;
                //JObject firstJObj =JsonConvert.DeserializeObject<JObject>(childRecordArray[0]["PRDescriplion"].ToString());
                //foreach (Control item in grpBox.Controls)
                //{
                //    if (item is TextBox)
                //    {
                //        TextBox t = (TextBox)item;
                //        if (!string.IsNullOrEmpty(t.Name)&& !string.IsNullOrEmpty(firstJObj[t.Name].ToString()))
                //        {
                //            t.Text = firstJObj[t.Name].ToString();
                //        }
                //    }
                //}
            }
            
        }

        /// <summary>
        /// 外箱标签查询功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_search_Click(object sender, EventArgs e)
        {
            DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_outter_printrecordState.SelectedItem;
            DictionaryEntry printStateSelItem = (DictionaryEntry)this.comb_outter_printState.SelectedItem;
            //加载打印记录
            loadPrintRecord(this.dgv_outter_printRecordList, custObj.Id, this.txt_outter_search_workcode.Text, recordStateSelItem.Key.ToString(), printStateSelItem.Key.ToString(),this.dtpk_outter_begindate.Value, this.dtpk_outter_enddate.Value, 1,true);
        }

        private void dgv_outter_printdetail_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string workcode = this.dgv_outter_printRecordList.Rows[e.Row.Index].Cells[2].Value.ToString();
            //chk_outter_workcodeList.Items.Remove(workcode);
        }

        private void btn_outter_print_Click(object sender, EventArgs e)
        {
            //打印处理，先获取或者控件输入值；
            //this.splitContainer3.Panel2.Controls
            object printNameTemp = this.comb_outter_printer.SelectedItem;
            object packageTypeTemp = this.combo_outter_packageType.SelectedItem;
            object packageQtyTemp = this.combo_outter_packQty.SelectedItem;
            bool mergin = false;
            string printNum = this.txt_outter_printNum.Text;
            string maxPrintNum = this.txt_outter_maxPrintNum.Text;

            #region 数据检查
            //string selectedItem = string.Empty;
            DataGridViewRowCollection selectRowCol = this.dgv_outter_printdetail.Rows;
            if (selectRowCol.Count<=0)
            {
                MessageBox.Show("请先加载外箱标签。");
                return;
            }
            

            List<string> labelListWorkCode = new List<string>();
            List<string> labelListCycle = new List<string>();
            string labelId = string.Empty;
            DataGridViewRow firstRow = null; // PRDescriplion
            foreach (DataGridViewRow item in selectRowCol)
            {
                if (firstRow == null) firstRow = item;
                break;
            }

            string tempString = string.Empty;
            string[] tempStringArray;
            for (int i = 0; i < chkworkcodelist.Items.Count; i++)
            {
                if (chkworkcodelist.GetItemChecked(i))
                {
                    tempString = chkworkcodelist.Items[i].ToString();
                    tempStringArray = tempString.Split(',');

                    if (custObj.WorkcodeRelaceAllG == 1)
                    {
                        if (!labelListWorkCode.Contains(tempStringArray[0]))
                        {
                            labelListWorkCode.Add(tempStringArray[0].Replace("-", ""));
                        }
                        
                    }
                    else
                    {
                        if (!labelListWorkCode.Contains(tempString))
                        {
                            labelListWorkCode.Add(tempStringArray[0]);
                        }
                    }

                    if (!labelListCycle.Contains(tempStringArray[1]))
                    {
                        labelListCycle.Add(tempStringArray[1]);
                    }
                }
            }

            if (custObj.LabelTemplateColString == null || custObj.LabelTemplateColString == "null")
            {
                MessageBox.Show("请点击更新模板");
                return;
            }

            if (printNameTemp == null)
            {
                MessageBox.Show("请选择打印机");
                return;
            }
            if (packageTypeTemp == null)
            {
                MessageBox.Show("请选择包装类型");
                return;
            }
            if (packageQtyTemp == null)
            {
                MessageBox.Show("请选择包装数量");
                return;
            }

            if (custObj.LabelShowOneWorkcode == 1)
            {
                if (labelListWorkCode.Count > 1)
                {
                    MessageBox.Show("该标签上只能显示一个工单号");
                    return;
                }
            }
            
            if (labelListWorkCode.Count<=0)
            {
                MessageBox.Show("请选择显示在标签上的工单");
                return;
            }

            if (printNum == null || string.IsNullOrEmpty(printNum))
            {
                MessageBox.Show("请输入打印数量");
                return;
            }

            if (Convert.ToInt32(printNum) > Convert.ToInt32(string.IsNullOrEmpty(maxPrintNum) ? "0" : maxPrintNum))
            {
                MessageBox.Show("打印张数不能大于最大允许打印张数");
                return;
            }
            #endregion

            #region 组装标签数据参数
            JObject printParamData = new JObject();
            //以第一个子标签作为外标签的大部分参数初始化外部标签；
            printParamData=JsonConvert.DeserializeObject<JObject>(firstRow.Cells[7].Value.ToString());
            printParamData = JsonConvert.DeserializeObject<JObject>(printParamData["ParamValues"].ToString());
            //组装其他输入的参数
            bool otherCtrlCheck = GetOtherInput(printParamData, this.grpbox_outter);
            if (!otherCtrlCheck)
            {
                return;
            }

            //重写外包装特有的数据
            printParamData["merginPrint"]=false;
            //JArray merginData = GetMerginData(packageQtyTemp);
            printParamData["merginData"]= "[]";
            DictionaryEntry packageType = (DictionaryEntry)packageTypeTemp;
            printParamData["packageType"] = packageType.Key.ToString();
            printParamData["lbl_PackageQty"] = packageQtyTemp.ToString();
            //最底层子标签中最大标签数量
            printParamData["lbl_MinPackageQty"] = GetMinPackageQty(OutterRecordId); // printParamData["lbl_PackageQty"].ToString(); 
            printParamData["lbl_WorkCodeList"] = string.Join(",", labelListWorkCode);
            printParamData["lbl_Cycle"] = string.Join(",", labelListCycle);
            printParamData["printNum"] = printNum.ToString();
            printParamData["Updator"] = Program.UserNo;
            printParamData["printNum"] = printNum.ToString();
            //获取模板里的默认值
            List<LabelParam> labelParamList = GetDefaultValueFromTemplate(printParamData, custObj);
            //刷新标签里与工单无关的计算列属性值；
            RefreshSystemCalcColumnData(printParamData);

            //组装配置的动态组装数据和流水号计算数据，需要连接服务器生成流水号或者序列号，可能需要包含在二维码里；
            int printCount = Convert.ToInt32(printNum);
            bool checkSerialParams = GetSerialNumber(printParamData, printCount, packageType.Key.ToString());
            if (!checkSerialParams)
            {
                MessageBox.Show("流水号规则配置必要参数为空");
                return;
            }
            //组装字段
            GetLabelCalcColumnData(printParamData, printCount, packageType.Key.ToString());
            //给组装字段补上bar_标签；
            InitBarColumnData(printParamData);
            //补上打印日期字段；
            if (printParamData.Property("lbl_PrintDate") == null)
            {
                printParamData["lbl_PrintDate"] = DateTime.Now.ToString("yyyy-MM-dd");
            }
            #endregion


            DialogResult result = ConfirmYesNo("选择打印机为："+ printNameTemp.ToString() + ",请确认打印机设备纸张尺寸为:" + custObj.LabelWidth + "*" + custObj.LabelHeight);
            if (result == DialogResult.No)
            {
                return;
            }

            PrintMessage prtMsg = PrintBar(printNameTemp.ToString(), labelParamList,custObj, printParamData, false);
            if (!prtMsg.Result)
            {
                MessageBox.Show(prtMsg.Message);
            }
            else
            {
                //外箱打印标签时更新打印记录
                string url = "/LabelPrint/UpdateParentLabelRecord";
                string returnMsg = string.Empty;
                JObject postData = new JObject();
                postData.Add("sysRecordId", OutterRecordId);
                if (printParamData.Property("md5ValPrimary") != null)
                {
                    postData.Add("md5ValPrimary", printParamData["md5ValPrimary"].ToString());
                }
                postData.Add("srl_SerialNumber", printParamData["srl_SerialNumber"].ToString());
                postData.Add("Descriplion", JsonConvert.SerializeObject(prtMsg.TemplateColList[0]));
                postData.Add("Updator", Program.UserNo);

                string body = JsonConvert.SerializeObject(postData);
                returnMsg = AjaxHelper.ClientRequest(url, body);
                ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
                if (!msgObj.IsSuccess)
                {
                    MessageBox.Show(msgObj.Messaage);
                }
                else
                {
                    MessageBox.Show("打印成功，打印记录已生成。");
                    this.dgv_outter_printdetail.Rows.Clear();
                    this.btn_outter_search_Click(sender,e);
                    //加载打印记录
                    //DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_outter_printrecordState.SelectedItem;
                    //DictionaryEntry printStateSelItem = (DictionaryEntry)this.comb_outter_printState.SelectedItem;
                    //loadPrintRecord(this.dgv_outter_printRecordList, CustomerId, this.txt_outter_search_workcode.Text, recordStateSelItem.Key.ToString(), printStateSelItem.Key.ToString(),this.dtpk_outter_begindate.Value, this.dtpk_outter_enddate.Value, 1, true);
                }
            }
        }

        /// <summary>
        /// 最底层子标签中最大标签数量
        /// </summary>
        /// <param name="outterRecordId"></param>
        /// <returns></returns>
        private int GetMinPackageQty(string outterRecordId)
        {
            int returnMsg =0;
            string url = "/LabelPrint/GetMinPackageQty";
            JObject reqData = new JObject();
            reqData.Add("outterRecordId", outterRecordId);
            string body = JsonConvert.SerializeObject(reqData);
            string returnTemp = AjaxHelper.ClientRequest(url, body);
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnTemp);
            if (msgObj.IsSuccess)
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(msgObj.Data.ToString());
                returnMsg = Convert.ToInt32(data["Data"][0][0]["maxQty"].ToString());
            }
            return returnMsg;
        }

        private void RefreshSystemCalcColumnData(JObject printParamData)
        {
            string url = "/LabelPrint/GetSystemPrintInfo";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("CustomerId", CustomerId);
            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (msgObj.IsSuccess)
            {
                JObject systemCalcData = JsonConvert.DeserializeObject<JObject>(msgObj.Data.ToString());

                List<LabelColumn> colListTemp = colList.FindAll(col => col.Inputtype == 0 && col.LinkWorkcode == 0);
                foreach (var item in colListTemp)
                {
                    if (printParamData.Property(item.MappingTempColCode) != null)
                    {
                        printParamData[item.MappingTempColCode] = systemCalcData[item.MappingTempColCode];
                    }
                    else
                    {
                        printParamData.Add(item.MappingTempColCode, systemCalcData[item.MappingTempColCode]);
                    }
                }
            }
        }

        private void btnUpdateTemplate_Click(object sender, EventArgs e)
        {
            PrintMessage pMsg= ReFreshTemplate(custObj);
            MessageBox.Show(pMsg.Message);
            tabControl1.SelectedIndex = 0;
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblPrintFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        
        /// <summary>
        /// 内箱作废申请 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_disable_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectRowCol = this.gdvLblRecord.SelectedRows;
            if (selectRowCol.Count <= 0)
            {
                MessageBox.Show("请选择报废的标签记录");
                return;
            }
            List<string> printRecordList = new List<string>();
            foreach (DataGridViewRow selRow in selectRowCol)
            {
                printRecordList.Add(selRow.Cells[1].Value.ToString());
            }

            DialogResult btnResult = MessageBox.Show("确定要作废选中的标签吗?", "提示", MessageBoxButtons.OKCancel);
            if (btnResult == DialogResult.OK)
            {
                //加载打印记录
                DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_recordState.SelectedItem;
                bool result=DisablePrintRecord(printRecordList);
                if (result)
                {
                    MessageBox.Show("操作成功.");
                    //加载打印记录
                    btn_SearchRecord_Click(sender,e);
                }
            }
        }
        
        /// <summary>
        /// 内箱破损补打 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_reprint_Click(object sender, EventArgs e)
        {
            bool result= RePrint(this.gdvLblRecord, this.comb_printlist);
            if (result)
            {
                this.btn_SearchRecord_Click(sender, e);
            }
        }

        /// <summary>
        /// 补打
        /// </summary>
        /// <param name="dgvObj"></param>
        /// <param name="combObj"></param>
        /// <returns></returns>
        private bool RePrint(DataGridView dgvObj,ComboBox combObj)
        {
            object printNameTemp = combObj.SelectedItem;
            if (printNameTemp == null)
            {
                MessageBox.Show("请选择打印机");
                return false;
            }
            DataGridViewSelectedRowCollection selectRowCol = dgvObj.SelectedRows;
            if (selectRowCol.Count <= 0)
            {
                MessageBox.Show("请选择一条破损补打的标签记录");
                return false;
            }
            DataGridViewRow selRow = selectRowCol[0];

            if (selRow.Cells[9].Value.ToString() == "未打印")
            {
                MessageBox.Show("未打印状态不能破损补打");
                return false;
            }
            if (selRow.Cells[10].Value.ToString() == "申请中")
            {
                MessageBox.Show("申请审核中不能破损补打");
                return false;
            }
            if (selRow.Cells[10].Value.ToString() != "可补打")
            {
                MessageBox.Show("该标签未完成审批流程，不能补打。");
                return false;
            }
            //该标签还未打印，不能破损补打；
            DialogResult btnResult = MessageBox.Show("确定要破损补打该标签吗?", "提示", MessageBoxButtons.OKCancel);
            if (btnResult == DialogResult.OK)
            {
                string url = "/LabelPrint/RePrint";
                string returnMsg = string.Empty;
                JObject reqData = new JObject();
                reqData.Add("LabelRecordId", selRow.Cells[1].Value.ToString());
                reqData.Add("Updator", Program.UserNo);
                string body = JsonConvert.SerializeObject(reqData);
                returnMsg = AjaxHelper.ClientRequest(url, body);
                ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
                if (!msgObj.IsSuccess)
                {
                    MessageBox.Show("查询出现异常.");
                    return false;
                }

                JObject respData = JsonConvert.DeserializeObject<JObject>(msgObj.Data.ToString());

                PackageLabel labelDataDetail = JsonConvert.DeserializeObject<PackageLabel>(respData["Descriplion"].ToString());
                List<PackageLabel> labelDataDetailList = new List<PackageLabel>();
                labelDataDetailList.Add(labelDataDetail);
                if (labelDataDetailList.Count == 0)
                {
                    MessageBox.Show("该标签出现异常。");
                    return false;
                }

                PrintMessage prtMsg = RePrintBar(printNameTemp.ToString(), custObj, labelDataDetailList, false);
                //if (respData["Descriplion"] == null)
                //{
                //    return false;
                //}
                //PrintMessage prtMsg = PrintBarFixData(printNameTemp.ToString(), custObj, JsonConvert.DeserializeObject<JObject>(respData["Descriplion"].ToString()), selRow.Cells[6].Value.ToString());
                if (!prtMsg.Result)
                {
                    MessageBox.Show(prtMsg.Message);
                    return false;
                }
                else
                {
                    //保存补打打印日志；
                    //补打完成后，需更新状态；
                    url = "/LabelPrint/UpdateRePrint";
                    returnMsg = string.Empty;
                    reqData = new JObject();
                    reqData.Add("LabelRecordId", selRow.Cells[1].Value.ToString());
                    reqData.Add("Creator", Program.UserNo);
                    reqData.Add("Remark", "标签破损补打");
                    reqData.Add("LabelData", JsonConvert.SerializeObject(prtMsg.TemplateColList[0]));
                    body = JsonConvert.SerializeObject(reqData);
                    returnMsg = AjaxHelper.ClientRequest(url, body);
                    msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
                    if (!msgObj.IsSuccess)
                    {
                        MessageBox.Show("补打保存日志出现异常.");
                        return false;
                    }
                    else
                    {
                        MessageBox.Show("补打成功.");
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 外箱破损补打  按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_reprint_Click(object sender, EventArgs e)
        {
            bool result=RePrint(this.dgv_outter_printRecordList,this.comb_outter_printer);
            if (result)
            {
                this.btn_outter_search_Click(sender, e);
            }
        }
        
        /// <summary>
        /// 外箱作废申请  按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_disable_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectRowCol = this.dgv_outter_printRecordList.SelectedRows;
            if (selectRowCol.Count <= 0)
            {
                MessageBox.Show("请选择报废的标签记录");
                return;
            }
            List<string> printRecordList = new List<string>();
            foreach (DataGridViewRow selRow in selectRowCol)
            {
                printRecordList.Add(selRow.Cells[1].ToString());
            }
            DialogResult btnResult = MessageBox.Show("外箱标签作废，将释放关联的内箱（盒）标签，是否继续?", "提示", MessageBoxButtons.OKCancel);
            if (btnResult == DialogResult.OK)
            { 
                //加载打印记录
                DictionaryEntry recordStateSelItem = (DictionaryEntry)this.comb_outter_printrecordState.SelectedItem;
                bool result=DisablePrintRecord(printRecordList);
                if (result)
                {
                    //加载打印记录
                    btn_outter_search_Click(sender, e);
                }
            }
        }

        /// <summary>
        /// 标签作废
        /// </summary>
        /// <param name="labelRecordIdList"></param>
        /// <returns></returns>
        private bool DisablePrintRecord(List<string> labelRecordIdList)
        {
            string url = "/LabelPrint/DisablePrintRecord";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("LabelRecordIdList", string.Join(",",labelRecordIdList));
            reqData.Add("Updator", Program.UserNo);
            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);

            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (!msgObj.IsSuccess)
            {
                MessageBox.Show("操作失败." + msgObj.Messaage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 内箱增加单箱数量 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_packageQtyTemp_Click(object sender, EventArgs e)
        {
            string inputText = this.comb_packageQty.Text;
            if (!this.comb_packageQty.Items.Contains(inputText))
            {
                this.comb_packageQty.Items.Add(inputText);
                this.comb_packageQty.SelectedItem = inputText;

            }
        }

        /// <summary>
        /// 外箱增加单箱数量 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_packageQtyTemp_Click(object sender, EventArgs e)
        {
            string inputText = this.combo_outter_packQty.Text;
            if (!this.combo_outter_packQty.Items.Contains(inputText))
            {
                this.combo_outter_packQty.Items.Add(inputText);
                this.combo_outter_packQty.SelectedItem = inputText;
            }
        }

        /// <summary>
        /// 内箱标签作废 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_disableApply_Click(object sender, EventArgs e)
        {
            //判断当前是否可以作废
            SaveApply(this.gdvLblRecord,"disable", "标签作废是否继续?","box");
        }

        /// <summary>
        /// 标签申请 申请弹框
        /// </summary>
        /// <param name="dgvObj"></param>
        /// <param name="applyType"></param>
        /// <param name="confirmTip"></param>
        /// <param name="boxType"></param>
        private void SaveApply(DataGridView dgvObj, string applyType,string confirmTip,string boxType)
        {
            DataGridViewSelectedRowCollection selectRowCol = dgvObj.SelectedRows;
            if (selectRowCol.Count <= 0)
            {
                MessageBox.Show("请选择标签记录");
                return;
            }
            bool checkPass = true;
            bool isUnusual = false;
            List<string> printRecordList = new List<string>();
            List<string> isBeforeOperationWorkcode = new List<string>();
            StringBuilder showWorkcode = new StringBuilder();
            foreach (DataGridViewRow selRow in selectRowCol)
            {
                if (selRow.Cells[10].Value.ToString() == "申请中")
                {
                    MessageBox.Show("存在选中标签处于申请中，不能重复申请；");
                    checkPass = false;
                    break;
                }
                if (selRow.Cells[10].Value.ToString() == "可补打")
                {
                    MessageBox.Show("存在选中标签处于可补打，请打印完成；");
                    checkPass = false;
                    break;
                }
                if (isLabelBeforeOperation(selRow.Cells[3].Value.ToString()))
                {
                    if (!isBeforeOperationWorkcode.Contains(selRow.Cells[3].Value.ToString()))
                    {
                        isBeforeOperationWorkcode.Add(selRow.Cells[3].Value.ToString());
                        showWorkcode.Append(selRow.Cells[3].Value.ToString() + ",");
                    }
                    isUnusual = true;
                    continue;
                }
                printRecordList.Add(selRow.Cells[1].Value.ToString());
            }
            
            foreach (String s in isBeforeOperationWorkcode)
            {
                foreach (DataGridViewRow row in dgvObj.Rows)
                {
                    if (row.Cells[3].Value.ToString() == s)
                    {
                        printRecordList.Add(row.Cells[1].Value.ToString());
                    }
                }
            }

            if(isUnusual == true)
            {
                if (MessageBox.Show("工单" + showWorkcode + "需在开单时生成标签.作废此类工单的标签时,同批次的所有标签都将发起作废申请", "是否继续?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    checkPass = false;
            }

            if (!checkPass) return;
            
            FrmApplyDialog frmApply = new FrmApplyDialog(printRecordList, applyType, boxType);
            frmApply.updateIt += new FrmApplyDialog.updateParentData(doSomething);
            frmApply.Show();
        }

        /// <summary>
        /// 调用不同包装的查询点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="boxType"></param>
        private void doSomething(object sender, EventArgs e,string boxType)
        {
            if (boxType == "box")
            {
                btn_SearchRecord_Click(sender, e);
            }
            else
            {
                btn_outter_search_Click(sender, e);
            }
            
        }

        /// <summary>
        /// 内箱补打申请 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_reprintApplay_Click(object sender, EventArgs e)
        {
            SaveApply(this.gdvLblRecord, "reprint", "确定标签需破损补打?", "box");
        }

        /// <summary>
        /// 工单输入框 按键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_workcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (!chklist_workcode.Items.Contains(workcode))
            {
                //如果输入的是回车键  
                if (e.KeyCode == Keys.Enter||e.KeyCode==Keys.ProcessKey)
                {
                    //根据输入单号，获取工单信息
                    string workcode = this.txt_workcode.Text;
                    if (workcode != workcode.ToUpper())
                    {
                        MessageBox.Show("工单号必须为大写!");
                        this.txt_workcode.Text = "";
                        return;
                    }

                    isBeforeOperation = isLabelBeforeOperation(workcode);

                    if (isBeforeOperation && (chklist_workcode.Items.Count>0 || this.dgv_workprintdetail.Rows.Count > 0))
                    {
                        MessageBox.Show("请先处理已扫入的工单!");
                        this.txt_workcode.Text = "";
                        return;
                    }

                    if (isBeforeOperation)
                    {
                        this.dgv_workprintdetail.MultiSelect = false;
                        this.label5.Text = "扫入工单";
                        this.comb_packageQty.Enabled = false;
                        this.dgv_workprintdetail.RowHeadersVisible = false;
                        this.dgv_workprintdetail.AllowUserToDeleteRows = false;
                        this.btn_clear.Visible = true;
                        this.label_printworkcode.Visible = true;
                        this.textBox_printworkcode.Visible = true;
                        this.txt_printNum.Enabled = false;
                        this.chk_merginprint.Enabled = false;
                        this.chklist_workcode.Enabled = false;
                        this.dgv_workprintdetail.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    }
                    else
                    {
                        this.chklist_workcode.Enabled = true;
                        this.comb_packageQty.Enabled = true;
                        this.dgv_workprintdetail.RowHeadersVisible = true;
                        this.label5.Text = "显示工单";
                        this.dgv_workprintdetail.AllowUserToDeleteRows = true;
                        this.btn_clear.Visible = false;
                        this.label_printworkcode.Visible = false;
                        this.textBox_printworkcode.Visible = false;
                        this.txt_printNum.Enabled = true;
                        this.chk_merginprint.Enabled = true;
                        this.dgv_workprintdetail.MultiSelect = true;
                        this.dgv_workprintdetail.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                    }

                    string url = "/LabelPrint/GetWorkcodeInfo";
                    string returnMsg = string.Empty;
                    JObject reqData = new JObject();
                    reqData.Add("CustomerId", CustomerId);
                    reqData.Add("Workcode", workcode);
                    reqData.Add("isBeforeOperation", isBeforeOperation.ToString());
                    string body = JsonConvert.SerializeObject(reqData);
                    returnMsg = AjaxHelper.ClientRequest(url, body);

                    ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
                    if (msgObj.IsSuccess)
                    {
                        JArray workinfos = JsonConvert.DeserializeObject<JArray>(msgObj.Data.ToString());
                        if (workinfos == null)
                        {
                            MessageBox.Show("工单不存在");
                            this.txt_workcode.Text = "";
                            return;
                        }

                        if(isBeforeOperation)
                        {
                            this.comb_packageQty.Items.Add(workinfos[0]["PackageMin"].ToString());
                            this.comb_packageQty.SelectedItem = workinfos[0]["PackageMin"].ToString();

                            this.dgv_workprintdetail.Columns[0].HeaderCell.Value = "";
                            this.dgv_workprintdetail.Columns[1].HeaderCell.Value = "工单号";
                            this.dgv_workprintdetail.Columns[2].HeaderCell.Value = "领取数量";
                            this.dgv_workprintdetail.Columns[3].HeaderCell.Value = "已打印数量";
                            this.dgv_workprintdetail.Columns[4].HeaderCell.Value = "已打印张数";
                            this.dgv_workprintdetail.Columns[5].HeaderCell.Value = "可打印数量(余料)";
                            this.dgv_workprintdetail.Columns[6].HeaderCell.Value = "当前工作站";
                            this.dgv_workprintdetail.Columns[7].HeaderCell.Value = "客户料号";
                        }
                        else
                        {
                            this.dgv_workprintdetail.Columns[0].HeaderCell.Value = "";
                            this.dgv_workprintdetail.Columns[1].HeaderCell.Value = "工单";
                            this.dgv_workprintdetail.Columns[2].HeaderCell.Value = "良品数";
                            this.dgv_workprintdetail.Columns[3].HeaderCell.Value = "已打印数量";
                            this.dgv_workprintdetail.Columns[4].HeaderCell.Value = "已打印张数";
                            this.dgv_workprintdetail.Columns[5].HeaderCell.Value = "可打印数量";
                            this.dgv_workprintdetail.Columns[6].HeaderCell.Value = "工序";
                            this.dgv_workprintdetail.Columns[7].HeaderCell.Value = "客户料号";
                        }

                        int cellIndex = 0;
                        JObject workinfo = null;
                        int rowIndex = 0;
                        

                        foreach(var item in workinfos)
                        {
                            cellIndex = 0;
                            workinfo = (JObject)item;
                            DataGridViewRow viewRow = new DataGridViewRow();

                            if (!enablePrintWorkcode.ContainsKey(workinfo["workcode"].ToString()))
                            {
                                enablePrintWorkcode.Add(workinfo["workcode"].ToString(), workinfo);
                            }

                            if (!chklist_workcode.Items.Contains(workcode))
                            {
                                chklist_workcode.Items.Add(workcode, true);
                            }

                            viewRow.CreateCells(this.dgv_workprintdetail);
                            viewRow.Cells[cellIndex++].Value = workinfo["orderDetailId"];
                            viewRow.Cells[cellIndex++].Value = workinfo["workcode"];
                            viewRow.Cells[cellIndex++].Value = workinfo["goodQty"];
                            viewRow.Cells[cellIndex++].Value = workinfo["printedQty"];
                            viewRow.Cells[cellIndex++].Value = workinfo["printedPages"];
                            if ("0" == workinfo["printedPages"].ToString())
                                selectRowIndexList.Add(rowIndex);
                            viewRow.Cells[cellIndex++].Value = workinfo["enablePrintQty"];
                            viewRow.Cells[cellIndex++].Value = workinfo["ProgramName"];
                            viewRow.Cells[cellIndex++].Value = string.IsNullOrEmpty(workinfo["ClientCode2"].ToString()) ? "空" : workinfo["ClientCode2"];
                            rowIndex++;
                            dgv_workprintdetail.Rows.Add(viewRow);
                            CalcMaxPrintNumer();
                            this.txt_workcode.Text = "";
                        }
                        if (selectRowIndexList.Count == 0 && isBeforeOperation == true)
                            MessageBox.Show("该大批次的所有标签均已打印完毕!");
                        selectRowIndex = selectRowIndexList.Count > 0 ? int.Parse(selectRowIndexList[0].ToString()) : 0;
                        CalcMaxPrintNumer();
                        //if(this.dgv_workprintdetail.Rows[selectRowIndex].Cells["workcode"].Value.ToString() != this.chklist_workcode.Items[0].ToString())
                        //{
                        //    for (var i = 0; i < this.dgv_workprintdetail.Rows.Count; i++)
                        //    {
                        //        var a = this.dgv_workprintdetail.Rows[i].Cells["workcode"].Value.ToString();
                        //        var b = this.chklist_workcode.Items[0].ToString();
                        //        if (this.dgv_workprintdetail.Rows[i].Cells["workcode"].Value.ToString() == this.chklist_workcode.Items[0].ToString())
                        //        {
                        //            if (int.Parse(this.dgv_workprintdetail.Rows[i].Cells["printedPages"].Value.ToString()) > 0)
                        //                MessageBox.Show("工单"+ this.chklist_workcode.Items[0].ToString()+"已打印完毕");
                        //            else
                        //                MessageBox.Show("请先打印工单" + this.dgv_workprintdetail[i, 1].ToString() + "标签");
                        //        }
                        //    }
                        //}
                        this.dgv_workprintdetail.Rows[selectRowIndex].Selected = true;
                        this.textBox_printworkcode.Text = this.dgv_workprintdetail.Rows[selectRowIndex].Cells["workcode"].Value.ToString();
                        this.dgv_workprintdetail.CurrentCell = this.dgv_workprintdetail.Rows[selectRowIndex].Cells[1];
                    }
                }
                _dt = DateTime.Now;
            }
        }

        /// <summary>
        /// 是否上机前打标签 按照最小批次打标签
        /// </summary>
        /// <returns></returns>
        private Boolean isLabelBeforeOperation(string workcode)
        {
            string url = "/LabelPrint/GetIsBeforeOperation";
            string returnMsg = string.Empty;
            string res = string.Empty;
            Boolean result_bool = false;
            JObject reqData = new JObject();
            reqData.Add("Workcode", workcode);
            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);
            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (msgObj.IsSuccess)
            {
                JObject result = JsonConvert.DeserializeObject<JObject>(msgObj.Data.ToString());
                if (result.Count > 0)
                {
                    if ("1".Equals(result["result"].ToString()))
                        result_bool = true;
                }
            }
            return result_bool;
        }

        /// <summary>
        /// 外箱补打申请 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_reprintapply_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectRowCol = this.dgv_outter_printRecordList.SelectedRows;
            if (selectRowCol.Count <= 0)
            {
                MessageBox.Show("请选择一条已打印的标签记录");
            }
            else
            {
                DataGridViewRow selRow = selectRowCol[0];
                if (selRow.Cells[9].Value.ToString() == "未打印")
                    MessageBox.Show("未打印状态不能申请破损补打");
                else
                    SaveApply(this.dgv_outter_printRecordList, "reprint", "确定标签需破损补打?", "outter");
            }
        }

        /// <summary>
        /// 外箱作废申请 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_outter_disableapply_Click(object sender, EventArgs e)
        {
            SaveApply(this.dgv_outter_printRecordList, "disable", "标签作废是否继续?", "outter");
        }

        /// <summary>
        /// 更新模板 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshTemplate_Click(object sender, EventArgs e)
        {
            PrintMessage pMsg = ReFreshTemplate(custObj);
            MessageBox.Show(pMsg.Message);
            tabControl1.SelectedIndex = 0;
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            enablePrintWorkcode.Clear();
            chklist_workcode.Items.Clear();
            this.dgv_workprintdetail.Rows.Clear();
            this.txt_workcode.Text = "";
            this.textBox_printworkcode.Text = "";
            selectRowIndexList.Clear();
            loadPackageQtyComboxData(this.comb_packageQty, custObj);
            CalcMaxPrintNumer();
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void client_treeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.client_treeView_MouseClick(sender, e);
        }

        /// <summary>
        /// 节点展开后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void client_treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            JObject searCondition = new JObject();
            searCondition.Add("PageSize", "100");
            searCondition.Add("PageNum", 1);
            searCondition.Add("CustCode", "");
            searCondition.Add("CustTitle", "");
            searCondition.Add("TemplateType", "");
            searCondition.Add("TreeSelect", e.Node.Text);
            loadLabelCustomerData(searCondition);

            foreach (TreeNode tn in this.client_treeView.Nodes)
            {
                if (tn.Text != e.Node.Text)
                    tn.Collapse();
            }
        }

        private void client_treeView_MouseClick(object sender, MouseEventArgs e)
        {
            string[] selected = this.client_treeView.SelectedNode.FullPath.Split('\\');
            if (selected.Length > 1)
            {
                JObject searCondition = new JObject();
                searCondition.Add("PageSize", "100");
                searCondition.Add("PageNum", 1);
                searCondition.Add("CustCode", "");
                searCondition.Add("CustTitle", "");
                searCondition.Add("TemplateType", DisGetTemplateType(selected.Length > 1 ? selected[1] : ""));
                searCondition.Add("TreeSelect", selected[0]);
                loadLabelCustomerData(searCondition);
            }
            else
            {
                Boolean IsNotExpanded = true;
                foreach (TreeNode tn in this.client_treeView.Nodes)
                {
                    if (tn.IsExpanded == true)
                        IsNotExpanded = false;
                }
                if (IsNotExpanded == true)
                {
                    JObject searCondition = new JObject();
                    searCondition.Add("PageSize", "100");
                    searCondition.Add("PageNum", 1);
                    searCondition.Add("CustCode", "");
                    searCondition.Add("CustTitle", "");
                    searCondition.Add("TemplateType", "");
                    searCondition.Add("TreeSelect", "");
                    loadLabelCustomerData(searCondition);
                }
            }
        }
    }
}
