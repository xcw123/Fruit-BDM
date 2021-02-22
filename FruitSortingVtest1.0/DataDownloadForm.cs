using FruitSortingVtest1.DB;
using Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FruitSortingVtest1
{
    public partial class DataDownloadForm : Form
    {
        Thread m_DownLoadThread = null;
        SqlLog log = new SqlLog();
        private DataBaseOperation databaseOperation = new DataBaseOperation();
        private AutoResetEvent ThreadAbortEventDone = new AutoResetEvent(false);

        /// <summary>
        /// 更新下载日志事件
        /// </summary>
        /// <param name="downLog"></param>
        public delegate void UpdateDownloadLogEventHandler(string downLog);
        public static event UpdateDownloadLogEventHandler UpdateDownloadLogEvent;

        public DataDownloadForm()
        {
            InitializeComponent();
            DataDownloadForm.UpdateDownloadLogEvent += new UpdateDownloadLogEventHandler(OnUpdateDownloadLogEvent);
        }

        private void DataDownloadForm_Load(object sender, EventArgs e)
        {
            m_DownLoadThread = new Thread(DownLoadDataThread);  //上传数据线程
            m_DownLoadThread.Priority = ThreadPriority.Normal;
            m_DownLoadThread.IsBackground = true;
            m_DownLoadThread.Start();
        }

        private void DownLoadDataThread()
        {
            try
            {
                bool bDownLoaded;
                WaitHandle[] waitEvent = new WaitHandle[2];
                waitEvent[0] = new EventWaitHandle(false, EventResetMode.AutoReset, "DownLoad", out bDownLoaded);
                waitEvent[1] = ThreadAbortEventDone;

                while(true)
                {
                    //waitEvent.WaitOne();
                    int result1 = WaitHandle.WaitAny(waitEvent, -1);
                    if (result1 == 1)
                    {
                        break;
                    }
                    else
                    {
                        bool bContinue = true;
                        int ErrorNum = 0;
                        while (bContinue)
                        {
                            int bFlag = OnDownLoadDataEvent();
                            switch (bFlag)
                            {
                                case 0:
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    bContinue = false;
                                    break;
                                case 3:
                                    ErrorNum++;
                                    break;
                                default:
                                    break;
                            }
                            if (ErrorNum >= 5)
                            {
                                bContinue = false;
                            }
                            Thread.Sleep(500);
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数DownLoadDataThread出错" + ex);
#endif
            }
        }

        /// <summary>
        /// 执行下载事件
        /// </summary>
        /// <returns>0-失败，1-成功，2-停止，3-异常</returns>
        private int OnDownLoadDataEvent()
        {
            try
            {
                DataSet dst1 = databaseOperation.GetFruitTop1NoEndProcessing();
                string strLog = "";
                if (dst1.Tables[0].Rows.Count > 0)
                {
                    #region 显示日志
                    
                    if (UpdateDownloadLogEvent != null)
                    {
                        strLog = "正在加工进行中，无法请求下载，请结束加工后再试......";
                        UpdateDownloadLogEvent(strLog);
                        log.WriteHttpLog(strLog);
                    }
                    #endregion
                    return 2;   //还有未结束加工的信息，无法执行下载任务
                }

                #region 比较时间，选择最近时间
                //DataSet dst2 = databaseOperation.GetFruitTop1();
                //string DownloadStartTime;
                //string dbNewEndTime = "";
                //if (dst2.Tables[0].Rows.Count > 0)
                //{
                //    dbNewEndTime = dst2.Tables[0].Rows[0]["EndTime"].ToString();
                //}
                //if (GlobalDataInterface.DownloadStartTime.CompareTo(dbNewEndTime) == -1)
                //{
                //    DownloadStartTime = dbNewEndTime;
                //}
                //else{
                //    DownloadStartTime = GlobalDataInterface.DownloadStartTime;
                //}
                #endregion

                #region 配置时间，选择配置时间
                string DownloadStartTime = GlobalDataInterface.DownloadStartTime;
                #endregion

                #region 显示日志
                if(UpdateDownloadLogEvent != null)
                {
                    strLog = "当前download开始时间：" + DownloadStartTime + "，正在请求下载......";
                    UpdateDownloadLogEvent(strLog);
                    log.WriteHttpLog(strLog);
                }
                #endregion

                #region 数据请求

                string strDeviceSearchCondition = GlobalDataInterface.DeviceNumber + "|" + DownloadStartTime;
                string strUrl = GlobalDataInterface.ServerURL + "DownLoadFruitProcessingInfo?DeviceSearchCondition=";
                string result = HttpHelper.OpenReadWithHttps(strUrl, strDeviceSearchCondition, 10000, new IPEndPoint(IPAddress.Parse(GlobalDataInterface.ServerBindLocalIP), 0));
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                string reMessage = jo["message"].ToString();
                //string reResult = jo["result"].ToString();
                string reStatus = jo["status"].ToString();
                string reStatusCode = jo["statusCode"].ToString();
                JObject jsResult = (JObject)jo["result"];
                #endregion

                #region 数据解析，以及数据存储
                if (reMessage.Contains("下传成功") && reMessage.Contains(GlobalDataInterface.DeviceNumber) && reStatusCode.Contains("SUCCESS"))
                {
                    JObject jsFPInfo = (JObject)jsResult["pInfo"]; //加工信息
                    int RemainNumber = int.Parse(jsResult["RemainNumber"].ToString()); //待传数量

                    if (RemainNumber == 0) //没有待下载的数据
                    {
                        #region 显示日志
                        if (UpdateDownloadLogEvent != null)
                        {
                            strLog = "当前剩余待下载数量：0";
                            UpdateDownloadLogEvent(strLog);
                            log.WriteHttpLog(strLog);
                        }
                        #endregion
                        return 2;
                    }

                    FruitProcessingInfo fpInfo = new FruitProcessingInfo();
                    fpInfo.DeviceNumber = jsFPInfo["DeviceNumber"].ToString();
                    fpInfo.t1_CustomerID = jsFPInfo["t1_CustomerID"].ToString();
                    fpInfo.t1_CustomerName = jsFPInfo["t1_CustomerName"].ToString();
                    fpInfo.t1_FarmName = jsFPInfo["t1_FarmName"].ToString();
                    fpInfo.t1_FruitName = jsFPInfo["t1_FruitName"].ToString();
                    fpInfo.t1_StartTime = jsFPInfo["t1_StartTime"].ToString();
                    fpInfo.t1_EndTime = jsFPInfo["t1_EndTime"].ToString();
                    fpInfo.t1_StartedState = jsFPInfo["t1_StartedState"].ToString();
                    fpInfo.t1_CompletedState = jsFPInfo["t1_CompletedState"].ToString();
                    fpInfo.t1_BatchWeight = jsFPInfo["t1_BatchWeight"].ToString();
                    fpInfo.t1_BatchNumber = jsFPInfo["t1_BatchNumber"].ToString();
                    fpInfo.t1_QualityGradeSum = jsFPInfo["t1_QualityGradeSum"].ToString();
                    fpInfo.t1_WeightOrSizeGradeNum = jsFPInfo["t1_WeightOrSizeGradeNum"].ToString();
                    fpInfo.t1_ExportSum = jsFPInfo["t1_ExportSum"].ToString();
                    fpInfo.t1_ColorGradeName = jsFPInfo["t1_ColorGradeName"].ToString();
                    fpInfo.t1_ShapeGradeName = jsFPInfo["t1_ShapeGradeName"].ToString();
                    fpInfo.t1_FlawGradeName = jsFPInfo["t1_FlawGradeName"].ToString();
                    fpInfo.t1_HardGradeName = jsFPInfo["t1_HardGradeName"].ToString();
                    fpInfo.t1_DensityGradeName = jsFPInfo["t1_DensityGradeName"].ToString();
                    fpInfo.t1_SugarDegreeGradeName = jsFPInfo["t1_SugarDegreeGradeName"].ToString();
                    fpInfo.t1_ProgramName = jsFPInfo["t1_ProgramName"].ToString();
                    JArray ja_t2_GradeID = (JArray)jsFPInfo["t2_GradeID"];
                    fpInfo.t2_GradeID = ja_t2_GradeID.ToObject<List<string>>();
                    JArray ja_t2_BoxNumber = (JArray)jsFPInfo["t2_BoxNumber"];
                    fpInfo.t2_BoxNumber = ja_t2_BoxNumber.ToObject<List<string>>();
                    JArray ja_t2_FruitNumber = (JArray)jsFPInfo["t2_FruitNumber"];
                    fpInfo.t2_FruitNumber = ja_t2_FruitNumber.ToObject<List<string>>();
                    JArray ja_t2_FruitWeight = (JArray)jsFPInfo["t2_FruitWeight"];
                    fpInfo.t2_FruitWeight = ja_t2_FruitWeight.ToObject<List<string>>();
                    JArray ja_t2_QualityName = (JArray)jsFPInfo["t2_QualityName"];
                    fpInfo.t2_QualityName = ja_t2_QualityName.ToObject<List<string>>();
                    JArray ja_t2_WeightOrSizeName = (JArray)jsFPInfo["t2_WeightOrSizeName"];
                    fpInfo.t2_WeightOrSizeName = ja_t2_WeightOrSizeName.ToObject<List<string>>();
                    JArray ja_t2_WeightOrSizeLimit = (JArray)jsFPInfo["t2_WeightOrSizeLimit"];
                    fpInfo.t2_WeightOrSizeLimit = ja_t2_WeightOrSizeLimit.ToObject<List<string>>();
                    JArray ja_t2_SelectWeightOrSize = (JArray)jsFPInfo["t2_SelectWeightOrSize"];
                    fpInfo.t2_SelectWeightOrSize = ja_t2_SelectWeightOrSize.ToObject<List<string>>();
                    JArray ja_t2_TraitWeightOrSize = (JArray)jsFPInfo["t2_TraitWeightOrSize"];
                    fpInfo.t2_TraitWeightOrSize = ja_t2_TraitWeightOrSize.ToObject<List<string>>();
                    JArray ja_t2_TraitColor = (JArray)jsFPInfo["t2_TraitColor"];
                    fpInfo.t2_TraitColor = ja_t2_TraitColor.ToObject<List<string>>();
                    JArray ja_t2_TraitShape = (JArray)jsFPInfo["t2_TraitShape"];
                    fpInfo.t2_TraitShape = ja_t2_TraitShape.ToObject<List<string>>();
                    JArray ja_t2_TraitFlaw = (JArray)jsFPInfo["t2_TraitFlaw"];
                    fpInfo.t2_TraitFlaw = ja_t2_TraitFlaw.ToObject<List<string>>();
                    JArray ja_t2_TraitHard = (JArray)jsFPInfo["t2_TraitHard"];
                    fpInfo.t2_TraitHard = ja_t2_TraitHard.ToObject<List<string>>();
                    JArray ja_t2_TraitDensity = (JArray)jsFPInfo["t2_TraitDensity"];
                    fpInfo.t2_TraitDensity = ja_t2_TraitDensity.ToObject<List<string>>();
                    JArray ja_t2_TraitSugarDegree = (JArray)jsFPInfo["t2_TraitSugarDegree"];
                    fpInfo.t2_TraitSugarDegree = ja_t2_TraitSugarDegree.ToObject<List<string>>();
                    JArray ja_t3_ExportID = (JArray)jsFPInfo["t3_ExportID"];
                    fpInfo.t3_ExportID = ja_t3_ExportID.ToObject<List<string>>();
                    JArray ja_t3_FruitNumber = (JArray)jsFPInfo["t3_FruitNumber"];
                    fpInfo.t3_FruitNumber = ja_t3_FruitNumber.ToObject<List<string>>();
                    JArray ja_t3_FruitWeight = (JArray)jsFPInfo["t3_FruitWeight"];
                    fpInfo.t3_FruitWeight = ja_t3_FruitWeight.ToObject<List<string>>();

                    DataSet dst3 = databaseOperation.GetFruitByEndTime(fpInfo.t1_EndTime);
                    if (dst3.Tables[0].Rows.Count > 0)//本地数据库已包含本条数据，需跳过
                    {
                        GlobalDataInterface.DownloadStartTime = fpInfo.t1_EndTime;
                        #region 显示日志
                        if (UpdateDownloadLogEvent != null)
                        {
                            strLog = "下载一条加工信息（数据已存在），批次时间：" + fpInfo.t1_EndTime + "，当前剩余待下载数量：" + (RemainNumber - 1).ToString();
                            UpdateDownloadLogEvent(strLog);
                            log.WriteHttpLog(strLog);
                        }
                        #endregion
                        return (RemainNumber - 1) == 0 ? 2 : 1;
                    }

                    bool bFlag = databaseOperation.InsertFruitInfo(fpInfo.t1_CustomerName, fpInfo.t1_FarmName, fpInfo.t1_FruitName, fpInfo.t1_StartTime, fpInfo.t1_EndTime,
                        fpInfo.t1_StartedState, fpInfo.t1_CompletedState, int.Parse(fpInfo.t1_BatchWeight), int.Parse(fpInfo.t1_BatchNumber), int.Parse(fpInfo.t1_QualityGradeSum),
                        int.Parse(fpInfo.t1_WeightOrSizeGradeNum), int.Parse(fpInfo.t1_ExportSum), fpInfo.t1_ColorGradeName, fpInfo.t1_ShapeGradeName, fpInfo.t1_FlawGradeName,
                        fpInfo.t1_HardGradeName, fpInfo.t1_DensityGradeName, fpInfo.t1_SugarDegreeGradeName, fpInfo.t1_ProgramName);
                    if (bFlag == false)
                    {
                        return 0;//插入有误
                    }

                    DataSet dst4 = databaseOperation.GetFruitByEndTime(fpInfo.t1_EndTime);
                    int CustomerID = int.Parse(dst4.Tables[0].Rows[0]["CustomerID"].ToString());    //由于CustomerID是自增的，所以要插入之后才知道ID号

                    for (int i=0; i<fpInfo.t2_GradeID.Count; i++)
                    {
                        bFlag = databaseOperation.InsertGradeInfo(CustomerID, int.Parse(fpInfo.t2_GradeID[i]), int.Parse(fpInfo.t2_BoxNumber[i]), int.Parse(fpInfo.t2_FruitNumber[i]),
                            int.Parse(fpInfo.t2_FruitWeight[i]), fpInfo.t2_QualityName[i], fpInfo.t2_WeightOrSizeName[i], float.Parse(fpInfo.t2_WeightOrSizeLimit[i]),
                            fpInfo.t2_SelectWeightOrSize[i], fpInfo.t2_TraitWeightOrSize[i], fpInfo.t2_TraitColor[i], fpInfo.t2_TraitShape[i], fpInfo.t2_TraitFlaw[i],
                            fpInfo.t2_TraitHard[i], fpInfo.t2_TraitDensity[i], fpInfo.t2_TraitSugarDegree[i]);
                    }

                    for (int i=0; i<fpInfo.t3_ExportID.Count; i++)
                    {
                        bFlag = databaseOperation.InsertExportInfo(CustomerID, int.Parse(fpInfo.t3_ExportID[i]), int.Parse(fpInfo.t3_FruitNumber[i]), int.Parse(fpInfo.t3_FruitWeight[i]));
                    }

                    GlobalDataInterface.DownloadStartTime = fpInfo.t1_EndTime;

                    #region 显示日志
                    if (UpdateDownloadLogEvent != null)
                    {
                        strLog = "下载一条加工信息并保存，批次时间：" + fpInfo.t1_EndTime + "，当前剩余待下载数量：" + (RemainNumber - 1).ToString();
                        UpdateDownloadLogEvent(strLog);
                        log.WriteHttpLog(strLog);
                    }
                    #endregion
                    return (RemainNumber - 1) == 0 ? 2 : 1;
                }
                else if (reMessage.Contains("下传失败") && reMessage.Contains(GlobalDataInterface.DeviceNumber) && reStatusCode.Contains("SUCCESS"))
                {
                    #region 显示日志
                    if (UpdateDownloadLogEvent != null)
                    {
                        strLog = "请求下载失败，设备编号无效！";
                        UpdateDownloadLogEvent(strLog);
                        log.WriteHttpLog(strLog);
                    }
                    #endregion
                    return 2;
                }
                else
                {
                    #region 显示日志
                    if (UpdateDownloadLogEvent != null)
                    {
                        strLog = "请求下载异常！";
                        UpdateDownloadLogEvent(strLog);
                        log.WriteHttpLog(strLog);
                    }
                    #endregion
                    return 3;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region 显示日志
                if (UpdateDownloadLogEvent != null)
                {
                    string errLog = "下载过程出现异常，请查看异常日志！";
                    UpdateDownloadLogEvent(errLog);
                    log.WriteHttpLog(errLog);
                }
                #endregion
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数OnDownLoadDataEvent出错" + ex);
#endif
                return 3;
            }
        }

        /// <summary>
        /// 下载（下传）日志打印
        /// </summary>
        /// <param name="downLog"></param>
        private void OnUpdateDownloadLogEvent(string downLog)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DataDownloadForm.UpdateDownloadLogEventHandler(OnUpdateDownloadLogEvent), downLog);
                return;
            }
            try
            {
                DownloadLogPrint(downLog);
            }
            catch (Exception ex)
            {
                GlobalDataInterface.WriteErrorInfo("MainForm中OnUpdateDownloadLogEvent函数出错：" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 下载（下传）日志打印
        /// </summary>
        /// <param name="strContent"></param>
        private void DownloadLogPrint(string strContent)
        {
            try
            {
                if (listBoxDataLog.Items.Count > 500)
                {
                    listBoxDataLog.Items.RemoveAt(0);
                    listBoxDataLog.Items.RemoveAt(0);
                }
                string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                this.listBoxDataLog.Items.Add(strDateTime + "  " + strContent);
                this.listBoxDataLog.Items.Add("");
                this.listBoxDataLog.TopIndex = listBoxDataLog.Items.Count - 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error:" + ex.ToString());
            }
        }

        private void HttpTest(string[] arg)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(HttpTestThread), arg);
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数HttpTest出错" + ex);
#endif
            } 
        }

        private void HttpTestThread(object arg)
        {
            try
            {
                string strUrl = GlobalDataInterface.ServerURL + "CommunicationTest?DeviceNumber=" + GlobalDataInterface.DeviceNumber;
                TimeoutWebClient _dbMyClient = new TimeoutWebClient(3000, new IPEndPoint(IPAddress.Parse(GlobalDataInterface.ServerBindLocalIP), 0));
                _dbMyClient.Encoding = System.Text.Encoding.UTF8;
                string result = _dbMyClient.DownloadString(strUrl);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                string reMessage = jo["message"].ToString();
                string reResult = jo["result"].ToString();
                string reStatus = jo["status"].ToString();
                string reStatusCode = jo["statusCode"].ToString();
                if (reResult.Contains("通讯成功") && reMessage.Contains(GlobalDataInterface.DeviceNumber))
                {
                    #region 显示日志
                    if (UpdateDownloadLogEvent != null)
                    {
                        string strLog = "通讯测试成功！";
                        UpdateDownloadLogEvent(strLog);
                        log.WriteHttpLog(strLog);
                    }
                    #endregion
                    //MessageBox.Show("通讯测试成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    #region 显示日志
                    if (UpdateDownloadLogEvent != null)
                    {
                        string strLog = "通讯测试失败！";
                        UpdateDownloadLogEvent(strLog);
                        log.WriteHttpLog(strLog);
                    }
                    #endregion
                    //MessageBox.Show("通讯测试失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数btnHttpTest_Click出错" + ex);
#endif
                #region 显示日志
                if (UpdateDownloadLogEvent != null)
                {
                    string strLog = "通讯测试失败！";
                    UpdateDownloadLogEvent(strLog);
                    log.WriteHttpLog(strLog);
                }
                #endregion
                //MessageBox.Show("通讯测试失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnHttpTest_Click(object sender, EventArgs e)
        {
            HttpTest(null); //通讯测试
        }

        private void btnDataRequest_Click(object sender, EventArgs e)
        {
            try
            {
                bool bDownLoaded;
                EventWaitHandle waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "DownLoad", out bDownLoaded);
                waitEvent.Set();
            }
            catch(Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数btnDataRequest_Click出错" + ex);
#endif
            }
        }

        private void DataDownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ThreadAbortEventDone.Set();
                m_DownLoadThread.Abort();
                DataDownloadForm.UpdateDownloadLogEvent -= new UpdateDownloadLogEventHandler(OnUpdateDownloadLogEvent);
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DataDownloadForm中函数DataDownloadForm_FormClosing出错" + ex);
#endif
            }
        }

        private void lblDeviceRegister_DoubleClick(object sender, EventArgs e)
        {
            DeviceRegisterForm deviceRegisterForm = new DeviceRegisterForm();
            deviceRegisterForm.ShowDialog();
        }
    }
}
