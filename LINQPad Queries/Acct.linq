<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.Compatibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.dll</Reference>
  <Namespace>Microsoft.VisualBasic</Namespace>
  <Namespace>Microsoft.VisualBasic.CompilerServices</Namespace>
</Query>

var email = "oyx68990@cuoly.com";
var encodedEmail = email.Replace("@", "acct");

// 需補足序號長度，長度如下：
(encodedEmail.Length * 5).Dump();

object b(string A_0, string A_1)
{
	int num1 = checked((int)Math.Round(unchecked(Conversion.Val(Strings.Mid(A_0, 1, 1)) - Conversion.Val(Strings.Mid(A_1, 1, 1))))); // 5
	int num2 = checked((int)Math.Round(unchecked(Conversion.Val(Strings.Mid(A_0, 2, 1)) - Conversion.Val(Strings.Mid(A_1, 2, 1))))); // 8
	int num3 = checked((int)Math.Round(unchecked(Conversion.Val(Strings.Mid(A_0, 3, 1)) - Conversion.Val(Strings.Mid(A_1, 3, 1))))); // 6
	int num4 = checked((int)Math.Round(unchecked(Conversion.Val(Strings.Mid(A_0, 4, 1)) - Conversion.Val(Strings.Mid(A_1, 4, 1))))); // 8
	if (num1 < 0)
		num1 = checked(10 + num1);
	if (num2 < 0)
		num2 = checked(10 + num2);
	if (num3 < 0)
		num3 = checked(10 + num3);
	if (num4 < 0)
		num4 = checked(10 + num4);
	return (object)(Strings.Format((object)num1, "0") + Strings.Format((object)num2, "0") + Strings.Format((object)num3, "0") + Strings.Format((object)num4, "0"));
}

string a1(object A_0)
{
	int num1 = checked((int)Math.Round(Conversion.Val(Strings.Mid(Conversions.ToString(A_0), 1, 1)))) ^ checked((int)Math.Round(Conversion.Val(Strings.Mid(Conversions.ToString(A_0), 4, 1))));
	if (num1 >= 10)
		checked { num1 -= 10; }
	int num2 = checked((int)Math.Round(Conversion.Val(Strings.Mid(Conversions.ToString(A_0), 2, 1))));
	int num3 = checked((int)Math.Round(Conversion.Val(Strings.Mid(Conversions.ToString(A_0), 3, 1))));
	int num4 = checked(num2 - num1);
	if (num4 < 0)
		num4 = checked(10 + num4);
	int num5 = checked(num3 - num1);
	if (num5 < 0)
		num5 = checked(10 + num5);
	return Conversions.ToString(Strings.Chr(checked(num4 * 10 + num5 + 35)));
}

object d(string A_0)
{
	object Left = (object)"";
	int num = Strings.Len(A_0);
	int Start = 1;
	while (Start <= num)
	{
		string str = a1((object)Conversions.ToString(b(Strings.Left(Strings.Mid(A_0, Start, checked(Start + 5)), 4), "5868")));

		Left = Operators.AddObject(Left, (object)str);
		checked { Start += 5; }
	}
	return Left;
}

string Encode(char chr)
{
	var c = Convert.ToInt32(chr);
	var a = c - 35;
	var num1 = 9;
	var num4 = a / 10;
	var num5 = a % 10;
	var num2 = num1 + (num4 - 10);
	var num3 = num1 + (num5 - 10);

	var code2 = num2 + 8;
	if (code2 >= 10) code2 -= 10;

	var code3 = num3 + 6;
	if (code3 >= 10) code3 -= 10;

	return $"3{code2}{code3}9";
}

var result = string.Join("-", encodedEmail.Select(c => Encode(c)));

result.Length.Dump();
result.Dump();

d(result).Dump();