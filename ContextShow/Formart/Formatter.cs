using System;
using System.Collections.Generic;
using System.Text;

namespace ContextShow.Formart
{
    public class Formatter
    {
        public Dictionary<string,string> Recoder;
        public List<string> KeyCollections;
        public int MaxKeyLength;
        public int MaxValueLenght;
        public char ColChar;
        public char RowChar;
        public string TableBaseChars;
        public Formatter()
        {
            Recoder = new Dictionary<string, string>();
        }

        //添加信息并自动计算最大Key和Value长度
        public string this[string key] {

            get
            {
                if (Recoder.ContainsKey(key))
                {
                    return Recoder[key];
                }
                return null;
            }

            set
            {
                if (!Recoder.ContainsKey(key))
                {
                    KeyCollections.Add(key);
                    SetMaxKeyLength(key);

                }
                Recoder[key] = value;
                SetMaxValueLength(value);
            }
        }


        /// <summary>
        /// 定制表格样式
        /// </summary>
        /// <param name="col">列分割线</param>
        /// <param name="row">行分割线</param>
        /// <param name="table">表格样式</param>
        public void GetTableFormart(char col= '│', char row = '─', string table = @"┌┬┐├┼┤└┴┘")
        {
            ColChar = col;
            RowChar = row;
            TableBaseChars = table;
        }

        public string CreateHeader()
        {

        }


        private void SetMaxValueLength(string value)
        {
            int temp = GetRealLength(value);
            if (temp > MaxValueLenght)
            {
                MaxValueLenght = temp;
            }
        }
        private void SetMaxKeyLength(string value)
        {
            int temp = GetRealLength(value);
            if (temp > MaxKeyLength)
            {
                MaxKeyLength = temp;
            }
        }

        /// <summary>
        /// 获取字符串在输出缓冲区的真实长度
        /// </summary>
        /// <param name="instance">需要计算的字符串</param>
        /// <returns>字符串的真是长度</returns>
        public static int GetRealLength(string instance)
        {
            int length = 0;
            for (int i = 0; i < instance.Length; i += 1)
            {
                if (instance[i] >= 0 && instance[i] <= 127)
                {
                    length += 1;
                }
            }
            return instance.Length * 2 - length;
        }
    }
}
