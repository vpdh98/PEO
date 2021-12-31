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
	public static ChoiceControler NpcCC = new ChoiceControler();
	public static String currentChoice = "GreetPhase";
	
	public static List<Quest> NpcQuestList;
	public static Quest currentQuest;
	/*
	public List<Quest> QuestList{get;set;}
			public List<TextAndPosition> GreetMessage{get;set;}
			public List<TextAndPosition> ConversationMessage{get;set;}
			public List<TextAndPosition> QuestAcceptMessage{get;set;}
			public List<TextAndPosition> QuestRejectMessage{get;set;}
			public List<TextAndPosition> QuestCompleteMassage{get;set;}
			public List<TextAndPosition> RevisitGreetMessage{get;set;}
			public List<TextAndPosition> RevisitConversationMessage{get;set;}
			public List<TextAndPosition> RevisitQuestAcceptMessage{get;set;}
			public List<TextAndPosition> RevisitQuestRejectMessage{get;set;}
			public List<TextAndPosition> PreQuestMessage{get;set;}
			public List<TextAndPosition> RevisitPreQuestMessage{get;set;}
			
			GreetPhase
			ConversationPhase
			QuestAccept
			QuestReject
			QuestComplete
	*/
	public static String Accost(Player player,NPC npc,String back){
		globalPlayer = player;
		globalNPC = npc;
		backField = back;
		
		NpcQuestList = npc.QuestList;
		
		NpcCC.ChangeChoiceText(choiceName:"GreetPhase",onlyShowText:npc.GetGreetMessage());
		//NpcCC.ChangeChoiceText(choiceName:"ConversationPhase",onlyShowText:npc.GetGreetMessage());
		//NpcCC.ChangeChoiceText(choiceName:"QuestAccept",onlyShowText:npc.GetGreetMessage());
		//NpcCC.ChangeChoiceText(choiceName:"QuestReject",onlyShowText:npc.GetGreetMessage());
		
		
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
		
		while(currentChoice != "end")
		{
			if(currentChoice == "end") return backField;
			keyInfo = Console.ReadKey();
			BDTG.SelectingText(keyInfo);
			if(keyInfo.Key == ConsoleKey.Enter)
			{
				currentChoice = (String)BDTG.GetCurrentSelectValue();
				currentQuest = NpcQuestList.Find(q => q.QuestName.Equals(currentChoice));
				if(currentQuest.QuestName == currentChoice)
				{
					BDTG.Display(currentQuest.QuestContents);
				}else{
					BDTG.Display(BCC.GetChoice(currentChoice));
				}
			}else{
				BDTG.Display();
			}
			
			
		}
		return backField;
		
	}
}

