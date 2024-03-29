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

//namespace Battle{

public class AttackInfo
{
	int finalDamage;
	public int FinalDamage
	{
		get
		{
			return finalDamage;
		}
		set
		{
			if(value <= 0)
			{
				finalDamage = 1;
			}
			else
			{
				finalDamage = value;
			}
		}
	}
	
	public int HpBeforeAttack{get;set;}
	

	public AttackInfo()
	{
		FinalDamage = 0;
		HpBeforeAttack = 0;
	}

	public void CalDamage(int defence)
	{
		FinalDamage -= defence;
	}
}

public enum ActionType
{
	ATTACK,
	BLOCK,
	DODGE
}

public static class DamageSystem
{
	public static Character Attacker;
	public static Character Defender;
	public static ActionType actionType;
	
	public static Enemy globerEnemy;
	public static Player globerPlayer;
	public static bool successCheck = false;
	public static String reactionMessage;
	public static bool died = false;
	
	public static bool timerStart = false;
	public static bool timerEnd = false;
	public static double count = 0;
	public static double limit = 3;
	public static bool timeOut = false;
	public static Task timer;
	
	public static String backField;
	public static bool battleAnd = false;
	public static bool turnAnd = false;
	
	public static Backgrounds backgrounds = new Backgrounds();
	public static ConsoleKeyInfo keyInfo;
	
	static int DODGE_CHANCE = 50;
	static int BLOCK_CHANCE = 50;
	static Random random = new Random();
	
	
	public static DisplayTextGame BDTG = new DisplayTextGame();
	public static ChoiceControler BCC = new ChoiceControler(new Scenario());
	


	public static void Attacking(Character Attacker,Character Defender)
	{
		Defender.Damage(Attacker.Attack());
	}
	
	public static void Blocking(Character Attacker,Character Defender)
	{
		double defenderDefense = Defender.Defense;
		double attackerAttackPower = Attacker.AttackPower;
		
		double blockChance = BLOCK_CHANCE * (defenderDefense / attackerAttackPower);
		//testLog(blockChance);
		if(random.Next(1,101) < (int)blockChance)
		{
			successCheck = true;
		}
		else
		{
			Defender.Damage(Attacker.Attack());
			successCheck = false;
		}
	}
	
	public static void Dodgeing(Character Attacker,Character Defender)
	{
		double defenderSpeed = Defender.AttackSpeed;
		double attackerSpeed = Attacker.AttackSpeed;
		if(attackerSpeed == 0 && defenderSpeed > 0){
			successCheck = true;
			return;
		}
		if(defenderSpeed == 0 && attackerSpeed > 0){
			successCheck = false;
			return;
		}
		
		double dodgeChance = DODGE_CHANCE * (defenderSpeed / attackerSpeed);
		if(random.Next(100) < (int)dodgeChance)
		{
			successCheck = true;
		}
		else
		{
			Defender.Damage(Attacker.Attack());
			successCheck = false;
		}
	}
}

public static class BattleSystem{
	/*
		전투시 데미지 계산되는 알고리즘 설명
		
		BattleCal은 비동기로 항상 실행되는 함수로
		
		먼저 정적 변수인 actionType에 공격의 타입을 넣고(ATTACK,DODGE,BLOCK등)
		정적 변수인 Attacker,Defender에 공격자와 피격대상의 객체를 넣으면
		해당 타입의 공격 판정이나 데미지 계산 등을 실행하고 정적변수를 비움
		
		*각각의 객체는 IDamageable인터페이스를 구현한 클래스의 객체여야함
		
	*/
		public static void BattleCal()
		{
			while(true)
			{
				if(Attacker!=null&&Defender!=null)
				{
					if(actionType == ActionType.ATTACK)
					{
						Attacking(Attacker,Defender);
					}
					if(actionType == ActionType.BLOCK)
					{
						Blocking(Defender,Attacker);
					}
					if(actionType == ActionType.DODGE)
					{
						Dodgeing(Defender,Attacker);
					}
					Attacker=null;
					Defender=null;
				}
				Thread.Sleep(10);
			}
		}

