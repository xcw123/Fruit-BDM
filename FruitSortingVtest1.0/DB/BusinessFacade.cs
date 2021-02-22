using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FruitSortingVtest1.DB
{
    public class BusinessFacade
    {
        ////根据sql语句获取相应的数据库数据表
        //public static DataSet GetTable(string sql)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.GetTable(sql);
        //}

        ////插入水果信息
        //public static Boolean InsertFruitInfo(string customerName, string farmName, string fruitName, string startTime, string startedState, string completedState)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.InsertFruitInfo(customerName,farmName,fruitName,startTime,startedState,completedState);
        //}

        ////插入等级信息
        //public static Boolean InsertGradeInfo(int customerID, int gradeID, int boxNumber, int fruitNumber, int fruitWeight,
        //    string qualityName, string weightOrSizeName, int weightOrSizeLimit, string selectWeightOrSize, string traitWeightOrSize,
        //    string traitColor, string traitShape, string traitFlaw, string traitHard, string traitDensity, string traitSugarDegree)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.InsertGradeInfo(customerID,gradeID,boxNumber,fruitNumber,fruitWeight,qualityName,weightOrSizeName,weightOrSizeLimit,
        //        selectWeightOrSize,traitWeightOrSize,traitColor,traitShape,traitFlaw,traitHard,traitDensity,traitSugarDegree);
        //}

        ////插入出口信息
        //public static Boolean InsertExportInfo(int customerID, int exportID, int fruitNumber, int fruitWeight)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.InsertExportInfo(customerID, exportID, fruitNumber, fruitWeight);
        //}

        ////更新水果客户信息
        //public static Boolean UpdateFruitCustomerInfo(int customerID, string customerName, string farmName, string fruitName)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.UpdateFruitCustomerInfo(customerID, customerName, farmName, fruitName);
        //}

        ////更新水果开始状态
        //public static Boolean UpdateFruitStartedState(int customerID, string startedState)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.UpdateFruitStartedState(customerID, startedState);
        //}

        ////更新水果的开始时间
        //public static Boolean UpdateFruitStartTime(int customerID, string startTime)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.UpdateFruitStartTime(customerID, startTime);
        //}

        ////更新水果完成状态
        //public static Boolean UpdateFruitCompletedState(int customerID, string endTime, string completedState, int batchWeight,
        //    int batchNumber, int qualityGradeSum, int weightOrSizeGradeSum, int exportSum,string colorGradeName,
        //    string shapeGradeName,string flawGradeName,string hardGradeName,string densityGradeName,string sugarDegreeGradeName)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.UpdateFruitCompletedState(customerID, endTime, completedState, batchWeight, batchNumber, qualityGradeSum,
        //        weightOrSizeGradeSum, exportSum,colorGradeName,shapeGradeName,flawGradeName,hardGradeName,densityGradeName,sugarDegreeGradeName);
        //}

        ////查找所有水果信息（降序排列）
        //public static DataSet GetFruitAll()
        //{
        //    DataAccess da = new DataAccess();
        //    return da.GetFruitAll();
        //}

        ////根据客户编号查找水果信息
        //public static DataSet GetFruitByCustomerID(int customerID)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.GetFruitByCustomerID(customerID);
        //}

        ////根据客户编号查找等级信息
        //public static DataSet GetGradeByCustomerID(int customerID)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.GetGradeByCustomerID(customerID);
        //}

        ////根据客户编号查找出口信息
        //public static DataSet GetExportByCustomerID(int customerID)
        //{
        //    DataAccess da = new DataAccess();
        //    return da.GetExportByCustomerID(customerID);
        //}

        ////查找最新插入的水果信息的编号
        //public static DataSet GetFruitTopCustomerID()
        //{
        //    DataAccess da = new DataAccess();
        //    return da.GetFruitTopCustomerID();
        //}
    }
}
