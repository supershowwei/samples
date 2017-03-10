using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

public class AttackPrevention : IHttpModule
{
    private static readonly string[] WhiteList = { "127.0.0.1" };

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
        context.BeginRequest += BeginRequest;
    }

    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted",
         Justification = "Reviewed. Suppression is OK here.")]
    private static void BeginRequest(object sender, EventArgs e)
    {
        var context = ((HttpApplication)sender).Context;
        var request = ((HttpApplication)sender).Request;
        var response = ((HttpApplication)sender).Response;

        string clientIP;
        if (IsPass(request, out clientIP)) return;

        var blockedKey = string.Concat("block_", clientIP);
        if (context.Cache[blockedKey] != null)
        {
            // 從 Cache 中有發現 Block ClientIP 就封鎖
            response.Close();
        }
        else
        {
            if (context.Cache[clientIP] == null)
            {
                CreateHitsInfo(context, clientIP);
            }

            var hitsInfo = context.Cache[clientIP] as HitsInfo;

            hitsInfo.Hits++;

            var diffSecondsFromStart = DateTime.Now.Subtract(hitsInfo.StartTime).TotalSeconds;

            if (IsAttack(hitsInfo, diffSecondsFromStart))
            {
                BlockClient(context, blockedKey);

                // ToDo: Log "IP '{clientIP}' 有異常操作現象，暫時封鎖 10 分鐘！"

                response.Close();
            }
            else if (diffSecondsFromStart > 3.4)
            {
                hitsInfo.Hits = 0;
                hitsInfo.StartTime = DateTime.Now;
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted",
         Justification = "Reviewed. Suppression is OK here.")]
    private static bool IsPass(HttpRequest request, out string clientIP)
    {
        clientIP = default(string);

        // 放過爬蟲
        if (request.Browser.Crawler) return true;

        // 放過存取靜態檔案的
        if (Regex.IsMatch(request.Url.AbsolutePath, @"\.[a-zA-Z0-9]{1,5}$")) return true;

        clientIP = GetClientIP(request);

        // IP 是空的、不符合 IPv4 格式的暫時不處理，先放過。
        if (string.IsNullOrEmpty(clientIP) || !Regex.IsMatch(clientIP, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")) return true;

        // 放過白名單
        if (WhiteList.Contains(clientIP)) return true;

        return false;
    }

    private static void BlockClient(HttpContext context, string blockedKey)
    {
        // 封鎖 10 分鐘
        context.Cache.Add(
            blockedKey,
            string.Empty,
            null,
            DateTime.Now.AddMinutes(10),
            Cache.NoSlidingExpiration,
            CacheItemPriority.Normal,
            null);
    }

    private static void CreateHitsInfo(HttpContext context, string clientIP)
    {
        context.Cache.Add(
            clientIP,
            new HitsInfo(clientIP, DateTime.Now),
            null,
            DateTime.Now.AddDays(1).Date,
            Cache.NoSlidingExpiration,
            CacheItemPriority.Normal,
            null);
    }

    private static bool IsAttack(HitsInfo hitsInfo, double diffSecondsFromStart)
    {
        return ((diffSecondsFromStart <= 1.4) && (hitsInfo.Hits >= 50))
               || ((diffSecondsFromStart <= 3.4) && (hitsInfo.Hits >= 120));
    }

    private static string GetClientIP(HttpRequest request)
    {
        return request.ServerVariables["HTTP_VIA"] == null
                   ? request.ServerVariables["REMOTE_ADDR"]
                   : request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    }
}