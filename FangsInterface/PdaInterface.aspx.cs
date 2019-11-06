using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WjwWebservice;

namespace FangsInterface
{
    public partial class PdaInterface : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region 注册
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbusername.Text.ToString().Trim();
            string password = tbpsw.Text.ToString().Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Response.Write(@"<script>alert('用户ID或密码不能为空！');</script>");
                return;
            }

            int status = Dal.queryPdaUserIsRegister(username);
            LogHelper.WriteLog("queryPdaUserIsRegister: " + status);

            #region 未登记
            if (status == -2) 
            {
                Response.Write(@"<script>alert('此用户ID还未登记！');</script>");
                return;
            }
            #endregion

            #region 已注册
            else if (status == -1)
            {
                Response.Write(@"<script>alert('此用户已注册，如忘记密码，请在终端进行修改！');</script>");
                return;
            }
            #endregion

            #region 登记未注册
            else
            {
                string pwd_md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
                bool isSuccess = Dal.pdaUserRegisterOrUpDate(username, pwd_md5);
                if (isSuccess)
                {
                    Response.Write(@"<script>alert('重置成功！');</script>");
                }
                else
                {
                    Response.Write(@"<script>alert('重置失败！');</script>");
                }
            }
            #endregion
        }
        #endregion
    }
}