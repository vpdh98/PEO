using System;
using System.Collections.Generic;
using System.Threading;
using Characters;
using static Define;

public static class GameWindows{
	static Backgrounds backgrounds = new Backgrounds();
	
	public static bool ConfirmWindow(String text,int xPos,int yPos){
		DisplayTextGame CDTG = new DisplayTextGame(false);
		
		Choice ConfirmCho = new Choice(){
				Name = "ConfirmWindow",
				SelectText = new List<TextAndPosition>()
							{new TextAndPosition("확인",xPos,yPos+5,true){PriorityLayer = 2},
							new TextAndPosition("취소",xPos+10,yPos+5,true){PriorityLayer = 3}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(text,xPos,yPos){PriorityLayer = 1,AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,true},{1,false}},
				BackgroundTextName = "confirm"
		};
		
		CDTG.Cho = ConfirmCho; //화면 할당
		CDTG.Show();
				
		bool confirm = false;	
			
		ConsoleKeyInfo keyInfo = Console.ReadKey();
		while(keyInfo.Key != ConsoleKey.Escape){
			CDTG.SelectingText(keyInfo);
					
			if(keyInfo.Key == ConsoleKey.Enter){
				confirm = (bool)CDTG.Cho.GetValueOn(CDTG.currentSelectNum);
				return confirm;
			}
			else{
				CDTG.Show();
				keyInfo = Console.ReadKey();
			}
		}
		return confirm;
	}
	
	public static void AlertWindow(String text,int windowXPos = SCREEN_POS_X,int windowYPos = SCREEN_POS_Y,int textXPos = SCREEN_POS_X+4,int textYPos = SCREEN_POS_Y,String background = "confirm",int delay = 0,ConsoleColor color = ConsoleColor.Black){
		DisplayTextGame CDTG = new DisplayTextGame(false){GlobalPositionX=windowXPos,GlobalPositionY=windowYPos};
		Choice AlertCho = new Choice(){
				Name = "AlertWindow",
				SelectText = new List<TextAndPosition>(),
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(20,text,textXPos,textYPos,20,color:color){PriorityLayer = 1,AlignH = true}},
				BackgroundTextName = background
		};
		
		CDTG.Cho = AlertCho; //화면 할당
		CDTG.Show();
		if(delay > 0){
			Thread.Sleep(delay);
		}
		else{
			ConsoleKeyInfo keyInfo = Console.ReadKey();
		}
	}
	
	public static void ExplanWindow(Item item,int windowXPos = SCREEN_POS_X,int windowYPos = SCREEN_POS_Y,int textXPos = SCREEN_POS_X+4,int textYPos = SCREEN_POS_Y,int background = 3){
		DisplayTextGame CDTG = new DisplayTextGame(false){GlobalPositionX=windowXPos,GlobalPositionY=windowYPos};
		List<TextAndPosition> tap = new List<TextAndPosition>();
		if(item is Weapon){
			Weapon wep = item as Weapon;
			tap = new List<TextAndPosition>()
								{new TextAndPosition(item.Explan(),textXPos,textYPos),
								new TextAndPosition("공격력: "+wep.AttackPower,textXPos,textYPos+11),
								new TextAndPosition("속도: "+wep.AttackSpeed,textXPos+15,textYPos+11)};
			
		}
		else if(item is Armor){
			Armor arm = item as Armor;
			tap = new List<TextAndPosition>()
								{new TextAndPosition(item.Explan(),textXPos,textYPos),
								new TextAndPosition("방어력: "+arm.Defense,textXPos,textYPos+11)};
		}
		else if(item.IsStackable){
			tap = new List<TextAndPosition>()
								{new TextAndPosition(item.Explan(),textXPos,textYPos),
								new TextAndPosition("수량: "+item.Amount,textXPos,textYPos+11)};
		}
		else{
			tap = new List<TextAndPosition>()
								{new TextAndPosition(item.Explan(),textXPos,textYPos)};
		}
		Choice ConfirmCho = new Choice(){
					Name = "ExplanWindow",
					OnlyShowText = tap,
					BackgroundTextName = "itemExplan"
			};
		
		CDTG.Cho = ConfirmCho; //화면 할당
		CDTG.Show();
	}
	
	public static void StateWindow(Player player,int XPos,int YPos){
		DisplayTextGame SDTG = new DisplayTextGame(false){GlobalPositionX=XPos,GlobalPositionY=YPos,ScreenSize_Width = 28,ScreenSize_Height = 21};
		
		Choice StateCho = new Choice(){
				Name = "StateWindow",
				OnlyShowText = new List<TextAndPosition>(){
					new TextAndPosition("[   상태창   ]",1,1){AlignH = true},
					new TextAndPosition("이름:"+player.Name,1,2),
					new TextAndPosition("HP:"+player.Hp+"/"+player.MaxHp,1,3),
					new TextAndPosition("공격력:"+player.AttackPower+(player.Weapon==null?"":"+"+player.Weapon.AttackPower+"("+player.Weapon.Name+")"),1,4),
					new TextAndPosition("방어력:"+player.Defense+(player.Armor==null?"":"("+player.Armor.Name+")"),1,5),
					new TextAndPosition("스피드:"+player.AttackSpeed,1,6),
					new TextAndPosition("상태이상:",1,7),
					new TextAndPosition("--진행중인 퀘스트 목록--",0,8){AlignH = true}
				},
				BackgroundTextName = "state"
		};
		for(int i = 0;i<player.QuestList.Count;i++){
			Quest tQuest = player.QuestList[i];
			StateCho.OnlyShowText.Add(new TextAndPosition(""+(1+i)+"."+tQuest.QuestName+(tQuest.isComplete?"(완료)":"(진행중)"),1,9+i));
		}
		
		SDTG.Cho = StateCho; //화면 할당
		SDTG.Show();
	}
}