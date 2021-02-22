using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using Interface;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Common
{
    public static class Commonfunction
    {
        #region 方法
        /// <summary>
        /// 根据ID取通道索引
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetChannelIndex(int ID)
        {
            int id = (((ID) & 0x000F) - 1);
            return id;
        }

        public static int GetIPMIndex(int ID)
        {
            int id = ((((ID) & 0x00F0)>>4) - 1);
            return id;
        }

        /// <summary>
        /// 根据ID取子系统索引，FSM
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetSubsysIndex(int ID)
        {
            int id = (((ID & 0XF00)>>8)-1);
            return id;
        }

        /// <summary>
        /// 根据ID取IPM的ID，FSM用于上位机通知下位机
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetSubSysID(int ID)
        {
            int id = ((ID) & 0x0F00) >> 4;
            return id;
        }

        /// <summary>
        /// 根据ID取IPM的ID，FSM用于上位机通知下位机
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetIPMID(int ID)
        {
            int id = ((ID)&0x0FF0) >> 4;
            return id;
        }
        /// <summary>
        /// 根据ID获取IPM中的Index
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int ChanelInIPMIndex(int ID)
        {
            int id = ((((ID) & 0x000F) - 1) % ConstPreDefine.CHANNEL_NUM);
            return id;
        }


        
        /// <summary>
        /// 通道ID编码
        /// </summary>
        /// <param name="x"></param>子系统
        /// <param name="y"></param>IPM
        /// <param name="z"></param>通道
        /// <returns></returns>
        public static int EncodeChannel(int x, int y, int z)
        {
            int result = ((x + 1) << 8 | (y + 1) << 4 | (z + 1));
            return result;
        }

        /// <summary>
        /// IPM的ID编码
        /// </summary>
        /// <param name="x"></param>子系统
        /// <param name="y"></param>IPM
        /// <returns></returns>
        public static int EncodeIPM(int x,int y)
        {
            int result = EncodeChannel(x, y, -1);
            return result;
        }

        /// <summary>
        /// 子系统ID编码
        /// </summary>
        /// <param name="x"></param>子系统
        /// <returns></returns>
        public static int EncodeSubsys(int x)
        {
            int result = EncodeChannel(x, -1, -1);
            return result;
        }

        public static int ChannelIndexToIpmIndex(int ChannelIndex)
        {
            return ChannelIndex / ConstPreDefine.CHANNEL_NUM;
        }


         

        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj"></param>结构体
        /// <returns></returns>byte数组
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);//得到结构体大小
            byte[] bytes = new byte[size];//创建byte数组
            try
            {
                IntPtr structPtr = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
                Marshal.StructureToPtr(structObj, structPtr, false);//将结构体拷到分配好的内存空间
                Marshal.Copy(structPtr, bytes, 0, size);//从内存空间拷到byte数组
                Marshal.FreeHGlobal(structPtr);//释放内存空间
                return bytes;//返回byte数组
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数StructToBytes出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数StructToBytes出错" + ex);
#endif
                return bytes;//返回byte数组
            }
        }
        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStruct(byte[] bytes,Type type)
        {
            try
            {
                int size = Marshal.SizeOf(type);//得到结构体大小
                if (size > bytes.Length)
                {
                    return null;
                }
                IntPtr structPtr = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
                Marshal.Copy(bytes, 0, structPtr, size);//从byte数组拷到内存空间
                object obj = Marshal.PtrToStructure(structPtr, type);//将内存空间转换为目标结构体
                Marshal.FreeHGlobal(structPtr);//释放内存空间
                return obj;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数BytesToStruct出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数BytesToStruct出错" + ex);
#endif
                return null;
            }
        }
        
        /// <summary>
        ///获取所有子系统ID号
        /// </summary>
        /// <param name="bChannelInfo"></param>子系统通道是否有效 stSysConfig.nChannelInfo
        /// <param name="arrayID"></param>返回所有子系统ID号数组
        public  static void GetAllSysID(byte[] bChannelInfo,ref List<int> arrayID)
        {
            try
            {
                arrayID.Clear();
                for (int i = ConstPreDefine.MAX_SUBSYS_NUM - 1; i >= 0; i--)
                {
                    for (int j = ConstPreDefine.MAX_CHANNEL_NUM - 1; j >= 0; j--)
                    {
                        if (bChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        {
                            int ID = EncodeSubsys(i);
                            arrayID.Add(ID);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetAllSysID出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetAllSysID出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取所有通道ID
        /// </summary>
        /// <param name="bChannelInfo"></param>
        /// <param name="arrayID"></param>
        /// <returns></returns>
        public static int GetAllChannelID(byte[] bChannelInfo, ref List<int> arrayID)
        {
            try
            {
                for (int i = 0; i < ConstPreDefine.MAX_SUBSYS_NUM; i++)
                {
                    for (int j = 0; j < ConstPreDefine.MAX_CHANNEL_NUM; j++)
                    {
                        if (bChannelInfo[i * ConstPreDefine.MAX_CHANNEL_NUM + j] == 1)
                        {
                            int ID = EncodeChannel(i, j, j);
                            arrayID.Add(ID);
                        }
                    }
                }
                return arrayID.Count;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetAllChannelID出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetAllChannelID出错" + ex);
#endif
                return arrayID.Count;
            }
        }

        /// <summary>
        /// 设置应用程序配置节点，如果已经存在此节点，则会修改节点的值，否则添加此节点
        /// </summary>
        /// <param name="key">节点名称</param>
        /// <param name="value">节点值</param>
        public static void SetAppSetting(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection appaSetting = (AppSettingsSection)config.GetSection("appSettings");
                if (appaSetting.Settings[key] == null)//如果不存在此节点，则添加
                {
                    appaSetting.Settings.Add(key, value);
                }
                else//如果存在此节点，则修改
                {
                    appaSetting.Settings[key].Value = value;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                config = null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数SetAppSetting出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数SetAppSetting出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取单个节点的值
        /// </summary>
        /// <param name="key">节点名称</param>
        /// <returns>节点值</returns>
        public static string GetAppSetting(string key)
        {
            
            try
            {
                string value;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection appaSetting = (AppSettingsSection)config.GetSection("appSettings");
                value = appaSetting.Settings[key].Value;
                config = null;
                return value;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetAppSetting出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetAppSetting出错" + ex);
#endif
                return "";
            }
            
        }

        /// <summary>
        /// YUV422Z转RGB32
        /// </summary>
        /// <param name="bYUV"></param>
        /// <param name="bRGB"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        public static void YUV422ChangeToRGB(byte[] bYUV, ref byte[] bRGB, int nWidth, int nHeight)
        {
            try
            {
                int nLen = nWidth * nHeight;
                int y0, y1;
                int u, v;
                int nTemp;

                for (int i = 0; i < (nLen / 2); i++)
                {
                    //提取YUV值
                    y0 = bYUV[i * 4];
                    y1 = bYUV[i * 4 + 2];
                    if (bYUV[i * 4 + 1] > 127)
                        u = (int)(bYUV[i * 4 + 1] - 256);
                    else
                        u = (int)bYUV[i * 4 + 1];
                    if (bYUV[i * 4 + 3] > 127)
                        v = (int)(bYUV[i * 4 + 3] - 256);
                    else
                        v = (int)bYUV[i * 4 + 3];

                    //R = Y+1.14V;
                    //G =Y-0.39U-0.58V;
                    //B = Y+2.03U;

                    //计算第一个像素
                    //r
                    nTemp = (short)(y0 + 1.14 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 8 + 2] = (byte)nTemp;

                    //G
                    nTemp = (short)(y0 - 0.39 * u - 0.58 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 8 + 1] = (byte)nTemp;

                    //B
                    nTemp = (short)(y0 + 2.03 * u);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 8] = (byte)nTemp;

                    //alfa
                    bRGB[i * 8 + 3] = 0;

                    //计算第二个像素
                    //R
                    nTemp = (short)(y1 + 1.14 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 8 + 6] = (byte)nTemp;

                    //G
                    nTemp = (short)(y1 - 0.39 * u - 0.58 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 8 + 5] = (byte)nTemp;

                    //B
                    nTemp = (short)(y1 + 2.03 * u);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 8 + 4] = (byte)nTemp;

                    //alfa
                    bRGB[i * 8 + 7] = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数YUV422ChangeToRGB出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数YUV422ChangeToRGB出错" + ex);
#endif
            }
                
        }

        /// <summary>
        /// YUV422Z转RGB24
        /// </summary>
        /// <param name="bYUV"></param>
        /// <param name="bRGB"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        public static void YUV422ChangeToRGB24(byte[] bYUV, ref byte[] bRGB, int nWidth, int nHeight)
        {
            try
            {
                int nLen = nWidth * nHeight;
                int y0, y1;
                int u, v;
                int nTemp;

                for (int i = 0; i < (nLen / 2); i++)
                {
                    //提取YUV值
                    y0 = bYUV[i * 4];
                    y1 = bYUV[i * 4 + 2];
                    if (bYUV[i * 4 + 1] > 127)
                        u = (int)(bYUV[i * 4 + 1] - 256);
                    else
                        u = (int)bYUV[i * 4 + 1];
                    if (bYUV[i * 4 + 3] > 127)
                        v = (int)(bYUV[i * 4 + 3] - 256);
                    else
                        v = (int)bYUV[i * 4 + 3];

                    //R = Y+1.14V;
                    //G =Y-0.39U-0.58V;
                    //B = Y+2.03U;

                    //计算第一个像素
                    //r
                    nTemp = (short)(y0 + 1.14 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 6 + 2] = (byte)nTemp;

                    //G
                    nTemp = (short)(y0 - 0.39 * u - 0.58 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 6 + 1] = (byte)nTemp;

                    //B
                    nTemp = (short)(y0 + 2.03 * u);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 6] = (byte)nTemp;

                    ////alfa
                    //bRGB[i * 8 + 3] = 0;

                    //计算第二个像素
                    //R
                    nTemp = (short)(y1 + 1.14 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 6 + 5] = (byte)nTemp;

                    //G
                    nTemp = (short)(y1 - 0.39 * u - 0.58 * v);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 6 + 4] = (byte)nTemp;

                    //B
                    nTemp = (short)(y1 + 2.03 * u);
                    if (nTemp > 255)
                        nTemp = 255;
                    if (nTemp < 0)
                        nTemp = 0;
                    bRGB[i * 6 + 3] = (byte)nTemp;

                    ////alfa
                    //bRGB[i * 8 + 7] = 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数YUV422ChangeToRGB24出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数YUV422ChangeToRGB出错" + ex);
#endif
            }

        }

        /// <summary>
        /// YUV422灰度转RGB24
        /// </summary>
        /// <param name="bYUV"></param>
        /// <param name="bRGB"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        public static void YUV422GrayChangeToRGB24(byte[] bYUV, ref byte[] bRGB, int nWidth, int nHeight)
        {
            try
            {
                int nLen = nWidth * nHeight;
                //int y0, y1;
                //int u, v;
                //int nTemp;

                for (int i = 0; i < nLen ; i++)
                {

                    //计算第一个像素
                    //R
                    bRGB[i * 3 + 2] = bYUV[i];

                    //G
                    bRGB[i * 3 + 1] = bYUV[i];

                    //B
                    bRGB[i * 3] = bYUV[i];

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数 YUV422GrayChangeToRGB24出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数 YUV422GrayChangeToRGB24出错" + ex);
#endif
            }

        }


        /// <summary>
        /// 形成YUV色谱图
        /// </summary>
        /// <param name="nLum"></param>
        /// <param name="RGBImage"></param>
        public static void IniColorChartElem(int nLum,ref byte[] RGBImage)
        {
            try
            {
                int u, v, nTemp;
                for (int i = 0; i < 256; i++)
                {
                    u = i - 128;

                    for (int j = 0; j < 256; j++)
                    {
                        v = j - 128;

                        for (int k = 0; k < 4; k++)
                        {
                            //R
                            nTemp = (int)(nLum + 1.14 * v);
                            if (nTemp > 255)
                                nTemp = 255;
                            if (nTemp < 0)
                                nTemp = 0;
                            RGBImage[(i * 256 + j) * 4 + 2] = (byte)nTemp;

                            //G
                            nTemp = (int)(nLum - 0.39 * u - 0.58 * v);
                            if (nTemp > 255)
                                nTemp = 255;
                            if (nTemp < 0)
                                nTemp = 0;
                            RGBImage[(i * 256 + j) * 4 + 1] = (byte)nTemp;

                            //B
                            nTemp = (int)(nLum + 2.03 * u);
                            if (nTemp > 255)
                                nTemp = 255;
                            if (nTemp < 0)
                                nTemp = 0;
                            RGBImage[(i * 256 + j) * 4] = (byte)nTemp;
                            RGBImage[(i * 256 + j) * 4 + 3] = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数IniColorChartElem出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数IniColorChartElem出错" + ex);
#endif
            }
        }

        /// <summary>
        /// MONO8转RGB32
        /// </summary>
        /// <param name="bYUV"></param>
        /// <param name="bRGB"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        public static void MONO8ChangeToRGB(byte[] bYUV, ref byte[] bRGB, int nWidth, int nHeight)
        {
            int nLen = nWidth * nHeight;
            for (int i = 0; i < nLen; i++)
            {
                bRGB[i * 4 + 2] = (byte)bYUV[i];
                bRGB[i * 4 + 1] = (byte)bYUV[i];
                bRGB[i * 4] = (byte)bYUV[i];
            }
        }

        /// <summary>
        /// 获取工程配置文件
        /// </summary>
        /// <param name="configFileNameList"></param>
        public static void GetAllProjectSettingFileName(ref List<string> configFileNameList)
        {
            try
            {
                configFileNameList.Clear();
                DirectoryInfo dir = new DirectoryInfo(System.Environment.CurrentDirectory + "\\config");
                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo file in files)
                {
                    if (file.Extension.Equals(".exp"))
                    {
                        string[] str = file.Name.Split('.');
                        string FileName = str[0];
                        for (int i = 1; i < str.Length - 1; i++)
                        {
                            if ((str.Length - 1) != i) FileName += ".";
                            FileName += str[i];
                        }
                        configFileNameList.Add(FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetAllProjectSettingFileName出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetAllProjectSettingFileName出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 获取用户配置文件
        /// </summary>
        /// <param name="configFileNameList"></param>
        public static void GetAllCommonSettingFileName(ref List<string> configFileNameList)
        {
            try
            {
                configFileNameList.Clear();
                DirectoryInfo dir = new DirectoryInfo(System.Environment.CurrentDirectory + "\\config");
                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo file in files)
                {
                    if (file.Extension.Equals(".cmc"))
                    {
                        string[] str = file.Name.Split('.');
                        string FileName = str[0];
                        for (int i = 1; i < str.Length - 1; i++)
                        {
                            if ((str.Length - 1) != i) FileName += ".";
                            FileName += str[i];
                        }
                        configFileNameList.Add(FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetAllCommonSettingFileName出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetAllCommonSettingFileName出错" + ex);
#endif
            }
        }

        public const int AVG_UV = 0x04;
        public const int AVG_H =0x02;
        public const int AVG_Y=0x01;
        public const int RATIO_UV=0x0C;
        public const int RATIO_H=0x0A;
        public const int RATIO_Y=0x09;
        public const uint DEFAULT_MINFRUITGRAY =30;
        public const float COEFF_R0 = 1.14f;
        public const float COEFF_G0 =-0.39f;
        public const float COEFF_G1 =-0.58f;
        public const float COEFF_B0 = 2.03f;
        public const float COEFF_Y0 = 0.299f;
        public const float COEFF_Y1 = 0.587f;
        public const float COEFF_Y2 = 0.114f;
        public const float COEFF_U0 = -0.147f;
        public const float COEFF_U1 = -0.289f;
        public const float COEFF_U2 = 0.436f;
        public const float COEFF_V0 = 0.615f;
        public const float COEFF_V1 = -0.515f;
        public const float COEFF_V2 = -0.100f;
        public const float SHERU = 0.5f;

        
        private static uint EXPEND1to4(uint a) 
        {
            uint B =(((a & 0xFF) << 0) | ((a & 0xFF) << 8) | ((a & 0xFF) << 16) | ((a & 0xFF) << 24));
            return B;
        }
        /****************************
        ucSrcImg			BGRA格式图
        nImageW,nImageH		图像宽高
        nCupNum				图像中的果杯数
        char cColorType		AVG_UV  AVG_H	AVG_Y	 RATIO_UV	RATIO_H		RATIO_Y 
        colorsRGB			三种颜色标记，含3个元素的数组[3]
        unColorIntervals	含3个元素的数组[3]，	若是UV空间,数组3个值均有效，且每一个按U0、V0、U1、V1顺序存于一个int
        H或若是Y空间,取数组前两个值		另H是取+180的范围值,UV是真实值右偏移128至0~255范围

        pColorCount			存储结果指针	RATIO型时，三个成员值均有效
        AVG型时，取对应前1/2个
        ****************************/
        public static void ColorStatistic(ref byte[] bSrcImg, int nImageW, int nImageH, int nCupNum,int []nLefts, int ColorType,
           ColorRGB[] colorRGB, uint[] unColorIntervals, ref int[] ColorCount)
        {

            //MessageBox.Show("nImageW=" + nImageW.ToString() + "\nImageH=" + nImageH.ToString() + "\nCupNum=" + nCupNum.ToString() + "\nStartX = " + nStartX.ToString() + "\nnCupW = " + nCupW.ToString());

            byte ucY, ucB = 0, ucG = 0, ucR = 0;
            int nB, nG, nR, nU, nV;
            int nMinY, nMaxY, nOffset0, nOffset;
            int nH1, nH, nMinVal, nMaxVal, nLeft, nRight, nPixTotal, nTempColorCount0, nTempColorCount1, nTempColorCount2, nOkCupNum;
            int erodTimes;
            uint unMinFruitGray = EXPEND1to4(DEFAULT_MINFRUITGRAY);

            //MemoryStream ms = new MemoryStream(bSrcImg);
            //Image image = System.Drawing.Image.FromStream(ms);
            //image.Save("src.bmp");

            try
            {
        
                //二值图
                byte[] bBinImg = new byte[nImageW * nImageH];

                nOffset0 = nOffset = 0;
                for (int y = 0; y < nImageH; y++)
                {
                    for (int x = 0; x < nImageW; x++, nOffset0++, nOffset += 4)
                    {
                        ucB = bSrcImg[nOffset];
                        ucG = bSrcImg[nOffset + 1];
                        ucR = bSrcImg[nOffset + 2];
                        if (0 != ucB || 0 != ucG || 0 != ucR)//Y分量判断
                        {
                            bBinImg[nOffset0] = 255;
                        }
                    }
                }

                // MemoryStream ms1 = new MemoryStream(bBinImg);
                //Image image1 = System.Drawing.Image.FromStream(ms);
                //image1.Save("Bin.bmp");

                //对二值图进行腐蚀，采用与IPM一致的方法
                uint[] unEdgePt = new uint[nImageH];
                for (int k = 0; k < nCupNum; k++)
                {
                    //每一行的左右边界
                    for (int j = 0; j < nImageH; j++)
                        unEdgePt[j] = 0x7FFF0000;
                    nMaxY = 0;
                    nMinY = nImageH;//每一个果杯的边界
                    nOffset0 = 0;
                    for (int j = 0; j < nImageH; j++, nOffset0 += nImageW)
                    {
                        for (int i = nLefts[k]; i < nLefts[k + 1]; i++)
                        {
                            if (bBinImg[nOffset0 + i] != 0)
                            {
                                unEdgePt[j] = (uint)i << 16;//左端点
                                if (j < nMinY)
                                {
                                    nMinY = j;//上边界
                                }
                                if (j > nMaxY)
                                {
                                    nMaxY = j;//下边界
                                }
                                break;
                            }
                        }

                        for (int i = nLefts[k + 1] - 1; i >= nLefts[k]; i--)
                        {
                            if (bBinImg[nOffset0 + i] != 0)
                            {
                                unEdgePt[j] |= (uint)i & 0xFFFF;//右端点
                                break;
                            }
                        }
                    }

                    if (nMinY < nMaxY)
                    {
                        //上下缩减
                        erodTimes = (nMaxY - nMinY) > 8 ? 4 : 1;
                        nOffset0 = nMinY * nImageW;
                        for (int y = nMinY; y < nMinY + erodTimes; y++, nOffset0 += nImageW)
                            for (int x = nLefts[k]; x < nLefts[k + 1]; x++)
                            {
                                bBinImg[nOffset0 + x] = 0;
                            }
                        nOffset0 = nMaxY * nImageW;
                        for (int y = nMaxY; y > nMaxY - erodTimes; y--, nOffset0 -= nImageW)
                            for (int x = nLefts[k]; x < nLefts[k + 1]; x++)
                            {
                                bBinImg[nOffset0 + x] = 0;
                            }

                        //左右缩减
                        nOffset0 = (nMinY + erodTimes) * nImageW;
                        for (int y = nMinY + erodTimes; y <= nMaxY - erodTimes; y++, nOffset0 += nImageW)
                        {
                            nLeft = (int)(unEdgePt[y] >> 16);
                            for (int i = 0; i < erodTimes; i++)
                            {
                                bBinImg[nOffset0 + nLeft + i] = 0;
                            }
                            nRight = (int)(unEdgePt[y] & 0xFFFF);
                            for (int i = 0; i < erodTimes; i++)
                            {
                                bBinImg[nOffset0 + nRight - i] = 0;
                            }
                        }
                    }
                }


                //MemoryStream ms2 = new MemoryStream(bBinImg);
                //Image image2 = System.Drawing.Image.FromStream(ms);
                //image2.Save("erodeBin.bmp");


                //准确的UV边界
                byte L_V0, L_U0, L_V1, L_U1, L_V2, L_U2, L_V3, L_U3, L_V4, L_U4, L_V5, L_U5;
                byte U0 = 0, V0 = 0, U1 = 0, V1 = 0, U2 = 0, V2 = 0, U3 = 0, V3 = 0, U4 = 0, V4 = 0, U5 = 0, V5 = 0;
                if (ColorType == RATIO_UV || ColorType == AVG_UV)
                {
                    unMinFruitGray &= 0xff;
                    L_U0 = (byte)((unColorIntervals[0] & 0xFF000000) >> 24);
                    L_V0 = (byte)((unColorIntervals[0] & 0x00FF0000) >> 16);
                    L_U1 = (byte)((unColorIntervals[0] & 0x0000FF00) >> 8);
                    L_V1 = (byte)(unColorIntervals[0] & 0x000000FF);
                    if (L_U0 > L_U1)
                    {
                        U1 = L_U0;
                        U0 = L_U1;
                    }
                    else
                    {
                        U1 = L_U1;
                        U0 = L_U0;
                    }
                    if (L_V0 > L_V1)
                    {
                        V1 = L_V0;
                        V0 = L_V1;
                    }
                    else
                    {
                        V1 = L_V1;
                        V0 = L_V0;
                    }

                    L_U2 = (byte)((unColorIntervals[1] & 0xFF000000) >> 24);
                    L_V2 = (byte)((unColorIntervals[1] & 0x00FF0000) >> 16);
                    L_U3 = (byte)((unColorIntervals[1] & 0x0000FF00) >> 8);
                    L_V3 = (byte)(unColorIntervals[1] & 0x000000FF);
                    if (L_U2 > L_U3)
                    {
                        U3 = L_U2;
                        U2 = L_U3;
                    }
                    else
                    {
                        U3 = L_U3;
                        U2 = L_U2;
                    }
                    if (L_V2 > L_V3)
                    {
                        V3 = L_V2;
                        V2 = L_V3;
                    }
                    else
                    {
                        V3 = L_V3;
                        V2 = L_V2;
                    }


                    L_U4 = (byte)((unColorIntervals[2] & 0xFF000000) >> 24);
                    L_V4 = (byte)((unColorIntervals[2] & 0x00FF0000) >> 16);
                    L_U5 = (byte)((unColorIntervals[2] & 0x0000FF00) >> 8);
                    L_V5 = (byte)(unColorIntervals[2] & 0x000000FF);
                    if (L_U4 > L_U5)
                    {
                        U5 = L_U4;
                        U4 = L_U5;
                    }
                    else
                    {
                        U5 = L_U5;
                        U4 = L_U4;
                    }
                    if (L_V4 > L_V5)
                    {
                        V5 = L_V4;
                        V4 = L_V5;
                    }
                    else
                    {
                        V5 = L_V5;
                        V4 = L_V4;
                    }
                }
                else
                {
                    if (unColorIntervals[0] > unColorIntervals[1])
                    {
                        uint temp = unColorIntervals[0];
                        unColorIntervals[0] = unColorIntervals[1];
                        unColorIntervals[1] = temp;
                    }
                }
                ColorCount[0] = 0;
                ColorCount[1] = 0;
                ColorCount[2] = 0;
                nOkCupNum = 0;
                switch (ColorType)
                {
                    case RATIO_UV:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nPixTotal = 0;
                                nTempColorCount0 = 0;
                                nTempColorCount1 = 0;
                                nTempColorCount2 = 0;

                                nOffset0 = 0;
                                for (int j = 0; j < nImageH; j++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];
                                    while (nLeft < nRight)
                                    {
                                        if (255 == bBinImg[nOffset0 + nLeft]) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nPixTotal++;
                                        nOffset = nOffset0 + nX << 2;
                                        nB = bSrcImg[nOffset];
                                        nG = bSrcImg[nOffset + 1];
                                        nR = bSrcImg[nOffset + 2];

                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
                                        nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
                                        nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

                                        nU = nU < 0 ? 0 : nU;
                                        nU = nU > 255 ? 255 : nU;
                                        nV = nV < 0 ? 0 : nV;
                                        nV = nV > 255 ? 255 : nV;
                                        if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
                                        {
                                            if (ucY > unMinFruitGray)//注意，保留了灰度限制
                                            {
                                                nTempColorCount2++;
                                                ucB = colorRGB[2].ucB;
                                                ucG = colorRGB[2].ucG;
                                                ucR = colorRGB[2].ucR;
                                            }
                                        }
                                        else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
                                        {
                                            if (ucY > unMinFruitGray)
                                            {
                                                nTempColorCount1++;
                                                ucB = colorRGB[1].ucB;
                                                ucG = colorRGB[1].ucG;
                                                ucR = colorRGB[1].ucR;
                                            }
                                        }
                                        else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
                                        {
                                            if (ucY > unMinFruitGray)
                                            {
                                                nTempColorCount0++;
                                                ucB = colorRGB[0].ucB;
                                                ucG = colorRGB[0].ucG;
                                                ucR = colorRGB[0].ucR;
                                            }
                                        }
                                        else
                                            continue;
                                        bSrcImg[nOffset] = ucB;
                                        bSrcImg[nOffset + 1] = ucG;
                                        bSrcImg[nOffset + 2] = ucR;


                                    }
                                }//每个果杯

                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                }

                                ColorCount[0] += nTempColorCount0;
                                ColorCount[1] += nTempColorCount1;
                                ColorCount[2] += nTempColorCount2;
                            }
                            break;
                        }
                    case AVG_UV:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nPixTotal = 0;
                                nTempColorCount0 = 0;
                                nTempColorCount1 = 0;
                                nOffset0 = 0;
                                for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];
                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nOffset = nOffset0 + nX << 2;
                                        nPixTotal++;
                                        nB = bSrcImg[nOffset];
                                        nG = bSrcImg[nOffset + 1];
                                        nR = bSrcImg[nOffset + 2];

                                        nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
                                        nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

                                        nU = nU < 0 ? 0 : nU;
                                        nU = nU > 255 ? 255 : nU;
                                        nV = nV < 0 ? 0 : nV;
                                        nV = nV > 255 ? 255 : nV;

                                        nTempColorCount0 += nU;
                                        nTempColorCount1 += nV;
                                    }
                                }
                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;//未四舍五入
                                    nTempColorCount1 = nTempColorCount1 / nPixTotal;
                                }
                                ColorCount[0] += nTempColorCount0;
                                ColorCount[1] += nTempColorCount1;
                            }
                        }
                        break;
                    case RATIO_Y:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nPixTotal = 0;
                                nTempColorCount0 = 0;
                                nTempColorCount1 = 0;
                                nTempColorCount2 = 0;
                                nOffset0 = 0;
                                for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];
                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nOffset = nOffset0 + nX << 2;
                                        nPixTotal++;
                                        ucB = bSrcImg[nOffset];
                                        ucG = bSrcImg[nOffset + 1];
                                        ucR = bSrcImg[nOffset + 2];

                                        ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
                                        if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
                                        {
                                            nTempColorCount0++;
                                            ucB = colorRGB[0].ucB;
                                            ucG = colorRGB[0].ucG;
                                            ucR = colorRGB[0].ucR;
                                        }
                                        else if (ucY < unColorIntervals[1])
                                        {
                                            nTempColorCount1++;
                                            ucB = colorRGB[1].ucB;
                                            ucG = colorRGB[1].ucG;
                                            ucR = colorRGB[1].ucR;
                                        }
                                        else
                                        {
                                            nTempColorCount2++;
                                            ucB = colorRGB[2].ucB;
                                            ucG = colorRGB[2].ucG;
                                            ucR = colorRGB[2].ucR;
                                        }
                                        bSrcImg[nOffset] = ucB;
                                        bSrcImg[nOffset + 1] = ucG;
                                        bSrcImg[nOffset + 2] = ucR;
                                    }
                                }
                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                }

                                ColorCount[0] += nTempColorCount0;
                                ColorCount[1] += nTempColorCount1;
                                ColorCount[2] += nTempColorCount2;
                            }
                        }
                        break;
                    case AVG_Y:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nPixTotal = 0;
                                nTempColorCount0 = 0;
                                nOffset0 = 0;
                                for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nPixTotal++;
                                        nOffset = nOffset0 + nX << 2;
                                        ucB = bSrcImg[nOffset];
                                        ucG = bSrcImg[nOffset + 1];
                                        ucR = bSrcImg[nOffset + 2];
                                        ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB + SHERU);
                                        nTempColorCount0 += ucY;
                                    }
                                }

                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;
                                }
                                ColorCount[0] += nTempColorCount0;
                            }
                        }
                        break;
                    case RATIO_H:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nPixTotal = 0;
                                nTempColorCount0 = 0;
                                nTempColorCount1 = 0;
                                nTempColorCount2 = 0;
                                nOffset0 = 0;
                                for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nPixTotal++;
                                        nOffset = nOffset0 + nX << 2;
                                        nB = bSrcImg[nOffset];
                                        nG = bSrcImg[nOffset + 1];
                                        nR = bSrcImg[nOffset + 2];

                                        nMaxVal = nR > nG ? nR : nG;
                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
                                        nMinVal = (nR < nG ? nR : nG);
                                        nMinVal = nMinVal < nB ? nMinVal : nB;
                                        if (nMaxVal != nMinVal)
                                        {
                                            if (nMaxVal == nR)
                                            {
                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
                                            }
                                            else if (nMaxVal == nG)
                                            {
                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
                                            }
                                            else
                                            {
                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
                                            }
                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
                                        }
                                        else
                                        {
                                            nH = 0;
                                        }
                                        nH = nH >= 360 ? 359 : nH;

                                        nH += 180;
                                        nH = nH >= 360 ? nH - 360 : nH;

                                        if (nH < unColorIntervals[0])
                                        {
                                            nTempColorCount0++;
                                            ucB = colorRGB[0].ucB;
                                            ucG = colorRGB[0].ucG;
                                            ucR = colorRGB[0].ucR;
                                        }
                                        else if (nH < unColorIntervals[1])
                                        {
                                            nTempColorCount1++;
                                            ucB = colorRGB[1].ucB;
                                            ucG = colorRGB[1].ucG;
                                            ucR = colorRGB[1].ucR;
                                        }
                                        else
                                        {
                                            nTempColorCount2++;
                                            ucB = colorRGB[2].ucB;
                                            ucG = colorRGB[2].ucG;
                                            ucR = colorRGB[2].ucR;
                                        }
                                        bSrcImg[nOffset] = ucB;
                                        bSrcImg[nOffset + 1] = ucG;
                                        bSrcImg[nOffset + 2] = ucR;
                                    }
                                }
                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
                                }

                                ColorCount[0] += nTempColorCount0;
                                ColorCount[1] += nTempColorCount1;
                                ColorCount[2] += nTempColorCount2;
                            }
                        }
                        break;
                    case AVG_H:
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nPixTotal = 0;
                                nTempColorCount0 = 0;
                                nOffset0 = 0;
                                for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];
                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nPixTotal++;
                                        nOffset = nOffset0 + nX << 2;
                                        nB = bSrcImg[nOffset];
                                        nG = bSrcImg[nOffset + 1];
                                        nR = bSrcImg[nOffset + 2];

                                        nMaxVal = nR > nG ? nR : nG;
                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
                                        nMinVal = (nR < nG ? nR : nG);
                                        nMinVal = nMinVal < nB ? nMinVal : nB;
                                        if (nMaxVal != nMinVal)
                                        {
                                            if (nMaxVal == nR)
                                            {
                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
                                            }
                                            else if (nMaxVal == nG)
                                            {
                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
                                            }
                                            else
                                            {
                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
                                            }
                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
                                        }
                                        else
                                        {
                                            nH = 0;
                                        }
                                        nH = nH >= 360 ? 359 : nH;
                                        nH = nH + 180 > 360 ? nH - 180 : nH + 180;
                                        //nH = nH >= 360 ? nH - 360 : nH;
                                        nTempColorCount0 += nH;
                                    }
                                }
                                if (nPixTotal != 0)
                                {
                                    nOkCupNum++;
                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;
                                    //nTempColorCount0 += 180;
                                    //nTempColorCount0 = nTempColorCount0 >= 360 ? nTempColorCount0 - 360 : nTempColorCount0;
                                }
                                ColorCount[0] += nTempColorCount0;
                            }
                        }
                        break;

                    default: break;
                }

                if (nOkCupNum > 0)
                {
                    ColorCount[0] = (ColorCount[0] + (nOkCupNum >> 1)) / nOkCupNum;
                    ColorCount[1] = (ColorCount[1] + (nOkCupNum >> 1)) / nOkCupNum;
                    ColorCount[2] = (ColorCount[2] + (nOkCupNum >> 1)) / nOkCupNum;

                    int tempSum = ColorCount[0] + ColorCount[1] + ColorCount[2];
                    if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV && (U5 - U4) * (V5 - V4) + (U3 - U2) * (V3 - V2) + (U1 - U0) * (V1 - V0) == 255 * 255))//保证加和为100
                    {
                        if (tempSum > 100)
                        {
                            int MaxVal = ColorCount[0] > ColorCount[1] ? ColorCount[0] : ColorCount[1];
                            MaxVal = MaxVal > ColorCount[2] ? MaxVal : ColorCount[2];
                            if (MaxVal == ColorCount[0])
                            {
                                ColorCount[0] = 100 - ColorCount[1] - ColorCount[2];
                            }
                            else if (MaxVal == ColorCount[1])
                            {
                                ColorCount[1] = 100 - ColorCount[0] - ColorCount[2];
                            }
                            else
                                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
                        }
                        else if (tempSum < 100)
                        {
                            int MinVal = ColorCount[0] < ColorCount[1] ? ColorCount[0] : ColorCount[1];
                            MinVal = MinVal < ColorCount[2] ? MinVal : ColorCount[2];
                            if (MinVal == ColorCount[0])
                            {
                                ColorCount[0] = 100 - ColorCount[1] - ColorCount[2];
                            }
                            else if (MinVal == ColorCount[1])
                            {
                                ColorCount[1] = 100 - ColorCount[0] - ColorCount[2];
                            }
                            else
                                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
                        }

                    }
                    else if (ColorType == AVG_UV)
                    {
                        ColorCount[2] = -1;
                        bool okFlag = true;
                        byte avgU = (byte)ColorCount[0];
                        byte avgV = (byte)ColorCount[1];
                        if ((U4 <= avgU && avgU <= U5) && (V4 <= avgV && avgV <= V5))
                        {
                            ucB = colorRGB[2].ucB;
                            ucG = colorRGB[2].ucG;
                            ucR = colorRGB[2].ucR;
                        }
                        else if ((U2 <= avgU && avgU <= U3) && (V2 <= avgV && avgV <= V3))
                        {
                            ucB = colorRGB[1].ucB;
                            ucG = colorRGB[1].ucG;
                            ucR = colorRGB[1].ucR;
                        }
                        else if ((U0 <= avgU && avgU <= U1) && (V0 <= avgV && avgV <= V1))
                        {
                            ucB = colorRGB[0].ucB;
                            ucG = colorRGB[0].ucG;
                            ucR = colorRGB[0].ucR;
                        }
                        else
                            okFlag = false;

                        if (okFlag)
                        {
                            for (int k = 0; k < nCupNum; k++)
                            {
                                nOffset0 = 0;
                                for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                                {
                                    nLeft = nLefts[k];
                                    nRight = nLefts[k + 1];
                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nLeft++;
                                    }

                                    while (nLeft < nRight)
                                    {
                                        if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                        {
                                            break;
                                        }
                                        nRight--;
                                    }

                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                    if (nRight < nLeft)
                                    {
                                        continue;
                                    }

                                    for (int nX = nLeft; nX < nRight; nX++)
                                    {
                                        nOffset = nOffset0 + nX << 2;
                                        bSrcImg[nOffset] = ucB;
                                        bSrcImg[nOffset + 1] = ucG;
                                        bSrcImg[nOffset + 2] = ucR;
                                    }
                                }
                            }
                        }

                    }
                    else if (ColorType == AVG_H || ColorType == AVG_Y)
                    {
                        ColorCount[1] = -1;
                        ColorCount[2] = -1;
                        if (ColorCount[0] < unColorIntervals[0])
                        {
                            ucB = colorRGB[0].ucB;
                            ucG = colorRGB[0].ucG;
                            ucR = colorRGB[0].ucR;
                        }
                        else if (ColorCount[0] < unColorIntervals[1])
                        {
                            ucB = colorRGB[1].ucB;
                            ucG = colorRGB[1].ucG;
                            ucR = colorRGB[1].ucR;
                        }
                        else
                        {
                            ucB = colorRGB[2].ucB;
                            ucG = colorRGB[2].ucG;
                            ucR = colorRGB[2].ucR;
                        }

                        for (int k = 0; k < nCupNum; k++)
                        {
                            nOffset0 = 0;
                            for (int i = 0; i < nImageH; i++, nOffset0 += nImageW)
                            {
                                nLeft = nLefts[k];
                                nRight = nLefts[k + 1];
                                while (nLeft < nRight)
                                {
                                    if (bBinImg[nOffset0 + nLeft] == 255) //mask 像素有效
                                    {
                                        break;
                                    }
                                    nLeft++;
                                }

                                while (nLeft < nRight)
                                {
                                    if (bBinImg[nOffset0 + nRight] == 255) //mask 像素有效
                                    {
                                        break;
                                    }
                                    nRight--;
                                }

                                nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
                                nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
                                if (nRight < nLeft)
                                {
                                    continue;
                                }

                                for (int nX = nLeft; nX < nRight; nX++)
                                {
                                    nOffset = nOffset0 + nX << 2;
                                    bSrcImg[nOffset] = ucB;
                                    bSrcImg[nOffset + 1] = ucG;
                                    bSrcImg[nOffset + 2] = ucR;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ColorCount[0] = -1;
                    ColorCount[1] = -1;
                    ColorCount[2] = -1;
                }

                //MemoryStream ms3 = new MemoryStream(bSrcImg);
                //Image image3 = System.Drawing.Image.FromStream(ms);
                //image3.Save("dst.bmp");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数ColorStatistic出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数ColorStatistic出错" + ex);
#endif
            }
        }


        /****************************
        ucSrcImg			BGRA格式图
        nImageW,nChannelHs[3]		图像宽高	
        nCupNum				图像中的果杯数
        char cColorType		AVG_UV  AVG_H	AVG_Y	 RATIO_UV	RATIO_H		RATIO_Y 
        colorsRGB			三种颜色标记，含3个元素的数组[3]
        unColorIntervals	含3个元素的数组[3]，	若是UV空间,数组3个值均有效，且每一个按U0、V0、U1、V1顺序存于一个int
        H或若是Y空间,取数组前两个值		另H是取+180的范围值,UV是真实值右偏移128至0~255范围

        pColorCount			存储结果指针	RATIO型时，三个成员值均有效
        AVG型时，取对应前1/2个
        ****************************/
//        public static void ColorStatistic24(ref byte[] bSrcImg, int nImageW, int[] nChannelHs, int nCupNum, int[] nLefts1, int[] nLefts0, int[] nLefts2, int ColorType,
//           ColorRGB[] colorRGB, uint[] unColorIntervals, ref int[] ColorCount)
//        {

//            //MessageBox.Show("nImageW=" + nImageW.ToString() + "\nImageH=" + nImageH.ToString() + "\nCupNum=" + nCupNum.ToString() + "\nStartX = " + nStartX.ToString() + "\nnCupW = " + nCupW.ToString());

//            byte ucY, ucB = 0, ucG = 0, ucR = 0;
//            int nB, nG, nR, nU, nV, nOffset, nW3, nX3;
//            int nH1, nH, nMinVal, nMaxVal, nLeft, nRight, nPixTotal, nTempColorCount0, nTempColorCount1, nTempColorCount2, nOkCupNum;
//            uint unMinFruitGray = EXPEND1to4(DEFAULT_MINFRUITGRAY);
//            try
//            {
//                //准确的UV边界
//                byte L_V0, L_U0, L_V1, L_U1, L_V2, L_U2, L_V3, L_U3, L_V4, L_U4, L_V5, L_U5;
//                byte U0 = 0, V0 = 0, U1 = 0, V1 = 0, U2 = 0, V2 = 0, U3 = 0, V3 = 0, U4 = 0, V4 = 0, U5 = 0, V5 = 0;
//                if (ColorType == RATIO_UV || ColorType == AVG_UV)
//                {
//                    unMinFruitGray &= 0xff;
//                    L_U0 = (byte)((unColorIntervals[0] & 0xFF000000) >> 24);
//                    L_V0 = (byte)((unColorIntervals[0] & 0x00FF0000) >> 16);
//                    L_U1 = (byte)((unColorIntervals[0] & 0x0000FF00) >> 8);
//                    L_V1 = (byte)(unColorIntervals[0] & 0x000000FF);
//                    if (L_U0 > L_U1)
//                    {
//                        U1 = L_U0;
//                        U0 = L_U1;
//                    }
//                    else
//                    {
//                        U1 = L_U1;
//                        U0 = L_U0;
//                    }
//                    if (L_V0 > L_V1)
//                    {
//                        V1 = L_V0;
//                        V0 = L_V1;
//                    }
//                    else
//                    {
//                        V1 = L_V1;
//                        V0 = L_V0;
//                    }

//                    L_U2 = (byte)((unColorIntervals[1] & 0xFF000000) >> 24);
//                    L_V2 = (byte)((unColorIntervals[1] & 0x00FF0000) >> 16);
//                    L_U3 = (byte)((unColorIntervals[1] & 0x0000FF00) >> 8);
//                    L_V3 = (byte)(unColorIntervals[1] & 0x000000FF);
//                    if (L_U2 > L_U3)
//                    {
//                        U3 = L_U2;
//                        U2 = L_U3;
//                    }
//                    else
//                    {
//                        U3 = L_U3;
//                        U2 = L_U2;
//                    }
//                    if (L_V2 > L_V3)
//                    {
//                        V3 = L_V2;
//                        V2 = L_V3;
//                    }
//                    else
//                    {
//                        V3 = L_V3;
//                        V2 = L_V2;
//                    }


//                    L_U4 = (byte)((unColorIntervals[2] & 0xFF000000) >> 24);
//                    L_V4 = (byte)((unColorIntervals[2] & 0x00FF0000) >> 16);
//                    L_U5 = (byte)((unColorIntervals[2] & 0x0000FF00) >> 8);
//                    L_V5 = (byte)(unColorIntervals[2] & 0x000000FF);
//                    if (L_U4 > L_U5)
//                    {
//                        U5 = L_U4;
//                        U4 = L_U5;
//                    }
//                    else
//                    {
//                        U5 = L_U5;
//                        U4 = L_U4;
//                    }
//                    if (L_V4 > L_V5)
//                    {
//                        V5 = L_V4;
//                        V4 = L_V5;
//                    }
//                    else
//                    {
//                        V5 = L_V5;
//                        V4 = L_V4;
//                    }
//                }
//                else
//                {
//                    if (unColorIntervals[0] > unColorIntervals[1])
//                    {
//                        uint temp = unColorIntervals[0];
//                        unColorIntervals[0] = unColorIntervals[1];
//                        unColorIntervals[1] = temp;
//                    }
//                }


//                nW3 = nImageW * 3;
//                ColorCount[0] = 0;
//                ColorCount[1] = 0;
//                ColorCount[2] = 0;
//                nOkCupNum = 0;
//                switch (ColorType)
//                {
//                    case RATIO_UV:
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                nTempColorCount0 = 0;
//                                nTempColorCount1 = 0;
//                                nTempColorCount2 = 0;
//                                nPixTotal = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
//                                        nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

//                                        nU = nU < 0 ? 0 : nU;
//                                        nU = nU > 255 ? 255 : nU;
//                                        nV = nV < 0 ? 0 : nV;
//                                        nV = nV > 255 ? 255 : nV;
//                                        if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
//                                        {
//                                            if (ucY > unMinFruitGray)//注意，保留了灰度限制
//                                            {
//                                                nTempColorCount2++;
//                                                ucB = colorRGB[2].ucB;
//                                                ucG = colorRGB[2].ucG;
//                                                ucR = colorRGB[2].ucR;
//                                            }
//                                        }
//                                        else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
//                                        {
//                                            if (ucY > unMinFruitGray)
//                                            {
//                                                nTempColorCount1++;
//                                                ucB = colorRGB[1].ucB;
//                                                ucG = colorRGB[1].ucG;
//                                                ucR = colorRGB[1].ucR;
//                                            }
//                                        }
//                                        else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
//                                        {
//                                            if (ucY > unMinFruitGray)
//                                            {
//                                                nTempColorCount0++;
//                                                ucB = colorRGB[0].ucB;
//                                                ucG = colorRGB[0].ucG;
//                                                ucR = colorRGB[0].ucR;
//                                            }
//                                        }
//                                        else
//                                            continue;
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //Mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
//                                        nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

//                                        nU = nU < 0 ? 0 : nU;
//                                        nU = nU > 255 ? 255 : nU;
//                                        nV = nV < 0 ? 0 : nV;
//                                        nV = nV > 255 ? 255 : nV;
//                                        if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
//                                        {
//                                            if (ucY > unMinFruitGray)//注意，保留了灰度限制
//                                            {
//                                                nTempColorCount2++;
//                                                ucB = colorRGB[2].ucB;
//                                                ucG = colorRGB[2].ucG;
//                                                ucR = colorRGB[2].ucR;
//                                            }
//                                        }
//                                        else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
//                                        {
//                                            if (ucY > unMinFruitGray)
//                                            {
//                                                nTempColorCount1++;
//                                                ucB = colorRGB[1].ucB;
//                                                ucG = colorRGB[1].ucG;
//                                                ucR = colorRGB[1].ucR;
//                                            }
//                                        }
//                                        else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
//                                        {
//                                            if (ucY > unMinFruitGray)
//                                            {
//                                                nTempColorCount0++;
//                                                ucB = colorRGB[0].ucB;
//                                                ucG = colorRGB[0].ucG;
//                                                ucR = colorRGB[0].ucR;
//                                            }
//                                        }
//                                        else
//                                            continue;
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //Right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
//                                        nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

//                                        nU = nU < 0 ? 0 : nU;
//                                        nU = nU > 255 ? 255 : nU;
//                                        nV = nV < 0 ? 0 : nV;
//                                        nV = nV > 255 ? 255 : nV;
//                                        if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
//                                        {
//                                            if (ucY > unMinFruitGray)//注意，保留了灰度限制
//                                            {
//                                                nTempColorCount2++;
//                                                ucB = colorRGB[2].ucB;
//                                                ucG = colorRGB[2].ucG;
//                                                ucR = colorRGB[2].ucR;
//                                            }
//                                        }
//                                        else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
//                                        {
//                                            if (ucY > unMinFruitGray)
//                                            {
//                                                nTempColorCount1++;
//                                                ucB = colorRGB[1].ucB;
//                                                ucG = colorRGB[1].ucG;
//                                                ucR = colorRGB[1].ucR;
//                                            }
//                                        }
//                                        else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
//                                        {
//                                            if (ucY > unMinFruitGray)
//                                            {
//                                                nTempColorCount0++;
//                                                ucB = colorRGB[0].ucB;
//                                                ucG = colorRGB[0].ucG;
//                                                ucR = colorRGB[0].ucR;
//                                            }
//                                        }
//                                        else
//                                            continue;
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }

