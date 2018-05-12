using System;
using System.Collections.Generic;

namespace MailClient.Dictionaries
{
	public interface ISpellChecker
	{
		bool Add(string word);
		bool Remove(string word);
		bool Spell(string word);
		List<string> Suggest(string word);
		List<string> Suggest(string word, int max);

		List<string> AvailableLanguages { get; }
		string Language { get; set; }

		bool SupportsLanguageIdentification { get; }
		bool AutomaticallyIdentifiesLanguages { get; set; }

		bool IsDisposed { get; }
	}
}