	public static String BattlePhase(Player player,Enemy monster,String back)
	{
		globerPlayer = player;
		globerEnemy = monster;
		currentChoice = "firstPhase"; //첫 화면
		
		limit = 3*(1.0*globerPlayer.AttackSpeed / globerEnemy.AttackSpeed);
		
		globerPlayer.ReactionMessage = globerEnemy.PlayerReactionMessage;
		
		backField = back;
		currentChoice = "firstPhase";
		battleAnd = false;
		timerStart = false;
		BCC.ChangeChoiceText(choiceName:"movePhase",onlyShowText:new TextAndPosition(globerEnemy.CurrentState(),15,3+5,1){AlignH = true});
		BCC.ChangeChoiceText(choiceName:"firstPhase",onlyShowText:new TextAndPosition(globerEnemy.GetRandomSpawnMessage().text,15,3+5,1){AlignH = true});
		//Task.Run(()=>DisplayTimer());
		
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
			
			
		}
		return backField;
	}
	
	
	
	public static void PlayerTurn(){
		timerEnd = false;
		turnAnd = false;
		while(!turnAnd){
			if(currentChoice == "movePhase"){
				turnAnd = true;
			}
				if(currentChoice == "attackPhase"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.ATTACK;
					BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerPlayer.AttackCry(),15,9,1){AlignH = true});
					
					Attacker = globerPlayer;
					Defender = globerEnemy;
					Thread.Sleep(10);
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					Console.ReadKey();
					BattleReaction();
				}
				else if(currentChoice == "block"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.BLOCK;
					Attacker = globerPlayer;
					Defender = globerEnemy;
					Thread.Sleep(10);
					if(successCheck){
						currentChoice = "successBlock";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerEnemy.BlockSuccess(),15,9,1){AlignH = true});
					}else{
						currentChoice = "failBlock";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerEnemy.BlockFail(),15,9,1){AlignH = true});
					}
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					Console.ReadKey();
					
					if(currentChoice == "failBlock"){
						currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext(); //monsterReactionPhase
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerPlayer.Reaction(),15,9,1){AlignH = true}); 
						BDTG.Display(BCC.GetChoiceClone(currentChoice));
						Console.ReadKey();
					}
					currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext(); //movePhase
						BDTG.Display(BCC.GetChoiceClone(currentChoice));
					
					if(globerPlayer.Hp <= 0){ 
						//testLog("in die");
						died = true;
						timeOut = false;
						return;
					}
					turnAnd = true;
					TimerStart();
				}
				else if(currentChoice == "dodge"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.DODGE;
					Attacker = globerPlayer;
					Defender = globerEnemy;
					Thread.Sleep(10);
					if(successCheck){
						currentChoice = "successDodge";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerEnemy.DodgeSuccess(),15,9,1){AlignH = true});
					}else{
						currentChoice = "failDodge";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerEnemy.DodgeFail(),15,9,1){AlignH = true});
					}
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					Console.ReadKey();
					
					if(currentChoice == "failDodge"){
						currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();//monsterReactionPhase
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerPlayer.Reaction(),15,9,1){AlignH = true});
						BDTG.Display(BCC.GetChoiceClone(currentChoice));
						Console.ReadKey();
					}
					currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();//movePhase
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					
					if(globerPlayer.Hp <= 0){ 
						//testLog("in die");
						died = true;
						timeOut = false;
						return;
					}
					turnAnd = true;
					TimerStart();
				}	
			
		}
	}
	
	
	public static void BattleReaction(){
		Random random = new Random();
		currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
		Choice cho;
		if(currentChoice == "reactionPhase"){ //8.22
			BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerEnemy.Reaction(),15,9,1){AlignH = true});
			BCC.ChangeChoiceText(choiceName:"movePhase",onlyShowText:new TextAndPosition(globerEnemy.CurrentState(),15,3+5,1){AlignH = true});
		}
		//12.07 공격 파워에 따른 메세지를 최대체력 비례에서 현재체력 비례로 변경함에 따라 몬스터에게 데미지가 들어가는 타이밍을 몬스터Reaction메세지를 받은 후로 바꾸려다가... 말음
		//대신 이전 현재체력을 나타내는 것 변수 추가
		
		
		BDTG.Display(BCC.GetChoiceClone(currentChoice));
		keyInfo = Console.ReadKey();
		if(globerEnemy.HpState() == 3){ //8.22 몬스터의 HP상태가 빈사 상태일때 배틀 페이즈 종료 const int Died = 3
			BCC.ChangeChoiceText(choiceName:"andPhase",onlyShowText:new TextAndPosition(globerEnemy.CurrentState(),15,3+5,1){AlignH = true});
			BDTG.Display(BCC.GetChoiceClone("andPhase"));
			keyInfo = Console.ReadKey();
			
			globerPlayer.PlayerQuestCheck(globerEnemy);
			//testLog(((HuntQuest)globerPlayer.QuestList[0]).MonsterCount[0]);
			List<Item> drops = globerEnemy.ItemDrop();
			foreach(Item i in drops){
				globerPlayer.inven.AddItem(i);
				int rx = random.Next(20,100);
				int ry = random.Next(2,13);
				AlertWindow(i.Name+" 획득!",windowXPos:rx,windowYPos:ry, textXPos:0,textYPos:9,background:"getItem",delay:10,color:ConsoleColor.DarkYellow);
			}
			if(drops.Count > 0)
				Console.ReadKey();
			
			if(globerEnemy.IsSpawnOnce)
				GameManager.DespawnEnemy(globerEnemy.Name,PlayData.WorldMap.GetChoice(backField));
			
			if(globerEnemy.DeathEvent != null)
				PlayData.delegateList.ActionList[globerEnemy.DeathEvent]();		//여기서 backField에 접근하므로 DespawnEnemy가 먼저 실행되어야한다.
				
			battleAnd = true;
			turnAnd = true;
		}
		else{
			currentChoice = "movePhase";
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			TimerStart();
			turnAnd = true;
		}
	}
	
	
	
	public static void EnemyTurn(){
		timeOut = false;
		BCC.ChangeChoiceText(choiceName:"monsterPreAttack",onlyShowText:new TextAndPosition(globerEnemy.PreAttackSymptom(),15,3+5,1){AlignH = true});
		BCC.ChangeChoiceText(choiceName:"monsterAttack",onlyShowText:new TextAndPosition(globerEnemy.AttackCry(),15,3+5,1){AlignH = true});
		String currentChoice = "monsterPreAttack"; //첫 화면
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
			WaitTimeOrInput(150); //1500ms를 기다리던지 아님 입력이 들어오면 다음으로 진행
			//Console.ReadKey();
		
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			Attacking(globerEnemy,globerPlayer);
			currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
			WaitTimeOrInput(150);
			//Console.ReadKey();
				
			BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerPlayer.Reaction(),15,9,1){AlignH = true});
				
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			currentChoice = "movePhase";
			WaitTimeOrInput(150);
			//Console.ReadKey();
			if(globerPlayer.Hp <= 0){ 
				//testLog("in die");
				died = true;
				return;
			}
		
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			TimerStart();
	}
	
	public static void BattleTimer()
	{
//		testLog("Timer 1");
		for(count = 0;count<limit;count+=0.01){
			if(timerEnd)
			{
				timerStart = false;
				timerEnd = false;
	//			testLog("Timer 2"); 
				return;
			} 
				
			Thread.Sleep(10);
		}
	//	testLog("Timer 3");
		timeOut = true;
		timerStart = false;
	}
	
	public static void DisplayTimer(){
		while(true){
			Console.SetCursorPosition(0,0);
			Console.Write(count);
			Thread.Sleep(100);
		}
	}
	
	public static void TimerStart(){
		if(!timerStart){
				timer = Task.Run(()=>BattleTimer());
				timerStart = true;
			}
	}
}
//}