using System;
using System.Collections.Generic;
using Characters;
//using static Characters.User;
//using static Game.GameManager;
//using Game;
using System.Threading;
//using System.Threading.Tasks;
//using System.Linq;
//using System.IO;
//using static BattleSystem;
using static Convenience;
//using static ItemData;
using static PlayData;

public static class QuestControler{ //퀘스트목록을 관리
	public static List<Quest> AllQuestList{get;set;}//모든 퀘스트 목록
	public static List<Quest> AcceptQuestList{get;set;}//수락한 퀘스트 목록
	public static List<Quest> CompleteQuestList{get;set;}//
	
	public static bool QuestComplete(Quest quest){
		return false;
	}
}

public class Quest : ICloneable{ //퀘스트 객체, 이걸 NPC와 Player끼리 주고 받으면서 퀘스트를 주거나 완료한다.
	public String QuestName = "UKNOWN"; //퀘스트 고유 이름
	public Choice questContents; //퀘스트 내용
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
	}
	
	public Object Clone(){
		return new Quest(this);
	}
}

public class HuntQuest : Quest{
	public List<String> MonsterNameList{get;set;}
	public int MonsterCount{get;set;}
	public int TargetNum{get;set;} //퀘스트 목표 숫자
	
	public override void CheckComplete(){
		if(MonsterCount >= TargetNum){
			isComplete = true;
		}
	}
	
	public override bool CheckTarget(Object info){
		Enemy enemy = null;
		if(info is Enemy){
			enemy = (Enemy)info;
		}
		if(MonsterNameList.Contains(enemy.Name)){
			MonsterCount++;
			CheckComplete();
			return true;
		}
		else{
			return false;
		}
	}
}

public class Reward : ICloneable{ //퀘스트의 보상
	
	protected Reward(Reward that){
		
	}
	
	public Object Clone(){
		return new Reward(this);
	}
}