//                                //果杯LMR均值
//                                if (nPixTotal != 0)
//                                {
//                                    nOkCupNum++;
//                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
//                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
//                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
//                                }

//                                ColorCount[0] += nTempColorCount0;
//                                ColorCount[1] += nTempColorCount1;
//                                ColorCount[2] += nTempColorCount2;
//                            }
//                            break;
//                        }
//                    case AVG_UV:
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                nTempColorCount0 = 0;
//                                nTempColorCount1 = 0;
//                                nTempColorCount2 = 0;
//                                nPixTotal = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
//                                        nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

//                                        nU = nU < 0 ? 0 : nU;
//                                        nU = nU > 255 ? 255 : nU;
//                                        nV = nV < 0 ? 0 : nV;
//                                        nV = nV > 255 ? 255 : nV;

//                                        nTempColorCount0 += nU;
//                                        nTempColorCount1 += nV;
//                                    }
//                                }
//                                //Mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
//                                        nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

//                                        nU = nU < 0 ? 0 : nU;
//                                        nU = nU > 255 ? 255 : nU;
//                                        nV = nV < 0 ? 0 : nV;
//                                        nV = nV > 255 ? 255 : nV;

//                                        nTempColorCount0 += nU;
//                                        nTempColorCount1 += nV;
//                                    }
//                                }
//                                //Right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
//                                        nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

