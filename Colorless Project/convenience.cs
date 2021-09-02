using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public static class Convenience{
	private static int logNum = 1;
	
	public static void print(String s){
		Console.WriteLine(s);
	}
	
	public static void testLog(String s){
		Console.WriteLine(logNum+":"+s);
		logNum++;
	}
	
	public static void testLog(int n){
		Console.WriteLine(logNum+":"+n);
		logNum++;
	}
	
	public static void testLog(bool b){
		Console.WriteLine(logNum+":"+b);
		logNum++;
	}
	
	public static bool isEmptyList<T>(List<T> list){
		if(list == null){
			Console.WriteLine("이 리스트는 null 입니다.");
			return false;
		}
		if(list.Any()){
			Console.WriteLine("이 리스트는 비었습니다.");
			return false;
		}else{
			Console.WriteLine("이 리스트는 비지 않았습니다.");
			return true;
		}
	}
}
