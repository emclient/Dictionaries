namespace MailClient.Dictionaries
{
	public class SpellCheckerIMSettings : SpellCheckerSettings
	{
		private static SpellCheckerIMSettings instance;

		public static SpellCheckerIMSettings Instance
		{
			get
			{
				lock (typeof(SpellCheckerIMSettings))
				{
					if (instance == null)
					{
						instance = new SpellCheckerIMSettings();
					}
					return instance;
				}
			}
		}
		

		private SpellCheckerIMSettings()
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
