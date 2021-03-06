﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ArchitectSample.WebApp.Filters
{
    public class ResultCacheAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Lockers = new ConcurrentDictionary<string, SemaphoreSlim>();

        public int Order { get; }

        public int Duration { get; set; }

        public string VaryByArgument { get; set; }

        public string VaryByHeader { get; set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var memoryCache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

            var cacheKey = this.GenerateCacheKey(context);

            if (memoryCache.TryGetValue(cacheKey, out IActionResult result))
            {
                context.Result = result;
            }
            else
            {
                var locker = Lockers.GetOrAdd(cacheKey, key => new SemaphoreSlim(1, 1));

                await locker.WaitAsync();

                if (memoryCache.TryGetValue(cacheKey, out result))
                {
                    locker.Release();

                    context.Result = result;
                }
                else
                {
                    var executedContext = await next();

                    if (executedContext.Exception == null && executedContext.Result != null && !executedContext.Canceled)
                    {
                        try
                        {
                            this.OutputAndCacheResult(executedContext, cacheKey);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    locker.Release();
                }
            }
        }

        private string GenerateCacheKey(ActionExecutingContext context)
        {
            void AppendByArguments(string[] varyByArguments, StringBuilder stringBuilder)
            {
                if (varyByArguments == null || varyByArguments.Length == 0) return;

                stringBuilder.Append("?");

                foreach (var argumentName in varyByArguments)
                {
                    var value = "[Default]";

                    if (context.ActionArguments.ContainsKey(argumentName))
                    {
                        value = JsonSerializer.Serialize(context.ActionArguments[argumentName]);
                    }

                    stringBuilder.AppendFormat("{0}={1}&", argumentName, value);
                }

                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            void AppendByHeaders(string[] varyByHeaders, StringBuilder stringBuilder)
            {
                if (varyByHeaders == null || varyByHeaders.Length == 0) return;

                stringBuilder.Append("\r\n");

                foreach (var headerName in varyByHeaders)
                {
                    string value = context.HttpContext.Request.Headers[headerName];

                    if (string.IsNullOrEmpty(value)) value = "[Empty]";

                    stringBuilder.AppendFormat("{0}: {1}\r\n", headerName, value).Append(headerName);
                }

                stringBuilder.Remove(stringBuilder.Length - 2, 2);
            }

            var keyBuilder = new StringBuilder();

            keyBuilder.Append(context.RouteData.Values["area"] ?? "[Empty]");
            keyBuilder.Append("/");
            keyBuilder.Append(context.RouteData.Values["controller"]);
            keyBuilder.Append("/");
            keyBuilder.Append(context.RouteData.Values["action"]);

            AppendByArguments(this.VaryByArgument?.Split(',', StringSplitOptions.RemoveEmptyEntries), keyBuilder);

            AppendByHeaders(this.VaryByHeader?.Split(',', StringSplitOptions.RemoveEmptyEntries), keyBuilder);

            return keyBuilder.ToString();
        }

        private void OutputAndCacheResult(ActionExecutedContext context, string cacheKey)
        {
            if (context.Result is ViewResult viewResult)
            {
                var executor = (ViewResultExecutor)context.HttpContext.RequestServices.GetService<IActionResultExecutor<ViewResult>>();

                var viewEngineResult = executor.FindView(context, viewResult);

                var view = viewEngineResult.View;

                using (view as IDisposable)
                {
                    var viewOptions = context.HttpContext.RequestServices.GetService<IOptions<MvcViewOptions>>();

                    var writer = new StringWriter();

                    var viewContext = new ViewContext(
                        context,
                        view,
                        viewResult.ViewData,
                        viewResult.TempData,
                        writer,
                        viewOptions.Value.HtmlHelperOptions);

                    view.RenderAsync(viewContext).GetAwaiter().GetResult();

                    context.Result = new ContentResult
                    {
                        Content = writer.ToString(),
                        ContentType = "text/html; charset=utf-8",
                        StatusCode = viewResult.StatusCode
                    };
                }
            }
            else if (context.Result is JsonResult jsonResult)
            {
                JsonSerializerOptions jsonSerializerOptions;

                if (jsonResult.SerializerSettings == null)
                {
                    var jsonOptions = context.HttpContext.RequestServices.GetService<IOptions<JsonOptions>>();

                    jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
                }
                else
                {
                    jsonSerializerOptions = jsonResult.SerializerSettings as JsonSerializerOptions;
                }

                var type = jsonResult.Value?.GetType() ?? typeof(object);

                context.Result = new ContentResult
                {
                    Content = JsonSerializer.Serialize(jsonResult.Value, type, jsonSerializerOptions),
                    ContentType = "application/json; charset=utf-8",
                    StatusCode = jsonResult.StatusCode
                };
            }

            var memoryCache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

            if (this.Duration > 0)
            {
                memoryCache.Set(cacheKey, context.Result, TimeSpan.FromSeconds(this.Duration));
            }
            else
            {
                memoryCache.Set(cacheKey, context.Result);
            }
        }
    }
}