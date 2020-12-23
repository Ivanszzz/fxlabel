using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AHLabelPrint
{
	public partial class FrmApplyDialog : Form
	{
        public delegate void updateParentData(object sender, EventArgs e, string boxType);
        public event updateParentData updateIt;

        private List<string> printRecordList;
        private ArrayList ApplyCombItemList = new ArrayList() { };
        private string ApplyType;
        private string BoxType;
        public FrmApplyDialog(List<string> labelRecordIdList, string applyType,string boxType)
		{
			InitializeComponent();
            printRecordList = labelRecordIdList;
            ApplyType = applyType;
            BoxType = boxType;
        }

        /// <summary>
        /// 作废申请记录
        /// </summary>
        /// <param name="labelRecordIdList"></param>
        /// <returns></returns>
        private bool AddApplyPrintRecord(List<string> labelRecordIdList)
        {
            string url = "/LabelPrint/AddApplyPrintRecord";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("LabelRecordIdList", string.Join(",", labelRecordIdList));
            reqData.Add("ApplyType", ApplyType);
            reqData.Add("ApplyReason", this.comb_applyreason.SelectedItem.ToString());
            reqData.Add("Remark", this.txt_remark.Text.ToString());
            reqData.Add("ApplyUserNo", Program.UserNo);
            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);

            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (!msgObj.IsSuccess)
            {
                MessageBox.Show("操作失败." + msgObj.Messaage);
                return false;
            }
            else
            {
                MessageBox.Show("操作成功");
                updateIt(null,null, BoxType);
                return true;
            }
        }

        private void loadApplyComboxData(ComboBox ctrlObj)
        {
            ApplyCombItemList.Add(new DictionaryEntry("disable", "申请作废"));
            ApplyCombItemList.Add(new DictionaryEntry("reprint", "破损补打"));
            ctrlObj.DataSource = ApplyCombItemList;
            ctrlObj.DisplayMember = "Value";
            ctrlObj.ValueMember = "Key";
        }

        private void btn_sumbitApply_Click(object sender, EventArgs e)
		{
            if (AddApplyPrintRecord(printRecordList))
            {
                this.Hide();
            }
		}

        private void FrmApplyDialog_Load(object sender, EventArgs e)
        {
            this.lbl_selRowsCount.Text = printRecordList.Count.ToString() + "行";
            loadApplyComboxData(this.comb_applytype);
            this.comb_applytype.SelectedItem = GetApplyType(ApplyType);
        }

        private DictionaryEntry GetApplyType(string applyType)
        {
            DictionaryEntry applyTypeTemp;
            foreach (DictionaryEntry item in ApplyCombItemList)
            {
                if(item.Key.ToString()== applyType)
                {
                    applyTypeTemp = item;
                    break;
                }
            }
            return applyTypeTemp;
        }

        private void btn_cancelApply_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
