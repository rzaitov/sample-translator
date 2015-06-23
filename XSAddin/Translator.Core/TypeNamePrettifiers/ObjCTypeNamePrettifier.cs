using System;

namespace Translator.Core
{
	public class ObjCTypeNamePrettifier
	{
		readonly string idTypeName;

		public ObjCTypeNamePrettifier (string idTypeName)
		{
			this.idTypeName = idTypeName;
		}

		public string Prettify (string rawTypeName)
		{
			if (rawTypeName == "id")
				return idTypeName;

			return rawTypeName;
		}
	}
}

