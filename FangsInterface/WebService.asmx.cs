using FangsInterface.Bean;
using FangsInterface.Tools;
using HzpCommonLib.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using WjwWebservice;

namespace FangsInterface
{
    /// <summary>
    /// WebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        #region #登陆
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "登陆")]
        public string checkLogin(string inputXml)
        {
            LogHelper.WriteLog("登陆[checkLogin] 入参：" + inputXml);
            try
            {  
                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNode rowNode = doc.SelectSingleNode("./data/row");
                #endregion

                #region 空判断
                string iCode = Util.getNodeValueNoErrByNodeName(rowNode, "iCode");
                if (string.IsNullOrEmpty(iCode))
                {
                    return Util.errMessage("用户ID不能为空！");
                }
                string iPassWord = Util.getNodeValueNoErrByNodeName(rowNode, "iPassWord");
                if (string.IsNullOrEmpty(iPassWord))
                {
                    return Util.errMessage("密码不能为空！");
                }
                #endregion

                #region 测试数据
                if (Util.isDebug())
                {
                    PdaUser user = new PdaUser();
                    user.Code = "1";
                    user.Msg = "执行成功";
                    user.row.oprId = "201708";
                    user.row.oprName = "诗仙李白";
                    user.row.role_no = "100";
                    user.row.role_name = "采集";
                    user.row.department = "门诊住院";

                    return JsonConvert.SerializeObject(user);
                }
                #endregion

                #region 正式
                else
                {
                    string pwd_md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(iPassWord, "md5");
                    DataSet dsData = Dal.checkLogin_ds(iCode, pwd_md5);
                    if (dsData.Tables[0].Rows.Count <= 0) return Util.errMessage("此用户ID还未注册，请先进行注册！");
                    else
                    {
                        if (dsData.Tables[0].Rows[0]["password"].ToString() != pwd_md5) return Util.errMessage("密码不正确！");
                        else
                        {
                            PdaUser user = new PdaUser();
                            user.Code = "1";
                            user.Msg = "执行成功";
                            user.row.oprId = dsData.Tables[0].Rows[0]["pid"].ToString();
                            user.row.oprName = dsData.Tables[0].Rows[0]["pname"].ToString();
                            string parentid = dsData.Tables[0].Rows[0]["parentId"].ToString();
                            if (parentid == "1068" || parentid == "58" || parentid == "1136")
                            {
                                parentid = "100";
                            }
                            else parentid = "101";
                            user.row.role_no = parentid;
                            user.row.role_name = parentid == "100" ? "采集" : "检验";
                            user.row.department = dsData.Tables[0].Rows[0]["dept"].ToString();

                            string result = JsonConvert.SerializeObject(user);
                            LogHelper.WriteLog("登陆[checkLogin] 出参：" + result);
                            return result;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("登陆[checkLogin] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 根据住院号( 腕带号) 查询病人信息
        /// <summary>
        /// 根据住院号( 腕带号) 查询病人信息
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "根据住院号( 腕带号) 查询病人信息")]
        public string getPatientInfo(string inputXml)
        {

            try
            {
                LogHelper.WriteLog("根据腕带号查询病人信息[getPatientInfo] 入参：" + inputXml);

                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNode rowNode = doc.SelectSingleNode("./data/row");
                #endregion

                #region 空判断
                string pId = Util.getNodeValueNoErrByNodeName(rowNode, "pId");
                if (string.IsNullOrEmpty(pId))
                {
                    return Util.errMessage("住院号（腕带号）不能为空！");
                }
                #endregion

                #region 测试数据
                if (Util.isDebug())
                {
                    PatientInfo patientInfo = new PatientInfo();
                    patientInfo.Code = "1";
                    patientInfo.Msg = "执行成功";
                    patientInfo.pInfo.pId = "006284561";
                    patientInfo.pInfo.pNme = "诗仙李白";
                    patientInfo.pInfo.bedNo = "16";

                    return JsonConvert.SerializeObject(patientInfo);
                }
                #endregion

                #region 正式
                else
                {
                    string firstPid = pId.TrimStart("90".ToCharArray());
                    string endPid = "" ;
                    if (firstPid.EndsWith("0"))
                    {
                        endPid = firstPid.Substring(0, firstPid.Length-1);
                    }

                    Util.errMessage("最终得到的病人id = " + endPid);

                    DataSet dsData = Dal.getPatientInfo_ds(endPid);
                    LogHelper.WriteLog("Count: " + dsData.Tables[0].Rows.Count);
                    if (dsData.Tables[0].Rows.Count <= 0)
                    {
                        return Util.errMessage("未查到此用户信息！");
                    }
                    else
                    {
                        PatientInfo patientInfo = new PatientInfo();
                        patientInfo.Code = "1";
                        patientInfo.Msg = "执行成功";
                        patientInfo.pInfo.pId = dsData.Tables[0].Rows[0]["住院号"].ToString();
                        patientInfo.pInfo.pNme = dsData.Tables[0].Rows[0]["姓名"].ToString();
                        patientInfo.pInfo.bedNo = dsData.Tables[0].Rows[0]["床号"].ToString();

                        string result = JsonConvert.SerializeObject(patientInfo);

                        LogHelper.WriteLog("根据腕带号查询病人信息[getPatientInfo] 出参：" + result);
                        return result;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("根据腕带号查询病人信息[getPatientInfo] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 根据条码查询病人医嘱信息
        /// <summary>
        /// 根据条码查询病人医嘱信息
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "根据条码查询病人医嘱信息")]
        public string getMedicalAdvice(string inputXml)
        {
            try
            {
                LogHelper.WriteLog("根据条码查询病人医嘱信息[getMedicalAdvice] 入参：" + inputXml);

                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNode rowNode = doc.SelectSingleNode("./data/row");
                #endregion

                #region 空判断
                string barCode = Util.getNodeValueNoErrByNodeName(rowNode, "barCode");
                if (string.IsNullOrEmpty(barCode))
                {
                    return Util.errMessage("条码号不能为空！");
                }
                string cType = Util.getNodeValueNoErrByNodeName(rowNode, "cType");
                string eType = Util.getNodeValueNoErrByNodeName(rowNode, "eType");
                if (string.IsNullOrEmpty(eType))
                {
                    return Util.errMessage("查询动作不能为空！");
                }
                #endregion

                #region 测试数据
                if (Util.isDebug())
                {
                    MedicalAdvice medicalAdvice = new MedicalAdvice();
                    medicalAdvice.Code = "1";
                    medicalAdvice.Msg = "执行成功";
                    string[][] tmpArray = new string[][] { 
                        new string[]{"006284561","诗仙李白","01","血常规"},
                        new string[]{"006284561","诗仙李白","02","肝功能"}
                    };
                    foreach (string[] item in tmpArray)
                    {
                        MedicalAdviceInfo info = new MedicalAdviceInfo();
                        info.barCode = item[0].ToString();
                        info.name = item[1].ToString();
                        info.adviceId = item[2].ToString();
                        info.adviceName = item[3].ToString();

                        medicalAdvice.data.Add(info);
                    }

                    return JsonConvert.SerializeObject(medicalAdvice);
                }
                #endregion

                #region 正式
                else
                {
                    DataSet dsData = Dal.getMedicalAdvice_ds(barCode, cType,eType);
                    if (dsData.Tables[0].Rows.Count <= 0)
                    {
                        return Util.errMessage("无记录！");
                    }
                    else
                    {
                        MedicalAdvice medicalAdvice = new MedicalAdvice();
                        medicalAdvice.Code = "1";
                        medicalAdvice.Msg = "执行成功";
                        foreach (DataRow item in dsData.Tables[0].Rows)
                        {
                            MedicalAdviceInfo info = new MedicalAdviceInfo();
                            info.barCode = item["样本条码"].ToString();
                            info.name = item["姓名"].ToString();
                            info.adviceId = item["医嘱id"].ToString();
                            info.adviceName = item["医嘱内容"].ToString();

                            medicalAdvice.data.Add(info);
                        }
                        string result = JsonConvert.SerializeObject(medicalAdvice);

                        LogHelper.WriteLog("根据条码查询病人医嘱信息[getMedicalAdvice] 出参：" + result);
                        return result;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("根据条码查询病人医嘱信息[getMedicalAdvice] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 采集提交
        /// <summary>
        /// 采集提交
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "采集提交")]
        public string collectSubmit(string inputXml)
        {
            try
            {
                LogHelper.WriteLog("采集提交[collectSubmit] 入参：" + inputXml);

                #region 测试数据
                if (Util.isDebug())
                {
                    CollectSubmit collectSubmit = new CollectSubmit();
                    collectSubmit.Code = "1";
                    collectSubmit.Msg = "执行成功";

                    return JsonConvert.SerializeObject(collectSubmit);
                }
                #endregion

                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNodeList rowList = doc.SelectNodes("./data/row");
                #endregion

                #region 空判断
                for (int i = 0; i < rowList.Count; i++)
                {
                    string oprId = Util.getNodeValueNoErrByNodeName(rowList[i], "oprId");
                    if (string.IsNullOrEmpty(oprId)) return Util.errMessage("采集员ID不能为空！");

                    string barCode = Util.getNodeValueNoErrByNodeName(rowList[i], "barCode");
                    if (string.IsNullOrEmpty(barCode)) return Util.errMessage("条码号不能为空！");

                    string adviceId = Util.getNodeValueNoErrByNodeName(rowList[i], "adviceId");
                    if (string.IsNullOrEmpty(adviceId)) return Util.errMessage("医嘱ID不能为空！");

                    int status = Dal.submit_ds("0", barCode, adviceId, oprId);
                    LogHelper.WriteLog(adviceId + " 采集提交[collectSubmit] status：" + status);
                }

                #endregion

                BaseModel model = new BaseModel();
                model.Code = "1";
                model.Msg = "执行成功";

                string result = JsonConvert.SerializeObject(model);
                LogHelper.WriteLog("采集提交[collectSubmit] 出参：" + result);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("采集提交[collectSubmit] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 送检提交
        /// <summary>
        /// 送检提交
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "送检提交")]
        public string checkSubmit(string inputXml)
        {
            try
            {
                LogHelper.WriteLog("送检提交[checkSubmit] 入参：" + inputXml);

                #region 测试数据
                if (Util.isDebug())
                {
                    CollectSubmit collectSubmit = new CollectSubmit();
                    collectSubmit.Code = "1";
                    collectSubmit.Msg = "执行成功";

                    return JsonConvert.SerializeObject(collectSubmit);
                }
                #endregion

                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNodeList rowList = doc.SelectNodes("./data/row");
                #endregion

                #region 空判断
                for (int i = 0; i < rowList.Count; i++)
                {
                    string oprId = Util.getNodeValueNoErrByNodeName(rowList[i], "oprId");
                    if (string.IsNullOrEmpty(oprId)) return Util.errMessage("采集员ID不能为空！");

                    string barCode = Util.getNodeValueNoErrByNodeName(rowList[i], "barCode");
                    if (string.IsNullOrEmpty(barCode)) return Util.errMessage("条码号不能为空！");

                    string adviceId = Util.getNodeValueNoErrByNodeName(rowList[i], "adviceId");
                    if (string.IsNullOrEmpty(adviceId)) return Util.errMessage("医嘱ID不能为空！");

                    int status = Dal.submit_ds("1", barCode, adviceId, oprId);
                    LogHelper.WriteLog(adviceId + " checkSubmit status：" + status);
                }

                #endregion

                BaseModel model = new BaseModel();
                model.Code = "1";
                model.Msg = "执行成功";

                string result = JsonConvert.SerializeObject(model);
                LogHelper.WriteLog("送检提交[checkSubmit] 出参：" + result);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("送检提交[checkSubmit] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 接收提交
        /// <summary>
        /// 接收提交
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "接收提交")]
        public string receiveSubmit(string inputXml)
        {
            try
            {
                LogHelper.WriteLog("接收提交[receiveSubmit] 入参：" + inputXml);

                #region 测试数据
                if (Util.isDebug())
                {
                    CollectSubmit collectSubmit = new CollectSubmit();
                    collectSubmit.Code = "1";
                    collectSubmit.Msg = "执行成功";

                    return JsonConvert.SerializeObject(collectSubmit);
                }
                #endregion

                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNodeList rowList = doc.SelectNodes("./data/row");
                #endregion

                #region 空判断
                for (int i = 0; i < rowList.Count; i++)
                {
                    string oprId = Util.getNodeValueNoErrByNodeName(rowList[i], "oprId");
                    if (string.IsNullOrEmpty(oprId)) return Util.errMessage("采集员ID不能为空！");

                    string barCode = Util.getNodeValueNoErrByNodeName(rowList[i], "barCode");
                    if (string.IsNullOrEmpty(barCode)) return Util.errMessage("条码号不能为空！");

                    string adviceId = Util.getNodeValueNoErrByNodeName(rowList[i], "adviceId");
                    if (string.IsNullOrEmpty(adviceId)) return Util.errMessage("医嘱ID不能为空！");

                    int status = Dal.submit_ds("2", barCode, adviceId, oprId);
                    LogHelper.WriteLog(adviceId + " receiveSubmit status：" + status);
                }

                #endregion

                BaseModel model = new BaseModel();
                model.Code = "1";
                model.Msg = "执行成功";

                string result = JsonConvert.SerializeObject(model);
                LogHelper.WriteLog("接收提交[receiveSubmit] 出参：" + result);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("接收提交[receiveSubmit] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 获取拒收理由
        /// <summary>
        /// 获取拒收理由
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "获取拒收理由")]
        public string getRejectionReason()
        {
            try
            {
                #region 测试数据
                if (Util.isDebug())
                {
                    RejectionReason rejectionReason = new RejectionReason();
                    rejectionReason.Code = "1";
                    rejectionReason.Msg = "执行成功";
                    string[][] tmpArray = new string[][] { 
                        new string[]{"01","标本不合格"},
                        new string[]{"02","量太少"}
                    };
                    foreach (string[] item in tmpArray)
                    {
                        RejectionReasonInfo info = new RejectionReasonInfo();
                        info.reasonId = item[0].ToString();
                        info.reasonName = item[1].ToString();

                        rejectionReason.data.Add(info);
                    }
                    return JsonConvert.SerializeObject(rejectionReason);
                }
                #endregion

                #region 正式
                else
                {
                    DataSet dsData = Dal.getRejectionReason_ds();
                    if (dsData.Tables[0].Rows.Count <= 0)
                    {
                        return Util.errMessage("无记录！");
                    }
                    else
                    {
                        RejectionReason rejectionReason = new RejectionReason();
                        rejectionReason.Code = "1";
                        rejectionReason.Msg = "执行成功";
                        foreach (DataRow item in dsData.Tables[0].Rows)
                        {
                            RejectionReasonInfo info = new RejectionReasonInfo();
                            info.reasonId = item[0].ToString();
                            info.reasonName = item[1].ToString();

                            rejectionReason.data.Add(info);
                        }
                        string result = JsonConvert.SerializeObject(rejectionReason);

                        LogHelper.WriteLog("获取拒收理由[getRejectionReason] 出参：" + result);
                        return result;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("获取拒收理由[getRejectionReason] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region 拒收提交
        /// <summary>
        /// 拒收提交
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "拒收提交")]
        public string rejectionSubmit(string inputXml)
        {
            try
            {
                LogHelper.WriteLog("拒收提交[rejectionSubmit] 入参：" + inputXml);

                #region 测试数据
                if (Util.isDebug())
                {
                    CollectSubmit collectSubmit = new CollectSubmit();
                    collectSubmit.Code = "1";
                    collectSubmit.Msg = "执行成功";

                    return JsonConvert.SerializeObject(collectSubmit);
                }
                #endregion

                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNodeList rowList = doc.SelectNodes("./data/row");
                #endregion

                #region 空判断
                for (int i = 0; i < rowList.Count; i++)
                {
                    string oprId = Util.getNodeValueNoErrByNodeName(rowList[i], "oprId");
                    if (string.IsNullOrEmpty(oprId)) return Util.errMessage("采集员ID不能为空！");

                    string barCode = Util.getNodeValueNoErrByNodeName(rowList[i], "barCode");
                    if (string.IsNullOrEmpty(barCode)) return Util.errMessage("条码号不能为空！");

                    string adviceId = Util.getNodeValueNoErrByNodeName(rowList[i], "adviceId");
                    if (string.IsNullOrEmpty(adviceId)) return Util.errMessage("医嘱ID不能为空！");

                    int status = Dal.submit_ds("3", barCode, adviceId, oprId);
                    LogHelper.WriteLog(adviceId + " rejectionSubmit status：" + status);
                }

                #endregion

                BaseModel model = new BaseModel();
                model.Code = "1";
                model.Msg = "执行成功";

                string result = JsonConvert.SerializeObject(model);
                LogHelper.WriteLog("拒收提交[rejectionSubmit] 出参：" + result);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("拒收提交[rejectionSubmit] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion

        #region #修改密码
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true, Description = "修改密码")]
        public string modifyPassword(string inputXml) 
        {
            LogHelper.WriteLog("修改密码[checkLogin] inputXml：" + inputXml);
            try
            {
                #region 解析入参
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputXml);
                XmlNode xnode = doc.SelectSingleNode("./data/row");
                #endregion
                 

                #region 空判断
                string user_id = Util.getNodeValueNoErrByNodeName(xnode, "user_id");
                if (string.IsNullOrEmpty(user_id))
                {
                    return Util.errMessage("用户ID不能为空！");
                }
                string pwd = Util.getNodeValueNoErrByNodeName(xnode, "pwd");
                string modifyPsw = Util.getNodeValueNoErrByNodeName(xnode, "modifyPsw");
                if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(modifyPsw))
                {
                    return Util.errMessage("密码不能为空！");
                }

                if (pwd == modifyPsw) 
                {
                    return Util.errMessage("新密码和原密码不能相同！");
                }
                #endregion

                #region 测试数据
                if (Util.isDebug())
                {
                    PdaUser user = new PdaUser();
                    user.Code = "1";
                    user.Msg = "执行成功";
                    user.row.oprId = "201708";
                    user.row.oprName = "诗仙李白";
                    user.row.role_no = "100";
                    user.row.role_name = "采集";
                    user.row.department = "门诊住院";

                    return JsonConvert.SerializeObject(user);
                }
                #endregion

                #region 正式
                else
                {
                    string pwd_md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(modifyPsw, "md5");
                    bool isSuccess = Dal.pdaUserRegisterOrUpDate(user_id, pwd_md5);
                    if (!isSuccess) return Util.errMessage("修改密码失败！");
                    else
                    {
                        BaseModel model = new BaseModel();
                        model.Code = "1";
                        model.Msg = "执行成功";

                        LogHelper.WriteLog(user_id + "修改密码[checkLogin] 成功");
                        return JsonConvert.SerializeObject(model);
                    }
                     
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("修改密码[checkLogin] 异常：" + ex.Message);
                return Util.errMessage(ex.Message);
            }
        }
        #endregion


    }
}