//                                        nU = nU < 0 ? 0 : nU;
//                                        nU = nU > 255 ? 255 : nU;
//                                        nV = nV < 0 ? 0 : nV;
//                                        nV = nV > 255 ? 255 : nV;

//                                        nTempColorCount0 += nU;
//                                        nTempColorCount1 += nV;
//                                    }
//                                }

//                                //果杯LMR均值
//                                if (nPixTotal != 0)
//                                {
//                                    nOkCupNum++;
//                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;//未四舍五入
//                                    nTempColorCount1 = nTempColorCount1 / nPixTotal;
//                                }

//                                ColorCount[0] += nTempColorCount0;
//                                ColorCount[1] += nTempColorCount1;
//                            }
//                        }
//                        break;
//                    case RATIO_Y:
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                nTempColorCount0 = 0;
//                                nTempColorCount1 = 0;
//                                nTempColorCount2 = 0;
//                                nPixTotal = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
//                                        {
//                                            nTempColorCount0++;
//                                            ucB = colorRGB[0].ucB;
//                                            ucG = colorRGB[0].ucG;
//                                            ucR = colorRGB[0].ucR;
//                                        }
//                                        else if (ucY < unColorIntervals[1])
//                                        {
//                                            nTempColorCount1++;
//                                            ucB = colorRGB[1].ucB;
//                                            ucG = colorRGB[1].ucG;
//                                            ucR = colorRGB[1].ucR;
//                                        }
//                                        else
//                                        {
//                                            nTempColorCount2++;
//                                            ucB = colorRGB[2].ucB;
//                                            ucG = colorRGB[2].ucG;
//                                            ucR = colorRGB[2].ucR;
//                                        }
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //Mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
//                                        {
//                                            nTempColorCount0++;
//                                            ucB = colorRGB[0].ucB;
//                                            ucG = colorRGB[0].ucG;
//                                            ucR = colorRGB[0].ucR;
//                                        }
//                                        else if (ucY < unColorIntervals[1])
//                                        {
//                                            nTempColorCount1++;
//                                            ucB = colorRGB[1].ucB;
//                                            ucG = colorRGB[1].ucG;
//                                            ucR = colorRGB[1].ucR;
//                                        }
//                                        else
//                                        {
//                                            nTempColorCount2++;
//                                            ucB = colorRGB[2].ucB;
//                                            ucG = colorRGB[2].ucG;
//                                            ucR = colorRGB[2].ucR;
//                                        }
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //Right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
//                                        {
//                                            nTempColorCount0++;
//                                            ucB = colorRGB[0].ucB;
//                                            ucG = colorRGB[0].ucG;
//                                            ucR = colorRGB[0].ucR;
//                                        }
//                                        else if (ucY < unColorIntervals[1])
//                                        {
//                                            nTempColorCount1++;
//                                            ucB = colorRGB[1].ucB;
//                                            ucG = colorRGB[1].ucG;
//                                            ucR = colorRGB[1].ucR;
//                                        }
//                                        else
//                                        {
//                                            nTempColorCount2++;
//                                            ucB = colorRGB[2].ucB;
//                                            ucG = colorRGB[2].ucG;
//                                            ucR = colorRGB[2].ucR;
//                                        }
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }

//                                //果杯LMR均值
//                                if (nPixTotal != 0)
//                                {
//                                    nOkCupNum++;
//                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
//                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
//                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
//                                }

