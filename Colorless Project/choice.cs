using System;
using System.Collections.Generic;
using Characters;
using System.Threading;
using static Convenience;

public enum ChoiceType{
			CHOICE,		//선택지
			BATTLE,		//전투
			TALK,		//대화
			GET,		//획득
			SET,		//장착
			QUICK,		//즉시 실행
			QUICKNEXT	//즉시 다음 선택지
}

public class Choice : ICloneable //선택지 부여 하는 클래스
{
		public int CTNum;      //ChoiceTextNumber
		public int OSTNum;		//OnlyShowTextNumber
		public int STNum;
		public int BGTNum;
		public String Name {get;set;}
		public delegate void Quick();
		public Quick QuickDelegate{get;set;} 
		
		
		
		public Choice(){
			CTNum = 0;
			OSTNum = 0;
			STNum = 0;
			BGTNum = 0;
			indicateChoice = new Dictionary<int,String>();
			backgroundText = new List<TextAndPosition>();
			choiceText = new List<TextAndPosition>();
			onlyShowText = new List<TextAndPosition>();
			streamText = new List<TextAndPosition>();
			choiceType = ChoiceType.CHOICE;
		}
		
		List<TextAndPosition> choiceText;
		public List<TextAndPosition> ChoiceText{
			get{
				return choiceText;
			}
			
			set{
				choiceText = value;
				SetCTNum();
			}
		}
		
		List<TextAndPosition> onlyShowText;
		public List<TextAndPosition> OnlyShowText{
			get{
				return onlyShowText;
			}
			set{
				onlyShowText = value;
				SetOSTNum();
			}
		}
		
		List<TextAndPosition> streamText;
		public List<TextAndPosition> StreamText{
			get{
				return streamText;
			}
			set{
				streamText = value;
				Streaming();
				SetSTNum();
			}
		}
		
		public void Streaming(){
				foreach(TextAndPosition text in streamText){
						text.isStream = true;
				}
			}
		
		List<TextAndPosition> backgroundText;
		public List<TextAndPosition> BackgroundText{
			get{
				return backgroundText;
			}
			set{
				backgroundText = value;
				SetBGTNum();
			}
		}
		
		List<TextAndPosition> returnText;
		public List<TextAndPosition> ReturnText{
			get{
				return returnText;
			}
			set{
				returnText = value;
			}
		}
		
		Dictionary<int,String> indicateChoice;
		public Dictionary<int,String> IndicateChoice{
			get{
				return indicateChoice;
			}
			set{
				indicateChoice = value;
			}
		}
		
		List<Monster> monsterList;
		public List<Monster> MonsterList{
			get{
				return monsterList;
			}
			set{
				monsterList = value;
			}
		}
		
		List<NPC> npcList;
		public List<NPC> NPCList{
			get{
				return npcList;
			}
			set{
				npcList = value;
			}
		}
		
		ChoiceType choiceType;
		public ChoiceType ChoiceType{
			get;
			set;
		}
		
		public String ChoiceNext(int num){ //다음 선택지를 indicateChoice에서 찾아 반환
			return indicateChoice[num];
		}
		
		
		void SetCTNum(){
			CTNum = ChoiceText.Count;
		}
		void SetOSTNum(){
			OSTNum = OnlyShowText.Count;
		}
		void SetSTNum(){
			STNum = StreamText.Count;
		}
		void SetBGTNum(){
			STNum = BackgroundText.Count;
		}
		
		
		public void Print(){
			Console.WriteLine(choiceText[0].text+CTNum);
		}
		
		public void QuickRun(){
			QuickDelegate();
		}
		
		public String QuickNext(){  //choiceType이 QUICKNEXT일때 빠르게 다음 선택지로 넘어갈때 DTG또는 Main에서 호출하는 함수
			//Console.ReadKey();
			return ChoiceNext(0);
		}
		
		public List<TextAndPosition> CopyList(List<TextAndPosition> list){
			List<TextAndPosition> copy = new List<TextAndPosition>();
			for(int i = 0;i<list.Count;i++){
				copy.Add((TextAndPosition)list[i].Clone());
			}
			return copy;
		}
		
		public void LeaveChoice(){
			if(ReturnText != null){
				OnlyShowText = ReturnText;
			}
		}
		
		protected Choice(Choice that){
			this.QuickDelegate = that.QuickDelegate;
			
			this.ChoiceText = new List<TextAndPosition>();
			this.OnlyShowText = new List<TextAndPosition>();
			this.StreamText = new List<TextAndPosition>();
			this.BackgroundText = new List<TextAndPosition>();
			this.MonsterList = new List<Monster>();
			this.NPCList = new List<NPC>();
			
			
			
			if(that.ChoiceText != null)
				this.ChoiceText = that.ChoiceText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(that.OnlyShowText != null)
				this.OnlyShowText = that.OnlyShowText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(that.StreamText != null)
				this.StreamText = that.StreamText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(that.BackgroundText != null)
				this.BackgroundText = that.BackgroundText.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
			
			this.IndicateChoice = that.IndicateChoice;
			this.CTNum = that.CTNum;
			this.OSTNum = that.OSTNum;
			this.STNum = that.STNum;
			this.BGTNum = that.BGTNum;
			this.Name = that.Name;
			this.ChoiceType = that.ChoiceType;
			
			testLog("in Choice Clone");
			if(that.MonsterList != null)
				this.MonsterList = that.MonsterList.ConvertAll(new Converter<Monster, Monster>(o => (Monster)o.Clone()));
			testLog("in Choice Clone2");
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
		
		public Choice this[String index]{
			get{
				return choiceDictionary[index];
			}
			set{
				choiceDictionary[index] = value;
			}
		}
		
		public Choice ChooseChoice(string choiceName){ //선택지를 골라낼 메소드
			return choiceDictionary[choiceName];
		}
		
		public void AddChoice(Choice choice){ //선택지를 추가할 메소드
			if(choiceDictionary.ContainsKey(choice.Name)){
				choiceDictionary[choice.Name] = choice;
			}else{
				choiceDictionary.Add(choice.Name,choice);
			}
		}
		
		public void AddScenario(Scenario scenario){ //선택지들의 뭉치인 시나리오를 통체로 추가할 메소드
			for(int i = 0;i<scenario.Count;i++){
				choiceDictionary.Add(scenario[i].Name,(Choice)scenario[i].Clone());
			}
		}
		
		public Choice SetChoice(String cName){
			return choiceDictionary[cName];
		}
		
		public Choice SetChoiceClone(String cName){
			return (Choice)choiceDictionary[cName].Clone();
		}
		
}
