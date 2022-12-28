<Query Kind="Statements" />

// 檢查 [BackendCache(VaryByDisplayMode = "Device")]
var dir = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web";
var controllers = Directory.GetFiles(dir, "*Controller.cs", SearchOption.AllDirectories);
var deviceRegex = new Regex("\\[BackendCache\\([^\\]]*VaryByDisplayMode = \"[^\"]*Device[^\"]*\"");
var prerenderRegex = new Regex("\\[Prerender\\]");
var excludedRegex = new Regex("TagCloud\\(|BestBloggers\\(|SkyrocketEncryptDate\\(");

foreach (var controller in controllers)
{
    var code = File.ReadAllText(controller);
    
    var blocks = code.Split("\r\n\r\n").Where(b => b.Contains("public IActionResult "));
    
    if (!blocks.All(b => excludedRegex.IsMatch(b) || deviceRegex.IsMatch(b)))
    {
        Console.WriteLine(controller);
    }
    
}