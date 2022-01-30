using System;
using System.Collections.Generic;
using Characters;
using System.Threading;
using static Convenience;
using System.Linq;
using System.IO;
using Game;
using MyJson;

public enum ChoiceType{
			NORMAL,		//선택지
			BATTLE,		//전투
			TALK,		//대화
			GET,		//획득
			SET,		//장착
			QUICK,		//즉시 실행
			QUICKNEXT	//즉시 다음 선택지
			//EXPLAN
}

public class Choice : ICloneable,ISaveToJson //선택지 부여 하는 클래스
{
	public int selectTextNum;      
	public int onlyShowTextNum;		
	public int streamTextNum;
	public int backgroundTextNum;
	public String Name {get;set;}//J
	public delegate void Quick();
	public Quick QuickDelegate{get;set;} 
	
	public bool IsSavePoint{get;set;} = false;
	public bool IsVisit{get;set;} = false;
	public bool IsShowStateWindow{get;set;} = true;

	private List<TextAndPosition> selectText;
	private List<TextAndPosition> onlyShowText;
	private List<TextAndPosition> streamText;
	private List<TextAndPosition> backgroundText;
	private List<TextAndPosition> returnText;
	private Dictionary<int,Object> indicateChoice;
	private List<Enemy> monsterList;
	private List<NPC> npcList;
	private ChoiceType choiceType;
	
	
		
	public Choice(){
		selectTextNum = 0;
		onlyShowTextNum = 0;
		streamTextNum = 0;
		backgroundTextNum = 0;
		selectText = new List<TextAndPosition>();
		onlyShowText = new List<TextAndPosition>();
		streamText = new List<TextAndPosition>();
		backgroundText = new List<TextAndPosition>();
		indicateChoice = new Dictionary<int,Object>();
		monsterList = new List<Enemy>();
		npcList = new List<NPC>();
		choiceType = ChoiceType.NORMAL;
	}


	public List<TextAndPosition> SelectText{
		get{
			return selectText;
		}

		set{
			selectText = value;
			selectTextNum = SelectText.Count;
		}
	}
	public List<TextAndPosition> OnlyShowText{
		get{
			return onlyShowText;
		}
		set{
			onlyShowText = value;
			onlyShowTextNum = OnlyShowText.Count;
		}
	}
	public List<TextAndPosition> StreamText{
		get{
			return streamText;
		}
		set{
			streamText = value;
			streamTextNum = streamText.Count;
			foreach(TextAndPosition text in streamText){
					text.isStream = true;
			}
			
		}
	}	
	public List<TextAndPosition> BackgroundText{
		get{
			return backgroundText;
		}
		set{
			backgroundText = value;
			backgroundTextNum = backgroundText.Count;
		}
	}
	public List<TextAndPosition> ReturnText{
		get{
			return returnText;
		}
		set{
			returnText = value;
		}
	}
	public Dictionary<int,Object> IndicateChoice{
		get{
			return indicateChoice;
		}
		set{
			indicateChoice = value;
		}
	}
	public List<Enemy> EnemyList{
		get{
			return monsterList;
		}
		set{
			monsterList = value;
		}
	}
	public List<NPC> NPCList{
		get{
			return npcList;
		}
		set{
			npcList = value;
		}
	}
	public ChoiceType ChoiceType{
		get;
		set;
	}
	
	public Object GetValueOn(int num){ //bool을 indicateChoice에서 찾아 반환
		return indicateChoice[num];
	}

	public void QuickRun(){
		QuickDelegate();
	}

	public Object QuickNext(){  //choiceType이 QUICKNEXT일때 빠르게 다음 선택지로 넘어갈때 DTG또는 Main에서 호출하는 함수
		//Console.ReadKey();
		return GetValueOn(0);
	}

	public void LeaveChoice(){
		if(ReturnText != null){
			if(StreamText.Count != 0){
				StreamText = ReturnText;
			}
			else{
				OnlyShowText = ReturnText;
			}
		}
	}
	
