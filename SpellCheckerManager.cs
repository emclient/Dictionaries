using System.Collections.Generic;
using System.IO;
using MailClient.Collections;
using NHunspell;
using System.Globalization;

namespace MailClient.Dictionaries
{
	public static class SpellCheckerManager
	{
		private static WeakDictionary<string, Hunspell> spellCheckerCultures = new WeakDictionary<string, Hunspell>(128);

		public static void AddWordToCustomDictionary(SpellCheckerSettings settings, string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			string culture = Path.GetFileNameWithoutExtension(settings.ActiveFilePair.DictFile);
			AddWordToCustomDictionary(culture, word);
		}

		public static void AddWordToCustomDictionary(Hunspell activeSpellChecker, string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			foreach (KeyValuePair<string, Hunspell> kvp in spellCheckerCultures)
			{
				if (kvp.Value == activeSpellChecker)
				{
					AddWordToCustomDictionary(kvp.Key, word);
					break;
				}
			}
		}

		private static void AddWordToCustomDictionary(string culture, string word)
		{
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
				System.Diagnostics.Debug.Assert(false);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		public static Hunspell CreateSpellChecker(SpellCheckerSettings settings)
		{
			string culture = Path.GetFileNameWithoutExtension(settings.ActiveFilePair.DictFile);

			if (spellCheckerCultures.ContainsKey(culture))
			{
				Hunspell spell = spellCheckerCultures[culture];

				if (spell != null && !spell.IsDisposed)
					return spell;
				else
					spellCheckerCultures.Remove(culture);
			}

			Hunspell spellChecker = new Hunspell(settings.ActiveFilePair.AffFile, settings.ActiveFilePair.DictFile);

			//load words from custom dictionary
			string customDictPath = Path.Combine(Program.DataStore.Location, culture + ".custdic");


			CultureInfo cultureInfo = null;

			if (File.Exists(customDictPath))
			{
				StreamReader reader = null;
				try
				{
					reader = new StreamReader(Microsoft.Experimental.IO.LongPathFile.Open(customDictPath, FileMode.Open, FileAccess.Read, FileShare.Read));

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
										cultureInfo = new CultureInfo(culture);
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
					System.Diagnostics.Debug.Assert(false);
				}
				finally
				{
					if (reader != null)
						reader.Close();
				}
			}
			
			spellCheckerCultures.Add(culture, spellChecker);

			return spellChecker;
		}

	}
}
