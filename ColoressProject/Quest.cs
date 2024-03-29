using System;
using System.Collections.Generic;
using Characters;
//using static Characters.User;
//using static Game.GameManager;
//using Game;
using System.Threading;
//using System.Threading.Tasks;
using System.Linq;
//using System.IO;
//using static BattleSystem;
using static Convenience;
using static ItemData;
using static PlayData;
using static GameWindows;
using MyJson;

public static class QuestControler{ //퀘스트목록을 관리
	public static List<Quest> AllQuestList{get;set;} = new List<Quest>();//모든 퀘스트 목록
	public static List<Quest> AcceptQuestList{get;set;} = new List<Quest>();//수락한 퀘스트 목록
	public static List<Quest> CompleteQuestList{get;set;} = new List<Quest>();//완료된 퀘스트 목록
	
	public static List<Quest> LastAccessedQuest{get;set;}
	
	public static Quest GetQuestByName(String name){
		IsEmptyList<Quest>(AllQuestList); //////////////////////////12.29 객체 초기화가 안되있음
		return AllQuestList.Find(q => q.QuestName.Equals(name));
	}
	
	public static bool ContainsCompleteQuest(List<Quest> questList){
		if(questList == null) return false;
		foreach(Quest q in questList){
			if(q.isComplete){
				return true;
			}
		}
		return false;
	}
	
	public static Quest FindQuestByName(List<Quest> qList,String questName){
		return qList.Find(q => q.QuestName.Equals(questName));
	}
	
	public static List<Quest> SameQuestList(List<Quest> qList1,List<Quest> qList2){
		if(qList1 == null || qList2 == null) return new List<Quest>();
		
		List<Quest> tQuestList = new List<Quest>();
		for(int j = 0;j<qList1.Count;j++){
			for(int i =0 ;i < qList2.Count;i++){
				if(qList1[j].QuestName == qList2[i].QuestName){
					tQuestList.Add(qList2[i]);
				}
			}
		}
		return tQuestList;
	}
	
}

public class Quest : ICloneable,ISaveToJson{ //퀘스트 객체, 이걸 NPC와 Player끼리 주고 받으면서 퀘스트를 주거나 완료한다.
	public String QuestName{get;set;} = "UKNOWN"; //퀘스트 고유 이름
	public String QuestContentsName{get;set;} //퀘스트 내용
	private Choice questContents; //퀘스트 내용
	public List<Reward> QuestReward{get;set;}  //퀘스트 보상,여러개일 수 있다.
	public bool isComplete = false; //퀘스트 완료 여부
	public bool isAccept = false; //퀘스트 수락 여부
	public bool isReject = false; //퀘스트 한번이라도 거절했는지
	
	public TextAndPosition QuestCompleteMessage{get;set;} = new TextAndPosition("잘했네",10){AlignH = true};
	
	public Choice QuestContents{
		get{
			return questContents;
		}
		set{
			questContents = value; 
		}
	}
	
	public Quest(){
	}
	
	public virtual void CheckComplete(){ //완료 여부 확인
	}
	
	public virtual bool CheckTarget(Object info){//퀘스트 목표가 맞는지 확인
		return false;
	}
	
	public virtual void TakeRewardAll(){
		for(int i = 0;i<QuestReward.Count;i++){
			QuestReward[i].TakeReward();
		}
	}
	
	protected Quest(Quest that){
		this.QuestReward = (List<Reward>)ListClone<Reward>(that.QuestReward);
		this.QuestName = that.QuestName;
		this.QuestContentsName = that.QuestContentsName;
		this.QuestCompleteMessage = (TextAndPosition)that.QuestCompleteMessage.Clone();
		this.isComplete = that.isComplete;
		this.isAccept = that.isAccept;
	}
	
	public Object Clone(){
		return new Quest(this);
	}
	
	// public String QuestName{get;set;} = "UKNOWN"; //퀘스트 고유 이름
	// public Choice QuestContents{get;set;} //퀘스트 내용
	// public List<Reward> QuestReward{get;set;}  //퀘스트 보상,여러개일 수 있다.
	// public bool isComplete = false; //퀘스트 완료 여부
	// public bool isAccept = false; //퀘스트 수락 여부
	// public bool isReject = false; //퀘스트 한번이라도 거절했는지
	
	public virtual String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Quest");
		json.AddItem("QuestName",QuestName);
		json.AddItem("QuestContentsName",QuestContentsName);
		json.AddJsonAbleList("QuestReward",QuestReward,true);
		json.OpenObject("QuestCompleteMessage");
		json.AddJsonAbleObject(QuestCompleteMessage);
		json.CloseObject(true);
		json.AddItem("isComplete",isComplete);
		json.AddItem("isAccept",isAccept);
		json.AddItem("isReject",isReject);
		json.CloseObject();
		
