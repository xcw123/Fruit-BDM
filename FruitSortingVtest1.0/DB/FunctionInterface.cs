using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FruitSortingVtest1.DB
{
    public class FunctionInterface
    {
        private const int MaxClientInfoNum = 10; //最多可保存的客户信息数 5->10 Modify by ChengSk 20190426

        //将NULL转化为""
        public static string NullToString(string strTemp)
        {
            if (strTemp == null)
            {
                return "";
            }
            return strTemp;
        }

        //获取数组中的最大值
        public static UInt64 GetMaxValue(UInt64[] uiTemp)
        {
            UInt64 temp = 0;
            for (int i = 0; i < uiTemp.Length; i++)
            {
                if (uiTemp[i] > temp)
                {
                    temp = uiTemp[i];
                }
            }
            return temp;
        }

        //获取数组的总值
        public static UInt64 GetSumValue(UInt64[] uiTemp)
        {
            UInt64 temp = 0;
            for (int i = 0; i < uiTemp.Length; i++)
            {
                temp += uiTemp[i];
            }
            return temp;
        }

        //获取数组中的最大值
        public static Int32 GetMaxValue(Int32[] uiTemp)
        {
            Int32 temp = 0;
            for (int i = 0; i < uiTemp.Length; i++)
            {
                if (uiTemp[i] > temp)
                {
                    temp = uiTemp[i];
                }
            }
            return temp;
        }

        //获取数组的总值
        public static Int32 GetSumValue(Int32[] uiTemp)
        {
            Int32 temp = 0;
            for (int i = 0; i < uiTemp.Length; i++)
            {
                temp += uiTemp[i];
            }
            return temp;
        }

        /// <summary>
        /// 将新客户信息进行汇总
        /// </summary>
        /// <param name="strContent">原汇总信息</param>
        /// <param name="strNew">新信息</param>
        /// <returns>总的信息</returns>
        public static string CombineString(string strContent, string strNew)
        {
            string strTemp = "";
            string[] strContentItem = strContent.Split('，');
            int index = 100;     //相同元素的下标
            bool bIsSame = false;//是否有相同元素
            for (int i = 0; i < strContentItem.Length; i++)
            {
                if (strNew.Equals(strContentItem[i]))
                {
                    index = i;
                    bIsSame = true;
                }
            }      
            if (bIsSame)//有相同元素
            {
                strTemp = strNew;
                for (int i = 0; i < strContentItem.Length; i++)
                {
                    if (i != index)
                    {
                        strTemp += "，" + strContentItem[i];
                    }
                }
            }
            else  //无相同元素
            {
                strTemp = strNew;
                if (strContentItem.Length == MaxClientInfoNum)
                {
                    for (int i = 0; i < MaxClientInfoNum - 1; i++)
                    {
                        strTemp += "，" + strContentItem[i];
                    }
                }
                else
                {
                    for (int i = 0; i < strContentItem.Length; i++)
                    {
                        strTemp += "，" + strContentItem[i];
                    }
                }                        
            }
            return strTemp;
        }
    }
}
