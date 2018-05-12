using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using MailClient.Collections;
using Microsoft.Experimental.IO;
using NHunspell;

namespace MailClient.Dictionaries
{
	public class HunspellChecker : ISpellChecker
	{
		static WeakDictionary<string, Hunspell> spellCheckers = new WeakDictionary<string, Hunspell>(128);

		protected Hunspell currentSpellChecker;
		protected string culture;

		public HunspellChecker(string culture, Hunspell hunspell)
		{
			this.currentSpellChecker = hunspell;
			this.culture = culture;
		}

		public virtual bool Add(string word)
		{
			var result = currentSpellChecker.Add(word);
			AddWordToCustomDictionary(word);
			return result;
		}

		public virtual bool Remove(string word)
		{
			var result = currentSpellChecker.Remove(word);
			RemoveWordFromCustomDictionary(word);
			return result;
		}

		public virtual bool Spell(string word)
		{
			return currentSpellChecker.Spell(word);
		}

		public virtual List<string> Suggest(string word)
		{
			return currentSpellChecker.Suggest(word);
		}

		public virtual List<string> Suggest(string word, int max)
		{
			return currentSpellChecker.Suggest(word, max);
		}

		public virtual string Language
		{
			get
			{
				return Language;
			}

			set
			{
				if (culture != value && spellCheckers.TryGetValue(value, out Hunspell hunspell))
				{
					culture = value;
					currentSpellChecker = hunspell;
				}
			}
		}

		public virtual List<string> AvailableLanguages
		{
			get
			{
				var list = new List<string>();
				foreach (var file in DictionaryManager.DictFiles)
					list.Add(file.CultureString);

				return list;
			}
		}

		public virtual bool SupportsLanguageIdentification
		{
			get { return false; }
		}

		public virtual bool AutomaticallyIdentifiesLanguages
		{
			get { return false; }
			set { }
		}

		public virtual bool IsDisposed
		{
			get
			{
				return currentSpellChecker.IsDisposed;
			}
		}

		#region Internals 

		internal static ISpellChecker Create(SpellCheckerSettings settings)
		{
			string culture = Path.GetFileNameWithoutExtension(settings.ActiveFilePair.DictFile);

			if (spellCheckers.ContainsKey(culture))
			{
				var spell = spellCheckers[culture];
				if (spell != null && !spell.IsDisposed)
					return new HunspellChecker(culture, spell);
				else
					spellCheckers.Remove(culture);
			}

			if (!CheckIfActiveDictionaryExists(settings.ActiveFilePair))
				return null;

			var spellChecker = new Hunspell(settings.ActiveFilePair.AffFile, settings.ActiveFilePair.DictFile);

			//load words from custom dictionary
			string customDictPath = Path.Combine(Program.DataStore.Location, culture + ".custdic");

			CultureInfo cultureInfo = null;
			if (File.Exists(customDictPath))
			{
				StreamReader reader = null;
				try
				{
					reader = new StreamReader(LongPathFile.Open(customDictPath, FileMode.Open, FileAccess.Read, FileShare.Read));

					string line;
					do
					{
						line = reader.ReadLine();
						if (!string.IsNullOrEmpty(line))
						{
							if (!spellChecker.Add(line))
							{
								if (cultureInfo == null)
								{
									try
									{
										cultureInfo = CultureInfo.GetCultureInfo(culture);
									}
									catch
									{
										cultureInfo = CultureInfo.InvariantCulture;
									}
								}

								spellChecker.Add(line.ToLower(cultureInfo));
							}
						}
					} while (line != null);
				}
				catch
				{
					Debug.Assert(false);
				}
				finally
				{
					if (reader != null)
						reader.Close();
				}
			}

			spellCheckers.Add(culture, spellChecker);
			return new HunspellChecker(culture, spellChecker);
		}

		internal static bool CheckIfActiveDictionaryExists(DictionaryFilePair pair)
		{
			return !string.IsNullOrEmpty(pair.DictFile) && !string.IsNullOrEmpty(pair.AffFile)
					&& LongPathFile.Exists(pair.DictFile) && LongPathFile.Exists(pair.AffFile);
		}

		internal virtual void AddWordToCustomDictionary(SpellCheckerSettings settings, string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			string culture = Path.GetFileNameWithoutExtension(settings.ActiveFilePair.DictFile);
			AddWordToCustomDictionary(culture, word);
		}

		internal virtual void RemoveWordFromCustomDictionary(string word)
		{
			Debug.WriteLine($"NotImplemented: {System.Reflection.MethodBase.GetCurrentMethod().Name}");
		}

		internal virtual void AddWordToCustomDictionary(string word)
		{
			AddWordToCustomDictionary(this.culture, word);
		}

		internal virtual void AddWordToCustomDictionary(string culture, string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			string customDictPath = Path.Combine(Program.DataStore.Location, culture + ".custdic");
			StreamWriter writer = null;
			try
			{
				writer = File.AppendText(customDictPath);

				writer.WriteLine(word);
				writer.Flush();
			}
			catch
			{
				Debug.Assert(false);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		#endregion
	}
}
