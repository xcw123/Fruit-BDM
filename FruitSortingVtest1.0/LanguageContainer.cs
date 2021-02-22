using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FruitSortingVtest1
{
    public class LanguageContainer
    {
        /// <summary>
        /// 获取语言版本的索引
        /// </summary>
        /// <param name="strLan">语言简称</param>
        /// <returns></returns>
        public static int LanguageVersionIndex(string strLan)
        {
            int Index = 0;
            switch(strLan)
            {
                case "zh":
                    Index = 0;
                    break;
                case "en":
                    Index = 1;
                    break;
                case "es":
                    Index = 2;
                    break;
                default:
                    break;
            }
            return Index;
        }
        
        #region AciditySetForm.cs
        
        public static string[] AciditySetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] AciditySetFormMessagebox1Text = new string[3]{
            "酸度等级名称不能为空！",
            "Acidity grade name cannot be empty!",
            "¡El nombre de la clase de acidez no puede estar vacío!"
        };
        public static string[] AciditySetFormMessagebox2Text = new string[3]{
            "酸度等级名称不能重名！",
            "Acidity grade name cannot be duplicated!",
            "¡El nombre de la clase de acidez no puede estar duplicado!"
        };
        public static string[] AciditySetFormMessagebox3Sub1Text = new string[3]{
            "酸度等级第",
            "The Brix degree of the acidity grade in the line",
            "¡El grado Brix de la clase acidez en la línea"
        };
        public static string[] AciditySetFormMessagebox3Sub2Text = new string[3]{
            "行的含糖量应大于第",
            "should be greater than the Brix degree in the line",
            "debe ser mayor que el grado Brix en la línea"
        };
        public static string[] AciditySetFormMessagebox3Sub3Text = new string[3]{
            "行的含糖量！",
            "!",
            "!"
        };
        #endregion

        #region BootFlashBurnForm.cs

        public static string[] BootFlashBurnFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] BootFlashBurnFormMessagebox1Text = new string[3]{
            "请输入数据长度！",
            "Pls input data length!",
            "Introducir longitud datos!"
        };
        public static string[] BootFlashBurnFormMessagebox2Text = new string[3]{
            "烧写成功！",
            "writing successfully!",
            "escrito con éxito!"
        };

        #endregion

        #region BrownSetForm.cs

        public static string[] BrownSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] BrownSetFormMessagebox1Text = new string[3]{
            "褐变等级名称不能为空！",
            "The browning grade name cannot be empty!",
            "¡El nombre de la clase pardeamiento no puede estar vacío!"
        };
        public static string[] BrownSetFormMessagebox2Text = new string[3]{
            "褐变等级名称不能重名！",
            "The browning grade name cannot be duplicated!",
            "¡El nombre de la clase pardeamiento no puede estar duplicado!"
        };
        public static string[] BrownSetFormMessagebox3Sub1Text = new string[3]{
            "褐变等级第",
            "The browning parameter of the browning grade in the line",
            "¡El parámetro pardeamiento de la clase pardeamiento en la línea"
        };
        public static string[] BrownSetFormMessagebox3Sub2Text = new string[3]{
            "行的褐变参数应大于第",
            "should be greater than the browning parameter in the line",
            "debe ser mayor que el parámetro pardeamiento en la línea"
        };
        public static string[] BrownSetFormMessagebox3Sub3Text = new string[3]{
            "行的褐变参数！",
            "!",
            "!"
        };
        #endregion

        #region BruiseSetForm.cs

        public static string[] BruiseSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] BruiseSetFormMessagebox1Text = new string[3]{
            "擦伤等级名称不能为空！",
            "The bruise grade name cannot be empty! ",
            "¡El nombre de la clase contusión no puede estar vacío!"
        };
        public static string[] BruiseSetFormMessagebox2Text = new string[3]{
            "擦伤等级名称不能重名！",
            "The bruise grade name cannot be duplicated!",
            "¡El nombre de la clase contusión no puede estar duplicado!"
        };
        public static string[] BruiseSetFormMessagebox6Sub1Text = new string[3]{
            "擦伤等级第",
            "The bruise area of the bruise grade in the line",
            "¡El parámetro contusión de la clase contusión en la línea"
        };
        public static string[] BruiseSetFormMessagebox6Sub2Text = new string[3]{
            "行的擦伤面积应大于第",
            "should be greater than the bruise area in the line",
            "debe ser mayor que el parámetro contusión en la línea"
        };
        public static string[] BruiseSetFormMessagebox6Sub3Text = new string[3]{
            "行的擦伤面积！",
            "!",
            "!"
        };
        public static string[] BruiseSetFormMessagebox6Sub4Text = new string[3]{
            "擦伤等级第",
            "The bruise quantity of the bruise grade in the line",
            "¡La cantidad contusión de la clase contusión en la línea"
        };
        public static string[] BruiseSetFormMessagebox6Sub5Text = new string[3]{
            "行的擦伤个数应大于第",
            "should be greater than the bruise quantity in the line",
            "debe ser mayor que la cantidad contusión en la línea"
        };
        public static string[] BruiseSetFormMessagebox6Sub6Text = new string[3]{
            "行的擦伤个数！",
            "!",
            "!"
        };
        #endregion

        #region ChannelExit.cs

        public static string[] ChannelExitMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] ChannelExitMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] ChannelExitMessagebox1Text = new string[3]{
            "只能输入A，B，C！",
            "Can only input A, B, C! ",
            "Solo puede introducir A, B, C! "
        };
        public static string[] ChannelExitMessagebox2Text = new string[3]{
            "只能输入0 ~ 18！",
            "Can only input 0 ~ 18! ",
            "¡Solo puede introducir 0 ~ 18! "
        };
        public static string[] ChannelExitMessagebox3Text = new string[3]{
            "通道出口设置界面保存配置出错！",
            "The save of configuration of lane & outlet setting interface is wrong!",
            "¡Los ajustes guardados en la configuración de líneas y salidas son incorrectos!"
        };
        public static string[] ChannelExitMessagebox4Sub1Text = new string[3]{
            "贴标",
            "The driver pin value setting of labelling",
            "¡El valor del controlador ajustado para la etiquetadora"
        };
        public static string[] ChannelExitMessagebox4Sub2Text = new string[3]{
            "驱动器管脚值设置重复！",
            "is repeated!",
            "está repetido!"
        };
        public static string[] ChannelExitMessagebox4Sub3Text = new string[3]{
            "出口",
            "The driver pin value setting of outlet",
            "¡El valor del controlador ajustado para la salida"
        };
        public static string[] ChannelExitMessagebox4Sub4Text = new string[3]{
            "驱动器管脚值设置重复！",
            "is repeated!",
            "está repetido!"
        };
        #endregion

        #region ChannelRange.cs

        public static string[] ChannelRangeMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] ChannelRangeMessagebox1Text = new string[3]{
            "宽度或高度不能为零！",
            "Width or height cannot be 0!",
            "¡Ancho o altura no puede ser 0!"
        };
        public static string[] ChannelRangeMessagebox2Text = new string[3]{
            "黑白相机果杯坐标位置超出范围！",
            "The cup coordinate position of black white camera is out of range!",
            "¡La posición coordinada de la taza de la cámara b/n  está fuera de rango!"
        };
        public static string[] ChannelRangeMessagebox3Text = new string[3]{
            "通道范围设置界面出错：",
            "Lane range setting interface is wrong: ",
            "Ajuste rango de líneas erróneo: "
        };
        public static string[] ChannelRangeMessagebox4Text = new string[3]{
            "通道范围设置界面出错：通道未选择！",
            "Lane range setting interface is wrong: lane unselected!",
            "Ajuste rango de líneas erróneo: ¡línea no selecionada!"
        };
        public static string[] ChannelRangeMessagebox5Text = new string[3]{
            "有效区域偏移超出范围！",
            "The effective area offset is out of range!",
            "Desviación efectiva de la zona!"
        };
        public static string[] ChannelRangeMessagebox6Text = new string[3]{
            "所选区域小于64*32！",
            "The selected area is less than 64 * 32!",
            "Área seleccionada inferior a 64 * 32！"
        };
        public static string[] ChannelRangeMessagebox7Text = new string[3]{
            "坐标越界，且不给nOffsetX和nOffsetY赋当前值！",
            "The cup coordinate position of black white camera is out of range!",
            "¡La posición coordinada de la taza de la cámara b/n  está fuera de rango!"
        };
        #endregion

        #region ClientInfoUpdateForm.cs

        public static string[] ClientInfoUpdateFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] ClientInfoUpdateFormRangeMessagebox1Text = new string[3]{
            "更新客户信息失败！",
            "Client information updating failed!",
            "¡La actualización de la información del cliente falló!"
        };
        #endregion

        #region ColorSetForm.cs

        public static string[] ColorSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] ColorSetFormMessagebox1Text = new string[3]{
            "参数设置无效！颜色列表第1行的值要比第2行的值大！",
            "Parameter setting is invalid! the value in the line 1 of the color table should be greater than the value in the line 2!",
            "¡Parámetro inválido! El valor en la línea 1 de la tabla de color debe ser mayor que el valor en la línea 2!"
        };
        public static string[] ColorSetFormMessagebox2Text = new string[3]{
            "颜色等级名称不能为空！",
            "Color grade name cannot be empty! ",
            "¡El nombre de la clase Color no puede estar vacío! "
        };
        public static string[] ColorSetFormMessagebox3Text = new string[3]{
            "颜色等级名称不能重名！",
            "Color grade name cannot be duplicated!",
            "¡El nombre de la clase Color no puede estar duplicado!"
        };
        public static string[] ColorSetFormMessagebox3Sub1Text = new string[3]{
            "第",
            "Parameter setting error in row ",
            "¡Error parámetro en línea "
        };
        public static string[] ColorSetFormMessagebox3Sub2Text = new string[3]{
            "行、第",
            ", column ",
            ", columna "
        };
        public static string[] ColorSetFormMessagebox3Sub3Text = new string[3]{
            "列参数设置错误！",
            "!",
            "!"
        };
        #endregion

        #region DatabaseSetForm.cs

        public static string[] DatabaseSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] DatabaseSetFormMessagebox1Text = new string[3]{
            "数据库连接成功！",
            "Database is connected successfully!",
            "¡Conexión con éxito a Base de Datos!"
        };
        public static string[] DatabaseSetFormMessagebox2Text = new string[3]{
            "数据库连接失败！",
            "Database connecting failed!",
            "¡Fallo conexión con Base de Datos!"
        };
        #endregion

        #region DensitySetForm.cs

        public static string[] DensitySetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] DensitySetFormMessagebox1Text = new string[3]{
            "密度等级名称不能为空！",
            "Density grade name cannot be empty! ",
            "¡El nombre de la clase Densidad no puede estar vacío!"
        };
        public static string[] DensitySetFormMessagebox2Text = new string[3]{
            "密度等级名称不能重名！",
            "Density grade name cannot be duplicated! ",
            "¡El nombre de la clase Densidad no puede estar duplicado!"
        };
        public static string[] DensitySetFormMessagebox3Sub1Text = new string[3]{
            "密度等级第",
            "The density value of the density grade in the line",
            "¡El valor densidad de la clase densidad en la línea"
        };
        public static string[] DensitySetFormMessagebox3Sub2Text = new string[3]{
            "行的密度值应大于第",
            "should be greater than the density value in the line",
            "debe ser mayor que el valor densidad en la línea"
        };
        public static string[] DensitySetFormMessagebox3Sub3Text = new string[3]{
            "行的密度值！",
            "!",
            "!"
        };
        #endregion

        #region FlawSetForm.cs

        public static string[] FlawSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] FlawSetFormMessagebox1Text = new string[3]{
            "瑕疵等级名称不能为空！",
            "Blemish grade name cannot be empty!",
            "¡El nombre de la clase Defecto no puede estar vacío!"
        };
        public static string[] FlawSetFormMessagebox2Text = new string[3]{
            "瑕疵等级名称不能重名！",
            "Blemish grade name cannot be duplicated!",
            "¡El nombre de la clase Defecto no puede estar duplicado!"
        };
        public static string[] FlawSetFormMessagebox6Sub1Text = new string[3]{
            "瑕疵等级第",
            "The blemish area of the blemish grade in the line",
            "¡El área de defecto de la clase defecto en la línea"
        };
        public static string[] FlawSetFormMessagebox6Sub2Text = new string[3]{
            "行的瑕疵面积应大于第",
            "should be greater than the blemish area in the line",
            "debe ser mayor que el área de defecto en la línea"
        };
        public static string[] FlawSetFormMessagebox6Sub3Text = new string[3]{
            "行的瑕疵面积！",
            "!",
            "!"
        };
        public static string[] FlawSetFormMessagebox6Sub4Text = new string[3]{
            "瑕疵等级第",
            "The blemish quantity of the blemish grade in the line",
            "¡La cantidad de defecto de la clase defecto en la línea"
        };
        public static string[] FlawSetFormMessagebox6Sub5Text = new string[3]{
            "行的瑕疵个数应大于第",
            "should be greater than the blemish quantity in the line",
            "debe ser mayor que la cantidad de defecto en la línea"
        };
        public static string[] FlawSetFormMessagebox6Sub6Text = new string[3]{
            "行的瑕疵个数！",
            "!",
            "!"
        };
        #endregion

        #region GradeSetForm.cs

        public static string[] GradeSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] GradeSetFormMessagebox1Text = new string[3]{
            "请选择分选标准（尺寸/重量）！",
            "Pls choose sorting standard(size/weight)!",
            "¡Elige clasificación estándar (tamaño/peso)!"
        };
        public static string[] GradeSetFormMessagebox7Sub1Text = new string[3]{
            "第",
            "Grade name in the line",
            "¡El nombre de clase en línea"
        };
        public static string[] GradeSetFormMessagebox7Sub2Text = new string[3]{
            "行的等级名称不能为空！",
            "cannot be empty!",
            "no puede estar vacío!"
        };
        public static string[] GradeSetFormMessagebox7Sub3Text = new string[3]{
            "第",
            "The grade name of the line",
            "¡El nombre de clase en línea"
        };
        public static string[] GradeSetFormMessagebox7Sub4Text = new string[3]{
            "行与第",
            "and the line",
            "y línea"
        };
        public static string[] GradeSetFormMessagebox7Sub5Text = new string[3]{
            "行等级名称重名！",
            "are duplicated!",
            "está duplicado!"
        };
        public static string[] GradeSetFormMessagebox7Sub6Text = new string[3]{
            "等级列表中第",
            "The line",
            "¡La línea"
        };
        public static string[] GradeSetFormMessagebox7Sub7Text = new string[3]{
            "行为无效值！",
            "in the grade table is invalid!",
            "en la tabla de clases no es válida! "
        };
        #endregion

        #region HollowSetForm.cs

        public static string[] HollowSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] HollowSetFormMessagebox1Text = new string[3]{
            "硬度等级名称不能为空！",
            "the name of hardness grades can not be empty!",
            "¡El nombre de la clase dureza no puede estar vacío!"
        };
        public static string[] HollowSetFormMessagebox2Text = new string[3]{
            "硬度等级名称不能重名！",
            "The name of hardness grades can not be duplicated!",
            "¡El nombre de clase dureza no puede estar duplicado!"
        };
        public static string[] HollowSetFormMessagebox3Sub1Text = new string[3]{
            "硬度等级第",
            "the  line",
            "el parámetro de la línea"
        };
        public static string[] HollowSetFormMessagebox3Sub2Text = new string[3]{
            "行的参数应大于第",
            "parameter in the hardness grades shall be greater than the line",
            "en la clase dureza debe ser mayor que el parámetro de la linea"
        };
        public static string[] HollowSetFormMessagebox3Sub3Text = new string[3]{
            "行！",
            "parameter!",
            ""
        };

        #endregion

        #region LanguageSelectForm.cs

        public static string[] LanguageSelectFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] LanguageSelectFormMessageboxQuestionCaption = new string[3] { 
            "询问信息", 
            "Question message", 
            "Question message" 
        };
        public static string[] LanguageSelectFormMessagebox1Text = new string[3]{
            "版本语言切换成功，重启软件后生效，是否立即重启？",
            "Version language switch successfully,and take effect after restarting the software, whether to restart immediately?",
            "El éxito de la transición después de la entrada en vigor de la versión lingüística, reiniciar el software, Si reiniciar inmediatamente?"
        };

        #endregion

        #region LoadConfigNewForm.cs

        public static string[] LoadConfigNewFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] LoadConfigNewFormMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] LoadConfigNewFormMessagebox1Text = new string[3]{
            "请选择配置文件！",
            "Pls choose the configuration file!",
            "¡Selecciona el archivo de configuración!"
        };
        public static string[] LoadConfigNewFormMessagebox2Text = new string[3]{
            "请选择正确的配置文件！",
            "Pls choose the correct configuration file!",
            "¡Seleccionar archivo configuración correcto!"
        };
        public static string[] LoadConfigNewFormMessagebox3Text = new string[3]{
            "载入配置失败！",
            "Configuration loading failed!",
            "¡Fallo cargando configuración! "
        };
        public static string[] LoadConfigNewFormMessagebox4Text = new string[3]{
            "确定要删除选中的配置文件？",
            "Do you want to delete the selected configuration file?",
            "¿Desea eliminar el archivo de configuración seleccionado?"
        };
        public static string[] LoadConfigNewFormMessagebox6Sub1Text = new string[3]{
            "版本不匹配！上位机是V",
            "Different program version! HMI version is V",
            "¡Diferente versión programa! versión HMI es V"
        };
        public static string[] LoadConfigNewFormMessagebox6Sub2Text = new string[3]{
            "，下位机是V",
            ", FSM version is V",
            ", versión FSM es V"
        };
        public static string[] LoadConfigNewFormMessagebox6Sub3Text = new string[3]{
            "！",
            "!",
            "!"
        };
        public static string[] LoadConfigNewFormMessagebox6Sub4Text = new string[3]{
            "版本不匹配！上位机是V",
            "Different program version! HMI version is V",
            "¡Diferente versión programa! versión HMI es V"
        };
        public static string[] LoadConfigNewFormMessagebox6Sub5Text = new string[3]{
            "，文件是V",
            ", File version is V",
            ", versión Archivo es V"
        };
        public static string[] LoadConfigNewFormMessagebox6Sub6Text = new string[3]{
            "！",
            "!",
            "!"
        };
        #endregion

        #region MainForm.cs

        public static string[] MainFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] MainFormMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] MainFormMessageboxWarningCaption = new string[3] { 
            "警告信息", 
            "Warning message", 
            "Warning message" 
        };
        public static string[] MainFormMessageboxQuestionCaption = new string[3] { 
            "询问信息", 
            "Question message", 
            "Question message" 
        };
        public static string[] MainFormMessagebox1Text = new string[3]{
            "等级只存在于当前出口，无法禁用！",
            "Grade only existing in current outlet cannot be prohibited!",
            "¡Clase solo existente en salida actual no puede ser eliminada!"
        };
        public static string[] MainFormMessagebox2Text = new string[3]{
            "拖拽太快，抛出异常！",
            "Exception appear while the mouse dragging slide too fast!",
            "¡Aparecerá una excepción mientras el ratón arrastra la diapositiva demasiado rápido!"
        };
        public static string[] MainFormMessagebox3Text = new string[3]{
            "是否保存配置信息？",
            "Do you want to save this configuration?",
            "¿Quieres Guardar esta configuración?"
        };
        public static string[] MainFormMessagebox4Text = new string[3]{
            "当前状态无法结束加工！",
            "Batch cannot be ended in the current state!",
            "¡No se puede finalizar el Lote en el estado actual!"
        };
        public static string[] MainFormMessagebox5Text = new string[3]{
            "是否结束本批次加工，确认后本批次数据将清零，自动生成表格，不能再进行修改！",
            "Do you want to end this batch? After you confirm, the batch data will be automatically generated in the Excel file, cannot be modify any more!",
            "¿Quieres finalizar este lote? Después de finalizar, se generara automaticamente un archivo Excel con los datos del lote, no se podrán modificar más!"
        };
        public static string[] MainFormMessagebox6Text = new string[3]{
            "更新水果信息完成状态时失败！",
            "Failed to update the fruit information completion status!",
            "¡Fallo al actualizar información de fruto  completion status!"
        };
        public static string[] MainFormMessagebox7Text = new string[3]{
            "插入等级信息时失败！",
            "Failed to insert grade information!",
            "¡Fallo al insertar información de clase!"
        };
        public static string[] MainFormMessagebox8Text = new string[3]{
            "插入出口信息时失败！",
            "Failed to insert outlet information!",
            "¡Fallo al insertar información de salida!"
        };
        public static string[] MainFormMessagebox9Text = new string[3]{
            "本次加工结束！",
            "This batch of sorting is over!",
            "¡Este lote de clasificación ha terminado!"
        };
        public static string[] MainFormMessagebox10Text = new string[3]{
            "初始状态数据为空，不能打印！",
            "Initial status data is empty, cannot print!",
            "¡Los datos están vacíos, no se pueden imprimir!"
        };
        public static string[] MainFormMessagebox11Text = new string[3]{
            "加工进行中，不能进行统计！",
            "The batch of sorting is on, cannot do the statistics!",
            "¡Lote de clasificación activado, no se puede hacer estadisticas!"
        };
        public static string[] MainFormMessagebox12Text = new string[3]{
            "当前选择的水果信息为空！",
            "The currently selected fruit information is empty!",
            "¡La información de Fruto seleccionada esta vacía!"
        };
        public static string[] MainFormMessagebox13Text = new string[3]{
            "当前选择的等级信息为空！",
            "The currently selected grade information is empty!",
            "¡La información de Clase seleccionada esta vacía!"
        };
        public static string[] MainFormMessagebox14Text = new string[3]{
            "当前选择的出口信息为空！",
            "The currently selected outlet information is empty!",
            "¡La información de Salida seleccionada esta vacía!"
        };
        public static string[] MainFormMessagebox15Text = new string[3]{
            "是否要清空此出口中的所有等级？",
            "Do you want to clear all the grades of this exit?",
            "Quieres borrar todas las clases de esta salida?"
        };
        public static string[] MainFormMessagebox16Text = new string[3]{
            "修改等级信息时失败！",
            "Failed to update grade information!",
            "¡Fallo al Modificación información de clase!"
        };
        public static string[] MainFormMessagebox17Text = new string[3]{
            "修改出口信息时失败！",
            "Failed to update outlet information!",
            "¡Fallo al Modificación información de salida!"
        };
        public static string[] MainFormMessagebox18Text = new string[3]{
            "是否确定清空出口？",
            "Are you sure you want to clear the exit?",
            "¿Está segura de vaciar la salida?"
        };

        public static string[] MainFormMessagebox19Text = new string[3]{
            "参数发送失败！",
            "The currently selected exit start command was not sent successfully！",
            "Orden de inicio de salida seleccionada actualmente！"
        };
       
        #endregion

        #region MainMenu.cs

        public static string[] MainMenuMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] MainMenuMessagebox1Text = new string[3]{
            "是否覆盖原来的配置信息？",
            "Do you want to overwrite the previous configuration information?",
            "¿Quieres sobreescribir la anterior información de configuración?"
        };
        public static string[] MainMenuMessagebox2Text = new string[3]{
            "配置备份成功！",
            "Configuration back up successfully!",
            "Backup configuración realizado con éxito!"
        };
        public static string[] MainMenuMessagebox3Text = new string[3]{
            "配置备份失败！",
            "Configuration backup failed!",
            "¡Fallo backup configuración!"
        };
        public static string[] MainMenuMessagebox4Text = new string[3]{
            "配置恢复成功！",
            "Configuration recovery succeed!",
            "¡Recuperación configuración con éxito!"
        };
        public static string[] MainMenuMessagebox5Text = new string[3]{
            "配置恢复失败！",
            "Configuration recovery failed!",
            "¡Fallo recuperación configuración!"
        };
        public static string[] MainMenuMessagebox6Text = new string[3]{
            "确定进行数据清零操作？",
            "Do you want to zero clearing the data?",
            "¿Quieres poner datos a cero?"
        };
        #endregion

        #region ProcessInfoForm.cs

        public static string[] ProcessInfoFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] ProcessInfoFormMessageboxWarningCaption = new string[3] { 
            "警告信息", 
            "Warning message", 
            "Warning message" 
        };
        public static string[] ProcessInfoFormMessagebox1Text = new string[3]{
            "查询条件不能为空！",
            "Query condition cannot be empty! ",
            "¡Las condiciones de búsqueda no puede estar vacías!"
        };
        public static string[] ProcessInfoFormMessagebox2Text = new string[3]{
            "请先选择要统计的条目！",
            "Pls choose the item to be counted!",
            "¡ Elige el artículo que se contará!"
        };
        public static string[] ProcessInfoFormMessagebox3Text = new string[3]{
            "加工进行中，不能进行统计！",
            "The batch of sorting is on, cannot do the statistics! ",
            "¡El lote esta activo, no se puede realizar las estadísticas!"
        };
        public static string[] ProcessInfoFormMessagebox4Text = new string[3]{
            "当前选择的水果信息为空！",
            "The currently selected fruit information is empty!",
            "¡La información de fruto seleccionada esta vacía!"
        };
        public static string[] ProcessInfoFormMessagebox5Text = new string[3]{
            "当前选择的等级信息为空！",
            "The currently selected grade information is empty!",
            "¡La información de clase seleccionada esta vacía!"
        };
        public static string[] ProcessInfoFormMessagebox6Text = new string[3]{
            "当前选择的出口信息为空！",
            "The currently selected outlet information is empty!",
            "¡La información de salida seleccionada esta vacía!"
        };
        #endregion

        #region ProjectSetForm.cs

        public static string[] ProjectSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] ProjectSetFormMessagebox1Text = new string[3]{
            "是否保存配置信息？",
            "Do you want to save this configuration?",
            "¿ Quieres guardar esta configuración?"
        };
        #endregion

        #region QualGradeSetForm.cs

        public static string[] QualGradeSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] QualGradeSetFormMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] QualGradeSetFormMessagebox1Text = new string[3]{
            "等级名称不能为空！",
            "Grade name cannot be empty!",
            "¡Nombre de clase no puede estar vacío!"
        };
        public static string[] QualGradeSetFormMessagebox2Text = new string[3]{
            "等级名称不能重复！",
            "Grade name cannot be duplicated!",
            "¡Nombre de clase no puede estar duplicado!"
        };
        public static string[] QualGradeSetFormMessagebox3Text = new string[3]{
            "参数保存出错，还要关闭吗？",
            "Parameter saving is wrong, continue to close it?",
            "Guardado parámetros incorrecto, ¿continuar para cerrar?"
        };
        #endregion

        #region QualityParamSetForm.cs

        public static string[] QualityParamSetFormMessageboxQuestionCaption = new string[3] { 
            "询问信息", 
            "Question message", 
            "Question message" 
        };
        public static string[] QualityParamSetFormMessagebox1Text = new string[3]{
            "参数保存出错，还要关闭吗？",
            "Parameter saving is wrong, continue to close it?",
            "Guardado parámetros incorrecto, ¿continuar para cerrar?"
        };
        #endregion

        #region QualitySetForm.cs

        public static string[] QualitySetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] QualitySetFormMessagebox1Text = new string[3]{
            "等级名称不能为空！",
            "Grade name cannot be empty!",
            "¡Nombre de clase no puede estar vacío!"
        };
        public static string[] QualitySetFormMessagebox2Text = new string[3]{
            "等级名称长度超过限制！",
            "The length of grade name exceeds the limit!",
            "¡La longitud del nombre de clase supera el límite!"
        };
        #endregion

        #region RigiditySetForm.cs

        public static string[] RigiditySetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] RigiditySetFormMessagebox1Text = new string[3]{
            "硬度等级名称不能为空！",
            "Hardness grade name cannot be empty!",
            "¡Nombre de clase dureza no puede estar vacío!"
        };
        public static string[] RigiditySetFormMessagebox2Text = new string[3]{
            "硬度等级名称不能重名！",
            "Hardness grade name cannot be duplicated!",
            "¡Nombre de clase dureza no puede estar duplicado!"
        };
        public static string[] RigiditySetFormMessagebox3Sub1Text = new string[3]{
            "硬度等级第",
            "The hardness value of the hardness grade in the line",
            "¡El valor dureza de la clase dureza en la línea"
        };
        public static string[] RigiditySetFormMessagebox3Sub2Text = new string[3]{
            "行的硬度值应大于第",
            "should be greater than the hardness value in the line",
            "debe ser mayor que el valor dureza en la línea"
        };
        public static string[] RigiditySetFormMessagebox3Sub3Text = new string[3]{
            "行的硬度值！",
            "!",
            "!"
        };
        #endregion

        #region RotSetForm.cs

        public static string[] RotSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] RotSetFormMessagebox1Text = new string[3]{
            "腐烂等级名称不能为空！",
            "Decay grade name cannot be empty!",
            "¡Nombre de clase podrido no puede estar vacío!"
        };
        public static string[] RotSetFormMessagebox2Text = new string[3]{
            "腐烂等级名称不能重名！",
            "Decay grade name cannot be duplicated!",
            "¡Nombre de clase podrido no puede estar duplicado!"
        };
        public static string[] RotSetFormMessagebox6Sub1Text = new string[3]{
            "腐烂等级第",
            "The decay area of the decay grade in the line",
            "¡El área podrido de la clase podrido en la línea"
        };
        public static string[] RotSetFormMessagebox6Sub2Text = new string[3]{
            "行的腐烂面积应大于第",
            "should be greater than the decay area in the line",
            "debe ser mayor que el área podrido en la línea"
        };
        public static string[] RotSetFormMessagebox6Sub3Text = new string[3]{
            "行的腐烂面积！",
            "!",
            "!"
        };
        public static string[] RotSetFormMessagebox6Sub4Text = new string[3]{
            "腐烂等级第",
            "The decay quantity of the decay grade in the line",
            "¡La cantidad podrido de la clase podrido en la línea"
        };
        public static string[] RotSetFormMessagebox6Sub5Text = new string[3]{
            "行的腐烂个数应大于第",
            "should be greater than the decay quantity in the line",
            "debe ser mayor que la cantidad podrido en la línea"
        };
        public static string[] RotSetFormMessagebox6Sub6Text = new string[3]{
            "行的腐烂个数！",
            "!",
            "!"
        };
        #endregion

        #region SaveConfigForm.cs

        public static string[] SaveConfigFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] SaveConfigFormMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] SaveConfigFormMessagebox1Text = new string[3]{
            "配置文件名称不能为空！",
            "File name of the configuration cannot be empty!",
            "¡El nombre del archivo de configuración no puede estar vacío!"
        };
        public static string[] SaveConfigFormMessagebox2Text = new string[3]{
            "是否覆盖原来的配置信息？",
            "Do you want to overwrite the previous configuration information?",
            "¿Quieres sobreescribir la anterior información de configuración?"
        };
        public static string[] SaveConfigFormMessagebox3Text = new string[3]{
            "保存配置失败！",
            "The save of configuration failed!",
            "¡El guardado de la configuración fallo!"
        };
        #endregion

        #region SeparationLogForm.cs

        public static string[] SeparationLogFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] SeparationLogFormMessagebox1Text = new string[3]{
            "时间间隔不能超过31天！",
            "Time interval cannot exceed 31 days!",
            "¡El intervalo de tiempono puede exceder de 31 dias!"
        };

        #endregion

        #region ShapeSetFormNew.cs

        public static string[] ShapeSetFormNewMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] ShapeSetFormNewMessagebox1Text = new string[3]{
            "形状等级名称不能为空！",
            "The shape grade name cannot be empty!",
            "¡Nombre de clase forma no puede estar vacío!"
        };
        public static string[] ShapeSetFormNewMessagebox2Text = new string[3]{
            "形状等级名称不能重名！",
            "The shape grade name cannot be duplicated!",
            "¡Nombre de clase forma no puede estar duplicado!"
        };
        public static string[] ShapeSetFormNewMessagebox3Sub1Text = new string[3]{
            "形状等级第",
            "The parameter value of the shape grade in the line",
            "¡El valor de la clase forma en la línea"
        };
        public static string[] ShapeSetFormNewMessagebox3Sub2Text = new string[3]{
            "行的参数值应大于第",
            "should be greater than the parameter value in the line",
            "debe ser mayor que el valor en la línea"
        };
        public static string[] ShapeSetFormNewMessagebox3Sub3Text = new string[3]{
            "行！",
            "!",
            "!"
        };
        #endregion

        #region SkinSetForm.cs

        public static string[] SkinSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] SkinSetFormMessagebox1Text = new string[3]{
            "干物质等级名称不能为空！",
            "The name of dry matter grades can not be empty!",
            "¡El nombre de la clase materia seca no puede estar vacío!"
        };
        public static string[] SkinSetFormMessagebox2Text = new string[3]{
            "干物质等级名称不能重名！",
            "The dry matter grade name cannot be duplicated!",
            "¡El nombre de la clase materia seca no puede estar duplicado!"
        };
        public static string[] SkinSetFormMessagebox3Sub1Text = new string[3]{
            "干物质等级第",
            "the parameter value of dry matter in line",
            "el valor del parámetro materia seca en la linea"
        };
        public static string[] SkinSetFormMessagebox3Sub2Text = new string[3]{
            "行的参数值应大于第",
            "shall be greater than line",
            "dbe ser mayor que en la linea"
        };
        public static string[] SkinSetFormMessagebox3Sub3Text = new string[3]{
            "行！",
            "!",
            "!"
        };
        #endregion

        #region StatisticsInfoForm1.cs

        public static string[] StatisticsInfoForm1MessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] StatisticsInfoForm1Messagebox1Text = new string[3]{
            "是否覆盖原来的配置信息？",
            "Do you want to overwrite the previous configuration information?",
            "¿Quieres sobreescribir la anterior información de configuración?"
        };
        public static string[] StatisticsInfoForm1Messagebox2Text = new string[3]{
            "导出Excel表格成功！",
            "Export excel report succesfully!",
            "Reporte Excel exportado correctamente!"
        };
        public static string[] StatisticsInfoForm1Messagebox3Text = new string[3]{
            "图片保存成功！",
            "Image save successfully!",
            "Image save successfully!"
        };
        #endregion

        #region StatisticsInfoForm2.cs

        public static string[] StatisticsInfoForm2MessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] StatisticsInfoForm2Messagebox1Text = new string[3]{
            "是否覆盖原来的配置信息？",
            "Do you want to overwrite the previous configuration information?",
            "¿Quieres sobreescribir la anterior información de configuración?"
        };
        public static string[] StatisticsInfoForm2Messagebox2Text = new string[3]{
            "导出Excel表格成功！",
            "Export into excel table successfully!",
            "¡Exportado a tabla excel con éxito!"
        };
        public static string[] StatisticsInfoForm2Messagebox3Text = new string[3]{
            "图片保存成功！",
            "Image save successfully!",
            "Image save successfully!"
        };
        #endregion

        #region StatisticsInfoForm3.cs

        public static string[] StatisticsInfoForm3MessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] StatisticsInfoForm3MessageboxQuestionCaption = new string[3] { 
            "询问信息", 
            "Question message", 
            "Question message" 
        };
        public static string[] StatisticsInfoForm3Messagebox1Text = new string[3]{
            "是否覆盖原来的配置信息？",
            "Do you want to overwrite the previous configuration information?",
            "¿Quieres sobreescribir la anterior información de configuración?"
        };
        public static string[] StatisticsInfoForm3Messagebox2Text = new string[3]{
            "导出Excel表格成功！",
            "Export into excel table successfully!",
            "¡Exportado a tabla excel con éxito!"
        };
        public static string[] StatisticsInfoForm3Messagebox3Text = new string[3]{
            "是否需要保存图片！",
            "Do you need to save image?",
            "Do you need to save image?"
        };
        public static string[] StatisticsInfoForm3Messagebox4Text = new string[3]{
            "图片保存成功！",
            "Image save successfully!",
            "Image save successfully!"
        };
        #endregion

        #region StatisticsInfoForm4.cs

        public static string[] StatisticsInfoForm4MessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] StatisticsInfoForm4Messagebox1Text = new string[3]{
            "是否覆盖原来的配置信息？",
            "Do you want to overwrite the previous configuration information?",
            "¿Quieres sobreescribir la anterior información de configuración?"
        };
        public static string[] StatisticsInfoForm4Messagebox2Text = new string[3]{
            "导出Excel表格成功！",
            "Export into excel table successfully!",
            "¡Exportado a tabla excel con éxito!"
        };
        #endregion

        #region SugarSetForm.cs

        public static string[] SugarSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] SugarSetFormMessagebox1Text = new string[3]{
            "含糖量等级名称不能为空！",
            "The Brix grade name cannot be empty!",
            "¡Nombre de clase Brix no puede estar vacío!"
        };
        public static string[] SugarSetFormMessagebox2Text = new string[3]{
            "含糖量等级名称不能重名！",
            "The Brix grade name cannot be duplicated!",
            "¡Nombre de clase Brix no puede estar duplicado!"
        };
        public static string[] SugarSetFormMessagebox3Sub1Text = new string[3]{
            "含糖量等级第",
            "The Brix degree of Brix grade in the line",
            "¡El valor de la clase grado Brix en la línea"
        };
        public static string[] SugarSetFormMessagebox3Sub2Text = new string[3]{
            "行的含糖量应大于第",
            "should be greater than the Brix degree in the line",
            "debe ser mayor que el valor grado Brix en la línea"
        };
        public static string[] SugarSetFormMessagebox3Sub3Text = new string[3]{
            "行的含糖量！",
            "!",
            "!"
        };
        #endregion

        #region SystemStruct.cs

        public static string[] SystemStructMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] SystemStructMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] SystemStructMessagebox1Text = new string[3]{
            "当前工作台出口数量设置没有达到设定量",
            "The outlet quantity in current work table does not reach the setting value!",
            "¡La cantidad de salidas en la tabla de trabajo actual no coincide con el valor configurado!"
        };
        public static string[] SystemStructMessagebox2Text = new string[3]{
            "请到通道出口页面对该通道进行相应设置，并注意修改等级设置--分选标准页面的相应内容",
            "Pls go to the lane & outlet page to set the lane parameter and change the grade setting--relevant information in the sorting standard page!",
            "¡Vaya a la página líneas y salidas para configurar el parámetro línea y cambiar la configuración de clase--información relacionada con la página de clasificado!"
        };
        public static string[] SystemStructMessagebox3Text = new string[3]{
            "请修改等级设置--分选标准页面的相应内容！",
            "Pls change the grade setting--relevant information in the sorting standard page!",
            "¡Cambie la configuración de clase--relacionada con la página de clasificado!"
        };
        public static string[] SystemStructMessagebox4Text = new string[3]{
            "请到通道出口页面进行通道设置！",
            "Pls go to the lane & outlet page to set the lane setting!",
            "¡Vaya a la página líneas y salidas para ajustar la configuración de línea!"
        };
        public static string[] SystemStructMessagebox5Text = new string[3]{
            "系统结构保存配置错误！",
            "The save of system structure configuration is wrong!",
            "¡El guardado de la configuración de la estructura del sistema es incorrecto!"
        };
        #endregion

        #region TangxinSetForm.cs

        public static string[] TangxinSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] TangxinSetFormMessagebox1Text = new string[3]{
            "糖心等级名称不能为空！",
            "Water core grade name cannot be empty!",
            "¡Nombre de clase Vitrescencia no puede estar vacío!"
        };
        public static string[] TangxinSetFormMessagebox2Text = new string[3]{
            "糖心等级名称不能重名！",
            "Water core grade name cannot be duplicated!",
            "¡Nombre de clase Vitrescencia no puede estar duplicado!"
        };
        public static string[] TangxinSetFormMessagebox3Sub1Text = new string[3]{
            "糖心等级第",
            "The parameter of the water core grade in line",
            "¡El valor de la clase Vitrescencia en la línea"
        };
        public static string[] TangxinSetFormMessagebox3Sub2Text = new string[3]{
            "行的参数应大于第",
            "should be greater than the parameter in the line",
            "debe ser mayor que el valor Vitrescencia en la línea"
        };
        public static string[] TangxinSetFormMessagebox3Sub3Text = new string[3]{
            "行！",
            "!",
            "!"
        };
        #endregion

        #region ValidateForm.cs

        public static string[] ValidateFormMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] ValidateFormMessagebox1Text = new string[3]{
            "密码输入错误！",
            "Password is incorrectly!",
            "¡Password incorrecto!"
        };
        #endregion

        #region VolveTestForm.cs

        public static string[] VolveTestFormMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] VolveTestFormMessagebox1Text = new string[3]{
            "通道出口设置界面保存配置出错！",
            "The save of configuration in outlet setting interface go wrong!",
            "El guardado de configuración en la interfaz de configuración de salida falla!"
        };

        #endregion

        #region WaterSetForm.cs

        public static string[] WaterSetFormMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] WaterSetFormMessagebox1Text = new string[3]{
            "含水率等级名称不能为空！",
            "The grade name of moisture content cannot be empty!",
            "¡Nombre de clase Contenido humedad no puede estar vacío!"
        };
        public static string[] WaterSetFormMessagebox2Text = new string[3]{
            "含水率等级名称不能重名！",
            "The grade name of moisture content cannot be duplicated!",
            "¡Nombre de clase Contenido humedad no puede estar duplicado!"
        };
        public static string[] WaterSetFormMessagebox3Sub1Text = new string[3]{
            "含水率等级第",
            "The parameter value of moisture content in the line",
            "¡El valor de la clase Contenido humedad en la línea"
        };
        public static string[] WaterSetFormMessagebox3Sub2Text = new string[3]{
            "行的参数值应大于第",
            "should be greater than the parameter value in the line",
            "debe ser mayor que el valor Contenido humedad en la línea"
        };
        public static string[] WaterSetFormMessagebox3Sub3Text = new string[3]{
            "行！",
            "!",
            "!"
        };
        #endregion

        #region WeightSet.cs

        public static string[] WeightSetMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] WeightSetMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] WeightSetMessagebox1Text = new string[3]{
            "请确认校正系数为1！",
            "Pls confirm the calibration coefficient is 1!",
            "¡Confirmar que el coeficiente de calibración es 1!"
        };
        public static string[] WeightSetMessagebox2Text = new string[3]{
            "重量保存出错！",
            "Weight saving is wrong!",
            "¡Guardado peso incorrecto!"
        };
        public static string[] WeightSetMessagebox3Text = new string[3]{
            "重量保存出错：通道未选择！",
            "Weight saving is wrong: the lane is unselected!",
            "¡Guardado peso incorrecto: la línea no está seleccionada!"
        };
        #endregion

        #region GlobalDataInterface.cs

        public static string[] GlobalDataInterfaceMessageboxInformationCaption = new string[3] { 
            "提示信息", 
            "Prompt message", 
            "Prompt message" 
        };
        public static string[] GlobalDataInterfaceMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] GlobalDataInterfaceMessagebox1Text = new string[3]{
            "插入水果信息失败！",
            "Failure to add fruit information!",
            "¡Fallo al añadir información fruto!"
        };
        public static string[] GlobalDataInterfaceMessagebox2Text = new string[3]{
            "更新水果信息开始状态时失败！",
            "Failure to update fruit information starting status!",
            "¡Fallo al actualizar información fruto iniciando estatus!"
        };
        public static string[] GlobalDataInterfaceMessagebox3Text = new string[3]{
            "更新水果信息开始时间时失败！",
            "Failure to update fruit information starting time!",
            "¡Fallo al actualizar información fruto iniciando tiempo!"
        };
        public static string[] GlobalDataInterfaceMessagebox5Sub1Text = new string[3]{
            "版本不同！上位机版本为V",
            "Different program version! HMI verision is V",
            "¡Diferente versión programa! HMI versión es V"
        };
        public static string[] GlobalDataInterfaceMessagebox5Sub2Text = new string[3]{
            "，下位机版本为V",
            ", FSM version is V",
            ", FSM versión es V"
        };
        public static string[] GlobalDataInterfaceMessagebox5Sub3Text = new string[3]{
            "！",
            "!",
            "!"
        };
        public static string[] GlobalDataInterfaceMessagebox5Sub4Text = new string[3]{
            "子系统",
            "Failure to connect subsystem ",
            "¡Fallo al conectar subsistema "
        };
        public static string[] GlobalDataInterfaceMessagebox5Sub5Text = new string[3]{
            "连接失败！",
            "!",
            "!"
        };
        #endregion

        #region Program.cs

        public static string[] ProgramMessageboxErrorCaption = new string[3] { 
            "错误信息", 
            "Error message", 
            "Error message" 
        };
        public static string[] ProgramMessageboxWarningCaption = new string[3] { 
            "警告信息", 
            "Warning message", 
            "Warning message" 
        };
        public static string[] ProgramMessagebox1Text = new string[3]{
            "检查本地IP...",
            "Check local IP...",
            "Comprobar IP local..."
        };
        public static string[] ProgramMessagebox2Text = new string[3]{
            "IP地址错误或没有网络连接！",
            "IP address error or network unconnected!",
            "¡Error dirección IP o red desconectada!"
        };
        public static string[] ProgramMessagebox3Text = new string[3]{
            "正在建立网络连接...",
            "Network is connecting...",
            "Red esta conectando..."
        };
        public static string[] ProgramMessagebox4Text = new string[3]{
            "正在查询子系统...",
            "The subsystem is being queried...",
            "El subsistema está siendo consultado..."
        };
        public static string[] ProgramMessagebox5Text = new string[3]{
            "子系统1连接失败，请检查连接...",
            "Subsystem 1 connection failure, please check the connection...",
            "Error al conectar Subsistema 1, Revisar conexión..."
        };
        public static string[] ProgramMessagebox6Text = new string[3]{
            "连接FSM失败，请检查连接...",
            "Failure to connect FSM, pls check the connection...",
            "Fallo al conectar FSM, comprobar la conexión..."
        };
        public static string[] ProgramMessagebox7Text = new string[3]{
            "子系统与配置数目不符，是否继续运行？",
            "Subsystem number is not matched with the number of the configuration, continue running or not?",
            "El número de Subsistema no coincide con el número de la configuración, ¿continuar o no?"
        };
        public static string[] ProgramMessagebox8Text = new string[3]{
            "正在启动...",
            "Starting up...",
            "Iniciando..."
        };
        public static string[] ProgramMessagebox9Text = new string[3]{
            "系统启动中，请稍等......",
            "System is starting up, pls wait ......",
            "Sistema iniciando, esperar ......"
        };
        public static string[] ProgramMessageBoxManagerOK = new string[3] { 
            "确定", 
            "Ok", 
            "Ok" 
        };
        public static string[] ProgramMessageBoxManagerNo = new string[3] { 
            "否", 
            "No", 
            "No" 
        };
        public static string[] ProgramMessageBoxManagerYes = new string[3] { 
            "是", 
            "Yes", 
            "Si" 
        };
        public static string[] ProgramMessageBoxManagerCancel = new string[3] { 
            "取消", 
            "Cancel", 
            "Cancelar" 
        };
        public static string[] ProgramMessageBoxManagerRetry = new string[3] { 
            "重试", 
            "Retry", 
            "Retry" 
        };
        public static string[] ProgramMessageBoxManagerIgnore = new string[3] { 
            "忽略", 
            "Ignore", 
            "Ignore" 
        };
        public static string[] ProgramMessageBoxManagerAbort = new string[3] { 
            "中止", 
            "Abort", 
            "Abort" 
        };
        #endregion
    }
}
