using System;
using System.Collections.Generic;

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
			JsonString += ((itemCount > 0)?",\n":"")+GetTap(depth)+(name!=null?"\""+name+"\":":"")+"["+"\n";
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
			if(!isItem)
				itemCount = 0;
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
			String objJsonString = obj.ToJsonString();
			String[] temp = objJsonString.Split('\n');
			JsonString += ((objectCount > 0)?",\n":"");
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
			
			if(index == -1) throw new Exception("index 값이 -1임");
			Convenience.testLog("here:"+str.Length);
			for(int i = 0;i<str.Length;i++){
				if(index-i < 0){ 
					return count;
				}
				if(str[index-i]==' '){ 
					count++;
				}	  
				else if(str[index-i]=='\n'){ 
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
			depthCount = DepthCounting(index);
			endIndex = index;
			//해당 객체의 끝의 인덱스를 저장, 찾은 아이템이 그 endIndex보다 큰 index위치에 있으면 검색 종료
			do{
				endIndex++;
				endIndex = JsonIndexOf("}",endIndex);
				if(endIndex == -1) throw new Exception("올바른 객체 형태가 아님 key->"+key+"<-key\n");
			}while(depthCount != DepthCounting(endIndex));
			
			
			
			index = JsonIndexOf(KeyTaging(key,ITEM_TAG),index);
			if(index > endIndex || index == -1){
				Convenience.testLog(endIndex);
				throw new Exception("찾는 아이템이 해당 객체에 없습니다.key->"+KeyTaging(key,ITEM_TAG)+"<-key\n");
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
			
			depthCount = DepthCounting(index);
			endIndex = index;
			//해당 객체의 끝의 인덱스를 저장, 찾은 아이템이 그 endIndex보다 큰 index위치에 있으면 검색 종료
			do{
				endIndex++;
				endIndex = JsonIndexOf("}",endIndex);
				if(endIndex == -1) throw new Exception("올바른 객체 형태가 아님 key->"+key+"<-key\n");
			}while(depthCount != DepthCounting(endIndex));
			
			//Key값의 위치에 있는 객체 text전체를 temp에 넣음
			temp = "";
			for(int i=index;i<endIndex;i++){
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
		
		public List<T> GetJsonAbleList<T>(String key) where T : ISaveToJson,new(){
			int index = 0;
			String temp = "";
			List<T> list = new List<T>();
			int depthCount = 0;
			int endIndex = 0;
			
			index = JsonListIndexOf(KeyTaging(key,ARRAY_TAG),index);
			if(index == -1) throw new Exception("해당하는 Array가 없습니다.key->"+KeyTaging(key,ARRAY_TAG)+"<-key\n");
			
			index = JsonIndexOf("[",index);//위에서 받은 [의 위치부터 시작해 value를 받아옴
			
			depthCount = DepthCounting(index);
			endIndex = index;
			//해당 객체의 끝의 인덱스를 저장, 찾은 아이템이 그 endIndex보다 큰 index위치에 있으면 검색 종료
			do{
				endIndex++;
				endIndex = JsonIndexOf("]",endIndex);
				if(endIndex == -1) throw new Exception("올바른 객체 형태가 아님 key->"+key+"<-key\n");
			}while(depthCount != DepthCounting(endIndex));
			
			//Key값의 위치에 있는 배열 text 전체를 temp에 넣음
			temp = "";
			for(int i=index+1;i<endIndex;i++){
				temp+=JsonString[i];
			}
			
			Convenience.testLog(temp);
			
			
			//배열에서 하나의 Object단위로 분리하여 Object화 시켜 List에 추가
			index = 0;
			Convenience.testLog(depthCount);
			///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 이 아래 DepthCountingOtherString에서부터 에러발생
			String objectString = "";
			depthCount = DepthCountingOtherString(temp,index);
			endIndex = index;
			//해당 객체의 끝의 인덱스를 저장, 찾은 아이템이 그 endIndex보다 큰 index위치에 있으면 검색 종료
			do{
				endIndex++;
				endIndex = JsonIndexOf("}",endIndex);
				if(endIndex == -1) throw new Exception("올바른 객체 형태가 아님 key->"+key+"<-key\n");
			}while(depthCount != DepthCountingOtherString(temp,endIndex));
			
			while(index < endIndex){
				for(int i=index;i<temp.Length;i++){
					objectString+=temp[i];
				}
				if(!IsCompleteObjectString(objectString)){
					//Convenience.testLog("obobjectString:"+objectString);
					if(list.Count == 0){
						return list;
						//throw new Exception("해당 배열에 요소가 없거나 해당 배열이 존재하지 않습니다");
					}else{
						break;
					}
				}
				T listItem = new T();
				listItem.JsonToObject(objectString);
				list.Add(listItem);
				objectString = "";
			}
			
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
		
		/*public Dictionary<T,G> GetDictionary<T,G>(String key){
			int index = 0;
			String temp = "";
			Dictionary<T,G> dic = new Dictionary<T,G>();
			
			while(index != -1){ //key에 해당하는 부분에 "을 찾아 key값 전체를 가져와 매개변수로 받은 key값과 비교후 해당하는 key가 있으면 그 줄에 :의 index를 넣음
				index = JsonIndexOf("\"",index);
				for(int i=index+1;i<JsonString.Length;i++){
					if(JsonString[i] == '\"'){
						break;
					}
					temp+=JsonString[i];
				}
				if(temp == key){
					index = JsonIndexOf("[",index);
					break;
				}else{
					temp = "";
					index = JsonIndexOf("]",index);
				}
			}
			index = JsonIndexOf("\"",index);//위에서 받은 [의 위치부터 시작해 value를 받아옴
			
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
				index = temp.IndexOf("\"",index);
				for(int i=index+1;i<temp.Length;i++){
					if(temp[i] == '\"'){
						break;
					}
					if(swich){
						K+=temp[i];
					}else{
						V+=temp[i];
					}
				}
				K = K.Replace("\"","");//밸류 양옆에 "제거
				V = V.Replace("\"","");
				if(swich){
					dic.Add(T.Parse(K),default(G));
					swich = false;
				}else{
					dic.Add(T.Parse(K),G.Parse(V));
					K = "";
					V = "";
					swich = true;
				}
			}
			
			return dic;
		}*/
		
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
				K = GetKeyFrontColon(index);
				V = GetValueBackColon(index);
				K = K.Replace("\"","");//밸류 양옆에 "제거
				V = V.Replace("\"","");
				try{
					dic.Add(int.Parse(K),V.ToString());
				}catch(System.FormatException e){
					return dic;
				}
			}
			
			return dic;
		}
		
		public String GetKeyFrontColon(int colonIndex){
			String key = "";
			int endIndex = JsonString.LastIndexOf("\"",colonIndex);
			int startIndex = JsonString.LastIndexOf("\"",endIndex-1);
			return JsonString.Substring(startIndex,endIndex);
		}
		
		public String GetValueBackColon(int colonIndex){
			String key = "";
			int startIndex = JsonString.IndexOf("\"",colonIndex);
			int endIndex = JsonString.IndexOf("\"",startIndex+1);
			return JsonString.Substring(startIndex,endIndex);
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