using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WjwWebservice
{
    public class LogHelper
    {

        public static void WriteLog(string strLog)
        {
            string sFilePath = "D:\\Pda日志\\" + DateTime.Now.ToString("yyyy-MM");
            string sFileName = "logger-" + DateTime.Now.ToString("dd") + ".log";
            sFileName = sFilePath + "\\" + sFileName; //文件的绝对路径

            if (!Directory.Exists(sFilePath))//验证路径是否存在
            {
                Directory.CreateDirectory(sFilePath);
                //不存在则创建
            }
            try
            {
                FileStream fileStream;
                if (File.Exists(sFileName))
                {
                    fileStream = new FileStream(sFileName, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fileStream = new FileStream(sFileName, FileMode.Create, FileAccess.Write);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ### " + strLog);
                streamWriter.Close();
                fileStream.Close();
            }
            catch(Exception ex)
            {
                //....
            }
        }

        public static void WriteUpLog(string strLog)
        {
            string sFilePath = "D:\\武汉市三通日志\\上传基础信息\\" + DateTime.Now.ToString("yyyy-MM");
            string sFileName = "logger-" + DateTime.Now.ToString("dd") + ".log";
            sFileName = sFilePath + "\\" + sFileName; //文件的绝对路径

            if (!Directory.Exists(sFilePath))//验证路径是否存在
            {
                Directory.CreateDirectory(sFilePath);
                //不存在则创建
            }
            try
            {
                FileStream fileStream;
                if (File.Exists(sFileName))
                {
                    fileStream = new FileStream(sFileName, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fileStream = new FileStream(sFileName, FileMode.Create, FileAccess.Write);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ### " + strLog);
                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                //....
            }
        }
    }
}