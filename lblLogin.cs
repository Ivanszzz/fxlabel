using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace AHLabelPrint
{
    
	public partial class lblLogin : Form
	{
		public lblLogin()
		{
			InitializeComponent();
            skinEngine1.SkinFile = System.Environment.CurrentDirectory + "\\Skins\\DiamondBlue.ssk";
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            string userNo = this.txt_username.Text;
            string pwd = this.txt_pwd.Text;

            using (FileStream fs = new FileStream("data.bin", FileMode.OpenOrCreate))
            {
                Dictionary<string, User> users = new Dictionary<string, User>();
                User user = new User();
                user.Username = userNo;
                BinaryFormatter bf = new BinaryFormatter();
                if (this.chk_remember.Checked)       //  如果单击了记住密码的功能
                {   //  在文件中保存密码
                    user.Password=pwd;
                }
                else
                {   //  不在文件中保存密码
                    user.Password = "";
                }
                //  选在集合中是否存在用户名 
                if (users.ContainsKey(userNo))
                {
                    users.Remove(userNo);
                }
                users.Add(userNo, user);
                //要先将User类先设为可以序列化(即在类的前面加[Serializable])
                bf.Serialize(fs, users);
            }

            string url = "/LabelCustomer/LabelLogin";
            string returnMsg = string.Empty;
            JObject reqData = new JObject();
            reqData.Add("UserName", "");
            reqData.Add("Password", pwd);
            reqData.Add("UserNO", userNo);//只显示子标签；未打印的；

            string body = JsonConvert.SerializeObject(reqData);
            returnMsg = AjaxHelper.ClientRequest(url, body);

            //{"IsSuccess":true,"Data":"{\"id\":\"726243\",\"cycle\":\"945Z\",\"workcode\":\"W-19111693-01-02\",\"enableprintqty\":\"2000\",\"printedqty\":\"0\",\"printedpages\":\"0\",\"goodqty\":\"2000\"}","Messaage":""}
            JObject msgObj = JsonConvert.DeserializeObject<JObject>(returnMsg);
            if ((bool)msgObj["IsSuccess"])
            {
                JObject info = JsonConvert.DeserializeObject<JObject>(msgObj["Data"].ToString());
                string userNO = info["UserNo"].ToString();
                string userName = string.Empty;
                if (info["UserName"] != null)
                {
                    userName = info["UserName"].ToString();
                }

                Program.logName = userName + "/" + userNO;
                Program.UserNo = userNO;
                lblPrintFrm lblPFrm = new lblPrintFrm();
                lblPFrm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(msgObj["Messaage"].ToString());
            }
        }

        private void lblLogin_Load(object sender, EventArgs e)
        {
            //  读取配置文件寻找记住的用户名和密码
            using (FileStream fs = new FileStream("data.bin", FileMode.OpenOrCreate))
            {
                if (fs.Length > 0)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Dictionary<string, User> users = bf.Deserialize(fs) as Dictionary<string, User>;
                    foreach (User user in users.Values)
                    {
                        this.txt_username.Items.Add(user.Username);
                    }

                    for (int i = 0; i < users.Count; i++)
                    {
                        if (this.txt_username.Text != "")
                        {
                            if (users.ContainsKey(this.txt_username.Text))
                            {
                                this.txt_pwd.Text = users[this.txt_username.Text].Password.ToString();
                                this.chk_remember.Checked = true;
                            }
                        }
                    }
                }
            }

            //  用户名默认选中第一个
            if (this.txt_username.Items.Count > 0)
            {
                this.txt_username.SelectedIndex = this.txt_username.Items.Count - 1;
            }
        }

        private void txt_username_SelectedValueChanged(object sender, EventArgs e)
        {
            //  首先读取记住密码的配置文件
            using (FileStream fs = new FileStream("data.bin", FileMode.OpenOrCreate))
            {
                if (fs.Length > 0)
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    Dictionary<string, User> users = bf.Deserialize(fs) as Dictionary<string, User>;

                    for (int i = 0; i < users.Count; i++)
                    {
                        if (this.txt_username.Text != "")
                        {
                            if (users.ContainsKey(txt_username.Text) && users[txt_username.Text].Password.ToString() != "")
                            {
                                this.txt_pwd.Text = users[txt_username.Text].Password.ToString();
                                this.chk_remember.Checked = true;
                            }
                            else
                            {
                                this.txt_pwd.Text = "";
                                this.chk_remember.Checked = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
