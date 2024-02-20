<Query Kind="Statements" />

// 檢查 [BackendCache(VaryByDisplayMode = "Device")]
var dir = @"D:\Work\wantgoo\src\WantGooArk\WantGooArk.Web";
var controllers = Directory.GetFiles(dir, "*Controller.cs", SearchOption.AllDirectories);
var deviceRegex = new Regex("\\[BackendCache\\([^\\]]*VaryByDisplayMode = \"[^\"]*Device[^\"]*\"");
var prerenderRegex = new Regex("\\[Prerender\\]");
var returnViewRegex = new Regex("return this.View\\(");
var excludedRegex = new Regex(string.Join("|", new[] {
"public IActionResult TagCloud\\(\\)",
"public async Task<IActionResult> Unsubscribe\\(",
"public IActionResult QuantityRelativeRatio\\(\\)"
}));
var excludedControllers = new List<string> { "DevController" };

foreach (var controller in controllers)
{
    if (excludedControllers.Contains(Path.GetFileNameWithoutExtension(controller))) continue;

    var code = File.ReadAllLines(controller);

    var blocks = code.Aggregate(new List<StringBuilder> { new StringBuilder() }, (result, line) =>
    {
        var blockBuilder = result.Last();

        blockBuilder.AppendLine(line);

        if (line == "        }")
        {
            result.Add(new StringBuilder());
        }

        return result;
    }).Where(b => b.Length > 0).Select(b => b.ToString()).ToList();

    blocks = blocks.Where(b => b.Contains("public IActionResult ") || b.Contains("public async Task<IActionResult> ")).ToList();
    blocks = blocks.Where(b => returnViewRegex.IsMatch(b)).ToList();
    blocks = blocks.Where(b => !excludedRegex.IsMatch(b)).ToList();
    blocks = blocks.Where(b => !deviceRegex.IsMatch(b)).ToList();

    if (!blocks.Any()) continue;

    Console.WriteLine(controller);
    //blocks.Dump(); break;
}