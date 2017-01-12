using System.Threading;

namespace MailClient.Dictionaries
{
	public class DictionaryLanguageSetting : IDictionarySetting
	{
		public DictionaryLanguageSetting()
		{
		}

		public DictionarySettingType Type
		{
			get { return DictionarySettingType.Language; }
		}

		public DictionaryFilePair FilePair
		{
			get
			{
				string cultureName;
				if (Program.Settings.TryGetValue("Culture", out cultureName))
					return DictionaryManager.FindBestDictionary(System.Globalization.CultureInfo.GetCultureInfo(cultureName));
				return DictionaryManager.FindBestDictionary(Thread.CurrentThread.CurrentUICulture); ;
			}
		}

		public override string ToString()
		{
			return Resources.Dictionaries_base.ApplicationLanguage;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			DictionaryLanguageSetting culture = obj as DictionaryLanguageSetting;

			return culture != null;
		}
	}
}
