#if MAC

using System;
using System.Collections.Generic;
using System.Linq;

#if XAMARINMAC
using AppKit;
using Foundation;
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
			var guesses = checker.GuessesForWordRange(new NSRange(0, word.Length), word, language, 0);
			return new List<string>(guesses);
		}

		public List<string> Suggest(string word, int max)
		{
			string language = checker.Language;
			var guesses = checker.GuessesForWordRange(new NSRange(0, word.Length), word, language, 0);
			return new List<string>(guesses.Take(Math.Min(max, guesses.Length)));
		}

		public string Language
		{
			get
			{
				return Language;
			}

			set
			{
				if (-1 != System.Array.IndexOf<string>(checker.AvailableLanguages, value))
					checker.Language = value;
			}
		}

		public List<string> AvailableLanguages
		{
			get
			{
				return new List<string>(checker.AvailableLanguages);
			}
		}

		public bool IsDisposed
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
