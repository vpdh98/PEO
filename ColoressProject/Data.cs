using System;
using System.Collections.Generic;
using Characters;
using static Define;
using static PlayData;
using System.IO;
using static DataManager;

public class Scenario{
		int clikX;
		int clikY;
		
		public List<Choice> choices = new List<Choice>();
		public Choice this[int index]{
			get{
				return choices[index];
			}
		}
		public int Count{get;set;}
		
		public Scenario(){ //지금은 choice생성할때 SelectText,OnlyShowText중 하나라도 없으면 실행 불가
			Backgrounds backgrounds = new Backgrounds();
			CharacterList characterList = new CharacterList();
			
			clikX = 15;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "c1",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",13+clikX,5+clikY,true){PriorityLayer = 3},
							  new TextAndPosition(200,"Exit",13+clikX,6+clikY,true){PriorityLayer = 4},
								new TextAndPosition(200,"옵션",13+clikX,7+clikY,true){PriorityLayer = 5}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("The Colorless",10+clikX,3+clikY,10,ConsoleColor.Green){PriorityLayer = 1},
							new TextAndPosition("개발:Peo",1,1,100){PriorityLayer = 2}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2"},{1,"test"},{2,"option"}},
				BackgroundText = backgrounds.GetBackground(0),
				IsSavePoint = true,
				IsShowStateWindow = false
			});
			
			clikX = 20;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "testStream",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",10+clikX,5+clikY,true){PriorityLayer = 4},
							  new TextAndPosition(200,"Exit",10+clikX,6+clikY,true){PriorityLayer = 5},
								new TextAndPosition(200,"옵션",10+clikX,7+clikY,true){PriorityLayer = 6}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(1000,"개발:Peo",1,1){PriorityLayer = 3}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("한 용사가 있었다.",5+clikX,3+clikY,30){PriorityLayer = 1},
							new TextAndPosition(1000,"하지만 그는 죽었다.",5+clikX,3+clikY,30,ConsoleColor.Red){PriorityLayer = 2}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2"},{1,"test"},{2,"option"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "option",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",10,5,true){PriorityLayer = 2},
							  new TextAndPosition(200,"Exit",10,6,true){PriorityLayer = 3}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("옵션은 없다.",5,3,30){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2"},{1,"exit"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "exit",
				ChoiceType = ChoiceType.QUICK,
				QuickDelegate = ()=>{Environment.Exit(0);}
			});
			
			choices.Add(new Choice(){
				Name = "test",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("나 갈 수 없 다",24,10,100,ConsoleColor.Red){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "c2",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("오른쪽",1,19,true),
							new TextAndPosition("가만히 있는다",7+clikX,8+clikY,true),
							new TextAndPosition("아앗.",7+clikX,9+clikY,true),
							 new TextAndPosition("문이 하나 보인다.",7+clikX,10+clikY,true),
							new TextAndPosition("왼쪽",56,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("용사는 길을 나섰다.",7+clikX,3+clikY,1),
							new TextAndPosition("울창한 숲.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("여전히 울창한 숲.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2-right"},{1,"testStream"},{2,"c2"},{3,"gyeongminsHouse"},{4,"village"}},
				EnemyList = new List<Enemy>()
							{characterList.GetEnemy("슬라임"),
							characterList.GetEnemy("뒤틀린 망자"),
							characterList.GetEnemy("헐크")},
				BackgroundText = backgrounds.GetBackground(6)
			});
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "village",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("누가 서있다",7+clikX,8+clikY,true),
							new TextAndPosition("돌아간다.",1,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("작은 마을이다.",7+clikX,3+clikY,1),
							new TextAndPosition("인기척이 거의 느껴지지 않는다.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("서늘한 공기에 햇살이 따스한 마을.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,Object>(){{0,"촌장"},{1,"c2"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "gyeongminsHouse",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("방으로 들어간다.",7+clikX,8+clikY,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("문을 지나가니 쾅! 소리와 함께 문이 닫혔다.",7+clikX,3+clikY,1){AlignH = true},
							 new TextAndPosition("여긴 어디지..?",7+clikX,4+clikY,1){AlignH = true},
							new TextAndPosition("주변에 온통 음식물이 흩어져 있다.",7+clikX,5+clikY,1){AlignH = true}},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("무엇인가 근처를 흟고 다닌 흔적이 있다.",7+clikX,3+clikY,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"gyeongminsHouse_Room"}},
				BackgroundText = backgrounds.GetBackground(7)
			});
			
			clikX = 23;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "gyeongminsHouse_Room",
				SelectText = new List<TextAndPosition>()
							{new TextAndPosition("방을 나간다.",26+clikX,12+clikY,true){PriorityLayer = 2}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("평범한 방.",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"방 밖에서 인기척이 느껴진다.",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1,AlignH = true}},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("방안에는 차가운 냉기만이 흐른다.",5+clikX,3+clikY,10){PriorityLayer = 1,AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"spawnGyeonmin"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "spawnGyeonmin",
				ChoiceType = ChoiceType.QUICK,
				QuickDelegate = ()=>{
					PlayData.accessAbleChoice.EnemyList.Add(PlayData.CList.GetEnemy("야생의 경민이",100));
				},
				IndicateChoice = new Dictionary<int,Object>(){{0,"gyeongminsHouse"}}
			});
			
			clikX = 23;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "c2-right",
				SelectText = new List<TextAndPosition>()
							{new TextAndPosition("다시시작",30+clikX,12+clikY,true){PriorityLayer = 2}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("함정이였다.",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"You Died",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"testStream"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			/*
			choices.Add(new Choice(){
				Name = "QuickNext-test",
				ChoiceType = ChoiceType.QUICKNEXT,
				SelectText = new List<TextAndPosition>()
							{new TextAndPosition("다시시작",12+clikX,7+clikY,true){PriorityLayer = 2}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("함정이였다.",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"You Died",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			*/
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "t0827-1",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("오른쪽",1,19,true),
							new TextAndPosition("왼쪽",56,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("Test 1",7+clikX,3+clikY,1),
							 new TextAndPosition("이것은 테스트 화면입니다.",7+clikX,4+clikY,50),
							new TextAndPosition("울창한 숲.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("여전히 울창한 숲.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,Object>(){{0,"t0827-1"},{1,"t0827-2"}},
				
				EnemyList = new List<Enemy>()
							{characterList.GetEnemy("슬라임"),
							characterList.GetEnemy("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "t0827-2",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("오른쪽",1,19,true),
							new TextAndPosition("왼쪽",56,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("Test 2",7+clikX,3+clikY,1),
							 new TextAndPosition("이것은 테스트 화면입니다.",7+clikX,4+clikY,50),
							new TextAndPosition("울창한 숲.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("여전히 울창한 숲.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,Object>(){{0,"t0827-1"},{1,"t0827-2"}},
				
				EnemyList = new List<Enemy>()
							{characterList.GetEnemy("슬라임"),
							characterList.GetEnemy("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "t0828-1",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("오른쪽",1,19,true),
							new TextAndPosition("왼쪽",56,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("Test 2",7+clikX,3+clikY,1),
							 new TextAndPosition("이것은 테스트 화면입니다.",7+clikX,4+clikY,50),
							new TextAndPosition("울창한 숲.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("여전히 울창한 숲.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,Object>(){{0,"t0828-1"},{1,"t0828-1"}},
				
				EnemyList = new List<Enemy>()
							{characterList.GetEnemy("슬라임"),
							characterList.GetEnemy("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "firstPhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("공격한다.",16,13,true),
							new TextAndPosition("도망친다.",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("globerEnemy.GetRandomSpawnMessage().text",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"},{1,"end"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "movePhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("공격",16,13,true),
							 new TextAndPosition("방어",28,13,true),
							new TextAndPosition("회피",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("globerEnemy.CurrentState()",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"attackPhase"},{1,"block"},{2,"dodge"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "attackPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("globerEnemy.Name"+"베기!",5,10,10,ConsoleColor.Red){AlignH = true,PriorityLayer=1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"reactionPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "reactionPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("(몬스터 피해 메세지)",5,10,10){AlignH = true,PriorityLayer=1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "andPhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("확인",16,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("globerEnemy.CurrentState()",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"backField"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "failBlock",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("막기 실패",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"monsterReactionPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "successBlock",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("막기 성공",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "failDodge",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("회피 실패",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"monsterReactionPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "successDodge",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("회피 성공",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
					Name = "monsterPreAttack",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition("globerEnemy.PreAttackCry()",15,3+5,1){AlignH = true}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"monsterAttack"}},
					BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
					Name = "monsterAttack",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition("globerEnemy.AttackCry()",15,3+5,1){AlignH = true}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"monsterReactionPhase"}},
					BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
					Name = "monsterReactionPhase",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition("globerPlayer.Reaction()",5,10,10){AlignH = true,PriorityLayer=1}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
					BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
					Name = "background",
					BackgroundText = backgrounds.GetBackground(5)
			});
			
			choices.Add(new Choice(){
				Name = "GreetPhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("대화.",16,13,true),
							new TextAndPosition("PreQuestMessage",40,13,true),
							 new TextAndPosition("돌아간다.",52,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("GreetMessage",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"ConversationPhase"},{1,"QuestIntroduction"},{2,"end"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
				Name = "QuestIntroduction",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("아뇨 괜찮습니다.",44,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("QuestIntroduction",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"GreetPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){ //Choice 전체가 다른 Choice로 대체 됨
				Name = "QuestExplan",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("아뇨 괜찮습니다.",44,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("QuestExplan",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"GreetPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			
			clikX = 23;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "ConversationPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("Conversation1",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"Conversation2",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1,AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"GreetPhase"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			
			
			choices.Add(new Choice(){
				Name = "QuestAccept",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("QuestAcceptMessage",5,10,10){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"GreetPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
				Name = "QuestReject",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("QuestRejectMessage",5,10,10){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"GreetPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
				Name = "CompleteQuestList",
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("CompleteQuestListMessage",15,3+5,1){AlignH = true}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
				Name = "QuestComplete",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("보상을 받는다.",16,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("QuestComplete",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"QuestReward"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			
			choices.Add(new Choice(){
				Name = "QuestReward",
				ChoiceType = ChoiceType.QUICK,
				QuickDelegate = ()=>{
					PlayData.accessAbleChoice.EnemyList.Add(PlayData.CList.GetEnemy("야생의 경민이",100));
				},
				IndicateChoice = new Dictionary<int,Object>(){{0,"gyeongminsHouse"}}
			});
			
			
			
			Count = choices.Count;
		}
}
	
	//┏ ┓ ━  ┛┗ ┃┣ ┳ ┫┻
public class Backgrounds{
	public List<List<TextAndPosition>> background;
	public ConsoleColor color;
	public int width = Define.SCREEN_WIDTH;
	public int height = Define.SCREEN_HEIGHT;
	
	
	
	public Backgrounds(){
		background = new List<List<TextAndPosition>>();
		
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("base"),0,0)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("battle"),0,0)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("inven"),12,0)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("confirm"),17,2)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("itemExplan"),16,2)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("getItem"),15,7)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("forest2"),-50,-8),new TextAndPosition(LoadBackground("space1"),0,0)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("background9"),-20,-5),new TextAndPosition(LoadBackground("space3"),7,6)}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{new TextAndPosition(LoadBackground("state"),0,0)}));

		Coloring();
	}

	public List<TextAndPosition> this[int index]{
		get{
			return background[index];
		}
		set{
			background[index] = value;
		}
	}

	public List<TextAndPosition> GetBackground(int index){
		return background[index];
	}

	public void Backgrounding(){
		foreach(List<TextAndPosition> list in background){
			for(int i = 0;i<list.Count;i++){
				list[i].PriorityLayer = 0;
			}
		}
	}

	public void Coloring(){
		if(color != null)
		foreach(List<TextAndPosition> list in background){
			for(int i = 0;i<list.Count;i++){
				list[i].color = color;
			}
		}
	}
}