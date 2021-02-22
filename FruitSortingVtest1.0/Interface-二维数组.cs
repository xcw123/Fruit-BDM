using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

 
namespace Interface
{
   
    public class ConstPreDefine
    {
        //const int INVALID_CHAR_VALUE = 0x7f;
        //const int INVALID_SHORT_VALUE = 0x7f7f;
        //const int INVALID_INT_VALUE = 0x7f7f7f7f;

        //约定：
        //1、无效值  每个字节设置为0x7f
        //2、术语简称：
        //	 子系统		SUBSYS
        //	 路			ROUTE
        //	 通道		CHANNEL
        //	 水果分选机	FSM
        //	 图像处理机	IPM
        //	 称重机		WM
        //	 上位机		HC
        //	 下位机		LC
        //3、上位机和下位机通过命令进行通信，命令格式采用“IDsrc + IDdest + CommandType + CommandParameter”的形式。

        // id定义规则
        // id统一使用一个形如0xABCD的16进制整型数表示，B、C、D分别代表子系统、路和通道的标号，A未使用（设置为0）
        // 当只有B不为0时，表示第B个子系统的水果分选机，如0x0100指SUBSYS1的FSM
        // 当只有B和C不为0时，表示第B个子系统的第C个图像处理机，如0x0230指SUBSYS2的IPM3
        // 当BCD都不为0时，表示第B个子系统第C路的第D个称重机，如0x0122指SUBSYS1的ROUTE2的WM2
        // 作为特例，上位机id值取0x1000
        // 以上id定义规则可支持的SUBSYS/ROUTE/WM最大值均为15

        // ip定义规则
        // 下位机中的FSM和IPM需定义ip,ip采用“模板+id”的方式定义,模板取LC_IP_ADDR_TEMPLATE,id则取0xABCD的BC部分
        // 如SUBSYS2的IPM3的id为0x0230,则其ip为"192.168.0.35" (注：35 = 0x23)
        // 上位机ip取"192.168.0.0"，上位机开放两个端口，端口1接收图像数据，端口2接收其它数据

        // 版本号格式定义(version)
        // 版本号组成由：主版本号 + . + 修正版本号(两个位的数值均为大于等于0的整数)
        // 当项目在进行了局部修改或 bug 修正时，主版本号不变 , 修正版本号加 1
        // 当项目在进行了重大修改或局部修正累积较多 , 而导致项目整体发生全局变化时 , 主版本号加 1 ,修正版本号复位为1
        // 以V1.1为例，版本的编码为1*100+1 = 101

        // 4、ARM 管脚16位，格式：前3位代表ARM的ID号，后10位代表那个管脚，中间3位代表排号（共3排）


        // 上位机id
        public const int HC_ID = 0x1000;
        // 上位机ip地址
        public  const string HC_IP_ADDR = "192.168.0.15";
        // 上位机端口号1
        public  const int HC_PORT1_NUM = 1127;
        // 上位机端口号2
        public const int HC_PORT2_NUM = 1128;
        // 下位机ip地址模板
        public const string LC_IP_ADDR_TEMPLATE = "192.168.0.";
        // FSM端口号
        public const int FSM_PORT_NUM = 1279;
        // IPM端口号
        public const int IPM_PORT_NUM = 1289;
        // 最大子系统数
        public const int MAX_SUBSYS_NUM = 4;  //增加为4个子系统 by ymh 2011-02-21
        // 单个子系统最多IPM个数
        public const int MAX_IPM_NUM = 6;
        // 单个子系统支持最大通道数
        public const int MAX_CHANNEL_NUM = 12;
        // 单个IPM支持最大通道数
        public const int CHANNEL_NUM = 2;
        // 单个IPM最多带的相机个数
        public const int MAX_CAMERA_NUM = 4;
        // 单个相机果杯框路数
        public const int CUP_CHANNEL_NUM = 2;
        // 其它常量
        public const int MAX_COLOR_GRADE_NUM = 16;  //颜色最多等级数
        public const int MAX_QUALITY_GRADE_NUM = 16;  //品质最多等级数
        public const int MAX_SIZE_GRADE_NUM = 16;	//尺寸（重量）最多等级数
        public const int MAX_EXIT_NUM = 48;  //最多出口数
        public const int MAX_COLOR_INTERVAL_NUM = 3;   //颜色最多区间数（红，黄，绿）
        public const int MAX_CUP_NUM = 10;	//最多果杯个数


        ///////////////////////////////////////////2010.8.13 by lz，修改协议用于增加每一通道的贴标设备数量
        public const int MAX_LABEL_NUM = 4;
        public const int VERSION = 301;	///////////////////版本号***********by ymh 2011-04-14

        public const int CAPTURE_WIDTH = 1024;
        public const int CAPTURE_HEIGHT = 768;
        public const int MAX_TEXT_LENGTH = 40;


        public const int GRADE_QUALITY_ON = 1;
        public const int GRADE_WEIGHTGRAM_ON = 2;
        public const int GRADE_WEIGHTGE_ON = 4;
        public const int GRADE_DIAMETER_ON = 8;  //直径
        public const int GRADE_AREA_ON = 16; //面积
        public const int GRADE_VOLUME_ON = 32; //体积  by ymh 2011-04-09

        public const int MAX_SHAPE_GRADE_NUM = 2;  //形状
        public const int MAX_FlAWAREA_GRADE_NUM = 6;  //瑕疵**************************************************************	
        public const int MAX_DENSITY_GRADE_NUM	= 6;  //密度	
        public const int MAX_RIGIDITY_GRADE_NUM = 6;  //硬度
        public const int MAX_SUGAR_GRADE_NUM = 6;  //含糖量 ***************************-by ymh 2012-03-03

        public const int MAX_LEVEL = 20;               // 水果瑕疵检测时分层最多分20层
        public const int MAX_YUV_THRESH_NUM = 500;	   // 用于瑕疵检测时，瑕疵所属颜色区间的最大区间数 ********************
     
        
    }

    // 命令类型定义

    //HC-->FSM
    public enum HC_FSM_COMMAND_TYPE:int
    {

