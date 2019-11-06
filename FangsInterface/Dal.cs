using FangsInterface.Bean;
using HzpCommonLib.DataAccess;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WjwWebservice;

namespace FangsInterface
{
    public class Dal
    {
        public static DataSet checkLogin_ds(string iCode, string iPassWord)
        {
            DataSet ds = null;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();
                string strSql = "select * from PdaUserInfo a where a.pid=:iCode";
                OracleParameter[] oracleParams = new OracleParameter[] {
                OracleHelper.PrepareParameter("iCode", OracleDbType.Varchar2, iCode)
                };

                ds = OracleHelper.ExecuteDataset(connection, CommandType.Text, strSql, oracleParams);

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
        }

        public static DataSet getPatientInfo_ds(string pid)
        {
            DataSet ds;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();

                LogHelper.WriteLog("pid: " + pid);
                string strSql = "select a.住院号,a.姓名,a.床号 from 检验采集信息表 a where a.病人id=:pid";
                OracleParameter[] oracleParams = new OracleParameter[] {
                OracleHelper.PrepareParameter("pid", OracleDbType.Varchar2, pid)
                };

                LogHelper.WriteLog("住院号 sqlStr: " + strSql);
                ds = OracleHelper.ExecuteDataset(connection, CommandType.Text, strSql, oracleParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
            return ds;
        }

        public static DataSet getMedicalAdvice_ds(string barCode, string cType,string eType)
        {
            DataSet ds;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();
                string strSql = "select a.样本条码,a.姓名,a.医嘱id,a.医嘱内容 from 检验采集信息表 a where a.样本条码=:barCode ";
                if (cType == "1") strSql += "and a.采集方式='自采'"; else if (cType == "2") strSql += "and a.采集方式!='自采'";
                switch (eType)
                {
                    case "1"://采集
                        strSql += " and a.采样时间 is null";
                        break;
                    case "2"://送检
                        strSql += " and a.采样时间 is not null and a.送检时间 is null";
                        break;
                    case "3"://提交
                        strSql += " and a.采样时间 is not null and a.送检时间 is not null and a.接收时间 is null";
                        break;
                }

                OracleParameter[] oracleParams = new OracleParameter[] {
                    OracleHelper.PrepareParameter("barCode", OracleDbType.Varchar2, barCode)
                };

                LogHelper.WriteLog("根据条码查询病人医嘱信息[getMedicalAdvice_ds] 入参：" + strSql);
                ds = OracleHelper.ExecuteDataset(connection, CommandType.Text, strSql, oracleParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
            return ds;
        }

        public static int submit_ds(string type, string barCode, string adviceId, string operId)
        {
            int status;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();
                OracleParameter[] oracleParams = new OracleParameter[] {
                OracleHelper.PrepareParameter("Type_In", OracleDbType.Decimal, Decimal.Parse(type)),
                OracleHelper.PrepareParameter("条码_In", OracleDbType.Varchar2, barCode),
                OracleHelper.PrepareParameter("医嘱ID_In", OracleDbType.Decimal,Decimal.Parse(adviceId)),
                OracleHelper.PrepareParameter("操作员_In", OracleDbType.Varchar2,operId),
                OracleHelper.PrepareParameter("备注_In", OracleDbType.Varchar2,"")
			    };
                oracleParams[0].Direction = ParameterDirection.Input;
                oracleParams[1].Direction = ParameterDirection.Input;
                oracleParams[2].Direction = ParameterDirection.Input;

                //oracleParams[2].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                //oracleParams[2].Value = medicalAdviceIds;
                //oracleParams[2].Size = medicalAdviceIds.Length;

                oracleParams[3].Direction = ParameterDirection.Input;
                oracleParams[4].Direction = ParameterDirection.Input;

                status = OracleHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "ZL_LIS_ACTION", oracleParams);
                LogHelper.WriteLog("type:" + type + " barCode :" + barCode + "提交 status：" + status); 
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("提交 异常：" + ex.Message); 
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
            return status;
        }

        public static DataSet getRejectionReason_ds()
        {
            DataSet ds;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();
                string strSql = "select * from 检验采集拒收理由表";
                OracleParameter[] oracleParams = new OracleParameter[] { };

                ds = OracleHelper.ExecuteDataset(connection, CommandType.Text, strSql, oracleParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
            return ds;
        }

        #region 查询用户id是都登记或者注册
        /// <summary>
        /// 查询用户id是都登记或者注册 未登记：-2  注册： -1  登记未注册：0
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public static int queryPdaUserIsRegister(string id)
        {
            int status = 0;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();
                string strSql = "select a.pid,a.password from PdaUserInfo a where a.pid=:pid";
                OracleParameter[] oracleParams = new OracleParameter[] {
                OracleHelper.PrepareParameter("pid", OracleDbType.Varchar2, id)
                };

                DataSet ds = OracleHelper.ExecuteDataset(connection, CommandType.Text, strSql, oracleParams);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //string pwd = ds.Tables[0].Rows[0]["password"].ToString();
                    status = 0;//string.IsNullOrEmpty(pwd) ? 0 : -1;
                }
                else { status = -2; }

                return status;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
        }
        #endregion

        #region 注册或者修改密码
        /// <summary>
        /// 注册或者修改密码
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="pwdMd5">密码</param>
        /// <returns></returns>
        public static bool pdaUserRegisterOrUpDate(string userid, string pwdMd5)
        {
            bool flag = true;
            OracleConnection connection = null;
            try
            {
                connection = OracleHelper.GetOracleConnection();
                string strSql = "update PdaUserInfo set password=:pwdMd5 where pid=:userid";
                OracleParameter[] oracleParams = new OracleParameter[] 
                {
                OracleHelper.PrepareParameter("pwdMd5", OracleDbType.Varchar2, pwdMd5),
                OracleHelper.PrepareParameter("userid", OracleDbType.Varchar2, userid)
                };

                int status = OracleHelper.ExecuteNonQuery(connection, CommandType.Text, strSql, oracleParams);
                LogHelper.WriteLog("注册或者修改密码 : " + status);
                return flag;
            }
            catch (Exception ex)
            {
                flag = false;
                LogHelper.WriteLog("pdaUserRegisterOrUpDate 异常: " + ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
        }
        #endregion
    }
}