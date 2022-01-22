<Query Kind="Statements" />

var wwwroot = @"E:\wantgoo\src\WantGooArk\WantGooArk.Web\wwwroot";
var bundleConfigFile = @"E:\wantgoo\src\WantGooArk\WantGooArk.Web\bundleconfig.json";

var files = Directory.GetFiles(Path.Combine(wwwroot, "css"), "*.min.*", SearchOption.AllDirectories);

files = files.Concat(Directory.GetFiles(Path.Combine(wwwroot, "js"), "*.min.*", SearchOption.AllDirectories)).ToArray();

var bundleConfig = File.ReadAllText(bundleConfigFile);

var unbundledFiles = files.Where(f => !new[] { "bot-detection.min.js", "signature.min.js" }.Any(x => f.Contains(x)) && !bundleConfig.Contains(Path.GetFileName(f))).ToList();

unbundledFiles.Dump();

//unbundledFiles.ForEach(f => File.Delete(f));