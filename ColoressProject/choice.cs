using System;
using System.Collections.Generic;
using Characters;
using System.Threading;
using static Convenience;
using System.Linq;
using System.IO;

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

public class Choice : ICloneable //선택지 부여 하는 클래스
{
	public int selectTextNum;      
	public int onlyShowTextNum;		
	public int streamTextNum;
	public int backgroundTextNum;
	public String Name {get;set;}
	public delegate void Quick();
	public Quick QuickDelegate{get;set;} 
	
	public bool IsSavePoint{get;set;} = false;

	private List<TextAndPosition> selectText;
	private List<TextAndPosition> onlyShowText;
	private List<TextAndPosition> streamText;
	private List<TextAndPosition> backgroundText;
	private List<TextAndPosition> returnText;
	private Dictionary<int,Object> indicateChoice;
	private List<Monster> monsterList;
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
		monsterList = new List<Monster>();
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
	public List<Monster> MonsterList{
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
			OnlyShowText = ReturnText;
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
		this.IndicateChoice = new Dictionary<int,Object>(that.IndicateChoice);

		this.QuickDelegate = that.QuickDelegate;
		
		this.selectTextNum = that.selectTextNum;
		this.onlyShowTextNum = that.onlyShowTextNum;
		this.streamTextNum = that.streamTextNum;
		this.backgroundTextNum = that.backgroundTextNum;
		this.Name = that.Name;
		this.ChoiceType = that.ChoiceType;
		this.IsSavePoint = that.IsSavePoint;

		if(!isEmptyList(that.MonsterList)){
			//try{
			if(that.MonsterList[0] != null){
				this.MonsterList = that.MonsterList.ConvertAll(new Converter<Monster, Monster>(o => (Monster)o.Clone()));
			}
			//}catch(Exception e){}
		}
		if(that.NPCList != null)
			this.NPCList = that.NPCList.ConvertAll(new Converter<NPC, NPC>(o => (NPC)o.Clone()));
	}

	public Object Clone(){
		return new Choice(this);
	}
}
	
public class ChoiceControler{
	public Dictionary<String,Choice> choiceDictionary; //선택지들을 담을 딕셔너리 클래스

	public ChoiceControler(){
		choiceDictionary = new Dictionary<String,Choice>();
	}
	
	public ChoiceControler(Scenario scenario):this(){
		for(int i = 0;i<scenario.Count;i++){
			choiceDictionary.Add(scenario[i].Name,(Choice)scenario[i].Clone());
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
	
	public Choice ChangeChoiceText(String choiceName,ChoiceControler choiceControler = null,TextAndPosition onlyShowText = null,TextAndPosition selectText = null){
		Choice cho = this.GetChoice(choiceName);
		if(choiceControler == null){
			choiceControler = this;
		}
		if(onlyShowText != null){
			cho.OnlyShowText = new List<TextAndPosition>() {onlyShowText};
		}
		if(selectText != null){
			cho.SelectText = new List<TextAndPosition>() {selectText};
		}
		return cho;
	}
		
}