using System;

namespace MyJson{
	public interface ISaveToJson{
		String ToJsonString();
	}
	
	public class Json{
		public String JsonString{get;set;} = "";
		private int itemCount = 0;
		private int objectCount = 0;
		private int depth = 0;
		
		public String OpenObject(String name=null){
			JsonString += ((objectCount > 0)?",\n":"")+GetTap(depth)+(name!=null?"\""+name+"\":":"")+"{"+"\n";
			depth++;
			return JsonString;
		}
		public String OpenArray(String name=null){
			JsonString += GetTap(depth)+(name!=null?"\""+name+"\":":"")+"["+"\n";
			depth++;
			return JsonString;
		}
		public String CloseObject(){
			depth--;
			JsonString += "\n"+GetTap(depth)+"}";
			objectCount++;
			itemCount = 0;
			return JsonString;
		}
		public String CloseArray(){
			depth--;
			JsonString += "\n"+GetTap(depth)+"]";
			objectCount = 0;
			itemCount = 0;
			return JsonString;
		}
		public String AddItem(String name,String val){
			JsonString += ((itemCount > 0)?",\n":"")+GetTap(depth)+"\""+name+"\""+":"+"\""+val+"\"";
			itemCount++;
			return JsonString;
		}
		public String GetTap(int num){
			String taps = "";
			for(int i = 0;i<num;i++){
				taps+="    ";
			}
			return taps;
		}
		
		public int JsonIndexOf(String str){
			return JsonString.IndexOf(str);
		}
		
		public int JsonIndexOf(String str, int startIndex){
			return JsonString.IndexOf(str,startIndex);
		}
		
		public String GetItem(String key){
			int index = 0;
			String temp = "";
			index = JsonIndexOf(":",JsonIndexOf(key));
			index = JsonIndexOf("\"",index);
			for(int i=index+1;i<JsonString.Length;i++){
				if(JsonString[i] == '\"'){
					break;
				}
				temp+=JsonString[i];
			}
			temp = temp.Replace("\"","");
			return temp;
		}
		
	}
}