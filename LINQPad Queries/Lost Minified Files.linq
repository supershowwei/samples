<Query Kind="Statements" />

var wwwroot = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web\wwwroot";
var bundleConfigFile = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web\bundleconfig.json";
var bundleConfig = File.ReadAllText(bundleConfigFile);

var files = Regex.Matches(bundleConfig, "\"outputFileName\": \"([^\"]+)\"").Cast<Match>().Select(m => m.Groups[1].Value.Replace("/", "\\").Replace("wwwroot", wwwroot)).Where(f => !File.Exists(f)).ToList();

files.Dump();
