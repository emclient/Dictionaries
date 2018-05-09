using System;
using System.IO;
using Microsoft.Experimental.IO;
using NHunspell;

namespace MailClient.Dictionaries
{
	public abstract class SpellCheckerSettings
	{
		private static SpellCheckerEmailSettings emailSettings;
		
		public static SpellCheckerEmailSettings EmailSettings
		{
			get {
				if (emailSettings == null)
					emailSettings = new SpellCheckerEmailSettings();
				return emailSettings;
			}
		}

		private static SpellCheckerGeneralSettings generalSettings;

		public static SpellCheckerGeneralSettings GeneralSettings
		{
			get
			{
				if (generalSettings == null)
					generalSettings = new SpellCheckerGeneralSettings();
				return generalSettings;
			}
		}

		private static SpellCheckerIMSettings imSettings;

		public static SpellCheckerIMSettings IMSettings
		{
			get
			{
				if (imSettings == null)
					imSettings = new SpellCheckerIMSettings();
				return imSettings;
			}
		}



		private IDictionarySetting activeSetting;


		public bool UseSpellChecker
		{
			get
			{
				return Program.Settings.GetValue(UseSpellCheckSettingsName, true);
			}
			set
			{
				Program.Settings.SetValue(UseSpellCheckSettingsName, value);
			}
		}

		internal IDictionarySetting ActiveSetting
		{
			get
			{
				if (activeSetting == null)
					activeSetting = LoadSetting();
				return activeSetting;
			}
			set
			{
				if (!value.Equals(activeSetting))
				{
					activeSetting = value;
					if (!value.FilePair.CultureString.Equals(string.Empty))
					{
						saveSetting(activeSetting);
					}
				}
			}
		}

		internal DictionaryFilePair ActiveFilePair
		{
			get
			{
				if (ActiveSetting != null)
					return ActiveSetting.FilePair;
				else
					return null;
			}
		}


		public ISpellChecker GetSpellChecker()
		{
			if (ActiveFilePair != null)
			{
				if (CheckIfActiveDictionaryExists())
				{
					FileInfo dictInfo = new FileInfo(ActiveFilePair.DictFile);
					FileInfo affInfo = new FileInfo(ActiveFilePair.AffFile);

					if ((dictInfo.Attributes & FileAttributes.Directory) == 0 &&
						(affInfo.Attributes & FileAttributes.Directory) == 0)
					{
						return SpellCheckerManager.CreateSpellChecker(this);
					}
				}
			}

			return null;
		}

		public bool CheckIfActiveDictionaryExists()
		{
			if (!string.IsNullOrEmpty(ActiveFilePair.DictFile) &&
				!string.IsNullOrEmpty(ActiveFilePair.AffFile) &&
				LongPathFile.Exists(ActiveFilePair.DictFile) &&
				LongPathFile.Exists(ActiveFilePair.AffFile))
			{
				return true;
			}

			return false;
		}



		public IDictionarySetting LoadSetting()
		{
			DictionarySettingType type = (DictionarySettingType)MailClient.Program.Settings.GetValue(TypeSettingsName, (int)DefaultDictionarySettingType);
			switch (type)
			{
				case DictionarySettingType.File:
					string fileName = MailClient.Program.Settings.GetValue(SpellCheckerFileSettingsName, "en-US.dic");
					string culture = fileName;
					if (culture.EndsWith(".dic", StringComparison.InvariantCultureIgnoreCase))
						culture = culture.Substring(0, culture.Length - 4);

					DictionaryFilePair pair;
                    DictionaryManager.TryCreateDictionaryPair(culture, out pair);
					return new DictionaryFileSetting(pair);
				case DictionarySettingType.Keyboard:
					return new DictionaryKeyboardSetting();
				case DictionarySettingType.Language:
					return new DictionaryLanguageSetting();
				case DictionarySettingType.UseGeneral:
					return new DictionaryUseGeneralSetting();

				default:
					throw new NotImplementedException("Missing Spellcheck Dictionary Setting.");
			}
		}

		private void saveSetting(IDictionarySetting setting)
		{
			MailClient.Program.Settings.SetValue(
				   TypeSettingsName,
				   (int)setting.Type);

			MailClient.Program.Settings.SetValue(
				SpellCheckerFileSettingsName,
				Path.GetFileName(setting.FilePair.DictFile));	// store only the name without path to maintain backwards compatibility
		}


		protected abstract string UseSpellCheckSettingsName { get; }
		protected abstract string TypeSettingsName { get; }
		protected abstract string SpellCheckerFileSettingsName { get; }
		protected abstract DictionarySettingType DefaultDictionarySettingType { get; }
	}
}