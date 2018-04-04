using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ContextShow
{

    public class ContextShowRequestOption : AbstractContextShowOption
    {
        public ContextShowRequestOption()
        {
            ShowQueryString = true;
<<<<<<< HEAD
=======
            ShowContent = true;
            ShowHeader = true;
>>>>>>> origin/master
            ShowLocalAddress = true;
            ShowRemoteAddress = true;
            ShowCookies = true;
            ShowApiPath = true;
            ShowMethod = true;
        }
        public bool ShowApiPath;
        public bool ShowMethod;
        public bool ShowQueryString;
        public bool ShowLocalAddress;
        public bool ShowRemoteAddress;
        public bool ShowCookies;
    }

    public class ContextShowResponseOption : AbstractContextShowOption
    {
        public ContextShowResponseOption()
        {
            ShowContent = true;
            ShowStatueCode = true;
        }
        public bool ShowStatueCode;

    }

    public abstract class AbstractContextShowOption
    {
        public AbstractContextShowOption()
        {
            ShowTime = true;
            ShowUrl = true;
            ShowHeader = true;
            ShowContent = true;
            KeyTabs = 4;
            ValueTabs = 2;
            HeaderTabs = -1;
        }
        public bool ShowTime;
        public bool ShowUrl;
        public bool ShowHeader;
        public bool ShowContent;
        public int KeyTabs;
        public int ValueTabs;
        public int HeaderTabs;

        public string LessTabString
        {
            get
            {
                return GetTabString(ValueTabs - 1);
            }
        }
        public string KeyTabString
        {
            get
            {
                return GetTabString(KeyTabs);
            }
        }
        public string ValueTabString
        {
            get
            {
                return GetTabString(ValueTabs);
            }
        }

        public string HeaderTabString
        {
            get
            {
                if (HeaderTabs<0)
                {
                    return GetTabString(KeyTabs - 1);
                }
                return GetTabString(HeaderTabs);
            }
        }

        private string GetTabString(int length)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i += 1)
            {
                result.Append("\t");
            }
            return result.ToString();
        }
    }

    public class ContextShowOption
    {
        public ContextShowOption()
        {
            Console.WindowHeight = 50;
            Console.WindowWidth = 115;
            IsFilterApiPaths = false;
            EnterApiPaths = new List<string>();
            IgnoreApiPaths = new List<string>();
            IsMergeInfo = true;
            SpliteLine = "\r\n==================================================================================================================\r\n";
        }
        public bool ShowInConsole;
        public bool ShowInDebug;
        public bool IsFilterApiPaths;
        public List<string> EnterApiPaths;
        public List<string> IgnoreApiPaths;
        public bool IsMergeInfo;
        public string SpliteLine;

        /// <summary>
        /// 添加API进入规则，匹配则显示
        /// </summary>
        /// <param name="path">正则表达式</param>
        /// <returns></returns>
        public ContextShowOption AddEnter(string path)
        {
            EnterApiPaths.Add(path);
            return this;
        }
        /// <summary>
        /// 添加API忽视规则，匹配则不显示
        /// </summary>
        /// <param name="path">正则表达式</param>
        /// <returns></returns>
        public ContextShowOption AddIgnore(string path)
        {
            IgnoreApiPaths.Add(path);
            return this;
        }

        public Regex[] EnterApis
        {
            get { return GetRegices(EnterApiPaths.ToArray()); }
        }
        public Regex[] IgnoreApis
        {
            get { return GetRegices(IgnoreApiPaths.ToArray()); }
        }
        private Regex[] GetRegices(string[] inputs)
        {
            Regex[] result = new Regex[inputs.Length];
            for (int i = 0; i < inputs.Length; i += 1)
            {
                result[i] = new Regex(inputs[i], RegexOptions.Compiled);
            }
            return result;
        }
    }

    public class AllContextShowOptions
    {
        public ContextShowOption Main;
        public ContextShowRequestOption Request;
        public ContextShowResponseOption Response;
    }

}
