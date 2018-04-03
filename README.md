ContextShowMiddleware

	跟踪请求/回应信息

使用场景

	当我们需要跟踪网站通信过程时，可以使用这个中间件，进行信息跟踪显示。但使用该中间件时，性能会受到影响，并不建议在生产环境中使用该中间件。

项目简介

	目前该项目处于初期，想法并未成型，虽然已经在Nuget上放出了稳定版，但是目前功能十分单一，目前需要挖掘大量需求，进入快速迭代期。欢迎大家积极参与。

项目团队

	目前参与人员AzulX、Denni.

使用说明

- 配置中间件
  - ShowInDebug以及ShowInConsole可以控制跟踪信息的输出方式
  - KeyTabs/ValueTabs可以控制显示格式(设置列)
  - CheckApiPaths/IgnoreApiPaths可以筛选指定URL
  - 为ControllerBase提供扩展,当你在使用WebApi时，可以使用Recoder("xxx")方法对返回结果进行记录

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddContextShow((request, response) => {
            
            //It would be no use when WaitForResponse=true
            request.WaitForResponse=true;
            
            request.ShowInDebug = false;
            response.ShowInDebug = false;
    
            request.IsFilterApiPaths = true;
            //配置过滤规则
            request.CheckApiPaths.Add(".*");
            request.IgnoreApiPaths.Add("/favicon.ico");
        });
        services.AddMvc();
    }

- 使用服务

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
           if (env.IsDevelopment())
           {
               app.UseDeveloperExceptionPage();
           }
           app.UseStaticFiles();
           app.UseContextShow();
           app.UseMvc();
    }

项目方向

- 提高信息的完整性
- 制定灵活的显示规则
- 解决并发问题，支持文件操作
- 规范变量的命名，如果有必要
- 是否应该模板化？
  - 模板化重构
  - 实体类Emit映射

项目计划

- 显示返回码
- 增加返回码过滤规则
- 显示匹配路由
- 支持I/O操作
- 支持数据库操作
- 支持Dashboard
- 支持报表分析

更新日志

- 2018-04-01：愚人节，今天将源代码上传到Github. 希望迭代几次之后能够正式发布到Nuget.
