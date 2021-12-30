using System;
using Characters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game;
using static Convenience;
using static DamageSystem;
using static GameWindows;
using static PlayData;

public enum NpcType{
	NOMAR_NPC,
	MERCHANT
}

public static class TalkToNPC{
	public static Player globalPlayer;
	public static NPC globalNPC;
	public static String backField;
	public static DisplayTextGame NpcDTG = new DisplayTextGame();
	
	public static String Accost(Player player,NPC npc,String back){
		globalPlayer = player;
		globalNPC = npc;
		backField = back;
		
		
		/*
		BDTG.Display(BCC.GetChoiceClone(currentChoice));
		while(true){
			keyInfo = Console.ReadKey();
			if(keyInfo.Key == ConsoleKey.Enter){
				currentChoice = (String)BDTG.GetCurrentSelectValue();
				if(currentChoice == "end") return backField;
				BDTG.Display(BCC.GetChoiceClone(currentChoice));
				break;
			}
			BDTG.SelectingText(keyInfo);
			BDTG.Display();
		}
		
		while(!battleAnd)
		{
			if(died)
			{
				died = false;
				globerPlayer.Hp = globerPlayer.MaxHp; //살아날때 체력 회복
				timeOut = false;
				count = 0;
				return savePoint;
			}
			TimerStart();
			while(true){
				Thread.Sleep(10);
				if(timeOut){ EnemyTurn(); }
				if(died)
				{
					died = false;
					globerPlayer.Hp = globerPlayer.MaxHp; //살아날때 체력 회복
					timeOut = false;
					count = 0;
					return savePoint;
				}
				if(Console.KeyAvailable) break;
			}
			keyInfo = Console.ReadKey();
			BDTG.SelectingText(keyInfo);
			if(keyInfo.Key == ConsoleKey.Enter){
				currentChoice = (String)BDTG.GetCurrentSelectValue();
				
				if(currentChoice == "movePhase") continue;
				timerEnd = true;
				timer.Wait();
				PlayerTurn();
				timeOut = false;
			}else{
				BDTG.Display();
			}
			
			
		}*/
		return backField;
		
	}
}

