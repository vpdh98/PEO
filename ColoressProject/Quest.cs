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

public static class QuestControler{ //퀘스트목록을 관리
	public static List<Quest> AllQuestList{get;set;}//모든 퀘스트 목록
	public static List<Quest> AcceptQuestList{get;set;}//수락한 퀘스트 목록
	public static List<Quest> CompleteQuestList{get;set;}//완료된 퀘스트 목록
	
	public static Quest GetQuestByName(String name){
		IsEmptyList<Quest>(AllQuestList); //////////////////////////12.29 객체 초기화가 안되있음
		return AllQuestList.Find(q => q.QuestName.Equals(name));
	}
	
	public static bool QuestComplete(Quest quest){
		return false;
	}
}

public class Quest : ICloneable{ //퀘스트 객체, 이걸 NPC와 Player끼리 주고 받으면서 퀘스트를 주거나 완료한다.
	public String QuestName{get;set;} = "UKNOWN"; //퀘스트 고유 이름
	public Choice QuestContents{get;set;} //퀘스트 내용
	public List<Reward> QuestReward{get;set;}  //퀘스트 보상,여러개일 수 있다.
	public bool isComplete = false; //퀘스트 완료 여부
	public bool isAccept = false; //퀘스트 수락 여부
	
	public Quest(){
		
	}
	
	public virtual void CheckComplete(){ //완료 여부 확인
	}
	
	public virtual bool CheckTarget(Object info){//퀘스트 목표가 맞는지 확인
		return false;
	}
	
	protected Quest(Quest that){
		this.QuestReward = (List<Reward>)ListClone<Reward>(that.QuestReward);
		this.QuestName = that.QuestName;
		this.QuestContents = that.QuestContents;
		this.isComplete = that.isComplete;
		this.isAccept = that.isAccept;
	}
	
	public Object Clone(){
		return new Quest(this);
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
		for(int i = 0;i<MonsterCount.Count;i++)
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
			MonsterCount[MonsterNameList.IndexOf(enemy.Name)]++;
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
}

public class VisitQuest : Quest{
	public List<bool> bClearTarget;
	public List<String> TargetName{get;set;}
	
	public override void CheckComplete()
	{
		if(bClearTarget.All(trueOrFalse => trueOrFalse==true))
		{
			isComplete = true;
		}
	}
	
	public override bool CheckTarget(Object info){
		Choice choice = null;
		if(info is Choice)
		{
			choice = (Choice)info;
		}
		else
		{
			return false;
		}
		if(TargetName.Contains(choice.Name))
		{
			bClearTarget[TargetName.IndexOf(choice.Name)] = true;
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

public class ConllectionQuest : Quest{
	public List<Item> ItemList{get;set;}
	public List<int> ItemAmount{get;set;}
	public List<int> TargetAmount{get;set;}

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
	
	protected ConllectionQuest(ConllectionQuest that){
		this.ItemList = (List<Item>)ListClone<Item>(that.ItemList);
		this.ItemAmount = new List<int>(that.ItemAmount);
		this.TargetAmount = new List<int>(that.ItemAmount);
	}
	
	public Object Clone(){
		return new ConllectionQuest(this);
	}
}

public class MeetingQuest:Quest{
	public List<String> MeetingTarget{get;set;}
	public List<bool> HasMeet{get;set;}
	
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

public class Reward : ICloneable{ //퀘스트의 보상
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
}

public class ItemReward : Reward{
	public List<Item> Items{get;set;}
	
	public ItemReward(){
	
	}
	
	public override void TakeReward(){
		for(int i = 0;i<Items.Count;i++){
			player.inven.AddItem(Items[i]);
		}
	}
}

public class EventReward : Reward{
	public Define.Event ChangeWorld{get;set;}
	
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
			QuestContents = new Choice(){
				Name = "SlimeHuntQuest",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("수락한다.",16,13,true),
							new TextAndPosition("거절한다.",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("우리 마을에 요즘 슬라임이 많아져\n 골치를 앓고 있네..\n자네가 좀 도와줄 수 있나?\n 슬라임 10마리만 잡아주면 되네.",15,3,10){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"QuestAccept"},{1,"QuestReject"}},
				BackgroundText = backgrounds.GetBackground(1)
			},
			QuestReward = new List<Reward>
			{
				new ItemReward()
				{
					Items = new List<Item>(){itemList.GetItem("전설의검")}
				}
			},
			MonsterNameList = new List<String>(){
				"슬라임"
			},
			TargetNum = new List<int>(){
				10
			}
		});
	}
}