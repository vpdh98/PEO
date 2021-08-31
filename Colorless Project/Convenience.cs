using System;
using System.Reflection;

public static class Convenience{
	private static int logNum = 1;
	
	public static void print(String s){
		Console.WriteLine(s);
	}
	
	public static void testLog(String s){
		Console.WriteLine(logNum+":"+s);
		logNum++;
	}
}
