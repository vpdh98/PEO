using System;
using System.Collections.Generic;
using Characters;
using static Define;

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
							  new TextAndPosition(200,"Exit",13+clikX,6+clikY,true,ConsoleColor.Yellow){PriorityLayer = 4},
								new TextAndPosition(200,"옵션",13+clikX,7+clikY,true){PriorityLayer = 5}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("The Colorless",10+clikX,3+clikY,10,ConsoleColor.Green){PriorityLayer = 1},
							new TextAndPosition("개발:Peo",1,1,100){PriorityLayer = 2}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2"},{1,"test"},{2,"option"}},
				BackgroundText = backgrounds.GetBackground(0)
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
							new TextAndPosition("왼쪽",56,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("용사는 길을 나섰다.",7+clikX,3+clikY,1),
							new TextAndPosition("울창한 숲.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("여전히 울창한 숲.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,Object>(){{0,"c2-right"},{1,"testStream"},{2,"c2"},{3,"c2"}},
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자"),
							characterList.GetMonster("헐크")},
				BackgroundText = backgrounds.GetBackground(0)
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
				
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자")},
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
				
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자")},
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
				
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "firstPhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("공격한다.",16,13,true),
							new TextAndPosition("도망간다.",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("globerMonster.GetRandomSpawnMessage().text",15,3+5,1){AlignH = true}},
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
							{new TextAndPosition("globerMonster.CurrentState()",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"attackPhase"},{1,"block"},{2,"dodge"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "attackPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("globerMonster.Name"+"베기!",5,10,10,ConsoleColor.Red){AlignH = true,PriorityLayer=1}},
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
							{new TextAndPosition("globerMonster.CurrentState()",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"backField"}},
				BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
				Name = "failBlock",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("막기 실패",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
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
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
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
					Name = "monsterAttack",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition("globerMonster.AttackCry()",15,3+5,1){AlignH = true}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"monsterReactionPhase"}},
					BackgroundText = backgrounds.GetBackground(1)
			});
			choices.Add(new Choice(){
					Name = "monsterReactionPhase",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition("globerMonster.Reaction()",5,10,10){AlignH = true,PriorityLayer=1}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
					BackgroundText = backgrounds.GetBackground(1)
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
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{
			new TextAndPosition("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓",0,0),
			new TextAndPosition("┃",0,1),new TextAndPosition("┃",width,1),
			new TextAndPosition("┃",0,2),new TextAndPosition("┃",width,2),
			new TextAndPosition("┃",0,3),new TextAndPosition("┃",width,3),
			new TextAndPosition("┃",0,4),new TextAndPosition("┃",width,4),
			new TextAndPosition("┃",0,5),new TextAndPosition("┃",width,5),
			new TextAndPosition("┃",0,6),new TextAndPosition("┃",width,6),
			new TextAndPosition("┃",0,7),new TextAndPosition("┃",width,7),
			new TextAndPosition("┃",0,8),new TextAndPosition("┃",width,8),
			new TextAndPosition("┃",0,9),new TextAndPosition("┃",width,9),
			new TextAndPosition("┃",0,10),new TextAndPosition("┃",width,10),
			new TextAndPosition("┃",0,11),new TextAndPosition("┃",width,11),
			new TextAndPosition("┃",0,12),new TextAndPosition("┃",width,12),
			new TextAndPosition("┃",0,13),new TextAndPosition("┃",width,13),
			new TextAndPosition("┃",0,14),new TextAndPosition("┃",width,14),
			new TextAndPosition("┃",0,15),new TextAndPosition("┃",width,15),
			new TextAndPosition("┃",0,16),new TextAndPosition("┃",width,16),
			new TextAndPosition("┃",0,17),new TextAndPosition("┃",width,17),
			new TextAndPosition("┃",0,18),new TextAndPosition("┃",width,18),
			new TextAndPosition("┃",0,19),new TextAndPosition("┃",width,19),
			new TextAndPosition("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛",0,20)
		}));
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{
			new TextAndPosition("┏━┳━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━┓",0,0),
			new TextAndPosition("┣",0,1),new TextAndPosition("━",1,1),new TextAndPosition("┛",2,1),new TextAndPosition("━",width-1,1),new TextAndPosition("┗",width-2,1),new TextAndPosition("┫",width,1),
			new TextAndPosition("┃",0,2),									new TextAndPosition("┃",width,2),
			new TextAndPosition("┃",0,3),									new TextAndPosition("┃",width,3),
			new TextAndPosition("┃",0,4),									new TextAndPosition("┃",width,4),
			new TextAndPosition("┃",0,5),									new TextAndPosition("┃",width,5),
			new TextAndPosition("┃",0,6),									new TextAndPosition("┃",width,6),
			new TextAndPosition("┃",0,7),									new TextAndPosition("┃",width,7),
			new TextAndPosition("┃",0,8),									new TextAndPosition("┃",width,8),
			new TextAndPosition("┃",0,9),									new TextAndPosition("┃",width,9),
			new TextAndPosition("┃",0,10),									new TextAndPosition("┃",width,10),
			new TextAndPosition("┃",0,11),									new TextAndPosition("┃",width,11),
			new TextAndPosition("┃",0,12),									new TextAndPosition("┃",width,12),
			new TextAndPosition("┃",0,13),									new TextAndPosition("┃",width,13),
			new TextAndPosition("┃",0,14),									new TextAndPosition("┃",width,14),
			new TextAndPosition("┃",0,15),									new TextAndPosition("┃",width,15),
			new TextAndPosition("┃",0,16),									new TextAndPosition("┃",width,16),
			new TextAndPosition("┃",0,17),									new TextAndPosition("┃",width,17),
			new TextAndPosition("┃",0,18),									new TextAndPosition("┃",width,18),
			new TextAndPosition("┣",0,19),new TextAndPosition("━",1,19),new TextAndPosition("┓",2,19),new TextAndPosition("━",width-1,19),new TextAndPosition("┏",width-2,19),new TextAndPosition("┫",width,19),
			new TextAndPosition("┗━┻━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━┛",0,20)
		}));
		
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{
			new TextAndPosition("┏━┳━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━┓",0+12,0),
			new TextAndPosition("┣",0+12,1),new TextAndPosition("━",1+12,1),new TextAndPosition("┫",2+12,1),new TextAndPosition("━",width-1,1),new TextAndPosition("┣",width-2,1),new TextAndPosition("┫",width,1),
			new TextAndPosition("┣",0+12,2),new TextAndPosition("━",1+12,2), new TextAndPosition("┫",2+12,2), new TextAndPosition("┣",width-2,2),new TextAndPosition("━",width-1,2),new TextAndPosition("┫",width,2),
			new TextAndPosition("┣",0+12,3),new TextAndPosition("━",1+12,3), new TextAndPosition("┫",2+12,3),	new TextAndPosition("┣",width-2,3),new TextAndPosition("━",width-1,3),new TextAndPosition("┫",width,3),
			new TextAndPosition("┣",0+12,4),new TextAndPosition("━",1+12,4), new TextAndPosition("┫",2+12,4),	new TextAndPosition("┣",width-2,4),new TextAndPosition("━",width-1,4),new TextAndPosition("┫",width,4),
			new TextAndPosition("┣",0+12,5),new TextAndPosition("━",1+12,5), new TextAndPosition("┫",2+12,5),	new TextAndPosition("┣",width-2,5),new TextAndPosition("━",width-1,5),new TextAndPosition("┫",width,5),
			new TextAndPosition("┣",0+12,6),new TextAndPosition("━",1+12,6), new TextAndPosition("┫",2+12,6),	new TextAndPosition("┣",width-2,6),new TextAndPosition("━",width-1,6),new TextAndPosition("┫",width,6),
			new TextAndPosition("┣",0+12,7),new TextAndPosition("━",1+12,7), new TextAndPosition("┫",2+12,7),	new TextAndPosition("┣",width-2,7),new TextAndPosition("━",width-1,7),new TextAndPosition("┫",width,7),
			new TextAndPosition("┣",0+12,8),new TextAndPosition("━",1+12,8), new TextAndPosition("┫",2+12,8),	new TextAndPosition("┣",width-2,8),new TextAndPosition("━",width-1,8),new TextAndPosition("┫",width,8),
			new TextAndPosition("┣",0+12,9),new TextAndPosition("━",1+12,9), new TextAndPosition("┫",2+12,9),	new TextAndPosition("┣",width-2,9),new TextAndPosition("━",width-1,9),new TextAndPosition("┫",width,9),
			new TextAndPosition("┣",0+12,10),new TextAndPosition("━",1+12,10), new TextAndPosition("┫",2+12,10),new TextAndPosition("┣",width-2,10),new TextAndPosition("━",width-1,10),new TextAndPosition("┫",width,10),
			new TextAndPosition("┣",0+12,11),new TextAndPosition("━",1+12,11), new TextAndPosition("┫",2+12,11),new TextAndPosition("┣",width-2,11),new TextAndPosition("━",width-1,11),new TextAndPosition("┫",width,11),
			new TextAndPosition("┣",0+12,12),new TextAndPosition("━",1+12,12), new TextAndPosition("┫",2+12,12),new TextAndPosition("┣",width-2,12),new TextAndPosition("━",width-1,12),new TextAndPosition("┫",width,12),
			new TextAndPosition("┗━┻━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━┛",0+12,13)
		}));
		
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{
			new TextAndPosition("┏━┳━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━┓",16,2),
			new TextAndPosition("┣",16,3),new TextAndPosition("━",17,3),new TextAndPosition("┛",18,3),new TextAndPosition("━",width-17,3),new TextAndPosition("┗",width-18,3),new TextAndPosition("┫",width-16,3),		new TextAndPosition("┃",16,4),new TextAndPosition("                                 ",17,4),new TextAndPosition("                          ",19,3),									new TextAndPosition("┃",width-16,4),
			new TextAndPosition("┃",16,5),new TextAndPosition("                                 ",17,5),									new TextAndPosition("┃",width-16,5),
			new TextAndPosition("┃",16,6),new TextAndPosition("                                 ",17,6),									new TextAndPosition("┃",width-16,6),
			new TextAndPosition("┃",16,7),new TextAndPosition("                                 ",17,7),									new TextAndPosition("┃",width-16,7),
			new TextAndPosition("┃",16,8),new TextAndPosition("                                 ",17,8),									new TextAndPosition("┃",width-16,8),
			new TextAndPosition("┃",16,9),new TextAndPosition("                                 ",17,9),									new TextAndPosition("┃",width-16,9),
			new TextAndPosition("┃",16,10),new TextAndPosition("                                ",17,10),									new TextAndPosition("┃",width-16,10),
			new TextAndPosition("┃",16,11),new TextAndPosition("                                ",17,11),									new TextAndPosition("┃",width-16,11),
			new TextAndPosition("┃",16,12),new TextAndPosition("                                ",17,12),									new TextAndPosition("┃",width-16,12),
			new TextAndPosition("┃",16,13),new TextAndPosition("                                ",17,13),									new TextAndPosition("┃",width-16,13),
			new TextAndPosition("┃",16,14),new TextAndPosition("                                ",17,14),new TextAndPosition("                           ",18,15),									new TextAndPosition("┃",width-16,14),
			new TextAndPosition("┣",16,15),new TextAndPosition("━",17,15),new TextAndPosition("┓",18,15),new TextAndPosition("━",width-17,15),new TextAndPosition("┏",width-18,15),new TextAndPosition("┫",width-16,15),
			new TextAndPosition("┗━┻━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━┛",16,16)
		}));
		
		int x = 50;
		int y = -2;
		background.Add(new List<TextAndPosition>(new TextAndPosition[]{
			new TextAndPosition("┏━┳━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━┓",16+x,2+y),
			new TextAndPosition("┣",16+x,3+y),new TextAndPosition("━",17+x,3+y),new TextAndPosition("┛",18+x,3+y),new TextAndPosition("━",width-17+x,3+y),new TextAndPosition("┗",width-18+x,3+y),new TextAndPosition("┫",width-16+x,3+y),		new TextAndPosition("┃",16+x,4+y),									new TextAndPosition("┃",width-16+x,4),
			new TextAndPosition("┃",16+x,5+y),									new TextAndPosition("┃",width-16+x,5+y),
			new TextAndPosition("┃",16+x,6+y),									new TextAndPosition("┃",width-16+x,6+y+y),
			new TextAndPosition("┃",16+x,7+y),									new TextAndPosition("┃",width-16+x,7+y),
			new TextAndPosition("┃",16+x,8+y),									new TextAndPosition("┃",width-16+x,8+y),
			new TextAndPosition("┃",16+x,9+y),									new TextAndPosition("┃",width-16+x,9+y),
			new TextAndPosition("┃",16+x,10+y),									new TextAndPosition("┃",width-16+x,10+y),
			new TextAndPosition("┃",16+x,11+y),									new TextAndPosition("┃",width-16+x,11+y),
			new TextAndPosition("┃",16+x,12+y),									new TextAndPosition("┃",width-16+x,12+y),
			new TextAndPosition("┃",16+x,13+y),									new TextAndPosition("┃",width-16+x,13+y),
			new TextAndPosition("┣",16+x,14+y),new TextAndPosition("━",17+x,14+y),new TextAndPosition("┓",18+x,14+y),new TextAndPosition("━",width-17+x,14+y),new TextAndPosition("┏",width-18+x,14+y),new TextAndPosition("┫",width-16+x,14+y),
			new TextAndPosition("┗━┻━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━┛",16+x,15+y)
		}));

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