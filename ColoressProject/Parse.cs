using System;

public static class Parse{
	public static ConsoleColor ParseConsoleColor(String color){
		ConsoleColor temp;
		Enum.TryParse(color,out temp);
		return temp;
	}
	
	public static TextLayout ParseTextLayout(String textLayout){
		TextLayout temp;
		Enum.TryParse(textLayout,out temp);
		return temp;
	}
}