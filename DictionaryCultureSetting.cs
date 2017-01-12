using System;

namespace MailClient.Dictionaries
{
	public class DictionaryCultureSetting : IDictionarySetting
	{
		DictionaryFilePair filePair;

		public DictionaryCultureSetting()
		{
			filePair = DictionaryManager.FindBestDictionary(System.Globalization.CultureInfo.CurrentCulture);
		}

		public DictionarySettingType Type
		{
			get { return DictionarySettingType.Culture; }
		}

		public DictionaryFilePair FilePair
		{
			get { return filePair; }
		}

		public override string ToString()
		{
			return Resources.Dictionaries_base.SystemSettings;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			DictionaryCultureSetting culture = obj as DictionaryCultureSetting;

			return culture != null;
		}
	}
}
