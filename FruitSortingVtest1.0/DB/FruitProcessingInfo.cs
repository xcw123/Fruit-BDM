using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FruitSortingVtest1.DB
{
    public class FruitProcessingInfo
    {
        public string DeviceNumber { get; set; }     //设备编号
        public string t1_CustomerID { get; set; }    //客户ID（int）
        public string t1_CustomerName { get; set; }  //客户名称
        public string t1_FarmName { get; set; }      //农场名称
        public string t1_FruitName { get; set; }     //水果名称
        public string t1_StartTime { get; set; }     //开始时间
        public string t1_EndTime { get; set; }       //结束时间
        public string t1_StartedState { get; set; }  //开始状态
        public string t1_CompletedState { get; set; }//完成状态
        public string t1_BatchWeight { get; set; }   //批重量（int）
        public string t1_BatchNumber { get; set; }   //批个数（int）
        public string t1_QualityGradeSum { get; set; }     //品质个数（int）
        public string t1_WeightOrSizeGradeNum { get; set; }//重量或尺寸个数（int）
        public string t1_ExportSum { get; set; }       //出口个数（int）
        public string t1_ColorGradeName { get; set; }  //颜色等级名称
        public string t1_ShapeGradeName { get; set; }  //形状等级名称
        public string t1_FlawGradeName { get; set; }   //瑕疵等级名称
        public string t1_HardGradeName { get; set; }   //硬度等级名称
        public string t1_DensityGradeName { get; set; }//密度等级名称
        public string t1_SugarDegreeGradeName { get; set; }  //含糖量等级名称
        public string t1_ProgramName { get; set; }       //程序名称
        public List<string> t2_GradeID { get; set; }     //等级ID（int）
        public List<string> t2_BoxNumber { get; set; }   //箱数（int）
        public List<string> t2_FruitNumber { get; set; } //水果个数（int）
        public List<string> t2_FruitWeight { get; set; } //水果重量（int）
        public List<string> t2_QualityName { get; set; } //品质名称
        public List<string> t2_WeightOrSizeName { get; set; }  //重量或尺寸名称
        public List<string> t2_WeightOrSizeLimit { get; set; } //重量或尺寸最小值（float）
        public List<string> t2_SelectWeightOrSize { get; set; }//重量或尺寸标志
        public List<string> t2_TraitWeightOrSize { get; set; } //重量或尺寸特征
        public List<string> t2_TraitColor { get; set; }  //颜色特征
        public List<string> t2_TraitShape { get; set; }  //形状特征
        public List<string> t2_TraitFlaw { get; set; }   //瑕疵特征
        public List<string> t2_TraitHard { get; set; }   //硬度特征
        public List<string> t2_TraitDensity { get; set; }//密度特征
        public List<string> t2_TraitSugarDegree { get; set; } //含糖量特征
        public List<string> t3_ExportID { get; set; }    //出口ID（int）
        public List<string> t3_FruitNumber { get; set; } //水果个数（int）
        public List<string> t3_FruitWeight { get; set; } //水果重量（int）

        public FruitProcessingInfo()
        {
            DeviceNumber = "";
            t1_CustomerID = "";
            t1_CustomerName = "";
            t1_FarmName = "";
            t1_FruitName = "";
            t1_StartTime = "";
            t1_EndTime = "";
            t1_StartedState = "";
            t1_CompletedState = "";
            t1_BatchWeight = "";
            t1_BatchNumber = "";
            t1_QualityGradeSum = "";
            t1_WeightOrSizeGradeNum = "";
            t1_ExportSum = "";
            t1_ColorGradeName = "";
            t1_ShapeGradeName = "";
            t1_FlawGradeName = "";
            t1_HardGradeName = "";
            t1_DensityGradeName = "";
            t1_SugarDegreeGradeName = "";
            t1_ProgramName = "";
            t2_GradeID = new List<string>();
            t2_BoxNumber = new List<string>();
            t2_FruitNumber = new List<string>();
            t2_FruitWeight = new List<string>();
            t2_QualityName = new List<string>();
            t2_WeightOrSizeName = new List<string>();
            t2_WeightOrSizeLimit = new List<string>();
            t2_SelectWeightOrSize = new List<string>();
            t2_TraitWeightOrSize = new List<string>();
            t2_TraitColor = new List<string>();
            t2_TraitShape = new List<string>();
            t2_TraitFlaw = new List<string>();
            t2_TraitHard = new List<string>();
            t2_TraitDensity = new List<string>();
            t2_TraitSugarDegree = new List<string>();
            t3_ExportID = new List<string>();
            t3_FruitNumber = new List<string>();
            t3_FruitWeight = new List<string>();
        }
    }

    public class DeviceInfo
    {
        public string DeviceNumber { get; set; } //设备编号
        public string MacAddress { get; set; }   //MAC地址
        public string FactoryTime { get; set; }  //出厂时间
        public string Country { get; set; }      //国家
        public string Area { get; set; }         //地区
        public string DetailAddress { get; set; }//详细地址
        public string Contactor { get; set; }    //联系人
        public string PhoneNumber { get; set; }  //联系电话
        public string TVAccount { get; set; }    //TV账号
        public string TVPassword { get; set; }   //TV密码
        public string RegistrationCode { get; set; } //注册码
        public DeviceInfo()
        {
            DeviceNumber = "";
            MacAddress = "";
            FactoryTime = "";
            Country = "";
            Area = "";
            DetailAddress = "";
            Contactor = "";
            PhoneNumber = "";
            TVAccount = "";
            TVPassword = "";
            RegistrationCode = "";
        }
    }
}
