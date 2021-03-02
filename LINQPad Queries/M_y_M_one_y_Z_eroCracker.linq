<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.Compatibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.dll</Reference>
  <Namespace>Microsoft.VisualBasic</Namespace>
  <Namespace>Microsoft.VisualBasic.CompilerServices</Namespace>
</Query>

// 從 13.4 版開始，會驗證信箱序號，先在登入畫面輸入使用者代號 2OLIOXOI，然後按 F12，停止信箱序號驗證。

M_y_M_one_y_Z_eroCracker.GenerateLicenseKey("johnny@wantgoo.com").Dump();

}
public class M_y_M_one_y_Z_eroCracker
{
	public static string GenerateLicenseKey(string email)
	{
		int num5;
		int num6;
		VBMath.Randomize();
		var str6 = "";
		var num4 = (int)Math.Round(Conversion.Int(25f * VBMath.Rnd() + 1f));
		var num10 = Strings.Len(email);
		for (var i = 1; i <= num10; i++)
		{
			int num8;
			num5 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));
			num6 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));
			while (num5 == num6)
			{
				num5 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));
				num6 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));
			}
			if (num5 > num6)
			{
				num8 = num5 - num6;
			}
			else
			{
				num8 = num6 - num5;
			}
			var str5 = Strings.Format(Strings.Asc(Strings.Mid(email, i, 1)), "000");
			var num = (int)Math.Round(Conversion.Val(Strings.Mid(str5, 1, 1)));
			var num2 = (int)Math.Round(Conversion.Val(Strings.Mid(str5, 2, 1)));
			var num3 = (int)Math.Round(Conversion.Val(Strings.Mid(str5, 3, 1)));
			var str = Conversions.ToString(Strings.Chr(num + 0x41 + num8));
			var str2 = Conversions.ToString(Strings.Chr(num2 + 0x41 + num8));
			var str3 = Conversions.ToString(Strings.Chr(num3 + 0x41 + num8));
			str6 = str6 + Conversions.ToString(Strings.Chr(num5 + 0x40)) + str + str2 +
				   Conversions.ToString(Strings.Chr(num6 + 0x40)) + str3;
		}
		num5 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));
		num6 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));
		var num7 = (int)Math.Round(Conversion.Int(26f * VBMath.Rnd() + 1f));

		return
			Conversions.ToString(Strings.Chr(num5 + 0x40)) + Conversions.ToString(Strings.Chr(num6 + 0x40)) +
			Conversions.ToString(Strings.Chr(num7 + 0x40)) + str6;
	}

	public static string DecodeLicenseKey(string licenseKey)
	{
		var str5 = "";
		var num10 = int.MinValue;
		try
		{
			int num11;
		Label_0001:
			ProjectData.ClearProjectError();
			var num9 = -2;
		Label_000A:
			num11 = 2;
			str5 = "";
		Label_0014:
			num11 = 3;
			StringType.MidStmtStr(ref licenseKey, 1, 1, " ");
		Label_0026:
			num11 = 4;
			StringType.MidStmtStr(ref licenseKey, 2, 1, " ");
		Label_0038:
			num11 = 5;
			StringType.MidStmtStr(ref licenseKey, 3, 1, " ");
		Label_004A:
			num11 = 6;
			licenseKey = Strings.Trim(licenseKey);
		Label_0055:
			num11 = 7;
			if (Strings.Len(licenseKey) % 5 == 0)
			{
				goto Label_007D;
			}
		Label_006C:
			num11 = 8;
			str5 = "";
			goto Label_02F4;
		Label_007D:
			num11 = 11;
			if (Strings.Len(licenseKey) >= 20)
			{
				goto Label_00A3;
			}
		Label_0091:
			num11 = 12;
			str5 = "";
			goto Label_02F4;
		Label_00A3:
			num11 = 15;
			if (Strings.Trim(licenseKey) != "")
			{
				goto Label_00D3;
			}
		Label_00C1:
			num11 = 0x10;
			str5 = "";
			goto Label_02F4;
		Label_00D3:
			num11 = 0x13;
			var num8 = Strings.Len(licenseKey);
			var start = 1;
			goto Label_01ED;
		Label_00E7:
			num11 = 20;
			var str = Strings.Mid(licenseKey, start, 5);
		Label_00F5:
			num11 = 0x15;
			var num4 = Strings.Asc(Strings.Mid(str, 1, 1));
		Label_0108:
			num11 = 0x16;
			var num5 = Strings.Asc(Strings.Mid(str, 4, 1));
		Label_011B:
			num11 = 0x17;
			var str2 = Strings.Mid(str, 2, 1);
		Label_0129:
			num11 = 0x18;
			var str3 = Strings.Mid(str, 3, 1);
		Label_0137:
			num11 = 0x19;
			var str4 = Strings.Mid(str, 5, 1);
		Label_0145:
			num11 = 0x1a;
			if (num4 <= num5)
			{
				goto Label_0162;
			}
		Label_0155:
			num11 = 0x1b;
			var num6 = num4 - num5;
			goto Label_0173;
		Label_0162:
			num11 = 0x1d;
		Label_0167:
			num11 = 30;
			num6 = num5 - num4;
		Label_0173:
			num11 = 0x20;
			var num = Strings.Asc(str2) - num6 - 0x41;
		Label_0185:
			num11 = 0x21;
			var num2 = Strings.Asc(str3) - num6 - 0x41;
		Label_0197:
			num11 = 0x22;
			var num3 = Strings.Asc(str4) - num6 - 0x41;
		Label_01A9:
			num11 = 0x23;
			str5 = str5 +
				   Conversions.ToString(
					   Strings.Chr(
						   (int)
						   Math.Round(
							   Conversion.Val(Conversions.ToString(num) + Conversions.ToString(num2) +
											  Conversions.ToString(num3)))));
		Label_01E2:
			num11 = 0x24;
			start += 5;
		Label_01ED:
			if (start <= num8)
			{
				goto Label_00E7;
			}
			goto Label_02F4;
		Label_0203:
			num10 = 0;
			switch (num10 + 1)
			{
				case 1:
					goto Label_0001;

				case 2:
					goto Label_000A;

				case 3:
					goto Label_0014;

				case 4:
					goto Label_0026;

				case 5:
					goto Label_0038;

				case 6:
					goto Label_004A;

				case 7:
					goto Label_0055;

				case 8:
					goto Label_006C;

				case 9:
				case 13:
				case 0x11:
				case 0x25:
					goto Label_02F4;

				case 10:
				case 11:
					goto Label_007D;

				case 12:
					goto Label_0091;

				case 14:
				case 15:
					goto Label_00A3;

				case 0x10:
					goto Label_00C1;

				case 0x12:
				case 0x13:
					goto Label_00D3;

				case 20:
					goto Label_00E7;

				case 0x15:
					goto Label_00F5;

				case 0x16:
					goto Label_0108;

				case 0x17:
					goto Label_011B;

				case 0x18:
					goto Label_0129;

				case 0x19:
					goto Label_0137;

				case 0x1a:
					goto Label_0145;

				case 0x1b:
					goto Label_0155;

				case 0x1c:
				case 0x1f:
				case 0x20:
					goto Label_0173;

				case 0x1d:
					goto Label_0162;

				case 30:
					goto Label_0167;

				case 0x21:
					goto Label_0185;

				case 0x22:
					goto Label_0197;

				case 0x23:
					goto Label_01A9;

				case 0x24:
					goto Label_01E2;

				default:
					goto Label_02E9;
			}
			num10 = num11;
			switch (num9 > -2 ? num9 : 1)
			{
				case 0:
					goto Label_02E9;

				case 1:
					goto Label_0203;
			}
		}
		catch (Exception obj1) //when (?)
		{
			ProjectData.SetProjectError(obj1);
			//goto Label_02A9;
			return string.Empty;
		}
	Label_02E9:
		throw ProjectData.CreateProjectError(-2146828237);
	Label_02F4:
		if (num10 != 0)
		{
			ProjectData.ClearProjectError();
		}
		return str5;
	}