		return json.JsonString;
	}
	public virtual void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.QuestName = json.GetItem("QuestName");
		this.QuestContentsName = json.GetItem("QuestContentsName");
		this.QuestReward = json.GetJsonAbleList<Reward>("QuestReward");
		this.QuestCompleteMessage = json.GetJsonAbleObject<TextAndPosition>("QuestCompleteMessage");
		this.isComplete = bool.Parse(json.GetItem("isComplete"));
		this.isAccept = bool.Parse(json.GetItem("isAccept"));
		this.isReject = bool.Parse(json.GetItem("isReject"));
	}
}

public class HuntQuest : Quest
{
	public List<String> MonsterNameList{get;set;}
	public List<int> MonsterCount{get;set;} = new List<int>();
	public List<int> TargetNum{get;set;} //퀘스트 목표 숫자
	
	public HuntQuest(){
	
	}
	
	public override void CheckComplete()
	{
		for(int i = 0;i<MonsterNameList.Count;i++)
		{
			if(MonsterCount[i] < TargetNum[i])
			{
				return;
			}
		}
		isComplete = true;
	}
	
	public override bool CheckTarget(Object info)
	{
		Enemy enemy = null;
		if(info is Enemy)
		{
			enemy = (Enemy)info;
		}
		else
		{
			return false;
		}
		if(MonsterNameList.Contains(enemy.Name))
		{
			try{
				MonsterCount[MonsterNameList.IndexOf(enemy.Name)]++;
			}catch(Exception e){
				MonsterCount.Add(0);
				MonsterCount[MonsterNameList.IndexOf(enemy.Name)]++;
			}
			CheckComplete();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	
	
	protected HuntQuest(HuntQuest that) : base(that){
		this.MonsterNameList = (List<String>)ListClone<String>(that.MonsterNameList);
		this.MonsterCount = new List<int>(that.MonsterCount);
		this.TargetNum = new List<int>(that.TargetNum);
	}
	
	public Object Clone(){
		return new HuntQuest(this);
	}
	
	public override String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Quest");
		json.AddList<String>("MonsterNameList",MonsterNameList,true);
		json.AddList<int>("MonsterCount",MonsterCount,true);
		json.AddList<int>("TargetNum",TargetNum,true);
		json.AddItem("QuestName",QuestName);
		json.AddItem("QuestContentsName",QuestContentsName);
		json.AddJsonAbleList("QuestReward",QuestReward,true);
		json.OpenObject("QuestCompleteMessage");
		json.AddJsonAbleObject(QuestCompleteMessage);
		json.CloseObject(true);
		json.AddItem("isComplete",isComplete);
		json.AddItem("isAccept",isAccept);
		json.AddItem("isReject",isReject);
		json.CloseObject();
		
		return json.JsonString;
	}
	public override void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.MonsterNameList = json.GetStringList("MonsterNameList");
		this.MonsterCount = json.GetIntList("MonsterCount");
		this.TargetNum = json.GetIntList("TargetNum");
		
		this.QuestName = json.GetItem("QuestName");
		this.QuestContentsName = json.GetItem("QuestContentsName");
		this.QuestReward = json.GetJsonAbleList<Reward>("QuestReward");
		this.QuestCompleteMessage = json.GetJsonAbleObject<TextAndPosition>("QuestCompleteMessage");
		this.isComplete = bool.Parse(json.GetItem("isComplete"));
		this.isAccept = bool.Parse(json.GetItem("isAccept"));
		this.isReject = bool.Parse(json.GetItem("isReject"));
	}
}

public class VisitQuest : Quest{
	public List<bool> bClearTarget = new List<bool>();
	private List<String> targetName;
	public List<String> TargetName{
		get{
			return targetName;
		}
		set{
			targetName = value;
			bClearTarget = Enumerable.Repeat(false,targetName.Count).ToList();
		}
	}
	
	public VisitQuest(){
	}
	
	public override void CheckComplete()
	{
		if(bClearTarget.All(trueOrFalse => trueOrFalse==true))
		{
			isComplete = true;
		}
	}
	
