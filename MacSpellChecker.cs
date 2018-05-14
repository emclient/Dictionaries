#if MAC

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms.Mac;

#if XAMARINMAC
using AppKit;
using Foundation;
using ObjCRuntime;
#elif MONOMAC
using MonoMac.Foundation;
using MonoMac.AppKit;
#endif

namespace MailClient.Dictionaries
{
	public class MacSpellChecker : ISpellChecker
	{
		NSSpellChecker checker;

		public MacSpellChecker()
		{
			checker = NSSpellChecker.SharedSpellChecker;
		}

		public bool Add(string word)
		{
			if (!checker.HasLearnedWord(word))
				checker.LearnWord(word);
			return true;
		}

		public bool Remove(string word)
		{
			if (checker.HasLearnedWord(word))
				checker.UnlearnWord(word);
			return true;
		}

		public bool Spell(string word)
		{
			var range = checker.CheckSpelling(word, 0);
			return range.Location == NSRange.NotFound;
		}

		public List<string> Suggest(string word)
		{
			string language = checker.Language;
			var guesses = checker.GuessesForWordRange(new NSRange(0, word.Length), word, language, 0) ?? new string[] { };
			return new List<string>(guesses);
		}

		public List<string> Suggest(string word, int max)
		{
			string language = checker.Language;
			var guesses = checker.GuessesForWordRange(new NSRange(0, word.Length), word, language, 0) ?? new string[] { };
			return new List<string>(guesses.Take(Math.Min(max, guesses.Length)));
		}

		public string Language
		{
			get
			{
				return checker.AutomaticallyIdentifiesLanguages ? "auto" : LanguageToCulture(checker.Language);
			}

			set
			{
				var code = CultureToLanguage(value);
				if (-1 != AvailableLanguages.IndexOf(code))
				{
					// Language codes of dictionaries installed by system usually contain underscore
					checker.AutomaticallyIdentifiesLanguages = false;
					checker.Language = code;
				}
				else if (-1 != AvailableLanguages.IndexOf(value))
				{
					// Language codes of dictionaries installed by user (or by us) may contain dash
					checker.AutomaticallyIdentifiesLanguages = false;
					checker.Language = value;
				}
				else
				{
					checker.AutomaticallyIdentifiesLanguages = true;
				}
			}
		}

		public virtual List<string> AvailableLanguages
		{
			get
			{
				var list = new List<string>();
				foreach (var lang in checker.AvailableLanguages)
				{
					var code = LanguageToCulture(lang);
					list.Add(code);
				}
				return list;
			}
		}

		public virtual bool SupportsLanguageIdentification
		{
			get { return true; }
		}

		public virtual bool AutomaticallyIdentifiesLanguages {
			get { return checker.AutomaticallyIdentifiesLanguages; }
			set { checker.AutomaticallyIdentifiesLanguages = value; }
		}

		public static string LanguageToCulture(string value)
		{
			if (value == "en" && !NSSpellChecker.SharedSpellChecker.AvailableLanguages.Contains("en-US"))
				return "en-US";

			return value.Replace('_', '-');
		}

		public static string CultureToLanguage(string value)
		{
			var available = NSSpellChecker.SharedSpellChecker.AvailableLanguages;
			if (available.Contains(value)) // user-downloaded items can contain dashes.
				return value;

			if (value == "en-US")
				return "en";

			return value.Replace('-', '_');
		}

		static IntPtr clsNSLocale = Class.GetHandle(typeof(NSLocale));
		static IntPtr selWindowsLocaleCodeFromLocaleIdentifier = Selector.GetHandle("windowsLocaleCodeFromLocaleIdentifier:");
		static IntPtr selLocaleIdentifierFromWindowsLocaleCode = Selector.GetHandle("localeIdentifierFromWindowsLocaleCode:");

		public static int WindowsLocaleCodeFromLocaleIdentifier(string value)
		{
			var val = NSString.CreateNative(value);
			var res = LibObjc.Int32_objc_msgSend_IntPtr(clsNSLocale, selWindowsLocaleCodeFromLocaleIdentifier, val);
			NSString.ReleaseNative(val);
			return res;
		}

		public virtual string LocaleIdentifierFromWindowsLocaleCode(string value)
		{
			var code = CultureInfo.GetCultureInfo(value).LCID;
			var res = LibObjc.IntPtr_objc_msgSend_Int32(clsNSLocale, selLocaleIdentifierFromWindowsLocaleCode, code);
			return NSString.FromHandle(res);
		}

		public virtual bool IsDisposed
		{
			get
			{
				return false;
			}
		}

		#region Internals

		// FIXME:
		internal static ISpellChecker Create(SpellCheckerSettings settings)
		{
			return new MacSpellChecker();
		}

		#endregion
	}
}

#endif
