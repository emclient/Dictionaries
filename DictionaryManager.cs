using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.Experimental.IO;

#if XAMARINMAC
using AppKit;
#elif MONOMAC
using MonoMac.AppKit;
#endif

namespace MailClient.Dictionaries
{
	public static class DictionaryManager
	{
		private static string builtinDictFolder;
		private static string userDictFolder;
		private static List<DictionaryFilePair> files;
		private static List<DictionaryFileSetting> fileSettings;

		public static string UserDictionaryDir
		{
			get { return userDictFolder; }
		}

		static DictionaryManager()
		{
			builtinDictFolder = Path.Combine(Program.Directory, "Dictionaries");
#if MAC
			userDictFolder = Path.Combine(MacApi.FoundationStatic.LibraryDirectory, "Spelling");
#else
			userDictFolder = Path.Combine(Program.DataStore.Location, "Dictionaries");
#endif
		}


		public static IEnumerable<DictionaryFileSetting> FileSettings
		{
			get
			{
				if (fileSettings == null)
					ReloadFiles();

				foreach (DictionaryFileSetting fileSetting in fileSettings)
				{
					yield return fileSetting;
				}
			}
		}

		public static IEnumerable<DictionaryFilePair> DictFiles
		{
			get
			{
				if (files == null)
					ReloadFiles();

				return files;
			}
		}


		/// <summary>
		/// Tries to locate dictionary file best matching the culture.
		/// Otherwise the default dictionary is returned.
		/// </summary>
		/// <param name="culture">example (en-US)</param>
		/// <returns></returns>
		public static DictionaryFilePair FindBestDictionary(CultureInfo culture)
		{
			CultureInfo ancestorCulture;
			do
			{
				DictionaryFilePair pair;
				if (TryCreateDictionaryPair(culture.Name, out pair))
					return pair;
				ancestorCulture = culture;
			} while (!CultureInfo.InvariantCulture.Equals(culture = culture.Parent));

			// try to find default culture
			try
			{
				CultureInfo specific = CultureInfo.CreateSpecificCulture(ancestorCulture.Name);

				DictionaryFilePair pair;
				if (TryCreateDictionaryPair(specific.ToString(), out pair))
					return pair;
			}
			catch (ArgumentException)
			{
				// Because zh-CHT has ancestor zh-Hant, which does not have a specific culture associated with it. 
				System.Diagnostics.Debug.Assert(false);
			}
			// any dictionary with the same language
			foreach (DictionaryFilePair filePair in DictFiles)
			{
				try
				{
					if (filePair.CultureString.StartsWith(ancestorCulture.Name, StringComparison.InvariantCultureIgnoreCase))
						return filePair;
				}
				catch (ArgumentException)
				{ }
			}

			// return default
			return DefaultDictionary;
		}

		public static DictionaryFilePair DefaultDictionary
		{
			get
			{
				DictionaryFilePair pair;
				TryCreateDictionaryPair("en-US", out pair);
				return pair;
			}
		}
#if !MAC
		public static bool TryCreateDictionaryPair(string culture, out DictionaryFilePair pair)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 10)
			{
				// sr-Latn-CS and sr-Cyrl-CS are no longer used. sr-Latn-RS and sr-Cyrl-RS are used instead,
				// but our dictionaries are still named in the old-fashioned way
				if (culture.StartsWith("sr") && culture.EndsWith("RS"))
					culture = culture.Substring(0, culture.Length - 2) + "CS";
			}

			string fileName = String.Format("{0}.dic", culture);

			// try to find it firstly in the built-in dictionary folder, secondly in the user folder
			string path = Path.Combine(builtinDictFolder, fileName);

			if (LongPathFile.Exists(path))
			{
				fileName = path;
				pair = DictionaryFilePair.FromFileName(fileName);
				return true;
			}
			else
			{
				path = Path.Combine(userDictFolder, fileName);

				if (LongPathFile.Exists(path))
				{
					fileName = path;
					pair = DictionaryFilePair.FromFileName(fileName);
					return true;
				}
				else
				{
					pair = new DictionaryFilePair();
					return false;
				}
			}
		}
#else
		public static bool TryCreateDictionaryPair(string culture, out DictionaryFilePair pair)
		{
			var langs = NSSpellChecker.SharedSpellChecker.AvailableLanguages;
			foreach (var lang in langs)
			{
				try
				{
					var code = MacSpellChecker.LanguageToCulture(lang);
					if (code == culture)
					{
						pair = DictionaryFilePair.FromFileName(code);
						return true;
					}
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine($"Failed creating dictionary file pair for culture '{culture}'");
				}
			}

			pair = new DictionaryFilePair();
			return false;
		}

#endif

		public static void ReloadFiles()
		{
			files = new List<DictionaryFilePair>();
			fileSettings = new List<DictionaryFileSetting>();

			LoadBuiltInDictionaries();
			LoadUserDictionaries();
		}

#if MAC
		static void LoadBuiltInDictionaries()
		{
			var langs = NSSpellChecker.SharedSpellChecker.AvailableLanguages;
			foreach(var lang in langs)
			{
				var code = MacSpellChecker.LanguageToCulture(lang);
				try
				{
					var dfp = DictionaryFilePair.FromFileName(code);
					var dfs = new DictionaryFileSetting(dfp);
					files.Add(dfp);
					fileSettings.Add(dfs);
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine($"Failed adding dictionary for culture '{code}'");
				}
			}
		}

		static void LoadUserDictionaries()
		{
		}
#else
		static void LoadBuiltInDictionaries()
		{
			if (LongPathDirectory.Exists(builtinDictFolder))
				loadDictionariesFromFolder(builtinDictFolder);
		}

		static void LoadUserDictionaries()
		{
			if (LongPathDirectory.Exists(userDictFolder))
				loadDictionariesFromFolder(userDictFolder);
		}
#endif
		private static void loadDictionariesFromFolder(string folder)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(folder);
			List<FileInfo> dicFiles = new List<FileInfo>(dirInfo.GetFiles("*.dic"));
			foreach (FileInfo fileInfo in dirInfo.GetFiles("*.aff"))
			{
				foreach (FileInfo fi in dicFiles)
				{
					if (Path.GetFileNameWithoutExtension(fileInfo.Name).Equals(Path.GetFileNameWithoutExtension(fi.Name)))
					{
						DictionaryFilePair dfp = new DictionaryFilePair(fileInfo.FullName, fi.FullName);
						files.Add(dfp);
						try
						{
							DictionaryFileSetting fileSetting = new DictionaryFileSetting(dfp);
							if (!fileSettings.Contains(fileSetting))
								fileSettings.Add(fileSetting);
						}
						catch (Exception)
						{ }

					}
				}
			}
		}

	}
}