	public override bool CheckTarget(Object info){
		
		String choiceName = "";
		if(info is String)
		{
			choiceName = (String)info;
		}
		else
		{
			return false;
		}
		if(TargetName.Contains(choiceName))
		{
			bClearTarget[TargetName.IndexOf(choiceName)] = true;
			CheckComplete();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	protected VisitQuest(VisitQuest that) : base(that){
		bClearTarget = new List<bool>(that.bClearTarget);
		this.TargetName = new List<String>(that.TargetName);
	}
	
	public Object Clone(){
		return new VisitQuest(this);
	}
}

public class CollectionQuest : Quest{
	public List<Item> ItemList{get;set;}
	public List<int> ItemAmount{get;set;}
	private List<int> targetAmount;
	public List<int> TargetAmount{
		get{
			return targetAmount;
		}
		set{
			targetAmount = value;
			ItemAmount = Enumerable.Repeat(0,targetAmount.Count).ToList();
		}
	}
	
	public CollectionQuest(){
	
	}
	
	public override void TakeRewardAll(){
		ItemPayment();
		for(int i = 0;i<QuestReward.Count;i++){
			QuestReward[i].TakeReward();
		}
	}

	public override void CheckComplete()
	{
		for(int i = 0;i<ItemAmount.Count;i++){
			if(ItemAmount[i] < TargetAmount[i]){
				return;
			}
		}
		isComplete = true;
	}
	
	public override bool CheckTarget(Object info){ //플레이어
		List<Item> playerInventoryList = player.inven.InventoryList;
		for(int i = 0;i<ItemList.Count;i++)
		{
			if(playerInventoryList.Contains(ItemList[i]))
			{
				if(ItemList[i].IsStackable)
				{
					ItemAmount[ItemList.IndexOf(ItemList[i])] = playerInventoryList.Find(obj => obj.Name == ItemList[i].Name).Amount; //////// Predicate나 Contains, IEquatable 참고 후 수정12.28
				}
				else
				{
					ItemAmount[ItemList.IndexOf(ItemList[i])] = 1;
				}
				CheckComplete();
				return true;
			}
		}
		return false;
	}
	
	public void ItemPayment(){
		if(!isComplete){
			testLog("이 CollectionQuest는 완료되지 않았습니다.");
			return;
		}
		List<Item> playerInventoryList = player.inven.InventoryList;
		
		for(int i = 0;i<ItemList.Count;i++){
			Item tItem = playerInventoryList.Find(x => x.Name == ItemList[i].Name);
			if(tItem.IsStackable){
				tItem.Amount -= TargetAmount[i];
				if(tItem.Amount <= 0){
					playerInventoryList.Remove(tItem);
				}
			}else{
				playerInventoryList.Remove(tItem);
			}
		}
		
	}
	
	protected CollectionQuest(CollectionQuest that){
		this.ItemList = (List<Item>)ListClone<Item>(that.ItemList);
		this.ItemAmount = new List<int>(that.ItemAmount);
		this.TargetAmount = new List<int>(that.ItemAmount);
	}
	
	public Object Clone(){
		return new CollectionQuest(this);
	}
}

public class MeetingQuest:Quest{
	private List<String> meetingTarget;
	public List<bool> HasMeet{get;set;}
	
	public List<String> MeetingTarget{
		get{
			return meetingTarget;
		}
		set{
			meetingTarget = value;
			HasMeet = Enumerable.Repeat(false,meetingTarget.Count).ToList();
		}
	}
	
	public MeetingQuest(){
	
	}
	
	public override void CheckComplete()
	{
		for(int i = 0;i<HasMeet.Count;i++){
			if(!HasMeet[i]){
				return;
			}
		}
		isComplete = true;
	}
	
	public override bool CheckTarget(Object info){
		NPC npc = null;
		if(info is NPC)
		{
			npc = (NPC)info;
		}
		else
		{
			return false;
		}
		for(int i = 0;i<MeetingTarget.Count;i++)
		{
			if(MeetingTarget[i] == npc.Name)
			{
				HasMeet[i] = true;
			}
		}
		CheckComplete();
		return false;
	}
	
	protected MeetingQuest(MeetingQuest that) : base(that){
		this.MeetingTarget = new List<String>(that.MeetingTarget);
		this.HasMeet = new List<bool>(that.HasMeet);
	}
	
	public Object Clone(){
		return new MeetingQuest(this);
	}
}

public class Reward : ICloneable,ISaveToJson{ //퀘스트의 보상
	public String Name{get;set;}
	
	public Reward(){
	
	}
	
	public virtual void TakeReward(){
		
	}
	
	protected Reward(Reward that){
		
	}
	
	public Object Clone(){
		return new Reward(this);
	}
	public virtual String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Reward");
		json.AddItem("Name",Name);
		json.CloseObject();
		
		return json.JsonString;
	}
	public virtual void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.Name = json.GetItem("Name");
	}
	
}

public class ItemReward : Reward{
	public List<Item> Items{get;set;}
	
	public ItemReward(){
	
	}
	
