using System;
using System.Collections.Generic;
using static Convenience;

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
		//private int arrayDepth = 0;
		private int arrayCount = 0;
		private int depth = 0;
		//private bool isItem = false;
		
		const String ARRAY_TAG = ":[";
		const String OBJECT_TAG = ":{";
		const String DICTIONARY_TAG = ":[";
		const String ITEM_TAG = ":\"";
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
			if(JsonString.Length == 0){
				JsonString += GetTap(depth)+(name!=null?"\""+name+"\":":"")+"["+"\n";
			}else{
				JsonString += ((itemCount > 0 || (JsonString[JsonString.Length-1]==']' || JsonString[JsonString.Length-1]=='}'))?",\n":"")+GetTap(depth)+(name!=null?"\""+name+"\":":"")+"["+"\n";
			}
			depth++;
			//arrayDepth++;
			return JsonString;
		}
		public String CloseObject(){
			depth--;
			JsonString += "\n"+GetTap(depth)+"}";
			objectCount++;
			itemCount = 0;
			return JsonString;
		}
		public String CloseArray(bool isItem = false){
			depth--;
			JsonString += "\n"+GetTap(depth)+"]";
			objectCount = 0;
			if(!isItem){
				itemCount = 0;
				objectCount = 0;
			}else{
				itemCount++;
				//objectCount++;
			}
			//arrayDepth--;
			//arrayCount++;
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
		public String AddJsonAbleObject(ISaveToJson obj){
			String objJsonString = "";
			String[] temp;
			if(obj == null)
			{
				return JsonString;//temp = (obj.ToString()+":null").Split('\n');
			}
			else
			{
				objJsonString = obj.ToJsonString();
				temp = objJsonString.Split('\n');
			}
			
			JsonString += ((objectCount > 0 || (JsonString[JsonString.Length-1]==']' || JsonString[JsonString.Length-1]=='}'))?",\n":"");
			for(int i = 0;i<temp.Length;i++){
				if(i == temp.Length-1){
					JsonString += GetTap(depth)+temp[i];	
				}
				else{
					JsonString += GetTap(depth)+temp[i]+"\n";
				}
			}
			objectCount++;
			return JsonString;
		}
		
		public String AddList<T>(String name,List<T> list,bool isItem = false){
			if(list == null) return "Error";
			OpenArray(name);
			for(int i = 0;i<list.Count;i++)
			{
				AddItem(i.ToString(),list[i]);
			}
			CloseArray(isItem);
			return JsonString;
		}
		
		//isItem이 true이면 해당 Array를 닫을때 아이템으로 인식해서 다음 Array를 쓰기할때 앞에 \n과 ,가 들어감
		public String AddJsonAbleList<T>(String name,List<T> list,bool isItem = false) where T : ISaveToJson
		{
			if(list == null) return "Error";
			OpenArray(name);
			foreach(T tap in list){
				AddJsonAbleObject(tap);
			}
			CloseArray(isItem);
			return JsonString;
		}
		
		public String AddDictionary<T,G>(String name,Dictionary<T,G> dic,bool isItem = false)
		{
			OpenArray(name);
			itemCount = 0;
			foreach(KeyValuePair<T,G> keyAndValue in dic){
				AddItem(keyAndValue.Key.ToString(),keyAndValue.Value);
			}
			CloseArray(isItem);
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
			if(startIndex < 0) return -1;
			return JsonString.IndexOf(str,startIndex);
		}
		
		public int JsonLastIndexOf(String str, int startIndex){
			if(startIndex < 0) return -1;
			return JsonString.LastIndexOf(str,startIndex);
		}
		
		public int JsonListIndexOf(String tagedKey, int startIndex){
			if(startIndex < 0) return -1;
			String temp = "";
			int index = startIndex;
			index = JsonIndexOf(tagedKey,index);
			if(index >= 0){
				index = JsonIndexOf("[",index);
			}
			return index;
		}
		
		public int DepthCounting(int index){
			int count = 0;
			for(int i = 0;i<JsonString.Length;i++){
				if(index-i < 0){ 
					return count;
				}
				if(JsonString[index-i]==' '){ 
					count++;
				}	  
				else if(JsonString[index-i]=='\n'){ 
					return count;
				}
			}
			return -1;
		}
		
		public int DepthCountingOtherString(String str,int index){
			int count = 0;
			if(str == null) return count;
			
			if(index == -1) return -1;//throw new Exception("index 값이 -1임");
			//Convenience.testLog("here:"+index);
			for(int i = 0;i<str.Length;i++){
				if(index-i < 0){ 
					return count;
				}
				if(str[index-i]==' '){ 
					count++;
				}	  
				else if(str[index-i]!=' '){ 
					return count;
				}
			}
			return -1;
		}
		
		public String GetItem(String key){
			int index = 0;
			int endIndex = 0;
			String temp = "";
			int depthCount = 0;
			index = JsonIndexOf("{");//{ 로 시작하는 Json 객체부터 시작
			//해당 객체의 끝의 인덱스를 저장, 찾은 아이템이 그 endIndex보다 큰 index위치에 있으면 검색 종료
			
			endIndex = IndexOfCompleteBracket(JsonString,index,'{','}');
			
			
			
			index = JsonIndexOf(KeyTaging(key,ITEM_TAG),index);
			if(endIndex == -1){
				throw new Exception("올바른 형식의 객체가 아닙니다.key->"+KeyTaging(key,ITEM_TAG)+"<-key\n"+JsonString);
			}
			if(index == -1){
				throw new Exception("찾는 아이템이 해당 객체에 없습니다.key->"+KeyTaging(key,ITEM_TAG)+"<-key\n"+JsonString);
			}
			
			
			if(index >= 0){
				index = JsonIndexOf(":",index);
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
			//깊이를 표현하기 위해 추가된 tap(공백4칸)을 제거
			temp = temp.Replace("    ","");
			
			return temp;
		}
		
		
		public T GetJsonAbleObject<T>(String key) where T : ISaveToJson,new(){
			int index = 0;
			String temp = "";
			T obj = new T();
			int endIndex = 0;
			int depthCount = 0;
			
			
			index = JsonIndexOf(KeyTaging(key,OBJECT_TAG),index);

			if(index >= 0){
				index = JsonIndexOf("{",index);
			}else{
				throw new Exception("해당 객체가 없습니다.");
			}
			
			//해당 객체의 끝의 인덱스를 저장, 찾은 아이템이 그 endIndex보다 큰 index위치에 있으면 검색 종료
			endIndex = IndexOfCompleteBracket(JsonString,index,'{','}');
			
			//Key값의 위치에 있는 객체 text전체를 temp에 넣음
			temp = "";
			for(int i=index;i<endIndex+1;i++){
				temp+=JsonString[i];
			}
			obj.JsonToObject(temp);
			
			return obj;
		}
		
		/*public T GetJsonAbleObject<T>(String type,String key) where T : ISaveToJson,new(){
			int index = 0;
			int indexTemp = 0;
			String temp = "";
			T obj = new T();
			//해당하는 type의 index찾기
			index = JsonIndexOf(KeyTaging(type,""),index);

			if(index >= 0){
				index = JsonIndexOf("{",index);
				temp = "";
			}else{
				return new Exception("해당하는 타입이 없습니다.");
			}
			
			//해당하는 type에서 key의 index찾기
			indexTemp = index;
			while(indexTemp != -1){ //key에 해당하는 부분에 "을 찾아 key값 전체를 가져와 매개변수로 받은 key값과 비교후 해당하는 key가 있으면 그 줄에 :의 index를 넣음
				
				for(int i=indexTemp+1;i<JsonString.Length;i++){
					if(JsonString[i] == '\"'){
						break;
					}
					temp+=JsonString[i];
				}
				if(temp == key){
					index = indexTemp;
					break;
				}else{
					temp = "";
					indexTemp = JsonIndexOf(":",indexTemp);
				}
			}
			
			//Key값의 위치에 있는 객체 text전체를 temp에 넣음
			temp = "";
			for(int i=index+1;i<JsonString.Length;i++){
				if(JsonString[i] == '}'){
					break;
				}
				temp+=JsonString[i];
			}
			obj.JsonToObject(temp);
			return obj;
		}*/
		
		public List<String> GetStringList(String key){
			int index = 0;
			String temp = "";
			List<String> list = new List<String>();
			int endIndex = 0;
			
			index = JsonListIndexOf(KeyTaging(key,ARRAY_TAG),index);
			if(index == -1) throw new Exception("해당하는 Array가 없습니다.key->"+KeyTaging(key,ARRAY_TAG)+"<-key\n");
			
			index = JsonIndexOf("[",index);//위에서 받은 [의 위치부터 시작해 value를 받아옴
			endIndex = IndexOfCompleteBracket(JsonString,index,'[',']');
			
			
			//Key값의 위치에 있는 배열 text 전체를 temp에 넣음
			temp = "";
			for(int i=index;i<endIndex+1;i++){
				temp+=JsonString[i];
			}
			
			index = 0;
			int count = 0;
			while(index != -1)
			{
				index = temp.IndexOf(":",index+1);
				list.Add(GetValueBackColon(temp,index));
			}
			return list;
		}
		
		public List<T> GetJsonAbleList<T>(String key) where T : ISaveToJson,new(){
			int index = 0;
			String temp = "";
			List<T> list = new List<T>();
			int endIndex = 0;
			
			index = JsonListIndexOf(KeyTaging(key,ARRAY_TAG),index);
			if(index == -1) throw new Exception("해당하는 Array가 없습니다.key->"+KeyTaging(key,ARRAY_TAG)+"<-key\n");
			
			index = JsonIndexOf("[",index);//위에서 받은 [의 위치부터 시작해 value를 받아옴
			endIndex = IndexOfCompleteBracket(JsonString,index,'[',']');
			
			
			//Key값의 위치에 있는 배열 text 전체를 temp에 넣음
			temp = "";
			for(int i=index;i<endIndex+1;i++){
				temp+=JsonString[i];
			}
			
			//배열에서 하나의 Object단위로 분리하여 Object화 시켜 List에 추가
			index = 0;
			String objectString = "";
			index = temp.IndexOf("{");
			while(index != -1)
			{
				endIndex = IndexOfCompleteBracket(temp,index,'{','}');
				//객체의 시작점({)을 찾지못해 index값이 -1이 나올경우 list를 반환
				if(index == -1)
				{
					return list;
				}
				for(int i=index;i<endIndex+1;i++)
				{
					objectString+=temp[i];
				}
				T obj = new T();
				obj.JsonToObject(objectString);
				list.Add(obj);
				objectString = "";
				index = temp.IndexOf("{",endIndex);
			}
			
			//testLog("key:"+key+"\ndepthCount1:"+depthCount+"\ndepthCount2:"+DepthCountingOtherString(temp,endIndex)+"\nindex1:"+index+"\nindex1:"+endIndex);
			
			
			
			return list;
		}
		
		public bool IsCompleteObjectString(String jsonString){
			if(jsonString.Contains("{")&&jsonString.Contains("}")){
				return true;
			}
			else{
				return false;
			}
		}
		
		
		public Dictionary<int,Object> GetDictionary(String key){
			int index = 0;
			String temp = "";
			Dictionary<int,Object> dic = new Dictionary<int,Object>();
			
			index = JsonIndexOf(KeyTaging(key,DICTIONARY_TAG),index);

			if(index >= 0){
				index = JsonIndexOf("[",index);
			}else{
				throw new Exception("해당하는 Dictionary가 없습니다.key->"+KeyTaging(key,DICTIONARY_TAG)+"<-key\n");
			}
			
			//index = JsonIndexOf("\"",index);//위에서 받은 [의 위치부터 시작해 value를 받아옴
			
			//Key값의 위치에 있는 배열 text 전체를 temp에 넣음
			temp = "";
			for(int i=index+1;i<JsonString.Length;i++){
				if(JsonString[i] == ']'){
					break;
				}
				temp+=JsonString[i];
			}
			
			//위에서 분리해낸 temp에서 key값과 value값을 찾아 넣어줌
			index = 0;
			bool swich = true;
			String K = "";
			String V = "";
			while(index >= 0){
				index = temp.IndexOf(":",index);
				if(index == -1)
				{
					break;
				}
				K = GetKeyFrontColon(temp,index);
				V = GetValueBackColon(temp,index);
				K = K.Replace("\"","");//밸류 양옆에 "제거
				V = V.Replace("\"","");
				try{
					dic.Add(int.Parse(K),V.ToString());
				}catch(System.FormatException e){
					testLog(e.ToString()+"\nK:"+K+"\nV:"+V+"\n"+temp);
					return dic;
				}
				catch(System.ArgumentException e){
					testLog(e.ToString()+"\nK:"+K+"\nV:"+V+"\n"+temp);
					return dic;
				}
				index++;
			}
			
			return dic;
		}
		
		public int IndexOfCompleteBracket(String str,int startIndex,Char firstBracket,Char lastBracket){
			Stack<Char> bStack = new Stack<Char>();
			if(startIndex == -1)
			{
				return -1;
			}
			for(int i = startIndex;i<str.Length;i++){
				if(str[i] == firstBracket)
				{
					bStack.Push(str[i]);
				}
				else if(str[i] == lastBracket){
					bStack.Pop();
					if(bStack.Count == 0){
						return i;
					}
				}
			}
			return -1;
		}
		
		public String GetLineFrontBracket(String str, int index){
			String key = "";
			int endIndex = 0;
			int startIndex = 0;
			endIndex = str.LastIndexOf("\"",index);
			startIndex = str.LastIndexOf("\n",endIndex-1);
			return str.Substring(startIndex,endIndex-startIndex+2);
		}
		
		public String GetKeyFrontColon(String str,int colonIndex){
			String key = "";
			if(colonIndex == -1)
			{
				return "";
			}
			int endIndex = str.LastIndexOf("\"",colonIndex);
			int startIndex = str.LastIndexOf("\"",endIndex-1);
			return str.Substring(startIndex,endIndex-startIndex);
		}
		
		public String GetValueBackColon(String str,int colonIndex){
			String key = "";
			if(colonIndex == -1)
			{
				return "";
			}
			int startIndex = str.IndexOf("\"",colonIndex);
			int endIndex = str.IndexOf("\"",startIndex+1);
			return str.Substring(startIndex,endIndex-startIndex);
		}
		
		public String KeyTaging(String key,String tag){
			return "\""+key+"\""+tag;
		}
		
		public void ErrorAt(String errorMessage){
			Error = true;
			this.errorMessage = errorMessage;
		}
		
	}
}