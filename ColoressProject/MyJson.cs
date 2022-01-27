using System;

namespace MyJson{
	public interface ISaveToJson{
		String ToJsonString();
		void JsonToObject(String jsonString);
	}
	
	public class Json{
		public String JsonString{get;set;} = "";
		public bool Error{get;set;} = false;
		private String errorMessage;
		private int itemCount = 0;
		private int objectCount = 0;
		private int depth = 0;
		
		public String ErrorMessage{
			get{
				Error = false;
				return errorMessage; 
			}
		}
		
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
		public String AddItem(String name,Object val){
			String temp;
			if(val == null) temp = "null";
			else temp = val.ToString();
			JsonString += ((itemCount > 0)?",\n":"")+GetTap(depth)+"\""+name+"\""+":"+"\""+temp+"\"";
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
			index = JsonIndexOf("{");//{ 로 시작하는 Json 객체부터 시작
			
			while(index != -1){ //key에 해당하는 부분에 "을 찾아 key값 전체를 가져와 매개변수로 받은 key값과 비교후 해당하는 key가 있으면 그 줄에 :의 index를 넣음
				index = JsonIndexOf("\"",index);
				for(int i=index+1;i<JsonString.Length;i++){
					if(JsonString[i] == '\"'){
						break;
					}
					temp+=JsonString[i];
				}
				if(temp == key){
					index = JsonIndexOf(":",index);
					break;
				}else{
					temp = "";
					index = JsonIndexOf("\n",index);
				}
			}
			index = JsonIndexOf("\"",index);//위에서 받은 :의 위치부터 시작해 value를 받아옴
	
			temp = "";
			for(int i=index+1;i<JsonString.Length;i++){
				if(JsonString[i] == '\"'){
					break;
				}
				temp+=JsonString[i];
			}
			temp = temp.Replace("\"","");//밸류 양옆에 "제거
			return temp;
		}
		
		public void ErrorAt(String errorMessage){
			Error = true;
			this.errorMessage = errorMessage;
		}
		
	}
}