	public override void TakeReward(){
		Random random = new Random();
		for(int i = 0;i<Items.Count;i++){
			player.inven.AddItem(Items[i]);
			int rx = random.Next(20,100);
			int ry = random.Next(2,13);
			AlertWindow(Items[i].Name+" 획득!",windowXPos:rx,windowYPos:ry, textXPos:0,textYPos:9,background:"getItem",delay:10,color:ConsoleColor.DarkYellow);
		}
	}
	
	public override String ToJsonString(){
		Json json = new Json();
		json.OpenObject("ItemReward");
		json.AddItem("Name",Name);
		json.AddJsonAbleList("Items",Items,true);
		json.CloseObject();
		
		return json.JsonString;
	}
	
	public override void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.Name = json.GetItem("Name");
		this.Items = json.GetJsonAbleList<Item>("Items");
	}
}

public class EventReward : Reward{
	public Action ChangeWorld{get;set;}
	
	public EventReward(){
	
	}
	
	public override void TakeReward(){
		ChangeWorld();
	}
}

public class EXPReward : Reward{
}


public class QuestData{
	public List<Quest> QuestDatas{get;set;} = new List<Quest>();
	public Backgrounds backgrounds = new Backgrounds();
	
	public QuestData(){
		QuestDatas.Add(new HuntQuest(){
			QuestName = "슬라임 사냥",
			QuestContentsName = "SlimeHuntQuest",
			//QuestContents = PlayData.WorldMap.GetChoice("SlimeHuntQuest"),
			QuestReward = new List<Reward>
			{
				new ItemReward()
				{
					Items = new List<Item>(){
					itemList.GetItem("전설의검"),
					new Item(){Name = "금반지"},
					new Item(){Name = "마을 공로 훈장"}
					}
				}
			},
			MonsterNameList = new List<String>(){
				"슬라임"
			},
			TargetNum = new List<int>(){
				1
			},
			QuestCompleteMessage = new TextAndPosition("오! 역시 슬라임 정도는 아무것도 아닌건가? 고맙네.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
		});
		
		QuestDatas.Add(new HuntQuest(){
			QuestName = "망자 사냥",
			QuestContentsName = "DeadManHuntQuest",
			//QuestContents = PlayData.WorldMap.GetChoice("DeadManHuntQuest"),
			QuestReward = new List<Reward>
			{
				new ItemReward()
				{
					Items = new List<Item>(){itemList.GetItem("황금갑옷")}
				}
			},
			MonsterNameList = new List<String>(){
				"뒤틀린 망자"
			},
			TargetNum = new List<int>(){
				1
			},
			QuestCompleteMessage = new TextAndPosition("망자를 이리 쉽게 처리하다니. 대단하군!",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
		});
		
		QuestDatas.Add(new VisitQuest(){
			QuestName = "폐가 방문",
			QuestContentsName = "VisitAbandonedHouse",
			//QuestContents = PlayData.WorldMap.GetChoice("VisitAbandonedHouse"),
			QuestReward = new List<Reward>
			{
				new ItemReward()
				{
					Items = new List<Item>(){new Item(){Name = "지하실열쇠"}}
				}
			},
			TargetName = new List<String>(){
				"gyeongminsHouse_Room"
			},
			QuestCompleteMessage = new TextAndPosition("날아다니는 냉장고가 습격했다고?\n도데체 무슨 소릴 하는건가?\n흐흠..아무튼 보상은 주겠네..",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
		});
		
		QuestDatas.Add(new CollectionQuest(){
			QuestName = "피부에 좋은 슬라임 젤",
			QuestContentsName = "GetSlimeJell",
			//QuestContents = PlayData.WorldMap.GetChoice("GetSlimeJell"),
			QuestReward = new List<Reward>
			{
				new ItemReward()
				{
					Items = new List<Item>(){itemList.GetItem("촌장의 일기")}
				}
			},
			ItemList = new List<Item>(){
				itemList.GetItem("슬라임 젤")
			},
			TargetAmount = new List<int>(){
				3
			},
			QuestCompleteMessage = new TextAndPosition("오오 고맙구먼. 아내가 좋아하겠어!",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
		});
		
		QuestDatas.Add(new MeetingQuest(){
			QuestName = "testMeeting",
			QuestContentsName = "testMeeting",
			//QuestContents = new ChoiceControler(new Scenario()).GetChoice("testMeeting"),
			QuestReward = new List<Reward>
			{
				new ItemReward()
				{
					Items = new List<Item>(){new Item(){Name = "보상"}}
				}
			},
			MeetingTarget = new List<String>(){
				"촌장"
			},
			QuestCompleteMessage = new TextAndPosition("그래 나일세.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
		});
	}
	
	public String QuestListToJsonString(){
		Json json = new Json();
		json.AddJsonAbleList<Quest>("QuestList",QuestDatas);
		return json.JsonString;
	}
}