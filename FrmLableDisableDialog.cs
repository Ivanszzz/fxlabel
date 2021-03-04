using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace AHLabelPrint
{
    public partial class FrmLableDisableDialog : Form
    {
        List<string> labelCodeList = new List<string>();
        public delegate void updateParentData(object sender, EventArgs e, string boxType);
        public event updateParentData updateIt;

        public FrmLableDisableDialog()
        {
            InitializeComponent();
        }

        private void labelinput_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.ProcessKey)
            {
                string labelcode = this.labelinput_textBox.Text;
                this.labelinput_textBox.Text = "";
                string md5Code = FormsAuthentication.HashPasswordForStoringInConfigFile(labelcode, "MD5");
                string url = "/LabelPrint/GetLableInfoByMD5Code";
                string returnMsg = string.Empty;
                JObject reqData = new JObject();
                reqData.Add("MD5", md5Code);
                string body = JsonConvert.SerializeObject(reqData);
                returnMsg = AjaxHelper.ClientRequest(url, body);
                ClientResponseMsg responseMsg = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
                if (responseMsg.IsSuccess)
                {
                    JObject labelinfo = JsonConvert.DeserializeObject<JObject>(responseMsg.Data.ToString());
                    if(labelinfo != null)
                    {
                        if (labelinfo["n_state"].ToString() != "1")
                        {
                            MessageBox.Show("该标签已作废!");
                            return;
                        }
                            
                        if (labelinfo["BoxType"].ToString() != "box")
                        {
                            MessageBox.Show("该标签不是内箱标签!");
                            return;
                        }
                            
                        if (labelinfo["s_AuditOper"].ToString() != "" && labelinfo["s_AuditOper"].ToString() != null && labelinfo["s_AuditOper"].ToString() != "normal")
                        {
                            MessageBox.Show("该标签存在未补打的记录或者未完成的审批流程!");
                            return;
                        }

                        if (labelCodeList.Contains(labelinfo["s_Id"].ToString()))
                        {
                            MessageBox.Show("该标签已存在列表中!");
                            return;
                        }

                        DataGridViewRow dataGridViewRow = new DataGridViewRow();
                        dataGridViewRow.CreateCells(this.Dgv_labellist);
                        int cellIndex = 0;
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["QRCodeMd5Val"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["WorkCode"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["ClientCode"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["SerialNumber"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["d_CreateTime"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["s_Creator"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["s_CustTitle"].ToString();
                        dataGridViewRow.Cells[cellIndex++].Value = labelinfo["s_Id"].ToString();
                        this.Dgv_labellist.Rows.Add(dataGridViewRow);
                        labelCodeList.Add(labelinfo["s_Id"].ToString());
                    }
                    else
                    {
                        MessageBox.Show("系统未查询到此标签!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(responseMsg.Messaage);
                }
            }
        }

        private void Dgv_labellist_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string n_id = this.Dgv_labellist.Rows[e.Row.Index].Cells[7].Value.ToString();
            labelCodeList.Remove(n_id);
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            string url = "/LabelPrint/AddApplyPrintRecord";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("LabelRecordIdList", string.Join(",", labelCodeList));
            reqData.Add("ApplyType", "disable");
            reqData.Add("ApplyReason", this.reason_comboBox.SelectedItem.ToString());
            reqData.Add("Remark", this.remake_textBox.Text.ToString());
            reqData.Add("ApplyUserNo", Program.UserNo);
            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);

            ClientResponseMsg msgObj = JsonConvert.DeserializeObject<ClientResponseMsg>(returnMsg);
            if (!msgObj.IsSuccess)
            {
                MessageBox.Show("操作失败." + msgObj.Messaage);
            }
            else
            {
                MessageBox.Show("操作成功");
                updateIt(null, null, "box");
                labelCodeList.Clear();
                this.Dgv_labellist.Rows.Clear();
                this.Hide();
            }
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void FrmLableDisableDialog_Load(object sender, EventArgs e)
        {
            this.reason_comboBox.SelectedIndex = 0;
        }
    }
}