        HC_CMD_DISPLAY_OFF = 0x0000,			//断开，HC-->FSM
        HC_CMD_WAVE_FORM_ON,					//波形捕捉开，HC-->FSM
        HC_CMD_WAVE_FORM_OFF,					//波形捕捉关，HC-->FSM
        HC_CMD_DATA_TRACKING_ON,				//数据追踪开，HC-->FSM
        HC_CMD_DATA_TRACKING_OFF,				//数据追踪关，HC-->FSM
        HC_CMD_CLEAR_DATA,						//数据清零，HC-->FSM
        HC_CMD_BACK_LEARN,						//更新背景，HC-->FSM
        HC_CMD_SHUT_DOWN,						//网络关机，HC-->FSM,  现在用于上位机关机时是否向FSM保存用户配置
        HC_CMD_GRADEINFO_ON,					//获取水果实时分级信息，HC-->FSM
        HC_CMD_GRADEINFO_OFF,					//不获取水果实时分级信息，HC-->FSM
        HC_CMD_WEIGHTINFO_ON,					//获取重量统计信息(只要统计信息,不需要波形和追踪数据)，HC-->FSM
        HC_CMD_WEIGHTINFO_OFF,					//不获取重量统计信息
        HC_CMD_SIMULATEDPULSE_ON,				//内信号源开
        HC_CMD_SIMULATEDPULSE_OFF,				//内信号源关
        HC_CMD_PROJ_OPENED,						//打开工程配置
        HC_CMD_PROJ_CLOSED,						//关闭工程配置
        HC_CMD_WEIGHTRESET,						//称重重置
        HC_CMD_TEST_CUP_ON,						//果杯测试开 
        HC_CMD_TEST_CUP_OFF,					//果杯测试关 
        HC_CMD_TEST_NET,						//网络测试，每隔20S自动发送一次

        HC_CMD_DISPLAY_ON,          			//连接 int型变量描述版本号，HC-->FSM -----2010.08.25  增加一个参数（版本号）
        HC_CMD_SYS_CONFIG,						//系统配置	stSysConfig，HC-->FSM
        HC_CMD_GRADE_INFO,						//等级设置	stGradeInfo，HC-->FSM
        HC_CMD_EXIT_INFO,						//出口信息设置 stExitInfo，HC-->FSM
        HC_CMD_WEIGHT_INFO,						//重量信息设置 stWeightBaseInfo，HC-->FSM
        HC_CMD_PARAS_INFO,						//通道范围等参数设置 stParas，HC-->FSM
        HC_CMD_TEST_VOLVE,						//电磁阀测试 stVolveTest，HC-->FSM
        HC_CMD_RESET_AD, 						//AD归零 stResetAD
        HC_CMD_GLOBAL_EXIT_INFO,				//全局出口信息设置 stGlobalExitInfo, HC-->FSM
        HC_CMD_GLOBAL_WEIGHT_INFO,				//全局重量信息设置 stGlobalWeightBaseInfo, HC-->FSM

        HC_CMD_FlAWAREA_INFO,					//瑕疵参数设置	----------------------------------------by ymh 2012-03-03
        HC_CMD_CUPSTATERESET,					//主界面最下端用户果杯状态复位	------------------------by ymh 2013-01-03

        HC_CMD_USERSET = 0x0800,				//只发结构变化部份  速度慢可能是这些数据要发到到多个下位机,(串行发送)
    }

    //FSM-->HC
    public enum FSM_HC_COMMAND_TYPE:int
    {
        FSM_CMD_CONFIG = 0x1000,			    //配置信息	stGlobal, FSM-->HC
        FSM_CMD_STATISTICS,					    //统计信息	stStatistics, FSM-->HC
        FSM_CMD_GRADEINFO,					    //水果实时分级信息	stFruitGradeInfo, FSM-->HC
        FSM_CMD_WEIGHTINFO,					    //重量统计信息	stWeightResult, FSM-->HC
        FSM_CMD_WAVEINFO,						//波形数据 stWaveInfo,FSM-->HC
        FSM_CMD_VERSIONERROR				    //上位机版本与下位机版本不一致, fsmv,FSM-->HC
    }

    //HC-->IPM
    public enum HC_IPM_COMMAND_TYPE:int
    {
        HC_CMD_SINGLE_SAMPLE = 0x2000,			//单张图像采集，HC-->IPM，用于传输同一水果在不同果杯中的图片
        HC_CMD_CONTINUOUS_SAMPLE_ON,			//连续采集开，HC-->IPM   由原来无参命令修改为带参命令，命令结构：cCameraNum
        HC_CMD_CONTINUOUS_SAMPLE_OFF,			//连续采集关，HC-->IPM
        HC_CMD_SHOW_BLOB_ON, 					//显示blob开，HC-->IPM
        //	HC_CMD_SHOW_BLOB_OFF,					//显示blob关，HC-->IPM	
        HC_CMD_AUTOBALANCE_ON,                  //启动白平衡  此命令后跟随  stWhiteBalanceParam 结构    /*======revised by guo   2011 6 29 =========*/
        //	HC_CMD_AUTOBALANCE_OFF,	
        HC_CMD_LEVELFEATURE_INFO,				//给IPM传输有关瑕疵检测的颜色区间信息
        HC_CMD_SINGLE_SAMPLE_SPOT				//单张采集有瑕疵的图（颜色+瑕疵两张）
    }

    //IPM-->HC
    public enum IPM_HC_COMMAND_TYPE:int
    {
        IPM_CMD_IMAGE = 0x3000,  				//图像数据	stImageInfo, IPM-->HC
        //	IPM_CMD_AUTOBALANCE_MEAN,               //传送白平衡获得的均值                          2011 6 28
        IPM_CMD_AUTOBALANCE_COEFFICIENT,        //IPM向HC传输自动白平衡的R G B 均值和白平衡系数， 此命令后跟随stWhiteBalanceCoefficient结构   revised by guo 2011 6 29 
        IPM_CMD_IMAGE_SPOT = 0x3002
    }

    ////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////上位机-->下位机通信协议///////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    // 上位机通过命令来控制下位机的工作状态。
    
