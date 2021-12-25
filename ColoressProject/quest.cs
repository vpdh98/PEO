using System;
using System.Collections.Generic;
//using Characters;
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

public static class QuestControler{ //진행중인 퀘스트목록을 관리
	public static List<Quest> AcceptQuestList{get;set;}
	
	public static bool QuestComplete(Quest quest){
		return false;
	}
}

public class Quest{ //퀘스트 객체, 이걸 NPC와 Player끼리 주고 받으면서 퀘스트를 주거나 완료한다.
	public Choice questContents;
	public Reward questReward; 
}

public class Reward{ //퀘스트의 보상
	
}

