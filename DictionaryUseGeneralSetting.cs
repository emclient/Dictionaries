namespace MailClient.Dictionaries
{
	public class DictionaryUseGeneralSetting : IDictionarySetting
	{
		public DictionaryUseGeneralSetting()
		{
		}

		public DictionarySettingType Type
		{
			get { return DictionarySettingType.UseGeneral; }
		}

		public DictionaryFilePair FilePair
		{
			get { return SpellCheckerSettings.GeneralSettings.ActiveFilePair; }
		}

		public override string ToString()
		{
			return Resources.Dictionaries_base.UseDefault;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return (obj is DictionaryUseGeneralSetting);
		}
	}
}
