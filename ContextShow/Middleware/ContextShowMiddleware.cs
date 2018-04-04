using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContextShow
{

    public class ContextShowMiddleware : IMiddleware
    {
        private readonly ContextShowRequestOption _request_option;
        private readonly ContextShowResponseOption _response_option;
        private readonly ContextShowOption _main_option;

        private readonly string RequestHeader;
        private readonly string ResponseHeader;

        private readonly Regex[] EnterFilters;
        private readonly Regex[] IgnoreFilters;

        private readonly string RequestKeyTabs;
        private readonly string RequestValueTabs;
        private readonly string LessRequestValueTabs;

        private readonly string ResponseKeyTabs;
        private readonly string ResponseValueTabs;
        private readonly string LessResponseValueTabs;

        private readonly string RequestHeaderTabs;
        private readonly string ResponseHeaderTabs;

        internal static ConcurrentDictionary<HttpResponse, string> _response_contents;
        public ContextShowMiddleware(AllContextShowOptions options)
        {
            _response_contents = new ConcurrentDictionary<HttpResponse, string>();
            _request_option = options.Request;
            _response_option = options.Response;
            _main_option = options.Main;

            //初始化Tab距离
            #region InitRequest
            RequestKeyTabs = _request_option.KeyTabString;
            RequestValueTabs = _request_option.ValueTabString;
            LessRequestValueTabs = _request_option.LessTabString;
            RequestHeaderTabs = _request_option.HeaderTabString;
            #endregion

            #region InitResponse
            ResponseKeyTabs = _response_option.KeyTabString;
            ResponseValueTabs = _response_option.ValueTabString;
            LessResponseValueTabs = _response_option.LessTabString;
            ResponseHeaderTabs = _response_option.HeaderTabString;
            #endregion




            //初始化匹配筛选器
            if (_main_option.IsFilterApiPaths)
            {
                EnterFilters = _main_option.EnterApis;
            }
            //初始化忽略筛选器
            IgnoreFilters = _main_option.IgnoreApis;

            RequestHeader = $"{_main_option.SpliteLine}{RequestHeaderTabs}┌────────────────────────┐\r\n{RequestHeaderTabs}│                     Request                    │\r\n{RequestHeaderTabs}└────────────────────────┘\r\n";
            ResponseHeader = $"{ResponseHeaderTabs}┌────────────────────────┐\r\n{ResponseHeaderTabs}│                     Response                   │\r\n{ResponseHeaderTabs}└────────────────────────┘\r\n";
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            HttpRequest request = context.Request;

            int result = 0;
            if (_main_option.IsFilterApiPaths)
            {
                for (result = 0; result < EnterFilters.Length; result += 1)
                {
                    if (EnterFilters[result].IsMatch(request.Path.Value))
                    {
                        break;
                    }
                }
                if (result > EnterFilters.Length - 1)
                {
                    await next(context);
                    return;
                }
            }
            for (result = 0; result < IgnoreFilters.Length; result += 1)
            {
                if (IgnoreFilters[result].IsMatch(request.Path.Value))
                {
                    await next(context);
                    return;
                }
            }

            request.EnableRewind();

            string content = string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(RequestHeader);

            if (_request_option.ShowTime)
            {
                builder.AppendLine($"{RequestKeyTabs}RtTime:{RequestValueTabs}{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ffff")}");
            }
            if (_request_option.ShowLocalAddress)
            {
                builder.AppendLine($"{RequestKeyTabs}Local:{RequestValueTabs}{context.Connection.LocalIpAddress}:{context.Connection.LocalPort}");
            }
            if (_request_option.ShowRemoteAddress)
            {
                builder.AppendLine($"{RequestKeyTabs}Remote:{RequestValueTabs}{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}");
            }
            if (_request_option.ShowMethod)
            {
                builder.AppendLine($"{RequestKeyTabs}Method:{RequestValueTabs}{request.Method}");
            }
            if (_request_option.ShowUrl)
            {
                builder.AppendLine($"{RequestKeyTabs}Url:{RequestValueTabs}{context.Connection.LocalIpAddress}:{context.Connection.LocalPort}{request.Path.Value}");
            }
            if (_request_option.ShowApiPath)
            {
                if (request.Path.HasValue)
                {
                    builder.AppendLine($"{RequestKeyTabs}ApiPath:{LessRequestValueTabs}{request.Path.Value}");
                }
                else
                {
                    builder.AppendLine($"{RequestKeyTabs}ApiPath:{LessRequestValueTabs}[No API Request]");
                }
            }
            if (_request_option.ShowQueryString)
            {
                if (request.QueryString.HasValue)
                {
                    builder.AppendLine($"{RequestKeyTabs}Query:{RequestValueTabs}" + request.QueryString.Value);
                }
                else
                {
                    builder.AppendLine($"{RequestKeyTabs}Query:{RequestValueTabs}[No Query String]");
                }
            }

            if (_request_option.ShowHeader)
            {
                if (request.Headers.Count != 0)
                {
                    int startIndex = 0;
                    builder.Append($"{RequestKeyTabs}Headers:{LessRequestValueTabs}");
                    foreach (var item in request.Headers)
                    {
                        if (startIndex == 0)
                        {
                            startIndex += 1;
                            builder.AppendLine($"{item.Key}:{item.Value}");
                        }
                        else
                        {
                            builder.AppendLine($"{RequestKeyTabs}{RequestValueTabs}{item.Key}:{item.Value}");
                        }
                    }
                }
            }

            if (_request_option.ShowCookies)
            {
                if (request.Cookies.Count != 0)
                {
                    int startIndex = 0;
                    builder.Append($"{RequestKeyTabs}Cookies:{LessRequestValueTabs}");
                    foreach (var item in request.Cookies)
                    {
                        if (startIndex == 0)
                        {
                            startIndex += 1;
                            builder.AppendLine($"{item.Key}:{item.Value}");
                        }
                        else
                        {
                            builder.AppendLine($"{RequestKeyTabs}{RequestValueTabs}{item.Key}:{item.Value}");
                        }
                    }
                }
            }

            if (_request_option.ShowContent)
            {
                request.Body.Position = 0;
                content = new StreamReader(request.Body).ReadToEnd();
                request.Body.Position = 0;
                builder.AppendLine($"{RequestKeyTabs}Content:{LessRequestValueTabs}{content}");
            }

            if (!_main_option.IsMergeInfo)
            {

                builder.AppendLine(_main_option.SpliteLine);
                if (_main_option.ShowInDebug)
                {
                    Debug.WriteLine(builder.ToString());
                }
                if (_main_option.ShowInConsole)
                {
                    Console.WriteLine(builder.ToString());
                }
                builder.Clear();
                builder.AppendLine("\r\n==================================================================================================================\r\n");
            }
            
            builder.AppendLine();
            builder.AppendLine(ResponseHeader);

            HttpResponse response = context.Response;

            if (_response_option.ShowContent)
            {

                Stream originalBody = context.Response.Body;
                try
                {
                    using (var memStream = new MemoryStream())
                    {
                        context.Response.Body = memStream;

                        await next(context);
                        if (_response_contents.ContainsKey(response))
                        {
                            content = _response_contents[response];
                        }
                        else
                        {
                            memStream.Position = 0;
                            content = new StreamReader(memStream).ReadToEnd();
                        }
                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    }
                }
                catch (Exception ex){
                    content = ex.Message;
                }
                context.Response.Body = originalBody;

            }
            else
            {
                await next(context);
            }

            if (_response_option.ShowTime)
            {
                builder.AppendLine($"{ResponseKeyTabs}RtTime:{RequestValueTabs}{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ffff")}");
            }
            if (_response_option.ShowUrl)
            {
                builder.AppendLine($"{ResponseKeyTabs}Url:{RequestValueTabs}{context.Connection.LocalIpAddress}:{context.Connection.LocalPort}{request.Path.Value}");
            }
            if (_response_option.ShowStatueCode)
            {
                builder.AppendLine($"{ResponseKeyTabs}Status:{RequestValueTabs}{response.StatusCode}");
            }
            if (_response_option.ShowHeader)
            {
                if (response.Headers.Count != 0)
                {
                    int startIndex = 0;
                    builder.Append($"{ResponseKeyTabs}Headers:{LessRequestValueTabs}");
                    foreach (var item in response.Headers)
                    {
                        if (startIndex == 0)
                        {
                            startIndex += 1;
                            builder.AppendLine($"{item.Key}:{item.Value}");
                        }
                        else
                        {
                            builder.AppendLine($"{ResponseKeyTabs}{RequestValueTabs}{item.Key}:{item.Value}");
                        }
                    }
                }
            }
            if (_response_option.ShowContent)
            {
                builder.AppendLine($"{ResponseKeyTabs}Content:{LessRequestValueTabs}{content}");
            }
            builder.AppendLine(_main_option.SpliteLine);
            if (_main_option.ShowInDebug)
            {
                Debug.WriteLine(builder.ToString());
            }
            if (_main_option.ShowInConsole)
            {
                Console.WriteLine(builder.ToString());
            }
        }
    }

}
