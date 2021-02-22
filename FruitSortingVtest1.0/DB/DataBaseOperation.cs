using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interface;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace FruitSortingVtest1.DB
{
    public class DataBaseOperation
    {
        #region 变量声明
        public string strConn;  //连接字符串
        SqlLog sqllog = new SqlLog(); //Add by ChengSk - 20181128
        #endregion


        #region 创建数据库连接字符串（无参的）
        /// <summary>
        /// 创建数据库连接字符串（无参的）
        /// </summary>
        public DataBaseOperation()
        {
            strConn = GlobalDataInterface.dataBaseConn;
        }
        #endregion

        #region 创建数据库连接字符串（有参的）
        /// <summary>
        /// 创建数据库连接字符串（有参的）
        /// </summary>
        /// <param name="strconn"></param>
        public DataBaseOperation(string strconn)
        {
            strConn = strconn;
        }
        #endregion


        #region 数据库插入操作                           通用函数
        /// <summary>
        /// 数据库插入操作
        /// </summary>
        /// <param name="strSQL">SQL插入语句</param>
        /// <returns>插入是否成功</returns>
        public Boolean InsertData(string strSQL)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.CommandType = CommandType.Text;
            try
            {
                conn.Open();
                int i = comm.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                if (i > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("InsertData Error: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("InsertData Error: " + ex.ToString());
                GlobalDataInterface.WriteErrorInfo("InsertData Error SQL: " + strSQL); //打印执行错误的SQL语句 Add by ChengSk - 20180919
#endif
            }  
            return false;
        }
        #endregion

        #region 数据库删除操作                           通用函数
        /// <summary>
        /// 数据库删除操作
        /// </summary>
        /// <param name="strSQL">SQL删除语句</param>
        /// <returns>删除是否成功</returns>
        public Boolean DeleteData(string strSQL)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.CommandType = CommandType.Text;
            try
            {
                conn.Open();
                int i = comm.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                if (i > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("DeleteData Error: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("DeleteData Error: " + ex.ToString());
                GlobalDataInterface.WriteErrorInfo("DeleteData Error SQL: " + strSQL); //打印执行错误的SQL语句 Add by ChengSk - 20180919
#endif
            }   
            return false;
        }
        #endregion

        #region 数据库修改操作                           通用函数
        /// <summary>
        /// 数据库修改操作
        /// </summary>
        /// <param name="strSQL">SQL修改语句</param>
        /// <returns>修改是否成功</returns>
        public Boolean UpdateData(string strSQL)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.CommandType = CommandType.Text;
            try
            {
                conn.Open();
                int i = comm.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                if (i > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("UpdateData Error: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("UpdateData Error: " + ex.ToString());
                GlobalDataInterface.WriteErrorInfo("UpdateData Error SQL: " + strSQL); //打印执行错误的SQL语句 Add by ChengSk - 20180919
#endif
            }
            return false;
        }
        #endregion

        #region 数据库查询操作（返回结果数据集）         通用函数
        /// <summary>
        /// 数据库查询操作（返回结果数据集）
        /// </summary>
        /// <param name="strSQL">SQL查询语句</param>
        /// <returns>查询结果数据集</returns>
        public DataSet SelectData(string strSQL)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet dst = new DataSet();
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
                da.Fill(dst);
                conn.Close();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SelectData Error: " + ex.ToString());
#if REALEASE
                GlobalDataInterface.WriteErrorInfo("SelectData Error:" + ex.ToString());
                GlobalDataInterface.WriteErrorInfo("SelectData Error SQL:" + strSQL); //打印执行错误的SQL语句 Add by ChengSk - 20180919
#endif
            }
            
            return dst;
        }
        #endregion


        #region 插入水果信息                                         专项函数
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
            string strSQL = "insert into tb_FruitInfo(CustomerName,FarmName,FruitName,StartTime,StartedState,CompletedState) values ('" +
                customerName + "','" + farmName + "','" + fruitName + "','" + startTime + "','" + startedState + "','" + completedState + "')";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return InsertData(strSQL);
        }
        #endregion

        #region 插入等级信息                                         专项函数
        /// <summary>
        /// 插入等级信息
        /// </summary>
        /// <param name="customerID">客户编号（外键）</param>
        /// <param name="gradeID">等级编号（外键）</param>
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
            string qualityName, string weightOrSizeName, float weightOrSizeLimit, string selectWeightOrSize, string traitWeightOrSize,
            string traitColor, string traitShape, string traitFlaw, string traitHard, string traitDensity, string traitSugarDegree)
        {
            boxNumber = (boxNumber < 0) ? 0 : boxNumber;       //Add by ChengSk - 20181127
            fruitNumber = (fruitNumber < 0) ? 0 : fruitNumber; //Add by ChengSk - 20181127
            fruitWeight = (fruitWeight < 0) ? 0 : fruitWeight; //Add by ChengSk - 20181127
            string strSQL = "insert into tb_GradeInfo(CustomerID,GradeID,BoxNumber,FruitNumber,FruitWeight,QualityName,WeightOrSizeName," +
                    "WeightOrSizeLimit,SelectWeightOrSize,TraitWeightOrSize,TraitColor,TraitShape,TraitFlaw,TraitHard,TraitDensity,TraitSugarDegree) values('" +
                    customerID + "','" + gradeID + "','" + boxNumber + "','" + fruitNumber + "','" + fruitWeight + "','" + qualityName + "','" + weightOrSizeName + "','" +
                    weightOrSizeLimit + "','" + selectWeightOrSize + "','" + traitWeightOrSize + "','" + traitColor + "','" + traitShape + "','" + traitFlaw + "','" +
                    traitHard + "','" + traitDensity + "','" + traitSugarDegree + "')";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return InsertData(strSQL); 
        }
        #endregion

        #region 插入出口信息                                         专项函数
        /// <summary>
        /// 插入出口信息
        /// </summary>
        /// <param name="customerID">客户编号（外键）</param>
        /// <param name="exportID">出口编号（外键）</param>
        /// <param name="fruitNumber">水果编号</param>
        /// <param name="fruitWeight">水果重量</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertExportInfo(int customerID, int exportID, int fruitNumber, int fruitWeight)
        {
            fruitNumber = (fruitNumber < 0) ? 0 : fruitNumber; //Add by ChengSk - 20181127
            fruitWeight = (fruitWeight < 0) ? 0 : fruitWeight; //Add by ChengSk - 20181127
            string strSQL = "insert into tb_ExportInfo(CustomerID,ExportID,FruitNumber,FruitWeight) values('" +
                customerID + "','" + exportID + "','" + fruitNumber + "','" + fruitWeight + "')";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return InsertData(strSQL);
        }
        #endregion

        #region 插入运行时间信息                                     专项函数
        /// <summary>
        /// 插入运行时间信息
        /// </summary>
        /// <param name="runningDate">运行日期（2018-01-29）</param>
        /// <param name="startTime">开始时间（08:25:40）</param>
        /// <param name="stopTime">结束时间（10:32:40）</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertRunningTimeInfo(string runningDate, string startTime, string stopTime)
        {
            string strSQL = "insert into tb_RunningTimeInfo(RunningDate, StartTime, StopTime) Values ('" +
                runningDate + "','" + startTime + "','" + stopTime + "')";
            return InsertData(strSQL);
        }
        #endregion

        #region 插入分选效率信息                                     专项函数
        /// <summary>
        /// 插入分选效率信息
        /// </summary>
        /// <param name="efficiencyValue">分选效率</param>
        /// <param name="currentDate">分选日期</param>
        /// <param name="currentTime">分选时间</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertSeparationEfficiencyInfo(string efficiencyValue, string currentDate, string currentTime)
        {
            string strSQL = "insert into tb_SeparationEfficiencyInfo(EfficiencyValue, CurrentDate, CurrentTime) Values ('" +
                efficiencyValue + "','" + currentDate + "','" + currentTime + "')";
            return InsertData(strSQL);
        }
        #endregion

        #region 修改水果信息（客户信息）                             专项函数
        /// <summary>
        /// 修改水果信息（客户信息）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="customerName">客户名称</param>
        /// <param name="farmName">农场名称</param>
        /// <param name="fruitName">水果品种</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitCustomerInfo(int customerID, string customerName, string farmName, string fruitName)
        {
            string strSQL = "update tb_FruitInfo set CustomerName='" + customerName + "',FarmName='" + farmName + "',FruitName='" +
                fruitName + "' where CustomerID='" + customerID.ToString() + "'";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return UpdateData(strSQL);
        }
        #endregion

        #region 修改水果信息（开始状态）                             专项函数
        /// <summary>
        /// 修改水果信息（开始状态）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="startedState">开始状态</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitStartedState(int customerID, string startedState)
        {
            string strSQL = "update tb_FruitInfo set StartedState='" + startedState + "' where CustomerID='" + customerID.ToString() + "'";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return UpdateData(strSQL);
        }
        #endregion
         
        #region 修改水果信息（开始时间）                             专项函数
        /// <summary>
        /// 修改水果信息（开始时间）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="startTime">开始时间</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateFruitStartTime(int customerID, string startTime)
        {
            string strSQL = "update tb_FruitInfo set StartTime='" + startTime + "' where CustomerID='" + customerID.ToString() + "'";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return UpdateData(strSQL);
        }
        #endregion

        #region 修改水果信息（完成状态）                             专项函数
        /// <summary>
        /// 修改水果信息（完成状态）
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
            int batchNumber, int qualityGradeSum, int weightOrSizeGradeSum, int exportSum, string colorGradeName,
            string shapeGradeName, string flawGradeName, string hardGradeName, string densityGradeName, string sugarDegreeGradeName,string programName)
        {
            string strSQL = "update tb_FruitInfo set EndTime='" + endTime + "',CompletedState='" + completedState + "',BatchWeight='" + batchWeight +
                "',BatchNumber='" + batchNumber + "',QualityGradeSum='" + qualityGradeSum + "',WeightOrSizeGradeSum='" + weightOrSizeGradeSum +
                "',ExportSum='" + exportSum + "',ColorGradeName='" + colorGradeName + "',ShapeGradeName='" + shapeGradeName + "',FlawGradeName='" +
                flawGradeName + "',HardGradeName='" + hardGradeName + "',DensityGradeName='" + densityGradeName + "',SugarDegreeGradeName='" +
                sugarDegreeGradeName + "',ProgramName='" + programName + "' where CustomerID='" + customerID.ToString() + "'";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return UpdateData(strSQL);
        }
        #endregion

        #region 修改等级信息                                         专项函数
        /// <summary>
        /// 修改等级信息
        /// </summary>
        /// <param name="customerID">客户编号（外键）</param>
        /// <param name="gradeID">等级编号（外键）</param>
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
        /// <returns>是否更新成功</returns>
        public Boolean UpdateGradeInfo(int customerID, int gradeID, int boxNumber, int fruitNumber, int fruitWeight,
            string qualityName, string weightOrSizeName, float weightOrSizeLimit, string selectWeightOrSize, string traitWeightOrSize,
            string traitColor, string traitShape, string traitFlaw, string traitHard, string traitDensity, string traitSugarDegree) //New Add
        {
            string strSQL = "update tb_GradeInfo set BoxNumber='" + boxNumber + "',FruitNumber='" + fruitNumber + "',FruitWeight='" + fruitWeight +
                "',QualityName='" + qualityName + "',WeightOrSizeName='" + weightOrSizeName + "',WeightOrSizeLimit='" + weightOrSizeLimit + "',SelectWeightOrSize='" + selectWeightOrSize +
                "',TraitWeightOrSize='" + traitWeightOrSize + "',TraitColor='" + traitColor + "',TraitShape='" + traitShape + "',TraitFlaw='" + traitFlaw + "',TraitHard='" + traitHard +
                "',TraitDensity='" + traitDensity + "',TraitSugarDegree='" + traitSugarDegree + "' where CustomerID='" + customerID.ToString() + "' and GradeID='" + gradeID.ToString() + "'";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return UpdateData(strSQL);
        }
        #endregion

        #region 修改出口信息                                         专项函数
        /// <summary>
        /// 修改出口信息
        /// </summary>
        /// <param name="customerID">客户编号（外键）</param>
        /// <param name="exportID">出口编号（外键）</param>
        /// <param name="fruitNumber">水果编号</param>
        /// <param name="fruitWeight">水果重量</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateExportInfo(int customerID, int exportID, int fruitNumber, int fruitWeight) //New Add
        {
            string strSQL = "update tb_ExportInfo set FruitNumber='" + fruitNumber + "',FruitWeight='" + fruitWeight + "' where CustomerID='" + customerID + "' and ExportID='" + exportID + "'";
            sqllog.WriteSqlLog(strSQL); //Add by ChengSk - 20181128
            return UpdateData(strSQL);
        }
        #endregion

        #region 修改运行时间信息（结束时间）                         专项函数
        /// <summary>
        /// 修改运行结束时间
        /// </summary>
        /// <param name="id">ID号</param>
        /// <param name="stopTime">结束时间</param>
        /// <returns>是否更新成功</returns>
        public Boolean UpdateRunningStopTime(int id, string stopTime)
        {
            string strSQL = "update tb_RunningTimeInfo set StopTime = '" + stopTime + "' where ID = '" + id.ToString() + "'";
            return UpdateData(strSQL);
        }
        #endregion

        #region 查询水果信息（查询所有，降序排列，返回查询结果集）   专项函数
        /// <summary>
        /// 查询水果信息（查询所有，降序排列，返回查询结果集）
        /// </summary>
        /// <returns>所有水果信息</returns>
        public DataSet GetFruitAll()
        {
            string strSQL = "select * from tb_FruitInfo order by CustomerID desc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询水果信息（条件查询：客户编号，返回查询结果集）   专项函数
        /// <summary>
        /// 查询水果信息（条件查询：客户编号，返回查询结果集）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns>相应的水果信息</returns>
        public DataSet GetFruitByCustomerID(int customerID)
        {
            string strSQL = "select * from tb_FruitInfo where CustomerID='" + customerID.ToString() + "'";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询水果信息（限定查询：最新数据，返回查询结果集）   专项函数
        /// <summary>
        /// 查询水果信息（限定查询：最新数据，返回查询结果集）
        /// </summary>
        /// <returns>水果信息编号</returns>
        public DataSet GetFruitTopCustomerID()
        {
            string strSQL = "select top 1 CustomerID from tb_FruitInfo order by CustomerID desc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询等级信息（条件查询：客户编号，返回查询结果集）   专项函数
        /// <summary>
        /// 查询等级信息（条件查询：客户编号，返回查询结果集）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns>相应的等级信息</returns>
        public DataSet GetGradeByCustomerID(int customerID)
        {
            string strSQL = "select * from tb_GradeInfo where CustomerID='" + customerID.ToString() + "' order by GradeID asc";
            return SelectData(strSQL);
        }
        #endregion
        
        #region 查询等级信息（条件查询：客户编号&等级编号，返回查询结果集）  专项函数
        /// <summary>
        /// 查询等级信息（条件查询：客户编号&等级编号，返回查询结果集）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="gradeID">等级编号</param>
        /// <returns>相应的等级信息</returns>
        public DataSet GetGradeByCustomerIDAndGradeID(int customerID, int gradeID) //New Add
        {
            string strSQL = "select * from tb_GradeInfo where CustomerID='" + customerID.ToString() + "' and GradeID = '" + gradeID.ToString() + "' order by GradeID asc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询出口信息（条件查询：客户编号，返回查询结果集）   专项函数
        /// <summary>
        /// 查询出口信息（条件查询：客户编号，返回查询结果集）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns>相应的出口信息</returns>
        public DataSet GetExportByCustomerID(int customerID)
        {
            string strSQL = "select * from tb_ExportInfo where CustomerID='" + customerID.ToString() + "' order by ExportID asc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询出口信息（条件查询：客户编号&出口编号，返回查询结果集）  专项函数
        /// <summary>
        /// 查询出口信息（条件查询：客户编号&出口编号，返回查询结果集）
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="exportID">出口编号</param>
        /// <returns>相应的出口信息</returns>
        public DataSet GetExportByCustomerIDAndExportID(int customerID, int exportID) //New Add
        {
            string strSQL = "select * from tb_ExportInfo where CustomerID='" + customerID.ToString() + "' and ExportID = '" + exportID.ToString() + "' order by ExportID asc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询运行时间信息（条件查询：运行日期，返回查询结果集） 专项函数
        /// <summary>
        /// 查询运行时间信息（条件查询：运行日期，返回查询结果集）
        /// </summary>
        /// <param name="runningDate">运行日期</param>
        /// <returns>相应的运行时间信息</returns>
        public DataSet GetRunningTimeInfo(string runningDate)
        {
            string strSQL = "select * from tb_RunningTimeInfo where RunningDate ='" + runningDate + "' order by ID asc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询运行时间信息（条件查询：结束时间，返回查询结果集） 专项函数
        /// <summary>
        /// 查询运行时间信息（条件查询：结束时间，返回查询结果集）
        /// </summary>
        /// <param name="stopTime">结束时间</param>
        /// <returns>相应的运行时间信息</returns>
        public DataSet GetRunningTimeInfoByStopTime(string stopTime)
        {
            string strSQL = "select * from tb_RunningTimeInfo where StopTime ='" + stopTime + "' order by ID asc";
            return SelectData(strSQL);

        }
        #endregion

        #region 查询分选效率信息（条件查询：分选日期，返回查询结果集） 专项函数
        /// <summary>
        /// 查询分选效率信息（条件查询：分选日期，返回查询结果）
        /// </summary>
        /// <param name="currentDate">分选日期</param>
        /// <returns>相应的分选效率信息</returns>
        public DataSet GetSeparationEfficiencyInfo(string currentDate)
        {
            string strSQL = "select * from tb_SeparationEfficiencyInfo where CurrentDate ='" + currentDate + "' order by ID asc";
            return SelectData(strSQL);
        }
        #endregion


        #region 查询水果信息（当前时间之后的第一条数据）               专项函数
        /// <summary>
        /// 查询水果信息（当前时间之后的第一条数据）
        /// </summary>
        /// <param name="endTime">结束加工时间</param>
        /// <returns></returns>
        public DataSet GetFruitTop1ByEndTime(string endTime)
        {
            string strSQL = "select top 1 * from tb_FruitInfo where EndTime > '" + endTime + "' and CompletedState = '1' order by EndTime asc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询水果信息（最新一条加工的数据）                     专项函数
        /// <summary>
        /// 查询水果信息（最新一条加工的数据）
        /// </summary>
        /// <returns></returns>
        public DataSet GetFruitTop1()
        {
            string strSQL = "select top 1 * from tb_FruitInfo where CompletedState = '1' order by EndTime desc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询水果信息（最新一条未完成的加工数据）               专项函数
        /// <summary>
        /// 查询水果信息（最新一条未完成的加工数据）
        /// </summary>
        /// <returns></returns>
        public DataSet GetFruitTop1NoEndProcessing()
        {
            string strSQL = "select top 1 * from tb_FruitInfo where CompletedState = '0' order by CustomerID desc";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询水果信息（查询条件：结束时间）                     专项函数
        /// <summary>
        /// 查询水果信息（查询条件：结束时间）
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataSet GetFruitByEndTime(string endTime)
        {
            string strSQL = "select * from tb_FruitInfo where CompletedState = '1' and EndTime = '" + endTime + "'";
            return SelectData(strSQL);
        }
        #endregion

        #region 查询水果信息（查询条件：未完成状态）                   专项函数
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet GetFruitByCompletedState()
        {
            string strSQL = "select top 1 * from tb_FruitInfo where CompletedState = '0' order by CustomerID desc";
            return SelectData(strSQL);
        }
        #endregion

        #region 插入水果信息（全部信息）                               专项函数
        /// <summary>
        /// 插入水果信息
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="farmName"></param>
        /// <param name="fruitName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startedState"></param>
        /// <param name="completedState"></param>
        /// <param name="batchWeight"></param>
        /// <param name="batchNumber"></param>
        /// <param name="qualityGradeSum"></param>
        /// <param name="weightOrSizeGradeNum"></param>
        /// <param name="exportNum"></param>
        /// <param name="colorGradeName"></param>
        /// <param name="shapeGradeName"></param>
        /// <param name="flawGradeName"></param>
        /// <param name="hardGradeName"></param>
        /// <param name="densityGradeName"></param>
        /// <param name="sugarDegreeGradeName"></param>
        /// <param name="programName"></param>
        /// <returns></returns>
        public Boolean InsertFruitInfo(string customerName, string farmName, string fruitName, string startTime,
            string endTime, string startedState, string completedState, int batchWeight, int batchNumber,
            int qualityGradeSum, int weightOrSizeGradeNum, int exportNum, string colorGradeName, string shapeGradeName,
            string flawGradeName, string hardGradeName, string densityGradeName, string sugarDegreeGradeName, string programName)
        {
            string strSQL = "insert into tb_FruitInfo(CustomerName,FarmName,FruitName,StartTime,EndTime," +
                "StartedState,CompletedState,BatchWeight,BatchNumber,QualityGradeSum,WeightOrSizeGradeSum,ExportSum," +
                "ColorGradeName,ShapeGradeName,FlawGradeName,HardGradeName,DensityGradeName,SugarDegreeGradeName,ProgramName) values(" +
                "'" + customerName + "','" + farmName + "','" + fruitName + "','" + startTime + "','" +
                endTime + "','" + startedState + "','" + completedState + "'," + batchWeight.ToString() + "," + batchNumber.ToString() + "," +
                qualityGradeSum.ToString() + "," + weightOrSizeGradeNum.ToString() + "," + exportNum.ToString() + ",'" + colorGradeName + "','" +
                shapeGradeName + "','" + flawGradeName + "','" + hardGradeName + "','" + densityGradeName + "','" + sugarDegreeGradeName + "','" + programName + "')";
            sqllog.WriteSqlLog(strSQL);
            return InsertData(strSQL);
        }
        #endregion


        #region 删除水果信息（条件删除：客户编号）
        public Boolean DeleteFruitInfo(int customerID)
        {
            string strSQL = "delete from tb_FruitInfo where CustomerID='" + customerID.ToString() + "'";
            sqllog.WriteSqlLog(strSQL);
            return DeleteData(strSQL);
        }
        #endregion
    }
}
