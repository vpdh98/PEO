using System;

public static class Parse{
	
	public static T ParseEnum<T>(String enumString) where T : struct{
		T temp;
		Enum.TryParse(enumString,out temp);
		return temp;
	}
}