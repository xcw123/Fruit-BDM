using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Interface;

namespace FruitSortingVtest1.DB
{
    public class DataAccess
    {
        public string strConn;   //连接字符串

        public DataAccess()
        {
            //strConn = ConfigurationManager.ConnectionStrings["dbConn"].ToString();
            strConn = GlobalDataInterface.dataBaseConn;
        }

        /// <summary>
        /// 根据sql语句获取相应的数据库数据表
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>查询结果</returns>
        public DataSet GetTable(string sql)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            da.Fill(dst);
            return dst;
        }

        /// <summary>
        /// 插入水果信息
        /// </summary>
        /// <param name="customerName">客户名称</param>
        /// <param name="farmName">农场名称</param>
        /// <param name="fruitName">水果品种</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="startedState">开始状态（0：未开始，1：开始）</param>
        /// <param name="completedState">完成状态（0：未完成，1：完成）</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertFruitInfo(string customerName, string farmName, string fruitName, string startTime, string startedState, string completedState)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("InsertFruitInfo", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerName", SqlDbType.VarChar, 50);
            comm.Parameters["@CustomerName"].Value = customerName;
            comm.Parameters.Add("@FarmName", SqlDbType.VarChar, 50);
            comm.Parameters["@FarmName"].Value = farmName;
            comm.Parameters.Add("@FruitName", SqlDbType.VarChar, 50);
            comm.Parameters["@FruitName"].Value = fruitName;
            comm.Parameters.Add("@StartTime", SqlDbType.VarChar, 50);
            comm.Parameters["@StartTime"].Value = startTime;
            comm.Parameters.Add("@StartedState", SqlDbType.VarChar, 1);
            comm.Parameters["@StartedState"].Value = startedState;
            comm.Parameters.Add("@CompletedState", SqlDbType.VarChar, 1);
            comm.Parameters["@CompletedState"].Value = completedState;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入等级信息
        /// </summary>
        /// <param name="customerID">客户编号（外键）</param>
        /// <param name="gradeID">等级编号</param>
        /// <param name="boxNumber">水果箱数</param>
        /// <param name="fruitNumber">水果个数</param>
        /// <param name="fruitWeight">水果重量</param>
        /// <param name="qualityName">品质名称</param>
        /// <param name="weightOrSizeName">重量/尺寸名称</param>
        /// <param name="weightOrSizeLimit">重量/尺寸限制（即最大值）</param>
        /// <param name="selectWeightOrSize">重量/尺寸选择（0：重量，1：尺寸）</param>
        /// <param name="traitWeightOrSize">重量/尺寸特征</param>
        /// <param name="traitColor">颜色特征</param>
        /// <param name="traitShape">形状特征</param>
        /// <param name="traitFlaw">瑕疵特征</param>
        /// <param name="traitHard">硬度特征</param>
        /// <param name="traitDensity">密度特征</param>
        /// <param name="traitSugarDegree">含糖量特征</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertGradeInfo(int customerID, int gradeID, int boxNumber, int fruitNumber, int fruitWeight,
            string qualityName, string weightOrSizeName, int weightOrSizeLimit, string selectWeightOrSize, string traitWeightOrSize,
            string traitColor, string traitShape, string traitFlaw, string traitHard, string traitDensity, string traitSugarDegree)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("InsertGradeInfo", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            comm.Parameters.Add("@GradeID", SqlDbType.Int);
            comm.Parameters["@GradeID"].Value = gradeID;
            comm.Parameters.Add("@BoxNumber", SqlDbType.Int);
            comm.Parameters["@BoxNumber"].Value = boxNumber;
            comm.Parameters.Add("@FruitNumber", SqlDbType.Int);
            comm.Parameters["@FruitNumber"].Value = fruitNumber;
            comm.Parameters.Add("@FruitWeight", SqlDbType.Int);
            comm.Parameters["@FruitWeight"].Value = fruitWeight;
            comm.Parameters.Add("@QualityName", SqlDbType.VarChar, 50);
            comm.Parameters["@QualityName"].Value = qualityName;
            comm.Parameters.Add("@WeightOrSizeName", SqlDbType.VarChar, 50);
            comm.Parameters["@WeightOrSizeName"].Value = weightOrSizeName;
            comm.Parameters.Add("@WeightOrSizeLimit", SqlDbType.Int);
            comm.Parameters["@WeightOrSizeLimit"].Value = weightOrSizeLimit;
            comm.Parameters.Add("@SelectWeightOrSize", SqlDbType.VarChar, 1);
            comm.Parameters["@SelectWeightOrSize"].Value = selectWeightOrSize;
            comm.Parameters.Add("@TraitWeightOrSize", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitWeightOrSize"].Value = traitWeightOrSize;
            comm.Parameters.Add("@TraitColor", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitColor"].Value = traitColor;
            comm.Parameters.Add("@TraitShape", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitShape"].Value = traitShape;
            comm.Parameters.Add("@TraitFlaw", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitFlaw"].Value = traitFlaw;
            comm.Parameters.Add("@TraitHard", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitHard"].Value = traitHard;
            comm.Parameters.Add("@TraitDensity", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitDensity"].Value = traitDensity;
            comm.Parameters.Add("@TraitSugarDegree", SqlDbType.VarChar, 50);
            comm.Parameters["@TraitSugarDegree"].Value = traitSugarDegree;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入出口信息
        /// </summary>
        /// <param name="customerID">客户编号（外键）</param>
        /// <param name="exportID">出口编号</param>
        /// <param name="fruitNumber">水果编号</param>
        /// <param name="fruitWeight">水果重量</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertExportInfo(int customerID, int exportID, int fruitNumber, int fruitWeight)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("InsertExportInfo", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            comm.Parameters.Add("@ExportID", SqlDbType.Int);
            comm.Parameters["@ExportID"].Value = exportID;
            comm.Parameters.Add("@FruitNumber",SqlDbType.Int);
            comm.Parameters["@FruitNumber"].Value = fruitNumber;
            comm.Parameters.Add("@FruitWeight", SqlDbType.Int);
            comm.Parameters["@FruitWeight"].Value = fruitNumber;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新水果客户信息
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="customerName">客户名称</param>
        /// <param name="farmName">农场名称</param>
        /// <param name="fruitName">水果品种</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitCustomerInfo(int customerID, string customerName, string farmName, string fruitName)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("UpdateFruitCustomerInfo", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            comm.Parameters.Add("@CustomerName", SqlDbType.VarChar, 50);
            comm.Parameters["@CustomerName"].Value = customerName;
            comm.Parameters.Add("@FarmName", SqlDbType.VarChar, 50);
            comm.Parameters["@FarmName"].Value = farmName;
            comm.Parameters.Add("@FruitName", SqlDbType.VarChar, 50);
            comm.Parameters["@FruitName"].Value = fruitName;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新水果开始状态
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="startedState">开始状态</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitStartedState(int customerID, string startedState)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("UpdateFruitStartedState", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            comm.Parameters.Add("@StartedState", SqlDbType.VarChar, 1);
            comm.Parameters["@StartedState"].Value = startedState;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新水果的开始时间
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="startTime">开始时间</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitStartTime(int customerID, string startTime)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("UpdateFruitStartTime", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            comm.Parameters.Add("@StartTime", SqlDbType.VarChar, 50);
            comm.Parameters["@StartTime"].Value = startTime;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新水果完成状态
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="completedState">完成状态</param>
        /// <param name="batchWeight">批重量</param>
        /// <param name="batchNumber">批个数</param>
        /// <param name="qualityGradeSum">品质等级个数</param>
        /// <param name="weightOrSizeGradeSum">重量/尺寸等级个数</param>
        /// <param name="exportSum">出口个数</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitCompletedState(int customerID, string endTime, string completedState, int batchWeight,
            int batchNumber, int qualityGradeSum, int weightOrSizeGradeSum, int exportSum,string colorGradeName,
            string shapeGradeName,string flawGradeName,string hardGradeName,string densityGradeName,string sugarDegreeGradeName)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("UpdateFruitCompletedState", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            comm.Parameters.Add("@EndTime", SqlDbType.VarChar, 50);
            comm.Parameters["@EndTime"].Value = endTime;
            comm.Parameters.Add("@CompletedState", SqlDbType.VarChar, 1);
            comm.Parameters["@CompletedState"].Value = completedState;
            comm.Parameters.Add("@BatchWeight", SqlDbType.Int);
            comm.Parameters["@BatchWeight"].Value = batchWeight;
            comm.Parameters.Add("@BatchNumber", SqlDbType.Int);
            comm.Parameters["@BatchNumber"].Value = batchNumber;
            comm.Parameters.Add("@QualityGradeSum", SqlDbType.Int);
            comm.Parameters["@QualityGradeSum"].Value = qualityGradeSum;
            comm.Parameters.Add("@WeightOrSizeGradeSum", SqlDbType.Int);
            comm.Parameters["@WeightOrSizeGradeSum"].Value = weightOrSizeGradeSum;
            comm.Parameters.Add("@ExportSum", SqlDbType.Int);
            comm.Parameters["@ExportSum"].Value = exportSum;
            comm.Parameters.Add("@ColorGradeName", SqlDbType.VarChar, 100);
            comm.Parameters["@ColorGradeName"].Value = colorGradeName;
            comm.Parameters.Add("@ShapeGradeName", SqlDbType.VarChar, 100);
            comm.Parameters["@ShapeGradeName"].Value = shapeGradeName;
            comm.Parameters.Add("@FlawGradeName", SqlDbType.VarChar, 100);
            comm.Parameters["@FlawGradeName"].Value = flawGradeName;
            comm.Parameters.Add("@HardGradeName", SqlDbType.VarChar, 100);
            comm.Parameters["@HardGradeName"].Value = hardGradeName;
            comm.Parameters.Add("@DensityGradeName", SqlDbType.VarChar, 100);
            comm.Parameters["@DensityGradeName"].Value = densityGradeName;
            comm.Parameters.Add("@SugarDegreeGradeName", SqlDbType.VarChar, 100);
            comm.Parameters["@SugarDegreeGradeName"].Value = sugarDegreeGradeName;
            conn.Open();
            int i = comm.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查找所有水果信息（降序排列）
        /// </summary>
        /// <returns>所有水果信息</returns>
        public DataSet GetFruitAll()
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("GetFruitAll", conn);
            comm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            da.Fill(dst);
            return dst;
        }

        /// <summary>
        /// 根据客户编号查找水果信息
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns>相应的水果信息</returns>
        public DataSet GetFruitByCustomerID(int customerID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("GetFruitByCustomerID", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            { 
            }
            da.Fill(dst);
            return dst;
        }

        /// <summary>
        /// 根据客户编号查找等级信息
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns>相应的等级信息</returns>
        public DataSet GetGradeByCustomerID(int customerID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("GetGradeByCustomerID", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            da.Fill(dst);
            return dst;
        }

        /// <summary>
        /// 根据客户编号查找出口信息
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns>相应的出口信息</returns>
        public DataSet GetExportByCustomerID(int customerID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("GetExportByCustomerID", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@CustomerID", SqlDbType.Int);
            comm.Parameters["@CustomerID"].Value = customerID;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            da.Fill(dst);
            return dst;
        }

        /// <summary>
        /// 查找最新插入的水果信息的编号
        /// </summary>
        /// <returns>水果信息编号</returns>
        public DataSet GetFruitTopCustomerID()
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand("GetFruitTopCustomerID", conn);
            comm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            da.Fill(dst);
            return dst;
        }
    }
}
