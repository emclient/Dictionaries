#if MAC

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using MacApi.Carbon;
using MailClient.UI;

namespace MailClient.Dictionaries
{
	public static class MacSpellCheckerUtility
	{
		public static CultureInfo CultureFromCurrentKeyboard()
		{
			try
			{
				var source = TextInputSource.CurrentKeyboardSource;
				var lang = source.FirstLanguage;
				lang = LanguageFromKeyboard(source.Identifier, lang);
				return CultureInfo.GetCultureInfo(lang);
			}
			catch
			{
				return System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture;
			}
		}

		public static string LanguageFromKeyboard(string identifier, string lang)
		{
			string value = null;
			if (null != (value = CustomMapping[identifier]))
			{
				if (SpellCheckerManager.instance != null && SpellCheckerManager.instance.AvailableLanguages.Contains(value))
					return value;
			}

			if (null != (value = DefaultMapping[identifier]))
				if (SpellCheckerManager.instance != null && SpellCheckerManager.instance.AvailableLanguages.Contains(value))
					return value;

			return lang;
		}

		static StringDictionary customMapping = null;
		static StringDictionary CustomMapping
		{
			get
			{
				if (customMapping == null)
					customMapping = LoadCustomMapping();
				return customMapping;
			}
		}

		static StringDictionary defaultMapping = null;
		static StringDictionary DefaultMapping
		{
			get
			{
				if (defaultMapping == null)
					defaultMapping = ParseMapping(defaultMappingText);
				return defaultMapping;
			}
		}

		static StringDictionary LoadCustomMapping()
		{
			try
			{
				if (File.Exists(CustomMappingPath))
					return ParseMapping(File.ReadAllText(CustomMappingPath));
			}
			catch
			{
				Debug.Assert(false, "Failed loading custom spellcheck mapping");
			}
			return new StringDictionary();
		}

		static void SaveCustomMapping()
		{
			try
			{
				var text = CustomMapping.Count != 0 ? ToString(CustomMapping) : customMappingTemplate; 
				File.WriteAllText(CustomMappingPath, text);
			}
			catch
			{
				Debug.Assert(false, "Failed saving custom spellcheck mapping");
			}
		}

		static string CustomMappingPath
		{
			get { return Path.Combine(CommonSettings.DefaultDBLocation, "spellcheck_mapping.txt"); }
		}

		static void DumpSources()
		{
			var sources = TextInputSource.List(true);
			foreach (var source in sources)
				Console.WriteLine($"{source.Identifier}, {source.FirstLanguage}, {source.LocalizedName}, {source.Category}, {source.Type}");
		}

		const string commentSeparator = "//";
		static readonly string[] comentSeparators = { commentSeparator };
		static readonly string[] lineSeparators = { "\r\n", "\r", "\n" };

		static string ToString(StringDictionary d)
		{
			var text = new StringBuilder();
			foreach (string key in d.Keys)
				text.AppendLine($"{key}={d[key]}");
			return text.ToString();
		}

		static StringDictionary ParseMapping(string text)
		{
			var d = new StringDictionary();

			var lines = text.Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);
			foreach(var line in lines)
			{
				var parts = line.Split('=');
				if (parts.Length > 1)
				{
					var key = parts[0].Trim();
					if (!key.StartsWith(commentSeparator, StringComparison.InvariantCulture))
					{
						var value = parts[1];
						if (value.IndexOf(commentSeparator, StringComparison.InvariantCulture) != -1)
							value = value.Split(comentSeparators, StringSplitOptions.None)[0];
						value = value.Trim();
						d[key] = value;
					}
				}
			}
			return d;
		}

		const string defaultMappingText =
@"com.apple.keylayout.British=en-GB
com.apple.keylayout.US=en-US
com.apple.keylayout.Canadian=en-CA
com.apple.keylayout.Australian=en-AU
com.apple.keylayout.Austrian=de-AT
com.apple.keylayout.SwissGerman=de-CH
com.apple.keylayout.Brazilian=pt-BR
";

		const string customMappingTemplate =
@"//Sample (remove slashes at the beginning of the line):
//com.apple.keylayout.Austrian=de-AT
";
	}
}

#endif

