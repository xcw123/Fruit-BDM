using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interface;

namespace FruitSortingVtest1.DB
{
    public class DataInterface
    {
        private bool bSourceDB;                 //数据结构是否来源于数据库（true），界面（false） 
        private int customerID;                 //客户编号
        private string customerName;            //客户名称
        private string farmName;                //农场名称
        private string fruitName;               //水果品种
        private string startTime;               //开始时间
        private string endTime;                 //结束时间
        private string startedState;            //开始状态
        private string completedState;          //完成状态
        private int qualityGradeSum;            //品质等级个数
        private int weightOrSizeGradeSum;       //重量or尺寸等级个数
        private int exportSum;                  //出口个数
        private string colorGradeName;          //颜色等级名称
        private string shapeGradeName;          //形状等级名称
        private string flawGradeName;           //瑕疵等级名称
        private string hardGradeName;           //硬度等级名称
        private string densityGradeName;        //密度等级名称
        private string sugarDegreeGradeName;    //含糖量等级名称
        private string programName;             //分选程序名称
        private stStatistics ioStStatistics;    //基本统计信息
        private stGradeInfo ioStStGradeInfo;    //基本等级信息

        #region 属性
        public bool BSourceDB
        {
            get { return bSourceDB; }
            set { bSourceDB = value; }
        }

        public int CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }
        
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }    

        public string FarmName
        {
            get { return farmName; }
            set { farmName = value; }
        }      

        public string FruitName
        {
            get { return fruitName; }
            set { fruitName = value; }
        }     

        public string StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }  

        public string EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }  

        public string StartedState
        {
            get { return startedState; }
            set { startedState = value; }
        }      

        public string CompletedState
        {
            get { return completedState; }
            set { completedState = value; }
        }    

        public int QualityGradeSum
        {
            get { return qualityGradeSum; }
            set { qualityGradeSum = value; }
        }      

        public int WeightOrSizeGradeSum
        {
            get { return weightOrSizeGradeSum; }
            set { weightOrSizeGradeSum = value; }
        }

        public int ExportSum
        {
            get { return exportSum; }
            set { exportSum = value; }
        }

        public string ColorGradeName
        {
            get { return colorGradeName; }
            set { colorGradeName = value; }
        }

        public string ShapeGradeName
        {
            get { return shapeGradeName; }
            set { shapeGradeName = value; }
        }

        public string FlawGradeName
        {
            get { return flawGradeName; }
            set { flawGradeName = value; }
        }

        public string HardGradeName
        {
            get { return hardGradeName; }
            set { hardGradeName = value; }
        }

        public string DensityGradeName
        {
            get { return densityGradeName; }
            set { densityGradeName = value; }
        }

        public string SugarDegreeGradeName
        {
            get { return sugarDegreeGradeName; }
            set { sugarDegreeGradeName = value; }
        }

        public string ProgramName
        {
            get { return programName; }
            set { programName = value; }
        }

        public stStatistics IoStStatistics
        {
            get { return ioStStatistics; }
            set { ioStStatistics = value; }
        }    

        public stGradeInfo IoStStGradeInfo
        {
            get { return ioStStGradeInfo; }
            set { ioStStGradeInfo = value; }
        }
        #endregion

        public DataInterface(bool bOK)
        {
            bSourceDB = false;
            customerID = 0;
            customerName = "";
            farmName = "";
            fruitName = "";
            startTime = "";
            endTime = "";
            startedState = "";
            completedState = "";
            qualityGradeSum = 0;
            weightOrSizeGradeSum = 0;
            exportSum = 0;
            colorGradeName = "";
            shapeGradeName = "";
            flawGradeName = "";
            hardGradeName = "";
            densityGradeName = "";
            sugarDegreeGradeName = "";
            programName = "";
            ioStStatistics = new stStatistics(true);
            ioStStGradeInfo = new stGradeInfo(true);
        }
    }
}
