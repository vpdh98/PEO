using System;
using Characters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Convenience;
using static DamageSystem;
using static GameWindows;

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
	
	public static Monster globerMonster;
	public static Player globerPlayer;
	public static bool successCheck = false;
	public static String reactionMessage;
	public static bool died = false;
	
	public static bool timerStart = false;
	public static bool timerEnd = false;
	public static double count = 0;
	public static int limit = 3;
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
	public static String currentChoice = "firstPhase"; //첫 화면


	public static void Attacking(Character Attacker,Character Defender)
	{
		Defender.Damage(Attacker.Attack());
	}
	
	public static void Blocking(Character Attacker,Character Defender)
	{
		double defenderDefense = Defender.Defense;
		double attackerAttackPower = Attacker.AttackPower;
		
		double blockChance = BLOCK_CHANCE * (defenderDefense / attackerAttackPower);
		testLog(blockChance);
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
		
		double dodgeChance = DODGE_CHANCE * (defenderSpeed / attackerSpeed);
		testLog(dodgeChance,false);
		if(random.Next(1,101) < (int)dodgeChance)
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

	public static String BattlePhase(Player player,Monster monster,String back)
	{
		globerPlayer = player;
		globerMonster = monster;
		
		globerPlayer.ReactionMessage = globerMonster.PlayerReactionMessage;
		
		backField = back;
		currentChoice = "firstPhase";
		battleAnd = false;
		timerStart = false;
		BCC.ChangeChoiceText(choiceName:"movePhase",onlyShowText:new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true});
		BCC.ChangeChoiceText(choiceName:"firstPhase",onlyShowText:new TextAndPosition(globerMonster.GetRandomSpawnMessage().text,15,3+5,1){AlignH = true});
		//Task.Run(()=>DisplayTimer());
		
		BDTG.Display(BCC.GetChoiceClone(currentChoice));
		keyInfo = Console.ReadKey();

		while(!battleAnd)
		{
			
			BDTG.SelectingText(keyInfo);
			if(keyInfo.Key == ConsoleKey.Enter) //Enter
			{
				
				currentChoice = (String)BDTG.Cho.GetValueOn(BDTG.currentSelectNum);

				if(currentChoice == "end") return backField;
				
				if(currentChoice == "movePhase"){ 
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					TimerStart();
					keyInfo = Console.ReadKey();
				}
				else if(!timeOut)
				{
					//testLog("Player Turn");
					timerEnd = true;
					timer.Wait();
					PlayerTurn();
				}
				else
				{
					//testLog("Monster Turn");
					//timer.Wait();
					MonsterTurn();	
				}
				
				if(died)
				{
					return "testStream";
				}
			}
			else
			{
				BDTG.Display(); //Display의 Init()과 choice초기화가 없는 버전
				keyInfo = Console.ReadKey();
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
					Defender = globerMonster;
					
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					Console.ReadKey();
					BattleReaction();
				}
				else if(currentChoice == "block"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.BLOCK;
					Attacker = globerPlayer;
					Defender = globerMonster;
					if(successCheck){
						currentChoice = "successBlock";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerMonster.BlockSuccess(),15,9,1){AlignH = true});
					}else{
						currentChoice = "failBlock";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerMonster.BlockFail(),15,9,1){AlignH = true});
					}
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					keyInfo = Console.ReadKey();
					turnAnd = true;
					TimerStart();
				}
				else if(currentChoice == "dodge"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.DODGE;
					Attacker = globerPlayer;
					Defender = globerMonster;
					if(successCheck){
						currentChoice = "successDodge";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerMonster.DodgeSuccess(),15,9,1){AlignH = true});
					}else{
						currentChoice = "failDodge";
						BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerMonster.DodgeFail(),15,9,1){AlignH = true});
					}
					BDTG.Display(BCC.GetChoiceClone(currentChoice));
					keyInfo = Console.ReadKey();
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
			BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerMonster.Reaction(),15,9,1){AlignH = true});
			BCC.ChangeChoiceText(choiceName:"movePhase",onlyShowText:new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true});
		}
		//12.07 공격 파워에 따른 메세지를 최대체력 비례에서 현재체력 비례로 변경함에 따라 몬스터에게 데미지가 들어가는 타이밍을 몬스터Reaction메세지를 받은 후로 바꾸려다가... 말음
		//대신 이전 현재체력을 나타내는 것 변수 추가
		
		
		BDTG.Display(BCC.GetChoiceClone(currentChoice));
		keyInfo = Console.ReadKey();
		if(globerMonster.HpState() == 3){ //8.22 몬스터의 HP상태가 빈사 상태일때 배틀 페이즈 종료 const int Died = 3
			BCC.ChangeChoiceText(choiceName:"andPhase",onlyShowText:new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true});
			BDTG.Display(BCC.GetChoiceClone("andPhase"));
			keyInfo = Console.ReadKey();
			List<Item> drops = globerMonster.ItemDrop();
			foreach(Item i in drops){
				globerPlayer.inven.AddItem(i);
				AlertWindow(i.Name+" 획득!",textXPos:25,textYPos:5,background:5);
			}
			battleAnd = true;
			turnAnd = true;
		}
		else{
			currentChoice = "movePhase";
			
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			TimerStart();
			keyInfo = Console.ReadKey();
			turnAnd = true;
		}
	}
	
	
	
	public static void MonsterTurn(){
		timeOut = false;
		BCC.ChangeChoiceText(choiceName:"monsterPreAttack",onlyShowText:new TextAndPosition(globerMonster.PreAttackSymptom(),15,3+5,1){AlignH = true});
		BCC.ChangeChoiceText(choiceName:"monsterAttack",onlyShowText:new TextAndPosition(globerMonster.AttackCry(),15,3+5,1){AlignH = true});
		String currentChoice = "monsterPreAttack"; //첫 화면
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
			Console.ReadKey();
		
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			Attacking(globerMonster,globerPlayer);
			currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
			Console.ReadKey();
				
			BCC.ChangeChoiceText(choiceName:currentChoice,onlyShowText:new TextAndPosition(globerPlayer.Reaction(),15,9,1){AlignH = true});
				
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			currentChoice = "movePhase";
			Console.ReadKey();
			if(globerPlayer.Hp <= 0){ 
				testLog("in die");
				died = true;
				return;
			}
		
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			TimerStart();
			keyInfo = Console.ReadKey();
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