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
	
	public static bool isEmptyList<T>(List<T> list,bool print = false){
		if(list == null){
			if(print) Console.WriteLine("이 리스트는 null 입니다.");
			return false;
		}
		if(!list.Any() || list.Count == 0){
			if(print) Console.WriteLine("이 리스트는 비었습니다.");
			return false;
		}else{
			if(print) Console.WriteLine("이 리스트는 비지 않았습니다.");
			return true;
		}
	}
	
	public static List<T> Copy<T>(List<T> o) where T:ICloneable {
		List<T> clone = new List<T>();
		foreach(T i in o){
			clone.Add((T)i.Clone());
		}
		return clone;
	}
	
	public static void PrintList<T>(List<T> list){
		for(int i = 0;i<list.Count;i++){
			Console.WriteLine("[{0}]: {1}",i,list[i].ToString());
		}
	}
	
	public static void PrintList<T,G>(Dictionary<T,G> dictionary){
		int count = 0;
		foreach(var d in dictionary){
			Console.WriteLine("[{0}]: {1}",count,d.ToString());
		}
	}
}
