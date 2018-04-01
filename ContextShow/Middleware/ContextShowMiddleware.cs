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
        private readonly ContextShowRequestOption _request_options;
        private readonly ContextShowResponseOption _response_option;

        private readonly string RequestHeader;
        private readonly string ResponseHeader;

        private readonly Regex[] PathFilers;
        private readonly Regex[] IgnoreFilters;

        private readonly string RequestKeyTabs;
        private readonly string RequestValueTabs;
        private readonly string LessRequestValueTabs;

        private readonly string ResponseKeyTabs;
        private readonly string ResponseValueTabs;
        private readonly string LessResponseValueTabs;

        private readonly string HeaderTabs;

        internal static ConcurrentDictionary<HttpResponse, string> _response_contents;
        public ContextShowMiddleware(ContextShowRequestOption request_options, ContextShowResponseOption response_option)
        {
            _response_contents = new ConcurrentDictionary<HttpResponse, string>();
            _request_options = request_options;
            _response_option = response_option;

            //初始化Tab距离
            for (int i = 0; i < _request_options.KeyTabs; i += 1)
            {
                RequestKeyTabs += "\t";
            }
            for (int i = 0; i < _request_options.ValueTabs; i += 1)
            {
                RequestValueTabs += "\t";
                if (i < _request_options.ValueTabs - 1)
                {
                    LessRequestValueTabs += "\t";
                }
            }
            for (int i = 0; i < _response_option.KeyTabs; i += 1)
            {
                ResponseKeyTabs += "\t";
            }
            for (int i = 0; i < _response_option.ValueTabs; i += 1)
            {
                ResponseValueTabs += "\t";
                if (i < _request_options.ValueTabs - 1)
                {
                    LessResponseValueTabs += "\t";
                }
            }
            for (int i = 0; i < _request_options.KeyTabs - 1; i += 1)
            {
                HeaderTabs += "\t";
            }

            //初始化匹配筛选器
            if (_request_options.IsFilterApiPaths)
            {
                PathFilers = new Regex[_request_options.CheckApiPaths.Count];
                for (int i = 0; i < _request_options.CheckApiPaths.Count; i += 1)
                {
                    PathFilers[i] = new Regex(_request_options.CheckApiPaths[i], RegexOptions.Compiled);
                }
            }
            //初始化忽略筛选器
            IgnoreFilters = new Regex[_request_options.IgnoreApiPaths.Count];
            for (int i = 0; i < _request_options.IgnoreApiPaths.Count; i += 1)
            {
                IgnoreFilters[i] = new Regex(_request_options.IgnoreApiPaths[i], RegexOptions.Compiled);
            }

            RequestHeader = $"\r\n==================================================================================================================\r\n{HeaderTabs}┌────────────────────────┐\r\n{HeaderTabs}│                     Request                    │\r\n{HeaderTabs}└────────────────────────┘\r\n";
            ResponseHeader = $"{HeaderTabs}┌────────────────────────┐\r\n{HeaderTabs}│                     Response                   │\r\n{HeaderTabs}└────────────────────────┘\r\n";
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            HttpRequest request = context.Request;

            int result = 0;
            if (_request_options.IsFilterApiPaths)
            {
                for (result = 0; result < PathFilers.Length; result += 1)
                {
                    if (PathFilers[result].IsMatch(request.Path.Value))
                    {
                        break;
                    }
                }
                if (result > PathFilers.Length - 1)
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
            if (_request_options.ShowTime)
            {
                builder.AppendLine($"{RequestKeyTabs}RtTime:{RequestValueTabs}{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ffff")}");
            }
            if (_request_options.ShowLocalAddress)
            {
                builder.AppendLine($"{RequestKeyTabs}Local:{RequestValueTabs}{context.Connection.LocalIpAddress}:{context.Connection.LocalPort}");
            }
            if (_request_options.ShowRemoteAddress)
            {
                builder.AppendLine($"{RequestKeyTabs}Remote:{RequestValueTabs}{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}");
            }
            if (_request_options.ShowMethod)
            {
                builder.AppendLine($"{RequestKeyTabs}Method:{RequestValueTabs}{request.Method}");
            }
            if (_request_options.ShowUrl)
            {
                builder.AppendLine($"{RequestKeyTabs}Url:{RequestValueTabs}{context.Connection.LocalIpAddress}:{context.Connection.LocalPort}{request.Path.Value}");
            }
            if (_request_options.ShowApiPath)
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
            if (_request_options.ShowQueryString)
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

            if (_request_options.ShowHeader)
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

            if (_request_options.ShowCookies)
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

            if (_request_options.ShowContent)
            {
                request.Body.Position = 0;
                content = new StreamReader(request.Body).ReadToEnd();
                request.Body.Position = 0;
                builder.AppendLine($"{RequestKeyTabs}Content:{LessRequestValueTabs}{content}");
            }

            if (_request_options.ShowHasSteam)
            {
                builder.AppendLine(request.Body == null ? $"{RequestKeyTabs}Has Stream and Length:{request.Body.Length} Current Pos:{request.Body.Position}" : $"{RequestKeyTabs}Body:{RequestValueTabs}No Stream.");
            }

            if (!_request_options.WaitForResponse)
            {
                builder.AppendLine("\r\n==================================================================================================================");
                if (_request_options.ShowInDebug)
                {
                    Debug.WriteLine(builder.ToString());
                }
                if (_request_options.ShowInConsole)
                {
                    Console.WriteLine(builder.ToString());
                }
                builder.Clear();
                builder.AppendLine("\r\n==================================================================================================================\r\n");
                builder.AppendLine(ResponseHeader);
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine(ResponseHeader);
            }


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
            if (_response_option.ShowMethod)
            {
                builder.AppendLine($"{ResponseKeyTabs}Method:{RequestValueTabs}{request.Method}");
            }
            if (_response_option.ShowUrl)
            {
                builder.AppendLine($"{ResponseKeyTabs}Url:{RequestValueTabs}{context.Connection.LocalIpAddress}:{context.Connection.LocalPort}{request.Path.Value}");
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
            builder.AppendLine("\r\n==================================================================================================================");
            if (_response_option.ShowInDebug)
            {
                Debug.WriteLine(builder.ToString());
            }
            if (_response_option.ShowInConsole)
            {
                Console.WriteLine(builder.ToString());
            }
        }
    }

}
