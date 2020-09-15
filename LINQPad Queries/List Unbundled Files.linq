<Query Kind="Statements" />

var wwwroot = @"E:\wantgoo\src\WantGooArk\WantGooArk.Web\wwwroot";
var bundleConfigFile = @"E:\wantgoo\src\WantGooArk\WantGooArk.Web\bundleconfig.json";

var files = Directory.GetFiles(Path.Combine(wwwroot, "css"), "*.min.*", SearchOption.AllDirectories);

files = files.Concat(Directory.GetFiles(Path.Combine(wwwroot, "js"), "*.min.*", SearchOption.AllDirectories)).ToArray();

var bundleConfig = File.ReadAllText(bundleConfigFile);

files.Where(f => !bundleConfig.Contains(Path.GetFileName(f))).Dump();