//                                ColorCount[0] += nTempColorCount0;
//                                ColorCount[1] += nTempColorCount1;
//                                ColorCount[2] += nTempColorCount2;
//                            }
//                        }
//                        break;
//                    case AVG_Y:
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                nTempColorCount0 = 0;
//                                nTempColorCount1 = 0;
//                                nTempColorCount2 = 0;
//                                nPixTotal = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        nTempColorCount0 += ucY;
//                                    }
//                                }
//                                //Mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        nTempColorCount0 += ucY;
//                                    }
//                                }
//                                //Right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
//                                        nTempColorCount0 += ucY;
//                                    }
//                                }

//                                //果杯LMR均值
//                                if (nPixTotal != 0)
//                                {
//                                    nOkCupNum++;
//                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;
//                                }
//                                ColorCount[0] += nTempColorCount0;
//                            }
//                        }
//                        break;
//                    case RATIO_H:
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                nTempColorCount0 = 0;
//                                nTempColorCount1 = 0;
//                                nTempColorCount2 = 0;
//                                nPixTotal = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nMaxVal = nR > nG ? nR : nG;
//                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
//                                        nMinVal = (nR < nG ? nR : nG);
//                                        nMinVal = nMinVal < nB ? nMinVal : nB;
//                                        if (nMaxVal != nMinVal)
//                                        {
//                                            if (nMaxVal == nR)
//                                            {
//                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
//                                            }
//                                            else if (nMaxVal == nG)
//                                            {
//                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
//                                            }
//                                            else
//                                            {
//                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
//                                            }
//                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
//                                        }
//                                        else
//                                        {
//                                            nH = 0;
//                                        }
//                                        nH = nH >= 360 ? 359 : nH;

//                                        nH += 180;
//                                        nH = nH >= 360 ? nH - 360 : nH;

//                                        if (nH < unColorIntervals[0])
//                                        {
//                                            nTempColorCount0++;
//                                            ucB = colorRGB[0].ucB;
//                                            ucG = colorRGB[0].ucG;
//                                            ucR = colorRGB[0].ucR;
//                                        }
//                                        else if (nH < unColorIntervals[1])
//                                        {
//                                            nTempColorCount1++;
//                                            ucB = colorRGB[1].ucB;
//                                            ucG = colorRGB[1].ucG;
//                                            ucR = colorRGB[1].ucR;
//                                        }
//                                        else
//                                        {
//                                            nTempColorCount2++;
//                                            ucB = colorRGB[2].ucB;
//                                            ucG = colorRGB[2].ucG;
//                                            ucR = colorRGB[2].ucR;
//                                        }
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //Mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nMaxVal = nR > nG ? nR : nG;
//                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
//                                        nMinVal = (nR < nG ? nR : nG);
//                                        nMinVal = nMinVal < nB ? nMinVal : nB;
//                                        if (nMaxVal != nMinVal)
//                                        {
//                                            if (nMaxVal == nR)
//                                            {
//                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
//                                            }
//                                            else if (nMaxVal == nG)
//                                            {
//                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
//                                            }
//                                            else
//                                            {
//                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
//                                            }
//                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
//                                        }
//                                        else
//                                        {
//                                            nH = 0;
//                                        }
//                                        nH = nH >= 360 ? 359 : nH;

//                                        nH += 180;
//                                        nH = nH >= 360 ? nH - 360 : nH;

//                                        if (nH < unColorIntervals[0])
//                                        {
//                                            nTempColorCount0++;
//                                            ucB = colorRGB[0].ucB;
//                                            ucG = colorRGB[0].ucG;
//                                            ucR = colorRGB[0].ucR;
//                                        }
//                                        else if (nH < unColorIntervals[1])
//                                        {
//                                            nTempColorCount1++;
//                                            ucB = colorRGB[1].ucB;
//                                            ucG = colorRGB[1].ucG;
//                                            ucR = colorRGB[1].ucR;
//                                        }
//                                        else
//                                        {
//                                            nTempColorCount2++;
//                                            ucB = colorRGB[2].ucB;
//                                            ucG = colorRGB[2].ucG;
//                                            ucR = colorRGB[2].ucR;
//                                        }
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //Right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nMaxVal = nR > nG ? nR : nG;
//                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
//                                        nMinVal = (nR < nG ? nR : nG);
//                                        nMinVal = nMinVal < nB ? nMinVal : nB;
//                                        if (nMaxVal != nMinVal)
//                                        {
//                                            if (nMaxVal == nR)
//                                            {
//                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
//                                            }
//                                            else if (nMaxVal == nG)
//                                            {
//                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
//                                            }
//                                            else
//                                            {
//                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
//                                            }
//                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
//                                        }
//                                        else
//                                        {
//                                            nH = 0;
//                                        }
//                                        nH = nH >= 360 ? 359 : nH;

//                                        nH += 180;
//                                        nH = nH >= 360 ? nH - 360 : nH;

//                                        if (nH < unColorIntervals[0])
//                                        {
//                                            nTempColorCount0++;
//                                            ucB = colorRGB[0].ucB;
//                                            ucG = colorRGB[0].ucG;
//                                            ucR = colorRGB[0].ucR;
//                                        }
//                                        else if (nH < unColorIntervals[1])
//                                        {
//                                            nTempColorCount1++;
//                                            ucB = colorRGB[1].ucB;
//                                            ucG = colorRGB[1].ucG;
//                                            ucR = colorRGB[1].ucR;
//                                        }
//                                        else
//                                        {
//                                            nTempColorCount2++;
//                                            ucB = colorRGB[2].ucB;
//                                            ucG = colorRGB[2].ucG;
//                                            ucR = colorRGB[2].ucR;
//                                        }
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }

//                                //果杯LMR均值
//                                if (nPixTotal != 0)
//                                {
//                                    nOkCupNum++;
//                                    nTempColorCount0 = (nTempColorCount0 * 100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
//                                    nTempColorCount1 = (nTempColorCount1 * 100 + (nPixTotal >> 1)) / nPixTotal;
//                                    nTempColorCount2 = (nTempColorCount2 * 100 + (nPixTotal >> 1)) / nPixTotal;
//                                }

//                                ColorCount[0] += nTempColorCount0;
//                                ColorCount[1] += nTempColorCount1;
//                                ColorCount[2] += nTempColorCount2;
//                            }
//                        }
//                        break;
//                    case AVG_H:
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                nTempColorCount0 = 0;
//                                nTempColorCount1 = 0;
//                                nTempColorCount2 = 0;
//                                nPixTotal = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nMaxVal = nR > nG ? nR : nG;
//                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
//                                        nMinVal = (nR < nG ? nR : nG);
//                                        nMinVal = nMinVal < nB ? nMinVal : nB;
//                                        if (nMaxVal != nMinVal)
//                                        {
//                                            if (nMaxVal == nR)
//                                            {
//                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
//                                            }
//                                            else if (nMaxVal == nG)
//                                            {
//                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
//                                            }
//                                            else
//                                            {
//                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
//                                            }
//                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
//                                        }
//                                        else
//                                        {
//                                            nH = 0;
//                                        }
//                                        nH = nH >= 360 ? 359 : nH;
//                                        nH = nH + 180 > 360 ? nH - 180 : nH + 180;
//                                        nTempColorCount0 += nH;
//                                    }
//                                }
//                                //Mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nMaxVal = nR > nG ? nR : nG;
//                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
//                                        nMinVal = (nR < nG ? nR : nG);
//                                        nMinVal = nMinVal < nB ? nMinVal : nB;
//                                        if (nMaxVal != nMinVal)
//                                        {
//                                            if (nMaxVal == nR)
//                                            {
//                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
//                                            }
//                                            else if (nMaxVal == nG)
//                                            {
//                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
//                                            }
//                                            else
//                                            {
//                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
//                                            }
//                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
//                                        }
//                                        else
//                                        {
//                                            nH = 0;
//                                        }
//                                        nH = nH >= 360 ? 359 : nH;
//                                        nH = nH + 180 > 360 ? nH - 180 : nH + 180;
//                                        nTempColorCount0 += nH;
//                                    }
//                                }
//                                //Right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        nPixTotal++;
//                                        nB = bSrcImg[nOffset + nX3];
//                                        nG = bSrcImg[nOffset + nX3 + 1];
//                                        nR = bSrcImg[nOffset + nX3 + 2];

//                                        nMaxVal = nR > nG ? nR : nG;
//                                        nMaxVal = nMaxVal > nB ? nMaxVal : nB;
//                                        nMinVal = (nR < nG ? nR : nG);
//                                        nMinVal = nMinVal < nB ? nMinVal : nB;
//                                        if (nMaxVal != nMinVal)
//                                        {
//                                            if (nMaxVal == nR)
//                                            {
//                                                nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
//                                            }
//                                            else if (nMaxVal == nG)
//                                            {
//                                                nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
//                                            }
//                                            else
//                                            {
//                                                nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
//                                            }
//                                            nH = nH1 < 0 ? nH1 + 360 : nH1;
//                                        }
//                                        else
//                                        {
//                                            nH = 0;
//                                        }
//                                        nH = nH >= 360 ? 359 : nH;
//                                        nH = nH + 180 > 360 ? nH - 180 : nH + 180;
//                                        nTempColorCount0 += nH;
//                                    }
//                                }
//                                if (nPixTotal != 0)
//                                {
//                                    nOkCupNum++;
//                                    nTempColorCount0 = nTempColorCount0 / nPixTotal;
//                                }
//                                ColorCount[0] += nTempColorCount0;
//                            }
//                        }
//                        break;

//                    default: break;
//                }

//                if (nOkCupNum > 0)
//                {
//                    ColorCount[0] = (ColorCount[0] + (nOkCupNum >> 1)) / nOkCupNum;
//                    ColorCount[1] = (ColorCount[1] + (nOkCupNum >> 1)) / nOkCupNum;
//                    ColorCount[2] = (ColorCount[2] + (nOkCupNum >> 1)) / nOkCupNum;

//                    int tempSum = ColorCount[0] + ColorCount[1] + ColorCount[2];
//                    if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV && (U5 - U4) * (V5 - V4) + (U3 - U2) * (V3 - V2) + (U1 - U0) * (V1 - V0) == 255 * 255))//保证加和为100
//                    {
//                        if (tempSum > 100)
//                        {
//                            int MaxVal = ColorCount[0] > ColorCount[1] ? ColorCount[0] : ColorCount[1];
//                            MaxVal = MaxVal > ColorCount[2] ? MaxVal : ColorCount[2];
//                            if (MaxVal == ColorCount[0])
//                            {
//                                ColorCount[0] = 100 - ColorCount[1] - ColorCount[2];
//                            }
//                            else if (MaxVal == ColorCount[1])
//                            {
//                                ColorCount[1] = 100 - ColorCount[0] - ColorCount[2];
//                            }
//                            else
//                                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
//                        }
//                        else if (tempSum < 100)
//                        {
//                            int MinVal = ColorCount[0] < ColorCount[1] ? ColorCount[0] : ColorCount[1];
//                            MinVal = MinVal < ColorCount[2] ? MinVal : ColorCount[2];
//                            if (MinVal == ColorCount[0])
//                            {
//                                ColorCount[0] = 100 - ColorCount[1] - ColorCount[2];
//                            }
//                            else if (MinVal == ColorCount[1])
//                            {
//                                ColorCount[1] = 100 - ColorCount[0] - ColorCount[2];
//                            }
//                            else
//                                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
//                        }

//                    }
//                    else if (ColorType == AVG_UV)
//                    {
//                        ColorCount[2] = -1;
//                        bool okFlag = true;
//                        byte avgU = (byte)ColorCount[0];
//                        byte avgV = (byte)ColorCount[1];
//                        if ((U4 <= avgU && avgU <= U5) && (V4 <= avgV && avgV <= V5))
//                        {
//                            ucB = colorRGB[2].ucB;
//                            ucG = colorRGB[2].ucG;
//                            ucR = colorRGB[2].ucR;
//                        }
//                        else if ((U2 <= avgU && avgU <= U3) && (V2 <= avgV && avgV <= V3))
//                        {
//                            ucB = colorRGB[1].ucB;
//                            ucG = colorRGB[1].ucG;
//                            ucR = colorRGB[1].ucR;
//                        }
//                        else if ((U0 <= avgU && avgU <= U1) && (V0 <= avgV && avgV <= V1))
//                        {
//                            ucB = colorRGB[0].ucB;
//                            ucG = colorRGB[0].ucG;
//                            ucR = colorRGB[0].ucR;
//                        }
//                        else
//                            okFlag = false;

//                        if (okFlag)
//                        {
//                            for (int k = 0; k < nCupNum; k++)
//                            {
//                                nOffset = 0;
//                                //Left通道
//                                for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts1[k];
//                                    nRight = nLefts1[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //mid通道
//                                for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts0[k];
//                                    nRight = nLefts0[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                                //right通道
//                                for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                                {
//                                    nLeft = nLefts2[k];
//                                    nRight = nLefts2[k + 1]-1;

//                                    nX3 = nLeft * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nLeft++;
//                                        nX3 += 3;
//                                    }

//                                    nX3 = nRight * 3;
//                                    while (nLeft < nRight)
//                                    {
//                                        if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                        {
//                                            break;
//                                        }
//                                        nRight--;
//                                        nX3 -= 3;
//                                    }

//                                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                    if (nRight < nLeft)
//                                    {
//                                        continue;
//                                    }

//                                    nX3 = nLeft * 3;
//                                    for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                    {
//                                        bSrcImg[nOffset + nX3] = ucB;
//                                        bSrcImg[nOffset + nX3 + 1] = ucG;
//                                        bSrcImg[nOffset + nX3 + 2] = ucR;
//                                    }
//                                }
//                            }
//                        }

//                    }
//                    else if (ColorType == AVG_H || ColorType == AVG_Y)
//                    {
//                        ColorCount[1] = -1;
//                        ColorCount[2] = -1;
//                        if (ColorCount[0] < unColorIntervals[0])
//                        {
//                            ucB = colorRGB[0].ucB;
//                            ucG = colorRGB[0].ucG;
//                            ucR = colorRGB[0].ucR;
//                        }
//                        else if (ColorCount[0] < unColorIntervals[1])
//                        {
//                            ucB = colorRGB[1].ucB;
//                            ucG = colorRGB[1].ucG;
//                            ucR = colorRGB[1].ucR;
//                        }
//                        else
//                        {
//                            ucB = colorRGB[2].ucB;
//                            ucG = colorRGB[2].ucG;
//                            ucR = colorRGB[2].ucR;
//                        }

//                        for (int k = 0; k < nCupNum; k++)
//                        {
//                            nOffset = 0;
//                            //Left通道
//                            for (int j = 0; j < nChannelHs[1]; j++, nOffset += nW3)
//                            {
//                                nLeft = nLefts1[k];
//                                nRight = nLefts1[k + 1]-1;

//                                nX3 = nLeft * 3;
//                                while (nLeft < nRight)
//                                {
//                                    if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                    {
//                                        break;
//                                    }
//                                    nLeft++;
//                                    nX3 += 3;
//                                }

//                                nX3 = nRight * 3;
//                                while (nLeft < nRight)
//                                {
//                                    if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                    {
//                                        break;
//                                    }
//                                    nRight--;
//                                    nX3 -= 3;
//                                }

//                                nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                if (nRight < nLeft)
//                                {
//                                    continue;
//                                }

//                                nX3 = nLeft * 3;
//                                for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                {
//                                    bSrcImg[nOffset + nX3] = ucB;
//                                    bSrcImg[nOffset + nX3 + 1] = ucG;
//                                    bSrcImg[nOffset + nX3 + 2] = ucR;
//                                }
//                            }
//                            //mid通道
//                            for (int j = 0; j < nChannelHs[0]; j++, nOffset += nW3)
//                            {
//                                nLeft = nLefts0[k];
//                                nRight = nLefts0[k + 1]-1;

//                                nX3 = nLeft * 3;
//                                while (nLeft < nRight)
//                                {
//                                    if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                    {
//                                        break;
//                                    }
//                                    nLeft++;
//                                    nX3 += 3;
//                                }

//                                nX3 = nRight * 3;
//                                while (nLeft < nRight)
//                                {
//                                    if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                    {
//                                        break;
//                                    }
//                                    nRight--;
//                                    nX3 -= 3;
//                                }

//                                nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                if (nRight < nLeft)
//                                {
//                                    continue;
//                                }

//                                nX3 = nLeft * 3;
//                                for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                {
//                                    bSrcImg[nOffset + nX3] = ucB;
//                                    bSrcImg[nOffset + nX3 + 1] = ucG;
//                                    bSrcImg[nOffset + nX3 + 2] = ucR;
//                                }
//                            }
//                            //right通道
//                            for (int j = 0; j < nChannelHs[2]; j++, nOffset += nW3)
//                            {
//                                nLeft = nLefts2[k];
//                                nRight = nLefts2[k + 1]-1;

//                                nX3 = nLeft * 3;
//                                while (nLeft < nRight)
//                                {
//                                    if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                    {
//                                        break;
//                                    }
//                                    nLeft++;
//                                    nX3 += 3;
//                                }

//                                nX3 = nRight * 3;
//                                while (nLeft < nRight)
//                                {
//                                    if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
//                                    {
//                                        break;
//                                    }
//                                    nRight--;
//                                    nX3 -= 3;
//                                }
//                                nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
//                                nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
//                                if (nRight < nLeft)
//                                {
//                                    continue;
//                                }

