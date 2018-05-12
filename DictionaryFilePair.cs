using System.IO;

namespace MailClient.Dictionaries
{
	public class DictionaryFilePair
	{
		readonly string affFile;
		readonly string dictFile;
		readonly string culture;

		public string AffFile
		{
			get { return affFile; }
		}

		public string DictFile
		{
			get { return dictFile; }
		}

		public string CultureString
		{
			get { return culture; }
		}

		public DictionaryFilePair()
		{
			affFile = dictFile = culture = string.Empty;
		}

		public DictionaryFilePair(string affFile, string dictFile)
		{
			this.affFile = affFile;
			this.dictFile = dictFile;

			culture = Path.GetFileNameWithoutExtension(dictFile);
		}

		public static DictionaryFilePair FromFileName(string fileName)
		{
			string path = Path.GetDirectoryName(fileName);
			string name = Path.GetFileNameWithoutExtension(fileName);

			return new DictionaryFilePair(
				Path.Combine(path, name + ".aff"),
				Path.Combine(path, name + ".dic"));
		}

		public override int GetHashCode()
		{
			return affFile.GetHashCode() ^ dictFile.GetHashCode();
		}
		public override bool Equals(object obj)
		{
#if MAC
			return obj is DictionaryFilePair pair && pair.CultureString == this.culture;
#else
			if (obj is DictionaryFilePair)
			{
				DictionaryFilePair pair = (DictionaryFilePair)obj;
				return this.affFile == pair.affFile && this.dictFile == pair.dictFile;
			}
			return false;
#endif
		}
	}
}
