﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;

//http://www.sulhome.com/blog/10/log-asp-net-core-request-and-response-using-middleware
public class LogRequestMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<LogRequestMiddleware> _logger;
    private Func<string, Exception, string> _defaultFormatter = (state, exception) => state;

    public LogRequestMiddleware(RequestDelegate next, ILogger<LogRequestMiddleware> logger)
    {
        this.next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestBodyStream = new MemoryStream();
        var originalRequestBody = context.Request.Body;

        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);

        var url = UriHelper.GetDisplayUrl(context.Request);
        var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
        if (!context.Request.Path.Value.Contains("FileUpload")
            && !context.Request.Path.Value.Contains("LeadUpload")
            && !context.Request.Path.Value.Contains("SaveAccountNote"))

        {
            _logger.Log(LogLevel.Information, 1,
                $"REQUEST METHOD: {context.Request.Method}, REQUEST BODY: {requestBodyText}, REQUEST URL: {url}", null,
                _defaultFormatter);
            Logger.Log("REQUEST_MIDDLEWARE",
                $"REQUEST METHOD: {context.Request.Method}, REQUEST BODY: {requestBodyText}, REQUEST URL: {url}");
        }

        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        await next(context);
        context.Request.Body = originalRequestBody;
    }
}
