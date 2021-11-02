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
	public static bool battleTurn = false;
	public static Monster globerMonster;
	public static Player globerPlayer;
	public static bool successCheck = false;
	public static String reactionMessage;
	public static bool died = false;
	public static bool timerStart = false;
	public static String backField;
	public static bool battleAnd = false;
	
	
	static int DODGE_CHANCE = 50;
	static int BLOCK_CHANCE = 50;
	static Random random = new Random();

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

	public static async Task<String> BattlePhase(Player player,Monster monster,String back){
		globerPlayer = player;
		globerMonster = monster;
		backField = back;
		var playerTurn = Task.Run(()=>PlayerTurn());
		
		testLog("playerTurn끝11");
		await playerTurn;
		testLog("playerTurn끝");
		return backField;
	}
	
	public static void PlayerTurn(){
		
			
			
			

			Backgrounds backgrounds = new Backgrounds();
			Choice Start = new Choice(){
				Name = "firstPhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("공격한다.",16,13,true),
							new TextAndPosition("도망간다.",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(globerMonster.GetRandomSpawnMessage().text,15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"},{1,"end"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B2 = new Choice(){
				Name = "movePhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("공격",16,13,true),
							 new TextAndPosition("방어",28,13,true),
							new TextAndPosition("회피",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"attackPhase"},{1,"block"},{2,"dodge"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B3 = new Choice(){
				Name = "attackPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(globerMonster.Name+"베기!",5,10,10,ConsoleColor.Red){AlignH = true,PriorityLayer=1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"reactionPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B4 = new Choice(){
				Name = "reactionPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("(몬스터 피해 메세지)",5,10,10){AlignH = true,PriorityLayer=1}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B5 = new Choice(){
				Name = "andPhase",
				SelectText = new List<TextAndPosition>()         
							{new TextAndPosition("확인",16,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"backField"}},
				BackgroundText = backgrounds.GetBackground(1)
			};
			
			Choice B6 = new Choice(){
				Name = "failBlock",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("막기 실패",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};
			
			Choice B7 = new Choice(){
				Name = "successBlock",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("막기 성공",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};
			
			Choice B8 = new Choice(){
				Name = "failDodge",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("회피 실패",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};
			
			Choice B9 = new Choice(){
				Name = "successDodge",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("회피 성공",15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};
			
			

			//Console.WriteLine(monster.GetRandomSpawnMessage().text);
			DisplayTextGame BDTG = new DisplayTextGame();
			ChoiceControler BCC = new ChoiceControler();
			String currentChoice = "firstPhase"; //첫 화면

			BCC.AddChoice(Start);
			BCC.AddChoice(B2);
			BCC.AddChoice(B3);
			BCC.AddChoice(B4);
			BCC.AddChoice(B5);
			BCC.AddChoice(B6);
			BCC.AddChoice(B7);
			BCC.AddChoice(B8);
			BCC.AddChoice(B9);
				while(!battleAnd){
				BDTG.Cho = BCC.GetChoiceClone(currentChoice); //초기 화면
				BDTG.Show();
				BDTG.delay = 0;
				ConsoleKeyInfo c = Console.ReadKey();
					while(c.Key != ConsoleKey.Escape){
					var monsterTurn = Task.Run(()=>MonsterTurn());
						if(died){
							backField = "testStream";
							battleTurn = false;
							battleAnd = true;
							return;
						}
						if(BCC.GetChoiceClone(currentChoice).ChoiceType != ChoiceType.QUICKNEXT)//QUICKNEXT를 실행했을때 다음으로 넘어가는중SelectingText에서 ArgumentOutOfRangeException발생하는 문제가 잇음 5.13
							BDTG.SelectingText(c);

						if(c.Key == ConsoleKey.Enter){
							//testLog("Enter");
							if(battleTurn == false){
								battleTurn = true;
							}else{
								testLog("durl");

								continue;
							}
							monsterTurn.Wait();
							currentChoice = (String)BDTG.Cho.GetValueOn(BDTG.currentSelectNum);// 선택한 보기에따라 초이스 선택
							if(globerMonster.HpState() == 3){ //8.22 몬스터의 HP상태가 빈사 상태일때 배틀 페이즈 종료 const int Died = 3
								BDTG.Init();
								Choice cho = BCC.GetChoiceClone("andPhase"); //BDTG의 Cho를 초기화 하면서 OnlyShowText에 있던 텍스트는 integratedList에 들어감으로 choice에 넣기전에 수정해 줘야함
								cho.OnlyShowText = new List<TextAndPosition>() //몬스터 상태메세지 초기화
										{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}};
								BDTG.Cho = cho;

								BDTG.Show();
								c = Console.ReadKey();
								battleTurn = false;
								battleAnd = true;
								testLog("전투끝");
								return;
							}
							if(currentChoice == "attackPhase"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
								actionType = ActionType.ATTACK;
								Choice cho = BCC.GetChoice(currentChoice);
										cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
											{new TextAndPosition(globerPlayer.AttackCry(),15,9,1){AlignH = true}};
								Attacker = globerPlayer;
								Defender = globerMonster;
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

							if(BCC.GetChoiceClone(currentChoice).ChoiceType == ChoiceType.QUICKNEXT){//QUICKNEXT구현을 위해 추가된 if문
								BDTG.Init();
								BDTG.Cho = BCC.GetChoiceClone(currentChoice);
								BDTG.Show();

								currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
									if(currentChoice == "reactionPhase"){ //8.22
										Choice cho = BCC.GetChoice(currentChoice);
										cho.OnlyShowText = new List<TextAndPosition>() 
											{new TextAndPosition(globerMonster.Reaction(),15,9,1){AlignH = true}};
										cho = BCC.GetChoice("movePhase");
										cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
											{new TextAndPosition(globerMonster.CurrentState(),15,3+5,1){AlignH = true}};
									}
									
									c = Console.ReadKey(); //8.24
									
							}
							BDTG.Init();									battleTurn = false;

							break;
						}
						//방향키나 숫자를 누르면 여기로 넘어옴
						BDTG.Show();
						c = Console.ReadKey();
					}

			}
	}
	
	public static void MonsterTurn(){
			Thread.Sleep(5000);
			if(battleTurn == false && !battleAnd){
				battleTurn = true;
			}
			else{
				if(battleAnd){
					battleAnd = false;
					return;
				}
				return;
			}
			DisplayTextGame BDTG = new DisplayTextGame();
			ChoiceControler BCC = new ChoiceControler();
			Backgrounds backgrounds = new Backgrounds();
			String currentChoice = "monsterAttack"; //첫 화면



			Choice B1 = new Choice(){
					Name = "monsterAttack",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition(globerMonster.AttackCry(),15,3+5,1){AlignH = true}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"reactionPhase"}},
					BackgroundText = backgrounds.GetBackground(1)
			};
			BCC.AddChoice(B1);

			Choice B2 = new Choice(){
					Name = "reactionPhase",
					ChoiceType = ChoiceType.QUICKNEXT,
					OnlyShowText = new List<TextAndPosition>()
								{new TextAndPosition(globerMonster.Reaction(),5,10,10){AlignH = true,PriorityLayer=1}},
					IndicateChoice = new Dictionary<int,Object>(){{0,"movePhase"}},
					BackgroundText = backgrounds.GetBackground(1)
			};
			BCC.AddChoice(B2);

			if(BCC.GetChoiceClone(currentChoice).ChoiceType == ChoiceType.QUICKNEXT){//QUICKNEXT구현을 위해 추가된 if문
				BDTG.Init();
				BDTG.Cho = BCC.GetChoiceClone(currentChoice);
				BDTG.Show();

				Attacking(globerMonster,globerPlayer);
				currentChoice = (String)BCC.GetChoiceClone(currentChoice).QuickNext();
				Console.ReadKey();
					if(currentChoice == "reactionPhase"){ //8.22
						Choice cho = BCC.GetChoice(currentChoice);
						cho.OnlyShowText = new List<TextAndPosition>() 
							{new TextAndPosition(globerPlayer.Reaction(),15,9,1){AlignH = true}};
					}
				BDTG.Init();
				BDTG.Cho = BCC.GetChoiceClone(currentChoice);
				BDTG.Show();
				//Console.ReadKey();
				if(globerPlayer.Hp <= 0) died = true;
			}
			battleTurn = false;
		
	}
}
//}