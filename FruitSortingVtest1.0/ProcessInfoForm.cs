using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FruitSortingVtest1.DB;
using Interface;
using FruitSortingVtest1._0;
using System.Resources;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Threading;

namespace FruitSortingVtest1
{
    public partial class ProcessInfoForm : Form
    {
        public MainForm mainForm;
        public DataInterface dataInterface;     //水果的数据接口
        private stStatistics statisticsInfo;    //统计信息
        private stGradeInfo gradeInfo;          //等级信息
        private DataSet dst;                    //获取所有的水果信息
        private DataSet dst1;                   //统计信息时获取tb_FruitInfo
        private DataSet dst2;                   //统计信息时获取tb_GradeInfo
        private DataSet dst3;                   //统计信息时获取tb_ExportInfo
        public string currentSelectCustomerID;  //当前选择的客户编号
        public string currentSelectCustomerName;//当前选择的客户名称
        public string currentSelectFarmName;    //当前选择的农场名称
        public string currentSelectFruitName;   //当前选择的水果品种
        public string currentSelectCompletedState;//当前选择的加工状态
        private Boolean bIsSelect = false;      //是否进行了信息检索
        private Boolean bIsHaveReset = false;   //判断是否需要刷新界面
        private string strCustomerName; //所有保存的客户名称
        private string[] customerName;
        private string strFarmName;     //所有保存的农场名称
        private string[] farmName;
        private string strFruitName;    //所有保存的水果品种
        private string[] fruitName;
        private ResourceManager m_resourceManager = new ResourceManager(typeof(ProcessInfoForm));//创建Mainform资源管理
        private DataBaseOperation databaseOperation = new DataBaseOperation();  //创建数据库操作对象
        private string strInitPrompt = "";

        public ProgressBoxForm m_ProgressBoxForm
        {
            get;
            set;
        }

        public ProcessInfoForm()
        {
            InitializeComponent();
            GlobalDataInterface.UpdateDataInterfaceEvent += new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);
            ClientInfoUpdateForm.UpdateListViewEvent += new ClientInfoUpdateForm.ListViewEventHandler(OnUpdateListViewEvent);
        }

