using System;
using System.Collections.Generic;

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
				BackgroundText = backgrounds.GetBackground(3)
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
	
	public static void AlertWindow(String text,int xPos,int yPos){
		DisplayTextGame CDTG = new DisplayTextGame(false);
		
		Choice ConfirmCho = new Choice(){
				Name = "ConfirmWindow",
				SelectText = new List<TextAndPosition>(),
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(text,xPos,yPos){PriorityLayer = 1,AlignH = true}},
				BackgroundText = backgrounds.GetBackground(3)
		};
		
		CDTG.Cho = ConfirmCho; //화면 할당
		CDTG.Show();
		ConsoleKeyInfo keyInfo = Console.ReadKey();
	}
}
