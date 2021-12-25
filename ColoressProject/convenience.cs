using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Convenience{
	private static int logNum = 1;
	public static int y = 0;
	public static Object alwaysDisplay;
	
	public static bool WaitTimeOrInput(int count){
		for(int i = 0;i<count && !Console.KeyAvailable;i++){
				Thread.Sleep(10);
			}
		if(Console.KeyAvailable){
			Console.ReadKey();
			return true;
		}
		return false;
	}
	
	public static void DisplayLog(){
		Task.Factory.StartNew(()=>{
			while(true){
				Console.SetCursorPosition(0,y);
				Console.WriteLine("                       ");
				Console.SetCursorPosition(0,y);
				Console.WriteLine(logNum+":"+alwaysDisplay);
				Thread.Sleep(100);
			}
		});
	}
	
	
	public static void print(String s){
		Console.WriteLine(s);
	}
	
	public static void testLog(Object o,bool delay = true){
		if(logNum == 1){
			//DisplayLog();
		}
		if(delay){
			Console.SetCursorPosition(0,y);
			Console.WriteLine("                       ");
			Console.SetCursorPosition(0,y);
			Console.WriteLine(logNum+":"+o);
			Console.ReadKey();
			logNum++;
		}else{
			alwaysDisplay=o;
			logNum++;
		}
		
	}
	
	public static bool isEmptyList<T>(List<T> list,bool print = false){
		if(list == null){
			if(print) Console.WriteLine("이 리스트는 null 입니다.");
			//Console.ReadKey();
			return true;
		}
		if(!list.Any() || list.Count == 0){
			if(print) Console.WriteLine("이 리스트는 비었습니다.");
			//Console.ReadKey();
			return true;
		}else{
			if(print) Console.WriteLine("이 리스트는 비지 않았습니다.");
			//Console.ReadKey();
			return false;
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
	
	public static T FindKeyByValue<T,G>(Dictionary<T,G> dictionary,G toFind){
		foreach (KeyValuePair<T,G> pair in dictionary){
			if(pair.Value.Equals(toFind)){
				return pair.Key;
			}
		}
		return default(T);
	}
}