//                                nX3 = nLeft * 3;
//                                for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
//                                {
//                                    bSrcImg[nOffset + nX3] = ucB;
//                                    bSrcImg[nOffset + nX3 + 1] = ucG;
//                                    bSrcImg[nOffset + nX3 + 2] = ucR;
//                                }
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    ColorCount[0] = -1;
//                    ColorCount[1] = -1;
//                    ColorCount[2] = -1;
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine("Commonfunction中函数ColorStatistic24出错" + ex);
//#if REALEASE
//                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数ColorStatistic出错" + ex);
//#endif
//            }
//        }
        ////
        //public static void  ColorStatistic1(ref byte[] bSrcImg,int nImageW,int nImageH,int nCupNum,int ColorType,
        //    ColorRGB[] colorRGB,uint[] unColorIntervals,ref int[] ColorCount)
        //{
        //    byte ucY;
        //    byte ucB=0,ucG=0,ucR=0;
        //    int nB,nG,nR,nU,nV;
        //    int nMinY,nMaxY;
        //    int nH1,nH,nMinVal, nMaxVal,nVertiOffset,nLeft,nRight,nPixTotal,nTempColorCount0,nTempColorCount1,nTempColorCount2,nOkCupNum; 
        //    int erodTimes=4;
        //    uint unMinFruitGray = EXPEND1to4(DEFAULT_MINFRUITGRAY);

        //    //果杯边界	要与IPM一致
        //    int[] nLefts = new int[ConstPreDefine.MAX_CUP_NUM + 1];
        //    int nCupW= 0xfffe & ((nImageW - 20) /nCupNum);
        //    for(int k=0;k<=nCupNum;k++)
        //    {
        //        nLefts[k]=nCupW*k+4;
        //    }

        //    //二值图
        //    byte[] bBinImg=new byte[nImageW*nImageH];
        //    //pbinImg=ucBinImg;
        //    for(int y=0;y<nImageH;y++)
        //    {
        //        for (int x=0;x<nImageW;x++)
        //        {	
        //            byte srcImgA=bSrcImg[(nImageW*y+x)*4];
        //            byte srcImgB=bSrcImg[(nImageW*y+x)*4+1];
        //            byte srcImgC=bSrcImg[(nImageW*y+x)*4+2];
        //            if(!(srcImgA==0&&srcImgB==0&&srcImgC==0))//Y分量判断
        //            {	
        //                bBinImg[y*nImageW+x]=255;
        //            }
        //        }
        //    }
	
        //    //对二值图进行腐蚀，采用与IPM一致的方法
        //    uint[] unEdgePt = new uint[nImageH];
        //   // pbinImg=ucBinImg;
        //    for(int k=0;k<nCupNum;k++)
        //    {
        //        for(int i = 0; i <nImageH; i ++)
        //            unEdgePt[i] = 0x7FFF0000;
        //        nMaxY=0;
        //        nMinY=nImageH;//每一个果杯的边界

        //        for (int i=0;i<nImageH;i++)
        //        {
        //            int index = 0;
        //            for (int j=nLefts[k];j<nLefts[k+1];j++,index++)
        //            {
                        
        //                if(bBinImg[i * nImageW+nLefts[k]+index]!=0)
        //                {
        //                    unEdgePt[i] =(uint)j<<16;//左端点
        //                    if (i<nMinY)
        //                    {
        //                        nMinY=i;//上边界
        //                    }
        //                    if (i>nMaxY)
        //                    {
        //                        nMaxY=i;//下边界
        //                    }
        //                    break;
        //                }
        //            }

        //            index = 0;
        //            for (int j=nLefts[k+1]-1;j>=nLefts[k];j--,index++)
        //            {
        //                if(bBinImg[i * nImageW+nLefts[k+1]-1-index]!=0)
        //                {
        //                    unEdgePt[i] |=(uint)j&0xFFFF;//右端点
        //                    break;
        //                }
        //            }
        //        }

        //        //上下缩减
        //        if (nMinY<nMaxY)
        //        {
        //            erodTimes=(nMaxY-nMinY) > 8 ? erodTimes : 1 ;
        //            for(int y=nMinY;y<nMinY+erodTimes;y++)
        //                for (int x=nLefts[k];x<nLefts[k+1];x++)
        //                {
        //                    bBinImg[y*nImageW+x]=0;
        //                }
        //            for(int y=nMaxY;y>nMaxY-erodTimes;y--)
        //                for (int x=nLefts[k];x<nLefts[k+1];x++)
        //                {
        //                    bBinImg[y*nImageW+x]=0;
        //                }
        //            //左右缩减
        //            for (int y=nMinY+erodTimes;y<=nMaxY-erodTimes;y++)
        //            {
        //                nVertiOffset = y * nImageW+(int)(unEdgePt[y]>>16);
        //                for (int j=0;j<erodTimes;j++)
        //                {
        //                    bBinImg[nVertiOffset+j]=0;
        //                }

        //                nVertiOffset = y * nImageW+(int)(unEdgePt[y]&0xFFFF);
        //                for (int j=0;j<erodTimes;j++)
        //                {
        //                    bBinImg[nVertiOffset-j]=0;
        //                }
        //            }
        //        }
		
		
        //    }

        //    //准确的UV边界
        //    byte L_V0,L_U0,L_V1,L_U1,L_V2,L_U2,L_V3,L_U3,L_V4,L_U4,L_V5,L_U5;
        //    byte U0 = 0, V0 = 0, U1 = 0, V1 = 0, U2 = 0, V2 = 0, U3 = 0, V3 = 0, U4 = 0, V4 = 0, U5 = 0, V5 = 0;
        //    if(ColorType == RATIO_UV || ColorType == AVG_UV)
        //    {
        //        unMinFruitGray &=0xff;
        //        L_U0=(byte)((unColorIntervals[0] & 0xFF000000)>>24);
        //        L_V0=(byte)((unColorIntervals[0] & 0x00FF0000)>>16);
        //        L_U1=(byte)((unColorIntervals[0] & 0x0000FF00)>>8);
        //        L_V1= (byte)(unColorIntervals[0] & 0x000000FF);
        //        if(L_U0>L_U1)
        //        {
        //            U1=L_U0;
        //            U0=L_U1;
        //        }
        //        else
        //        {
        //            U1=L_U1;
        //            U0=L_U0;
        //        }
        //        if(L_V0>L_V1)
        //        {
        //            V1=L_V0;
        //            V0=L_V1;
        //        }
        //        else
        //        {
        //            V1=L_V1;
        //            V0=L_V0;
        //        }

        //        L_U2=(byte)((unColorIntervals[1] & 0xFF000000)>>24);
        //        L_V2=(byte)((unColorIntervals[1] & 0x00FF0000)>>16);
        //        L_U3=(byte)((unColorIntervals[1] & 0x0000FF00)>>8);
        //        L_V3= (byte)(unColorIntervals[1] & 0x000000FF);
        //        if(L_U2>L_U3)
        //        {
        //            U3=L_U2;
        //            U2=L_U3;
        //        }
        //        else
        //        {
        //            U3=L_U3;
        //            U2=L_U2;
        //        }
        //        if(L_V2>L_V3)
        //        {
        //            V3=L_V2;
        //            V2=L_V3;
        //        }
        //        else
        //        {
        //            V3=L_V3;
        //            V2=L_V2;
        //        }


        //        L_U4=(byte)((unColorIntervals[2] & 0xFF000000)>>24);
        //        L_V4=(byte)((unColorIntervals[2] & 0x00FF0000)>>16);
        //        L_U5=(byte)((unColorIntervals[2] & 0x0000FF00)>>8);
        //        L_V5= (byte)(unColorIntervals[2] & 0x000000FF);
        //        if(L_U4>L_U5)
        //        {
        //            U5=L_U4;
        //            U4=L_U5;
        //        }
        //        else
        //        {
        //            U5=L_U5;
        //            U4=L_U4;
        //        }
        //        if(L_V4>L_V5)
        //        {
        //            V5=L_V4;
        //            V4=L_V5;
        //        }
        //        else
        //        {
        //            V5=L_V5;
        //            V4=L_V4;
        //        }
        //    }
        //    else
        //    {
        //        if (unColorIntervals[0]>unColorIntervals[1])
        //        {
        //            uint temp=unColorIntervals[0];
        //            unColorIntervals[0]=unColorIntervals[1];
        //            unColorIntervals[1]=temp;
        //        }
        //    }
        //    ColorCount[0]=0;
        //    ColorCount[1]=0;
        //    ColorCount[2]=0;
        //    nOkCupNum=0;
        //    switch(ColorType)
        //    {
        //        case RATIO_UV : 
        //            { 	
        //                for(int k=0;k<nCupNum;k++)
        //                {
        //                    nPixTotal=0;
        //                    nTempColorCount0=0;
        //                    nTempColorCount1=0;
        //                    nTempColorCount2=0;

        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[ nVertiOffset + nRight] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                       // punImg =bSrcImg[(nVertiOffset+nLeft)*4];
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            nPixTotal++;
        //                            nB=bSrcImg[(nVertiOffset+nLeft)*4+index+2];
        //                            nG=bSrcImg[(nVertiOffset+nLeft)*4+index+1];
        //                            nR=bSrcImg[(nVertiOffset+nLeft)*4+index];
						
        //                            ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB+ SHERU);
        //                            nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU+128);
        //                            nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU+128);

        //                            nU= nU < 0   ? 0  :nU;
        //                            nU= nU>255 ? 255:nU;
        //                            nV= nV<0   ? 0  :nV;
        //                            nV= nV>255 ? 255:nV;

        //                            if((U4<=nU&&nU<=U5)&&(V4<=nV&&nV<=V5))					
        //                            {
        //                                if(ucY>unMinFruitGray)//注意，保留了灰度限制
        //                                {	
        //                                    nTempColorCount2++;
        //                                    ucB=colorRGB[2].ucB;
        //                                    ucG=colorRGB[2].ucG;
        //                                    ucR=colorRGB[2].ucR;
        //                                }
        //                            }
        //                            else if((U2<=nU&&nU<=U3)&&(V2<=nV&&nV<=V3))
        //                            {
        //                                if(ucY>unMinFruitGray)
        //                                {	
        //                                    nTempColorCount1++;
        //                                    ucB=colorRGB[1].ucB;
        //                                    ucG=colorRGB[1].ucG;
        //                                    ucR=colorRGB[1].ucR;
        //                                }
        //                            }
        //                            else if((U0<=nU&&nU<=U1)&&(V0<=nV&&nV<=V1))
        //                            {
        //                                if(ucY>unMinFruitGray)
        //                                {	
        //                                    nTempColorCount0++;
        //                                    ucB=colorRGB[0].ucB;
        //                                    ucG=colorRGB[0].ucG;
        //                                    ucR=colorRGB[0].ucR;
        //                                }
        //                            }
        //                            else
        //                                continue;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index]=ucB;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+1]=ucG;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+2]=ucR;
                                    

        //                        }
        //                    }//每个果杯

        //                    if (nPixTotal!=0)
        //                    {
        //                        nOkCupNum++;
        //                        nTempColorCount0 = (nTempColorCount0*100 + (nPixTotal >> 1)) / nPixTotal;//四舍五入	
        //                        nTempColorCount1 = (nTempColorCount1*100 + (nPixTotal >> 1)) / nPixTotal;	
        //                        nTempColorCount2 = (nTempColorCount2*100 + (nPixTotal >> 1)) / nPixTotal;
        //                    }

        //                    ColorCount[0]+=nTempColorCount0;
        //                    ColorCount[1]+=nTempColorCount1;
        //                    ColorCount[2]+=nTempColorCount2;
        //                }
        //                break;
        //            }
        //        case AVG_UV : 
        //            { 
        //                for(int k=0;k<nCupNum;k++)
        //                {
        //                    nPixTotal=0;
        //                    nTempColorCount0=0;
        //                    nTempColorCount1=0;
        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bSrcImg[ nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bSrcImg[nVertiOffset + nRight] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            nPixTotal++;
        //                            nB=bSrcImg[(nVertiOffset+nLeft)*4+index+2];
        //                            nG=bSrcImg[(nVertiOffset+nLeft)*4+index+1];
        //                            nR=bSrcImg[(nVertiOffset+nLeft)*4+index];

        //                            nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU)+128);
        //                            nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU)+128);

        //                            nU= nU < 0   ? 0  :nU;
        //                            nU= nU>255 ? 255:nU;
        //                            nV= nV<0   ? 0  :nV;
        //                            nV= nV>255 ? 255:nV;

        //                            nTempColorCount0 +=nU;
        //                            nTempColorCount1 +=nV;
        //                            /*ucU = ((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) >> 16);
        //                            ucV = ((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) >> 16);

        //                            nTempColorCount0 +=ucU;
        //                            nTempColorCount1 +=ucV;*/

        //                        }
        //                    }
        //                    if (nPixTotal!=0)
        //                    {
        //                        nOkCupNum++;
        //                        nTempColorCount0 =nTempColorCount0/ nPixTotal;//未四舍五入
        //                        nTempColorCount1 =nTempColorCount1/ nPixTotal;
        //                    }
        //                    ColorCount[0]+=nTempColorCount0;
        //                    ColorCount[1]+=nTempColorCount1;
        //                }
        //            }
        //            break;
        //        case RATIO_Y : 
        //            {   
        //                for(int k=0;k<nCupNum;k++)
        //                {
        //                    nPixTotal=0;
        //                    nTempColorCount0=0;
        //                    nTempColorCount1=0;
        //                    nTempColorCount2=0;
        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if (bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if (bBinImg[nVertiOffset + nRight] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            nPixTotal++;
        //                            ucB=bSrcImg[(nVertiOffset+nLeft)*4+index+2];
        //                            ucG=bSrcImg[(nVertiOffset+nLeft)*4+index+1];
        //                            ucR=bSrcImg[(nVertiOffset+nLeft)*4+index];

        //                            ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB+ SHERU);
        //                            if(ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
        //                            {
        //                                nTempColorCount0++;
        //                                ucB=colorRGB[0].ucB;
        //                                ucG=colorRGB[0].ucG;
        //                                ucR=colorRGB[0].ucR;
        //                            }
        //                            else if(ucY < unColorIntervals[1])
        //                            {
        //                                nTempColorCount1++;
        //                                ucB=colorRGB[1].ucB;
        //                                ucG=colorRGB[1].ucG;
        //                                ucR=colorRGB[1].ucR;
        //                            }
        //                            else
        //                            {
        //                                nTempColorCount2++;
        //                                ucB=colorRGB[2].ucB;
        //                                ucG=colorRGB[2].ucG;
        //                                ucR=colorRGB[2].ucR;
        //                            }
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+2]=ucR;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+1]=ucG;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index]=ucB;
        //                        }
        //                    }
        //                    if (nPixTotal!=0)
        //                    {
        //                        nOkCupNum++;
        //                        nTempColorCount0 = (nTempColorCount0*100 + (nPixTotal >> 1)) / nPixTotal;
        //                        nTempColorCount1 = (nTempColorCount1*100 + (nPixTotal >> 1)) / nPixTotal;	
        //                        nTempColorCount2 = (nTempColorCount2*100 + (nPixTotal >> 1)) / nPixTotal;
        //                    }

        //                    ColorCount[0]+=nTempColorCount0;
        //                    ColorCount[1]+=nTempColorCount1;
        //                    ColorCount[2]+=nTempColorCount2;
        //                }
        //            }
        //            break;
        //        case AVG_Y : 
        //            {   
        //                for(int k=0;k<nCupNum;k++)
        //                {
        //                    nPixTotal=0;
        //                    nTempColorCount0=0;
        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[ nVertiOffset + nRight] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            nPixTotal++;
        //                            ucB=bSrcImg[(nVertiOffset+nLeft)*4+index+2];
        //                            ucG=bSrcImg[(nVertiOffset+nLeft)*4+index+1];
        //                            ucR=bSrcImg[(nVertiOffset+nLeft)*4+index];
        //                            ucY = (byte)(COEFF_Y0 * ucR + COEFF_Y1 * ucG + COEFF_Y2 * ucB+ SHERU);
        //                            nTempColorCount0 += ucY;
        //                        }
        //                    }

        //                    if (nPixTotal!=0)
        //                    {
        //                        nOkCupNum++;
        //                        nTempColorCount0 =nTempColorCount0/ nPixTotal;
        //                    }
        //                    ColorCount[0]+=nTempColorCount0;
        //                }
        //            }
        //            break;
        //        case RATIO_H : 
        //            { 
        //                for(int k=0;k<nCupNum;k++)
        //                {
        //                    nPixTotal=0;
        //                    nTempColorCount0=0;
        //                    nTempColorCount1=0;
        //                    nTempColorCount2=0;
        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nRight] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            nPixTotal++;
        //                            nB=bSrcImg[(nVertiOffset+nLeft)*4+index+2];
        //                            nG=bSrcImg[(nVertiOffset+nLeft)*4+index+1];
        //                            nR=bSrcImg[(nVertiOffset+nLeft)*4+index];

        //                            nMaxVal = nR > nG ? nR : nG;
        //                            nMaxVal = nMaxVal > nB ? nMaxVal : nB;
        //                            nMinVal = (nR < nG ? nR : nG);
        //                            nMinVal = nMinVal < nB ? nMinVal : nB;
        //                            if(nMaxVal != nMinVal)
        //                            {
        //                                if(nMaxVal == nR)
        //                                {
        //                                    nH1 = 60 *(nG - nB) / (nMaxVal - nMinVal);
        //                                }
        //                                else if(nMaxVal == nG)
        //                                {
        //                                    nH1 = 60 *(nB - nR) / (nMaxVal - nMinVal) + 120;
        //                                }
        //                                else 
        //                                {
        //                                    nH1 = 60 *(nR - nG) / (nMaxVal - nMinVal) + 240;
        //                                }
        //                                nH = nH1 < 0 ? nH1 + 360 : nH1;
        //                            }
        //                            else
        //                            {
        //                                nH = 0;
        //                            }
        //                            nH = nH >= 360 ? 359 : nH;

        //                            //nH += 180;
        //                            //nH = nH >= 360 ? nH - 360 : nH;

        //                            if(nH < unColorIntervals[0])
        //                            {
        //                                nTempColorCount0++;
        //                                ucB=colorRGB[0].ucB;
        //                                ucG=colorRGB[0].ucG;
        //                                ucR=colorRGB[0].ucR;
        //                            }
        //                            else if(nH < unColorIntervals[1])
        //                            {
        //                                nTempColorCount1++;
        //                                ucB=colorRGB[1].ucB;
        //                                ucG=colorRGB[1].ucG;
        //                                ucR=colorRGB[1].ucR;
        //                            }
        //                            else 
        //                            {
        //                                nTempColorCount2++;
        //                                ucB=colorRGB[2].ucB;
        //                                ucG=colorRGB[2].ucG;
        //                                ucR=colorRGB[2].ucR;
        //                            }
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+2]=ucR;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+1]=ucG;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index]=ucB;
        //                        }
        //                    }
        //                    if (nPixTotal!=0)
        //                    {
        //                        nOkCupNum++;
        //                        nTempColorCount0 = (nTempColorCount0*100 + (nPixTotal >> 1)) / nPixTotal;
        //                        nTempColorCount1 = (nTempColorCount1*100 + (nPixTotal >> 1)) / nPixTotal;	
        //                        nTempColorCount2 = (nTempColorCount2*100 + (nPixTotal >> 1)) / nPixTotal;
        //                    }

        //                    ColorCount[0]+=nTempColorCount0;
        //                    ColorCount[1]+=nTempColorCount1;
        //                    ColorCount[2]+=nTempColorCount2;
        //                }
        //            }
        //            break;
        //        case AVG_H : 
        //            { 
        //                for(int k=0;k<nCupNum;k++)
        //                {
        //                    nPixTotal=0;
        //                    nTempColorCount0=0;
        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[ nVertiOffset + nRight]== 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            nPixTotal++;
        //                            nB=bSrcImg[(nVertiOffset+nLeft)*4+index+2];
        //                            nG=bSrcImg[(nVertiOffset+nLeft)*4+index+1];
        //                            nR=bSrcImg[(nVertiOffset+nLeft)*4+index];

        //                            nMaxVal = nR > nG ? nR : nG;
        //                            nMaxVal = nMaxVal > nB ? nMaxVal : nB;
        //                            nMinVal = (nR < nG ? nR : nG);
        //                            nMinVal = nMinVal < nB ? nMinVal : nB;
        //                            if(nMaxVal != nMinVal)
        //                            {
        //                                if(nMaxVal == nR)
        //                                {
        //                                    nH1 = 60 *(nG - nB) / (nMaxVal - nMinVal);
        //                                }
        //                                else if(nMaxVal == nG)
        //                                {
        //                                    nH1 = 60 *(nB - nR) / (nMaxVal - nMinVal) + 120;
        //                                }
        //                                else 
        //                                {
        //                                    nH1 = 60 *(nR - nG) / (nMaxVal - nMinVal) + 240;
        //                                }
        //                                nH = nH1 < 0 ? nH1 + 360 : nH1;
        //                            }
        //                            else
        //                            {
        //                                nH = 0;
        //                            }
        //                            nH = nH >= 360 ? 359 : nH;
        //                            nH += 180;
        //                            nH = nH >= 360 ? nH - 360 : nH;
        //                            nTempColorCount0 +=nH;
        //                        }
        //                    }
        //                    if (nPixTotal!=0)
        //                    {
        //                        nOkCupNum++;
        //                        nTempColorCount0 =nTempColorCount0/ nPixTotal;
        //                        nTempColorCount0 += 180;
        //                        nTempColorCount0 = nTempColorCount0>= 360 ? nTempColorCount0 - 360 : nTempColorCount0;
        //                    }
        //                    ColorCount[0]+=nTempColorCount0;
        //                }
        //            }
        //            break;

        //        default : break;
        //    }

        //    if (nOkCupNum>0)
        //    {
        //        ColorCount[0] = (ColorCount[0] + (nOkCupNum >> 1)) / nOkCupNum;	
        //        ColorCount[1] = (ColorCount[1]+ (nOkCupNum >> 1)) / nOkCupNum;
        //        ColorCount[2] = (ColorCount[2] + (nOkCupNum >> 1)) / nOkCupNum;

        //        int tempSum=ColorCount[0]+ColorCount[1]+ColorCount[2];
        //        if (ColorType == RATIO_H || ColorType == RATIO_Y||(ColorType==RATIO_UV&&(U5-U4)*(V5-V4)+(U3-U2)*(V3-V2)+(U1-U0)*(V1-V0)==255*255))//保证加和为100
        //        {
        //            if(tempSum>100)
        //            {
        //                int MaxVal =ColorCount[0]>ColorCount[1]?ColorCount[0]:ColorCount[1];
        //                MaxVal=MaxVal>ColorCount[2]?MaxVal:ColorCount[2];
        //                if (MaxVal==ColorCount[0])
        //                {
        //                    ColorCount[0]=100-ColorCount[1]-ColorCount[2];
        //                } 
        //                else if (MaxVal==ColorCount[1])
        //                {
        //                    ColorCount[1]=100-ColorCount[0]-ColorCount[2];
        //                } 
        //                else
        //                    ColorCount[2]=100-ColorCount[0]-ColorCount[1];
        //            }
        //            else if (tempSum<100)
        //            {
        //                int MinVal =ColorCount[0]<ColorCount[1]?ColorCount[0]:ColorCount[1];
        //                MinVal=MinVal<ColorCount[2]?MinVal:ColorCount[2];
        //                if (MinVal==ColorCount[0])
        //                {
        //                    ColorCount[0]=100-ColorCount[1]-ColorCount[2];
        //                } 
        //                else if (MinVal==ColorCount[1])
        //                {
        //                    ColorCount[1]=100-ColorCount[0]-ColorCount[2];
        //                } 
        //                else
        //                    ColorCount[2]=100-ColorCount[0]-ColorCount[1];
        //            }

        //        }
        //        else if(ColorType==AVG_UV)
        //        {
        //            ColorCount[2]=-1;
        //            bool okFlag=true;
        //            byte avgU=(byte)ColorCount[0];
        //            byte avgV=(byte)ColorCount[1];
        //            if((U4<=avgU&&avgU<=U5)&&(V4<=avgV&&avgV<=V5))		
        //            {
        //                ucB=colorRGB[2].ucB;
        //                ucG=colorRGB[2].ucG;
        //                ucR=colorRGB[2].ucR;
        //            }
        //            else if((U2<=avgU&&avgU<=U3)&&(V2<=avgV&&avgV<=V3))
        //            {
        //                ucB=colorRGB[1].ucB;
        //                ucG=colorRGB[1].ucG;
        //                ucR=colorRGB[1].ucR;
        //            }
        //            else if((U0<=avgU&&avgU<=U1)&&(V0<=avgV&&avgV<=V1))
        //            {
        //                ucB=colorRGB[0].ucB;
        //                ucG=colorRGB[0].ucG;
        //                ucR=colorRGB[0].ucR;
        //            }
        //            else
        //                okFlag=false;

        //            if(okFlag)
        //            {
        //                for (int k=0;k<nCupNum;k++)
        //                {
        //                    for (int i=0;i<nImageH;i++)
        //                    {
        //                        nVertiOffset =i * nImageW;
        //                        nLeft  =nLefts[k] ;  
        //                        nRight =nLefts[k+1];

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nLeft++;
        //                        }

        //                        while(nLeft < nRight)
        //                        {
        //                            if(bBinImg[nVertiOffset + nRight] == 255) //mask 像素有效
        //                            {
        //                                break;
        //                            }
        //                            nRight--;
        //                        }

        //                        nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                        nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                        if(nRight < nLeft)
        //                        {
        //                            continue;
        //                        }

        //                        int index=0;
        //                        for(int nX = nLeft; nX < nRight; nX ++,index+=4)
        //                        {
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+2]=ucR;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index+1]=ucG;
        //                            bSrcImg[(nVertiOffset+nLeft)*4+index]=ucB;
        //                        }
        //                    }
        //                }	
        //            }

        //        }
        //        else if(ColorType==AVG_H||ColorType==AVG_Y)
        //        {
        //            ColorCount[1] = -1;
        //            ColorCount[2] = -1;
        //            if (ColorCount[0] < unColorIntervals[0])
        //            {
        //                ucB=colorRGB[0].ucB;
        //                ucG=colorRGB[0].ucG;
        //                ucR=colorRGB[0].ucR;
        //            }
        //            else if (ColorCount[0] < unColorIntervals[1])
        //            {
        //                ucB=colorRGB[1].ucB;
        //                ucG=colorRGB[1].ucG;
        //                ucR=colorRGB[1].ucR;
        //            }
        //            else 
        //            {
        //                ucB=colorRGB[2].ucB;
        //                ucG=colorRGB[2].ucG;
        //                ucR=colorRGB[2].ucR;
        //            }

        //            for (int k=0;k<nCupNum;k++)
        //            {
        //                for (int i=0;i<nImageH;i++)
        //                {
        //                    nVertiOffset =i * nImageW;
        //                    nLeft  =nLefts[k] ;  
        //                    nRight =nLefts[k+1];

        //                    while(nLeft < nRight)
        //                    {
        //                        if(bBinImg[nVertiOffset + nLeft] == 255) //mask 像素有效
        //                        {
        //                            break;
        //                        }
        //                        nLeft++;
        //                    }

        //                    while(nLeft < nRight)
        //                    {
        //                        if(bBinImg[nVertiOffset + nRight] == 255) //mask 像素有效
        //                        {
        //                            break;
        //                        }
        //                        nRight--;
        //                    }

        //                    nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
        //                    nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
        //                    if(nRight < nLeft)
        //                    {
        //                        continue;
        //                    }

        //                    for(int nX = nLeft; nX < nRight; nX ++)
        //                    {
        //                        bSrcImg[(nVertiOffset+nX)*4+2] =ucR;
        //                        bSrcImg[(nVertiOffset+nX)*4+1] =ucG;
        //                        bSrcImg[(nVertiOffset+nX)*4] =ucB;
        //                    }
        //                }
        //            }	
        //        }	
        //    }
        //    else
        //    {
        //        ColorCount[0] = -1;
        //        ColorCount[1] = -1;
        //        ColorCount[2] = -1;
        //    }
        //}

