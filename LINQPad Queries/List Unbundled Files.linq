<Query Kind="Statements" />

var wwwroot = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web\wwwroot";
var bundleConfigFile = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web\bundleconfig.json";
var excluded = new[] { "day-trading-competition\\driver.min.css", "bot-detection.min.js", "signalr.min.js" };

var files = Directory.GetFiles(Path.Combine(wwwroot, "css"), "*.min.*", SearchOption.AllDirectories);

files = files.Concat(Directory.GetFiles(Path.Combine(wwwroot, "js"), "*.min.*", SearchOption.AllDirectories)).ToArray();

var bundleConfig = File.ReadAllText(bundleConfigFile);

var unbundledFiles = files.Where(f => !excluded.Any(x => f.Contains(x)) && !bundleConfig.Contains(f.Replace(wwwroot, string.Empty).Replace("\\", "/"))).ToList();

//unbundledFiles = unbundledFiles.Where(f => new[] { "_my-club-layout.min.css.min.css" }.Any(x => f.EndsWith(x))).ToList();
//unbundledFiles.ForEach(f => File.Delete(f));

unbundledFiles.Dump();

