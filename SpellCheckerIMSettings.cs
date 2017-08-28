namespace MailClient.Dictionaries
{
	public class SpellCheckerIMSettings : SpellCheckerSettings
	{
		internal SpellCheckerIMSettings()
		{
		}

		protected override string UseSpellCheckSettingsName
		{
			get
			{
				return "UseIMSpellchecker";
			}
		}
		protected override string TypeSettingsName
		{
			get
			{
				return "SpellCheckerIMType";
			}
		}
		protected override string SpellCheckerFileSettingsName
		{
			get
			{
				return "SpellCheckerIMFile";
			}
		}
		protected override DictionarySettingType DefaultDictionarySettingType
		{
			get
			{
				return DictionarySettingType.UseGeneral;
			}
		}	
	}
}