	public void TextArrangement(){//Layout종류에 따라 Text들을 정렬 ChoiceControler에 Scenario를 넣을때 호출했다.
		int firstX = 0;
		int lastY = 0;
		int count = 0;
		if(selectText != null){
			for(int i = 0;i<selectText.Count;i++){ //SelectText의 레이아웃 정렬
				TextAndPosition tap = selectText[i];
				if(tap.Layout == null) continue;
				if(tap.Layout == TextLayout.SELECT_DEFAULT){
					if(selectText.Count == 1){
						tap[0] = 22;
						tap[1] = 13;
					}
					else{
						firstX = GameManager.selectListFirststPositionX(selectText);
						lastY = GameManager.selectListLastPositionY(selectText);
						tap[0] = firstX;
						tap[1] = lastY + ++count;
					}
				}
			}
		}
		if(selectText != null){
			for(int i = 0;i<selectText.Count;i++){//SelectText중 CROSSROADS의 레이아웃 정렬
				TextAndPosition tap = selectText[i];
				List<TextAndPosition> crossroads = new List<TextAndPosition>();
				for(int j = 0;j<selectText.Count;j++){
					if(selectText[i][1] >= 19)
						crossroads.Add(selectText[i]);
				}
				if(crossroads.Count != 0){
					if(tap.Layout == TextLayout.CROSSROADS_DEFAULT){
						if(crossroads.Count == 1){
							tap[0] = 1;
							tap[1] = 19;
						}
						else if(crossroads.Count == 2){
							firstX = 60 - (tap.text.Length + Convenience.GetKoreanCount(tap.text));
							tap[0] = firstX;
							tap[1] = 19;
						}
						else if(crossroads.Count > 2){
							throw new Exception("이미 갈림길이 2개 있음.");
						}

					}
				}
			}
		}
		if(onlyShowText != null){
			for(int i = 0;i<onlyShowText.Count;i++){
				TextAndPosition tap = onlyShowText[i];
				if(tap.Layout == null) continue;
				if(tap.Layout == TextLayout.ONLY_SHOW_DEFAULT){
					if(onlyShowText.Count == 1){
						tap[0] = 15;
						tap[1] = 8;
					}
					else{
						firstX = GameManager.selectListFirststPositionX(onlyShowText);
						lastY = GameManager.selectListLastPositionY(onlyShowText);
						tap[0] = firstX;
						tap[1] = lastY;
					}
				}
			}
		}
		if(streamText != null){
			for(int i = 0;i<streamText.Count;i++){
				TextAndPosition tap = streamText[i];
				if(tap.Layout == null) continue;
				if(tap.Layout == TextLayout.ONLY_SHOW_DEFAULT){
					tap[0] = 15;
					tap[1] = 8;
				}
			}
		}
		if(returnText != null){
			for(int i = 0;i<returnText.Count;i++){
				TextAndPosition tap = returnText[i];
				if(tap.Layout == null) continue;
				if(tap.Layout == TextLayout.ONLY_SHOW_DEFAULT){
					if(returnText.Count == 1){
						tap[0] = 15;
						tap[1] = 8;
					}
					else{
						firstX = GameManager.selectListFirststPositionX(returnText);
						lastY = GameManager.selectListLastPositionY(returnText);
						tap[0] = firstX;
						tap[1] = lastY;
					}
				}
			}
		}
	}
	
	

	protected Choice(Choice that):this(){
		if(that.SelectText != null)
			this.SelectText = that.SelectText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
		if(that.OnlyShowText != null)
			this.OnlyShowText = that.OnlyShowText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
		if(that.StreamText != null)
			this.StreamText = that.StreamText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
		if(that.BackgroundText != null)
			this.BackgroundText = that.BackgroundText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
		if(that.ReturnText != null)
			this.ReturnText = that.ReturnText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
		
		this.IndicateChoice = new Dictionary<int,Object>(that.IndicateChoice);

		this.QuickDelegate = that.QuickDelegate;
		
		this.selectTextNum = that.selectTextNum;
		this.onlyShowTextNum = that.onlyShowTextNum;
		this.streamTextNum = that.streamTextNum;
		this.backgroundTextNum = that.backgroundTextNum;
		this.Name = that.Name;
		this.ChoiceType = that.ChoiceType;
		this.IsSavePoint = that.IsSavePoint;
		this.IsShowStateWindow = that.IsShowStateWindow;

		if(!IsEmptyList(that.EnemyList)){
			//try{
			if(that.EnemyList[0] != null){
				this.EnemyList = that.EnemyList.ConvertAll(new Converter<Enemy, Enemy>(o => (Enemy)o.Clone()));
			}
			//}catch(Exception e){}
		}
		if(that.NPCList != null)
			this.NPCList = that.NPCList.ConvertAll(new Converter<NPC, NPC>(o => (NPC)o.Clone()));
	}

	public Object Clone(){
		return new Choice(this);
	}
	
	// public int selectTextNum;      
	// public int onlyShowTextNum;		
	// public int streamTextNum;
	// public int backgroundTextNum;
	// public String Name {get;set;}//J
	// public delegate void Quick();
	// public Quick QuickDelegate{get;set;} 
	
	// public bool IsSavePoint{get;set;} = false;
	// public bool IsVisit{get;set;} = false;
	// public bool IsShowStateWindow{get;set;} = true;

	// private List<TextAndPosition> selectText;
	// private List<TextAndPosition> onlyShowText;
	// private List<TextAndPosition> streamText;
	// private List<TextAndPosition> backgroundText;
	// private List<TextAndPosition> returnText;
	// private Dictionary<int,Object> indicateChoice;
	// private List<Enemy> monsterList;
	// private List<NPC> npcList;
	// private ChoiceType choiceType;
	
	public String ToJsonString(){
		Json json = new Json();
		json.OpenObject(Name);
		json.AddItem("selectTextNum",selectTextNum);
		json.AddItem("onlyShowTextNum",onlyShowTextNum);
		json.AddItem("streamTextNum",streamTextNum);
		json.AddItem("backgroundTextNum",backgroundTextNum);
		json.AddItem("Name",Name);
		json.AddItem("selectTextNum",selectTextNum);
		
		
		return "";
	}
	
