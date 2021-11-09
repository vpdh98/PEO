using System;
using Characters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Convenience;
using static DamageSystem;
using static GameWindows;

//namespace Battle{

public class AttackInfo{
	int final_damage;
	public int Final_damage
	{
		get
		{
			return final_damage;
		}
		set
		{
			if(value <= 0){
				final_damage = 1;
			}else{
				final_damage = value;
			}
		}
	}

	public AttackInfo()
	{
		Final_damage = 0;
	}

	public void CalDamage(int defence){
		Final_damage -= defence;
	}
}

public enum ActionType{
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
	public static int count = 0;
	public static int limit = 5;
	public static bool timeOut = false;
	public static Task timer;
	
	public static String backField;
	public static bool battleAnd = false;
	public static bool turnAnd = false;
	
	public static Backgrounds backgrounds = new Backgrounds();
	public static ConsoleKeyInfo c;
	
	static int DODGE_CHANCE = 50;
	static int BLOCK_CHANCE = 50;
	static Random random = new Random();
	
	
	public static DisplayTextGame BDTG = new DisplayTextGame();
	public static ChoiceControler BCC = new ChoiceControler(new Scenario());
	public static String currentChoice = "firstPhase"; //첫 화면


	public static void Attacking(Character Attacker,Character Defender){
		Defender.Damage(Attacker.Attack());
	}
	
	public static void Blocking(Character Attacker,Character Defender){
		double defenderDefense = Defender.Defense;
		double attackerAttackPower = Attacker.AttackPower;
		
		double blockChance = BLOCK_CHANCE * (defenderDefense / attackerAttackPower);
		testLog(blockChance,false);
		if(random.Next(1,101) < (int)blockChance){
			successCheck = true;
		}
		else{
			Defender.Damage(Attacker.Attack());
			successCheck = false;
		}
	}
	
	public static void Dodgeing(Character Attacker,Character Defender){
		double defenderSpeed = Defender.AttackSpeed;
		double attackerSpeed = Attacker.AttackSpeed;
		
		double dodgeChance = DODGE_CHANCE * (defenderSpeed / attackerSpeed);
		testLog(dodgeChance,false);
		if(random.Next(1,101) < (int)dodgeChance){
			successCheck = true;
		}
		else{
			Defender.Damage(Attacker.Attack());
			successCheck = false;
		}
	}
}

public static class BattleSystem{
		public static void BattleCal(){
			while(true){
				if(Attacker!=null&&Defender!=null){
					if(actionType == ActionType.ATTACK){
						Attacking(Attacker,Defender);
					}
					if(actionType == ActionType.BLOCK){
						Blocking(Defender,Attacker);
					}
					if(actionType == ActionType.DODGE){
						Dodgeing(Defender,Attacker);
					}
					Attacker=null;
					Defender=null;
				}
				Thread.Sleep(10);
			}
		}

	public static String BattlePhase(Player player,Monster monster,String back){
		
		globerPlayer = player;
		globerMonster = monster;
		backField = back;
		currentChoice = "firstPhase";
		battleAnd = false;
		timerStart = false;
		Choice cho = BCC.GetChoice("movePhase");
			cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
				{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}};
			cho = BCC.GetChoice("firstPhase");
				cho.OnlyShowText = new List<TextAndPosition>() 
					{new TextAndPosition(globerMonster.GetRandomSpawnMessage().text,15,3+5,1){AlignH = true}};
		currentChoice = BDTG.ChoicePick(BDTG,BCC,currentChoice);
		BDTG.delay = 0;
		
