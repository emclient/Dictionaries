using System;
using System.Globalization;

namespace MailClient.Dictionaries
{
	public class DictionaryFileSetting : IDictionarySetting
	{
		DictionaryFilePair filePair;
		string displayName;

		public DictionaryFileSetting(DictionaryFilePair filePair)
		{
			this.filePair = filePair;
		}

		public DictionarySettingType Type
		{
			get { return DictionarySettingType.File; }
		}

		public DictionaryFilePair FilePair
		{
			get { return filePair; }
		}

		public override string ToString()
		{
			if (displayName == null)
			{
				try
				{
					var culture = CultureInfo.GetCultureInfo(filePair.CultureString);
					displayName = culture.DisplayName;
				}
				catch
				{
					displayName = filePair.CultureString;
				}
				displayName = displayName ?? String.Empty;
			}
			return displayName;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode() ^ filePair.CultureString.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (obj is DictionaryFileSetting)
				return this.FilePair.CultureString.Equals(((DictionaryFileSetting)obj).FilePair.CultureString, StringComparison.InvariantCultureIgnoreCase);
			
			return false;
		}
	}
}
