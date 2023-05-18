namespace Sample_Demo
{
	using System.Linq;

	public class Helper
	{
		public static string ConvertInputToSingleQuotes( string serviceTags )
		{
			string[ ] strA = serviceTags.Trim( ).Split( ',' );
			string singleQuotesStrings = "";
			for ( int i = 0; i < strA.Length; i++ )
			{
				singleQuotesStrings += "'" + strA[i] + "'";
				if ( i < strA.Length - 1 )
				{
					singleQuotesStrings += ",";
				}
			}
			return singleQuotesStrings;
		}
	}
}