        public ProcessInfoForm(DataInterface dataInterface)
        {
            InitializeComponent();
            GlobalDataInterface.UpdateDataInterfaceEvent += new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent);
            ClientInfoUpdateForm.UpdateListViewEvent += new ClientInfoUpdateForm.ListViewEventHandler(OnUpdateListViewEvent);
        }

        private void OnUpdateDataInterfaceEvent(DataInterface dataInterface1)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent), dataInterface1);
                return;
            }
            bool bIsExist = false;   //判断LvwFruitData中是否存在当前的客户编号
            for (int i = 0; i < LvwFruitData.Items.Count; i++)
            {
                //if (dataInterface1.CustomerID == Convert.ToInt32(LvwFruitData.Items[i].SubItems[0].Text))
                if (dataInterface1.CustomerID == Convert.ToInt32(LvwFruitData.Items[i].SubItems[0].Text) && dataInterface1.CustomerID != 0) //Modify by ChengSk - 20181211
                {
                    LvwFruitData.Items[i].SubItems[7].Text = dataInterface1.IoStStatistics.nWeightCount.ToString();
                    LvwFruitData.Items[i].SubItems[8].Text = dataInterface1.IoStStatistics.nTotalCount.ToString();
                    bIsExist = true;
                }
            }
            if (!bIsSelect)
            {
                if (!bIsExist && dataInterface1.CustomerID != 0) //LvwFruitData中不存在当前客户编号，并且当前客户编号不为零，说明数据库中存在新数据，需刷新
                {
                    BtnReSet_Click(null, null);
                }
            }  
            //当水果数量有<100到>=100时，因需要更改开始时间，所以要刷新时间
            if (dataInterface1.IoStStatistics.nTotalCount < 100)
            {
                bIsHaveReset = true;
            }
            if (bIsHaveReset && (dataInterface1.IoStStatistics.nTotalCount >= 100))
            {
                bIsHaveReset = false;
                //LvwFruitData.Items[dataInterface1.CustomerID].SubItems[5].Text = dataInterface1.StartTime;
                BtnReSet_Click(null, null);               
            }
            
        }

        private void OnUpdateListViewEvent(string clientName, string farmName, string fruitName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ClientInfoUpdateForm.ListViewEventHandler(OnUpdateListViewEvent), clientName,farmName,fruitName);
                return;
            }
            for (int i = 0; i < LvwFruitData.Items.Count; i++)
            {
                if (Convert.ToInt32(currentSelectCustomerID) == Convert.ToInt32(LvwFruitData.Items[i].SubItems[0].Text))
                {
                    LvwFruitData.Items[i].SubItems[1].Text = clientName;
                    LvwFruitData.Items[i].SubItems[2].Text = farmName;
                    LvwFruitData.Items[i].SubItems[3].Text = fruitName;
                }
            }
            //LvwFruitData.SelectedItems[0].SubItems[1].Text = clientName;
            //LvwFruitData.SelectedItems[0].SubItems[2].Text = farmName;
            //LvwFruitData.SelectedItems[0].SubItems[3].Text = fruitName;
            currentSelectCustomerName = clientName;
            currentSelectFarmName = farmName;
            currentSelectFruitName = fruitName;
        }

        private void ProcessInfoForm_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            //更新ComboBox框
            UpdateComboBoxContent();

            DtpStartTime.Enabled = false;
            DtpEndTime.Enabled = false;

            #region ListView标题栏设置
            LvwFruitData.Columns.Add("CustomerID", m_resourceManager.GetString("LblCustomerID.Text"));
            LvwFruitData.Columns["CustomerID"].Width = 100; 
            LvwFruitData.Columns["CustomerID"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("CustomerName", m_resourceManager.GetString("LblCustomerName.Text"));
            LvwFruitData.Columns["CustomerName"].Width = 100;
            LvwFruitData.Columns["CustomerName"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("FarmName", m_resourceManager.GetString("LblFarmName.Text"));
            LvwFruitData.Columns["FarmName"].Width = 100;
            LvwFruitData.Columns["FarmName"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("FruitName", m_resourceManager.GetString("LblFruitName.Text"));
            LvwFruitData.Columns["FruitName"].Width = 100;
            LvwFruitData.Columns["FruitName"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("CompletedState", m_resourceManager.GetString("LblCompletedState.Text"));
            LvwFruitData.Columns["CompletedState"].Width = 100;
            LvwFruitData.Columns["CompletedState"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("StartTime", m_resourceManager.GetString("LblStartTime.Text"));
            LvwFruitData.Columns["StartTime"].Width = 150;
            LvwFruitData.Columns["StartTime"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("EndTime", m_resourceManager.GetString("LblEndTime.Text"));
            LvwFruitData.Columns["EndTime"].Width = 150;
            LvwFruitData.Columns["EndTime"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("BatchWeight", m_resourceManager.GetString("LblBatchWeight.Text"));
            LvwFruitData.Columns["BatchWeight"].Width = 100;
            LvwFruitData.Columns["BatchWeight"].TextAlign = HorizontalAlignment.Center;
            LvwFruitData.Columns.Add("BatchNumber", m_resourceManager.GetString("LblBatchNumber.Text"));
            LvwFruitData.Columns["BatchNumber"].Width = 100;
            LvwFruitData.Columns["BatchNumber"].TextAlign = HorizontalAlignment.Center;
            #endregion

            #region 获取水果信息并插入ListView
            //dst = BusinessFacade.GetFruitAll();
            dst = databaseOperation.GetFruitAll();
            InSertListView();
            #endregion
        }

        private void InSertListView()
        {
            //LvwFruitData清空
            LvwFruitData.Items.Clear();

            UInt64 BatchWeightTotals = 0; //批重量之和 Add by ChengSk - 20181123
            UInt64 BatchNumberTotals = 0; //批个数之和 Add by ChengSk - 20181123

            #region 循环往ListView中插入数据
            for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
            {
                ListViewItem lv = new ListViewItem(dst.Tables[0].Rows[i]["CustomerID"].ToString());
                lv.SubItems.Add(dst.Tables[0].Rows[i]["CustomerName"].ToString());
                lv.SubItems.Add(dst.Tables[0].Rows[i]["FarmName"].ToString());
                lv.SubItems.Add(dst.Tables[0].Rows[i]["FruitName"].ToString());
                if (dst.Tables[0].Rows[i]["CompletedState"].ToString().Equals("1"))
                {
                    lv.SubItems.Add(m_resourceManager.GetString("LblProcessOff.Text")); //加工已完成
                }
                else
                {
                    lv.SubItems.Add(m_resourceManager.GetString("LblProcessOn.Text")); //加工进行中
                }
                lv.SubItems.Add(dst.Tables[0].Rows[i]["StartTime"].ToString());
                lv.SubItems.Add(dst.Tables[0].Rows[i]["EndTime"].ToString());
                lv.SubItems.Add(dst.Tables[0].Rows[i]["BatchWeight"].ToString());
                BatchWeightTotals += UInt64.Parse(dst.Tables[0].Rows[i]["BatchWeight"].ToString()); //Add by ChengSk - 20181123
                lv.SubItems.Add(dst.Tables[0].Rows[i]["BatchNumber"].ToString());
                BatchNumberTotals += UInt64.Parse(dst.Tables[0].Rows[i]["BatchNumber"].ToString()); //Add by ChengSk - 20181123
                LvwFruitData.Items.Add(lv);
            }
            #endregion

            //添加“总计”行 Add by ChengSk - 20181123
            ListViewItem lvi = new ListViewItem("0");
            lvi.SubItems.Add("Sum");
            lvi.SubItems.Add("Sum");
            lvi.SubItems.Add("Sum");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add(BatchWeightTotals.ToString());
            lvi.SubItems.Add(BatchNumberTotals.ToString());
            LvwFruitData.Items.Add(lvi);

            #region 单双行更换背景色
            for (int i = 0; i < LvwFruitData.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    LvwFruitData.Items[i].BackColor = DrawGraphProtocol.myBackColor;
                }
            }
            #endregion
        }

        private void LvwFruitData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LvwFruitData.SelectedIndices != null && LvwFruitData.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = LvwFruitData.SelectedIndices;
                currentSelectCustomerID = LvwFruitData.SelectedItems[0].SubItems[0].Text;
                GlobalDataInterface.SerialNum = currentSelectCustomerID;//序列号
                currentSelectCustomerName = LvwFruitData.SelectedItems[0].SubItems[1].Text;
                currentSelectFarmName = LvwFruitData.SelectedItems[0].SubItems[2].Text;
                currentSelectFruitName = LvwFruitData.SelectedItems[0].SubItems[3].Text;
                currentSelectCompletedState = LvwFruitData.SelectedItems[0].SubItems[4].Text;
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {

            //str.substring(0, 1);
            //str.substring(str.Length - 1, 1);
            if (CboCustomerName.Text.Equals("") && CboFarmName.Text.Equals("") && CboFruitName.Text.Equals("") &&
                !(DtpStartTime.Enabled && DtpEndTime.Enabled))
            {
                //MessageBox.Show("查询条件不能全为空！");
                //MessageBox.Show("0x30001101 Query content can not be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("0x30001101 " + LanguageContainer.ProcessInfoFormMessagebox1Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bIsSelect = true;  //标记为进行了查询操作

            #region 组合查询语句
            string strSQL = "select * from tb_FruitInfo where ";
            bool bEvenSearchCondition = false;  //之前是否已有检索条件
            //客户名称是否为空
            if (!CboCustomerName.Text.Equals(""))
            {
                if (CboCustomerName.Text.Substring(0, 1) == "*" && CboCustomerName.Text.Substring(CboCustomerName.Text.Length - 1, 1) == "*")
                {
                    CboCustomerName.Text = CboCustomerName.Text.Substring(1, CboCustomerName.Text.Length - 2);
                    strSQL += "CustomerName like " + "'%" + CboCustomerName.Text + "%' ";    //add by xcw - 20191212
                }
                else
                {
                    strSQL += "CustomerName = " + "'" + CboCustomerName.Text + "' ";
                }
                    
                bEvenSearchCondition = true;
            }
            //农场名称是否为空
            if (!CboFarmName.Text.Equals(""))
            {
                if (bEvenSearchCondition)
                {
                    strSQL += "and ";
                }
                if (CboFarmName.Text.Substring(0, 1) == "*" && CboFarmName.Text.Substring(CboFarmName.Text.Length - 1, 1) == "*")
                {
                    CboFarmName.Text = CboFarmName.Text.Substring(1, CboFarmName.Text.Length - 2);
                    strSQL += "FarmName like " + "'%" + CboFarmName.Text + "%' ";   //add by xcw - 20191212
                }
                else
                {
                    strSQL += "FarmName = " + "'" + CboFarmName.Text + "' ";
                }
                bEvenSearchCondition = true;
            }
            //水果名称是否为空
            if (!CboFruitName.Text.Equals(""))
            {
                if (bEvenSearchCondition)
                {
                    strSQL += "and ";
                }
                if (CboFruitName.Text.Substring(0, 1) == "*" && CboFruitName.Text.Substring(CboFruitName.Text.Length - 1, 1) == "*")
                {
                    CboFruitName.Text = CboFruitName.Text.Substring(1, CboFruitName.Text.Length - 2);
                    strSQL += "FruitName like " + "'%" + CboFruitName.Text + "%' "; //add by xcw - 20191212
                }
                else
                {
                    strSQL += "FruitName = " + "'" + CboFruitName.Text + "' ";
                }
                bEvenSearchCondition = true;
            }
            //开始结束时间是否为空
            if (DtpStartTime.Enabled && DtpEndTime.Enabled)
            {
                if (bEvenSearchCondition)
                {
                    strSQL += "and ";
                }
                string startTime = DtpStartTime.Text+" 00:00:00";
                string endTime = DtpEndTime.Text +" 00:00:00";
                strSQL += "StartTime > " + "'" + startTime + "' and EndTime < " + "'" + endTime + "' ";
            }
            strSQL += "order by CustomerID desc";
            #endregion
            //dst = BusinessFacade.GetTable(strSQL);
            dst = databaseOperation.SelectData(strSQL);
            InSertListView();
        }

        private void BtnStatistics_Click(object sender, EventArgs e)
        {
            #region 判断当前选择CustomerID是否为空
            //if (currentSelectCustomerID == null || currentSelectCustomerID.Equals(""))
            if (currentSelectCustomerID == null || currentSelectCustomerID.Equals("") || currentSelectCustomerID == "0") //Modify by ChengSk - 20181123
            {
                //MessageBox.Show("请先选择要统计的条目！");
                //MessageBox.Show("30001102 Please select entries to statistics first!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("30001102 " + LanguageContainer.ProcessInfoFormMessagebox2Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #endregion

            //重新实例化
            dataInterface = new DataInterface(true);
            statisticsInfo = new stStatistics(true);
            gradeInfo = new stGradeInfo(true);

            dataInterface.BSourceDB = true;          

            #region 从数据库中取相应条目信息放到DataSet中
            //获取tb_FruitInfo
            //dst1 = BusinessFacade.GetFruitByCustomerID(Convert.ToInt32(currentSelectCustomerID));
            dst1 = databaseOperation.GetFruitByCustomerID(Convert.ToInt32(currentSelectCustomerID)); 
            if (dst1.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0"))
            {
                //MessageBox.Show("加工进行中 不能进行统计！");
                //MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("20001101 " + LanguageContainer.ProcessInfoFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //获取tb_GradeInfo
            //dst2 = BusinessFacade.GetGradeByCustomerID(Convert.ToInt32(currentSelectCustomerID));
            dst2 = databaseOperation.GetGradeByCustomerID(Convert.ToInt32(currentSelectCustomerID)); 
            //获取tb_ExportInfo
            //dst3 = BusinessFacade.GetExportByCustomerID(Convert.ToInt32(currentSelectCustomerID));
            dst3 = databaseOperation.GetExportByCustomerID(Convert.ToInt32(currentSelectCustomerID)); 
            #endregion

            #region 往dataInterface中插入水果信息
            //往dataInterface中插入水果信息
            if(dst1.Tables[0].Rows.Count <= 0)
            {
                //MessageBox.Show("选择条目的水果信息为空！");
                //MessageBox.Show("30001103 The currently selected fruit information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("30001103 " + LanguageContainer.ProcessInfoFormMessagebox4Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dataInterface.BSourceDB = true;
            dataInterface.CustomerID = Convert.ToInt32(dst1.Tables[0].Rows[0]["CustomerID"].ToString());
            dataInterface.CustomerName = dst1.Tables[0].Rows[0]["CustomerName"].ToString();
            dataInterface.FarmName = dst1.Tables[0].Rows[0]["FarmName"].ToString();
            dataInterface.FruitName = dst1.Tables[0].Rows[0]["FruitName"].ToString();
            dataInterface.StartTime = dst1.Tables[0].Rows[0]["StartTime"].ToString();
            dataInterface.EndTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
            dataInterface.StartedState = dst1.Tables[0].Rows[0]["StartedState"].ToString();
            dataInterface.CompletedState = dst1.Tables[0].Rows[0]["CompletedState"].ToString();
            dataInterface.QualityGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString());
            if (dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString().Equals(""))
            {
                dataInterface.WeightOrSizeGradeSum = 0;
            }
            else
            {
                dataInterface.WeightOrSizeGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString());
            }          
            dataInterface.ExportSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["ExportSum"].ToString());
            dataInterface.ColorGradeName = dst1.Tables[0].Rows[0]["ColorGradeName"].ToString();
            dataInterface.ShapeGradeName = dst1.Tables[0].Rows[0]["ShapeGradeName"].ToString();
            dataInterface.FlawGradeName = dst1.Tables[0].Rows[0]["FlawGradeName"].ToString();
            dataInterface.HardGradeName = dst1.Tables[0].Rows[0]["HardGradeName"].ToString();
            dataInterface.DensityGradeName = dst1.Tables[0].Rows[0]["DensityGradeName"].ToString();
            dataInterface.SugarDegreeGradeName = dst1.Tables[0].Rows[0]["SugarDegreeGradeName"].ToString();
            dataInterface.ProgramName = dst1.Tables[0].Rows[0]["ProgramName"].ToString();
            statisticsInfo.nWeightCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchWeight"].ToString());
            statisticsInfo.nTotalCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchNumber"].ToString());
            #endregion

            #region 往dataInterface中插入等级信息
            //往dataInterface中插入等级信息
            if (dst2.Tables[0].Rows.Count <= 0)
            {
                //MessageBox.Show("选择条目的等级信息为空！");
                //MessageBox.Show("30001104 The currently selected Grade information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("30001104 " + LanguageContainer.ProcessInfoFormMessagebox5Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for (int i = 0; i < dst2.Tables[0].Rows.Count; i++)
            {
                //存水果箱数
                statisticsInfo.nBoxGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToInt32(dst2.Tables[0].Rows[i]["BoxNumber"].ToString());
                //存水果个数
                statisticsInfo.nGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitNumber"].ToString());
                //存水果重量
                statisticsInfo.nWeightGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitWeight"].ToString());
                //存品质名称
                if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) == 0)
                {
                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                    0,
                    FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                    gradeInfo.strQualityGradeName,
                    i * ConstPreDefine.MAX_TEXT_LENGTH);
                    //取品质名称
                    //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                    //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                    //    ConstPreDefine.MAX_TEXT_LENGTH));

                    //存重量/尺寸名称
                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                        0,
                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                        gradeInfo.strSizeGradeName,
                        i * ConstPreDefine.MAX_TEXT_LENGTH);
                }
                else  //有品质特征时：品质+尺寸  品质+重量
                {
                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                    0,
                    FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                    gradeInfo.strQualityGradeName,
                    (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) / ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                    //取品质名称
                    //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                    //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                    //    ConstPreDefine.MAX_TEXT_LENGTH));
                    //存重量/尺寸名称

                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                        0,
                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                        gradeInfo.strSizeGradeName,
                        (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) % ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                }
                //存重量/尺寸限制
                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nMinSize = float.Parse(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString());//Convert.ToInt32(Convert.ToInt32(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString()));
                //存重量/尺寸选择
                if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) > 0)  //有品质
                {
                    if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                    {
                        gradeInfo.nClassifyType = 4;
                    }
                    else  //选重量
                    {
                        gradeInfo.nClassifyType = 2;
                    }
                }
                else  //无品质
                {
                    if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                    {
                        gradeInfo.nClassifyType = 4;
                    }
                    else  //选重量
                    {
                        gradeInfo.nClassifyType = 2;
                    }
                }
                //存重量/尺寸特征

                //存颜色特征
                if (dst2.Tables[0].Rows[i]["TraitColor"].ToString() == null || dst2.Tables[0].Rows[i]["TraitColor"].ToString().Equals(""))
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = 0x7F;
                }
                else
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = Convert.ToSByte(dst2.Tables[0].Rows[i]["TraitColor"].ToString());
                }
                //存形状特征
                if (dst2.Tables[0].Rows[i]["TraitShape"].ToString() == null || dst2.Tables[0].Rows[i]["TraitShape"].ToString().Equals(""))
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = 0x7F;
                }
                else
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitShape"].ToString());
                }
                //存瑕疵特征
                if (dst2.Tables[0].Rows[i]["TraitFlaw"].ToString() == null || dst2.Tables[0].Rows[i]["TraitFlaw"].ToString().Equals(""))
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = 0x7F;
                }
                else
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitFlaw"].ToString());
                }
                //存硬度特征
                if (dst2.Tables[0].Rows[i]["TraitHard"].ToString() == null || dst2.Tables[0].Rows[i]["TraitHard"].ToString().Equals(""))
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = 0x7F;
                }
                else
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitHard"].ToString());
                }
                //存密度特征
                if (dst2.Tables[0].Rows[i]["TraitDensity"].ToString() == null || dst2.Tables[0].Rows[i]["TraitDensity"].ToString().Equals(""))
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = 0x7F;
                }
                else
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitDensity"].ToString());
                }
                //存含糖量特征
                if (dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString() == null || dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString().Equals(""))
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = 0x7F;
                }
                else
                {
                    gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString());
                }

            }
            #endregion

            #region 往dataInterface中插入出口信息
            //往dataInterface中插入出口信息
            if (dst3.Tables[0].Rows.Count <= 0)
            {
                //MessageBox.Show("选择条目的出口信息为空！");
                //MessageBox.Show("30001105 The currently selected Outlets information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("30001105 " + LanguageContainer.ProcessInfoFormMessagebox6Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxInformationCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for (int i = 0; i < dst3.Tables[0].Rows.Count; i++)
            {
                statisticsInfo.nExitCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitNumber"].ToString());
                statisticsInfo.nExitWeightCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitWeight"].ToString());
            }
            #endregion

            #region 往DataInterface类中汇总结构体数据
            //往DataInterface类中汇总数据
            dataInterface.IoStStatistics = statisticsInfo;
            dataInterface.IoStStGradeInfo = gradeInfo;
            #endregion

            #region 判断当前加工状态能否进行统计
            if (dataInterface.StartedState.Equals("1") && dataInterface.CompletedState.Equals("0"))
            {
                //MessageBox.Show("加工进行中 不能进行统计！");
                //MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("20001101 " + LanguageContainer.ProcessInfoFormMessagebox3Text[GlobalDataInterface.selectLanguageIndex],
                    LanguageContainer.ProcessInfoFormMessageboxWarningCaption[GlobalDataInterface.selectLanguageIndex], 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion

            int selectType = dataInterface.IoStStGradeInfo.nClassifyType;

            //if ((selectType & 1) == 1) //有品质
            //if (dataInterface.IoStStGradeInfo.nQualityGradeNum > 0)  //有品质
            if (dataInterface.QualityGradeSum > 0) //有品质
            {
                //if ((selectType >> 3 & 1) == 1 || (selectType >> 4 & 1) == 1 || (selectType >> 5 & 1) == 1) //有尺寸
                if((selectType & 0x1C) > 0)  //有尺寸
                {
                    StatisticsInfoForm3 statisticsInfoForm3 = new StatisticsInfoForm3(dataInterface);
                    statisticsInfoForm3.ShowDialog(this);
                }
                else                 //有重量
                {
                    StatisticsInfoForm4 statisticsInfoForm4 = new StatisticsInfoForm4(dataInterface);
                    statisticsInfoForm4.ShowDialog(this);
                }
            }
            else                     //无品质
            {
                //if ((selectType >> 3 & 1) == 1 || (selectType >> 4 & 1) == 1 || (selectType >> 5 & 1) == 1) //有尺寸
                if ((selectType & 0x1C) > 0)  //有尺寸
                {
                    StatisticsInfoForm1 statisticsInfoForm1 = new StatisticsInfoForm1(dataInterface);
                    statisticsInfoForm1.ShowDialog(this);
                }
                else                 //有重量
                {
                    StatisticsInfoForm2 statisticsInfoForm2 = new StatisticsInfoForm2(dataInterface);
                    statisticsInfoForm2.ShowDialog(this);
                }
            }
        }

        private void ChkStartTime_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkStartTime.Checked)
            {
                DtpStartTime.Enabled = true;
            }
            else
            {
                DtpStartTime.Enabled = false;
            }
        }

        private void ChkEndTime_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkEndTime.Checked)
            {
                DtpEndTime.Enabled = true;
            }
            else
            {
                DtpEndTime.Enabled = false;
            }
        }

        private void BtnReSet_Click(object sender, EventArgs e)
        {
            bIsSelect = false;   //重新标记为无查询操作
            //dst = BusinessFacade.GetFruitAll();
            dst = databaseOperation.GetFruitAll(); 
            InSertListView();

            this.ExportExcelbutton.Enabled = false;   //Add by ChengSk - 20190527
            this.BtnDetailStatistics.Enabled = false; //Add by ChengSk - 20190527
            this.GradeExcelbutton.Enabled = false;    //Add by ChengSk - 20191111
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LvwFruitData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("当前所选ID号为：" + currentSelectCustomerID.ToString());
            if (currentSelectCompletedState.Equals(m_resourceManager.GetString("LblProcessOn.Text")))  //判断是否是加工进行中
            {
                ClientInfoUpdateForm clientInfoUpdateForm = new ClientInfoUpdateForm();
                clientInfoUpdateForm.ShowDialog(this);
            }       
        }

        //更新ComboBox框
        public void UpdateComboBoxContent()
        {
            //获取客户信息
            strCustomerName = FileOperate.ReadFile(2, mainForm.clientInfoFileName);
            strFarmName = FileOperate.ReadFile(3, mainForm.clientInfoFileName);
            strFruitName = FileOperate.ReadFile(4, mainForm.clientInfoFileName);
            //往ComboBox中添加选项
            if (strCustomerName != null)
            {
                CboCustomerName.Items.Clear();
                customerName = strCustomerName.Split('，');
                for (int i = 0; i < customerName.Length; i++)
                {
                    CboCustomerName.Items.Add(customerName[i]);
                }
            }
            if (strFarmName != null)
            {
                CboFarmName.Items.Clear();
                farmName = strFarmName.Split('，');
                for (int i = 0; i < farmName.Length; i++)
                {
                    CboFarmName.Items.Add(farmName[i]);
                }
            }
            if (strFruitName != null)
            {
                CboFruitName.Items.Clear();
                fruitName = strFruitName.Split('，');
                for (int i = 0; i < fruitName.Length; i++)
                {
                    CboFruitName.Items.Add(fruitName[i]);
                }
            }
        }

        private void ProcessInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalDataInterface.UpdateDataInterfaceEvent -= new GlobalDataInterface.DataInterfaceEventHandler(OnUpdateDataInterfaceEvent); //Add by ChengSk - 20180830
            ClientInfoUpdateForm.UpdateListViewEvent -= new ClientInfoUpdateForm.ListViewEventHandler(OnUpdateListViewEvent);   //Add by ChengSk - 20180830
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                bIsSelect = true;  //标记为进行了筛选操作  Add by ChengSk - 20190514

                List<ProcessInfoModel> lstProcessInfo = new List<ProcessInfoModel>();

                int count = this.LvwFruitData.SelectedItems.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this.LvwFruitData.SelectedItems[i].SubItems[6].Text.Trim() == "")
                        continue; //Add by ChengSk - 20190527 正在加工的不参与筛选

                    ProcessInfoModel processInfoModel = new ProcessInfoModel();
                    processInfoModel.CustomerID = this.LvwFruitData.SelectedItems[i].SubItems[0].Text;
                    processInfoModel.CustomerName = this.LvwFruitData.SelectedItems[i].SubItems[1].Text;
                    processInfoModel.FarmName = this.LvwFruitData.SelectedItems[i].SubItems[2].Text;
                    processInfoModel.FruitName = this.LvwFruitData.SelectedItems[i].SubItems[3].Text;
                    processInfoModel.CompletedState = this.LvwFruitData.SelectedItems[i].SubItems[4].Text;
                    processInfoModel.StartTime = this.LvwFruitData.SelectedItems[i].SubItems[5].Text;
                    processInfoModel.EndTime = this.LvwFruitData.SelectedItems[i].SubItems[6].Text;
                    processInfoModel.BatchWeight = this.LvwFruitData.SelectedItems[i].SubItems[7].Text;
                    processInfoModel.BatchNumber = this.LvwFruitData.SelectedItems[i].SubItems[8].Text;
                    lstProcessInfo.Add(processInfoModel);
                }

                InSertListView(lstProcessInfo);

                this.ExportExcelbutton.Enabled = true;   //Add by ChengSk - 20190527
                this.BtnDetailStatistics.Enabled = true; //Add by ChengSk - 20190527
                this.GradeExcelbutton.Enabled = true;    //Add by ChengSk - 20191111
            }
            catch (Exception ex)
            {
                MessageBox.Show("BtnFilter_Click Error! " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void InSertListView(List<ProcessInfoModel> lstProcessInfo)
        {
            //LvwFruitData清空
            LvwFruitData.Items.Clear();

            UInt64 BatchWeightTotals = 0; //批重量之和 Add by ChengSk - 20181123
            UInt64 BatchNumberTotals = 0; //批个数之和 Add by ChengSk - 20181123

            #region 循环往ListView中插入数据
            for (int i = 0; i < lstProcessInfo.Count; i++)
            {
                ListViewItem lv = new ListViewItem(lstProcessInfo[i].CustomerID);
                lv.SubItems.Add(lstProcessInfo[i].CustomerName);
                lv.SubItems.Add(lstProcessInfo[i].FarmName);
                lv.SubItems.Add(lstProcessInfo[i].FruitName);
                lv.SubItems.Add(lstProcessInfo[i].CompletedState);
                lv.SubItems.Add(lstProcessInfo[i].StartTime);
                lv.SubItems.Add(lstProcessInfo[i].EndTime);
                lv.SubItems.Add(lstProcessInfo[i].BatchWeight);
                lv.SubItems.Add(lstProcessInfo[i].BatchNumber);
                BatchWeightTotals += UInt64.Parse(lstProcessInfo[i].BatchWeight);
                BatchNumberTotals += UInt64.Parse(lstProcessInfo[i].BatchNumber);
                LvwFruitData.Items.Add(lv);
            }
            #endregion

            //添加“总计”行 Add by ChengSk - 20181123
            ListViewItem lvi = new ListViewItem("0");
            lvi.SubItems.Add("Sum");
            lvi.SubItems.Add("Sum");
            lvi.SubItems.Add("Sum");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add(BatchWeightTotals.ToString());
            lvi.SubItems.Add(BatchNumberTotals.ToString());
            LvwFruitData.Items.Add(lvi);

            #region 单双行更换背景色
            for (int i = 0; i < LvwFruitData.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    LvwFruitData.Items[i].BackColor = DrawGraphProtocol.myBackColor;
                }
            }
            #endregion
        }

        private void ExportExcelbutton_Click(object sender, EventArgs e)
        {
            if (LvwFruitData.Items.Count == 1)
            {
                MessageBox.Show("0x30001109 Query content is empty and cannot be exported!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "XLS格式(*.xls)|";
                dlg.RestoreDirectory = true;
                string strFileName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                strFileName = strFileName.Replace("-", "");
                strFileName = strFileName.Replace(" ", "");
                strFileName = strFileName.Replace(":", "");
                dlg.FileName = strFileName;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string temp = dlg.FileName + ".xls";
                    FileInfo File = new FileInfo(temp);
                    if (File.Exists)
                    {
                        //DialogResult result = MessageBox.Show("是否覆盖原来的配置信息?", "保存配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        DialogResult result = MessageBox.Show("0x30001021 Whether to overwrite the original configuration information?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                        File.Delete();
                    }

                    Excel.Application m_Excel = new Excel.Application();//创建一个Excel对象(同时启动EXCEL.EXE进程)
                    m_Excel.SheetsInNewWorkbook = 3;//工作表的个数
                    Microsoft.Office.Interop.Excel._Workbook m_Book = (Excel._Workbook)(m_Excel.Workbooks.Add(Missing.Value));//添加新工作簿
                    Excel._Worksheet m_Sheet = m_Book.Worksheets[1];

                    #region 创建表头

                    //最后载入logo图片
                    Excel.Range range = m_Sheet.get_Range("D1", "F1");   //取得单元格范围
                    range.MergeCells = true; //合并单元格
                    range.RowHeight = 30;
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;  //单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    Bitmap bitmap = new Bitmap(PrintProtocol.logoPathName);//创建位图对象;   
                    float width = (float)(bitmap.Width * range.Height / bitmap.Height);
                    m_Sheet.Shapes.AddPicture(PrintProtocol.logoPathName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                        100 + range.Left + range.Width / 2 - width / 2, range.Top, width, range.Height);
                    bitmap.Dispose();

                    //开始时间
                    range = m_Sheet.get_Range("C3", "C3");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblStartTime.Text") + ":";//开始时间
                    //range.ColumnWidth = 20;

                    //完成时间
                    range = m_Sheet.get_Range("F3", "F3");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblEndTime.Text") + ":";  //完成时间
                    //range.ColumnWidth = 20;

                    //客户数
                    range = m_Sheet.get_Range("C4", "C4");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblCustomerCount.Text") + ":";//客户数
                    //range.ColumnWidth = 20;

                    //批次数
                    range = m_Sheet.get_Range("F4", "F4");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblBatchCount.Text") + ":";  //批次数
                    //range.ColumnWidth = 20;

                    //总重量
                    range = m_Sheet.get_Range("C5", "C5");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblWeightTotals.Text") + ":";//总重量
                    //range.ColumnWidth = 20;

                    //总个数
                    range = m_Sheet.get_Range("F5", "F5");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblNumberTotals.Text") + ":";  //总个数
                    //range.ColumnWidth = 20;

                    range = m_Sheet.get_Range("D3", "D5");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = false;  //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右

                    range = m_Sheet.get_Range("G3", "G5");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = false;  //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右

                    range = m_Sheet.Range[m_Sheet.Cells[1, 3], m_Sheet.Cells[5, 7]];//取得单元格范围
                    //range.Font.Name = "宋体";//设置字体
                    //range.Font.Size = 12;    //字体大小
                    //range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    //range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向居中
                    range.Borders.LineStyle = Excel.XlLineStyle.xlDot;//设置边框
                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;//设置顶部边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;//设置底部边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;//设置左边边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;//设置右边边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细

                    #endregion

                    #region 创建表格

                    //序列号
                    range = m_Sheet.get_Range("A7", "A7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblCustomerID.Text");//序列号
                    range.ColumnWidth = 20;

                    //客户名称
                    range = m_Sheet.get_Range("B7", "B7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblCustomerName.Text");//客户名称
                    range.ColumnWidth = 20;

                    //农场名称
                    range = m_Sheet.get_Range("C7", "C7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblFarmName.Text");//农场名称
                    range.ColumnWidth = 20;

                    //水果名称
                    range = m_Sheet.get_Range("D7", "D7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblFruitName.Text");//水果名称
                    range.ColumnWidth = 20;

                    //加工状态
                    range = m_Sheet.get_Range("E7", "E7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblCompletedState.Text");//加工状态
                    range.ColumnWidth = 20;

                    //开始时间
                    range = m_Sheet.get_Range("F7", "F7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblStartTime.Text");//开始时间
                    range.ColumnWidth = 20;

                    //完成时间
                    range = m_Sheet.get_Range("G7", "G7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblEndTime.Text");//完成时间
                    range.ColumnWidth = 20;

                    //批重量
                    range = m_Sheet.get_Range("H7", "H7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblBatchWeight.Text");//批重量
                    range.ColumnWidth = 20;

                    //批个数
                    range = m_Sheet.get_Range("I7", "I7");//取得单元格范围
                    range.MergeCells = false; //合并单元格
                    range.Font.Name = "宋体"; //设置字体
                    range.Font.Size = 12;     //字体大小
                    range.Font.Bold = true;   //字体加粗
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向靠右
                    range.Value2 = m_resourceManager.GetString("LblBatchNumber.Text");//批个数
                    range.ColumnWidth = 20;

                    int lvwCount = LvwFruitData.Items.Count;

                    range = m_Sheet.Range[m_Sheet.Cells[7, 1], m_Sheet.Cells[7 + lvwCount - 1, 9]];//取得单元格范围
                    range.Font.Name = "宋体";//设置字体
                    range.Font.Size = 12;    //字体大小
                    range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格横向居中
                    range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//单元格纵向居中
                    range.Borders.LineStyle = Excel.XlLineStyle.xlDot;//设置边框
                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;//设置顶部边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;//设置底部边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;//设置左边边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;//设置右边边框
                    range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).Weight = Excel.XlBorderWeight.xlMedium;//边框常规粗细
                    //range.EntireRow.AutoFit();
                    //range.EntireColumn.AutoFit();

                    ////自动设置列宽 放到最后
                    //range = m_Sheet.Columns;
                    //range.AutoFit();
                    #endregion

                    #region 填充数据
                    List<string> lstCustomers = new List<string>(); //客户名称列表（不重复）
                    for (int i = 0; i < lvwCount - 1; i++)
                    {
                        if (!lstCustomers.Contains(LvwFruitData.Items[i].SubItems[1].Text))
                            lstCustomers.Add(LvwFruitData.Items[i].SubItems[1].Text);
                    }

                    if (lvwCount - 2 >= 0)
                    {
                        m_Sheet.Cells[3, 4] = LvwFruitData.Items[lvwCount - 2].SubItems[5].Text; //开始时间
                        m_Sheet.Cells[3, 7] = LvwFruitData.Items[0].SubItems[6].Text; //完成时间
                        m_Sheet.Cells[4, 4] = lstCustomers.Count.ToString(); //客户量
                        m_Sheet.Cells[4, 7] = (lvwCount - 1).ToString();     //批次数
                        //m_Sheet.Cells[5, 4] = LvwFruitData.Items[lvwCount - 1].SubItems[7].Text + " g"; //总重量
                        //m_Sheet.Cells[5, 7] = LvwFruitData.Items[lvwCount - 1].SubItems[8].Text; //总个数
                        m_Sheet.Cells[5, 4] = string.Format("{0:N0}", long.Parse(LvwFruitData.Items[lvwCount - 1].SubItems[7].Text)) + " g"; //总重量 逗号隔开 Modify by ChengSk - 20190926
                        m_Sheet.Cells[5, 7] = string.Format("{0:N0}", long.Parse(LvwFruitData.Items[lvwCount - 1].SubItems[8].Text)); //总个数 逗号隔开 Modify by ChengSk - 20190926
                    }

                    for (int i = 0; i < lvwCount - 1; i++)  //不打印最后一行
                    {
                        m_Sheet.Cells[8 + i, 1] = LvwFruitData.Items[i].SubItems[0].Text;
                        m_Sheet.Cells[8 + i, 2] = LvwFruitData.Items[i].SubItems[1].Text;
                        m_Sheet.Cells[8 + i, 3] = LvwFruitData.Items[i].SubItems[2].Text;
                        m_Sheet.Cells[8 + i, 4] = LvwFruitData.Items[i].SubItems[3].Text;
                        m_Sheet.Cells[8 + i, 5] = LvwFruitData.Items[i].SubItems[4].Text;
                        m_Sheet.Cells[8 + i, 6] = LvwFruitData.Items[i].SubItems[5].Text;
                        m_Sheet.Cells[8 + i, 7] = LvwFruitData.Items[i].SubItems[6].Text;
                        m_Sheet.Cells[8 + i, 8] = string.Format("{0:N0}", long.Parse(LvwFruitData.Items[i].SubItems[7].Text)); //逗号隔开 Modify by ChengSk - 20190926
                        m_Sheet.Cells[8 + i, 9] = string.Format("{0:N0}", long.Parse(LvwFruitData.Items[i].SubItems[8].Text)); //逗号隔开 Modify by ChengSk - 20190926
                    }

                    #endregion

                    ////自动设置列宽 放到最后
                    //range = m_Sheet.Columns;
                    //range.AutoFit();

                    #region 保存Excel,清除进程

                    m_Book.SaveAs(dlg.FileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange,
                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);//保存Excel
                    m_Book.Close(false, Missing.Value, Missing.Value);
                    m_Excel.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Sheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Book);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_Excel);
                    GC.Collect();
                    m_Book = null;
                    m_Sheet = null;
                    m_Excel = null;
                    #endregion

                    MessageBox.Show("Export excel report successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export Error! " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void ShowSplashForm()
        {
            m_ProgressBoxForm = new ProgressBoxForm(strInitPrompt);
            Application.Run(m_ProgressBoxForm);
        }

        private void BtnDetailStatistics_Click(object sender, EventArgs e)
        {
            try
            {
                int ItemsCount = LvwFruitData.Items.Count;
                if (ItemsCount == 1)
                {
                    MessageBox.Show("0x30001109 Query content is empty and cannot be detailed export forms!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                strInitPrompt = "Total number of reports: " + (ItemsCount - 1).ToString() + ", Exporting: 0 ... ...";

                Thread thSplash = new Thread(new ThreadStart(ShowSplashForm));
                thSplash.Priority = ThreadPriority.Normal;
                thSplash.IsBackground = true;
                thSplash.Start();

                Thread.Sleep(2000);

                string currentCustomerID = "";
                string currentCustomerName = "";
                string currentFarmName = "";
                string currentFruitName = "";
                for (int k = 0; k < ItemsCount - 1; k++)
                {
                    try
                    {
                        currentCustomerID = LvwFruitData.Items[k].SubItems[0].Text;
                        currentCustomerName = LvwFruitData.Items[k].SubItems[1].Text; //客户名称
                        currentFarmName = LvwFruitData.Items[k].SubItems[2].Text;  //农场名称
                        currentFruitName = LvwFruitData.Items[k].SubItems[3].Text; //水果名称
                        GlobalDataInterface.SerialNum = LvwFruitData.Items[k].SubItems[0].Text;//序列号
                        if (currentCustomerID == null || currentCustomerID.Equals("") || currentCustomerID == "0")
                        {
                            MessageBox.Show("30001102 Please select entries to statistics first!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (m_ProgressBoxForm != null)
                            {
                                m_ProgressBoxForm.Invoke(new MethodInvoker(delegate { m_ProgressBoxForm.Close(); }));
                            }
                            thSplash.Join(); //必须关闭啊
                            return;
                        }

                        if (m_ProgressBoxForm != null)
                        {
                            strInitPrompt = "Total number of reports: " + (ItemsCount - 1).ToString() + ", Exporting: " + (k + 1).ToString() + " ... ...";
                            m_ProgressBoxForm.Invoke(new MethodInvoker(delegate { m_ProgressBoxForm.lblPrompt.Text = strInitPrompt; }));
                        }

                        #region 查询数据

                        dataInterface = new DataInterface(true);
                        statisticsInfo = new stStatistics(true);
                        gradeInfo = new stGradeInfo(true);

                        dataInterface.BSourceDB = true;

                        #region 从数据库中取相应条目信息放到DataSet中
                        //获取tb_FruitInfo
                        //dst1 = BusinessFacade.GetFruitByCustomerID(Convert.ToInt32(currentCustomerID));
                        dst1 = databaseOperation.GetFruitByCustomerID(Convert.ToInt32(currentCustomerID));
                        if (dst1.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0"))
                        {
                            //MessageBox.Show("加工进行中 不能进行统计！");
                            MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        //获取tb_GradeInfo
                        //dst2 = BusinessFacade.GetGradeByCustomerID(Convert.ToInt32(currentCustomerID));
                        dst2 = databaseOperation.GetGradeByCustomerID(Convert.ToInt32(currentCustomerID));
                        //获取tb_ExportInfo
                        //dst3 = BusinessFacade.GetExportByCustomerID(Convert.ToInt32(currentCustomerID));
                        dst3 = databaseOperation.GetExportByCustomerID(Convert.ToInt32(currentCustomerID));
                        #endregion

                        #region 往dataInterface中插入水果信息
                        //往dataInterface中插入水果信息
                        if (dst1.Tables[0].Rows.Count <= 0)
                        {
                            //MessageBox.Show("选择条目的水果信息为空！");
                            MessageBox.Show("30001103 The currently selected fruit information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        dataInterface.BSourceDB = true;
                        dataInterface.CustomerID = Convert.ToInt32(dst1.Tables[0].Rows[0]["CustomerID"].ToString());
                        dataInterface.CustomerName = dst1.Tables[0].Rows[0]["CustomerName"].ToString();
                        dataInterface.FarmName = dst1.Tables[0].Rows[0]["FarmName"].ToString();
                        dataInterface.FruitName = dst1.Tables[0].Rows[0]["FruitName"].ToString();
                        dataInterface.StartTime = dst1.Tables[0].Rows[0]["StartTime"].ToString();
                        dataInterface.EndTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                        dataInterface.StartedState = dst1.Tables[0].Rows[0]["StartedState"].ToString();
                        dataInterface.CompletedState = dst1.Tables[0].Rows[0]["CompletedState"].ToString();
                        dataInterface.QualityGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString());
                        if (dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString().Equals(""))
                        {
                            dataInterface.WeightOrSizeGradeSum = 0;
                        }
                        else
                        {
                            dataInterface.WeightOrSizeGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString());
                        }
                        dataInterface.ExportSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["ExportSum"].ToString());
                        dataInterface.ColorGradeName = dst1.Tables[0].Rows[0]["ColorGradeName"].ToString();
                        dataInterface.ShapeGradeName = dst1.Tables[0].Rows[0]["ShapeGradeName"].ToString();
                        dataInterface.FlawGradeName = dst1.Tables[0].Rows[0]["FlawGradeName"].ToString();
                        dataInterface.HardGradeName = dst1.Tables[0].Rows[0]["HardGradeName"].ToString();
                        dataInterface.DensityGradeName = dst1.Tables[0].Rows[0]["DensityGradeName"].ToString();
                        dataInterface.SugarDegreeGradeName = dst1.Tables[0].Rows[0]["SugarDegreeGradeName"].ToString();
                        dataInterface.ProgramName = dst1.Tables[0].Rows[0]["ProgramName"].ToString();
                        statisticsInfo.nWeightCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchWeight"].ToString());
                        statisticsInfo.nTotalCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchNumber"].ToString());
                        #endregion

                        #region 往dataInterface中插入等级信息
                        //往dataInterface中插入等级信息
                        if (dst2.Tables[0].Rows.Count <= 0)
                        {
                            //MessageBox.Show("选择条目的等级信息为空！");
                            MessageBox.Show("30001104 The currently selected Grade information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        for (int i = 0; i < dst2.Tables[0].Rows.Count; i++)
                        {
                            //存水果箱数
                            statisticsInfo.nBoxGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToInt32(dst2.Tables[0].Rows[i]["BoxNumber"].ToString());
                            //存水果个数
                            statisticsInfo.nGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitNumber"].ToString());
                            //存水果重量
                            statisticsInfo.nWeightGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitWeight"].ToString());
                            //存品质名称
                            if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) == 0)
                            {
                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                                0,
                                FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                                gradeInfo.strQualityGradeName,
                                i * ConstPreDefine.MAX_TEXT_LENGTH);
                                //取品质名称
                                //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                                //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                                //    ConstPreDefine.MAX_TEXT_LENGTH));

                                //存重量/尺寸名称
                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                                    0,
                                    FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                                    gradeInfo.strSizeGradeName,
                                    i * ConstPreDefine.MAX_TEXT_LENGTH);
                            }
                            else  //有品质特征时：品质+尺寸  品质+重量
                            {
                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                                0,
                                FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                                gradeInfo.strQualityGradeName,
                                (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) / ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                                //取品质名称
                                //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                                //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                                //    ConstPreDefine.MAX_TEXT_LENGTH));
                                //存重量/尺寸名称

                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                                    0,
                                    FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                                    gradeInfo.strSizeGradeName,
                                    (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) % ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                            }
                            //存重量/尺寸限制
                            gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nMinSize = float.Parse(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString());//Convert.ToInt32(Convert.ToInt32(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString()));
                            //存重量/尺寸选择
                            if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) > 0)  //有品质
                            {
                                if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                                {
                                    gradeInfo.nClassifyType = 4;
                                }
                                else  //选重量
                                {
                                    gradeInfo.nClassifyType = 2;
                                }
                            }
                            else  //无品质
                            {
                                if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                                {
                                    gradeInfo.nClassifyType = 4;
                                }
                                else  //选重量
                                {
                                    gradeInfo.nClassifyType = 2;
                                }
                            }
                            //存重量/尺寸特征

                            //存颜色特征
                            if (dst2.Tables[0].Rows[i]["TraitColor"].ToString() == null || dst2.Tables[0].Rows[i]["TraitColor"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = Convert.ToSByte(dst2.Tables[0].Rows[i]["TraitColor"].ToString());
                            }
                            //存形状特征
                            if (dst2.Tables[0].Rows[i]["TraitShape"].ToString() == null || dst2.Tables[0].Rows[i]["TraitShape"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitShape"].ToString());
                            }
                            //存瑕疵特征
                            if (dst2.Tables[0].Rows[i]["TraitFlaw"].ToString() == null || dst2.Tables[0].Rows[i]["TraitFlaw"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitFlaw"].ToString());
                            }
                            //存硬度特征
                            if (dst2.Tables[0].Rows[i]["TraitHard"].ToString() == null || dst2.Tables[0].Rows[i]["TraitHard"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitHard"].ToString());
                            }
                            //存密度特征
                            if (dst2.Tables[0].Rows[i]["TraitDensity"].ToString() == null || dst2.Tables[0].Rows[i]["TraitDensity"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitDensity"].ToString());
                            }
                            //存含糖量特征
                            if (dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString() == null || dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString());
                            }

                        }
                        #endregion

                        #region 往dataInterface中插入出口信息
                        //往dataInterface中插入出口信息
                        if (dst3.Tables[0].Rows.Count <= 0)
                        {
                            //MessageBox.Show("选择条目的出口信息为空！");
                            MessageBox.Show("30001105 The currently selected Outlets information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        for (int i = 0; i < dst3.Tables[0].Rows.Count; i++)
                        {
                            statisticsInfo.nExitCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitNumber"].ToString());
                            statisticsInfo.nExitWeightCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitWeight"].ToString());
                        }
                        #endregion

                        #region 往DataInterface类中汇总结构体数据
                        //往DataInterface类中汇总数据
                        dataInterface.IoStStatistics = statisticsInfo;
                        dataInterface.IoStStGradeInfo = gradeInfo;
                        #endregion

                        #region 判断当前加工状态能否进行统计
                        if (dataInterface.StartedState.Equals("1") && dataInterface.CompletedState.Equals("0"))
                        {
                            //MessageBox.Show("加工进行中 不能进行统计！");
                            MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        #endregion

                        #endregion

                        #region 开始打印

                        if (!Directory.Exists(System.Environment.CurrentDirectory + "\\Report\\"))
                            Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\Report\\");

                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.Filter = "XLS格式(*.xls)|";
                        dlg.RestoreDirectory = true;
                        string strFileName = dataInterface.EndTime;
                        strFileName = strFileName.Replace("-", "");
                        strFileName = strFileName.Replace(" ", "");
                        strFileName = strFileName.Replace(":", "");
                        dlg.FileName = System.Environment.CurrentDirectory + "\\Report\\" +
                            currentCustomerName + " - " + currentFarmName + " - " + currentFruitName + " - " + strFileName + ".xls";

                        string temp = dlg.FileName;
                        FileInfo File = new FileInfo(temp);
                        //if (File.Exists)
                        //{
                        //    //DialogResult result = MessageBox.Show("是否覆盖原来的配置信息?", "保存配置", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //    DialogResult result = MessageBox.Show("0x30001021 Whether to overwrite the original configuration information?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //    if (result == DialogResult.No)
                        //    {
                        //        return;
                        //    }
                        //}
                        string[] resource = { DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text")),m_resourceManager.GetString("LblPrintBatchReport.Text"),m_resourceManager.GetString("LblPrintCustomerName.Text"),
                                     m_resourceManager.GetString("LblPrintFarmName.Text"),m_resourceManager.GetString("LblPrintFruitVarieties.Text"),m_resourceManager.GetString("LblPrintTotalPieces.Text"),
                                     m_resourceManager.GetString("LblPrintTotalWeight.Text"),m_resourceManager.GetString("LblPrintTName.Text"),m_resourceManager.GetString("LblPrintTotalCartons.Text"),
                                     m_resourceManager.GetString("LblPrintAveFruitWeight.Text"),m_resourceManager.GetString("LblPrintGName.Text"),m_resourceManager.GetString("LblPrintProgramName.Text"),
                                     m_resourceManager.GetString("LblExcelStartTime.Text"),m_resourceManager.GetString("LblExcelEndTime.Text"), m_resourceManager.GetString("LblMainReportName.Text"), 
                                     m_resourceManager.GetString("LblMainReportSize.Text"),m_resourceManager.GetString("LblMainReportPieces.Text"), m_resourceManager.GetString("LblMainReportPiecesPer.Text"),
                                     m_resourceManager.GetString("LblMainReportWeights.Text"),m_resourceManager.GetString("LblMainReportWeightPer.Text"), m_resourceManager.GetString("LblMainReportCartons.Text"),
                                     m_resourceManager.GetString("LblMainReportCartonsPer.Text"), m_resourceManager.GetString("LblCustomerID.Text")};

                        if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) > 0) //有品质 
                        {
                            ExcelReportFunc.CreateExcel(dlg.FileName, dataInterface, resource, true, true);
                        }
                        else{
                            ExcelReportFunc.CreateExcel(dlg.FileName, dataInterface, resource, false, true);
                        } //Modify by ChengSk - 20191104
                        
                        #endregion
                    }
                    catch (Exception ee)
                    {
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("ProcessInfoForm中函数BtnDetailStatistics_Click内部出错: " + ee.StackTrace);
#endif
                    }
                }

                if (m_ProgressBoxForm != null)
                {
                    m_ProgressBoxForm.Invoke(new MethodInvoker(delegate { m_ProgressBoxForm.Close(); }));
                }
                thSplash.Join(); //必须关闭啊

                MessageBox.Show("Detailed report success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string v_OpenFolderPath = System.Environment.CurrentDirectory + "\\Report";
                System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath); //打开目录
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProcessInfoForm中函数BtnDetailStatistics_Click出错: " + ex.StackTrace);
#endif
            }
        }

        private void GradeExcelbutton_Click(object sender, EventArgs e)
        {
            try
            {
                this.GradeExcelbutton.Enabled = false;
                int ItemsCount = LvwFruitData.Items.Count;
                if (ItemsCount == 1)
                {
                    MessageBox.Show("0x30001109 Query content is empty and cannot be detailed export forms!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.GradeExcelbutton.Enabled = true;
                    return;
                }

                string currentCustomerID = "";
                string currentCustomerName = "";
                string currentFarmName = "";
                string currentFruitName = "";

                DataInterface grademergeDataInterface = new DataInterface(true);
                stStatistics grademergeStatisticsInfo = new stStatistics(true);
                stGradeInfo grademergeGradeInfo = new stGradeInfo(true);
                GlobalDataInterface.SerialNum = LvwFruitData.Items[0].SubItems[0].Text;//序列号
                for (int k = 0; k < ItemsCount - 1; k++)
                {
                    try
                    {
                        currentCustomerID = LvwFruitData.Items[k].SubItems[0].Text;
                        currentCustomerName = LvwFruitData.Items[k].SubItems[1].Text; //客户名称
                        currentFarmName = LvwFruitData.Items[k].SubItems[2].Text;     //农场名称
                        currentFruitName = LvwFruitData.Items[k].SubItems[3].Text;    //水果名称

                        if (currentCustomerID == null || currentCustomerID.Equals("") || currentCustomerID == "0")
                        {
                            MessageBox.Show("30001102 Please select entries to statistics first!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.GradeExcelbutton.Enabled = true;
                            return;
                        }

                        #region 查询数据

                        dataInterface = new DataInterface(true);
                        statisticsInfo = new stStatistics(true);
                        gradeInfo = new stGradeInfo(true);

                        dataInterface.BSourceDB = true;
                        grademergeDataInterface.BSourceDB = true;

                        #region 从数据库中取相应条目信息放到DataSet中
                        //获取tb_FruitInfo
                        //dst1 = BusinessFacade.GetFruitByCustomerID(Convert.ToInt32(currentCustomerID));
                        dst1 = databaseOperation.GetFruitByCustomerID(Convert.ToInt32(currentCustomerID));
                        if (dst1.Tables[0].Rows[0]["CompletedState"].ToString().Equals("0"))
                        {
                            //MessageBox.Show("加工进行中 不能进行统计！");
                            MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.GradeExcelbutton.Enabled = true;
                            return;
                        }
                        //获取tb_GradeInfo
                        //dst2 = BusinessFacade.GetGradeByCustomerID(Convert.ToInt32(currentCustomerID));
                        dst2 = databaseOperation.GetGradeByCustomerID(Convert.ToInt32(currentCustomerID));
                        //获取tb_ExportInfo
                        //dst3 = BusinessFacade.GetExportByCustomerID(Convert.ToInt32(currentCustomerID));
                        dst3 = databaseOperation.GetExportByCustomerID(Convert.ToInt32(currentCustomerID));
                        #endregion

                        #region 往dataInterface中插入水果信息
                        //往dataInterface中插入水果信息
                        if (dst1.Tables[0].Rows.Count <= 0)
                        {
                            //MessageBox.Show("选择条目的水果信息为空！");
                            MessageBox.Show("30001103 The currently selected fruit information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.GradeExcelbutton.Enabled = true;
                            return;
                        }
                        dataInterface.BSourceDB = true;
                        dataInterface.CustomerID = Convert.ToInt32(dst1.Tables[0].Rows[0]["CustomerID"].ToString());
                        dataInterface.CustomerName = dst1.Tables[0].Rows[0]["CustomerName"].ToString();
                        dataInterface.FarmName = dst1.Tables[0].Rows[0]["FarmName"].ToString();
                        dataInterface.FruitName = dst1.Tables[0].Rows[0]["FruitName"].ToString();
                        dataInterface.StartTime = dst1.Tables[0].Rows[0]["StartTime"].ToString();
                        dataInterface.EndTime = dst1.Tables[0].Rows[0]["EndTime"].ToString();
                        dataInterface.StartedState = dst1.Tables[0].Rows[0]["StartedState"].ToString();
                        dataInterface.CompletedState = dst1.Tables[0].Rows[0]["CompletedState"].ToString();
                        dataInterface.QualityGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString());
                        if (dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString().Equals(""))
                        {
                            dataInterface.WeightOrSizeGradeSum = 0;
                        }
                        else
                        {
                            dataInterface.WeightOrSizeGradeSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["WeightOrSizeGradeSum"].ToString());
                        }
                        dataInterface.ExportSum = Convert.ToInt32(dst1.Tables[0].Rows[0]["ExportSum"].ToString());
                        dataInterface.ColorGradeName = dst1.Tables[0].Rows[0]["ColorGradeName"].ToString();
                        dataInterface.ShapeGradeName = dst1.Tables[0].Rows[0]["ShapeGradeName"].ToString();
                        dataInterface.FlawGradeName = dst1.Tables[0].Rows[0]["FlawGradeName"].ToString();
                        dataInterface.HardGradeName = dst1.Tables[0].Rows[0]["HardGradeName"].ToString();
                        dataInterface.DensityGradeName = dst1.Tables[0].Rows[0]["DensityGradeName"].ToString();
                        dataInterface.SugarDegreeGradeName = dst1.Tables[0].Rows[0]["SugarDegreeGradeName"].ToString();
                        dataInterface.ProgramName = dst1.Tables[0].Rows[0]["ProgramName"].ToString();
                        statisticsInfo.nWeightCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchWeight"].ToString());
                        statisticsInfo.nTotalCount = Convert.ToUInt32(dst1.Tables[0].Rows[0]["BatchNumber"].ToString());
                        grademergeStatisticsInfo.nWeightCount += statisticsInfo.nWeightCount;  //merge
                        grademergeStatisticsInfo.nTotalCount += statisticsInfo.nTotalCount;    //merge

                        if (k == 0)
                        {
                            grademergeDataInterface.CustomerName = dataInterface.CustomerName;
                            grademergeDataInterface.FarmName = dataInterface.FarmName;
                            grademergeDataInterface.FruitName = dataInterface.FruitName;
                            grademergeDataInterface.EndTime = dataInterface.EndTime;
                            grademergeDataInterface.StartedState = dataInterface.StartedState;
                            grademergeDataInterface.CompletedState = dataInterface.CompletedState;
                            grademergeDataInterface.QualityGradeSum = dataInterface.QualityGradeSum;
                            grademergeDataInterface.WeightOrSizeGradeSum = dataInterface.WeightOrSizeGradeSum;
                            grademergeDataInterface.ExportSum = dataInterface.ExportSum;
                            grademergeDataInterface.ColorGradeName = dataInterface.ColorGradeName;
                            grademergeDataInterface.ShapeGradeName = dataInterface.ShapeGradeName;
                            grademergeDataInterface.FlawGradeName = dataInterface.FlawGradeName;
                            grademergeDataInterface.HardGradeName = dataInterface.HardGradeName;
                            grademergeDataInterface.DensityGradeName = dataInterface.DensityGradeName;
                            grademergeDataInterface.SugarDegreeGradeName = dataInterface.SugarDegreeGradeName;
                            grademergeDataInterface.ProgramName = dataInterface.ProgramName;
                        }
                        else if (k == ItemsCount - 2)
                        {
                            grademergeDataInterface.StartTime = dataInterface.StartTime;
                        }
                        #endregion

                        #region 往dataInterface中插入等级信息
                        //往dataInterface中插入等级信息
                        if (dst2.Tables[0].Rows.Count <= 0)
                        {
                            //MessageBox.Show("选择条目的等级信息为空！");
                            MessageBox.Show("30001104 The currently selected Grade information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.GradeExcelbutton.Enabled = true;
                            return;
                        }
                        for (int i = 0; i < dst2.Tables[0].Rows.Count; i++)
                        {
                            //存水果箱数
                            statisticsInfo.nBoxGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToInt32(dst2.Tables[0].Rows[i]["BoxNumber"].ToString());
                            grademergeStatisticsInfo.nBoxGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] += statisticsInfo.nBoxGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())]; //merge
                            //存水果个数
                            statisticsInfo.nGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitNumber"].ToString());
                            grademergeStatisticsInfo.nGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] += statisticsInfo.nGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())]; //merge
                            //存水果重量
                            statisticsInfo.nWeightGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] = Convert.ToUInt32(dst2.Tables[0].Rows[i]["FruitWeight"].ToString());
                            grademergeStatisticsInfo.nWeightGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())] += statisticsInfo.nWeightGradeCount[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())]; //merge
                            //存品质名称
                            if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) == 0)
                            {
                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                                0,
                                FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                                gradeInfo.strQualityGradeName,
                                i * ConstPreDefine.MAX_TEXT_LENGTH);
                                //取品质名称
                                //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                                //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                                //    ConstPreDefine.MAX_TEXT_LENGTH));

                                //存重量/尺寸名称
                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                                    0,
                                    FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                                    gradeInfo.strSizeGradeName,
                                    i * ConstPreDefine.MAX_TEXT_LENGTH);

                                if (k == 0)
                                {
                                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                                        0,
                                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                                        grademergeGradeInfo.strQualityGradeName,
                                        i * ConstPreDefine.MAX_TEXT_LENGTH);

                                    //存重量/尺寸名称
                                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                                        0,
                                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                                        grademergeGradeInfo.strSizeGradeName,
                                        i * ConstPreDefine.MAX_TEXT_LENGTH);
                                }
                            }
                            else  //有品质特征时：品质+尺寸  品质+重量
                            {
                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                                0,
                                FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                                gradeInfo.strQualityGradeName,
                                (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) / ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                                //取品质名称
                                //MessageBox.Show(Encoding.Default.GetString(gradeInfo.strQualityGradeName,
                                //    Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) * ConstPreDefine.MAX_TEXT_LENGTH,
                                //    ConstPreDefine.MAX_TEXT_LENGTH));
                                //存重量/尺寸名称

                                Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                                    0,
                                    FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                                    gradeInfo.strSizeGradeName,
                                    (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) % ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);

                                if (k == 0)
                                {
                                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()),
                                        0,
                                        FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["QualityName"].ToString()).Length,
                                        grademergeGradeInfo.strQualityGradeName,
                                        (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) / ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);

                                    Encoding.Default.GetBytes(FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()),
                                          0,
                                          FunctionInterface.NullToString(dst2.Tables[0].Rows[i]["WeightOrSizeName"].ToString()).ToString().Length,
                                          grademergeGradeInfo.strSizeGradeName,
                                          (Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString()) % ConstPreDefine.MAX_SIZE_GRADE_NUM) * ConstPreDefine.MAX_TEXT_LENGTH);
                                }
                            }
                            //存重量/尺寸限制
                            gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nMinSize = float.Parse(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString());//Convert.ToInt32(Convert.ToInt32(dst2.Tables[0].Rows[i]["WeightOrSizeLimit"].ToString()));
                            if (k == 0)
                            {
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nMinSize = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nMinSize;
                            }
                            //存重量/尺寸选择
                            if (Convert.ToInt32(dst1.Tables[0].Rows[0]["QualityGradeSum"].ToString()) > 0)  //有品质
                            {
                                if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                                {
                                    gradeInfo.nClassifyType = 4;
                                }
                                else  //选重量
                                {
                                    gradeInfo.nClassifyType = 2;
                                }
                            }
                            else  //无品质
                            {
                                if (dst2.Tables[0].Rows[i]["SelectWeightOrSize"].ToString().Equals("0"))   //选尺寸
                                {
                                    gradeInfo.nClassifyType = 4;
                                }
                                else  //选重量
                                {
                                    gradeInfo.nClassifyType = 2;
                                }
                            }
                            if (k == 0)
                            {
                                grademergeGradeInfo.nClassifyType = gradeInfo.nClassifyType;
                            }
                            //存重量/尺寸特征

                            //存颜色特征
                            if (dst2.Tables[0].Rows[i]["TraitColor"].ToString() == null || dst2.Tables[0].Rows[i]["TraitColor"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = Convert.ToSByte(dst2.Tables[0].Rows[i]["TraitColor"].ToString());
                            }
                            //存形状特征
                            if (dst2.Tables[0].Rows[i]["TraitShape"].ToString() == null || dst2.Tables[0].Rows[i]["TraitShape"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitShape"].ToString());
                            }
                            //存瑕疵特征
                            if (dst2.Tables[0].Rows[i]["TraitFlaw"].ToString() == null || dst2.Tables[0].Rows[i]["TraitFlaw"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitFlaw"].ToString());
                            }
                            //存硬度特征
                            if (dst2.Tables[0].Rows[i]["TraitHard"].ToString() == null || dst2.Tables[0].Rows[i]["TraitHard"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitHard"].ToString());
                            }
                            //存密度特征
                            if (dst2.Tables[0].Rows[i]["TraitDensity"].ToString() == null || dst2.Tables[0].Rows[i]["TraitDensity"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitDensity"].ToString());
                            }
                            //存含糖量特征
                            if (dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString() == null || dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString().Equals(""))
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = 0x7F;
                            }
                            else
                            {
                                gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = (SByte)Convert.ToByte(dst2.Tables[0].Rows[i]["TraitSugarDegree"].ToString());
                            }

                            if (k == 0)
                            {
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].nColorGrade;
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbShapeSize;
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbFlawArea;
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbRigidity;
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbDensity;
                                grademergeGradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar = gradeInfo.grades[Convert.ToInt32(dst2.Tables[0].Rows[i]["GradeID"].ToString())].sbSugar;
                            }
                        }
                        #endregion

                        #region 往dataInterface中插入出口信息
                        //往dataInterface中插入出口信息
                        if (dst3.Tables[0].Rows.Count <= 0)
                        {
                            //MessageBox.Show("选择条目的出口信息为空！");
                            MessageBox.Show("30001105 The currently selected Outlets information is empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.GradeExcelbutton.Enabled = true;
                            return;
                        }
                        for (int i = 0; i < dst3.Tables[0].Rows.Count; i++)
                        {
                            statisticsInfo.nExitCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitNumber"].ToString());
                            grademergeStatisticsInfo.nExitCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] += statisticsInfo.nExitCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())]; //merge
                            statisticsInfo.nExitWeightCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] = Convert.ToUInt32(dst3.Tables[0].Rows[i]["FruitWeight"].ToString());
                            grademergeStatisticsInfo.nExitWeightCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())] += statisticsInfo.nExitWeightCount[Convert.ToInt32(dst3.Tables[0].Rows[i]["ExportID"].ToString())]; //merge
                        }
                        #endregion

                        #region 往DataInterface类中汇总结构体数据
                        ////往DataInterface类中汇总数据
                        //dataInterface.IoStStatistics = statisticsInfo;
                        //dataInterface.IoStStGradeInfo = gradeInfo;
                        #endregion

                        #region 判断当前加工状态能否进行统计
                        if (dataInterface.StartedState.Equals("1") && dataInterface.CompletedState.Equals("0"))
                        {
                            //MessageBox.Show("加工进行中 不能进行统计！");
                            MessageBox.Show("20001101 In the processing, can not be statistics!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.GradeExcelbutton.Enabled = true;
                            return;
                        }
                        #endregion
                        #endregion  
                    }
                    catch (Exception ee)
                    {
#if REALEASE
                        GlobalDataInterface.WriteErrorInfo("ProcessInfoForm中函数BtnDetailStatistics_Click内部出错: " + ee.StackTrace);
#endif
                    }
                }

                #region 往DataInterface类中汇总结构体数据
                //往DataInterface类中汇总数据
                grademergeDataInterface.IoStStatistics = grademergeStatisticsInfo;
                grademergeDataInterface.IoStStGradeInfo = grademergeGradeInfo;
                #endregion

                #region 开始打印

                if (!Directory.Exists(System.Environment.CurrentDirectory + "\\Report\\"))
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\Report\\");

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "XLS格式(*.xls)|";
                dlg.RestoreDirectory = true;
                string strFileName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                strFileName = strFileName.Replace("-", "");
                strFileName = strFileName.Replace(" ", "");
                strFileName = strFileName.Replace(":", "");
                strFileName = "等级合并_" + strFileName;
                dlg.FileName = System.Environment.CurrentDirectory + "\\Report\\" + strFileName + ".xlsx";  //将.xls改成.xlsx Modify by ChengSk - 20191104

                string temp = dlg.FileName;
                FileInfo File = new FileInfo(temp);
                string[] resource = { DateTime.Now.ToString(m_resourceManager.GetString("LblPrintDateTime.Text")),m_resourceManager.GetString("LblPrintBatchReport.Text"),m_resourceManager.GetString("LblPrintCustomerName.Text"),
                                     m_resourceManager.GetString("LblPrintFarmName.Text"),m_resourceManager.GetString("LblPrintFruitVarieties.Text"),m_resourceManager.GetString("LblPrintTotalPieces.Text"),
                                     m_resourceManager.GetString("LblPrintTotalWeight.Text"),m_resourceManager.GetString("LblPrintTName.Text"),m_resourceManager.GetString("LblPrintTotalCartons.Text"),
                                     m_resourceManager.GetString("LblPrintAveFruitWeight.Text"),m_resourceManager.GetString("LblPrintGName.Text"),m_resourceManager.GetString("LblPrintProgramName.Text"),
                                     m_resourceManager.GetString("LblExcelStartTime.Text"),m_resourceManager.GetString("LblExcelEndTime.Text"), m_resourceManager.GetString("LblMainReportName.Text"),
                                     m_resourceManager.GetString("LblMainReportSize.Text"),m_resourceManager.GetString("LblMainReportPieces.Text"), m_resourceManager.GetString("LblMainReportPiecesPer.Text"),
                                     m_resourceManager.GetString("LblMainReportWeights.Text"),m_resourceManager.GetString("LblMainReportWeightPer.Text"), m_resourceManager.GetString("LblMainReportCartons.Text"),
                                     m_resourceManager.GetString("LblMainReportCartonsPer.Text"), m_resourceManager.GetString("LblCustomerID.Text")};
                if (grademergeDataInterface.QualityGradeSum > 0)       //有品质 Modify by ChengSk - 20191104
                {
                    ExcelReportFunc.CreateExcel(dlg.FileName, grademergeDataInterface, resource, true, true);
                }
                else
                {
                    ExcelReportFunc.CreateExcel(dlg.FileName, grademergeDataInterface, resource, false, true);
                }

                MessageBox.Show("Grade merge report success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string v_OpenFolderPath = System.Environment.CurrentDirectory + "\\Report";
                System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath); //打开目录

                this.GradeExcelbutton.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("ProcessInfoForm中函数GradeExcelbutton_Click出错: " + ex.StackTrace);
#endif
                MessageBox.Show("选择批次对应的等级数不一致，导致出错！错误信息：" + ex.ToString());
                this.GradeExcelbutton.Enabled = true;
            }
        }
    }

    public class ProcessInfoModel
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string FarmName { get; set; }
        public string FruitName { get; set; }
        public string CompletedState { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BatchWeight { get; set; }
        public string BatchNumber { get; set; }
    }
}
