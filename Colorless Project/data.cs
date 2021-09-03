using System;
using System.Collections.Generic;
using Characters;

public class Scenario{
		int clikX;
		int clikY;
		
		List<Choice> choices = new List<Choice>();
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
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"},{1,"test"},{2,"option"}},
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
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"},{1,"test"},{2,"option"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "option",
				SelectText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",10,5,true){PriorityLayer = 2},
							  new TextAndPosition(200,"Exit",10,6,true){PriorityLayer = 3}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("옵션은 없다.",5,3,30){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"},{1,"exit"}},
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
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition()},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("나 갈 수 없 다",5,3,100,ConsoleColor.Red)},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"}}
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
				IndicateChoice = new Dictionary<int,String>(){{0,"QuickNext-test"},{1,"testStream"}},
				
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});
			/*
			clikX = 23;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "c2-right",
				SelectText = new List<TextAndPosition>()
							{new TextAndPosition("다시시작",12+clikX,7+clikY,true){PriorityLayer = 2}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("함정이였다.",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"You Died",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"QuickNext-test"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			*/
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
			/*
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
				IndicateChoice = new Dictionary<int,String>(){{0,"t0827-1"},{1,"t0827-2"}},
				
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
				IndicateChoice = new Dictionary<int,String>(){{0,"t0827-1"},{1,"t0827-2"}},
				
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
				IndicateChoice = new Dictionary<int,String>(){{0,"t0828-1"},{1,"t0828-1"}},
				
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});*/
			
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
			new TextAndPosition("┣",0,19),new TextAndPosition("━",1,19),new TextAndPosition("┓",2,19),new TextAndPosition("━",width-1,19),new TextAndPosition("┏",width-2,19),new TextAndPosition("┫",width,19),
			new TextAndPosition("┗━┻━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━┛",0,20)
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