		while(!battleAnd)
		{
			
			if(!timerStart){
				timer = Task.Run(()=>BattleTimer());
				timerStart = true;
			}
			currentChoice = BDTG.ChoicePick(BDTG,BCC,currentChoice);
			if(!timeOut)
			{
				testLog("Player Turn");
				timerEnd = true;
				timer.Wait();
				PlayerTurn();
			}
			else
			{
				testLog("Monster Turn");
				MonsterTurn();	
			}

			if(globerMonster.HpState() == 3){ //8.22 몬스터의 HP상태가 빈사 상태일때 배틀 페이즈 종료 const int Died = 3

				cho = BCC.GetChoiceClone("andPhase"); //BDTG의 Cho를 초기화 하면서 OnlyShowText에 있던 텍스트는 integratedList에 들어감으로 choice에 넣기전에 수정해 줘야함
				cho.OnlyShowText = new List<TextAndPosition>() //몬스터 상태메세지 초기화
						{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}};
				BDTG.Display(cho);
				c = Console.ReadKey();
				return backField;
			}
			
		}
		return backField;
	}
	public static void PlayerTurn(){
		turnAnd = false;
		while(!turnAnd){
			if(died)
			{
				backField = "testStream";
				battleAnd = true;
				return;
			}
			if(currentChoice == "movePhase"){
				turnAnd = true;
			}
				if(currentChoice == "attackPhase"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.ATTACK;
					Choice cho = BCC.GetChoice(currentChoice);
							cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
								{new TextAndPosition(globerPlayer.AttackCry(),15,9,1){AlignH = true}};
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
						Choice cho = BCC.GetChoice(currentChoice);
							cho.OnlyShowText = new List<TextAndPosition>() 
								{new TextAndPosition(globerMonster.BlockSuccess(),15,9,1){AlignH = true}};
					}else{
						currentChoice = "failBlock";
						Choice cho = BCC.GetChoice(currentChoice);
							cho.OnlyShowText = new List<TextAndPosition>() 
								{new TextAndPosition(globerMonster.BlockFail(),15,9,1){AlignH = true}};
					}
				}
				else if(currentChoice == "dodge"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
					actionType = ActionType.DODGE;
					Attacker = globerPlayer;
					Defender = globerMonster;
					if(successCheck){
						currentChoice = "successDodge";
						Choice cho = BCC.GetChoice(currentChoice);
							cho.OnlyShowText = new List<TextAndPosition>() 
								{new TextAndPosition(globerMonster.DodgeSuccess(),15,9,1){AlignH = true}};
					}else{
						currentChoice = "failDodge";
						Choice cho = BCC.GetChoice(currentChoice);
							cho.OnlyShowText = new List<TextAndPosition>() 
								{new TextAndPosition(globerMonster.DodgeFail(),15,9,1){AlignH = true}};
					}
				}	
		}
	}
	
	
	public static void BattleReaction(){
		currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
		Choice cho;
		if(currentChoice == "reactionPhase"){ //8.22
			cho = BCC.GetChoice(currentChoice);
			cho.OnlyShowText = new List<TextAndPosition>() 
				{new TextAndPosition(globerMonster.Reaction(),15,9,1){AlignH = true}};
			cho = BCC.GetChoice("movePhase");
			cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
				{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}};
		}
		BDTG.Display(BCC.GetChoiceClone(currentChoice));
		c = Console.ReadKey(); //8.24
		currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
		turnAnd = true;
	}
	
	
	
	public static void MonsterTurn(){
		timeOut = false;
		Choice cho = BCC.GetChoice("monsterAttack");
		cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
			{new TextAndPosition(globerMonster.AttackCry(),15,3+5,1){AlignH = true}};
		String currentChoice = "monsterAttack"; //첫 화면
			BDTG.Display(BCC.GetChoiceClone(currentChoice));
			Attacking(globerMonster,globerPlayer);
			currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
			Console.ReadKey();
				
			cho = BCC.GetChoice(currentChoice);
			cho.OnlyShowText = new List<TextAndPosition>() 
				{new TextAndPosition(globerPlayer.Reaction(),15,9,1){AlignH = true}};
				
			BDTG.Display(BCC.GetChoiceClone(currentChoice));;
			currentChoice = "movePhase";
			Console.ReadKey();
			if(globerPlayer.Hp <= 0) died = true;
		
		
	}
	
	public static void BattleTimer(){
		testLog("Timer 1",false);
				for(count = 0;count<limit;count++){
					if(timerEnd){timerStart = false;testLog("Timer 2",false); return;} 
					Thread.Sleep(1000);
				}
		testLog("Timer 3",false);
				timeOut = true;
		timerStart = false;
	}
}
//}