    // 系统配置信息,发送给每一个FSM  (HC_ID, FSM, HC_CMD_SYS_CONFIG, stSysConfig)
    [StructLayout(LayoutKind.Sequential)]
    public struct stSysConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM * 2*4)]
        public byte[,] exitstate; //出口布局 支持最多255个出口，4排 exitstate[4][ConstPreDefine.MAX_EXIT_NUM * 2]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM)]
        public byte[,] nChannelInfo;	//子系统通道的信息 1-有效 0-无效 byte[ConstPreDefine.MAX_SUBSYS_NUM, ConstPreDefine.MAX_CHANNEL_NUM]
        public int nSubsysNum;													//子系统数目
        public int nExitNum;													//出口数目
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public int[] nDataRegistration;//图像数据和重量数据配准int[ConstPreDefine.MAX_SUBSYS_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public int[] nImageSugar;	  // 图像数据和内部品质含糖量数据配准 int[ConstPreDefine.MAX_SUBSYS_NUM]
        public int width;														//分辨率  宽
        public int height;														//分辨率  高
        public byte nClassificationInfo;								//从低到高：最低位代表视觉，其次代表重量，再次之代表内部品质，为1时有效，如：0000101表示视觉+内部品质。
        public byte nSystemInfo;                                        //系统信息，低四位有效，从低到高依次代表：RM100C,RM100CIR,RM200C,RM200CIR 1,2,4,8

        public stSysConfig(bool IsOK)
        {
             exitstate = new byte[4,ConstPreDefine.MAX_EXIT_NUM * 2];
             nChannelInfo = new byte[ConstPreDefine.MAX_SUBSYS_NUM, ConstPreDefine.MAX_CHANNEL_NUM];
             nSubsysNum = 0;
             nExitNum = 0;
             nDataRegistration = new int[ConstPreDefine.MAX_SUBSYS_NUM];
             nImageSugar = new int[ConstPreDefine.MAX_SUBSYS_NUM];
             width = 0;
             height = 0;
             nClassificationInfo = 0;
             nSystemInfo = 0;
        }
    }

    // 等级设置信息,发送给每一个FSM (HC_ID, FSM, HC_CMD_GRADE_INFO, stGradeInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stColorIntervalItem
    {
	    public byte	nMinU;			//最小U分量
	    public byte nMaxU;			//最大U分量
	    public byte nMinV;			//最小V分量
	    public byte nMaxV;			//最大V分量

        public stColorIntervalItem(bool IsOK)
        {
            nMinU = 0;
            nMaxU = 0;
            nMinV = 0;
            nMaxV = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stPercentInfo 
    {
	    public byte	nMax;				//最大比例
	    public byte	nMin;				//最小比例

        public stPercentInfo(bool IsOK)
        {
            nMax = 0;
            nMin = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stGradeItemInfo
    {
	    public byte	bShapeSize;                       //描述品质中的形状，1表示大于fShapeFactor，0表示小于，0x7F表示该品质未用形状信息进行分类--new
        public short nColorGrade;                              //颜色等级，0x7F表示该品质不用颜色信息进行分类--new

	    public byte	bDensity;							//描述品质中的密度，0表示第一个密度等级，......5表示第六个密度等级，0x7F表示该品质未用密度信息进行分类								********************************************
	    public byte	nFlawArea;						//描述品质中的瑕疵，0表示第一个瑕疵等级，1表示第二个瑕疵等级，2表示第三个瑕疵等级，0x7F表示该品质未用瑕疵信息进行分类		by ymh 2011-04-09
	    public byte	nRigidity;						//描述品质中的硬度，0表示第一个硬度等级，1表示第二个硬度等级，2表示第三个硬度等级，0x7F表示该品质未用硬度信息进行分类
	    public byte	nSugar;							//描述品质中的含糖量，0表示第一个含糖量等级，1表示第二个含糖量等级，2表示第三个含糖量等级，3表示第四个含糖量等级，0x7F表示该品质未用含糖量信息进行分类**************	

	    public int nMinSize;									//最小尺寸(或者体积或者重量)，上位机不同的分选标准，代表不同的量
	    public int nMaxSize;									//最大尺寸(或者体积或者重量)，上位机不同的分选标准，代表不同的量
	    public long exit;									    //等级所在的出口 64位，1表示在，0表示不在
	    public int nFruitNum;									//切换个数或切换重量(装箱量)---------------现在不用其做出口的切换，只用来计算每箱重---by ymh2011-03-12

	    public byte	nLabelbyGrade;					//某等级的贴标序号 0：不贴标，1-4：贴标机的序号（1，2，3，4）。该数据只有在贴标方式（stGradeInfo::nLabelType）为1时有效。

        public stGradeItemInfo(bool IsOK)
        {
            bShapeSize = 0;
            nColorGrade = 0;
            bDensity = 0;
            nFlawArea = 0;
            nRigidity = 0;
            nSugar = 0;
            nMinSize = 0;
            nMaxSize = 0;
            exit = 0;
            nFruitNum = 0;
            nLabelbyGrade = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stGradeInfo
    {	
        public float   fShapeFactor;            //用于描述水果的形状，水果的最大直径/最小直径 -- new
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_INTERVAL_NUM)]
        public stColorIntervalItem[] intervals;	//颜色区间stColorIntervalItem[ConstPreDefine.MAX_COLOR_INTERVAL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_GRADE_NUM*ConstPreDefine.MAX_COLOR_INTERVAL_NUM)]
        public stPercentInfo[,] percent;        //各个颜色等级在每个颜色区间的范围stPercentInfo[ConstPreDefine.MAX_COLOR_GRADE_NUM,ConstPreDefine.MAX_COLOR_INTERVAL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public stGradeItemInfo[,] grades;	    //等级信息stGradeItemInfo[ConstPreDefine.MAX_QUALITY_GRADE_NUM,ConstPreDefine.MAX_SIZE_GRADE_NUM]	
        public int nClassifyType;												//最低位开始，依次代表 品质，重量（克），重量（个），直径，面积，体积，第9~11位依次表示最小直径
                                                                            //  最大直径和垂直直径，（这3个量仅在表示直径或者体积的位 为 1 的时候有效）	*************** by ymh 2011-04-09
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] strColorGradeName;	    //颜色等级名称byte[ConstPreDefine.MAX_COLOR_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] strQualityGradeName;     //品质等级名称--new byte[ConstPreDefine.MAX_QUALITY_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SIZE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] strSizeGradeName;		//尺寸等级名称byte[ConstPreDefine.MAX_SIZE_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SHAPE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] strShapeGradeName;	    //形状等级的名称 --new byte[ConstPreDefine.MAX_SHAPE_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] stFlawareaGradeName;	  // 瑕疵等级的名称	byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]*********************************************************************			
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] stDensityGradeName;	 // 密度等级的名称byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] stRigidityGradeName;	 // 硬度等级的名称byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[,] stSugarGradeName;	 // 含糖量等级的名称byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]				   ymh     2012-03-03		
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_DENSITY_GRADE_NUM)]
        public float[] fDensityFactor;		 // 用于描述水果的密度,最后一个元素置0 float[ConstPreDefine.MAX_DENSITY_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM*2)]
        public uint[,] unFlawAreaFactor;	 // 用于描述水果的瑕疵，[瑕疵面积][瑕疵个数]uint[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,2]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_RIGIDITY_GRADE_NUM)]
        public float[] fRigidityFactor;		 // 用于描述水果的硬度，最后一个元素置0 float[ConstPreDefine.MAX_RIGIDITY_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUGAR_GRADE_NUM)]
        public float[] fSugarFactor;		 // 用于描述水果的含糖量，最后一个元素置0  float[ConstPreDefine.MAX_SUGAR_GRADE_NUM]***************************************************

    ///////////////////////////////2010.08.10 by lz, 修改协议用于增加每个页面（上下两个通道）的贴标出口数量
        public byte nLabelType;              //贴标的工作方式选择，0：所有等级和所有出口都不贴标；1：按等级方式进行贴标；2：按出口方式进行贴标	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public byte[] nLabelbyExit;          //某出口的贴标机序号 0：不贴标，1-4：贴标机的序号（1，2，3，4）。该数据只有在贴标方式（stGradeInfo::nLabelType）为2时有效 byte[ConstPreDefine.MAX_EXIT_NUM]。

    ///////////////////////////////
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public int[] nExitSwitchNum;	     //	int[ConstPreDefine.MAX_EXIT_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public byte[] nSwitchLabel;			 //--按照出口切换水果,nSwitchLabel 为0 的出口按照个数来切换，nSwitchLabel为1 的出口按照重量来切换 byte[ConstPreDefine.MAX_EXIT_NUM]
                                                                            //--nSwitchLabel为2 的出口按照体积来切换,0x7F表示该出口不进行切换----by ymh 2011-04-09
        public byte ColorType;    //0000 0000，低四位有效，低四位的首位为0代表按照平均值来进行颜色分级，为1代表按照百分比来分级
                           //后三位分别对应按照UV，H，和灰度值来作为标准，为1 时有效,当选择平均值时，将水果的结果直接赋值给color0
                           //首位为1代表进行瑕疵分选，首位为0表示不进行瑕疵分选
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] ColorIntervals;   //存储两个滑块对应的值 int[2]/********************************  by ymh 2012-07-18 *****/																																		
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] ExitEnabled;	//64位，其中【0】的32位和【1】的低16位有效，表示当前出口是否有效出口---*********by ymh 2012-08-20*** int[2]

        public stGradeInfo(bool IsOK)
        {
            fShapeFactor = 0;
            intervals = new stColorIntervalItem[ConstPreDefine.MAX_COLOR_INTERVAL_NUM];
            percent = new stPercentInfo[ConstPreDefine.MAX_COLOR_GRADE_NUM, ConstPreDefine.MAX_COLOR_INTERVAL_NUM];
            grades = new stGradeItemInfo[ConstPreDefine.MAX_QUALITY_GRADE_NUM, ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nClassifyType = 0;
            strColorGradeName = new byte[ConstPreDefine.MAX_COLOR_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            strQualityGradeName = new byte[ConstPreDefine.MAX_QUALITY_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            strSizeGradeName = new byte[ConstPreDefine.MAX_SIZE_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            strShapeGradeName = new byte[ConstPreDefine.MAX_SHAPE_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            stFlawareaGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            stDensityGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            stRigidityGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            stSugarGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM, ConstPreDefine.MAX_TEXT_LENGTH];
            fDensityFactor = new float[ConstPreDefine.MAX_DENSITY_GRADE_NUM];
            unFlawAreaFactor = new uint[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM, 2];
            fRigidityFactor = new float[ConstPreDefine.MAX_RIGIDITY_GRADE_NUM];
            fSugarFactor = new float[ConstPreDefine.MAX_SUGAR_GRADE_NUM];

            nLabelType = 0;
            nLabelbyExit = new byte[ConstPreDefine.MAX_EXIT_NUM];
            
            nExitSwitchNum = new int[ConstPreDefine.MAX_EXIT_NUM];
            nSwitchLabel = new byte[ConstPreDefine.MAX_EXIT_NUM];

            ColorType = 0;
            ColorIntervals = new int[2];
            ExitEnabled = new int[2];
        }
   
    }

    // 出口信息设置,发送给选中子系统的FSM (HC_ID, IPM, HC_CMD_EXIT_INFO, stExitInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stExitItemInfo
    {
	    public int nDis;			//距离
	    public int nOffset;		    //偏移
	    public short nDriverPin;	//驱动器管脚设置

        public stExitItemInfo(bool IsOK)
        {
            nDis = 0;
            nOffset = 0;
            nDriverPin = 0;
        }
    }

    // 贴标信息设置
    [StructLayout(LayoutKind.Sequential)]
    public struct stLabelItemInfo
    {
	    public int nDis;            //一个子系统的12个通道贴标机1（2，3，4）的节距是相同的
	    public short nDriverPin;	//驱动器管脚设置

        public stLabelItemInfo(bool IsOK)
        {
            nDis = 0;
            nDriverPin = 0;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stExitInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_LABEL_NUM)]
        public stLabelItemInfo[] labelexit;	//贴标出口 stLabelItemInfo[ConstPreDefine.MAX_LABEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public stExitItemInfo[] exits; //stExitItemInfo[ConstPreDefine.MAX_EXIT_NUM]

        public stExitInfo(bool IsOK)
        {
            labelexit = new stLabelItemInfo[ConstPreDefine.MAX_LABEL_NUM];
            exits = new stExitItemInfo[ConstPreDefine.MAX_EXIT_NUM];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stGlobalExitInfo
    {
	    public int nPulse;							//电磁阀脉宽
	    public int nLabelPulse;						//贴标脉宽

        public stGlobalExitInfo(bool IsOK)
        {
            nPulse = 0;
            nLabelPulse = 0;
        }
    }

    // 重量信息设置,发送给选中子系统的FSM (HC_ID, WM, HC_CMD_WEIGHT_INFO, stWeightBaseInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stWeightBaseInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] fGADParam;					//G-AD系数 float[2]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] waveinterval;		//波形中间组的参数byte[2]
	    public float fTemperatureParams;			//校正系数

        public stWeightBaseInfo(bool IsOK)
        {
            fGADParam = new float[2];
            waveinterval = new byte[2];
            fTemperatureParams = 0;
        }
    }

    // 全局重量信息设置,发送给每一个FSM (HC_ID, FSM, HC_CMD_WEIGHT_INFO, stGlobalWeightBaseInfo)
    [StructLayout(LayoutKind.Sequential)]
    public  struct stGlobalWeightBaseInfo
    {
	    public short nMinGradeThreshold;							//最小等级阈值
	    public short nCupDeviationThreshold;						//杯偏差阈值
	    public short nCupBreakageThreshold;						//杯破损阈值
	    public short nBaseCupNum;									//基准果杯数
	    public float fFilterParam;									//滤波系数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public short[] nTotalCupNums;//杯总数	short[ConstPreDefine.MAX_SUBSYS_NUM]

        public stGlobalWeightBaseInfo(bool IsOK)
        {
            nMinGradeThreshold = 0;
            nCupDeviationThreshold = 0;
            nCupBreakageThreshold = 0;
            nBaseCupNum = 0;
            fFilterParam = 0;
            nTotalCupNums = new short[ConstPreDefine.MAX_SUBSYS_NUM];
        }
    }

    //通道范围等参数设置,发送给选中子系统的FSM (HC_ID, IPM, HC_CMD_PARAS_INFO, stParas)
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitCup
    {
	    //int nCupNum;//真实果杯数目
	    public int nTop;
	    public int nBottom;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CUP_NUM + 1)]
	    public int[] nLeft;//果杯坐标的维数与最大数量的果杯数一致int[ConstPreDefine.MAX_CUP_NUM + 1]

        public stFruitCup(bool IsOK)
        {
            nTop = 0;
            nBottom = 0;
            nLeft = new int[ConstPreDefine.MAX_CUP_NUM + 1];
        }
    }

    // 每个相机的参数
    [StructLayout(LayoutKind.Sequential)]
    public struct stCameraParas
    {
	    public byte cCameraNum;				//保留用，相机id
	    public int nGain;						//GAIN
	    public int nShutter;					//快门
	    public float fGammaCorrection;			//GAMMA校正
	    public byte nWhiteBalanceR;	//白平衡R分量
	    public byte nWhiteBalanceG;	//白平衡G分量
	    public byte  nWhiteBalanceB;	//白平衡B分量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public stFruitCup[] cup;	//位置	stFruitCup[ConstPreDefine.CHANNEL_NUM]

        public stCameraParas(bool IsOK)
        {
            cCameraNum = 0;
            nGain = 0;
            nShutter = 0;
            fGammaCorrection = 0;
            nWhiteBalanceR = 0;
            nWhiteBalanceG = 0;
            nWhiteBalanceB = 0;
            cup = new stFruitCup[ConstPreDefine.CHANNEL_NUM];
            for (int i = 0; i < cup.Length; i++)
            {
                cup[i] = new stFruitCup(true);
            }
        }
    }

    //每个IPM的参数
    [StructLayout(LayoutKind.Sequential)]
    public struct stParas
    {
	    public int nCupNum;					//果杯数目
	    public float fPixelRatio;				//像素当量

	    public int nMaxGrayThreshold;			//颜色最大亮度
	    public int nMinGrayThreshold;			//颜色最小亮度
	    public int nDetectionThreshold;		//分离水果阈值
	    public int nDefaultDetectionThreshold;	//默认分离水果阈值（只从下位机往上位机传）
	
	    public byte nXYEdgeBreakTh;   //水果重叠阈值
	    public float fFruitCupRangeTh;		    //水果越界阈值
	    public float fSeedPointRange; 		    //种子点区间范围，

	    public ushort nDiameterColor;	// 16位，首位无效，低位表示最小直径颜色，次位表示最大直径颜色，再位表示垂直直径颜色
	    // 三者取值均为~4,对应红色、黄色、绿色、蓝色、粉色
	    // 如：010 001 000  表示最小直径为红色直径，最大为黄色，垂直为绿色直径		2013.5.17
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CAMERA_NUM)]
        public stCameraParas[] cameraParas;//stCameraParas[ConstPreDefine.MAX_CAMERA_NUM]

        public stParas(bool IsOK)
        {
            nCupNum = 0;
            fPixelRatio = 0;
            nMaxGrayThreshold = 0;
            nMinGrayThreshold = 0;
            nDetectionThreshold = 0;
            nDefaultDetectionThreshold =0;
            nXYEdgeBreakTh = 0;
            fFruitCupRangeTh = 0;
            fSeedPointRange = 0;
            nDiameterColor = 0;
            cameraParas =new stCameraParas[ConstPreDefine.MAX_CAMERA_NUM];
            for (int i = 0; i < cameraParas.Length; i++)
            {
                cameraParas[i] = new stCameraParas(true);
            }
        }
    }

     /* 斑点检测参数
     *
     *
     * 2012.3.3
     *
     * guo
     */
    [StructLayout(LayoutKind.Sequential)]
    public struct stYuvRange
    {
		public int nYmax;
		public int nYmin;		
		public int nUmax;
		public int nUmin;
		public int nVmax;
		public int nVmin;

        public stYuvRange(bool IsOK)
        {
            nYmax = 0;
            nYmin = 0;
            nUmax = 0;
            nUmin = 0;
            nVmax = 0;
            nVmin = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stYuvThresh
    {
	    public int nNum;								//  该结构由于数据量比较大不通过FSM，EEPROM也不保存该信息，由IPM与HC各自保存一份
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_YUV_THRESH_NUM)]
        public stYuvRange[] stYuv;	//  	IPM启动时，读取自己目录下的TXT文档进行初始化 stYuvRange[ConstPreDefine.MAX_YUV_THRESH_NUM]

        public stYuvThresh(bool IsOK)
        {
            nNum = 0;
            stYuv = new stYuvRange[ConstPreDefine.MAX_YUV_THRESH_NUM];
        }
    }								// 该结构由HC直接读本目录下的TXT文档，然后将其传送给IPM

    [StructLayout(LayoutKind.Sequential)]
    public struct stLevelFeature
    {
	    public int nLevelNum;                   //所分层数
	    public float[] fThreshold =  new float[ConstPreDefine.MAX_LEVEL];

        public stLevelFeature(bool IsOK)
        {
            nLevelNum = 0;
            for (int i = 0; i < ConstPreDefine.MAX_LEVEL; i++)
            {
                fThreshold[i] = 0;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stHuaPiGuoThresh
    {			
		public int[] nGrayThresh = new int[4];			//灰度阈值---2012-03-27  by--ymh
		public float[] fAreaRatioThresh = new float[4];	//比例阈值---2012-03-27  by--ymh	

        public stHuaPiGuoThresh(bool IsOK)
        {
            for(int i=0;i<4;i++)
            {
                nGrayThresh[i] = 0;
                fAreaRatioThresh[i] = 0;  
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stSpotDetectThresh
    {
	    public int nSpotAreaMin;                     //瑕疵最小面积阈值
	    public int nSpotAreaMax;					  //瑕疵最大面积阈值   fSpotAreaRatioMax * FruitArea
	    public int nSpotGrayCenterGap;               //灰度检测斑点中心距离阈值
	    public int nSpotBlueCenterGap;               //蓝色分量检测出的斑点与当前链表中已有斑点的距离阈值
	    public int nLightAreaToZeroThresh;           //过亮区域置0阈值
	    public int nMakeBlueBinThresh;               //由图像蓝色分量生成二值图像的阈值

	    public int nErodeTimesForEfctOutline;        //获取轮廓要腐蚀的次数 
	    public int nLayerNumForEdgeSpot;             //选取第几层的二值图像

	    public int nYuvBinDilateTimes;               //经YUV空间阈值处理后二值图像的膨胀次数
	    public int nYuvBinErodeTimes;				  //经YUV空间阈值处理后二值图像的腐蚀次数
	    public int nBlueBinDilateTimes;              //由像素蓝色分量获得的二值图像要进行膨胀的次数

	    public float fOutlineEfectPointRatio;        //斑点轮廓合格点所占的比例

	    public int nVertiOffsetRange;               //垂直偏差************************************************
	    public int nAngleOffsetRange;               //角度偏差     2012-03-27  by--ymh
	    public int nAreaOffsetRange;                //面积偏差*************************************************
	    public stLevelFeature stLevelFeature;
	    public stHuaPiGuoThresh stHuaPiGuoThresh;	//花皮果参数 -----2012-03-27  by--ymh

        public stSpotDetectThresh(bool IsOK)
        {
            nSpotAreaMin = 0;
            nSpotAreaMax = 0;
            nSpotGrayCenterGap = 0;
            nSpotBlueCenterGap = 0;
            nLightAreaToZeroThresh = 0;
            nMakeBlueBinThresh = 0;
            nErodeTimesForEfctOutline = 0;
            nLayerNumForEdgeSpot = 0;
            nYuvBinDilateTimes = 0;
            nYuvBinErodeTimes = 0;
            nBlueBinDilateTimes = 0;
            fOutlineEfectPointRatio = 0;
            nVertiOffsetRange = 0;
            nAngleOffsetRange = 0;
            nAreaOffsetRange = 0;
            stLevelFeature = new stLevelFeature();
            stHuaPiGuoThresh = new stHuaPiGuoThresh();
        }
    }




    // 电磁阀测试, 发送给选中子系统的FSM (HC_ID, IPM, HC_CMD_TEST_VOLVE, stVolveTest)
    [StructLayout(LayoutKind.Sequential)]
    public struct stTestParam
    {
	    public byte nExitId;//对出口号进行编码，第一列的编号数目为0到47，第二列的编号数目为48到95
						      //255时表示关闭,254时表示在用户设置里使用电磁阀测试 253-250表示1通道的贴标测试
						      //  249-246 表示2通道的贴标测试

        public stTestParam(bool IsOK)
        {
            nExitId = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stVolveTest
    {
	    public stTestParam a;// 电磁阀测试Id	
	    public stExitInfo  b;//出口信息

        public stVolveTest(bool IsOK)
        {
            a = new stTestParam();
            b = new stExitInfo(true);
        }
    }

    //AD归零
    [StructLayout(LayoutKind.Sequential)]
    public struct stResetAD
    {
	    public int value;//0 - AD0; 1 - AD1

        public stResetAD(bool IsOK)
        {
            value = 0;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////下位机-->上位机通信协议///////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    // 下位机通过命令将系统实时信息传输给上位机。
    // 上位机使用2个端口接收数据，图像数据由端口1接收，其它数据由端口2接收。

    // 图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, stImageInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stImageInfo
    {
	    int  nRouteId;
	    int  width;
	    int  height;
	    public uint[] unFlawArea = new uint[ConstPreDefine.CHANNEL_NUM];//上下两个通道
	    public uint[] unFlawNum = new uint[ConstPreDefine.CHANNEL_NUM];
	    public byte[] imagedata = new byte[ConstPreDefine.CAPTURE_WIDTH*ConstPreDefine.CAPTURE_HEIGHT*2];	//两张图片，一张拼图，和一张瑕疵检测图

        public stImageInfo(bool IsOK)
        {
            nRouteId = 0;
            width = 0;
            height = 0;
            for (int i = 0; i < ConstPreDefine.CHANNEL_NUM; i++)
            {
                unFlawArea[i] = 0;
                unFlawNum[i] = 0;
            }
            for (int i = 0; i < ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 2; i++)
            {
                imagedata[i] = 0;
            }
        }
    }

    //上位机与IPM 之间进行白平衡设置时所用到的3个结构， FSM 没有用到-----by  gwf 2011-06-28
    [StructLayout(LayoutKind.Sequential)]
    public struct stWhiteBalanceMean
    {
	    public byte MeanR;
	    public byte MeanG;
	    public byte MeanB;

        public stWhiteBalanceMean(bool IsOK)
        {
            MeanR = 0;
            MeanG = 0;
            MeanB = 0;
        }
    }

    ///////////////////////////revised by LZ 2011.06.28
    [StructLayout(LayoutKind.Sequential)]
    public struct stWhiteBalanceParam
    {
	    public byte WhiteBalanceCameraId;				//当前需要进行自动白平衡的相机	0~3
	    public int minx;                  //区域x坐标
	    public int miny;                  //区域y坐标    
	    public int maxx;                  //区域x坐标
	    public int maxy;
	    public stWhiteBalanceMean MeanValue;      //R G B 标准值

        public stWhiteBalanceParam(bool IsOK)
        {
            WhiteBalanceCameraId = 0;
            minx = 0;
            miny = 0;
            maxx = 0;
            maxy = 0;
            MeanValue = new stWhiteBalanceMean();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WhiteBalanceCoefficient{      //白平衡系数               revised by guo 2011 6 29    /*=============revised by guo   2011 6 29=========================*/
	    public stWhiteBalanceMean  MeanValue;     //区域的R G B 实际值
	    public byte CoR;                 // 校正后的系数
	    public byte CoG;
	    public byte CoB;

        public WhiteBalanceCoefficient(bool IsOK)
        {
            MeanValue = new stWhiteBalanceMean();
            CoR = 0;
            CoG = 0;
            CoB = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stCameraNum{
	    public byte cCameraNum;    //相机编号：1~4，由相机IP地址网关确定，例如：192.168.2.10表示相机编号为2，则cCameraNum == 2
        public stCameraNum(bool IsOK)
        {
            cCameraNum = 0;
        }
    }



    // 全局配置信息,用于上位机获取每一个下位机子系统的配置,FSM发送过来 (FSM, HC_ID, FSM_CMD_CONFIG, stGlobal)
    [StructLayout(LayoutKind.Sequential)]
    public struct stGlobal
    {
	    public int nSubsysId;								//子系统id,FSM

	    // 系统配置和等级信息 每一个子系统都是相同的
	    public stSysConfig sys;							//系统配置
	    public stGradeInfo grade;							//等级信息	
	    public stGlobalExitInfo gexit;						//全局出口信息
	    public stGlobalWeightBaseInfo gweight;				//全局重量信息

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CHANNEL_NUM)]
	    public stExitInfo[] exit;			//出口信息stExitInfo[ConstPreDefine.MAX_CHANNEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_IPM_NUM)]
	    public stParas[] paras;				//IPM参数信息stParas[ConstPreDefine.MAX_IPM_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CHANNEL_NUM)]
	    public stWeightBaseInfo[] weights;	//称重参数stWeightBaseInfo[ConstPreDefine.MAX_CHANNEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_IPM_NUM)]
	    public stSpotDetectThresh[] spotdetectthresh;	//瑕疵参数信息stSpotDetectThresh[ConstPreDefine.MAX_IPM_NUM]	

	    public byte nNetState;								// 8位，低6位有效，低6位代表6个IPM，0 正常，1故障

        public stGlobal(bool IsOK)
        {
            nSubsysId = 0;
            sys = new stSysConfig(true);
            grade = new stGradeInfo(true);
            gexit = new stGlobalExitInfo(true);
            gweight = new stGlobalWeightBaseInfo(true);
            exit = new stExitInfo[ConstPreDefine.MAX_CHANNEL_NUM];
            paras = new stParas[ConstPreDefine.MAX_IPM_NUM];
            weights = new stWeightBaseInfo[ConstPreDefine.MAX_CHANNEL_NUM];
            spotdetectthresh = new stSpotDetectThresh[ConstPreDefine.MAX_IPM_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
            {
                exit[i] = new stExitInfo(true);
                weights[i] = new stWeightBaseInfo(true);
            }

            for (int i = 0; i < ConstPreDefine.MAX_IPM_NUM; i++)
            {
                paras[i] = new stParas(true);
                spotdetectthresh[i] = new stSpotDetectThresh(true);
            }
            nNetState = 0;
        }
    }

    // 基本统计信息,FSM发送过来 (FSM, HC_ID, FSM_CMD_STATISTICS, stStatistics)
    [StructLayout(LayoutKind.Sequential)]
    public struct stStatistics
    {
	    public int nSubsysId;												    //子系统id,FSM
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public int[,] nBoxGradeCount;	//各个等级的箱数int[ConstPreDefine.MAX_QUALITY_GRADE_NUM,ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public int[] nBoxGradeWeight;  //重量分选时的每箱重int[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public ulong[] nGradeCount;//各个等级的水果数(生产总数)ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public ulong[] nWeightGradeCount;//重量分选时的各个等级的生产重量ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public ulong[] nExitCount;		//各个出口的水果数，单位：个ulong[ConstPreDefine.MAX_EXIT_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public ulong[] nExitWeightCount;//重量分选时各个出口的重量。单位：克------by ymh 2011-02-21 ulong[ConstPreDefine.MAX_EXIT_NUM]
	    public ulong nTotalCount;											//水果批个数
	    public ulong nWeightCount;											//水果批重量********unsigned long 对应上位机的数据类型是 __int64 ----by ymh 2011-04-14	
	    public int nTotalCupNum;											//总的果杯数
	    public int nInterval;												//与上次发送统计信息的间隔数
	    public int nIntervalSumperminute;									//一分钟内光电开关的个数,计算分选速度-----------by ymh 2011-02-21
	    public byte nNetState;						// 8位，低6位有效，低6位代表6个IPM，最低位代表IPM1，0 正常，1故障
	    public ushort nCupState;   		//12个通道的果杯状态，低12位有效，最低位代表通道1，0正常，1故障	
	    public ushort nPulseInterval;     //2000以上时，分选速度为0；单位为ms
	    public ushort nUnpushFruitCount;  //遗漏的水果个数,上位机每20秒调用一次	

        public stStatistics(bool IsOK)
        {
            nSubsysId = 0;
            nBoxGradeCount = new int[ConstPreDefine.MAX_QUALITY_GRADE_NUM, ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nBoxGradeWeight = new int[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nGradeCount = new ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nWeightGradeCount = new ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nExitCount = new ulong[ConstPreDefine.MAX_EXIT_NUM];
            nExitWeightCount = new ulong[ConstPreDefine.MAX_EXIT_NUM];
            nTotalCount = 0;
            nWeightCount = 0;
            nTotalCupNum = 0;
            nInterval = 0;
            nIntervalSumperminute = 0;
            nNetState = 0;
            nCupState = 0;
            nPulseInterval = 0;
            nUnpushFruitCount = 0;
        }
    }


    /// 水果实时分级信息,IPM发送给FSM (IPM, HC_ID, IPM_CMD_FRUITINFO, stFruitVisionGradeInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitVisionParam
    {
	    public uint unMaxR;
	    public uint unMinR;
	    /*unsigned int unPlumbR;				//垂直于对称轴的最大直径的量******************************************
	    unsigned int unCentreR;				//对称轴
	    unsigned int unCoreR;				//垂直对称轴且过重心的量						
	    unsigned int unValue;				//用来分选的量,描述按照尺寸分选时采用的水果的某一条直径，该值为上面5条直径其中之一
	    */
	    public uint unSymmetryAxisD;			//对称轴
	    public uint unVertiSAMaxL;				//垂直对称轴且最大的弦
	    public int unVertiSACenterD;			//垂直对称轴且过重心								
	    public uint unSelectBasis;				//用来分选的量,描述按照尺寸分选时采用的水果的某一条直径，该值为上面5条直径其中之一
	    public float fDiameterRatio;           	    //最大直径/最小直径
	    public int unColorRate0;
	    public uint unColorRate1;
	    public uint unColorRate2;
	    public uint unArea;				//水果的投影面积******************************************************	
	    public uint unFlawArea;			//水果的瑕疵面积		change by ymh  2011-02-16
	    public uint unVolume;				//水果的体积	******************************************************
	    public uint unFlawNum;				//水果瑕疵斑点的个数-------------------------by ymh 2012-03-03	

        public stFruitVisionParam(bool IsOK)
        {
            unMaxR = 0;
            unMinR = 0;
            unSymmetryAxisD = 0;
            unVertiSAMaxL = 0;
            unVertiSACenterD = 0;
            unSelectBasis = 0;
            fDiameterRatio = 0;
            unColorRate0 = 0;
            unColorRate1 = 0;
            unColorRate2 = 0;
            unArea = 0;
            unFlawArea = 0;
            unVolume = 0;
            unFlawNum = 0;
        }
    }

    /// 水果实时分级信息,FSM发送过来 (IPM, HC_ID, FSM_CMD_GRADEINFO, stFruitGradeInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitParam
    {
	    public stFruitVisionParam visionparam;
	    public uint unGrade;			//等级
	    public float fRigidity;				//硬度
	    public float fSugar;					//含糖量					by ymh 2011-04-09
	    public float fWeight;			    //果重	
	    public float fDensity;				//密度   水果重量/体积，单位：克/立方毫米

        public stFruitParam(bool IsOK)
        {
            visionparam = new stFruitVisionParam(true);
            unGrade = 0;
            fRigidity = 0;
            fSugar = 0;
            fWeight = 0;
            fDensity = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitGradeInfo
    {
	    public int nRouteId;							//IPM的id号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public stFruitParam[] param;		//两个通道的水果等级信息stFruitParam[ConstPreDefine.CHANNEL_NUM]
	    public int nDefaultDetectionThreshold;			//默认分离水果阈值

        public stFruitGradeInfo(bool IsOK)
        {
            nRouteId = 0;
            param = new stFruitParam[ConstPreDefine.CHANNEL_NUM];
            nDefaultDetectionThreshold = 0;

        }
    }

    // 重量统计信息,FSM发送过来 (WM, HC_ID, FSM_CMD_WEIGHTINFO, stWeightResult)
    [StructLayout(LayoutKind.Sequential)]
    public struct stWeightStat
    {		
	    public float fCupAverageWeight;			//果杯均重
	    public ushort nAD0;				//AD0通道
	    public ushort nAD1;				//AD1通道
	    public ushort nStandardAD0;
	    public ushort nStandardAD1;
        public stWeightStat(bool IsOK)
        {
            fCupAverageWeight = 0;
            nAD0 = 0;
            nAD1 = 0;
            nStandardAD0 = 0;
            nStandardAD1 = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stTrackingData
    {
	    public int nVehicleId;						//车号
	    public float fFruitWeight;					//果重
	    public float fVehicleWeight;				//车重
	    public short nADFruit;						//AD果
	    public short nADVehicle;					//AD车

        public stTrackingData(bool IsOK)
        {
            nVehicleId = 0;
            fFruitWeight = 0;
            fVehicleWeight = 0;
            nADFruit = 0;
            nADVehicle = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stWeightResult
    {
	    public int nChannelId;							//通道id,WM
	    public byte state;								//果杯状态 0-正常 1-故障

	    //统计信息
	    public stWeightStat paras;

	    ////波形数据
	    //short waveform0[3][256];				//3组
	    //short waveform1[3][256];				//3组
	
	    //追踪数据
	    public stTrackingData data;
	    public float fVehicleWeight0;
	    public float fVehicleWeight1;

        public stWeightResult(bool IsOK)
        {
            nChannelId = 0;
            state = 0;
            paras = new stWeightStat();
            data = new stTrackingData();
            fVehicleWeight0 = 0;
            fVehicleWeight1 = 0;
        }
    }

    // 波形数据,FSM发送过来 (WM, HC_ID, FSM_CMD_WAVEINFO, stWaveInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stWaveInfo
    {
	    public int nChannelId;							//通道id,WM

        public short[] waveform0 = new short[256];					//3组
	    public short[] waveform1 = new short[256];					//3组

        public stWaveInfo(bool IsOK)
        {
            nChannelId = 0;
            for (int i = 0; i < 256; i++)
            {
                waveform0[i] = 0;
                waveform1[i] = 0;
            }
        }
    }
}