public static void GetCupImgOutRect(byte[] bSrcImg,int nImageW3,int nImageH,int nLeft,int nRight,ref int nMinX,ref int nMaxX,ref int nMinY,ref int nMaxY)
{
	bool bExist;
    int nOffsetTemp, nOffset, nX3;

	//求nMinY
	nOffset=0;
	nMinY=0;
	bExist=false;
	for (;nMinY<nImageH;nMinY++,nOffset += nImageW3)
	{
		nX3 = nLeft * 3;
		for (int x=nLeft;x<nRight;x++,nX3+=3)
		{
			if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				bExist=true;
				break;
			}
		}
		if (bExist)
		{
			break;
		}
	}
	//求nMaxY
	nMaxY=nImageH-1;
    nOffset = (nImageH - 1) * nImageW3;
	bExist=false;
	for (;nMaxY>=nMinY;nMaxY--, nOffset-= nImageW3)
	{
		nX3 = nLeft * 3;
		for (int x=nLeft;x<nRight;x++,nX3+=3)
		{
			if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				bExist=true;
				break;
			}
		}
		if (bExist)
		{
			break;
		}
	}

	//求nMinX
	nX3 = nLeft * 3;
	nMinX=nLeft;
	bExist=false;
	nOffset=nMinY*nImageW3;
	for (;nMinX<nRight;nMinX++,nX3+=3)
	{
		nOffsetTemp=nOffset;
		for (int y=nMinY;y<=nMaxY;y++,nOffsetTemp+=nImageW3)
		{
			if (bSrcImg[nOffsetTemp + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				bExist=true;
				break;
			}
		}
		if (bExist)
		{
			break;
		}
	}
	//求nMaxX
	nX3 = (nRight-1) * 3;
	nMaxX=nRight-1;
	bExist=false;
	nOffset=nMinY*nImageW3;
	for (;nMaxX>=nMinX;nMaxX--,nX3-=3)
	{
		nOffsetTemp=nOffset;
		for (int y=nMinY;y<=nMaxY;y++,nOffsetTemp+=nImageW3)
		{
			if (bSrcImg[nOffsetTemp + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				bExist=true;
				break;
			}
		}
		if (bExist)
		{
			break;
		}
	}
}

public static void CupImgColorStatistic(ref byte[] bSrcImg,int nImageW3,int nImageH,int nMinX,int nMaxX,int nMinY,int nMaxY,int nCutLeft,int nCutRight,int ColorType,ColorRGB[] colorRGB, uint[] unColorIntervals, ref int[] ColorTemp)
{
    int nLeft, nRight, nX3, nOffset, nH1, nH, nMinVal, nMaxVal, nB, nG,nR,nU,nV;
    byte ucY, ucU, ucV,ucR,ucG,ucB;


    //准确的UV边界
    byte L_V0, L_U0, L_V1, L_U1, L_V2, L_U2, L_V3, L_U3, L_V4, L_U4, L_V5, L_U5;
    byte U0 = 0, V0 = 0, U1 = 0, V1 = 0, U2 = 0, V2 = 0, U3 = 0, V3 = 0, U4 = 0, V4 = 0, U5 = 0, V5 = 0;
    if (ColorType == RATIO_UV || ColorType == AVG_UV)
    {
        L_U0 = (byte)((unColorIntervals[0] & 0xFF000000) >> 24);
        L_V0 = (byte)((unColorIntervals[0] & 0x00FF0000) >> 16);
        L_U1 = (byte)((unColorIntervals[0] & 0x0000FF00) >> 8);
        L_V1 = (byte)(unColorIntervals[0] & 0x000000FF);
        if (L_U0 > L_U1)
        {
            U1 = L_U0;
            U0 = L_U1;
        }
        else
        {
            U1 = L_U1;
            U0 = L_U0;
        }
        if (L_V0 > L_V1)
        {
            V1 = L_V0;
            V0 = L_V1;
        }
        else
        {
            V1 = L_V1;
            V0 = L_V0;
        }

        L_U2 = (byte)((unColorIntervals[1] & 0xFF000000) >> 24);
        L_V2 = (byte)((unColorIntervals[1] & 0x00FF0000) >> 16);
        L_U3 = (byte)((unColorIntervals[1] & 0x0000FF00) >> 8);
        L_V3 = (byte)(unColorIntervals[1] & 0x000000FF);
        if (L_U2 > L_U3)
        {
            U3 = L_U2;
            U2 = L_U3;
        }
        else
        {
            U3 = L_U3;
            U2 = L_U2;
        }
        if (L_V2 > L_V3)
        {
            V3 = L_V2;
            V2 = L_V3;
        }
        else
        {
            V3 = L_V3;
            V2 = L_V2;
        }


        L_U4 = (byte)((unColorIntervals[2] & 0xFF000000) >> 24);
        L_V4 = (byte)((unColorIntervals[2] & 0x00FF0000) >> 16);
        L_U5 = (byte)((unColorIntervals[2] & 0x0000FF00) >> 8);
        L_V5 = (byte)(unColorIntervals[2] & 0x000000FF);
        if (L_U4 > L_U5)
        {
            U5 = L_U4;
            U4 = L_U5;
        }
        else
        {
            U5 = L_U5;
            U4 = L_U4;
        }
        if (L_V4 > L_V5)
        {
            V5 = L_V4;
            V4 = L_V5;
        }
        else
        {
            V5 = L_V5;
            V4 = L_V4;
        }
    }
    else
    {
        if (unColorIntervals[0] > unColorIntervals[1])
        {
            uint temp = unColorIntervals[0];
            unColorIntervals[0] = unColorIntervals[1];
            unColorIntervals[1] = temp;
        }
    }


	ColorTemp[0] = 0;
	ColorTemp[1] = 0;
	ColorTemp[2] = 0;

	nOffset=nMinY*nImageW3;
	for (int j =nMinY; j<=nMaxY; j++, nOffset += nImageW3)
	{
		nLeft = nMinX;
		nRight =nMaxX;
		nX3 = nLeft * 3;
		while (nLeft < nRight)
		{
			if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				break;
			}
			nLeft++;
			nX3 += 3;
		}
		nX3 = nRight * 3;
		while (nLeft < nRight)
		{
			if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				break;
			}
			nRight--;
			nX3 -= 3;
		}
		nLeft=nLeft>nCutLeft?nLeft:nCutLeft;
		nRight=nRight<nCutRight?nRight:nCutRight;
		nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
		nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
		if (nRight < nLeft)
		{
			continue;
		}

		nX3 = nLeft * 3;
		for (int nX = nLeft; nX < nRight; nX++, nX3 += 3)
		{
			nB = bSrcImg[nOffset + nX3];
			nG = bSrcImg[nOffset + nX3 + 1];
			nR = bSrcImg[nOffset + nX3 + 2];
			switch (ColorType)
			{
			case RATIO_UV:
				ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
				nU = (int)(COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU + 128);
				nV = (int)(COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU + 128);

				nU = nU < 0 ? 0 : nU;
				nU = nU > 255 ? 255 : nU;
				nV = nV < 0 ? 0 : nV;
				nV = nV > 255 ? 255 : nV;
				if ((U4 <= nU && nU <= U5) && (V4 <= nV && nV <= V5))
				{
					ColorTemp[2]++;
					ucB = colorRGB[2].ucB;
					ucG = colorRGB[2].ucG;
					ucR = colorRGB[2].ucR;
				}
				else if ((U2 <= nU && nU <= U3) && (V2 <= nV && nV <= V3))
				{
					ColorTemp[1]++;
					ucB = colorRGB[1].ucB;
					ucG = colorRGB[1].ucG;
					ucR = colorRGB[1].ucR;
				}
				else if ((U0 <= nU && nU <= U1) && (V0 <= nV && nV <= V1))
				{
					
					ColorTemp[0]++;
					ucB = colorRGB[0].ucB;
					ucG = colorRGB[0].ucG;
					ucR = colorRGB[0].ucR;
				}
				else
					continue;
				bSrcImg[nOffset + nX3] = ucB;
				bSrcImg[nOffset + nX3 + 1] = ucG;
				bSrcImg[nOffset + nX3 + 2] = ucR;
			break;
			case AVG_UV:
				nU = (int)((COEFF_U0 * nR + COEFF_U1 * nG + COEFF_U2 * nB + SHERU) + 128);
				nV = (int)((COEFF_V0 * nR + COEFF_V1 * nG + COEFF_V2 * nB + SHERU) + 128);

				nU = nU < 0 ? 0 : nU;
				nU = nU > 255 ? 255 : nU;
				nV = nV < 0 ? 0 : nV;
				nV = nV > 255 ? 255 : nV;

				ColorTemp[0] += nU;
				ColorTemp[1] += nV;
				ColorTemp[2]++;
			break;
			case RATIO_Y:
				ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
				if (ucY < unColorIntervals[0])//根据第二个像素来赋值颜色
				{
					ColorTemp[0]++;
					ucB = colorRGB[0].ucB;
					ucG = colorRGB[0].ucG;
					ucR = colorRGB[0].ucR;
				}
				else if (ucY < unColorIntervals[1])
				{
					ColorTemp[1]++;
					ucB = colorRGB[1].ucB;
					ucG = colorRGB[1].ucG;
					ucR = colorRGB[1].ucR;
				}
				else
				{
					ColorTemp[2]++;
					ucB = colorRGB[2].ucB;
					ucG = colorRGB[2].ucG;
					ucR = colorRGB[2].ucR;
				}
				bSrcImg[nOffset + nX3] = ucB;
				bSrcImg[nOffset + nX3 + 1] = ucG;
				bSrcImg[nOffset + nX3 + 2] = ucR;
			break;
			case AVG_Y:
				ucY = (byte)(COEFF_Y0 * nR + COEFF_Y1 * nG + COEFF_Y2 * nB + SHERU);
				ColorTemp[0] += ucY;
				ColorTemp[2]++;
			break;
			case RATIO_H:
				nMaxVal = nR > nG ? nR : nG;
				nMaxVal = nMaxVal > nB ? nMaxVal : nB;
				nMinVal = (nR < nG ? nR : nG);
				nMinVal = nMinVal < nB ? nMinVal : nB;
				if (nMaxVal != nMinVal)
				{
					if (nMaxVal == nR)
					{
						nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
					}
					else if (nMaxVal == nG)
					{
						nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
					}
					else
					{
						nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
					}
					nH = nH1 < 0 ? nH1 + 360 : nH1;
				}
				else
				{
					nH = 0;
				}
				nH = nH >= 360 ? 359 : nH;

				nH += 180;
				nH = nH >= 360 ? nH - 360 : nH;

				if (nH < unColorIntervals[0])
				{
					ColorTemp[0]++;
					ucB = colorRGB[0].ucB;
					ucG = colorRGB[0].ucG;
					ucR = colorRGB[0].ucR;
				}
				else if (nH < unColorIntervals[1])
				{
					ColorTemp[1]++;
					ucB = colorRGB[1].ucB;
					ucG = colorRGB[1].ucG;
					ucR = colorRGB[1].ucR;
				}
				else
				{
					ColorTemp[2]++;
					ucB = colorRGB[2].ucB;
					ucG = colorRGB[2].ucG;
					ucR = colorRGB[2].ucR;
				}
				bSrcImg[nOffset + nX3] = ucB;
				bSrcImg[nOffset + nX3 + 1] = ucG;
				bSrcImg[nOffset + nX3 + 2] = ucR;
			break;
			case AVG_H:
				nMaxVal = nR > nG ? nR : nG;
				nMaxVal = nMaxVal > nB ? nMaxVal : nB;
				nMinVal = (nR < nG ? nR : nG);
				nMinVal = nMinVal < nB ? nMinVal : nB;
				if (nMaxVal != nMinVal)
				{
					if (nMaxVal == nR)
					{
						nH1 = 60 * (nG - nB) / (nMaxVal - nMinVal);
					}
					else if (nMaxVal == nG)
					{
						nH1 = 60 * (nB - nR) / (nMaxVal - nMinVal) + 120;
					}
					else
					{
						nH1 = 60 * (nR - nG) / (nMaxVal - nMinVal) + 240;
					}
					nH = nH1 < 0 ? nH1 + 360 : nH1;
				}
				else
				{
					nH = 0;
				}
				nH = nH >= 360 ? 359 : nH;
				nH = nH + 180 > 360 ? nH - 180 : nH + 180;
				ColorTemp[0] += nH;
				ColorTemp[2]++;
			break;
			default: break;
			}
		}
	}
}

