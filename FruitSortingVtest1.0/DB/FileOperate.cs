using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FruitSortingVtest1.DB
{
    public class FileOperate
    {
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="sb">文件内容</param>
        /// <param name="patch">文件路径</param>
        public static void WriteFile(StringBuilder sb, string patch)
        {
            FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="curLine">当前行号</param>
        /// <param name="patch">文件路径</param>
        public static string ReadFile(int curLine, string patch)
        {
            FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            StringBuilder sb = new StringBuilder();
            string line = "";
            string temp = null;
            for (int i = 0; line != null; i++)
            {
                if (i == curLine - 1)
                {
                    temp = sr.ReadLine();
                }
                else
                {
                    line = sr.ReadLine();
                }
            }
            sr.Close();
            fs.Close();
            return temp;
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="curLine">要修改的行数</param>
        /// <param name="newLineValue">改行修改后的值</param>
        /// <param name="patch">文件绝对路径</param>
        public static void EditFile(int curLine, string newLineValue, string patch)
        {
            FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            StringBuilder sb = new StringBuilder();
            string line = "";
            string temp;
            string strNull;
            for (int i = 0; line != null; i++)
            {
                if (i == curLine - 1)
                {
                    temp = sr.ReadLine();
                    line = newLineValue;
                    if (line.Length >= temp.Length)
                    {
                        sb.Append(line + "\r\n");
                    }
                    else
                    {
                        strNull = new string(' ', temp.Length - line.Length);
                        line += strNull;
                        sb.Append(line + "\r\n");
                    }
                }
                else
                {
                    line = sr.ReadLine();
                    sb.Append(line + "\r\n");
                }
            }
            sr.Close();
            fs.Close();

            FileStream fs1 = new FileStream(patch, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1);
            sw.Write(sb.ToString());
            sw.Close();
            fs1.Close();
        }
    }
}
