using System.Windows.Forms;
using System;
using System.Linq;
using MailClient.UI.Forms;

namespace MailClient.Dictionaries
{
	public class SpellCheckerHandler
	{
		// after the user changes dictionary on one place (eg. multiple formEvent forms opened), spellCheckers on other places are not updated, 
		// but it is already stored in the SpellCheckerSettings -> creating the contextMenu and setting the checked item could be wrong
		IDictionarySetting cachedSettings;
		SpellCheckerSettings settings;


		/// <summary>
		/// Occurs when the spellCheck is changed by an action coming from the outside of the app (eg. changing the keyboard language when the Active keyboard language setting is set)
		/// </summary>
		public event EventHandler SpellCheckerNeedsUpdate;

		/// <summary>
		/// Occurs when the user changes the spellChecker language by clicking on the context menu item.
		/// </summary>
		public event EventHandler SpellCheckerChanged;

		/// <summary>
		/// Occurs after dictionaries are downloaded.
		/// </summary>
		public event EventHandler DictionaryDownloaded;


		public SpellCheckerSettings SpellCheckerSettings
		{
			get
			{
				return settings;
			}
		}


		public SpellCheckerHandler(SpellCheckerSettings settings, Form parentForm)
		{
			this.settings = settings;

			this.cachedSettings = settings.ActiveSetting;

			if (settings.ActiveSetting is DictionaryKeyboardSetting || (settings.ActiveSetting is DictionaryUseGeneralSetting && SpellCheckerSettings.GeneralSettings.ActiveSetting is DictionaryKeyboardSetting))
				parentForm.InputLanguageChanged += parentForm_InputLanguageChanged;
		}

		public void CreateContextMenu(ContextMenuStrip contextMenu)
		{
			ToolStripItem dicMenuItem = null;
			ToolStripItem lastItem = null;
			foreach (ToolStripItem item in contextMenu.Items)
			{
				if ((item.Tag as string) == "dictionary")
					dicMenuItem = item;
				if (dicMenuItem == null && item.Available)
					lastItem = item;
			}

			if (dicMenuItem != null)
				contextMenu.Items.Remove(dicMenuItem);

			if (lastItem != null && !(lastItem is ToolStripSeparator))
			{
				contextMenu.Items.Add(new ToolStripSeparator());
			}

			ToolStripMenuItem dicts = new ToolStripMenuItem(Resources.UI.Controls_base.SpellCheckLanguage);
			dicts.Tag = "dictionary";
			contextMenu.Items.Add(dicts);

			CreateContextMenu(dicts);
		}

		public void CreateContextMenu(ToolStripMenuItem menuItem)
		{
			// do not assign the event multiple times (we do not know if we havent assigned it already, so try to remove it before)
			menuItem.DropDownItemClicked -= new ToolStripItemClickedEventHandler(SpellCheckMenu_DropDownItemClicked);
			menuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(SpellCheckMenu_DropDownItemClicked);

			var checker = this.SpellCheckerSettings.GetSpellChecker();
			if (checker != null && checker.SupportsLanguageIdentification)
			{
				var dict = new DictionaryAutoSetting();
				var item = new ToolStripMenuItem(Resources.UI.Forms.AutomaticByLanguage);
				item.Tag = dict;
				item.Checked = dict.FilePair.Equals(cachedSettings.FilePair);
				menuItem.DropDownItems.Add(item);
				menuItem.DropDownItems.Add(new ToolStripSeparator());
			}

			int count = 0;
			var dictionaries = DictionaryManager.FileSettings.OrderBy(dict => dict.ToString());
			foreach (DictionaryFileSetting dictionary in dictionaries)
			{
				count++;

				ToolStripMenuItem item = new ToolStripMenuItem(dictionary.ToString());
				item.Tag = dictionary;
				item.Checked = dictionary.FilePair.Equals(cachedSettings.FilePair);
				menuItem.DropDownItems.Add(item);
			}

			if (count > 0)
			{
				menuItem.DropDownItems.Add(new ToolStripSeparator());
			}

			var menuItemMore = new ToolStripMenuItem(Resources.UI.Forms.DownloadDictionaries);
			menuItemMore.Tag = "download";
			menuItem.DropDownItems.Add(menuItemMore);
		}

		void parentForm_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
		{
			if (SpellCheckerNeedsUpdate != null)
				SpellCheckerNeedsUpdate(this, EventArgs.Empty);
		}


		private void SpellCheckMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
			if (item != null)
			{
				if (item.Tag is IDictionarySetting)
				{
					settings.ActiveSetting = cachedSettings = (IDictionarySetting)item.Tag;
					if (SpellCheckerChanged != null)
						SpellCheckerChanged(this, EventArgs.Empty);
				}
				else if (item.Tag is string && ((string)item.Tag) == "download")
				{
					DownloadDictionary();
				}
			}
		}

		public void DownloadDictionary()
		{
			using (FormDownloadDictionary frm = new FormDownloadDictionary())
			{
				if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					if (DictionaryDownloaded != null)
						DictionaryDownloaded(this, EventArgs.Empty);
				}
			}
		}
	}
}