public static void CupImgDrawColor(ref byte[] bSrcImg,int nImageW3,int nImageH,int nMinX,int nMaxX,int nMinY,int nMaxY,int nCutLeft,int nCutRight,byte ucR,byte ucG,byte ucB)
{
	int nLeft,nRight,nX3,nOffset;
	nOffset=nMinY*nImageW3;
	for (int j =nMinY; j<=nMaxY; j++, nOffset += nImageW3)
	{
		nLeft = nMinX;
		nRight =nMaxX;
		nX3 = nLeft * 3;
		while (nLeft < nRight)
		{
			if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				break;
			}
			nLeft++;
			nX3 += 3;
		}
		nX3 = nRight * 3;
		while (nLeft < nRight)
		{
			if (bSrcImg[nOffset + nX3] > 0 || bSrcImg[nOffset + nX3 + 1] > 0 || bSrcImg[nOffset + nX3 + 2] > 0)
			{
				break;
			}
			nRight--;
			nX3 -= 3;
		}
        nLeft = nLeft > nCutLeft ? nLeft : nCutLeft;
		nRight=nRight<nCutRight?nRight:nCutRight;
		nLeft = (int)((nLeft + 1) & 0xfffffffe);   //消除边缘临界点
		nRight = (int)((nRight - 1) & 0xfffffffe); //消除边缘临界点
		if (nRight < nLeft)
		{
			continue;
		}
		nX3=nLeft*3;
		for (int nX = nLeft; nX < nRight; nX++,nX3+=3)
		{
			bSrcImg[nOffset+nX3] = ucB;
			bSrcImg[nOffset+nX3+1] = ucG;
			bSrcImg[nOffset+nX3+2] = ucR;
		}
	}
}

