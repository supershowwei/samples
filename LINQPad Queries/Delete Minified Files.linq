<Query Kind="Statements" />

var wwwroot = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web\wwwroot";
var bundleConfigFile = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web\bundleconfig.json";
var bundleConfig = File.ReadAllText(bundleConfigFile);

foreach (var minFile in Regex.Matches(bundleConfig, "\"outputFileName\": \"([^\"]+)\"").Cast<Match>().Select(m => m.Groups[1].Value.Replace("/", "\\").Replace("wwwroot", wwwroot)))
{
	File.Delete(minFile);
}