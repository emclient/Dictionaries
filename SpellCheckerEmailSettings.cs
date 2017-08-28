namespace MailClient.Dictionaries
{
	public class SpellCheckerEmailSettings : SpellCheckerSettings
	{
		internal SpellCheckerEmailSettings()
		{
		}

		protected override string UseSpellCheckSettingsName
		{
			get
			{
				return "UseComposeSpellchecker";
			}
		}
		protected override string TypeSettingsName
		{
			get
			{
				return "SpellCheckerComposeType";
			}
		}
		protected override string SpellCheckerFileSettingsName
		{
			get
			{
				return "SpellCheckerComposeFile";
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
