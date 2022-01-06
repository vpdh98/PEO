using System;
using Characters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game;
using static Convenience;
//using static DamageSystem;
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
	public static ChoiceControler NpcCC = new ChoiceControler(new Scenario());
	public static String currentChoice = "GreetPhase";
	
	public static List<Quest> NpcQuestList;
	public static Quest currentQuest;
	
	public static ConsoleKeyInfo keyInfo;
	
	public static String lastQuestName;
	public static int lastQuestIndex;
	
	public static List<Quest> sameQuestList;
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
		sameQuestList = QuestControler.SameQuestList(player.QuestList,npc.QuestList);
		
		globalPlayer = player;
		globalNPC = npc;
		backField = back;
		
		NpcQuestList = npc.QuestList;
		
		currentChoice = "GreetPhase";
		
		if(!NpcCC.GetChoice(currentChoice).IsVisit){
			NpcTalkInit();
		}
		
		if(QuestControler.ContainsCompleteQuest(sameQuestList)){
			ReplaceQuestCompleteText();
		}else{
			ReplaceQuestIntroductionText();
		}
		
		
		NpcDTG.Display(NpcCC.GetChoiceClone(currentChoice));
		/*while(true){
			keyInfo = Console.ReadKey();
			if(keyInfo.Key == ConsoleKey.Enter){
				currentChoice = (String)NpcDTG.GetCurrentSelectValue();
				if(currentChoice == "end") return backField;
				currentQuest = NpcQuestList.Find(q => q.QuestName.Equals(currentChoice));
				if(currentQuest != null && currentQuest.QuestName == currentChoice)
				{
					NpcDTG.Display(currentQuest.QuestContents);
					lastQuestName = currentChoice;
					lastQuestIndex = NpcDTG.currentSelectNum;
				}
				else{
					NpcDTG.Display(NpcCC.GetChoice(currentChoice));
				}
				break;
			}
			NpcDTG.SelectingText(keyInfo);
			NpcDTG.Display();
		}*/
		
		while(currentChoice != "end")
		{
			
			keyInfo = Console.ReadKey();
			NpcDTG.SelectingText(keyInfo);
			if(keyInfo.Key == ConsoleKey.Enter)
			{
				currentChoice = (String)NpcDTG.GetCurrentSelectValue();
				if(currentChoice == "end"){ 
					NpcCC.GetChoice("ConversationPhase").LeaveChoice();
					NpcCC.GetChoice("GreetPhase").LeaveChoice();
					return backField;
				}
				
				if(currentChoice == "QuestAccept"){
					Quest tQuest = NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName));
					tQuest.isAccept = true;
					globalPlayer.QuestList.Add(tQuest);//마지막에 수락한 Quest의 객체를 플레이어에게 넘김
					if(tQuest.isReject){
						NpcCC.ChangeChoiceText(choiceName:"QuestAccept",onlyShowText:npc.GetRevisitQuestAcceptMessage());
						NpcCC.ChangeChoiceText(choiceName:"QuestReject",onlyShowText:npc.GetRevisitQuestRejectMessage());
					}else{
						NpcCC.ChangeChoiceText(choiceName:"QuestAccept",onlyShowText:npc.GetQuestAcceptMessage());
						NpcCC.ChangeChoiceText(choiceName:"QuestReject",onlyShowText:npc.GetQuestRejectMessage());
					}
					NpcCC.RemoveChoiceSelectText("QuestIntroduction",lastQuestIndex); //목록에서 삭제
					if(NpcCC.GetChoice("QuestIntroduction").SelectText.Count == 1)
					{
						NpcCC.RemoveChoiceSelectText("GreetPhase",1);
					}
				}
				if(currentChoice == "QuestReject"){
					Quest tQuest = NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName));
					if(tQuest.isReject){
						NpcCC.ChangeChoiceText(choiceName:"QuestAccept",onlyShowText:npc.GetRevisitQuestAcceptMessage());
						NpcCC.ChangeChoiceText(choiceName:"QuestReject",onlyShowText:npc.GetRevisitQuestRejectMessage());
					}else{
						NpcCC.ChangeChoiceText(choiceName:"QuestAccept",onlyShowText:npc.GetQuestAcceptMessage());
						NpcCC.ChangeChoiceText(choiceName:"QuestReject",onlyShowText:npc.GetQuestRejectMessage());
					}
						
				}
				if(currentChoice == "QuestComplete"){
					Choice tChoice = NpcCC.GetChoice("QuestReward");
					lastQuestName = PlayData.CurrentSelectedText.text;
					tChoice.QuickDelegate = ()=>{
						Quest tQuest = NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName));
						tQuest.TakeRewardAll();
					};
				}
				
				//AAAAAAA선택한 Choice Display전AAAAAAA//
				currentQuest = NpcQuestList.Find(q => q.QuestName.Equals(currentChoice));
				if(currentQuest != null && currentQuest.QuestName == currentChoice)
				{
					lastQuestIndex = NpcDTG.currentSelectNum;
					NpcDTG.Display(currentQuest.QuestContents);
					lastQuestName = currentChoice;
				}
				else if(currentChoice == "QuestReward"){
					NpcDTG.Cho = NpcCC.GetChoice("QuestReward");
					NpcDTG.Cho.QuickRun();
					
					player.QuestList.Remove(NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName)));
					NpcQuestList.Remove(NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName)));
					NpcCC.RemoveChoiceSelectText("GreetPhase",1);
					
					currentChoice = (String)NpcDTG.Cho.QuickNext();
					Console.ReadKey();
					NpcDTG.Display(NpcCC.GetChoice(currentChoice));
				}
				else{
					NpcDTG.Display(NpcCC.GetChoice(currentChoice));
				}
				//VVVVVVV선택한 Choice Display후VVVVVVV//
				if(currentChoice == "QuestReject"){
					//testLog(lastQuestName);
					NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName)).isReject = true;
					//testLog(NpcQuestList.Find(q => q.QuestName.Equals(lastQuestName)).isReject);
				}
				
				
					
				if(NpcDTG.Cho.ChoiceType == ChoiceType.QUICKNEXT){
					currentChoice = (String)NpcDTG.Cho.QuickNext();
					Console.ReadKey();
					NpcDTG.Display(NpcCC.GetChoice(currentChoice));
				}
				
				
			}
			else
			{
				NpcDTG.Display();
			}
			
			
		}
		return backField;
		
	}
	
	
	public static void ReplaceQuestCompleteText(){
		if(NpcCC.GetChoice("QuestIntroduction").SelectText.Count == 1){
			Dictionary<int,Object> indicateList = NpcCC.GetChoice("GreetPhase").IndicateChoice;
			foreach(int key in indicateList.Keys){
				if((String)indicateList[key] == "QuestIntroduction"){
					NpcCC.RemoveChoiceSelectText("GreetPhase",key);
				}
			}
			
		}
		if(QuestControler.ContainsCompleteQuest(sameQuestList)){
				NpcCC.ChangeChoiceText(choiceName:"QuestComplete",onlyShowText:globalNPC.GetQuestCompleteMassage());
				NpcCC.ChangeChoiceText(choiceName:"CompleteQuestList",onlyShowText:globalNPC.GetCompleteQuestListMessage());
				NpcCC.AddChoiceSelectText("GreetPhase",globalNPC.GetPreCompleteQuestListMessage(),"CompleteQuestList");
			}
			if(sameQuestList != null && sameQuestList.Count > 0){
				int firstX = 22; //선택지가 아무것도 없을때 첫 선택지의 위치
				int lastY = 13;
				for(int i=0;i<sameQuestList.Count;i++){
					NpcCC.AddChoiceSelectText("CompleteQuestList",new TextAndPosition(sameQuestList[i].QuestName,firstX,lastY++ +1,true),"QuestComplete");
					firstX = GameManager.selectListFirststPositionX(NpcCC.GetChoice("CompleteQuestList").SelectText);
					lastY = GameManager.selectListLastPositionY(NpcCC.GetChoice("CompleteQuestList").SelectText);
				}
			}
	}
	
	public static void ReplaceQuestIntroductionText(){
		if(NpcQuestList != null&&NpcQuestList.Count > 0){
				int firstX = 22; //선택지가 아무것도 없을때 첫 선택지의 위치
				int lastY = 13;
				NpcCC.ChangeChoiceSelectText("GreetPhase",1,globalNPC.GetPreQuestMessage().text);
				for(int i=0;i<NpcQuestList.Count;i++){
					NpcCC.AddChoiceSelectText("QuestIntroduction",new TextAndPosition(NpcQuestList[i].QuestName,firstX,lastY++ +1,true),NpcQuestList[i].QuestName);
					firstX = GameManager.selectListFirststPositionX(NpcCC.GetChoice("QuestIntroduction").SelectText);
					lastY = GameManager.selectListLastPositionY(NpcCC.GetChoice("QuestIntroduction").SelectText);
				}
			}
			else{
				if(NpcCC.GetChoice("QuestIntroduction").SelectText.Count == 1){
				Dictionary<int,Object> indicateList = NpcCC.GetChoice("GreetPhase").IndicateChoice;
				foreach(int key in indicateList.Keys){
				if((String)indicateList[key] == "QuestIntroduction"){
					NpcCC.RemoveChoiceSelectText("GreetPhase",key);
				}
			}
			
		}
			}
	}
	
	public static void NpcTalkInit(){
		NpcCC.ChangeChoiceText(choiceName:"GreetPhase",onlyShowText:new TextAndPosition(globalNPC.GetGreetMessage().text,15,3+5,1){AlignH = true});
		NpcCC.ChangeChoiceText(choiceName:"ConversationPhase",streamText:globalNPC.GetConversationMessageList());
		NpcCC.ChangeChoiceText(choiceName:"QuestAccept",onlyShowText:globalNPC.GetQuestAcceptMessage());
		NpcCC.ChangeChoiceText(choiceName:"QuestReject",onlyShowText:globalNPC.GetQuestRejectMessage());
		NpcCC.ChangeChoiceText(choiceName:"QuestIntroduction",onlyShowText:globalNPC.GetQuestIntroductionMessage());
		NpcCC.ChangeChoiceText(choiceName:"GreetPhase",returnText:globalNPC.GetRevisitGreetMessage());
		NpcCC.ChangeChoiceText(choiceName:"QuestAccept",returnText:globalNPC.GetRevisitQuestAcceptMessage());
		NpcCC.ChangeChoiceText(choiceName:"QuestReject",returnText:globalNPC.GetRevisitQuestRejectMessage());
		NpcCC.ChangeChoiceText(choiceName:"ConversationPhase",returnText:globalNPC.GetRevisitConversationMessage());
	}
}

