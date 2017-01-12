using System;
using System.Globalization;

namespace MailClient.Dictionaries
{
	public class DictionaryFileSetting : IDictionarySetting
	{
		DictionaryFilePair filePair;
		CultureInfo culture;

		public DictionaryFileSetting(DictionaryFilePair filePair)
		{
			this.filePair = filePair;
			this.culture = new CultureInfo(filePair.CultureString);
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
			return culture != null ? culture.DisplayName : string.Empty;
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
