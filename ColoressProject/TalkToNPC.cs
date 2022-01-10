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



public static class TalkToNPC
{
	public static Player globalPlayer; //player객체가 들어갈 변수
	public static NPC globalNPC;		//NPC객체가 들어갈 변수
	public static String backField;		//NPC화면에서 다시 돌아갈 장소를 저장
	public static DisplayTextGame NpcDTG = new DisplayTextGame();		//TalkToNPC에서 쓸 DisplayTextGame객체
	public static ChoiceControler NpcCC = new ChoiceControler(new Scenario());	//TalkToNPC에서 쓸 ChoiceControler객체
	public static String currentChoice = "GreetPhase";		//현재 화면의 이름
	
	public static List<Quest> NpcQuestList;		//NPC의 퀘스트 목록을 저장
	public static Quest currentQuest;			//출력하는데에 사용할 퀘스트 객체를 담을 변수
	
	public static ConsoleKeyInfo keyInfo;		//Console.ReadKey()로 받아오는 입력 값을 저장할 변수
	
	public static String lastQuestName;			//마지막으로 접근한 퀘스트의 이름을 담을 변수
	public static int lastQuestIndex;			//마지막으로 접근한 퀘스트의 선택지 인덱스를 담는 변수
	
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
	
	
	
	
	
