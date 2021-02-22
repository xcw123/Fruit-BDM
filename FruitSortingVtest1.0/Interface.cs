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
        // SIM id
        public const int SIM_ID = 0x1100;
        // SIM ip地址
        public const string SIM_IP_ADDR = "192.168.0.14"; //192.168.0.14
        // FSM端口号
        public const int FSM_PORT_NUM = 1279;
        // IPM端口号
        public const int IPM_PORT_NUM = 1289;
        //SIM端口号
        public const int SIM_PORT_NUM = 1129;
        // 最大子系统数
        public const int MAX_SUBSYS_NUM = 4;
        // 单个子系统最多IPM个数
        public const int MAX_IPM_NUM = 12;
        // 单个子系统支持最大通道数
        public const int MAX_CHANNEL_NUM = 12;
        // 单个IPM支持最大通道数
        public const int CHANNEL_NUM = 2;
        // 单个IPM最多带的相机个数
        public const int MAX_CAMERA_NUM = 9;	//左中右各3个相机（1个color，2个NIR）
        //单个IPM最多带的彩色相机个数
        public const int MAX_COLOR_CAMERA_NUM = 3;	//每个IPM最多带的彩色相机个数
        //单个IPM最多带的红外相机个数
        public const int MAX_NIR_CAMERA_NUM = 6;	//每个IPM最多带的红外相机个数
        //单个IPM最多带相机种类数		
        public const int MAX_CAMERA_TYPE = 3;//彩色+两个波段的红外
        //每种相机不同方位相机数		
        public const int MAX_CAMERA_DIRECTION = 3;//左中右3个

        ////单通道最多相机数
        //public const int MAX_CAMERA_NUM_CHANNEL = 4;	//因为通道相机均分	(MAX_CAMERA_NUM/CHANNEL_NUM)		 2016.4.7
        ////相机类型
        //public const int CAMERA_TYPE_NUM = 2;	//彩色红外两种相机 2016.4.7
        //单通道最多pair(彩色红外对)个数
       // public const int MAX_PAIR_NUM_CHANNEL = 2;		 //因为1个通道最多(MAX_CAMERA_NUM_CHANNEL/CAMERA_TYPE_NUM)   2016.4.7 
        // 单个相机果杯框路数
       // public const int CUP_CHANNEL_NUM = 1;

        // 其它常量
        public const int MAX_QUALITY_GRADE_NUM = 16;  //品质最多等级数
        public const int MAX_SIZE_GRADE_NUM = 16;	  //尺寸（重量）最多等级数
        public const int MAX_EXIT_NUM = 48;           //最多出口数
        public const int MAX_EXIT_DISPALYNAME_LENGTH = 20;    //最大显示名称的长度（出口）
        public const int MAX_EXIT_ADDITIONALNAME_LENGTH = 100;//最大附加名称的长度（出口）
        public const int MAX_COLOR_INTERVAL_NUM = 3;  //颜色最多区间数（红，黄，绿）
        public const int MAX_CUP_NUM = 10;	          //最多果杯个数
        public const int MAX_SPLICE_CUP_NUM = 19;	  //最大颜色显示界面果杯个数
        public const int PARAS_TAGINFO_NUM = 6;       //去标签参数个数
        public const int BYTE_NUM_PER_PAGE = 64;
        public const int MAX_LABEL_NUM = 4;
        public const int VERSION = 40201;	                  //版本号，用于比较 //Modify by ChengSk - 20180403 20190516 （只比较主版本号4，副版本号01，不比较修正版本号03）
        public const string VERSION_SHOW = "4.2.1";           //版本号，用于显示 //Add by ChengSk - 20171023  Modify by ChengSk - 20180403  20190516
        public const int FSM_DSP_MODULE = 1;//当前的FSM是 DSP----DM642
        public const int FSM_STM32_MODULE = 2;//表示当前的FSM是 STM32--H7
        public const int MAX_FRUIT_TYPE = 32;//水果种类最多32种（大种类，中国脐橙和南非脐橙算同一类）
        public const int CAPTURE_WIDTH = 2560;       //拼图的最大长度
        public const int CAPTURE_HEIGHT = 1920;       //拼图的最大宽度
        public const int MAX_TEXT_LENGTH = 12;        //20太长，改为12--->12限制，改成14
        public const int MAX_FRUIT_NAME_LENGTH = 50;    //水果名称最大长度
        public const int MAX_CLIENTINFO_LENGTH = 20;  //客户信息的长度 Add by ChengSk - 20190929
        public const int IPM_INIT_TIME = 4000;//IPM初始化相机时间 ms

        //public const int GRADE_QUALITY_ON = 1;
        public const int GRADE_WEIGHTGRAM_ON = 1;     //（重量）克
        public const int GRADE_WEIGHTGE_ON = 2;       //（重量）个
        public const int GRADE_DIAMETER_ON = 4;       //（尺寸）直径
        public const int GRADE_AREA_ON = 8;           //（尺寸）面积
        public const int GRADE_VOLUME_ON = 16;        //（尺寸）体积

        public const int MAX_COLOR_GRADE_NUM = 16;    //颜色
        public const int MAX_SHAPE_GRADE_NUM = 6;     //形状 2015-4-8
        public const int MAX_FlAWAREA_GRADE_NUM = 6;  //瑕疵	

        public const int MAX_BRUISE_GRADE_NUM = 6;    //擦伤
        public const int MAX_ROT_GRADE_NUM = 6;       //腐烂

        public const int MAX_DENSITY_GRADE_NUM	= 6;  //密度	

        public const int MAX_SUGAR_GRADE_NUM = 6;     //糖度
        public const int MAX_ACIDITY_GRADE_NUM = 6;   //酸度
        public const int MAX_HOLLOW_GRADE_NUM = 6;    //空心
        public const int MAX_SKIN_GRADE_NUM = 6;      //浮皮
        public const int MAX_BROWN_GRADE_NUM = 6;     //褐变
        public const int MAX_TANGXIN_GRADE_NUM = 6;   //糖心

        public const int MAX_RIGIDITY_GRADE_NUM = 6;  //硬度
        public const int MAX_WATER_GRADE_NUM = 6;     //含水率

        public const int MAX_FRUIT_TYPE_MAJOR_CLASS_NUM = 32;        //水果大类数量 2015-6-26 ivycc
        public const int MAX_FRUIT_TYPE_SUB_CLASS_NUM = 8;        //水果子类数量 2015-6-26 ivycc
        public const int MAX_FRUIT_TEXT_LENGTH = 20;        //20太长，改为12
        public const string BROADCAST_IP_ADDR = "192.168.10.255"; //广播地址
        public const string BROADCAST_LOCAL_IP_ADDR = "192.168.10.254"; //广播地址
        public const int BROADCAST_PORT_NUM = 4567;              //广播端口号
        public const int MAX_SPLICE_IMAGE_WIDTH = 3200;			//瑕疵检测测试图像宽 2016.2.29
        public const int MAX_SPLICE_IMAGE_HEIGHT = 512;         //瑕疵检测测试图像高 2016.2.29

        //************************系统类型****************************//
        public const int RM_M = 16;//000 010 000 1种彩色(1)				每三位代表一个相机种类，顺序是前中后；其中3个bit又分别代表左、中、右
        public const int RM_LMR	= 56;	//000 111 000 1种彩色(3)

        public const int RM_F_M	= 144;	//010 010 000 1种彩色1种红外(1+1)
        public const int RM_LFR_LMR	= 504;	//111 111 000 1种彩色1种红外(3+3)

        public const int RM_F_M_B = 146;	//010 010 010 1种彩色2种红外(1+2)
        public const int RM_LFR_LMR_LBR = 511;	//111 111 111 1种彩色2种红外(3+6)

        public const int RM_LR = 40;	//000 101 000 1种彩色(2)
        public const int RM_LR_LR = 360;    //101 101 000 1种彩色1种红外(2+2)   Add by ChengSk - 20190226
        public const int RM_LR_LR_LR = 365; //101 101 101 1种彩色2种红外(2+2+2) Add by ChengSk - 20190226

        //只有红外
        public const int RM_F = 128;	//010 000 000 1种红外(1)
        public const int RM_LRF = 320;  //101 000 000 1种红外(2)  Add by ChengSk - 20190226
        public const int RM_LFR = 448;	//111 000 000 1种红外(3)

        public const int RM_F_B = 130;	//010 000 010 2种红外(2)
        public const int RM_LFR_LBR = 455;	//111 000 111 2种红外(6)

        //新协议系统类型 Modify by ChengSk - 20190520 
        
        //低9位有效表示9个相机，1 表示相机存在 0000 0001 1111 1111 (依次：左中右NIR2，左中右NIR1，左中右COLOR)
        //后 —> NIR2 —>NIR1 —> Color —> 前

        public const int RM2_FM = 2;     //000 000 010  1种彩色(1)  [对应旧协议: RM_M]
        public const int RM2_FLMR = 7;   //000 000 111  1种彩色(3)  [对应旧协议: RM_LMR]

        public const int RM2_MM_FM = 18; //000 010 010  1种彩色1种红外(1+1)  [对应旧协议: RM_F_M]
        public const int RM2_MLMR_FLMR = 63; //000 111 111  1种彩色1种红外(3+3)  [对应旧协议: RM_LFR_LMR]

        public const int RM2_BM_MM_FM = 146; //010 010 010  1种彩色2种红外(1+2)  [对应旧协议: RM_F_M_B]
        public const int RM2_BLMR_MLMR_FLMR = 511;   //111 111 111  1种彩色2种红外(3+6)  [对应旧协议: RM_LFR_LMR_LBR]

        public const int RM2_FLR = 5; //000 000 101  1种彩色(2)  [对应旧协议: RM_LR]
        public const int RM2_MLR_FLR = 45;      //000 101 101  1种彩色1种红外(2+2)  [对应旧协议: RM_LR_LR]
        public const int RM2_BLR_MLR_FLR = 365; //101 101 101  1种彩色2种红外(2+2+2)  [对应旧协议: RM_LR_LR_LR]

        public const int RM2_MM = 16;    //000 010 000  1种红外(1)  [对应旧协议: RM_F]
        public const int RM2_MLR = 40;   //000 101 000  1种红外(2)  [对应旧协议: RM_LRF]
        public const int RM2_MLMR = 56;  //000 111 000  1种红外(3)  [对应旧协议: RM_LFR]

        public const int RM2_BM_MM = 144;    //010 010 000  2种红外(2)  [对应旧协议: RM_F_B]
        public const int RM2_BLMR_MLMR = 504;//111 111 000  2种红外(6)  [对应旧协议: RM_LFR_LBR]

        //////////////////////////////////////////////////////////////////////
        //////////////////////      以下内部品质接口      ////////////////////
        #region 内部品质 Add by ChengSk - 20190114
        
        public const int STX = 0x2;
        public const int SBC_STATUS_REQ = 0x11;
        public const int SBC_STATUS_SET = 0x12;
        public const int SBC_STATUS_REP = 0x13;
        public const int SBC_INFO_REQ = 0x18;
        public const int SBC_INFO_REP = 0x1A;
        public const int SBC_PARA_REQ = 0x1B;
        public const int SBC_PARA_SET = 0x1C;
        public const int SBC_PARA_REP = 0x1D;
        public const int SBC_AMO_REQ = 0x20;
        public const int SBC_AMO_SET = 0x28;
        public const int SBC_AMO_REP = 0x30;
        public const int SBC_DARKR_START_REQ = 0x41;
        public const int SBC_DARKR_DATA_REQ = 0x42;
        public const int SBC_DARKR_DATA_REP = 0x43;
        public const int SBC_DARKS_S_START_REQ = 0x44;
        public const int SBC_DARKS_S_DATA_REQ = 0x45;
        public const int SBC_DARKS_S_DATA_REP = 0x46;
        public const int SBC_DARKS_M_START_REQ = 0x5A;
        public const int SBC_DARKS_M_DATA_REQ = 0x5B;
        public const int SBC_DARKS_M_DATA_REP = 0x5C;
        public const int SBC_DARKS_L_START_REQ = 0x5D;
        public const int SBC_DARKS_L_DATA_REQ = 0x5E;
        public const int SBC_DARKS_L_DATA_REP = 0x5F;
        public const int SBC_REF_START_REQ = 0x47;
        public const int SBC_REF_DATA_REQ = 0x48;
        public const int SBC_REF_DATA_REP = 0x49;
        public const int SBC_MEAS_S_START_REQ = 0x4A;
        public const int SBC_MEAS_M_START_REQ = 0x6A;
        public const int SBC_MEAS_L_START_REQ = 0x6B;
        public const int SBC_MEAS_DATA_REQ = 0x4B;
        public const int SBC_MEAS_DATA_REP = 0x4C;
        public const int SBC_MAKE_DATA_REQ = 0x4D;
        public const int SBC_MAKE_DATA_REP = 0x4E;
        public const int SBC_MCU_LAMP_SET = 0x51;
        public const int SBC_MCU_ALARM_SET = 0x53;

        public const int MAX_BFR_SIZE = 16384;
        public const int MAX_PIXEL_NO = 1024;
        public const int NO_WAVELENGTH_FIRST = 650;
        public const int NO_WAVELENGTH_LAST = 950;
        public const int MAX_SPLINE_NO = (NO_WAVELENGTH_LAST - NO_WAVELENGTH_FIRST) + 1;
        public const int MAX_PARA_AMO_NO = 7;
        public const int MAX_RAW_DATA_NO = 6;

        public const int NO_RAW_DARKR = 0;
        public const int NO_RAW_DARKS_S = 1;
        public const int NO_RAW_DARKS_M = 2;
        public const int NO_RAW_DARKS_L = 3;
        public const int NO_RAW_REF = 4;
        public const int NO_RAW_SAMPLE = 5;

        public const int VAL_DEF_INTG_TIME = 20;
        public const int VAL_DEF_WARMUP_TIME = 1;
        public const int VAL_DEF_FRUIT_SIZE_MIN = 15;
        public const int VAL_DEF_FRUIT_SIZE_MAX = 500;

        public const string DXRAW_FILENAME = "Reemoon-Sample-";

        public const int DEV_CONT_ON = 1;
        public const int DEV_CONT_OFF = 0;
        public const int DEV_CONT_NOT = 0xFF;

        public const string TCP_SERVER_DEST_PORT = "1128";
        public const string FILE_NAME_PARA = "par";
        public const string FILE_NAME_AMO = "amo";
        public const string FILE_NAME_JDXRA = "rmj";
        public const string FILE_NAME_RAWRA = "rmr";
        public const string FILE_NAME_AMORA = "rma";

        // 下位机ip地址模板
        //public const string LC_IP_ADDR_TEMPLATE = "192.168.0."; //重复定义
        public const int LC_PORT_NUM = 1128;

        #endregion
    }

    // 命令类型定义

    //HC-->FSM
    public enum HC_FSM_COMMAND_TYPE:int
    {
        //*不带参数发给所有子系统*//
        HC_CMD_DISPLAY_OFF = 0x0000,			//断开，HC-->FSM
	    HC_CMD_CLEAR_DATA,						//数据清零，HC-->FSM
        HC_CMD_SAVE_CURRENT_DATA,               //HC发送该命令时，FSM只清理批量加工的数据，但是保存当前的各出口的装箱量
	    HC_CMD_PROJ_OPENED,						//打开工程配置
	    HC_CMD_PROJ_CLOSED,						//关闭工程配置
	    HC_CMD_WEIGHTRESET,						//称重重置
	    HC_CMD_TEST_CUP_ON,						//果杯测试开 
	    HC_CMD_TEST_CUP_OFF,					//果杯测试关 
	    HC_CMD_CUPSTATERESET,					//主界面最下端用户果杯状态复位(以上命令（不带参数）HMI发送给所有的子系统)
	
	    HC_CMD_WAVE_FORM_ON,					//波形捕捉开，HC-->FSM
	    HC_CMD_WAVE_FORM_OFF,					//波形捕捉关，HC-->FSM
	    HC_CMD_DATA_TRACKING_ON,				//数据追踪开，HC-->FSM
	    HC_CMD_DATA_TRACKING_OFF,				//数据追踪关，HC-->FSM	
	    HC_CMD_BACK_LEARN,						//更新背景，HC-->FSM
	    HC_CMD_SHUT_DOWN,						//网络关机，HC-->FSM,  现在用于上位机关机时是否向FSM保存用户配置
	    HC_CMD_GRADEINFO_ON,					//获取水果实时分级信息，HC-->FSM
	    HC_CMD_GRADEINFO_OFF,					//不获取水果实时分级信息，HC-->FSM
	    HC_CMD_WEIGHTINFO_ON,					//获取重量统计信息(只要统计信息,不需要波形和追踪数据)，HC-->FSM
	    HC_CMD_WEIGHTINFO_OFF,					//不获取重量统计信息
	    HC_CMD_SIMULATEDPULSE_ON,				//内信号源开
	    HC_CMD_SIMULATEDPULSE_OFF,				//内信号源关	
	    HC_CMD_TEST_NET,						//网络测试，每隔20S自动发送一次
        HC_CMD_MOTOR_ENABLE,                    //电机使能 2015-4-7
        HC_CMD_BOOT_APP_TO_BOOT,                //用于APP跳转boot程序
        HC_CMD_SAVE_PARAS,                      //stm32 的 fsm保存参数进Flash
        HC_CMD_DISPLAY_ON,        		//连接 int型变量描述版本号，HC-->FSM -----2010.08.25  增加一个参数（版本号）  //改为从0x0050开始，Modify by ChengSk - 20190924

        //*带参数发给所有子系统*//
        HC_CMD_SYS_CONFIG = 0x0050,						//系统配置	stSysConfig，HC-->FSM
        HC_CMD_GRADE_INFO,						//等级设置	stGradeInfo，HC-->FSM(以上3个命令（带参数）HMI发送给所有的子系统) 品质设置发送-20150529修改

	    HC_CMD_EXIT_INFO,						//出口信息设置 stExitInfo，HC-->FSM
	    HC_CMD_WEIGHT_INFO,						//重量信息设置 stWeightBaseInfo，HC-->FSM
	    HC_CMD_PARAS_INFO,						//通道范围等参数设置 stParas，HC-->FSM
	    HC_CMD_TEST_VOLVE,						//电磁阀测试 stVolveTest，HC-->FSM
        HC_CMD_TEST_ALL_LANE_VOLVE,             //测试所有通道的同一出口 stVolveTest，HC-->FSM
        HC_CMD_RESET_AD, 						//AD归零 stResetAD
	    HC_CMD_GLOBAL_EXIT_INFO,				//全局出口信息设置 stGlobalExitInfo, HC-->FSM
	    HC_CMD_GLOBAL_WEIGHT_INFO,				//全局重量信息设置 stGlobalWeightBaseInfo, HC-->FSM
	    HC_CMD_FlAWAREA_INFO,					//瑕疵参数设置		
        HC_CMD_GLOBAL_INFO,                     //初始化配置参数设置	
        
        HC_CMD_MOTOR_INFO,						//电机使能参数 2015-5-8
        HC_CMD_COLOR_GRADE_INFO,                //品质等级设置（修改颜色） 2015-5-29
        HC_CMD_DENSITY_INFO,                    //模拟密度信息设置
        HC_CMD_BOOT_FLASH_BURN = 0x0100         //用于烧写boot程序 2015-4-7add(下传信息为一个int datalength 一个不定长度的byte[] 数组长度为datalength*512)
    }

    //FSM-->HCstFruitGradeInfo
    public enum FSM_HC_COMMAND_TYPE:int
    {
        FSM_CMD_CONFIG = 0x1000,			    //配置信息	stGlobal, FSM-->HC
        FSM_CMD_STATISTICS,					    //统计信息	stStatistics, FSM-->HC
        FSM_CMD_GRADEINFO,					    //水果实时分级信息	stFruitGradeInfo, FSM-->HC
        FSM_CMD_WEIGHTINFO,					    //重量统计信息	stWeightResult, FSM-->HC
        FSM_CMD_WAVEINFO,						//波形数据 stWaveInfo,FSM-->HC
        FSM_CMD_VERSIONERROR,				    //上位机版本与下位机版本不一致, fsmv,FSM-->HC
        FSM_CMD_BURN_FLASH_PROGRESS ,            //烧写FSM进度显示 2015-4-7add(上传信息为一个int)
        FSM_CMD_BURN_DEBUG						//FSM向上位机传输调试信息
    }

    //HC-->IPM
    public enum HC_IPM_COMMAND_TYPE:int
    {
        HC_CMD_SINGLE_SAMPLE = 0x2000,			//单张图像采集，HC-->IPM，用于传输同一水果在不同果杯中的图片
        HC_CMD_CONTINUOUS_SAMPLE_ON,			//连续采集开，HC-->IPM   由原来无参命令修改为带参命令，命令结构：cCameraNum
        HC_CMD_CONTINUOUS_SAMPLE_OFF,			//连续采集关，HC-->IPM
        HC_CMD_SHOW_BLOB_ON, 					//显示blob开，HC-->IPM
        //	HC_CMD_SHOW_BLOB_OFF,					//显示blob关，HC-->IPM	
        HC_CMD_AUTOBALANCE_ON_CAMERA,           //相机自带白平衡
        HC_CMD_AUTOBALANCE_ON,                  //启动白平衡  此命令后跟随  stWhiteBalanceParam 结构
        HC_CMD_SINGLE_SAMPLE_SPOT,				//单张采集有瑕疵的图（颜色+瑕疵两张）
        HC_CMD_TAG_BGR,							//选定标签区域的BGR均值，对应stTagBGR结构----2015.6.15
        HC_CMD_SHUTDOWN,                        //命令IPM关机 2016-12-5
        HC_CMD_SPOT_DETECT_TEST,				//发送瑕疵检测效果测试原始拼图及有用信息 只发17，ID号为272	2016.2.29
        HC_CMD_SHUTTER_ADJUST_ON,               //快门调节开   - Add by ChengSk - 20190627
        HC_CMD_SHUTTER_ADJUST_OFF               //开门调节关   - Add by ChengSk - 20190627
    }

    //IPM-->HC
    public enum IPM_HC_COMMAND_TYPE:int
    {
        IPM_CMD_IMAGE = 0x3000,  				//图像数据	stImageInfo, IPM-->HC
        //	IPM_CMD_AUTOBALANCE_MEAN,               //传送白平衡获得的均值
        IPM_CMD_AUTOBALANCE_COEFFICIENT,        //IPM向HC传输自动白平衡的R G B 均值和白平衡系数， 此命令后跟随stWhiteBalanceCoefficient结构 
        IPM_CMD_IMAGE_SPLICE,					//颜色拼图
        IPM_CMD_IMAGE_SPOT,                     //瑕疵拼图
        IPM_CMD_SHUTTER_ADJUST                  //快门调节   - Add by ChengSk - 20190627
    }
    public enum FSM_IPM_COMMAND_TYPE:int
    {
        FSM_CMD_IPM_ON = 0x4000,                //FSM与IPM连接，FSM-->IPM
        //FSM_CMD_IPM_OFF,                      //FSM与IPM断开，FSM-->IPM
        //FSM_CMD_BACK_LEARN,                   //更新背景，FSM-->IPM
        FSM_CMD_SYSTEM_INFO,                    //更新分辨率,通知IPM当前系统类型 
        FSM_CMD_PARAS_INFO,                     //FSM向IPM发送的参数信息，FSM-->IPM
        FSM_CMD_GRADE_INFO,                     //颜色区间设置，FSM-->IPM
        FSM_CMD_TIMETAG,                        //时间标，  FSM-->IPM
        FSM_CMD_FlAWAREA_INFO                   //FSM向IPM发送瑕疵参数信息,保留备用
    };
    public enum BROADCAST_SOURCE_TYPE : int
    {
        BROADCAST_SOURCE_FSM = 0x0001           //数据源来自FSM
    }
    public enum IQS_FSM_COMMAND_TYPE
    {
        IQS_CMD_FRUITINFO = 0x6000              //IQS传水果的糖度信息
    };
    public enum IPM_FSM_COMMAND_TYPE:int
    {
        IPM_CMD_FRUITINFO = 0x5000,             //水果颜色和尺寸信息, IPM-->FSM
        IPM_CMD_CONFIG,                         //配置信息, IPM-->FSM
        IPM_CMD_ON                              //IPM与FSM握手命令
    };
    public enum BROADCAST_COMMAND_TYPE : int
    {
        BROADCAST_CMD_SYSCONFIG = 0x0001,       //系统配置信息
        BROADCAST_CMD_GRADEINFO,                //系统等级信息
        BROADCAST_CMD_EXITDISPLAYINFO           //出口显示信息
    }

    public enum BROADCAST_DATA_TYPE : int
    {
        BROADCAST_DATA_STATISTICS = 0x0010      //基本统计信息（所有子系统）
    }

    //HMI-->SIM
    public enum HMI_SIM_COMMAND_TYPE : int
    {
        HMI_SIM_GRADE_INFO = 0x7100 ,        //HMI给SIM发送水果分级信息stGradeInfo包
        HMI_SIM_INSPECTION_OVER,	           //HMI给SIM发送抽检完毕命令（无参）
        HMI_SIM_DISPLAY_OUTLETS            //HMI给SIM发送抽检完毕命令（无参）


    }
    //SIM-->HMI
    public enum SIM_HMI_COMMAND_TYPE : int
    {
        SIM_HMI_DISPLAY_ON = 0x7000,            //SIM启动（无参）
        SIM_HMI_INSPECTION_ON,                  //SIM发生抽检信息给HMIstGradeInfo包
        SIM_HMI_INSPECTION_OFF                  //SIM发送停止命令给HMI（无参）

    }
    ////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////上位机-->下位机通信协议///////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    // 上位机通过命令来控制下位机的工作状态。

    // 系统配置信息,发送给每一个FSM  (HC_ID, FSM, HC_CMD_SYS_CONFIG, stSysConfig)
    [StructLayout(LayoutKind.Sequential)]
    public struct stSysConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM * 2*4)]
        public byte[] exitstate;       //出口布局 支持最多255个出口，4排 exitstate[4][ConstPreDefine.MAX_EXIT_NUM * 2]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM * ConstPreDefine.MAX_CHANNEL_NUM)]
        //public byte[] nChannelInfo;	   //子系统通道的信息 1-有效 0-无效 byte[ConstPreDefine.MAX_SUBSYS_NUM, ConstPreDefine.MAX_CHANNEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public byte[] nChannelInfo;	   //子系统通道数量，例：nChannelInfo[0]=3，代表子系统1中有3个通道
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public byte[] nImageUV;         //图像数据与紫外线相机配准int[ConstPreDefine.MAX_SUBSYS_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public byte[] nDataRegistration;//图像数据和重量数据配准int[ConstPreDefine.MAX_SUBSYS_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public byte[] nImageSugar;	   //图像数据和内部品质含糖量数据配准 int[ConstPreDefine.MAX_SUBSYS_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public byte[] nImageUltrasonic; //图像数据与超声波数据配准int[ConstPreDefine.MAX_SUBSYS_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CAMERA_NUM * 2)]
        public int[] nCameraDelay;      //最大9个相机，一个相机两个参数，interpacket-delay和transmission-delay
        public int width;						  //分辨率  宽
        public int height;						  //分辨率  高
        public int packetSize;                    //数据包的大小（传图片时使用）
        public ushort nSystemInfo;                //系统信息，低9位有效表示9个相机，1 表示相机存在  0000 0001 1111 1111 (依次：左中右NIR2，左中右NIR1，左中右COLOR)
        public byte nSubsysNum;	                  //子系统数目
        public byte nExitNum;				      //出口数目
        
        public byte nClassificationInfo;		  //低到高依次表示：CIR视觉，UV视觉，重量，内部品质，超声波，为1时有效，如：0000101表示CIR视觉+重量
        public byte multiFreq;                    //是否倍频 0位否 1位是
        public byte nCameraType;                  //相机类型（0,1,2,3），不同相机不同分辨率(共四种，scout_scA1000_30gc,ace_acA1300_60gc,ace_acA1600_60gc,ace_acA2000_50gc)
        //public byte ucIfSpotDetect;		          //为1代表进行瑕疵分选，为0表示不进行瑕疵分选
        public byte CIRClassifyType;              //低到高位：依次颜色，形状，瑕疵，为1时有效，例：00000010表示形状被勾选
        public byte UVClassifyType;               //低到高位：依次擦伤，腐烂，为1时有效，例：00000011表示擦伤腐烂同时被勾选
        public byte WeightClassifyTpye;           //低到高位：以次密度  - Add by ChengSk 20190828
        public byte InternalClassifyType;			  //糖度，酸度，硬度,(HOLLOW未用到)干物质，糖心，褐变，
        public byte UltrasonicClassifyType;       //低到高位：依次硬度，含水率，为1时有效，例：00000001表示硬度被勾选
        public byte IfWIFIEnable;                 //HMI是否启用WIFI功能 - Add by ChengSk 20190828
        public byte CheckExit;                    //抽检出口 - Add by ChengSk 20190828
        public byte CheckNum;                     //抽检数量 - Add by ChengSk 20190828
        public byte nIQSEnable;                   //是否存在IQS,00000111表示通道1  2   3   有IQS - 20191111 

        public stSysConfig(bool IsOK)
        {
             exitstate = new byte[4*ConstPreDefine.MAX_EXIT_NUM * 2];
             //nChannelInfo = new byte[ConstPreDefine.MAX_SUBSYS_NUM*ConstPreDefine.MAX_CHANNEL_NUM];
             nChannelInfo = new byte[ConstPreDefine.MAX_SUBSYS_NUM];
             nImageUV = new byte[ConstPreDefine.MAX_SUBSYS_NUM];
             nDataRegistration = new byte[ConstPreDefine.MAX_SUBSYS_NUM];
             nImageSugar = new byte[ConstPreDefine.MAX_SUBSYS_NUM];
             nImageUltrasonic = new byte[ConstPreDefine.MAX_SUBSYS_NUM];
             nCameraDelay = new int[ConstPreDefine.MAX_CAMERA_NUM * 2];
             nSubsysNum = 0;
             nExitNum = 0;
             nCameraType = 0;
             width = 0;
             height = 0;
             packetSize = 0;
             nClassificationInfo = 0;
             multiFreq = 0;
             nSystemInfo = 0;
             //ucIfSpotDetect = 0;
             CIRClassifyType = 0;
             UVClassifyType = 0;
             WeightClassifyTpye = 0;
             //InternalClassifyType = 0;
             UltrasonicClassifyType = 0;
             IfWIFIEnable = 0;
             CheckExit = 0;
             CheckNum = 0;
             nIQSEnable = 0;
             InternalClassifyType = 0;
        }
        public void ToCopy(stSysConfig Src)
        {
            Src.exitstate.CopyTo(exitstate, 0);
            Src.nChannelInfo.CopyTo(nChannelInfo, 0);
            Src.nImageUV.CopyTo(nImageUV, 0);
            Src.nDataRegistration.CopyTo(nDataRegistration, 0);
            Src.nImageSugar.CopyTo(nImageSugar, 0);
            Src.nImageUltrasonic.CopyTo(nImageUltrasonic, 0);
            Src.nCameraDelay.CopyTo(nCameraDelay, 0);
            nSubsysNum = Src.nSubsysNum;
            nExitNum = Src.nExitNum;
            nCameraType = Src.nCameraType;
            width = Src.width;
            height = Src.height;
            packetSize = Src.packetSize;
            nClassificationInfo = Src.nClassificationInfo;
            multiFreq = Src.multiFreq;
            nSystemInfo = Src.nSystemInfo;
            //ucIfSpotDetect = Src.ucIfSpotDetect;
            CIRClassifyType = Src.CIRClassifyType;
            UVClassifyType = Src.UVClassifyType;
            WeightClassifyTpye = Src.WeightClassifyTpye;
            InternalClassifyType = Src.InternalClassifyType;
            UltrasonicClassifyType = Src.UltrasonicClassifyType;
            IfWIFIEnable = Src.IfWIFIEnable;
            CheckExit = Src.CheckExit;
            CheckNum = Src.CheckNum;
            nIQSEnable = Src.nIQSEnable;
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

        public void ToCopy(stColorIntervalItem Src)
        {
            this.nMinU = Src.nMinU;
            this.nMaxU = Src.nMaxU;
            this.nMinV = Src.nMinV;
            this.nMaxV = Src.nMaxV;
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

        public void ToCopy(stPercentInfo Src)
        {
            this.nMax = Src.nMax;
            this.nMin = Src.nMin;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct stGradeItemInfo
    {
        public long exit;							    //等级所在的出口 64位，1表示在，0表示不在
        public float nMinSize;							//最小尺寸(或者体积或者重量)，上位机不同的分选标准，代表不同的量
        public float nMaxSize;						    //最大尺寸(或者体积或者重量)，上位机不同的分选标准，代表不同的量
        public int nFruitNum;							//切换个数或切换重量(装箱量)---------------现在不用其做出口的切换，只用来计算每箱重---by ymh2011-03-12
        public sbyte nColorGrade;                       //颜色等级，0x7F表示该品质不用颜色信息进行分类--new
        public sbyte sbShapeSize;                       //描述品质中的形状，1表示大于fShapeFactor，0表示小于，0x7F表示该品质未用形状信息进行分类--new
	    public sbyte sbDensity;							//描述品质中的密度，0表示第一个密度等级，......5表示第六个密度等级，0x7F表示该品质未用密度信息进行分类
        public sbyte sbFlawArea;						//描述品质中的瑕疵，0表示第一个瑕疵等级，1表示第二个瑕疵等级，2表示第三个瑕疵等级，0x7F表示该品质未用瑕疵信息进行分类
        public sbyte sbBruise;                          //描述品质中的擦伤，0表示第一个擦伤等级，1表示第二个擦伤等级，2表示第三个擦伤等级，0x7F表示该品质未用擦伤信息进行分类
        public sbyte sbRot;                             //描述品质中的腐烂，0表示第一个腐烂等级，1表示第二个腐烂等级，2表示第三个腐烂等级，0x7F表示该品质未用腐烂信息进行分类
        public sbyte sbSugar;							//描述品质中的糖度，0表示第一个糖度等级，1表示第二个糖度等级，2表示第三个糖度等级，0x7F表示该品质未用糖度信息进行分类	
        public sbyte sbAcidity;                         //描述品质中的酸度，0表示第一个酸度等级，1表示第二个酸度等级，2表示第三个酸度等级，0x7F表示该品质未用酸度信息进行分类	
        public sbyte sbHollow;                          //描述品质中的空心，0表示第一个空心等级，1表示第二个空心等级，2表示第三个空心等级，0x7F表示该品质未用空心信息进行分类	
        public sbyte sbSkin;                            //描述品质中的浮皮，0表示第一个浮皮等级，1表示第二个浮皮等级，2表示第三个浮皮等级，0x7F表示该品质未用浮皮信息进行分类
        public sbyte sbBrown;                           //描述品质中的褐变，0表示第一个褐变等级，1表示第二个褐变等级，2表示第三个褐变等级，0x7F表示该品质未用褐变信息进行分类
        public sbyte sbTangxin;                         //描述品质中的糖心，0表示第一个糖心等级，1表示第二个糖心等级，2表示第三个糖心等级，0x7F表示该品质未用糖心信息进行分类
        public sbyte sbRigidity;						//描述品质中的硬度，0表示第一个硬度等级，1表示第二个硬度等级，2表示第三个硬度等级，0x7F表示该品质未用硬度信息进行分类
        public sbyte sbWater;                           //描述品质中的含水率，0表示第一个含水率等级，1表示第二个含水率等级，2表示第三个含水率等级，0x7F表示该品质未用含水率信息进行分类
        public sbyte sbLabelbyGrade;					//某等级的贴标序号 0：不贴标，1-4：贴标机的序号（1，2，3，4）。该数据只有在贴标方式（stGradeInfo::nLabelType）为1时有效。

        public stGradeItemInfo(bool IsOK)
        {
            exit = 0;
            nMinSize = 0.0f;
            nMaxSize = 0.0f;
            nFruitNum = 0;
            nColorGrade = 0;
            sbShapeSize = 0;
            sbDensity = 0;
            sbFlawArea = 0;
            sbBruise = 0;
            sbRot = 0;
            sbSugar = 0;
            sbAcidity = 0;
            sbHollow = 0;
            sbSkin = 0;
            sbBrown = 0;
            sbTangxin = 0;
            sbRigidity = 0;
            sbWater = 0;
            sbLabelbyGrade = 0;
        }

        public void ToCopy(stGradeItemInfo Src)
        {
            this.exit = Src.exit;
            this.nMinSize = Src.nMinSize;
            this.nMaxSize = Src.nMaxSize;
            this.nFruitNum = Src.nFruitNum;
            this.nColorGrade = Src.nColorGrade;
            this.sbShapeSize = Src.sbShapeSize;
            this.sbDensity = Src.sbDensity;
            this.sbFlawArea = Src.sbFlawArea;
            this.sbBruise = Src.sbBruise;
            this.sbRot = Src.sbRot;
            this.sbSugar = Src.sbSugar;
            this.sbAcidity = Src.sbAcidity;
            this.sbHollow = Src.sbHollow;
            this.sbSkin = Src.sbSkin;
            this.sbBrown = Src.sbBrown;
            this.sbTangxin = Src.sbTangxin;
            this.sbRigidity = Src.sbRigidity;
            this.sbWater = Src.sbWater;
            this.sbLabelbyGrade = Src.sbLabelbyGrade;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stGradeInfo
    {	
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_INTERVAL_NUM)]
        public stColorIntervalItem[] intervals;	//颜色区间stColorIntervalItem[ConstPreDefine.MAX_COLOR_INTERVAL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_GRADE_NUM*ConstPreDefine.MAX_COLOR_INTERVAL_NUM)]
        public stPercentInfo[] percent;         //各个颜色等级在每个颜色区间的范围stPercentInfo[ConstPreDefine.MAX_COLOR_GRADE_NUM,ConstPreDefine.MAX_COLOR_INTERVAL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public stGradeItemInfo[] grades;	    //等级信息stGradeItemInfo[ConstPreDefine.MAX_QUALITY_GRADE_NUM,ConstPreDefine.MAX_SIZE_GRADE_NUM]	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] ExitEnabled;	            //64位，其中【0】的32位和【1】的低16位有效，表示当前出口是否有效出口---*********by ymh 2012-08-20*** int[2]
        //public int nClassifyType;										    //最低位开始，依次代表 品质，重量（克），重量（个），直径，面积，体积，第9~11位依次表示最小直径
                                                                            //最大直径和垂直直径，（这3个量仅在表示直径或者体积的位 为 1 的时候有效）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] ColorIntervals;           //存储两个滑块对应的值 int[2]		
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public int[] nExitSwitchNum;	       //int[ConstPreDefine.MAX_EXIT_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.PARAS_TAGINFO_NUM)]
        public byte[] nTagInfo;                //标签信息               
        public int nFruitType;                 //转化成十进制表示水果的种类  32*16=512种 2015-6-25 ivycc
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        //public int[] nMotorEnableSwitchNum;							//电机使能切换个数，0表示无效,by ymh 2014.12.31
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FRUIT_NAME_LENGTH)]
        public byte[] strFruitName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * 2)]
        public uint[] unFlawAreaFactor;	        //用于描述水果的瑕疵，[瑕疵面积][瑕疵个数]uint[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,2]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_BRUISE_GRADE_NUM * 2)]
        public uint[] unBruiseFactor;           //用于描述水果的擦伤，最后一个元素置0 float[ConstPreDefine.MAX_BRUISE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_ROT_GRADE_NUM * 2)]
        public uint[] unRotFactor;              //用于描述水果的腐烂，最后一个元素置0 float[ConstPreDefine.MAX_ROT_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_DENSITY_GRADE_NUM)]
        public float[] fDensityFactor;		    //用于描述水果的密度，最后一个元素置0 float[ConstPreDefine.MAX_DENSITY_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUGAR_GRADE_NUM)]
        public float[] fSugarFactor;		    //用于描述水果的糖度，最后一个元素置0 float[ConstPreDefine.MAX_SUGAR_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_ACIDITY_GRADE_NUM)]
        public float[] fAcidityFactor;          //用于描述水果的酸度，最后一个元素置0 float[ConstPreDefine.MAX_ACIDITY_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_HOLLOW_GRADE_NUM)]
        public float[] fHollowFactor;           //用于描述水果的空心，最后一个元素置0 float[ConstPreDefine.MAX_HOLLOW_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SKIN_GRADE_NUM)]
        public float[] fSkinFactor;             //用于描述水果的浮皮，最后一个元素置0 float[ConstPreDefine.MAX_SKIN_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_BROWN_GRADE_NUM)]
        public float[] fBrownFactor;            //用于描述水果的褐变，最后一个元素置0 float[ConstPreDefine.MAX_BROWN_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TANGXIN_GRADE_NUM)]
        public float[] fTangxinFactor;          //用于描述水果的糖心，最后一个元素置0 float[ConstPreDefine.MAX_TANGXIN_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_RIGIDITY_GRADE_NUM)]
        public float[] fRigidityFactor;		    //用于描述水果的硬度，最后一个元素置0 float[ConstPreDefine.MAX_RIGIDITY_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_WATER_GRADE_NUM)]
        public float[] fWaterFactor;            //用于描述水果的含水率，最后一个元素置0 float[ConstPreDefine.MAX_WATER_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SHAPE_GRADE_NUM)]
        public float[]  fShapeFactor;              //用于描述水果的形状，水果的最大直径/最小直径 2015-4-8
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        //public float[] Delay_time;			//电机启动延时，单位：S（秒）by ymh 2014.12.31 up2015-4-9
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        //public float[] Hold_time;			//电机持续时间，单位：S（秒）by ymh 2014.12.31 up2015-4-9
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SIZE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strSizeGradeName;	    	//尺寸等级名称 byte[ConstPreDefine.MAX_SIZE_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strQualityGradeName;      //品质等级名称 byte[ConstPreDefine.MAX_QUALITY_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_DENSITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stDensityGradeName;	    //密度等级名称 byte[ConstPreDefine.MAX_DENSITY_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strColorGradeName;	    //颜色等级名称 byte[ConstPreDefine.MAX_COLOR_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SHAPE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strShapeGradeName;	    //形状等级名称 byte[ConstPreDefine.MAX_SHAPE_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stFlawareaGradeName;	    //瑕疵等级名称 byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]			
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_BRUISE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stBruiseGradeName;        //擦伤等级名称 byte[ConstPreDefine.MAX_BRUISE_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_ROT_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stRotGradeName;           //腐烂等级名称 byte[ConstPreDefine.MAX_ROT_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUGAR_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stSugarGradeName;         //糖度等级名称 byte[ConstPreDefine.MAX_SUGAR_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_ACIDITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stAcidityGradeName;       //酸度等级名称 byte[ConstPreDefine.MAX_ACIDITY_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_HOLLOW_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stHollowGradeName;        //空心等级名称 byte[ConstPreDefine.MAX_HOLLOW_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SKIN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stSkinGradeName;          //浮皮等级名称 byte[ConstPreDefine.MAX_SKIN_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_BROWN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stBrownGradeName;         //褐变等级名称 byte[ConstPreDefine.MAX_BROWN_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_BROWN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        //public byte[] stHardnessGradeName;      //空心(硬度)等级名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TANGXIN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stTangxinGradeName;       //糖心等级名称 byte[ConstPreDefine.MAX_TANGXIN_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stRigidityGradeName;	    //硬度等级名称 byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_WATER_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] stWaterGradeName;	        //含水率等级名称 byte[ConstPreDefine.MAX_WATER_GRADE_NUM,ConstPreDefine.MAX_TEXT_LENGTH]			
        
        public byte ColorType;    //0000 0000，低四位有效，低四位的首位为0代表按照平均值来进行颜色分级，为1代表按照百分比来分级
                                  //后三位分别对应按照UV，H，和灰度值来作为标准，为1 时有效,当选择平均值时，将水果的结果直接赋值给color0
                                  //首位为1代表进行瑕疵分选，首位为0表示不进行瑕疵分选
        ///////////////////////////////2010.08.10 by lz, 修改协议用于增加每个页面（上下两个通道）的贴标出口数量
        public byte nLabelType;                 //贴标的工作方式选择，0：所有等级和所有出口都不贴标；1：按等级方式进行贴标；2：按出口方式进行贴标	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public byte[] nLabelbyExit;             //某出口的贴标机序号 0：不贴标，1-4：贴标机的序号（1，2，3，4）。该数据只有在贴标方式（stGradeInfo::nLabelType）为2时有效 byte[ConstPreDefine.MAX_EXIT_NUM]。
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public byte[] nSwitchLabel;			    //按照出口切换水果,nSwitchLabel 为0 的出口按照个数来切换，nSwitchLabel为1 的出口按照重量来切换 byte[ConstPreDefine.MAX_EXIT_NUM]
                                                //nSwitchLabel为2 的出口按照体积来切换,0x7F表示该出口不进行切换

        //public byte ValidDiameter;              //0 最小直径，1 最大直径，2 对称轴，3 最大弦，4 垂轴径
        public byte nSizeGradeNum;              //尺寸等级数量
        public byte nQualityGradeNum;           //品质等级数量
        public byte nClassifyType;              //分选标准:重量+大小,1表示重量（克）2表示重量（个），4表示大小（直径），8表示大小（面积），16表示大小（体积）
        public short nCheckNum;                  //抽检个数
        
        public stGradeInfo(bool IsOK)
        {
            intervals = new stColorIntervalItem[ConstPreDefine.MAX_COLOR_INTERVAL_NUM];
            percent = new stPercentInfo[ConstPreDefine.MAX_COLOR_GRADE_NUM* ConstPreDefine.MAX_COLOR_INTERVAL_NUM];
            grades = new stGradeItemInfo[ConstPreDefine.MAX_QUALITY_GRADE_NUM* ConstPreDefine.MAX_SIZE_GRADE_NUM];
            ExitEnabled = new int[2];
            //nClassifyType = 0;
            ColorIntervals = new int[2];
            nExitSwitchNum = new int[ConstPreDefine.MAX_EXIT_NUM];
            nTagInfo = new byte[ConstPreDefine.PARAS_TAGINFO_NUM];
            strFruitName = new byte[ConstPreDefine.MAX_FRUIT_NAME_LENGTH]; 
            unFlawAreaFactor = new uint[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * 2];
            fDensityFactor = new float[ConstPreDefine.MAX_DENSITY_GRADE_NUM];
            unBruiseFactor = new uint[ConstPreDefine.MAX_BRUISE_GRADE_NUM * 2];
            unRotFactor = new uint[ConstPreDefine.MAX_ROT_GRADE_NUM * 2];
            fSugarFactor = new float[ConstPreDefine.MAX_SUGAR_GRADE_NUM];
            fAcidityFactor = new float[ConstPreDefine.MAX_ACIDITY_GRADE_NUM];
            fHollowFactor = new float[ConstPreDefine.MAX_HOLLOW_GRADE_NUM];
            fSkinFactor = new float[ConstPreDefine.MAX_SKIN_GRADE_NUM];
            fBrownFactor = new float[ConstPreDefine.MAX_BROWN_GRADE_NUM];
            fTangxinFactor = new float[ConstPreDefine.MAX_TANGXIN_GRADE_NUM];
            fRigidityFactor = new float[ConstPreDefine.MAX_RIGIDITY_GRADE_NUM];
            fWaterFactor = new float[ConstPreDefine.MAX_WATER_GRADE_NUM];
            fShapeFactor = new float[ConstPreDefine.MAX_SHAPE_GRADE_NUM];
            //Delay_time = new float[ConstPreDefine.MAX_EXIT_NUM];
           // Hold_time = new float[ConstPreDefine.MAX_EXIT_NUM];
            strSizeGradeName = new byte[ConstPreDefine.MAX_SIZE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            strQualityGradeName = new byte[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stDensityGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            strColorGradeName = new byte[ConstPreDefine.MAX_COLOR_GRADE_NUM* ConstPreDefine.MAX_TEXT_LENGTH]; 
            strShapeGradeName = new byte[ConstPreDefine.MAX_SHAPE_GRADE_NUM* ConstPreDefine.MAX_TEXT_LENGTH];
            stFlawareaGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM* ConstPreDefine.MAX_TEXT_LENGTH];
            stBruiseGradeName = new byte[ConstPreDefine.MAX_BRUISE_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stRotGradeName = new byte[ConstPreDefine.MAX_ROT_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stSugarGradeName = new byte[ConstPreDefine.MAX_SUGAR_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stAcidityGradeName = new byte[ConstPreDefine.MAX_ACIDITY_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stHollowGradeName = new byte[ConstPreDefine.MAX_HOLLOW_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stSkinGradeName = new byte[ConstPreDefine.MAX_SKIN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stBrownGradeName = new byte[ConstPreDefine.MAX_BROWN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stTangxinGradeName = new byte[ConstPreDefine.MAX_TANGXIN_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            stRigidityGradeName = new byte[ConstPreDefine.MAX_FlAWAREA_GRADE_NUM* ConstPreDefine.MAX_TEXT_LENGTH];
            stWaterGradeName = new byte[ConstPreDefine.MAX_WATER_GRADE_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
            ColorType = 0;
            nLabelType = 0;
            nLabelbyExit = new byte[ConstPreDefine.MAX_EXIT_NUM];
            nSwitchLabel = new byte[ConstPreDefine.MAX_EXIT_NUM];
            
            //ValidDiameter = 0;
            nSizeGradeNum = 0;
            nQualityGradeNum = 0;
            nClassifyType = 0;
            nFruitType = 1;
            nCheckNum = 0;
            //stHardnessGradeName = new byte[] { };
        }

        public void ToCopy(stGradeInfo Src)
        {
            
            for (int i = 0; i < ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
            {
                this.intervals[i].ToCopy(Src.intervals[i]);
            }
            for (int i = 0; i < ConstPreDefine.MAX_COLOR_GRADE_NUM * ConstPreDefine.MAX_COLOR_INTERVAL_NUM; i++)
            {
                this.percent[i].ToCopy(Src.percent[i]);
            }
            for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
            {
                this.grades[i].ToCopy(Src.grades[i]);
            }
            Src.ExitEnabled.CopyTo(this.ExitEnabled, 0);
            //this.nClassifyType = Src.nClassifyType;
            Src.ColorIntervals.CopyTo(this.ColorIntervals, 0);
            Src.nExitSwitchNum.CopyTo(this.nExitSwitchNum, 0);
            Src.nTagInfo.CopyTo(this.nTagInfo, 0);
            //Src.nMotorEnableSwitchNum.CopyTo(this.nMotorEnableSwitchNum, 0);
            Src.unFlawAreaFactor.CopyTo(this.unFlawAreaFactor, 0);
            Src.fDensityFactor.CopyTo(this.fDensityFactor, 0);
            Src.unBruiseFactor.CopyTo(this.unBruiseFactor, 0);
            Src.unRotFactor.CopyTo(this.unRotFactor, 0);
            Src.fSugarFactor.CopyTo(this.fSugarFactor, 0);
            Src.fAcidityFactor.CopyTo(this.fAcidityFactor, 0);
            Src.fHollowFactor.CopyTo(this.fHollowFactor, 0);
            Src.fSkinFactor.CopyTo(this.fSkinFactor, 0);
            Src.fBrownFactor.CopyTo(this.fBrownFactor, 0);
            Src.fTangxinFactor.CopyTo(this.fTangxinFactor, 0);
            Src.fRigidityFactor.CopyTo(this.fRigidityFactor, 0);
            Src.fWaterFactor.CopyTo(this.fWaterFactor, 0);
            Src.fShapeFactor.CopyTo(this.fShapeFactor, 0);
            //Src.Delay_time.CopyTo(this.Delay_time, 0);
            //Src.Hold_time.CopyTo(this.Hold_time, 0);
            Src.strFruitName.CopyTo(this.strFruitName, 0);
            Src.strSizeGradeName.CopyTo(this.strSizeGradeName, 0);
            Src.strQualityGradeName.CopyTo(this.strQualityGradeName, 0);
            Src.stDensityGradeName.CopyTo(this.stDensityGradeName, 0);
            Src.strColorGradeName.CopyTo(this.strColorGradeName, 0);
            Src.strShapeGradeName.CopyTo(this.strShapeGradeName , 0);
            Src.stFlawareaGradeName.CopyTo(this.stFlawareaGradeName  , 0);
            Src.stBruiseGradeName.CopyTo(this.stBruiseGradeName, 0);
            Src.stRotGradeName.CopyTo(this.stRotGradeName, 0);
            Src.stSugarGradeName.CopyTo(this.stSugarGradeName, 0);
            Src.stAcidityGradeName.CopyTo(this.stAcidityGradeName, 0);
            Src.stHollowGradeName.CopyTo(this.stHollowGradeName, 0);
            Src.stSkinGradeName.CopyTo(this.stSkinGradeName, 0);
            Src.stBrownGradeName.CopyTo(this.stBrownGradeName, 0);
            Src.stTangxinGradeName.CopyTo(this.stTangxinGradeName, 0);
            Src.stRigidityGradeName.CopyTo(this.stRigidityGradeName , 0);
            Src.stWaterGradeName.CopyTo(this.stWaterGradeName, 0);
            this.ColorType = Src.ColorType;
            this.nLabelType = Src.nLabelType;
            Src.nLabelbyExit.CopyTo(this.nLabelbyExit, 0);
            Src.nSwitchLabel.CopyTo(this.nSwitchLabel , 0);
            //this.ValidDiameter = Src.ValidDiameter;
            this.nSizeGradeNum = Src.nSizeGradeNum;
            this.nQualityGradeNum = Src.nQualityGradeNum;
            this.nClassifyType = Src.nClassifyType;
            this.nFruitType = Src.nFruitType;
            this.nCheckNum = Src.nCheckNum;
        }

        public bool IsEqual(stGradeInfo Src)
        {
            bool equal = true;
            if (!this.intervals.SequenceEqual<stColorIntervalItem>(Src.intervals))
            {
                equal = false;
                return equal;
            }
            if (!this.percent.SequenceEqual<stPercentInfo>(Src.percent))
            {
                equal = false;
                return equal;
            }
            if (!this.grades.SequenceEqual<stGradeItemInfo>(Src.grades))
            {
                equal = false;
                return equal;
            }
            if (!this.ExitEnabled.SequenceEqual<int>(Src.ExitEnabled))
            {
                equal = false;
                return equal;
            }
            //if (this.nClassifyType != Src.nClassifyType)
            //{
            //    equal = false;
            //    return equal;
            //}
            if (!this.ColorIntervals.SequenceEqual<int>(Src.ColorIntervals))
            {
                equal = false;
                return equal;
            }
            if (!this.nExitSwitchNum.SequenceEqual<int>(Src.nExitSwitchNum))
            {
                equal = false;
                return equal;
            }
            //if(!this.nMotorEnableSwitchNum.SequenceEqual<int>(Src.nMotorEnableSwitchNum))
            //{
            //    equal = false;
            //    return equal;
            //}
            if (!this.unFlawAreaFactor.SequenceEqual<uint>(Src.unFlawAreaFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fDensityFactor.SequenceEqual<float>(Src.fDensityFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.unBruiseFactor.SequenceEqual<uint>(Src.unBruiseFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.unRotFactor.SequenceEqual<uint>(Src.unRotFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fSugarFactor.SequenceEqual<float>(Src.fSugarFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fAcidityFactor.SequenceEqual<float>(Src.fAcidityFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fHollowFactor.SequenceEqual<float>(Src.fHollowFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fSkinFactor.SequenceEqual<float>(Src.fSkinFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fBrownFactor.SequenceEqual<float>(Src.fBrownFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fTangxinFactor.SequenceEqual<float>(Src.fTangxinFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fRigidityFactor.SequenceEqual<float>(Src.fRigidityFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fWaterFactor.SequenceEqual<float>(Src.fWaterFactor))
            {
                equal = false;
                return equal;
            }
            if (!this.fShapeFactor.SequenceEqual<float>(Src.fShapeFactor))
            {
                equal = false;
                return equal;
            }
            //if (!this.Delay_time.SequenceEqual<float>(Src.Delay_time))
            //{
            //    equal = false;
            //    return equal;
            //}
            //if (!this.Hold_time.SequenceEqual<float>(Src.Hold_time))
            //{
            //    equal = false;
            //    return equal;
            //}
            if (!this.strSizeGradeName.SequenceEqual<byte>(Src.strSizeGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.strQualityGradeName.SequenceEqual<byte>(Src.strQualityGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stDensityGradeName.SequenceEqual<byte>(Src.stDensityGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.strColorGradeName.SequenceEqual<byte>(Src.strColorGradeName))
            {
                equal = false;
                return equal;
            }   
            if (!this.strShapeGradeName.SequenceEqual<byte>(Src.strShapeGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stFlawareaGradeName.SequenceEqual<byte>(Src.stFlawareaGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stBruiseGradeName.SequenceEqual<byte>(Src.stBruiseGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stRotGradeName.SequenceEqual<byte>(Src.stRotGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stSugarGradeName.SequenceEqual<byte>(Src.stSugarGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stAcidityGradeName.SequenceEqual<byte>(Src.stAcidityGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stHollowGradeName.SequenceEqual<byte>(Src.stHollowGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stSkinGradeName.SequenceEqual<byte>(Src.stSkinGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stBrownGradeName.SequenceEqual<byte>(Src.stBrownGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stTangxinGradeName.SequenceEqual<byte>(Src.stTangxinGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stRigidityGradeName.SequenceEqual<byte>(Src.stRigidityGradeName))
            {
                equal = false;
                return equal;
            }
            if (!this.stWaterGradeName.SequenceEqual<byte>(Src.stWaterGradeName))
            {
                equal = false;
                return equal;
            } 
            if (this.ColorType != Src.ColorType)
            {
                equal = false;
                return equal;
            }
            if (this.nLabelType != Src.nLabelType)
            {
                equal = false;
                return equal;
            }
            if (!this.nLabelbyExit.SequenceEqual<byte>(Src.nLabelbyExit))
            {
                equal = false;
                return equal;
            }
            if (!this.nSwitchLabel.SequenceEqual<byte>(Src.nSwitchLabel))
            {
                equal = false;
                return equal;
            }
            
            //if (this.ValidDiameter != Src.ValidDiameter)
            //{
            //    equal = false;
            //    return equal;
            //}
            if (this.nSizeGradeNum != Src.nSizeGradeNum)
            {
                equal = false;
                return equal;
            }
            if (this.nQualityGradeNum != Src.nQualityGradeNum)
            {
                equal = false;
                return equal;
            }
            if (this.nClassifyType != Src.nClassifyType)
            {
                equal = false;
                return equal;
            }
            if (this.nFruitType != Src.nFruitType)
            {
                equal = false;
                return equal;
            }
            return equal;
        }
   
    }

    [StructLayout(LayoutKind.Sequential)]
    public  struct stMotorInfo
    {
	    public byte   bExitId;			//出口ID号
	    public byte   bMotorSwitch;		//电机使能标志，0表示个数使能，1表示重量使能
	    public int    nMotorEnableSwitchNum;//电机使能个数，0表示无效
        public int nMotorEnableSwitchWeight;//电机使能重量，0表示无效
	    public float 	fDelay_time;//电机启动时间
        public float 	fHold_time;//电机持续时间

        public stMotorInfo(bool IsOK)
        {
            bExitId = 0;
            bMotorSwitch =0;
            nMotorEnableSwitchNum = 0;
            nMotorEnableSwitchWeight = 0;
            fDelay_time = 0;
            fHold_time = 0;
        }

        public void ToCopy(stMotorInfo Src)
        {
            this.bExitId = Src.bExitId;
            this.bMotorSwitch = Src.bMotorSwitch;
            this.nMotorEnableSwitchNum = Src.nMotorEnableSwitchNum;
            this.nMotorEnableSwitchWeight = Src.nMotorEnableSwitchWeight;
            this.fDelay_time = Src.fDelay_time;
            this.fHold_time = Src.fHold_time;
        }
    }

    // 出口信息设置,发送给选中子系统的FSM (HC_ID, IPM, HC_CMD_EXIT_INFO, stExitInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stExitItemInfo
    {
	    public short nDis;			//距离
        public short nOffset;		    //偏移
	    public short nDriverPin;	//驱动器管脚设置

        public stExitItemInfo(bool IsOK)
        {
            nDis = 0x07f;
            nOffset = 0x07f;
            nDriverPin = 0x07f;
        }
    }

    // 贴标信息设置
    [StructLayout(LayoutKind.Sequential)]
    public struct stLabelItemInfo
    {
        public short nDis;            //一个子系统的12个通道贴标机1（2，3，4）的节距是相同的
	    public short nDriverPin;	//驱动器管脚设置

        public stLabelItemInfo(bool IsOK)
        {
            nDis = 0x07f;
            nDriverPin = 0x07f;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stExitInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_LABEL_NUM)]
        public stLabelItemInfo[] labelexit;	//贴标出口 stLabelItemInfo[ConstPreDefine.MAX_LABEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public stExitItemInfo[] exits;      //stExitItemInfo[ConstPreDefine.MAX_EXIT_NUM]

        public stExitInfo(bool IsOK)
        {
            labelexit = new stLabelItemInfo[ConstPreDefine.MAX_LABEL_NUM];
            exits = new stExitItemInfo[ConstPreDefine.MAX_EXIT_NUM];
            for(int i = 0;i<ConstPreDefine.MAX_LABEL_NUM;i++)
            {
                labelexit[i] = new stLabelItemInfo(true);
            }
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                exits[i] = new stExitItemInfo(true);
            }
        }
        public void Clear()
        {
            this = new stExitInfo(true);
        }
        public void ToCopy(stExitInfo Src)
        {
            Src.labelexit.CopyTo(this.labelexit, 0);
            Src.exits.CopyTo(this.exits, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stGlobalExitInfo
    {
        public short nPulse;							//电磁阀脉宽
        public short nLabelPulse;						//贴标脉宽
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public short[] nDriverPin;      //电机输出管脚设置，每个出口一个by ymh 2014.12.31
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public float[] Delay_time;                //电机启动时间    小数点后面一位小数，最小0.1~~~最大25.0
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public float[] Hold_time;                 //电机持续时间	  小数点后面一位小数，最小0.1~~~最大25.0 //add by xcw 20200506 

        public stGlobalExitInfo(bool IsOK)
        {
            nPulse = 0;
            nLabelPulse = 0;
            nDriverPin = new short[ConstPreDefine.MAX_EXIT_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                nDriverPin[i] = 0x07f;
            }
            Delay_time = new float[ConstPreDefine.MAX_EXIT_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                Delay_time[i] = 0.0f;
            }
            Hold_time = new float[ConstPreDefine.MAX_EXIT_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                Hold_time[i] = 0.0f;
            }

        }
        public void ToCopy(stGlobalExitInfo Src)
        {
            this.nPulse = Src.nPulse;
            this.nLabelPulse = Src.nLabelPulse;
            this.nDriverPin = Src.nDriverPin;
            this.Delay_time = Src.Delay_time;
            this.Hold_time = Src.Hold_time;
        }
    }

    // 重量信息设置,发送给选中子系统的FSM (HC_ID, WM, HC_CMD_WEIGHT_INFO, stWeightBaseInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stWeightBaseInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] fGADParam;					//G-AD系数 float[2]
        public float fTemperatureParams;			//校正系数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] waveinterval;		            //波形中间组的参数byte[2]
	    

        public stWeightBaseInfo(bool IsOK)
        {
            fGADParam = new float[2];
            fTemperatureParams = 0;
            waveinterval = new byte[2];
        }

        public void ToCopy(stWeightBaseInfo Src)
        {
            Src.fGADParam.CopyTo(this.fGADParam, 0);
            this.fTemperatureParams = Src.fTemperatureParams;
            Src.waveinterval.CopyTo(this.waveinterval, 0);
        }
    }

    // 全局重量信息设置,发送给每一个FSM (HC_ID, FSM, HC_CMD_WEIGHT_INFO, stGlobalWeightBaseInfo)
    [StructLayout(LayoutKind.Sequential)]
    public  struct stGlobalWeightBaseInfo
    {
        public float fFilterParam;									//滤波系数
	    public short nMinGradeThreshold;							//最小等级阈值
	    public short nCupDeviationThreshold;						//杯偏差阈值
	    public short nCupBreakageThreshold;						    //杯破损阈值
	    public short nBaseCupNum;									//基准果杯数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SUBSYS_NUM)]
        public short[] nTotalCupNums;                               //杯总数	short[ConstPreDefine.MAX_SUBSYS_NUM]
        public short RefWeight;                                     //用于调整校正系数时的参考值 - Add by ChengSk 20190828
        public byte WeightTh;                                       //参考值的幅度范围 -  Add by ChengSk 20190828

        public stGlobalWeightBaseInfo(bool IsOK)
        {
            fFilterParam = 0;
            nMinGradeThreshold = 0;
            nCupDeviationThreshold = 0;
            nCupBreakageThreshold = 0;
            nBaseCupNum = 0;
            nTotalCupNums = new short[ConstPreDefine.MAX_SUBSYS_NUM];
            RefWeight = 0;
            WeightTh = 0;
        }

        public void ToCopy(stGlobalWeightBaseInfo Src)
        {
            this.fFilterParam = Src.fFilterParam;
            this.nMinGradeThreshold = Src.nMinGradeThreshold;
            this.nCupDeviationThreshold = Src.nCupDeviationThreshold;
            this.nCupBreakageThreshold = Src.nCupBreakageThreshold;
            this.nBaseCupNum = Src.nBaseCupNum;
            Src.nTotalCupNums.CopyTo(this.nTotalCupNums, 0);
            this.RefWeight = Src.RefWeight;
            this.WeightTh = Src.WeightTh;
        }
    }

    //通道范围等参数设置,发送给选中子系统的FSM (HC_ID, IPM, HC_CMD_PARAS_INFO, stParas)
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitCup
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] nLeft;  //只传果杯外框，自动生成果杯中心点(内部每个果杯框中心点) 2015-6-15 ivycc
	    //int nCupNum;        //真实果杯数目
	    public int nTop;
	    public int nBottom;
        public int nOffsetX;   //彩色相机相对NIR1坐标的偏移
        public int nOffsetY;	//NIR2相机相对NIR1坐标的偏移	

        public stFruitCup(bool IsOK)
        {
            nLeft = new int[2];
            nTop = 0;
            nBottom = 0;
            nOffsetX = 0;
            nOffsetY = 0;
        }
        public void ToCopy(stFruitCup Src)
        {
            Src.nLeft.CopyTo(this.nLeft, 0);
            this.nTop = Src.nTop;
            this.nBottom = Src.nBottom;
            this.nOffsetX = 0;
            this.nOffsetY = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stboot_flash
    {

        public int nLength;
        public byte pData;

        public stboot_flash(bool IsOK)
        {
            nLength = 0;
            pData = 0;
        }
        public void ToCopy(stboot_flash Src)
        {
            this.nLength = Src.nLength;
            this.pData = Src.pData;
        }
    }

    // 每个彩色相机的参数
    [StructLayout(LayoutKind.Sequential,Pack = 4)]
    public struct stCameraParas
    {
        
        public stWhiteBalanceMean MeanValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public stFruitCup[] cup;       //位置	stFruitCup[ConstPreDefine.CHANNEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public int[] nROIOffsetY;        //3.2版本根据有效区域在原图的偏移参数采集图像
        public int nTriggerDelay;      //相机延时（单位：微秒，范围0~3000）
        public int nShutter;           //快门
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public int [] nDetectionThreshold;       //彩色相机分离水果阈值
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public int[] nDetectWhiteTh;            //分离白板背景阈值   S通道  //Add by ChengSk - 20190726
        public float fGammaCorrection; //GAMMA校正
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public float[] fPixelRatio;             //像素当量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public float[] fFruitCupRangeTh;        //水果越界阈值
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public byte[] nXYEdgeBreakTh;           //水果重叠阈值
	    public byte cCameraNum;		   //保留用，相机id
        //public byte nWhiteBalanceR;	   //白平衡R分量
        //public byte nWhiteBalanceG;	   //白平衡G分量
        //public byte  nWhiteBalanceB;   //白平衡B分量
       

        public stCameraParas(bool IsOK)
        {
            MeanValue = new stWhiteBalanceMean(true);
            cup = new stFruitCup[ConstPreDefine.CHANNEL_NUM];
            nROIOffsetY = new int[ConstPreDefine.CHANNEL_NUM];
            nTriggerDelay = 0;
            nShutter = 0;
            nDetectionThreshold = new int[ConstPreDefine.CHANNEL_NUM];
            nDetectWhiteTh = new int[ConstPreDefine.CHANNEL_NUM];
            fGammaCorrection = 0;
            fPixelRatio = new float[ConstPreDefine.CHANNEL_NUM];
            for (int i = 0; i < ConstPreDefine.CHANNEL_NUM; i++)
            {
                fPixelRatio[i] = 0.000f;
            }
            fFruitCupRangeTh = new float[ConstPreDefine.CHANNEL_NUM];
            for (int i = 0; i < ConstPreDefine.CHANNEL_NUM; i++)
            {
                fFruitCupRangeTh[i] = 0.0f;
            }
            nXYEdgeBreakTh = new byte[ConstPreDefine.CHANNEL_NUM];
            cCameraNum = 0;

        }

        public void ToCopy(stCameraParas Src)
        {
            MeanValue.ToCopy(Src.MeanValue);
            Src.cup.CopyTo(this.cup, 0);
            Src.nROIOffsetY.CopyTo(this.nROIOffsetY, 0);
            this.nTriggerDelay = Src.nTriggerDelay;
            this.nShutter = Src.nShutter;
            Src.nDetectionThreshold.CopyTo(this.nDetectionThreshold,0);
            Src.nDetectWhiteTh.CopyTo(this.nDetectWhiteTh, 0);
            this.fGammaCorrection = Src.fGammaCorrection;
            this.fPixelRatio = Src.fPixelRatio;
            this.fFruitCupRangeTh = Src.fFruitCupRangeTh;
            Src.nXYEdgeBreakTh.CopyTo(this.nXYEdgeBreakTh, 0);
            this.cCameraNum = Src.cCameraNum;
            //this.nWhiteBalanceR = Src.nWhiteBalanceR;
            //this.nWhiteBalanceG = Src.nWhiteBalanceG;
            //this.nWhiteBalanceB = Src.nWhiteBalanceB;   



        }
    }

    // 每个红外相机的参数
    [StructLayout(LayoutKind.Sequential)]
    public struct stIRCameraParas
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public stFruitCup[] cup;	   //位置	stFruitCup[ConstPreDefine.CHANNEL_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public int[] nROIOffsetY;        //3.2版本根据有效区域在原图的偏移参数采集图像
        public int nTriggerDelay;      //相机延时（单位：微秒，范围0~3000）
        public int nShutter;           //快门
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public int[] nIRDetectionThreshold;		//红外相机分离阈值 2016.4.7
        public float fGammaCorrection; //GAMMA校正
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public float[] fPixelRatio;             //像素当量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]

        public float[] fFruitCupRangeTh;        //水果越界阈值
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]

        public byte[] nXYEdgeBreakTh;           //水果重叠阈值
        public byte cCameraNum;		   //保留用，相机id

        public stIRCameraParas(bool IsOK)
        {
            cup = new stFruitCup[ConstPreDefine.CHANNEL_NUM];
            nROIOffsetY = new int[ConstPreDefine.CHANNEL_NUM];
            nTriggerDelay = 0;
            nShutter = 0;
            nIRDetectionThreshold = new int[ConstPreDefine.CHANNEL_NUM];
            fGammaCorrection = 0;
            fPixelRatio = new float[ConstPreDefine.CHANNEL_NUM];
            for (int i = 0; i < ConstPreDefine.CHANNEL_NUM; i++)
            {
                fPixelRatio[i] = 0.0f;
            }
            fFruitCupRangeTh = new float[ConstPreDefine.CHANNEL_NUM];
            for (int i = 0; i < ConstPreDefine.CHANNEL_NUM; i++)
            {
                fFruitCupRangeTh[i] = 0.0f;
            }
            nXYEdgeBreakTh = new byte[ConstPreDefine.CHANNEL_NUM];
            cCameraNum = 0;
        }

        public void ToCopy(stIRCameraParas Src)
        {
            Src.cup.CopyTo(this.cup, 0);
            Src.nROIOffsetY.CopyTo(this.nROIOffsetY, 0);
            this.nTriggerDelay = Src.nTriggerDelay;
            this.nShutter = Src.nShutter;
            Src.nIRDetectionThreshold.CopyTo(this.nIRDetectionThreshold, 0);
            this.fGammaCorrection = Src.fGammaCorrection;
            this.fPixelRatio = Src.fPixelRatio;
            this.fFruitCupRangeTh = Src.fFruitCupRangeTh;
            Src.nXYEdgeBreakTh.CopyTo(this.nXYEdgeBreakTh, 0);
            this.cCameraNum = Src.cCameraNum;

        }
    }

    //每个IPM的参数
    [StructLayout(LayoutKind.Sequential)]
    public struct stParas
    {
        //public ushort nDiameterColor;	// 16位，首位无效，低位表示最小直径颜色，次位表示最大直径颜色，再位表示垂直直径颜色
        // 三者取值均为~4,对应红色、黄色、绿色、蓝色、粉色
        // 如：010 001 000  表示最小直径为红色直径，最大为黄色，垂直为绿色直径		2013.5.17
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_CAMERA_NUM)]
        public stCameraParas[] cameraParas;   //stCameraParas[ConstPreDefine.MAX_CAMERA_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_NIR_CAMERA_NUM)]
        public stIRCameraParas[] irCameraParas;   //stCameraParas[ConstPreDefine.MAX_CAMERA_NUM] 2016.4.7
        public int nCupNum;					  //果杯数目

        public stParas(bool IsOK)
        {
            cameraParas = new stCameraParas[ConstPreDefine.MAX_COLOR_CAMERA_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_COLOR_CAMERA_NUM; i++)
            {
                cameraParas[i] = new stCameraParas(true);
            }
            irCameraParas = new stIRCameraParas[ConstPreDefine.MAX_NIR_CAMERA_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_NIR_CAMERA_NUM; i++)
            {
                irCameraParas[i] = new stIRCameraParas(true);
            }
            nCupNum = 0;
        }
        public void ToCopy(stParas Src)
        {
            for (int i = 0; i < ConstPreDefine.MAX_COLOR_CAMERA_NUM; i++)
            {
                this.cameraParas[i].ToCopy(Src.cameraParas[i]);
            }
            for (int i = 0; i < ConstPreDefine.MAX_NIR_CAMERA_NUM; i++)
            {
                this.irCameraParas[i].ToCopy(Src.irCameraParas[i]);
            }
            this.nCupNum = Src.nCupNum;  
        }
    }

    //HC_CMD_SPOT_DETECT_TEST命令时发送的数据结构 2016.2.29
    [StructLayout(LayoutKind.Sequential)]
    public struct stSpotDetectTestInfo
    {
        public int nImgW;
        public int nImgH;
        //public int nCupNum;//默认值是当前上位机设置的果杯个数
        //public int nStartX;//默认是20
        //public int nFruitType;//默认值是当前上位机设置的水果类型
        //public int nCupW;		//默认是nNewCupW   如右nCupW=(nLeft[1]-nLeft[0])/nCupNum;nNewCupW=(2*nCupW+4)&0xfffe; nLeft是上位机外边界框
        
        public stSpotDetectTestInfo(bool IsOK)
        {
            nImgW = 0;
            nImgH = 0;
            //nCupNum = 0;//默认值是当前上位机设置的果杯个数
            //nStartX = 0;//默认是20
            //nFruitType = 0;//默认值是当前上位机设置的水果类型
            //nCupW = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stSpotDetectTest
    {
        public stSpotDetectTestInfo spotDetectTestInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SPLICE_IMAGE_WIDTH * ConstPreDefine.MAX_SPLICE_IMAGE_HEIGHT * 3)]//ConstPreDefine.MAX_SPLICE_IMAGE_WIDTH * ConstPreDefine.MAX_SPLICE_IMAGE_HEIGHT * 3
        public byte[] imagedataC;//BGR格式的一张拼图 固定大小MAX_SPLICE_IMAGE_WIDTH*MAX_SPLICE_IMAGE_HEIGHT*3
        public stSpotDetectTest(bool IsOK)
        {
            spotDetectTestInfo = new stSpotDetectTestInfo(true);
            imagedataC = new byte[ConstPreDefine.MAX_SPLICE_IMAGE_WIDTH * ConstPreDefine.MAX_SPLICE_IMAGE_HEIGHT * 3];
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

    //[StructLayout(LayoutKind.Sequential)]
    //public struct stYuvThresh
    //{
    //    public int nNum;								//该结构由于数据量比较大不通过FSM，EEPROM也不保存该信息，由IPM与HC各自保存一份
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_YUV_THRESH_NUM)]
    //    public stYuvRange[] stYuv;	                    //IPM启动时，读取自己目录下的TXT文档进行初始化 stYuvRange[ConstPreDefine.MAX_YUV_THRESH_NUM]

    //    public stYuvThresh(bool IsOK)
    //    {
    //        nNum = 0;
    //        stYuv = new stYuvRange[ConstPreDefine.MAX_YUV_THRESH_NUM];
    //    }
    //}								                    //该结构由HC直接读本目录下的TXT文档，然后将其传送给IPM

    //[StructLayout(LayoutKind.Sequential)]
    //public struct stLevelFeature
    //{
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_LEVEL)]
    //    public float[] fThreshold;                      //new float[ConstPreDefine.MAX_LEVEL]
    //    public int nLevelNum;                           //所分层数

    //    public stLevelFeature(bool IsOK)
    //    {
    //        fThreshold = new float[ConstPreDefine.MAX_LEVEL];
    //        nLevelNum = 0;
    //    }
    //    public void ToCopy(stLevelFeature Src)
    //    {
    //        Src.fThreshold.CopyTo(this.fThreshold,0);
    //        this.nLevelNum = Src.nLevelNum;
    //    }
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct stHuaPiGuoThresh
    //{
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //    public int[] nGrayThresh;			//灰度阈值---2012-03-27  by--ymh new int[4]
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //    public float[] fAreaRatioThresh;	//比例阈值---2012-03-27  by--ymh new float[4]	

    //    public stHuaPiGuoThresh(bool IsOK)
    //    {
    //        nGrayThresh = new int[4];
    //        fAreaRatioThresh = new float[4];
    //    }
    //    public void ToCopy(stHuaPiGuoThresh Src)
    //    {
    //        Src.nGrayThresh.CopyTo(this.nGrayThresh,0);
    //        Src.fAreaRatioThresh.CopyTo(this.fAreaRatioThresh, 0);
    //    }
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct stSpotDetectThresh
    //{
    //    public stLevelFeature stLevelFeature;
    //    //public stHuaPiGuoThresh stHuaPiGuoThresh;	   //花皮果参数 -----2012-03-27  by--ymh
    //    public int nSpotAreaMin;                     //瑕疵最小面积阈值
    //    public int nSpotAreaMax;					 //瑕疵最大面积阈值   fSpotAreaRatioMax * FruitArea
    //    public int nSpotGrayCenterGap;               //灰度检测斑点中心距离阈值
    //    public int nSpotBlueCenterGap;               //蓝色分量检测出的斑点与当前链表中已有斑点的距离阈值
    //    public int nLightAreaToZeroThresh;           //过亮区域置0阈值
    //    public int nMakeBlueBinThresh;               //由图像蓝色分量生成二值图像的阈值

    //    public int nErodeTimesForEfctOutline;        //获取轮廓要腐蚀的次数 
    //    public int nLayerNumForEdgeSpot;             //选取第几层的二值图像

    //    public int nYuvBinDilateTimes;               //经YUV空间阈值处理后二值图像的膨胀次数
    //    public int nYuvBinErodeTimes;				 //经YUV空间阈值处理后二值图像的腐蚀次数
    //    public int nBlueBinDilateTimes;              //由像素蓝色分量获得的二值图像要进行膨胀的次数
    //    public int nH_threshold_G;				     //绿色水果H分量阈值  默认40,
    //    public int nGreenFruitAreaThresh;		     //绿色水果有效面积阈值，默认200
    //    //public int nVertiOffsetRange;               //垂直偏差************************************************
    //    //public int nAngleOffsetRange;               //角度偏差     2012-03-27  by--ymh
    //    //public int nAreaOffsetRange;                //面积偏差*************************************************
    //    public float fOutlineEfectPointRatio;        //斑点轮廓合格点所占的比例

    //    public stSpotDetectThresh(bool IsOK)
    //    {
    //        stLevelFeature = new stLevelFeature(true);
    //        //stHuaPiGuoThresh = new stHuaPiGuoThresh(true);
    //        nSpotAreaMin = 0;
    //        nSpotAreaMax = 0;
    //        nSpotGrayCenterGap = 0;
    //        nSpotBlueCenterGap = 0;
    //        nLightAreaToZeroThresh = 0;
    //        nMakeBlueBinThresh = 0;
    //        nErodeTimesForEfctOutline = 0;
    //        nLayerNumForEdgeSpot = 0;
    //        nYuvBinDilateTimes = 0;
    //        nYuvBinErodeTimes = 0;
    //        nBlueBinDilateTimes = 0;
    //       // nVertiOffsetRange = 0;
    //       // nAngleOffsetRange = 0;
    //       // nAreaOffsetRange = 0;
    //        nH_threshold_G = 0;
    //        nGreenFruitAreaThresh = 0; 
    //        fOutlineEfectPointRatio = 0;
    //    }
    //    public void ToCopy(stSpotDetectThresh Src)
    //    {
    //        this.stLevelFeature.ToCopy(Src.stLevelFeature);
    //       // this.stHuaPiGuoThresh.ToCopy(Src.stHuaPiGuoThresh);
    //        this.nSpotAreaMin = Src.nSpotAreaMin;
    //        this.nSpotAreaMax = Src.nSpotAreaMax;
    //        this.nSpotGrayCenterGap = Src.nSpotGrayCenterGap;
    //        this.nSpotBlueCenterGap = Src.nSpotBlueCenterGap;
    //        this.nLightAreaToZeroThresh = Src.nLightAreaToZeroThresh;
    //        this.nMakeBlueBinThresh = Src.nMakeBlueBinThresh;
    //        this.nErodeTimesForEfctOutline = Src.nErodeTimesForEfctOutline;
    //        this.nLayerNumForEdgeSpot = Src.nLayerNumForEdgeSpot;
    //        this.nYuvBinDilateTimes = Src.nYuvBinDilateTimes;
    //        this.nYuvBinErodeTimes = Src.nYuvBinErodeTimes;
    //        this.nBlueBinDilateTimes = Src.nBlueBinDilateTimes;
    //       // this.nVertiOffsetRange = Src.nVertiOffsetRange;
    //       // this.nAngleOffsetRange = Src.nAngleOffsetRange;
    //       // this.nAreaOffsetRange = Src.nAreaOffsetRange;
    //        this.nH_threshold_G = Src.nH_threshold_G;
    //        this.nGreenFruitAreaThresh = Src.nGreenFruitAreaThresh;
    //        this.fOutlineEfectPointRatio = Src.fOutlineEfectPointRatio;
    //    }
    //}




    // 电磁阀测试, 发送给选中子系统的FSM (HC_ID, IPM, HC_CMD_TEST_VOLVE, stVolveTest)
    [StructLayout(LayoutKind.Sequential)]
    public struct stTestParam
    {
	    public byte nExitId;  //对出口号进行编码，第一列的编号数目为0到47，第二列的编号数目为48到95
						      //255时表示关闭,254时表示在用户设置里使用电磁阀测试 253-250表示1通道的贴标测试
						      //249-246 表示2通道的贴标测试

        public stTestParam(bool IsOK)
        {
            nExitId = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stVolveTest
    {
	    public byte ExitId; //电磁阀测试Id,255时表示关闭,254时表示在用户设置里使用电磁阀测试
						    //编号数目0到47表示出口，48-95代表电机出口，101到104表示4个贴标机

        public stVolveTest(bool IsOK)
        {
            ExitId = 255;
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

    //连续采集界面图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, stImageInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stImageInfo			//发送连续采集时，传输此结构	
    {
        public int ImageDataLength;		//单张图片的数据大小，YUV422时是w*h*2
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CAPTURE_WIDTH* ConstPreDefine.CAPTURE_HEIGHT*2)]
        //public char[] imagedataC;
        public int width;
        public int height;
	    public int nRouteId;
        public UInt32 unVerify;         //数据校验
        public byte ucImageFormat;	//	1表示MONO 2表示YUV422
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        //public int[] nTop;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        //public int[] nBottom;
        public stImageInfo(bool IsOK)
        {
            ImageDataLength = 0;
            width = 0;
            height = 0;
            nRouteId = 0;
            unVerify = 0;
            ucImageFormat = 0;
            //imagedataC = new char[] { };
            //nTop = new int[] { };
            //nBottom = new int[] { };
        }

        public void ToCopy(stImageInfo Src)
        {
            this.ImageDataLength = Src.ImageDataLength;
            this.width = Src.width;
            this.height = Src.height;
            this.nRouteId = Src.nRouteId;
        }

    };
    [StructLayout(LayoutKind.Sequential)]
    public struct stImageData
    {
        public stImageInfo imageInfo;
        public byte[] imagedata;		//连续采集界面
        public stImageData(int imageLenth)
        {
            imageInfo = new stImageInfo(true);
            imagedata = new byte[imageLenth];
        }
        public void ToCopy(stImageData Src)
        {
            this.imageInfo.ToCopy(Src.imageInfo);
            //this.unColorRate0 = Src.unColorRate0;
            Src.imagedata .CopyTo(this.imagedata, 0);
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct stImageInfoPreParam
    {
        public int TotalDataLength;//总数长度（包含ImageDataLength+imageInfo长度，不包含自身）
        
        //byte ImageNumber;
        stImageData imageData;
        public stImageInfoPreParam(int imageLenth)
        {
            TotalDataLength = 0;     
            //ImageNumber = 0;
            imageData = new stImageData(imageLenth);
        }
        public void ToCopy(stImageInfoPreParam Src)
        {
            this.TotalDataLength = Src.TotalDataLength;
            this.imageData.ToCopy(Src.imageData);

        }
    }

    //// 颜色设置界面图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, stImageInfo)
    //[StructLayout(LayoutKind.Sequential)]
    //public struct stImageInfo
    //{
    //    public int ImageDataLength;//图像数据长度
    //    public int nRouteId;
    //    public int width;
    //    public int height;
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
    //    //public uint[] unFlawArea;//上下两个通道 uint[ConstPreDefine.CHANNEL_NUM]
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
    //    //public uint[] unFlawNum;//uint[ConstPreDefine.CHANNEL_NUM]
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
    //    public int[] nTop;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
    //    public int[] nBottom;
    //    //public uint unColorRate0;    //平均值模式下：颜色均值
    //    //public int nStartX;
    //    //   [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SPLICE_CUP_NUM+1)]
    //    //   public int[] nLefts0;//2016-9-18 
    //    //   [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SPLICE_CUP_NUM + 1)]
    //    //   public int[] nLefts1;//2016-9-18 
    //    //   [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SPLICE_CUP_NUM + 1)]
    //    //   public int[] nLefts2;//2016-9-18 
    //    //   //int nLefts[MAX_CAMERA_DIRECTION][MAX_SPLICE_CUP_NUM+1];//102的顺序
    //    //   [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CAMERA_DIRECTION)]
    //    //public int[] nChannelH;
    //    //   public int nValidCupNum;    //add by xcw - 20191122
    //    //   public int nColorMidLen;
    //    //   public float fColorCutY;
    //    //   //public int nCupW;
    //    //   public byte bPixelBit;//1是红外 2是彩色

    //    public stImageInfo(bool IsOK)
    //    {
    //        ImageDataLength = 0;
    //        nRouteId = 0;
    //        width = 0;
    //        height = 0;
    //        //unFlawArea = new uint[ConstPreDefine.CHANNEL_NUM];
    //        //unFlawNum = new uint[ConstPreDefine.CHANNEL_NUM];
    //        nTop = new int[ConstPreDefine.CHANNEL_NUM];
    //        nBottom = new int[ConstPreDefine.CHANNEL_NUM];
    //        //unColorRate0 = 0;
    //        // nLefts0 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM + 1];
    //        // nLefts1 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM + 1];
    //        // nLefts2 = new int[ConstPreDefine.MAX_SPLICE_CUP_NUM + 1];
    //        // nChannelH = new int[ConstPreDefine.MAX_CAMERA_DIRECTION];
    //        // nValidCupNum = 0;
    //        // nColorMidLen = 0;
    //        // fColorCutY = 0.0f;
    //        //// nCupW = -1;
    //        // bPixelBit = 0;
    //        //chousile
    //    }
    //    public void ToCopy(stImageInfo Src)
    //    {
    //        this.ImageDataLength = Src.ImageDataLength;
    //        this.width = Src.width;
    //        this.height = Src.height;
    //        //Src.unFlawArea.CopyTo(this.unFlawArea, 0);
    //        //Src.unFlawNum.CopyTo(this.unFlawNum, 0);
    //        this.nRouteId = Src.nRouteId;
    //        Src.nTop.CopyTo(this.nTop, 0);
    //        Src.nBottom.CopyTo(this.nBottom, 0);
    //        //this.unColorRate0 = Src.unColorRate0;  
    //        //Array.Copy(Src.nLefts0, this.nLefts0, ConstPreDefine.MAX_SPLICE_CUP_NUM + 1);
    //        //Array.Copy(Src.nLefts1, this.nLefts1, ConstPreDefine.MAX_SPLICE_CUP_NUM + 1);
    //        //Array.Copy(Src.nLefts2, this.nLefts2, ConstPreDefine.MAX_SPLICE_CUP_NUM + 1);
    //        //Array.Copy(Src.nChannelH, this.nChannelH, ConstPreDefine.MAX_CAMERA_DIRECTION );
    //        ////  this.nCupW = Src.nCupW;
    //        //this.nValidCupNum = Src.nValidCupNum;
    //        //this.nColorMidLen = Src.nColorMidLen;
    //        //this.fColorCutY = Src.fColorCutY;
    //        //this.bPixelBit = Src.bPixelBit;
    //    }
    //}
    // 颜色设置界面图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, stSpliceImageInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stSpliceImageInfo
    {
        public int ImageDataLength;//图像数据长度
        //public int nSingleImageLen;		//单张图片的数据大小,w*h或者w*h*2+w*h
        public int width;
        public int height;
        public int nRouteId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CAMERA_DIRECTION)]
        public int[] nChannelH;
        public int nValidCupNum;    //add by xcw - 20191122
        public int nColorMidLen;
        public uint unFlawArea;//上下两个通道 uint[ConstPreDefine.CHANNEL_NUM]
        public uint unFlawNum;//uint[ConstPreDefine.CHANNEL_NUM]
        public float fColorCutY;
        public byte bPixelBit;//1是红外 2是彩色

        public stSpliceImageInfo(bool IsOK)
        {
            ImageDataLength = 0;
            //nSingleImageLen = 0;
            width = 0;
            height = 0;
            nRouteId = 0;
            nChannelH = new int[ConstPreDefine.MAX_CAMERA_DIRECTION];
            nValidCupNum = 0;
            nColorMidLen = 0;
            unFlawArea = 0;
            unFlawNum = 0;
            fColorCutY = 0.0f;
            bPixelBit = 0;

        }
        public void ToCopy(stSpliceImageInfo Src)
        {
            this.ImageDataLength = Src.ImageDataLength;
            this.width = Src.width;
            this.height = Src.height;
            this.nRouteId = Src.nRouteId;
            Array.Copy(Src.nChannelH, this.nChannelH, ConstPreDefine.MAX_CAMERA_DIRECTION);
            this.nValidCupNum = Src.nValidCupNum;
            this.nColorMidLen = Src.nColorMidLen;
            this.unFlawArea = Src.unFlawArea;
            this.unFlawNum = Src.unFlawNum;
            this.fColorCutY = Src.fColorCutY;
            this.bPixelBit = Src.bPixelBit;
        }
    }

    // 颜色设置界面图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, stImageInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stSpliceImageData
    {
        public stSpliceImageInfo imageInfo;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 2)]
        //[MarshalAs(UnmanagedType.AsAny)]
        public byte[] imagedataC;	//两张图片，一张拼图，和一张瑕疵检测图 byte[ConstPreDefine.CAPTURE_WIDTH*ConstPreDefine.CAPTURE_HEIGHT*2]

        public stSpliceImageData(int imageLenth)
        {
            imageInfo = new stSpliceImageInfo(true);
            //unColorRate0 = 0;
            imagedataC = new byte[imageLenth];
        }
        public void ToCopy(stSpliceImageData Src)
        {
            this.imageInfo.ToCopy(Src.imageInfo);
            //this.unColorRate0 = Src.unColorRate0;
            Src.imagedataC.CopyTo(this.imagedataC, 0);
        }
    }
    // 颜色设置界面图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, 上位机为了解析方便所包了一层stImageInfoPreParam)，IPM将参数都放入stImageInfo
    [StructLayout(LayoutKind.Sequential)]
    public struct stSpliceImageInfoPreParam
    {
        public int TotalDataLength;//总数长度（包含ImageDataLength+imageInfo长度，不包含自身）
        
        //byte ImageNumber;
        stSpliceImageData imageData;
        public stSpliceImageInfoPreParam(int imageLenth)
        {
            TotalDataLength = 0;     
            //ImageNumber = 0;
            imageData = new stSpliceImageData(imageLenth);
        }
        public void ToCopy(stSpliceImageInfoPreParam Src)
        {
            this.TotalDataLength = Src.TotalDataLength;
            this.imageData.ToCopy(Src.imageData);
        }
    }

    //// 瑕疵图像信息 IPM发送过来(不包含图像数据)
    //[StructLayout(LayoutKind.Sequential)]
    //public struct stSpotImageInfo
    //{
    //    //public int ImageDataLength;//图像数据长度
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 2)]
    //    public byte[] imagedataC;	//彩色相机一张拼图
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 2)]
    //    public byte[] imagedataIR;	//黑白相机一张拼图 
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
    //    public uint unFlawArea;   //上下两个通道 uint[ConstPreDefine.CHANNEL_NUM] 2015-4-14
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
    //    public uint unFlawNum;    //uint[ConstPreDefine.CHANNEL_NUM] 2015-4-14
    //    //public byte ImageNumber;//1为彩色，2为彩色与黑白
    //    public int nRouteId;
    //    public int width;
    //    public int height;

    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM * 2)]
    //    public int[] nTop;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM * 2)]
    //    public int[] nBottom;


    //    public stSpotImageInfo(int ImageLength, int ImageNumber)
    //    {
    //        //ImageDataLength = 0;
    //        // ImageNumber = 1;
    //        imagedataC = new byte[ImageLength];
    //        if (ImageNumber == 2)
    //            imagedataIR = new byte[ImageLength];
    //        else
    //            imagedataIR = new byte[0];
    //        unFlawArea = 0;
    //        unFlawNum = 0;
    //        nRouteId = 0;
    //        width = 0;
    //        height = 0;

    //        nTop = new int[ConstPreDefine.CHANNEL_NUM];
    //        nBottom = new int[ConstPreDefine.CHANNEL_NUM];

    //    }
    //    public void ToCopy(stSpotImageInfo Src)
    //    {
    //        //this.ImageDataLength = Src.ImageDataLength;
    //        // this.ImageNumber = Src.ImageNumber;
    //        if (Src.imagedataC.Length == this.imagedataC.Length && Src.imagedataC.Length != 0)
    //        {
    //            Src.imagedataC.CopyTo(this.imagedataC, 0);
    //        }
    //        if (Src.imagedataIR.Length == this.imagedataIR.Length && Src.imagedataIR.Length != 0)
    //        {
    //            Src.imagedataIR.CopyTo(this.imagedataIR, 0);
    //        }
    //        this.unFlawArea = Src.unFlawArea;
    //        this.unFlawNum = Src.unFlawNum;
    //        //Src.unFlawArea.CopyTo(this.unFlawArea, 0);
    //        //Src.unFlawNum.CopyTo(this.unFlawNum, 0);
    //        this.nRouteId = Src.nRouteId;
    //        this.width = Src.width;
    //        this.height = Src.height;
    //        Src.nTop.CopyTo(this.nTop, 0);
    //        Src.nBottom.CopyTo(this.nBottom, 0);
    //    }
    //}


    //// 瑕疵图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, stSpotImageData)
    //[StructLayout(LayoutKind.Sequential)]
    //public struct stSpotImageData
    //{
    //    public stSpotImageInfo spotImageInfo;
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 2)]
    //    public byte[] imagedataC;	//彩色相机一张拼图
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CAPTURE_WIDTH * ConstPreDefine.CAPTURE_HEIGHT * 2)]
    //    public byte[] imagedataIR;	//黑白相机一张拼图 

    //    public stSpotImageData(int ImageLength, int ImageNumber)
    //    {
    //        spotImageInfo = new stSpotImageInfo(ImageLength,ImageNumber);
    //        imagedataC = new byte[ImageLength];
    //        if (ImageNumber==2)
    //            imagedataIR = new byte[ImageLength];   
    //        else
    //            imagedataIR = new byte[0];   
    //    }
    //    public void ToCopy(stSpotImageData Src)
    //    {
    //        this.spotImageInfo.ToCopy(Src.spotImageInfo);
    //        if (Src.imagedataC.Length == this.imagedataC.Length && Src.imagedataC.Length != 0)
    //        {
    //            Src.imagedataC.CopyTo(this.imagedataC, 0);
    //        }
    //        if (Src.imagedataIR.Length == this.imagedataIR.Length && Src.imagedataIR.Length != 0)
    //        {
    //            Src.imagedataIR.CopyTo(this.imagedataIR, 0);
    //        }
    //    }
    //}



    //// 颜色图像信息 IPM发送过来(IPM, HC_ID, IPM_CMD_IMAGE, 上位机为了解析方便所包了一层stSpotImageInfoPreParam)，IPM将参数都放入stSpotImageInfo
    //[StructLayout(LayoutKind.Sequential)]
    //public struct stSpotImageInfoPreParam
    //{
    //    public int TotalDataLength;//总数长度（包含ImageDataLength+imageInfo+ImageNumber长度，不包含自身）

    //    public stSpliceImageData spotimageData;
    //    public stSpotImageInfoPreParam(int imageLenth,int imageNum)
    //    {
    //        TotalDataLength = 0;
    //        spotimageData = new stSpliceImageData(imageLenth, imageNum);
    //    }
    //    public void ToCopy(stSpotImageInfoPreParam Src)
    //    {
    //        this.TotalDataLength = Src.TotalDataLength;
    //        this.spotimageData.ToCopy(Src.spotimageData);
    //    }
    //}

    //上位机与IPM 之间进行白平衡设置时所用到的3个结构， FSM 没有用到-----by  gwf 2011-06-28
    [StructLayout(LayoutKind.Sequential)]
    public struct stWhiteBalanceMean
    {
	    public int MeanR;
	    public int MeanG;
	    public int MeanB;

        public stWhiteBalanceMean(bool IsOK)
        {
            MeanR = 0;
            MeanG = 0;
            MeanB = 0;
        }
        public void ToCopy(stWhiteBalanceMean Src)
        {
            this.MeanR = Src.MeanR;
            this.MeanG = Src.MeanG;
            this.MeanB = Src.MeanB;
        }

    }

    ///////////////////////////revised by LZ 2011.06.28
    [StructLayout(LayoutKind.Sequential)]
    public struct stWhiteBalanceParam
    {
        public stBGR BGR;      //R G B 标准值
	    public int minx;                          //区域x坐标
	    public int miny;                          //区域y坐标    
	    public int maxx;                          //区域x坐标
	    public int maxy;
        public byte WhiteBalanceCameraId;		  //当前需要进行自动白平衡的相机	0~3

        public stWhiteBalanceParam(bool IsOK)
        {
            BGR = new stBGR();
            minx = 0;
            miny = 0;
            maxx = 0;
            maxy = 0;
            WhiteBalanceCameraId = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stWhiteBalanceCoefficient
    {      //白平衡系数            
        public stBGR BGR;     //区域的R G B 实际值
        public stWhiteBalanceMean MeanValue;      //校正后的系数
        //public int CoR;                          
        //public int CoG;
        //public int CoB;

        public stWhiteBalanceCoefficient(bool IsOK)
        {
            BGR = new stBGR(true);
            MeanValue = new stWhiteBalanceMean(true);     //校正后的系数
            //CoR = 0;
            //CoG = 0;
            //CoB = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stCameraNum
    {
        public byte cCameraNum;    //连续采集时0~8（编号示意如下：）		颜色拼图时0~8（跟连续采集一致） 瑕疵拼图时是0~2（分别代表0-彩色相机 1-前边红外相机 2-后边红外相机）
						//					4 1 7					HC_CMD_SINGLE_SAMPLE、HC_CMD_SINGLE_SAMPLE_SPOT命令
						//					3 0 6
                        //					5 2 8
       // public byte bChannelIndex;    //通道 0为IPM第一通道,1为IPM第二通道
        public stCameraNum(bool IsOK)
        {
            cCameraNum = 0;
           // bChannelIndex = 0;
        }
    }

    /// <summary>
    /// 连续采集命令
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stContinousCapture
    {
        public byte cCameraNum;//连续采集时0-8相机序列
        public byte bEvenShow; //  仅在倍频下有效
        public stContinousCapture(bool IsOK)
        {
            cCameraNum = 0;
            bEvenShow = 0;
        }
    }

    /*颜色界面 标签图片标定颜色均值----2015-06-15 ivycc*/
    [StructLayout(LayoutKind.Sequential)]
    public struct stBGR
    {

        public byte bB;
        public byte bG;
        public byte bR;
        public stBGR(bool IsOK)
        {
            bB = 0;
            bG = 0;
            bR = 0;
        }
    };

    //多相机校准 - Add by ChengSk - 20190627
    [StructLayout(LayoutKind.Sequential)]
    public struct stShutterAdjust
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_CAMERA_NUM)]
        public UInt16[] colorY;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_CAMERA_NUM)]
        public UInt16[] colorH;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_CAMERA_NUM)]
        public UInt16[] nir1Y;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_COLOR_CAMERA_NUM)]
        public UInt16[] nir2Y;

        public stShutterAdjust(bool IsOK)
        {
            colorY = new UInt16[ConstPreDefine.MAX_COLOR_CAMERA_NUM];
            colorH = new UInt16[ConstPreDefine.MAX_COLOR_CAMERA_NUM];
            nir1Y = new UInt16[ConstPreDefine.MAX_COLOR_CAMERA_NUM];
            nir2Y = new UInt16[ConstPreDefine.MAX_COLOR_CAMERA_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_COLOR_CAMERA_NUM; i++)
            {
                colorY[i] = (UInt16)0;
                colorH[i] = (UInt16)0;
                nir1Y[i] = (UInt16)0;
                nir2Y[i] = (UInt16)0;
            }
        }

        public void ToCopy(stShutterAdjust Src)
        {
            for (int i = 0; i < ConstPreDefine.MAX_COLOR_CAMERA_NUM; i++)
            {
                this.colorY[i] = Src.colorY[i];
                this.colorH[i] = Src.colorH[i];
                this.nir1Y[i] = Src.nir1Y[i];
                this.nir2Y[i] = Src.nir2Y[i];
            }
        }
    };

    // 全局配置信息,用于上位机获取每一个下位机子系统的配置,FSM发送过来 (FSM, HC_ID, FSM_CMD_CONFIG, stGlobal)
    [StructLayout(LayoutKind.Sequential,Pack = 4)]
    public struct stGlobal
    {
	    // 系统配置和等级信息 每一个子系统都是相同的
	    public stSysConfig sys;							    //系统配置 （I/O）
	    public stGradeInfo grade;							//等级信息	
        public stGlobalExitInfo gexit;						//全局出口信息 （I/O）
        public stGlobalWeightBaseInfo gweight;				//全局重量信息 （I/O）
        public stAnalogDensity analogdensity;               //模拟密度信息
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CHANNEL_NUM)]
        public stExitInfo[] exit;			//出口信息stExitInfo[ConstPreDefine.MAX_CHANNEL_NUM] （I/O）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_IPM_NUM)]
        public stParas[] paras;				//IPM参数信息stParas[ConstPreDefine.MAX_IPM_NUM] （I/O）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CHANNEL_NUM)]
        public stWeightBaseInfo[] weights;	//称重参数stWeightBaseInfo[ConstPreDefine.MAX_CHANNEL_NUM] （I/O）
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_IPM_NUM)]
        //public stSpotDetectThresh[] spotdetectthresh;	    //瑕疵参数信息stSpotDetectThresh[ConstPreDefine.MAX_IPM_NUM] （I/O）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
	    public stMotorInfo[] motor;					//出口电机信息
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] cFSMInfo;                             //fsm编译日期 - Add by ChengSk - 20191111
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] cIPMInfo;                             //IPM编译日期 - Add by ChengSk - 20191111
        public int nSubsysId;								//子系统id,FSM
        public int nVersion;//版本号
	    public byte nNetState;								// 8位，低6位有效，低6位代表6个IPM，0 正常，1故障
        public byte nFsmRestart;                            //FSM是否重启，HMI根据该值判断是否保存当前批次的加工数据 1重启,HMI收到0时删除第一条数据 - Add 20190516
        public byte nFsmModule;								//当前FSM是  DSP   还是  STM32


        public stGlobal(bool IsOK)
        {
            sys = new stSysConfig(true);
            grade = new stGradeInfo(true);
            gexit = new stGlobalExitInfo(true);
            gweight = new stGlobalWeightBaseInfo(true);
            analogdensity = new stAnalogDensity(true);
            exit = new stExitInfo[ConstPreDefine.MAX_CHANNEL_NUM];
            paras = new stParas[ConstPreDefine.MAX_IPM_NUM];
            weights = new stWeightBaseInfo[ConstPreDefine.MAX_CHANNEL_NUM];
            motor= new stMotorInfo[ConstPreDefine.MAX_EXIT_NUM];
            //spotdetectthresh = new stSpotDetectThresh[ConstPreDefine.MAX_IPM_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_CHANNEL_NUM; i++)
            {
                exit[i] = new stExitInfo(true);
                weights[i] = new stWeightBaseInfo(true);
            }
            for (int i = 0; i < ConstPreDefine.MAX_IPM_NUM; i++)
            {
                paras[i] = new stParas(true);
                //spotdetectthresh[i] = new stSpotDetectThresh(true);
            }
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                motor[i] = new stMotorInfo(true);
            }
            cFSMInfo = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            cIPMInfo = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            nSubsysId = 0;
            nVersion = 0;
            nNetState = 0;
            nFsmRestart = 1;
            nFsmModule = 1;
        }
    }

    // 基本统计信息,FSM发送过来 (FSM, HC_ID, FSM_CMD_STATISTICS, stStatistics)
    [StructLayout(LayoutKind.Sequential)]
    public struct stStatistics
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public ulong[] nGradeCount;      //总个数ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public ulong[] nWeightGradeCount;//总重量ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public ulong[] nExitCount;		 //各个出口的水果个数，单位：个ulong[ConstPreDefine.MAX_EXIT_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM)]
        public ulong[] nExitWeightCount; //重量分选时各个出口的重量。单位：克 ulong[ConstPreDefine.MAX_EXIT_NUM]
        public ulong nTotalCount;		 //水果批个数
        public ulong nWeightCount;		 //水果批重量********unsigned long 对应上位机的数据类型是 __int64
	    public int nSubsysId;			 //子系统id,FSM
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public int[] nBoxGradeCount;	 //各个等级的箱数int[ConstPreDefine.MAX_QUALITY_GRADE_NUM,ConstPreDefine.MAX_SIZE_GRADE_NUM]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM)]
        public int[] nBoxGradeWeight;    //重量分选时的每箱重int[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM]       
	    public int nTotalCupNum;		 //总的果杯数
	    public int nInterval;			 //与上次发送统计信息的间隔数
	    public int nIntervalSumperminute;//一分钟内光电开关的个数,计算分选速度
	    public ushort nCupState;   		 //12个通道的果杯状态，低12位有效，最低位代表通道1，0正常，1故障	
	    public ushort nPulseInterval;    //2000以上时，分选速度为0；单位为ms
	    public ushort nUnpushFruitCount; //遗漏的水果个数,上位机每20秒调用一次	
        public byte nNetState;			 // 8位，低6位有效，低6位代表6个IPM，最低位代表IPM1，0 正常，1故障
        public byte nWeightSetting;      //重量整定标志 1 整定完毕 0 基准整定 2015-6-25 ivycc
        public byte nSCMState;           //SCM状态，0正常，1故障  2015-11-29 ivycc
        public byte nIQSNetState;        //糖度传感器网络状态，0 正常，1故障 by ymh 2018-11-05 Add by ChengSk - 20181123

        public stStatistics(bool IsOK)
        {
            nGradeCount = new ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nWeightGradeCount = new ulong[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nExitCount = new ulong[ConstPreDefine.MAX_EXIT_NUM];
            nExitWeightCount = new ulong[ConstPreDefine.MAX_EXIT_NUM];
            nTotalCount = 0;
            nWeightCount = 0;
            nSubsysId = 0;
            nBoxGradeCount = new int[ConstPreDefine.MAX_QUALITY_GRADE_NUM*ConstPreDefine.MAX_SIZE_GRADE_NUM];
            nBoxGradeWeight = new int[ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM];          
            nTotalCupNum = 0;
            nInterval = 0;
            nIntervalSumperminute = 0;
            nCupState = 0;
            nPulseInterval = 0;
            nUnpushFruitCount = 0;
            nNetState = 0;
            nWeightSetting = 0;
            nSCMState = 0;
            nIQSNetState = 0;
        }

        public void ToCopy(stStatistics Src)
        {
            for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM * ConstPreDefine.MAX_SIZE_GRADE_NUM; i++)
            {
                this.nGradeCount[i] = Src.nGradeCount[i];
                this.nWeightGradeCount[i] = Src.nWeightGradeCount[i];
                this.nBoxGradeCount[i] = Src.nBoxGradeCount[i];
                this.nBoxGradeWeight[i] = Src.nBoxGradeWeight[i];
            }
            for (int i = 0; i < ConstPreDefine.MAX_EXIT_NUM; i++)
            {
                this.nExitCount[i] = Src.nExitCount[i];
                this.nExitWeightCount[i] = Src.nExitWeightCount[i];
            }
            this.nTotalCount = Src.nTotalCount;
            this.nWeightCount = Src.nWeightCount;
            this.nSubsysId = Src.nSubsysId;
            this.nTotalCupNum = Src.nTotalCupNum;
            this.nInterval = Src.nInterval;
            this.nIntervalSumperminute = Src.nIntervalSumperminute;
            this.nCupState = Src.nCupState;
            this.nPulseInterval = Src.nPulseInterval;
            this.nUnpushFruitCount = Src.nUnpushFruitCount;
            this.nNetState = Src.nNetState;
            this.nWeightSetting = Src.nWeightSetting;
            this.nSCMState = Src.nSCMState;
            this.nIQSNetState = Src.nIQSNetState;
        }
    }


    /// 水果实时分级信息,IPM发送给FSM (IPM, HC_ID, IPM_CMD_FRUITINFO, stFruitVisionGradeInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitVisionParam
    {
	   
	    public uint unColorRate0;
	    public uint unColorRate1;
	    public uint unColorRate2;
	    public uint unArea;				    //水果的投影面积******************************************************	
	    public uint unFlawArea;			    //水果的瑕疵面积		change by ymh  2011-02-16
	    public uint unVolume;				//水果的体积	******************************************************
	    public uint unFlawNum;				//水果瑕疵斑点的个数-------------------------by ymh 2012-03-03	
        public float unMaxR;
        public float unMinR;
        /*unsigned int unPlumbR;			//垂直于对称轴的最大直径的量******************************************
        unsigned int unCentreR;				//对称轴
        unsigned int unCoreR;				//垂直对称轴且过重心的量						
        unsigned int unValue;				//用来分选的量,描述按照尺寸分选时采用的水果的某一条直径，该值为上面5条直径其中之一
        */
        //public float unSymmetryAxisD;			//对称轴                               //Note by ChengSk - 20190923
        //public float unVertiSAMaxL;				//垂直对称轴且最大的弦             //Note by ChengSk - 20190923
        //public float unVertiSACenterD;			//垂直对称轴且过重心			   //Note by ChengSk - 20190923
        //public float unDAvg;                       //平均直径      2015-06-29 ivycc  //Note by ChengSk - 20190923
        public float unSelectBasis;				//用来分选的量,描述按照尺寸分选时采用的水果的某一条直径，该值为上面5条直径其中之一
        public float fDiameterRatio;           	    //最大直径/最小直径
        public float fMinDRatio;            //猕猴桃的大面积果杯的小直径与小面积果杯的小直径之比
        public stFruitVisionParam(bool IsOK)
        {
            
            unColorRate0 = 0;
            unColorRate1 = 0;
            unColorRate2 = 0;
            unArea = 0;
            unFlawArea = 0;
            unMaxR = 0.0f;
            unMinR = 0.0f;
            //unSymmetryAxisD = 0.0f;
            //unVertiSAMaxL = 0.0f;
            //unVertiSACenterD = 0.0f;
            //unDAvg = 0.0f;
            unSelectBasis = 0.0f;
            unVolume = 0;
            unFlawNum = 0;
            fDiameterRatio = 0.0f;
            fMinDRatio = 0.0f;
        }
    }

    //水果实时分级信息，紫外线相机的IPM发送给FSM，FSM转发过来
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitUVParam
    {
        public uint unBruiseArea;             //擦伤面积
        public uint unBruiseNum;              //擦伤个数
        public uint unRotArea;                //腐烂面积
        public uint unRotNum;                 //腐烂个数
        public uint unRigidity;               //硬度
        public uint unWater;                  //含水率（硬度和含水率一起有超声波检测仪发送至UVipm）
        public UInt32 unTimeTag;

        public stFruitUVParam(bool IsOK)
        {
            unBruiseArea = 0;
            unBruiseNum = 0;
            unRotArea = 0;
            unRotNum = 0;
            unRigidity = 0;
            unWater = 0;
            unTimeTag = 0;
        }
    }

    //水果实时分级信息，NIR(含糖量检测仪)发送给FSM,FSM转发过来
    [StructLayout(LayoutKind.Sequential)]
    public struct stNIRParam
    {
        public float fSugar;                   //糖度数据
        public float fAcidity;                 //酸度数据
        public float fHollow;                  //空心数据
        public float fSkin;                    //浮皮数据
        public float fBrown;                   //褐变数据
        public float fTangxin;                 //糖心数据
        public UInt32 unTimeTag;

        public stNIRParam(bool IsOK)
        {
            fSugar = 0;
            fAcidity = 0;
            fHollow = 0;
            fSkin = 0;
            fBrown = 0;
            fTangxin = 0;
            unTimeTag = 0;
        }
    }

    /// 水果实时分级信息,FSM发送过来 (FSM, HC_ID, FSM_CMD_GRADEINFO, stFruitGradeInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitParam
    {
	    public stFruitVisionParam visionParam;
        public stFruitUVParam uvParam;
        public stNIRParam nirParam;
        public float fWeight;			    //果重
        public float fDensity;				//密度
	    public uint unGrade;			    //等级

        public stFruitParam(bool IsOK)
        {
            visionParam = new stFruitVisionParam(true);
            uvParam = new stFruitUVParam(true);
            nirParam = new stNIRParam(true);
            fWeight = 0;
            fDensity = 0;
            unGrade = 0;            
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitGradeInfo
    {

        //IPM的id号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.CHANNEL_NUM)]
        public stFruitParam[] param;		           //两个通道的水果等级信息stFruitParam[ConstPreDefine.CHANNEL_NUM]
	    //public int nDefaultDetectionThreshold;		 //默认分离水果阈值
        public int nRouteId;	
        public stFruitGradeInfo(bool IsOK)
        {
            param = new stFruitParam[ConstPreDefine.CHANNEL_NUM];
            //nDefaultDetectionThreshold = 0;
            nRouteId = 0;
        }
    }

    // 重量统计信息,FSM发送过来 (WM, HC_ID, FSM_CMD_WEIGHTINFO, stWeightResult)
    [StructLayout(LayoutKind.Sequential)]
    public struct stWeightStat
    {		
	    public float fCupAverageWeight;			//果杯均重
	    public ushort nAD0;				        //AD0通道
	    public ushort nAD1;				        //AD1通道
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
	    public ushort nADFruit;						//AD果
	    public ushort nADVehicle;					//AD车

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
        public stTrackingData data;                     //追踪数据
        public stWeightStat paras;                      //统计信息
	    public int nChannelId;							//通道id,WM
        public float fVehicleWeight0;
        public float fVehicleWeight1;
	    public byte state;								//果杯状态 0-正常 1-故障

	    ////波形数据
	    //short waveform0[3][256];				//3组
	    //short waveform1[3][256];				//3组

        public stWeightResult(bool IsOK)
        {
            data = new stTrackingData();
            paras = new stWeightStat();
            nChannelId = 0;
            fVehicleWeight0 = 0;
            fVehicleWeight1 = 0;
            state = 0;   
        }
    }

    // 波形数据,FSM发送过来 (WM, HC_ID, FSM_CMD_WAVEINFO, stWaveInfo)
    [StructLayout(LayoutKind.Sequential)]
    public struct stWaveInfo
    {
	    public int nChannelId;							//通道id,WM
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] waveform0;					    //3组 short[256]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public ushort[] waveform1;					    //3组 short[256]

        public stWaveInfo(bool IsOK)
        {
            nChannelId = 0;
            waveform0 = new ushort[256];
            waveform1 = new ushort[256];
        }
    }

    // 水果设置界面 下发到FSM命令HC_CMD_DENSITY_INFO 2015-6-29 ivycc
    [StructLayout(LayoutKind.Sequential)]
    public struct stAnalogDensity
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FRUIT_TYPE_MAJOR_CLASS_NUM)]
        public float[] uAnalogDensity;							//模拟密度，用于没有重量分选时模拟重量，单位：克/立方厘米


        public stAnalogDensity(bool IsOK)
        {
            uAnalogDensity = new float[ConstPreDefine.MAX_FRUIT_TYPE_MAJOR_CLASS_NUM];
        }

        public void ToCopy(stAnalogDensity Src)
        {
            Array.Copy(Src.uAnalogDensity, this.uAnalogDensity, ConstPreDefine.MAX_FRUIT_TYPE_MAJOR_CLASS_NUM);
        }
    }


    //上位机自定义结构体
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;
        public int nOffsetX;
        public int nOffsetY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ColorRGB
    {
        public byte ucR;
        public byte ucG;
        public byte ucB;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct QaulGradeInfo
    {
        public string QualName;
        public int ColorIndex;
        public int ShapeIndex;
        public int DensityIndex;
        public int FlawIndex;
        public int BruiseIndex;
        public int RotIndex;
        public int SugarIndex;
        public int AcidityIndex;
        public int HollowIndex;
        public int SkinIndex;
        public int BrownIndex;
        public int TangxinIndex;
        public int RigidityIndex;
        public int WaterIndex;

        public void ToCopy(QaulGradeInfo Src)
        {
            Src.QualName = string.Copy(QualName);
            ColorIndex = Src.ColorIndex;
            ShapeIndex = Src.ShapeIndex;
            DensityIndex = Src.DensityIndex;
            FlawIndex = Src.FlawIndex;
            BruiseIndex = Src.BruiseIndex;
            RotIndex = Src.RotIndex;
            SugarIndex = Src.SugarIndex;
            AcidityIndex = Src.AcidityIndex;
            HollowIndex = Src.HollowIndex;
            SkinIndex = Src.SkinIndex;
            BrownIndex = Src.BrownIndex;
            TangxinIndex = Src.TangxinIndex;
            RigidityIndex = Src.RigidityIndex;
            WaterIndex = Src.WaterIndex;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct QaulityGradeItem
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] GradeName;
        public sbyte ColorGrade;
        public sbyte sbShapeGrade;
        public sbyte sbDensity;
        public sbyte sbFlaw;
        public sbyte sbBruise;
        public sbyte sbRot;
        public sbyte sbSugar;
        public sbyte sbAcidity;
        public sbyte sbHollow;
        public sbyte sbSkin;
        public sbyte sbBrown;
        public sbyte sbTangxin;
        public sbyte sbRigidity;
        public sbyte sbWater;
        public int FruitNum;

        public QaulityGradeItem(bool IsOK)
        {
            GradeName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            ColorGrade = 0x7f;
            sbShapeGrade = 0x7f;
            sbDensity = 0x7f;
            sbFlaw = 0x7f;
            sbBruise = 0x7f;
            sbRot = 0x7f;
            sbSugar = 0x7f;
            sbAcidity = 0x7f;
            sbHollow = 0x7f;
            sbSkin = 0x7f;
            sbBrown = 0x7f;
            sbTangxin = 0x7f;
            sbRigidity = 0x7f;
            sbWater = 0x7f;
            FruitNum = 0x7f7f7f7f;
        }

        public void ToCopy(QaulityGradeItem Src)
        {
            Src.GradeName.CopyTo(GradeName,0);
            ColorGrade = Src.ColorGrade;
            sbShapeGrade = Src.sbShapeGrade;
            sbDensity = Src.sbDensity;
            sbFlaw = Src.sbFlaw;
            sbBruise = Src.sbBruise;
            sbRot = Src.sbRot;
            sbSugar = Src.sbSugar;
            sbAcidity = Src.sbAcidity;
            sbHollow = Src.sbHollow;
            sbSkin = Src.sbSkin;
            sbBrown = Src.sbBrown;
            sbTangxin = Src.sbTangxin;
            sbRigidity = Src.sbRigidity;
            sbWater = Src.sbWater;
            FruitNum = Src.FruitNum;
        }

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct QualityGradeInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_QUALITY_GRADE_NUM)]
        public QaulityGradeItem[] Item;
        public int GradeCnt;


        public QualityGradeInfo(bool IsOK)
        {
            Item = new QaulityGradeItem[ConstPreDefine.MAX_QUALITY_GRADE_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_QUALITY_GRADE_NUM; i++)
                Item[i] = new QaulityGradeItem(true);
            GradeCnt = 0;
        }


        public void ToCopy(QualityGradeInfo Src)
        {
            for(int i=0;i<ConstPreDefine.MAX_QUALITY_GRADE_NUM;i++)
                Item[i].ToCopy(Src.Item[i]);
            GradeCnt = Src.GradeCnt;
        }
    };

    /// <summary>
    /// 出口布局列表结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ExitState
    {
        public int Index;
        public int ColumnIndex;
        public int ItemIndex;
    }

    /// <summary>
    /// 软件语言
    /// </summary>
    public enum LANGUAGE_TYPE : int
    {
        Chinese = 0x0,			                //中文（默认）
        English = 0x1,                          //英文
        Spanish = 0x2                           //西班牙语 
    }

    /// <summary>
    /// 颜色界面-》颜色列表背景颜色
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stColorList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] color1;           //颜色1
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] color2;           //颜色2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] color3;           //颜色3
        public stColorList(bool IsOK)
        {
            color1 = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            color2 = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            color3 = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
        }
    }

    /// <summary>
    /// 客户信息 - Add by ChengSk - 20190929
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stClientInfo 
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CLIENTINFO_LENGTH)]
        public byte[] customerName;     //客户名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CLIENTINFO_LENGTH)]
        public byte[] farmName;         //农场名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_CLIENTINFO_LENGTH)]
        public byte[] fruitName;        //水果名称
        public stClientInfo(bool IsOK)
        {
            customerName = new byte[ConstPreDefine.MAX_CLIENTINFO_LENGTH];
            farmName = new byte[ConstPreDefine.MAX_CLIENTINFO_LENGTH];
            fruitName = new byte[ConstPreDefine.MAX_CLIENTINFO_LENGTH];
        }
    }

    /// <summary>
    /// 水果种类成员（对应IPM算法）
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitTypeMember
    {
        public int iFruitTypeID;//水果种类编号 -1代表没有编号 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FRUIT_TEXT_LENGTH)]
        public byte[] bFruitName;//水果名称 用户可自定义
        public stFruitTypeMember(bool IsOK)
        {
            iFruitTypeID = -1;
            bFruitName = new byte[ConstPreDefine.MAX_FRUIT_TEXT_LENGTH];
        }
    }

    /// <summary>
    /// 水果种类（对应IPM算法）
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stFruitType
    {
        public int iCurrentFruitNumber;//现有水果种类数目
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_FRUIT_TYPE_MAJOR_CLASS_NUM*ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM)]
        public stFruitTypeMember[] member;
        public stFruitType(bool IsOK)
        {
            iCurrentFruitNumber = 0;
            member = new stFruitTypeMember[ConstPreDefine.MAX_FRUIT_TYPE_MAJOR_CLASS_NUM*ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM];
            for (int i = 0; i < ConstPreDefine.MAX_FRUIT_TYPE_MAJOR_CLASS_NUM * ConstPreDefine.MAX_FRUIT_TYPE_SUB_CLASS_NUM; i++)
            {
                member[i] = new stFruitTypeMember(true);
            }
        }
    }



    //// 基本统计信息,HC->平板
    //[StructLayout(LayoutKind.Sequential)]
    //public struct stBroadcastStatistics
    //{
    //    public stStatistics statistics;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
    //    public byte[] strStartTime;      //分选开始时间
    //    public float fSeparationEfficiency; //分选效率
    //    public float fRealWeightCount;   //实时产量
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
    //    public byte[] strProgramName;    //分选程序名称

    //    public stBroadcastStatistics(bool IsOK)
    //    {
    //        statistics = new stStatistics(true);
    //        strStartTime = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
    //        fSeparationEfficiency = 0;
    //        fRealWeightCount = 0;
    //        strProgramName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
    //    }
    //    public void Clear()
    //    {
    //        this = new stBroadcastStatistics(true);
    //    }
    //}
    // 基本统计信息,HC->平板
    [StructLayout(LayoutKind.Sequential)]
    public struct stBroadcastStatistics
    {
        public stStatistics statistics;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strStartTime;      //分选开始时间
        public float fSeparationEfficiency; //分选效率
        public float fRealWeightCount;   //实时产量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strProgramName;    //分选程序名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_LABEL_NUM * ConstPreDefine.MAX_TEXT_LENGTH)]
        public byte[] strLabelName;    //贴标名称

        public stBroadcastStatistics(bool IsOK)
        {
            statistics = new stStatistics(true);
            strStartTime = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            fSeparationEfficiency = 0;
            fRealWeightCount = 0;
            strProgramName = new byte[ConstPreDefine.MAX_TEXT_LENGTH];
            strLabelName = new byte[ConstPreDefine.MAX_LABEL_NUM * ConstPreDefine.MAX_TEXT_LENGTH];
        }
        public void Clear()
        {
            this = new stBroadcastStatistics(true);
        }
    }

    // 平板系统信息,HC->平板
    [StructLayout(LayoutKind.Sequential)]
    public struct stBroadcastSysConfig
    {
        public stSysConfig sysConfig;
        public int nLanguage; //0 中文 1 英文
        public long exitDisplayType; //出口名称显示（最多64个出口，1表示使用“显示名称”，0表示使用“产品名称”）
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM * ConstPreDefine.MAX_EXIT_DISPALYNAME_LENGTH)]
        public byte[] strDisplayName;//出口显示名称  byte[ConstPreDefine.MAX_EXIT_NUM, ConstPreDefine.MAX_EXIT_DISPALYNAME_LENGTH]
        
        public stBroadcastSysConfig(bool IsOK)
        {
            sysConfig = new stSysConfig(true);
            nLanguage = 0;
            exitDisplayType = 0;
            strDisplayName = new byte[ConstPreDefine.MAX_EXIT_NUM * ConstPreDefine.MAX_EXIT_DISPALYNAME_LENGTH];
        }

        public void ToCopy(stBroadcastSysConfig Src)
        {
            this.sysConfig.ToCopy(Src.sysConfig);
            this.nLanguage = Src.nLanguage;
            this.exitDisplayType = Src.exitDisplayType;
            Src.strDisplayName.CopyTo(this.strDisplayName, 0);
        }
    }

    // 出口附加信息，HC->平板（在stBroadcastSysConfig数据包之后发送）
    [StructLayout(LayoutKind.Sequential)]
    public struct stExitAdditionalTextData
    {
        //public byte[] Additionaldata; //出口附加信息
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_EXIT_NUM * ConstPreDefine.MAX_EXIT_ADDITIONALNAME_LENGTH)]
        public byte[] Additionaldata;

        //public stExitAdditionalTextData(int exitNum)
        public stExitAdditionalTextData(bool IsOK)
        {
            Additionaldata = new byte[ConstPreDefine.MAX_EXIT_NUM * ConstPreDefine.MAX_EXIT_ADDITIONALNAME_LENGTH];
        }

        public void ToCopy(stExitAdditionalTextData Src)
        {
            Src.Additionaldata.CopyTo(this.Additionaldata, 0);
        }
    }

    //////////////////////////////////////////////////////////////////////
    //////////////////////      以下内部品质接口      ////////////////////

    #region 内部品质 Add by ChengSk - 20190114
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TSYS_DEV_PARAMETER
    {
        public ushort IntgTimeR;    //基准积分时间 [5-200]
        public ushort IntgTimeSL;   //大尺寸水果的积分时间 [5-200]
        public ushort IntgTimeSM;   //中尺寸水果的积分时间 [5-200]
        public ushort IntgTimeSS;   //小尺寸水果的积分时间 [5-200]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_PARA_AMO_NO)]
        public byte[] CalUsedFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_PARA_AMO_NO)]
        public TIpcSystemSlopOffsetFormat[] SlopOffset;
        public byte SmoothUsedFlag; //是否应用“平滑”
        public byte SmoothingPoint; //平滑（0-9 1-9 2-11 3-13） [索引1]
        public byte FirFilterUsedFlag;  //是否应用“FIR滤波系数”
        public float FirFilterRatio;    //FIR滤波系数 [0-1]
        public byte DerivUsedFlag;  //是否应用“计算参数”
        public byte DerivOrder;     //计算参数的计算方法（0-1 1-2） [索引0]
        public byte DerivSegSize;   //计算参数的强度（0-3 1-5 2-7 3-9） [索引1]
        public byte BaseLUsedFlag;      //是否应用“基准线”
        public ushort BaseLOffsetPoint; //基准线的基准 [700-900]
        public float BaseLOffsetMin;    //基准线的基准值 [0-3]
        public byte FruitSizeLFlag; //大尺寸水果的选中状态
        public byte FruitSizeSFlag; //小尺寸水果的选中状态
        public byte dummy3;
        public ushort FruitSizeL;   //大尺寸水果的直径 [大于15，且小于CupPitchSize，大于FruitSizeS]
        public ushort FruitSizeS;   //小尺寸水果的直径 [大于15，且小于CupPitchSize]
        public float dummy4;
        public ushort ScanCountVal; //最小水果15的积分次数 [1-50]
        public ushort Decision950nmMax; //最小水果15的最大强度 [50-40000]
        public byte ProcessingMethod;   //最小水果15的方式（0-平均 1-最佳）
        public ushort WarmupTime;   //硬件参数暖灯时间 [0-30]
        public byte dummy5;
        public ushort CupPitchSize; //硬件参数果杯节距 [40-500]
        public ushort dummy6;
        public ushort dummy7;
        public TSYS_DEV_PARAMETER(bool IsOK)
        {
            //IntgTimeR = 0;
            //IntgTimeSL = 0;
            //IntgTimeSM = 0;
            //IntgTimeSS = 0;
            //CalUsedFlag = new byte[ConstPreDefine.MAX_PARA_AMO_NO];
            //SlopOffset = new TIpcSystemSlopOffsetFormat[ConstPreDefine.MAX_PARA_AMO_NO];
            //for (int i = 0; i < SlopOffset.Length; i++)
            //{
            //    SlopOffset[i] = new TIpcSystemSlopOffsetFormat(IsOK);
            //}
            //SmoothUsedFlag = 0;
            //SmoothingPoint = 0;
            //FirFilterUsedFlag = 0;
            //FirFilterRatio = 0;
            //DerivUsedFlag = 0;
            //DerivOrder = 0;
            //DerivSegSize = 0;
            //BaseLUsedFlag = 0;
            //BaseLOffsetPoint = 0;
            //BaseLOffsetMin = 0;
            //FruitSizeLFlag = 0;
            //FruitSizeSFlag = 0;
            //dummy3 = 0;
            //FruitSizeL = 0;
            //FruitSizeS = 0;
            //dummy4 = 0;
            //ScanCountVal = 0;
            //Decision950nmMax = 0;
            //ProcessingMethod = 0;
            //WarmupTime = 0;
            //dummy5 = 0;
            //CupPitchSize = 0;
            //dummy6 = 0;
            //dummy7 = 0;

            //测试专用
            IntgTimeR = 20;
            IntgTimeSL = 20;
            IntgTimeSM = 15;
            IntgTimeSS = 10;
            CalUsedFlag = new byte[ConstPreDefine.MAX_PARA_AMO_NO];
            SlopOffset = new TIpcSystemSlopOffsetFormat[ConstPreDefine.MAX_PARA_AMO_NO];
            for (int i = 0; i < SlopOffset.Length; i++)
            {
                SlopOffset[i] = new TIpcSystemSlopOffsetFormat(IsOK);
            }
            SmoothUsedFlag = 0;
            SmoothingPoint = 9;
            FirFilterUsedFlag = 0;
            FirFilterRatio = 0.05f;
            DerivUsedFlag = 0;
            DerivOrder = 1;
            DerivSegSize = 5;
            BaseLUsedFlag = 0;
            BaseLOffsetPoint = 820;
            BaseLOffsetMin = 0.00f;
            FruitSizeLFlag = 0;
            FruitSizeSFlag = 0;
            dummy3 = 0;
            FruitSizeL = 90;
            FruitSizeS = 70;
            dummy4 = 0;
            ScanCountVal = 50;
            Decision950nmMax = 5000;
            ProcessingMethod = 0;
            WarmupTime = 10;
            dummy5 = 0;
            CupPitchSize = 95;
            dummy6 = 0;
            dummy7 = 0;
        }

        public void ToCopy(TSYS_DEV_PARAMETER Src)
        {
            this.IntgTimeR = Src.IntgTimeR;
            this.IntgTimeSL = Src.IntgTimeSL;
            this.IntgTimeSM = Src.IntgTimeSM;
            this.IntgTimeSS = Src.IntgTimeSS;
            Src.CalUsedFlag.CopyTo(this.CalUsedFlag, 0);
            Src.SlopOffset.CopyTo(this.SlopOffset, 0);
            this.SmoothUsedFlag = Src.SmoothUsedFlag;
            this.SmoothingPoint = Src.SmoothingPoint;
            this.FirFilterUsedFlag = Src.FirFilterUsedFlag;
            this.FirFilterRatio = Src.FirFilterRatio;
            this.DerivUsedFlag = Src.DerivUsedFlag;
            this.DerivOrder = Src.DerivOrder;
            this.DerivSegSize = Src.DerivSegSize;
            this.BaseLUsedFlag = Src.BaseLUsedFlag;
            this.BaseLOffsetPoint = Src.BaseLOffsetPoint;
            this.BaseLOffsetMin = Src.BaseLOffsetMin;
            this.FruitSizeLFlag = Src.FruitSizeLFlag;
            this.FruitSizeSFlag = Src.FruitSizeSFlag;
            this.dummy3 = Src.dummy3;
            this.FruitSizeL = Src.FruitSizeL;
            this.FruitSizeS = Src.FruitSizeS;
            this.dummy4 = Src.dummy4;
            this.ScanCountVal = Src.ScanCountVal;
            this.Decision950nmMax = Src.Decision950nmMax;
            this.ProcessingMethod = Src.ProcessingMethod;
            this.WarmupTime = Src.WarmupTime;
            this.dummy5 = Src.dummy5;
            this.CupPitchSize = Src.CupPitchSize;
            this.dummy6 = Src.dummy6;
            this.dummy7 = Src.dummy7;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TIpcSystemSlopOffsetFormat
    {
        public float slope;
        public float offset;
        public TIpcSystemSlopOffsetFormat(bool IsOK)
        {
            slope = 0;
            offset = 0;
        }
        public void ToCopy(TIpcSystemSlopOffsetFormat Src)
        {
            this.slope = Src.slope;
            this.offset = Src.offset;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TSYS_DEV_INFORMATION
    {
        public TMsgByte24Format ProductID;     //光谱仪信息-产品序列号
        public TMsgByte24Format ProductSerial; //光谱仪信息-产品编号
        public TMsgByte24Format SpectraSerial; //光谱仪信息-光谱仪编号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] dummy;
        public _tag_DblArray unitDblAr;  //光谱仪信息-光谱补偿（A0,A1,A2,A3,A4,A5）
        public TSYS_DEV_INFORMATION(bool IsOK)
        {
            ProductID = new TMsgByte24Format(IsOK);
            ProductSerial = new TMsgByte24Format(IsOK);
            SpectraSerial = new TMsgByte24Format(IsOK);
            dummy = new byte[20];
            unitDblAr = new _tag_DblArray(IsOK);
        }

        public void ToCopy(TSYS_DEV_INFORMATION Src)
        {
            ProductID.ToCopy(Src.ProductID);
            ProductSerial.ToCopy(Src.ProductSerial);
            SpectraSerial.ToCopy(Src.SpectraSerial);
            Src.dummy.CopyTo(this.dummy, 0);
            this.unitDblAr.ToCopy(Src.unitDblAr);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TMsgByte24Format
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] buf;
        public TMsgByte24Format(bool IsOK)
        {
            buf = new byte[24];
        }

        public void ToCopy(TMsgByte24Format Src)
        {
            Src.buf.CopyTo(this.buf, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct _tag_DblArray
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] DblArray;
        public _tag_DblArray(bool IsOK)
        {
            DblArray = new float[6];
        }
        public void ToCopy(_tag_DblArray Src)
        {
            Src.DblArray.CopyTo(this.DblArray, 0);
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TSYS_AMO_PARAMETER
    {
        public float B0ArrayData;
        public TMsgSingle151Format BArrayBuffer;
        public TSYS_AMO_PARAMETER(bool IsOK)
        {
            B0ArrayData = 0;
            BArrayBuffer = new TMsgSingle151Format(IsOK);
        }
        public void ToCopy(TSYS_AMO_PARAMETER Src)
        {
            this.B0ArrayData = Src.B0ArrayData;
            this.BArrayBuffer.ToCopy(Src.BArrayBuffer);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TMsgSingle151Format
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_SPLINE_NO)]
        public float[] buf;
        public TMsgSingle151Format(bool IsOK)
        {
            buf = new float[ConstPreDefine.MAX_SPLINE_NO];
        }
        public void ToCopy(TMsgSingle151Format Src)
        {
            Src.buf.CopyTo(this.buf, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TMsgSingle1024Format
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_PIXEL_NO)]
        public float[] buf;
        public TMsgSingle1024Format(bool IsOK)
        {
            buf = new float[ConstPreDefine.MAX_PIXEL_NO];
        }
        public void ToCopy(TMsgSingle1024Format Src)
        {
            Src.buf.CopyTo(this.buf, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TMsgWord1024Format
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_PIXEL_NO)]
        public ushort[] buf;
        public TMsgWord1024Format(bool IsOK)
        {
            buf = new ushort[ConstPreDefine.MAX_PIXEL_NO];
        }
        public void ToCopy(TMsgWord1024Format Src)
        {
            Src.buf.CopyTo(this.buf, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TIpcSystemSpecAbsDataFormat
    {
        public byte aCnt;
        public byte vCnt;
        public byte kCnt;
        public ushort CupInterval;
        public ushort FruitSize;
        public ushort IntgTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ConstPreDefine.MAX_PARA_AMO_NO)]
        public float[] cal;
        public TMsgSingle151Format abs;
        public TIpcSystemSpecAbsDataFormat(bool IsOK)
        {
            aCnt = 0;
            vCnt = 0;
            kCnt = 0;
            CupInterval = 0;
            FruitSize = 0;
            IntgTime = 0;
            cal = new float[ConstPreDefine.MAX_PARA_AMO_NO];
            abs = new TMsgSingle151Format(IsOK);
        }

        public void ToCopy(TIpcSystemSpecAbsDataFormat Src)
        {
            this.aCnt = Src.aCnt;
            this.vCnt = Src.vCnt;
            this.kCnt = Src.kCnt;
            this.CupInterval = Src.CupInterval;
            this.FruitSize = Src.FruitSize;
            this.IntgTime = Src.IntgTime;
            Src.cal.CopyTo(this.cal, 0);
            this.abs.ToCopy(Src.abs);
        }
      
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TIpcSystemStatusDataFormat
    {
        public byte mstate;
        public byte conn;
        public ushort CupInterval;
        public byte mDarkSts;
        public byte mRefSts;
        public TIpcSystemStatusDataFormat(bool IsOK)
        {
            mstate = 0;
            conn = 0;
            CupInterval = 0;
            mDarkSts = 0;
            mRefSts = 0;
        }

        public void ToCopy(TIpcSystemStatusDataFormat Src)
        {
            this.mstate = Src.mstate;
            this.conn = Src.conn;
            this.CupInterval = Src.CupInterval;
            this.mDarkSts = Src.mDarkSts;
            this.mRefSts = Src.mRefSts;
        }
    }

    public class SendCommand
    {
        private byte _msgtype;
        private byte[] _msgdata;
        private int _interval = 1;
        public byte MsgType
        {
            set { _msgtype = value; }
            get { return _msgtype; }
        }
        public byte[] MsgData
        {
            set { _msgdata = value; }
            get { return _msgdata; }
        }
        public int Interval
        {
            set { _interval = value; }
            get { return _interval; }
        }
    }
    #endregion
}