	public void JsonToObject(String jsonString){
		
	}
}
	
public class ChoiceControler{
	public Dictionary<String,Choice> choiceDictionary; //선택지들을 담을 딕셔너리 클래스

	public ChoiceControler(){
		choiceDictionary = new Dictionary<String,Choice>();
	}
	
	public ChoiceControler(Scenario scenario):this(){
		Choice temp;
		
		for(int i = 0;i<scenario.Count;i++){
			temp  = (Choice)scenario[i].Clone();
			temp.TextArrangement(); //Text들의 Layout에 따라 재정렬
			choiceDictionary.Add(scenario[i].Name,temp);
		}
	}

	public Choice this[String index]{
		get{
			return choiceDictionary[index];
		}
		set{
			choiceDictionary[index] = value;
		}
	}
	
	public void AddChoice(Choice choice){ //선택지를 추가할 메소드
		if(choiceDictionary.ContainsKey(choice.Name)){
			choiceDictionary[choice.Name] = choice;
		}else{
			choiceDictionary.Add(choice.Name,choice);
		}
	}

	public Choice GetChoice(String choiceName){
		return choiceDictionary[choiceName];
	}

	public Choice GetChoiceClone(String choiceName){
		return (Choice)choiceDictionary[choiceName].Clone();
	}
	
	public Choice ChangeChoiceText(String choiceName,ChoiceControler choiceControler = null,TextAndPosition onlyShowText = null,TextAndPosition selectText = null,List<TextAndPosition> streamText = null,TextAndPosition returnText = null){
		if(choiceControler == null){
			choiceControler = this;
		}
		//testLog(choiceName);
		Choice cho = choiceControler.GetChoice(choiceName);
		if(onlyShowText != null){
			cho.OnlyShowText = new List<TextAndPosition>() {onlyShowText};
		}
		if(selectText != null){
			cho.SelectText = new List<TextAndPosition>() {selectText};
		}
		if(streamText != null){
			cho.StreamText = streamText;
		}
		if(returnText != null){
			cho.ReturnText = new List<TextAndPosition>() {returnText};
		}
		cho.TextArrangement();
		return cho;
	}
	
	public Choice AddChoiceEnemy(String choiceName,Enemy monster,ChoiceControler choiceControler = null){
		if(choiceControler == null){
			choiceControler = this;
		}
		
		Choice cho = choiceControler.GetChoice(choiceName);
		
		cho.EnemyList.Add(monster);
		cho.TextArrangement();
		return cho;
	}
	
	public void RemoveChoiceSelectText(String choiceName,int selectIndex){
		Choice choice = choiceDictionary[choiceName];
		choice.SelectText.RemoveAt(selectIndex);
		choice.IndicateChoice.Remove(selectIndex);
		choice.IndicateChoice = GameManager.DictionaryRearrangement(choice.IndicateChoice);
		choice.TextArrangement();
	}
	
	public void RemoveChoiceSelectTextByText(String choiceName,string text){
		Choice choice = choiceDictionary[choiceName];
		int index = choice.SelectText.IndexOf(choice.SelectText.Find(t => t.text == text));
		choice.SelectText.RemoveAt(index);
		choice.IndicateChoice.Remove(index);
		choice.IndicateChoice = GameManager.DictionaryRearrangement(choice.IndicateChoice);
		choice.TextArrangement();
	}
	
	public void RemoveChoiceSelectTextByValue(String choiceName,String stringValue){
		Choice choice = choiceDictionary[choiceName];
		Object key = DictionaryFindKeyByValue(choice.IndicateChoice,stringValue);
		if(key == null){
			//testLog("key가 null입니다.:RemoveChoiceSelectTextByValue():"+stringValue);
			return;
		}
		choice.SelectText.RemoveAt((int)key);
		choice.IndicateChoice.Remove((int)key);
		choice.IndicateChoice = GameManager.DictionaryRearrangement(choice.IndicateChoice);
		choice.TextArrangement();
	}
	
	public void AddChoiceSelectText(String choiceName,TextAndPosition selectText,Object indicate){
		Choice choice = choiceDictionary[choiceName];
		choice.IndicateChoice = GameManager.DictionaryRearrangement(choice.IndicateChoice);
		//testLog(choice.IndicateChoice[choice.SelectText.Count]);
		try{
		choice.IndicateChoice.Add(choice.SelectText.Count,indicate);
		}catch(Exception e){
			testLog(e.ToString()+":"+choice.SelectText.Count+":"+indicate);
			return;
			//choice.IndicateChoice[choice.SelectText.Count]=indicate;
		}
		choice.SelectText.Add(selectText);
		choice.TextArrangement();
	}
	
	public void ChangeChoiceSelectText(String choiceName,int index,String selectString){
		Choice choice = choiceDictionary[choiceName];
		choice.SelectText[index].text = selectString;
		choice.TextArrangement();
	}
		
}