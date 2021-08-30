using System;
using Characters;
using System.Collections.Generic;
using System.Threading;
using static DamageSystem;

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
			final_damage = value;
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
public static class DamageSystem
{
	public static Character Attacker;
	public static Character Defender;

	public static void Attacking(Character Attacker,Character Defender){
		Defender.Damage(Attacker.Attack());
	}
}

public static class BattleSystem{
		public static void BattleCal(){
			while(true){
				if(Attacker!=null&&Defender!=null){
					Attacking(Attacker,Defender);
					Attacker=null;
					Defender=null;
				}
				Thread.Sleep(10);
			}
		}

		public static String BattlePhase(Player player,Monster monster,String back){
			String backField = back;
			bool battleAnd = false;

			Backgrounds backgrounds = new Backgrounds();
			Choice Start = new Choice(){
				Name = "firstPhase",
				ChoiceText = new List<TextAndPosition>()         
							{new TextAndPosition("공격한다.",16,13,true),
							new TextAndPosition("도망간다.",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(monster.GetRandomSpawnMessage().text,15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,String>(){{0,"movePhase"},{1,"end"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B2 = new Choice(){
				Name = "movePhase",
				ChoiceText = new List<TextAndPosition>()         
							{new TextAndPosition("공격",16,13,true),
							 new TextAndPosition("방어",28,13,true),
							new TextAndPosition("회피",40,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(monster.CurrentState(),15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,String>(){{0,"attackPhase"},{1,"b4"},{2,"b4"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B3 = new Choice(){
				Name = "attackPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(monster.Name+"베기!",5,9,10,ConsoleColor.Red){AlignH = true,PriorityLayer=1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"reactionPhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B4 = new Choice(){
				Name = "reactionPhase",
				ChoiceType = ChoiceType.QUICKNEXT,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("(몬스터 피해 메세지)",5,9,10){AlignH = true,PriorityLayer=1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"movePhase"}},
				BackgroundText = backgrounds.GetBackground(1)
			};

			Choice B5 = new Choice(){
				Name = "andPhase",
				ChoiceText = new List<TextAndPosition>()         
							{new TextAndPosition("확인",16,13,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(monster.CurrentState(),15,3+5,1){AlignH = true}},
				IndicateChoice = new Dictionary<int,String>(){{0,"backField"}},
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

				while(!battleAnd){
				BDTG.Cho = BCC.SetChoice(currentChoice); //초기 화면
				//DTG.PrintListInfo();
				BDTG.Show();
				BDTG.delay = 0;
				ConsoleKeyInfo c = Console.ReadKey();

					while(c.Key != ConsoleKey.Escape){
						if(BCC.SetChoice(currentChoice).ChoiceType != ChoiceType.QUICKNEXT)//QUICKNEXT를 실행했을때 다음으로 넘어가는중SelectingText에서 ArgumentOutOfRangeException발생하는 문제가 잇음 5.13
							BDTG.SelectingText(c);

						if(c.Key == ConsoleKey.Enter){
							currentChoice = BDTG.Cho.ChoiceNext(BDTG.currentSelectNum);// 선택한 보기에따라 초이스 선택

							if(monster.HpState() == 3){ //8.22 몬스터의 HP상태가 빈사 상태일때 배틀 페이즈 종료 const int Died = 3
								BDTG.Init();
								Choice cho = BCC.SetChoice("andPhase"); //BDTG의 Cho를 초기화 하면서 OnlyShowText에 있던 텍스트는 integratedList에 들어감으로 choice에 넣기전에 수정해 줘야함
								cho.OnlyShowText = new List<TextAndPosition>() //몬스터 상태메세지 초기화
										{new TextAndPosition(monster.CurrentState(),15,3+5,1){AlignH = true}};
								BDTG.Cho = cho;

								BDTG.Show();
								c = Console.ReadKey();
								return backField;
							}

							if(currentChoice == "attackPhase"){ //Attacker,Defender에 값을 넣으면 서로 데미지 계산 1회 실행
								Attacker = player;
								Defender = monster;
							}

							if(BCC.SetChoice(currentChoice).ChoiceType == ChoiceType.QUICKNEXT){//QUICKNEXT구현을 위해 추가된 if문
								BDTG.Init();
								BDTG.Cho = BCC.SetChoice(currentChoice);
								BDTG.Show();

								currentChoice = BCC.SetChoice(currentChoice).QuickNext();
									if(currentChoice == "reactionPhase"){ //8.22
										Choice cho = BCC.SetChoice("movePhase");
										cho.OnlyShowText = new List<TextAndPosition>() //몬스터가 데미지 입을때마다 몬스터 상태메세지 초기화
											{new TextAndPosition(monster.CurrentState(),15,3+5,1){AlignH = true}};
									}
								c = Console.ReadKey(); //8.24
							};

							BDTG.Init();
							break;
						}
						//방향키나 숫자를 누르면 여기로 넘어옴
						BDTG.Show();	
						Console.WriteLine("@@@@@@@@@@@@"+monster.HpState()+";;;;;;;;;;;;;;");
						c = Console.ReadKey();
					}

			}
			return backField;
		}
}
//}