	public static String Accost(Player player,NPC npc,String back)
	{ //NPC객체와의 소통,출력을 담당할 메소드. 플레이어와 NPC간의 모든 상호작용을 담당함
		player.PlayerQuestCheck(npc); //MeetingQuest의 완료조건 확인
		sameQuestList = QuestControler.SameQuestList(player.QuestList,npc.QuestList); //플레이어가 가지고 있는 퀘스트 목록과 NPC가 가지고 있는 퀘스트의 목록중 일치하는 퀘스트를 저장
		
		globalPlayer = player; //매개변수로 받아온 player의 객체를 저장
		globalNPC = npc;		//NPC의 객체를 저장
		backField = back;		//돌아갈 필드의 이름을 저장
		
		NpcQuestList = npc.QuestList;	//NPC의 퀘스트 목록을 저장
		
		currentChoice = "GreetPhase";	//첫 화면을 저장
		
		if(!NpcCC.GetChoice(currentChoice).IsVisit) //현재 초이스가 방문한적이 없다면 NPC관련 Choice의 텍스트들을 초기화
		{
			NpcTalkInit();
		}
		
		if(QuestControler.ContainsCompleteQuest(sameQuestList)) //일치하는 퀘스트중 완료한 퀘스트가 있다면 실행
		{	
			ReplaceQuestCompleteText(); //QuestComplete이라는 이름의 Choice의 선택지를 추가.동시에 GreetPhase에 CompleteQuestList로 가는 선택지를 추가
		}
		else
		{
			ReplaceQuestIntroductionText();	//QuestIntroduction이라는 이름의 Choice의 선택지를 추가, 동시에 GreetPhase에 QuestIntroduction으로 가는 선택지를 추가
		}
		
		NpcDTG.Display(NpcCC.GetChoiceClone(currentChoice));
		
		while(currentChoice != "end")
		{
			
			keyInfo = Console.ReadKey();																						//1.입력 받음
			NpcDTG.SelectingText(keyInfo);																						//2.입력에 따라 내부적으로 선택지의 Index변경
			if(keyInfo.Key == ConsoleKey.I) { player.inven.OpenInventory(); }
			if(keyInfo.Key == ConsoleKey.Enter)																					//3.그 입력이 Enter키 일 경우
			{
				currentChoice = (String)NpcDTG.GetCurrentSelectValue();															//4.현재 선택지의 index에 있는 value를 가져옴
				if(currentChoice == "end")
				{//currentChoice가 end면 이전 장소로 돌아간다.
					NpcCC.GetChoice("ConversationPhase").LeaveChoice(); //해당 Choice의 출력 문자를 변경(ex. 처음 보는구먼. 반갑네 -> 또 왔구먼. 잘 쉬다가게)
					NpcCC.GetChoice("GreetPhase").LeaveChoice();
					return backField; //이전 필드의 이름을 return;
				}
				
				if(currentChoice == "QuestAccept")
				{//현재 Choice가 QuestAccept라면 실행
					Quest tQuest = QuestControler.FindQuestByName(NpcQuestList,lastQuestName);
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
						NpcCC.RemoveChoiceSelectTextByValue("GreetPhase","QuestIntroduction");
					}
					//ReplaceQuestIntroductionText();
				}
				if(currentChoice == "QuestReject"){
					Quest tQuest = QuestControler.FindQuestByName(NpcQuestList,lastQuestName);
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
						Quest tQuest = QuestControler.FindQuestByName(NpcQuestList,lastQuestName);
						tQuest.TakeRewardAll();
					};
					NpcCC.ChangeChoiceText(choiceName:"QuestComplete",onlyShowText:QuestControler.FindQuestByName(NpcQuestList,lastQuestName).QuestCompleteMessage);
				}
				
				//AAAAAAA선택한 Choice Display전AAAAAAA//
				currentQuest = QuestControler.FindQuestByName(NpcQuestList,currentChoice);
				if(currentQuest != null && currentQuest.QuestName == currentChoice)
				{
					lastQuestIndex = NpcDTG.currentSelectNum;
					NpcDTG.Display(currentQuest.QuestContents);
					lastQuestName = currentChoice;
				}
				else if(currentChoice == "QuestReward"){
					if(QuestControler.FindQuestByName(NpcQuestList,lastQuestName).QuestReward.Count+player.inven.InventoryList.Count > 20){//보상의 아이템 갯수와 플레이어 인벤의 아이템 갯수의 합이 20을 초과하면 보상지급 취
						currentChoice = "GreetPhase";
						AlertWindow("인벤토리 공간이 부족합니다!",textXPos:24,textYPos:9);
						NpcDTG.Display(NpcCC.GetChoice(currentChoice));
						continue;
					}
					NpcDTG.Cho = NpcCC.GetChoice("QuestReward");
					NpcDTG.Cho.QuickRun();
					NpcCC.RemoveChoiceSelectTextByText("CompleteQuestList",lastQuestName);
					QuestControler.CompleteQuestList.Add(QuestControler.FindQuestByName(NpcQuestList,lastQuestName));
					player.QuestList.Remove(QuestControler.FindQuestByName(NpcQuestList,lastQuestName));
					sameQuestList.Remove(QuestControler.FindQuestByName(NpcQuestList,lastQuestName));
					NpcQuestList.Remove(QuestControler.FindQuestByName(NpcQuestList,lastQuestName));
					
					if(!QuestControler.ContainsCompleteQuest(sameQuestList)){
						NpcCC.RemoveChoiceSelectTextByValue("GreetPhase","CompleteQuestList");
					}
					
					if(!QuestControler.ContainsCompleteQuest(sameQuestList) && NpcCC.GetChoice("QuestIntroduction").SelectText.Count > 1){
						ReplaceQuestIntroductionText();
					}
					
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
					QuestControler.FindQuestByName(NpcQuestList,lastQuestName).isReject = true;
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
		
			
		NpcCC.RemoveChoiceSelectTextByValue("GreetPhase","QuestIntroduction");
		
		if(QuestControler.ContainsCompleteQuest(sameQuestList)){
				NpcCC.ChangeChoiceText(choiceName:"QuestComplete",onlyShowText:globalNPC.GetQuestCompleteMassage());
				NpcCC.ChangeChoiceText(choiceName:"CompleteQuestList",onlyShowText:globalNPC.GetCompleteQuestListMessage());
				NpcCC.AddChoiceSelectText("GreetPhase",globalNPC.GetPreCompleteQuestListMessage(),"CompleteQuestList");
			
			if(sameQuestList != null && sameQuestList.Count > 0){
				int firstX = 22; //선택지가 아무것도 없을때 첫 선택지의 위치
				int lastY = 13;
				for(int i=0;i<sameQuestList.Count;i++){
					if(sameQuestList[i].isComplete){
						NpcCC.AddChoiceSelectText("CompleteQuestList",new TextAndPosition(sameQuestList[i].QuestName,firstX,lastY++ +1,true),"QuestComplete");
						firstX = GameManager.selectListFirststPositionX(NpcCC.GetChoice("CompleteQuestList").SelectText);
						lastY = GameManager.selectListLastPositionY(NpcCC.GetChoice("CompleteQuestList").SelectText);
					}
				}
			}
		}
	}
	
	public static void ReplaceQuestIntroductionText(){
		if(NpcQuestList != null&&NpcQuestList.Count > 0){
				int firstX = 22; //선택지가 아무것도 없을때 첫 선택지의 위치
				int lastY = 13;
				if(Convenience.DictionaryContainsValue(NpcCC.GetChoice("GreetPhase").IndicateChoice,"QuestIntroduction")){
					NpcCC.RemoveChoiceSelectTextByValue("GreetPhase","QuestIntroduction");
					NpcCC.AddChoiceSelectText("GreetPhase",globalNPC.GetPreQuestMessage(),"QuestIntroduction");
				}
				else{
					NpcCC.AddChoiceSelectText("GreetPhase",globalNPC.GetPreQuestMessage(),"QuestIntroduction");
				}
				for(int i=0;i<NpcQuestList.Count;i++){
					if(!NpcQuestList[i].isAccept && !Convenience.DictionaryContainsValue(NpcCC.GetChoice("QuestIntroduction").IndicateChoice,NpcQuestList[i].QuestName)){
						NpcCC.AddChoiceSelectText("QuestIntroduction",new TextAndPosition(NpcQuestList[i].QuestName,firstX,lastY+1,true),NpcQuestList[i].QuestName);
						firstX = GameManager.selectListFirststPositionX(NpcCC.GetChoice("QuestIntroduction").SelectText);
						lastY = GameManager.selectListLastPositionY(NpcCC.GetChoice("QuestIntroduction").SelectText);
					}
				}
			}
			else{
				if(NpcCC.GetChoice("QuestIntroduction").SelectText.Count == 1){
					NpcCC.RemoveChoiceSelectTextByValue("GreetPhase","QuestIntroduction");
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