public static void ColorStatistic24(ref byte[] bSrcImg, int nImageW, int[] nChannelHs, int nCupNum, int[] nLefts1, int[] nLefts0, int[] nLefts2,int nMidLen,float fCutY,int ColorType,
	ColorRGB[] colorRGB, uint[] unColorIntervals, ref int[] ColorCount)
{

	int nOffset,nW3,nX3,nCx,nPerimeter,nPixTotal;
    byte ucB=0, ucG=0, ucR=0;
    int[] nMinX = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nMaxX = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nMinY = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nMaxY = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nCutLeft = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nCutRight = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];

	int[] nMinX2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nMaxX2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nMinY2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nMaxY2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nCutLeft2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];
	int[] nCutRight2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM];

	int[] nTempColorCount = new int[3];
	try
	{
		//准确的UV边界
		byte L_V0, L_U0, L_V1, L_U1, L_V2, L_U2, L_V3, L_U3, L_V4, L_U4, L_V5, L_U5;
		byte U0 = 0, V0 = 0, U1 = 0, V1 = 0, U2 = 0, V2 = 0, U3 = 0, V3 = 0, U4 = 0, V4 = 0, U5 = 0, V5 = 0;
		if (ColorType == RATIO_UV || ColorType == AVG_UV)
		{
			L_U0 = (byte)((unColorIntervals[0] & 0xFF000000) >> 24);
			L_V0 = (byte)((unColorIntervals[0] & 0x00FF0000) >> 16);
			L_U1 = (byte)((unColorIntervals[0] & 0x0000FF00) >> 8);
			L_V1 = (byte)(unColorIntervals[0] & 0x000000FF);
			if (L_U0 > L_U1)
			{
				U1 = L_U0;
				U0 = L_U1;
			}
			else
			{
				U1 = L_U1;
				U0 = L_U0;
			}
			if (L_V0 > L_V1)
			{
				V1 = L_V0;
				V0 = L_V1;
			}
			else
			{
				V1 = L_V1;
				V0 = L_V0;
			}

			L_U2 = (byte)((unColorIntervals[1] & 0xFF000000) >> 24);
			L_V2 = (byte)((unColorIntervals[1] & 0x00FF0000) >> 16);
			L_U3 = (byte)((unColorIntervals[1] & 0x0000FF00) >> 8);
			L_V3 = (byte)(unColorIntervals[1] & 0x000000FF);
			if (L_U2 > L_U3)
			{
				U3 = L_U2;
				U2 = L_U3;
			}
			else
			{
				U3 = L_U3;
				U2 = L_U2;
			}
			if (L_V2 > L_V3)
			{
				V3 = L_V2;
				V2 = L_V3;
			}
			else
			{
				V3 = L_V3;
				V2 = L_V2;
			}


			L_U4 = (byte)((unColorIntervals[2] & 0xFF000000) >> 24);
			L_V4 = (byte)((unColorIntervals[2] & 0x00FF0000) >> 16);
			L_U5 = (byte)((unColorIntervals[2] & 0x0000FF00) >> 8);
			L_V5 = (byte)(unColorIntervals[2] & 0x000000FF);
			if (L_U4 > L_U5)
			{
				U5 = L_U4;
				U4 = L_U5;
			}
			else
			{
				U5 = L_U5;
				U4 = L_U4;
			}
			if (L_V4 > L_V5)
			{
				V5 = L_V4;
				V4 = L_V5;
			}
			else
			{
				V5 = L_V5;
				V4 = L_V4;
			}
		}
		else
		{
			if (unColorIntervals[0] > unColorIntervals[1])
			{
				uint temp = unColorIntervals[0];
				unColorIntervals[0] = unColorIntervals[1];
				unColorIntervals[1] = temp;
			}
		}


		nW3 = nImageW * 3;
		ColorCount[0] = 0;
		ColorCount[1] = 0;
		ColorCount[2] = 0;
		nPixTotal=0;
        nPerimeter = 0;
		for (int k = 0; k < nCupNum; k++)
		{
			if(nChannelHs[1]>0&&nChannelHs[2]>0)
			{
				//Left通道
                GetCupImgOutRect(bSrcImg, nW3, nChannelHs[1], nLefts1[k], nLefts1[k + 1], ref nMinX[k], ref nMaxX[k], ref nMinY[k], ref nMaxY[k]);
				nMaxY[k]=nMaxY[k]-(int)((nMaxY[k]-nMinY[k]+1)*fCutY);
				nCx=nMinX[k]+(nMaxX[k]-nMinX[k]>>1);
				if (k==0)
				{
					nPerimeter=(int)(3.1415926535897932384626433832795f*(nMaxX[k]-nMinX[k]));
				}
						
				nCutLeft[k]=nCx-nMidLen;
				nCutRight[k]=(nCx+nMidLen)<(nCutLeft[k]+nPerimeter-(k*nMidLen<<1))?(nCx+nMidLen):(nCutLeft[k]+nPerimeter-(k*nMidLen<<1));
                CupImgColorStatistic(ref bSrcImg, nW3, nChannelHs[1], nMinX[k], nMaxX[k], nMinY[k], nMaxY[k], nCutLeft[k], nCutRight[k], ColorType, colorRGB, unColorIntervals, ref nTempColorCount);	
				if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV))
				{
					ColorCount[0]+=nTempColorCount[0];
					ColorCount[1]+=nTempColorCount[1];
					ColorCount[2]+=nTempColorCount[2];
					nPixTotal+=nTempColorCount[0]+nTempColorCount[1]+nTempColorCount[2];
				}
				else 
				{
                    ColorCount[0] += nTempColorCount[0];
                    ColorCount[1] += nTempColorCount[1];
					nPixTotal+=nTempColorCount[2];
				}
				
				//Right通道
				byte[] bSrcImgRight = new byte[nChannelHs[2]*nW3];
				Array.Copy(bSrcImg,(nChannelHs[0]+nChannelHs[1])*nW3,bSrcImgRight,0,nChannelHs[2]*nW3);
                GetCupImgOutRect(bSrcImgRight, nW3, nChannelHs[2], nLefts2[k], nLefts2[k + 1], ref nMinX2[k], ref nMaxX2[k], ref nMinY2[k], ref nMaxY2[k]);
				nMinY2[k]=(int)(nMinY2[k]+(nMaxY2[k]-nMinY2[k]+1)*fCutY);
				nCx=nMinX2[k]+(nMaxX2[k]-nMinX2[k]>>1);
	
				nCutLeft2[k]=nCx-nMidLen;
				nCutRight2[k]=(nCx+nMidLen)<(nCutLeft2[k]+nPerimeter-(k*nMidLen<<1))?(nCx+nMidLen):(nCutLeft2[k]+nPerimeter-(k*nMidLen<<1));
                CupImgColorStatistic(ref bSrcImgRight, nW3, nChannelHs[2], nMinX2[k], nMaxX2[k], nMinY2[k], nMaxY2[k], nCutLeft2[k], nCutRight2[k], ColorType, colorRGB, unColorIntervals, ref nTempColorCount);
                Array.Copy(bSrcImgRight, 0, bSrcImg, (nChannelHs[0] + nChannelHs[1]) * nW3, nChannelHs[2] * nW3);
                if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV))
				{
					ColorCount[0]+=nTempColorCount[0];
					ColorCount[1]+=nTempColorCount[1];
					ColorCount[2]+=nTempColorCount[2];
					nPixTotal+=nTempColorCount[0]+nTempColorCount[1]+nTempColorCount[2];
				}
				else 
				{
                    ColorCount[0] += nTempColorCount[0];
                    ColorCount[1] += nTempColorCount[1];
					nPixTotal+=nTempColorCount[2];
				}

			}
			else
			{
				//Mid通道
				byte[] bSrcImgMid = new byte[nChannelHs[0]*nW3];
				Array.Copy(bSrcImg,nChannelHs[1]*nW3,bSrcImgMid,0,nChannelHs[0]*nW3);
                GetCupImgOutRect(bSrcImgMid, nW3, nChannelHs[0], nLefts0[k], nLefts0[k + 1], ref nMinX[k], ref nMaxX[k], ref nMinY[k], ref nMaxY[k]);
				nCx=nMinX[k]+(nMaxX[k]-nMinX[k]>>1);
				if (k==0)
				{
					nPerimeter=(int)(3.1415926535897932384626433832795f*(nMaxX[k]-nMinX[k]));
				}

				nCutLeft[k]=nCx-nMidLen;
				nCutRight[k]=(nCx+nMidLen)<(nCutLeft[k]+nPerimeter-(k*nMidLen<<1))?(nCx+nMidLen):(nCutLeft[k]+nPerimeter-(k*nMidLen<<1));
                CupImgColorStatistic(ref bSrcImgMid, nW3, nChannelHs[0], nMinX[k], nMaxX[k], nMinY[k], nMaxY[k], nCutLeft[k], nCutRight[k], ColorType, colorRGB, unColorIntervals, ref nTempColorCount);
                Array.Copy(bSrcImgMid, 0, bSrcImg, nChannelHs[1] * nW3, nChannelHs[0] * nW3);
				if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV))
				{
					ColorCount[0]+=nTempColorCount[0];
					ColorCount[1]+=nTempColorCount[1];
					ColorCount[2]+=nTempColorCount[2];
					nPixTotal+=nTempColorCount[0]+nTempColorCount[1]+nTempColorCount[2];
				}
				else 
				{
					ColorCount[0]+=nTempColorCount[0];
					ColorCount[1]+=nTempColorCount[1];
					nPixTotal+=nTempColorCount[2];
				}
			}
		}
		
		if (nPixTotal>0)
		{
			if (ColorType == RATIO_H || ColorType == RATIO_Y || (ColorType == RATIO_UV))
			{
                ColorCount[0] = (100*ColorCount[0] + (nPixTotal >> 1)) / nPixTotal;
                ColorCount[1] = (100*ColorCount[1] + (nPixTotal >> 1)) / nPixTotal;
                ColorCount[2] = 100 - ColorCount[0] - ColorCount[1];
                if (ColorCount[2] < 0)
                {
                    if (ColorCount[0] > ColorCount[1])
                        ColorCount[0] = 100 - ColorCount[1];
                    else
                        ColorCount[1] = 100 - ColorCount[0];
                    ColorCount[2] = 0;
                }
			}
			else if(ColorType == AVG_UV)
			{
				ColorCount[0] = (ColorCount[0] + (nPixTotal >> 1)) / nPixTotal;
				ColorCount[1] = (ColorCount[1] + (nPixTotal >> 1)) / nPixTotal;
				ColorCount[2] = -1;
				bool okFlag=true;
				byte avgU = (byte)ColorCount[0];
				byte avgV = (byte)ColorCount[1];
				if ((U4 <= avgU && avgU <= U5) && (V4 <= avgV && avgV <= V5))
				{
					ucB = colorRGB[2].ucB;
					ucG = colorRGB[2].ucG;
					ucR = colorRGB[2].ucR;
				}
				else if ((U2 <= avgU && avgU <= U3) && (V2 <= avgV && avgV <= V3))
				{
					ucB = colorRGB[1].ucB;
					ucG = colorRGB[1].ucG;
					ucR = colorRGB[1].ucR;
				}
				else if ((U0 <= avgU && avgU <= U1) && (V0 <= avgV && avgV <= V1))
				{
					ucB = colorRGB[0].ucB;
					ucG = colorRGB[0].ucG;
					ucR = colorRGB[0].ucR;
				}
				else
					okFlag = false;

				if (okFlag)
				{
                    for (int k = 0; k < nCupNum; k++)
                    {
                        if (nChannelHs[1] > 0 && nChannelHs[2] > 0)
                        {
                            CupImgDrawColor(ref bSrcImg, nW3, nChannelHs[1], nMinX[k], nMaxX[k], nMinY[k], nMaxY[k], nCutLeft[k], nCutRight[k], ucR, ucG, ucB);
                            byte[] bSrcImgRight = new byte[nChannelHs[2] * nW3];
                            Array.Copy(bSrcImg, (nChannelHs[0] + nChannelHs[1]) * nW3, bSrcImgRight, 0, nChannelHs[2] * nW3);
                            CupImgDrawColor(ref bSrcImgRight, nW3, nChannelHs[2], nMinX2[k], nMaxX2[k], nMinY2[k], nMaxY2[k], nCutLeft2[k], nCutRight2[k], ucR, ucG, ucB);
                            Array.Copy(bSrcImgRight, 0, bSrcImg, (nChannelHs[0] + nChannelHs[1]) * nW3, nChannelHs[2] * nW3);
                        }
                        else
                        {
                            byte[] bSrcImgMid = new byte[nChannelHs[0] * nW3];
                            Array.Copy(bSrcImg, nChannelHs[1] * nW3, bSrcImgMid, 0, nChannelHs[0] * nW3);
                            CupImgDrawColor(ref  bSrcImgMid, nW3, nChannelHs[0], nMinX[k], nMaxX[k], nMinY[k], nMaxY[k], nCutLeft[k], nCutRight[k], ucR, ucG, ucB);
                            Array.Copy(bSrcImgMid, 0, bSrcImg, nChannelHs[1] * nW3, nChannelHs[0] * nW3);
                        }
                    }       
				}
			}
			else
			{
				ColorCount[0] = (ColorCount[0] + (nPixTotal >> 1)) / nPixTotal;
				ColorCount[1] = -1;
				ColorCount[2] = -1;
				if (ColorCount[0] < unColorIntervals[0])
				{
					ucB = colorRGB[0].ucB;
					ucG = colorRGB[0].ucG;
					ucR = colorRGB[0].ucR;
				}
				else if (ColorCount[0] < unColorIntervals[1])
				{
					ucB = colorRGB[1].ucB;
					ucG = colorRGB[1].ucG;
					ucR = colorRGB[1].ucR;
				}
				else
				{
					ucB = colorRGB[2].ucB;
					ucG = colorRGB[2].ucG;
					ucR = colorRGB[2].ucR;
				}

                for (int k = 0; k < nCupNum; k++)
                {
                    if (nChannelHs[1] > 0 && nChannelHs[2] > 0)
                    {
                        CupImgDrawColor(ref bSrcImg, nW3, nChannelHs[1], nMinX[k], nMaxX[k], nMinY[k], nMaxY[k], nCutLeft[k], nCutRight[k], ucR, ucG, ucB);
                        byte[] bSrcImgRight = new byte[nChannelHs[2] * nW3];
                        Array.Copy(bSrcImg, (nChannelHs[0] + nChannelHs[1]) * nW3, bSrcImgRight, 0, nChannelHs[2] * nW3);
                        CupImgDrawColor(ref bSrcImgRight, nW3, nChannelHs[2], nMinX2[k], nMaxX2[k], nMinY2[k], nMaxY2[k], nCutLeft2[k], nCutRight2[k], ucR, ucG, ucB);
                        Array.Copy(bSrcImgRight, 0, bSrcImg, (nChannelHs[0] + nChannelHs[1]) * nW3, nChannelHs[2] * nW3);
                    }
                    else
                    {
                        byte[] bSrcImgMid = new byte[nChannelHs[0] * nW3];
                        Array.Copy(bSrcImg, nChannelHs[1] * nW3, bSrcImgMid, 0, nChannelHs[0] * nW3);
                        CupImgDrawColor(ref bSrcImgMid, nW3, nChannelHs[0], nMinX[k], nMaxX[k], nMinY[k], nMaxY[k], nCutLeft[k], nCutRight[k], ucR, ucG, ucB);
                        Array.Copy(bSrcImgMid, 0, bSrcImg, nChannelHs[1] * nW3, nChannelHs[0] * nW3);
                    }
                }
				
			}
			
		}
		else
		{
			ColorCount[0] = -1;
			ColorCount[1] = -1;
			ColorCount[2] = -1;
		}
	}
	catch (Exception ex)
	{
		Trace.WriteLine("Commonfunction中函数ColorStatistic24出错" + ex);
#if REALEASE
		GlobalDataInterface.WriteErrorInfo("Commonfunction中函数ColorStatistic出错" + ex);
#endif
	}
}
        /// <summary>
        /// 获取UV值范围（颜色设置窗口）
        /// </summary>
        /// <param name="SrcRGBImage"></param>
        /// <param name="SrcRect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect GetRGB32ColorIntervalRect(byte[] SrcRGBImage, Rect SrcRect,int width,int height)
        {
            //Y =  0.299R + 0.587G + 0.114B
            //U = -0.147R - 0.289G + 0.436B
            //V =  0.615R - 0.515G - 0.100B
            Rect rect = new Rect();
            try
            {
                
                int MaxU = -128, MaxV = -128, MinU = -128, MinV = -128;//纵坐标为U，横坐标为V
                byte R, G, B;//纵坐标为U，横坐标为V
                int U, V;
                R = SrcRGBImage[((SrcRect.Top) * width * 4) + (SrcRect.Left) * 4 + 2];
                G = SrcRGBImage[((SrcRect.Top) * width * 4) + (SrcRect.Left) * 4 + 1];
                B = SrcRGBImage[((SrcRect.Top) * width * 4) + (SrcRect.Left) * 4];
                U = (int)(-0.147 * R - 0.289 * G + 0.436 * B);
                V = (int)(0.615 * R - 0.515 * G - 0.100 * B);
                MaxU = MinU = U;
                MaxV = MinV = V;
                for (int j = 0; j < SrcRect.Bottom - SrcRect.Top; j++)
                {
                    for (int i = 1; i < SrcRect.Right - SrcRect.Left; i++)
                    {
                        R = SrcRGBImage[((SrcRect.Top + j) * width * 4) + (SrcRect.Left + i) * 4 + 2];
                        G = SrcRGBImage[((SrcRect.Top + j) * width * 4) + (SrcRect.Left + i) * 4 + 1];
                        B = SrcRGBImage[((SrcRect.Top + j) * width * 4) + (SrcRect.Left + i) * 4];

                        U = (int)(-0.147 * R - 0.289 * G + 0.436 * B);
                        V = (int)(0.615 * R - 0.515 * G - 0.100 * B);
                        if (U > MaxU)
                            MaxU = U;
                        if (U < MinU)
                            MinU = U;
                        if (V > MaxV)
                            MaxV = V;
                        if (V < MinV)
                            MinV = V;

                    }


                }
                //if (MaxU == 0 && MaxV == 0 && MinU == -128 && MinV == -128)
                //{
                //    MinU = MinV = 0;
                //}
                rect.Left = MinV + 128;
                if (rect.Left < 0)
                    rect.Left = 0;
                if (rect.Left > 255)
                    rect.Left = 255;
                rect.Right = MaxV + 128;
                if (rect.Right < 0)
                    rect.Right = 0;
                if (rect.Right > 255)
                    rect.Right = 255;
                rect.Top = MinU + 128;
                if (rect.Top < 0)
                    rect.Top = 0;
                if (rect.Top > 255)
                    rect.Top = 255;
                rect.Bottom = MaxU + 128;
                if (rect.Bottom < 0)
                    rect.Bottom = 0;
                if (rect.Bottom > 255)
                    rect.Bottom = 255;
                return rect;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetColorIntervalRect出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetColorIntervalRect出错" + ex);
#endif
                return rect;
            }
        }

        /// <summary>
        /// 获取UV值范围（颜色设置窗口）
        /// </summary>
        /// <param name="SrcRGBImage"></param>
        /// <param name="SrcRect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect GetRGB24ColorIntervalRect(byte[] SrcRGBImage, Rect SrcRect, int width, int height)
        {
            //Y =  0.299R + 0.587G + 0.114B
            //U = -0.147R - 0.289G + 0.436B
            //V =  0.615R - 0.515G - 0.100B
            Rect rect = new Rect();
            try
            {

                int MaxU = -128, MaxV = -128, MinU = -128, MinV = -128;//纵坐标为U，横坐标为V
                byte R, G, B;//纵坐标为U，横坐标为V
                int U, V;
                R = SrcRGBImage[((SrcRect.Top) * width * 3) + (SrcRect.Left) * 3 + 2];
                G = SrcRGBImage[((SrcRect.Top) * width * 3) + (SrcRect.Left) * 3 + 1];
                B = SrcRGBImage[((SrcRect.Top) * width * 3) + (SrcRect.Left) * 3];
                U = (int)(-0.147 * R - 0.289 * G + 0.436 * B);
                V = (int)(0.615 * R - 0.515 * G - 0.100 * B);
                MaxU = MinU = U;
                MaxV = MinV = V;
                for (int j = 0; j < SrcRect.Bottom - SrcRect.Top; j++)
                {
                    for (int i = 1; i < SrcRect.Right - SrcRect.Left; i++)
                    {
                        R = SrcRGBImage[((SrcRect.Top + j) * width * 3) + (SrcRect.Left + i) * 3 + 2];
                        G = SrcRGBImage[((SrcRect.Top + j) * width * 3) + (SrcRect.Left + i) * 3 + 1];
                        B = SrcRGBImage[((SrcRect.Top + j) * width * 3) + (SrcRect.Left + i) * 3];

                        U = (int)(-0.147 * R - 0.289 * G + 0.436 * B);
                        V = (int)(0.615 * R - 0.515 * G - 0.100 * B);
                        if (U > MaxU)
                            MaxU = U;
                        if (U < MinU)
                            MinU = U;
                        if (V > MaxV)
                            MaxV = V;
                        if (V < MinV)
                            MinV = V;

                    }


                }
                //if (MaxU == 0 && MaxV == 0 && MinU == -128 && MinV == -128)
                //{
                //    MinU = MinV = 0;
                //}
                rect.Left = MinV + 128;
                if (rect.Left < 0)
                    rect.Left = 0;
                if (rect.Left > 255)
                    rect.Left = 255;
                rect.Right = MaxV + 128;
                if (rect.Right < 0)
                    rect.Right = 0;
                if (rect.Right > 255)
                    rect.Right = 255;
                rect.Top = MinU + 128;
                if (rect.Top < 0)
                    rect.Top = 0;
                if (rect.Top > 255)
                    rect.Top = 255;
                rect.Bottom = MaxU + 128;
                if (rect.Bottom < 0)
                    rect.Bottom = 0;
                if (rect.Bottom > 255)
                    rect.Bottom = 255;
                return rect;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetColorIntervalRect出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetColorIntervalRect出错" + ex);
#endif
                return rect;
            }
        }

        /// <summary>
        /// 获取一定范围RGB均值（颜色设置窗口）
        /// </summary>
        /// <param name="SrcRGBImage"></param>
        /// <param name="SrcRect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static stBGR GetAverageColor(byte[] SrcRGBImage, Rect SrcRect, int width, int height)
        {
            //Y =  0.299R + 0.587G + 0.114B
            //U = -0.147R - 0.289G + 0.436B
            //V =  0.615R - 0.515G - 0.100B
            stBGR AverageTagBGR = new stBGR(true);
            try
            {   
                int R, G, B;//纵坐标为U，横坐标为V
                R = 0; G = 0; B = 0;
                for (int j = 0; j < SrcRect.Bottom - SrcRect.Top; j++)
                {
                    for (int i = 0; i < SrcRect.Right - SrcRect.Left; i++)
                    {
                        R += SrcRGBImage[((SrcRect.Top + j) * width * 4) + (SrcRect.Left + i) * 4 + 2];
                        G += SrcRGBImage[((SrcRect.Top + j) * width * 4) + (SrcRect.Left + i) * 4 + 1];
                        B += SrcRGBImage[((SrcRect.Top + j) * width * 4) + (SrcRect.Left + i) * 4];
                    }
                }
                byte Interval =(byte)((SrcRect.Bottom - SrcRect.Top)*(SrcRect.Right - SrcRect.Left));
                AverageTagBGR.bR = (byte)(R / Interval * 1.0);
                AverageTagBGR.bG = (byte)(G / Interval * 1.0);
                AverageTagBGR.bB = (byte)(B / Interval * 1.0);
                return AverageTagBGR;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数GetAverageColor出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数GetAverageColort出错" + ex);
#endif
                return AverageTagBGR;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stBackupFileInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
            public byte[] FileName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH*4)]
            public byte[] Path;
            public long index;
            public long length;

            public stBackupFileInfo(bool IsOK)
            {
                FileName = new byte[ConstPreDefine.MAX_TEXT_LENGTH*2];
                Path = new byte[ConstPreDefine.MAX_TEXT_LENGTH * 4];
                index = 0;
                length = 0;
            }
        }

        /// <summary>
        /// 备份配置(允许创建一个子目录)
        /// </summary>
        public static bool BackupConfigure(string DesFileName)
        {
            try
            {
                stBackupFileInfo backupFile;
                List<FileInfo> listFiles = new List<FileInfo>();//保存所有文件信息
                List<stBackupFileInfo> listBackupFiles = new List<stBackupFileInfo>();//写入文件信息头
                string SrcPath = System.Environment.CurrentDirectory;
                SrcPath += "\\config\\";
                DesFileName += ".fs";
                long MaxLenth = 0;
                long offset = sizeof(int) + sizeof(long);
                //获取所有文件
                DirectoryInfo directory = new DirectoryInfo(SrcPath);
                DirectoryInfo[] directoryArray = directory.GetDirectories();
                FileInfo[] fileInfoArray = directory.GetFiles();
                byte[] tempName = new byte[ConstPreDefine.MAX_TEXT_LENGTH * 4];
                if (fileInfoArray.Length > 0)
                {
                    listFiles.AddRange(fileInfoArray);
                    for (int i = 0; i < listFiles.Count; i++)
                    {
                        offset += Marshal.SizeOf(typeof(stBackupFileInfo));
                        if (i > 0)
                            offset += listBackupFiles[i - 1].length;
                        backupFile = new stBackupFileInfo(true);
                        tempName = Encoding.Default.GetBytes(listFiles[i].Name);
                        Array.Copy(tempName, 0, backupFile.FileName, 0, tempName.Length);
                        backupFile.index = offset;
                        backupFile.length = listFiles[i].Length;

                        listBackupFiles.Add(backupFile);
                        if (listFiles[i].Length > MaxLenth)
                            MaxLenth = listFiles[i].Length;
                    }
                }
                foreach (DirectoryInfo _directoryInfo in directoryArray)
                {
                    DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                    FileInfo[] fileInfoArrayex = directoryA.GetFiles();
                    
                    if (fileInfoArrayex.Length > 0)
                    {
                        listFiles.AddRange(fileInfoArrayex);
                        for (int i = listBackupFiles.Count; i < listFiles.Count; i++)
                        {
                            offset += Marshal.SizeOf(typeof(stBackupFileInfo));
                            if (i > 0)
                                offset += listBackupFiles[i - 1].length;
                            backupFile = new stBackupFileInfo(true);
                            tempName = Encoding.Default.GetBytes(listFiles[i].Name);
                            Array.Copy(tempName, 0, backupFile.FileName, 0, tempName.Length);
                            tempName = Encoding.Default.GetBytes(directoryA.Name);
                            Array.Copy(tempName, 0, backupFile.Path, 0, tempName.Length);
                            backupFile.length = listFiles[i].Length;
                            listBackupFiles.Add(backupFile);
                            if (listFiles[i].Length > MaxLenth)
                                MaxLenth = listFiles[i].Length;
                        }
                    }

                }


                //写文件
                byte[] FileData = new byte[MaxLenth];
                FileStream DstStream = new FileStream(DesFileName, FileMode.Create, FileAccess.Write);
                DstStream.Seek(0, SeekOrigin.Begin);
                FileData = BitConverter.GetBytes(MaxLenth);
                DstStream.Write(FileData, 0, sizeof(long));//写入最大读取长度
                FileData = BitConverter.GetBytes(listBackupFiles.Count);
                DstStream.Write(FileData, 0, sizeof(int));//写入文件数量

                for (int i = 0; i < listBackupFiles.Count; i++)
                {
                    int a = Marshal.SizeOf(typeof(stBackupFileInfo));
                    FileData = Commonfunction.StructToBytes(listBackupFiles[i]);
                    DstStream.Write(FileData, 0, Marshal.SizeOf(typeof(stBackupFileInfo)));//写文件信息头
                    FileData = new byte[MaxLenth];
                    FileStream SrcStream = listFiles[i].OpenRead();
                    SrcStream.Read(FileData, 0, (int)listFiles[i].Length);
                    DstStream.Write(FileData, 0, (int)listFiles[i].Length);//写文件内容
                    SrcStream.Close();
                }
                DstStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数BackupConfigure出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数BackupConfigure出错" + ex);
#endif
                return false;
            }
        }

        /// <summary>
        /// 恢复配置
        /// </summary>
        /// <param name="DesFileName"></param>
        public static bool RecoveryConfigure(string SrcFileName)
        {
            try
            {
                stBackupFileInfo backupFile;
                byte[] FileData = new byte[sizeof(long)];
                FileStream SrcStream = File.OpenRead(SrcFileName);
                FileStream DstStream;
                SrcStream.Read(FileData, 0, sizeof(long));
                long MaxLenth = BitConverter.ToInt64(FileData, 0);//获取最大读取长度
                FileData = new byte[MaxLenth];

                SrcStream.Read(FileData, 0, sizeof(int));
                int Count = BitConverter.ToInt32(FileData, 0);//获取文件数量

                for (int i = 0; i < Count; i++)
                {
                    SrcStream.Read(FileData, 0, Marshal.SizeOf(typeof(stBackupFileInfo)));
                    backupFile = (stBackupFileInfo)Commonfunction.BytesToStruct(FileData, typeof(stBackupFileInfo));
                    string FileName = System.Environment.CurrentDirectory;
                    if (!Directory.Exists(System.Environment.CurrentDirectory + "\\config\\"))
                        Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\config\\");
                    FileName += "\\config\\";
                    string childPath = Encoding.Default.GetString(backupFile.Path).TrimEnd('\0');
                    if (childPath != "")
                    {
                        FileName += childPath + "\\";
                        if (!Directory.Exists(FileName))                  
                        {
                            Directory.CreateDirectory(FileName);
                        }
                    }
                    
                    FileName += Encoding.Default.GetString(backupFile.FileName).TrimEnd('\0');

                    DstStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);//创建文件
                    DstStream.Seek(0, SeekOrigin.Begin);
                    SrcStream.Read(FileData, 0, (int)backupFile.length);
                    DstStream.Write(FileData, 0, (int)backupFile.length);//写文件内容
                    DstStream.Close();
                }
                SrcStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Commonfunction中函数RecoveryConfigure出错" + ex);
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("Commonfunction中函数RecoveryConfigure出错" + ex);
#endif
                return false;
            }
        }
        #endregion
    }
}
