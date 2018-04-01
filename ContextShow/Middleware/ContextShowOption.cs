using System;
using System.Collections.Generic;

namespace ContextShow
{

    public class ContextShowRequestOption : ContextShowOption
    {
        public ContextShowRequestOption()
        {
            ShowTime = true;
            ShowUrl = true;
            ShowQueryString = true;
            ShowHeader = true;
            ShowLocalAddress = true;
            ShowRemoteAddress = true;
            ShowInDebug = true;
            ShowInConsole = true;
            ShowHasSteam = true;
            ShowCookies = true;
            ShowApiPath = true;
            WaitForResponse = true;
            ShowMethod = true;
            IsFilterApiPaths = false;
            CheckApiPaths = new List<string>();
            IgnoreApiPaths = new List<string>();
            Console.WindowHeight = 50;
            Console.WindowWidth = 115;
            KeyTabs = 4;
            ValueTabs = 2;
        }
        public bool ShowQueryString;
        public bool ShowLocalAddress;
        public bool ShowHasSteam;
        public bool ShowRemoteAddress;
        public bool WaitForResponse;
        public bool ShowCookies;
        public bool IsFilterApiPaths;
        public List<string> CheckApiPaths;
        public List<string> IgnoreApiPaths;
        public int KeyTabs;
        public int ValueTabs;
    }

    public class ContextShowResponseOption : ContextShowOption
    {
        public ContextShowResponseOption()
        {
            ShowTime = true;
            ShowUrl = true;
            ShowHeader = true;
            ShowInDebug = true;
            ShowInConsole = true;
            ShowApiPath = true;
            ShowContent = true;
            ShowMethod = false;
            KeyTabs = 4;
            ValueTabs = 2;
        }
        public int KeyTabs;
        public int ValueTabs;
    }


    public class ContextShowOption
    {
        public bool ShowTime;
        public bool ShowUrl;
        public bool ShowHeader;
        public bool ShowInConsole;
        public bool ShowInDebug;
        public bool ShowApiPath;
        public bool ShowContent;
        public bool ShowMethod;
    }
}
