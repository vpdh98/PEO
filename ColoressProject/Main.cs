using System;
using System.Collections.Generic;
using Characters;
using static Characters.User;
using static Game.GameManager;
using Game;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using static BattleSystem;
using static Convenience;
using static ItemData;
using static PlayData;
//using Item;
//using Battle;

			/* //파일 생성하는 법
			DirectoryInfo dr = new DirectoryInfo("Data");
			dr.Create();
			FileStream fsa = File.Create("Data/a.txt");
			fsa.Close();
			*/

			

public static class Define
{
		public const int ERROR = 99999;
		public const String BATTLEPHASE = "BattlePhase";
		public const String INVENTORY = "Inventory";
		public const int SCREEN_WIDTH = 64;
		public const int SCREEN_HEIGHT = 20;
		public const int SCREEN_POS_X = 60;
		public const int SCREEN_POS_Y = 8;
		public delegate void Event();
}

public static class ItemData{
	public static ItemList itemList = new ItemList();
}

public static class PlayData{
	public static String savePoint = "c1";
	public static String currentChoice = "c1"; //첫 화면
	public static CharacterList CList = new CharacterList();
	public static Choice currentOpenChoice; //현재 Display되고 있는 Choice를 담는변수. 접근에 주의!!
	public static Choice accessAbleChoice; //접근할 Choice를 담는다
	
	public static List<String> alreadySpawnedEnemyList = new List<String>();
	
	public static ChoiceControler WorldMap = new ChoiceControler(new Scenario());
	
	public static Player player = CList.GetPlayer("용사");
	
	
}




namespace Game
{
	public class main
	{
		//public delegate void AttackMethod(Character Attacker,Character Defender);
		//public static AttackMethod attack;
		static ConsoleKeyInfo keyInfo;
		static DisplayTextGame DTG = new DisplayTextGame();
		
		
		
		public static void Main()
		{
			player = CList.GetPlayer("용사");
			
			player.inven.AddItem(new Item());
			player.inven.AddItem(itemList.GetItem("전설의검"));
			player.inven.AddItem(new Item(){Name = "HP물약"});
			player.inven.AddItem(new Item(){Name = "MP물약"});
			player.inven.AddItem(new Item(){Name = "무색 프리즘"});
			player.inven.AddItem(new Item(){Name = "???"});
			player.inven.AddItem(new Weapon(){Name = "낡은 검",ItemExplan="이것은 네후쉽이 파는 검 중\n하나로 낡아 오히려 도움이\n되지 않을것 같은 검이다.",AttackPower = -3,AttackSpeed = 1});
			player.inven.AddItem(new Item(){Name = "고장난 시계"});
			player.inven.AddItem(new Item(){Name = "꿀"});
			player.inven.AddItem(new Item(){Name = "향로"});
			player.inven.AddItem(new Item(){Name = "피리"});
			player.inven.AddItem(new Item(){Name = "파리"});
			player.inven.AddItem(new Armor(){Name = "낡은 방패",ItemExplan="이것은 네후쉽이 파는 방패 중\n하나로 공격을 막을 수 있을지\n장담할 수 없다.",Defense = 1});
			player.inven.AddItem(new Armor(){Name = "황금 갑옷",ItemExplan="보기만해도 눈이 부시는 황금 \n갑옷 이다. 과연 방어력은 어\n떨지..",Defense = 10});
			player.inven.AddItem(new Item(){Name = "네후쉽의 수프"});
			player.inven.AddItem(new Item(){Name = "Red 프리즘"});
			player.inven.AddItem(new Item(){Name = "??????"});
			
			
			Task.Factory.StartNew(BattleCal);	
			
			DTG.Display(GameManager.SpawnUniqueEnemy_Stay(WorldMap.GetChoice(currentChoice)));
			keyInfo = Console.ReadKey();
			
			while(keyInfo.Key != ConsoleKey.Escape)
			{
				DTG.SelectingText(keyInfo);
				if(keyInfo.Key == ConsoleKey.I) { player.inven.OpenInventory(); }
				
				if(keyInfo.Key == ConsoleKey.Enter) //Enter
				{
					currentChoice = (String)DTG.GetCurrentSelectValue();

					if(CList.GetEnemy(currentChoice) != null){ currentChoice = BattlePhase(player,CList.GetEnemy(currentChoice),DTG.Cho.Name); } //currentChoice에 현제 선택된 몬스터 이름이 들어가 있음 //8.23
					if(CList.GetNpcOriginal(currentChoice) != null){currentChoice = TalkToNPC.Accost(player,CList.GetNpcOriginal(currentChoice),DTG.Cho.Name);} //currentChoice에 NPC의 이름이 들어가면 실행
					DTG.Cho.LeaveChoice();
					
					if(WorldMap.GetChoiceClone(currentChoice).ChoiceType == ChoiceType.QUICK) { runQuick(); }  //QUICK구현을 위해 추가된 if문
					
					if(WorldMap.GetChoiceClone(currentChoice).ChoiceType == ChoiceType.QUICKNEXT) { runQuickNext(); } //QUICKNEXT구현을 위해 추가된 if문
					DTG.Display(GameManager.SpawnUniqueEnemy_Stay(WorldMap.GetChoice(currentChoice)));
					keyInfo = Console.ReadKey();
				}
				else
				{
					DTG.Display(); //Display의 Init()과 choice초기화가 없는 버전
					keyInfo = Console.ReadKey();
				}
			}
				
			
		}
		
		public static void runQuickNext()
		{
			DTG.Display(WorldMap.GetChoiceClone(currentChoice));
			currentChoice = (String)WorldMap.GetChoiceClone(currentChoice).QuickNext();
			Thread.Sleep(2000);
			//keyInfo = Console.ReadKey();
		}
		
		public static void runQuick()
		{
			DTG.Cho = WorldMap.GetChoice(currentChoice);
			currentChoice = (String)WorldMap.GetChoiceClone(currentChoice).QuickNext();
			Choice tcho = WorldMap.GetChoice(currentChoice);
			accessAbleChoice = tcho;
			DTG.Cho.QuickRun(); //delegate실행
			DTG.Cho.QuickDelegate = ()=>{}; //한번만 실행되도록 비움
			DTG.Display(GameManager.SpawnUniqueEnemy_Stay(tcho));
		}
		
	}	
	
	
	public static class GameManager
	{
		
		public static Choice SpawnUniqueEnemy_Stay(Choice choice)
		{ //몬스터 선택지가 중앙에 있는 기존 선택지 마지막 열 다음으로 들어가도록 해주는 메소드
			if(choice.EnemyList == null)
			{
				return choice;
			}
			Random random = new Random();
			
			//int count = 0; //2021.09.08추가
			
			List<Enemy> monsterList = choice.EnemyList;
			List<TextAndPosition> selectList = choice.SelectText;
			int selectListCount = choice.selectTextNum;
			int monsterListCount = monsterList.Count;
			int spawnChance;
			int firstX = selectListFirststPositionX(selectList);
			int lastY = selectListLastPositionY(selectList);
			int mPositionX = firstX;
			int mPositionY = lastY+1;
			for(int i= 0;i<monsterListCount;i++)
			{
				if(!monsterList[i].IsSpawnOnce) continue; //한번만 스폰되는 몬스터를 통과시킨다.
				if(alreadySpawnedEnemyList.Contains(monsterList[i].Name)) continue; //이미 소환됫던 몬스터는 컷
				
				if(!monsterList[i].IsSpawn)
				{
					spawnChance = monsterList[i].SpawnChance;
					if(random.Next(1,101) < spawnChance)
					{
						alreadySpawnedEnemyList.Add(monsterList[i].Name);//소환한 몬스터를 리스트에 추가
						//monsterList[i].EnemyInfo();
						Console.WriteLine(monsterList[i].Name);//@@@@@@@@@@@@@@@@@@@@@@
						selectList.Add(new TextAndPosition(monsterList[i].GetRandomSelectMessage().text,mPositionX,mPositionY++,true));
						choice.IndicateChoice.Add(selectList.Count-1,monsterList[i].Name);                            //배틀페이즈로 들어가는 선택지로 추가
						monsterList[i].IsSpawn = true;
					}
				}
			}
			
			return choice;
		}
		
		public static int selectListLastPositionX(List<TextAndPosition> sellist)
		{
			int lastNum = 0;
			foreach(TextAndPosition tap in sellist){
				if(lastNum < tap.x && tap.x<50){//좌우로 이동하는 선택지의 포지선 값을 제외
					lastNum = tap.x;
				}
			}
			return lastNum;
		}
		public static int selectListLastPositionY(List<TextAndPosition> sellist)
		{
			int lastNum = 0;
			foreach(TextAndPosition tap in sellist)
			{
				if(lastNum < tap.y && tap.y < 19)
				{ //좌우로 이동하는 선택지의 포지선 값을 제외
					lastNum = tap.y;
				}
			}
			return lastNum;
		}
		public static int selectListFirststPositionX(List<TextAndPosition> sellist)
		{
			int firstNum = 9999;
			foreach(TextAndPosition tap in sellist)
			{
				if(firstNum > tap.x && tap.x > 1)
				{
					firstNum = tap.x;
				}
			}
			return firstNum;
		}
		
		
		public static int selectListFirstPositionY(List<TextAndPosition> sellist)
		{
			int firstNum = 9999;
			foreach(TextAndPosition tap in sellist)
			{
				if(firstNum > tap.y && tap.y > 1)
				{
					firstNum = tap.y;
				}
			}
			return firstNum;
		}
		
		public static void Equip(Item item)
		{
			if(item.GetType().Name == "Weapon")
			{
				PlayData.player.Weapon = (Weapon)item;
			}
			if(item.GetType().Name == "Armor")
			{
				PlayData.player.Armor = (Armor)item;
			}
		}
		
		public static void Eat(Item item)
		{
			if(item.GetType().Name == "Potion")
			{
				PlayData.player.Hp += ((Potion)item).Hp;
			}
		}
		
		public static void DespawnEnemy(String monsterName, Choice targetChoice){
			int index = FindKeyByValue(targetChoice.IndicateChoice,monsterName);
			targetChoice.SelectText.RemoveAt(index);
			targetChoice.IndicateChoice.Remove(index);
		}
		
	}
	
	/*class TextSerialization{
		String TextSerialize(ShowInfo sif);
		ShowInfo TextDeserialize(String);
	}*/
	
}

//2021.3.1 
//1.Damage 처리를 위한 IDamageable 인터페이스와 DamageSystem클래스 만듦
//2.Charater 클래스 만듦

//2021.3.2
//DamageSystem을 구현하던중 전달하려는 Charater의 자식 클래스들의 필드값을 참조값으로 전달하려고 Attacking메소드의 매개변수에 ref키워드를 넣었는데 
//ref키워드를 넣기전에는 자식 클래스인 Player와 Monster클래스가 Charater클래스로 형변환이 잘 이루어졌는데 넣고 나서는 형변환이 되지 않음
//그래서 매개변수를 Charater가 아닌 각각의 클래스로 넣었더니 됨. 하지만 Charater클래스 형식으로 Player,Monster를 다 전달하고 싶음
//해결: 먼저 변수를 선언할때 각 클래스가 아닌 Character로 선언한 변수 안에 Player,Monster를 할당함
//그리고 나서 그냥 실행했을때는 Player와 Monster의 메소드가 아닌 Character의 메소드가 호출되는 문제가 있었지만 virtual과 override키워드를 사용하니 해결
//추가 사항: 그냥 처음 한대로 하고 override키워드를 사용해 구현했더니 됨.. 따로 ref키워드로 참조할 필요가 없엇음 헛수고

//2021.3.4
//DisplayTextGame 클래스를 구현하여 텍스트를 방향키로 셀렉트 할 수 있는 기능까지 구현
//Show 메소드를 수정하여 Choice클래스를 매개변수로 받았을때 정형화된 화면을 출력할 수 있도록 구현해야함 
//보여주는 텍스트의 정보를 담을 ShowInfo클래스 생성,필요한 필드는 텍스트의 위치값과 텍스트 값
//현재 목표는 Choice클래스를 DTG에서 받았을때 Display를 해주고 선택지를 선택했을때 다음 Choice로 넘어가는 것 구현하기(유니티의 Scene과 비슷)

//2021.3.7
//TextAndPosition클래스를 구현하여 적용
//SceneControler 같은 choice들을 통합적으로 관리해줄 클래스를 만들 필요를 느낌
//그래서 ChoiceControler를 만들고 Dictionary클래스를 통해 choice들을 관리하게 만듦
//이제 DisplayTextGame클래스를 통해 ChoiceControler에서 choice를 가져와 선택 가능한 텍스트들과 읽기만 할 수 있는 텍스트들을 구분해 출력하는 것을 해야함
//구분해서 출력하는 기능을 구현 하였으나.. 콘솔에서는 텍스트가 위에서부터 순차적으로 출력되어야 한다는 점을 간과
//그래서 먼저 출력된 텍스트가 더 위에 있으면 나중에 출력되는 아래있는 텍스트가 다 가려버림
//선택할 수 있는 텍스트와 읽기만 할 수 있는 텍스트들을 합친 List를 만들고 y값을 기준으로 sorting해서 순차적으로 출력하게 함
//그중에 selectList는 따로 구분될 수 있도록 TextAndPosition클래스에 isSelect라는 맴버 추가
//Choice들의 집합인 Scenario 클래스를 따로 만들어 Choice들을 따로 모아서 관리, 각 choice들은 각각의 name으로 관리한다.
//다음엔 전투 시스템,폰트색 변화 등을 구현하는것을 목표로 한다.전투에 대한 정보는 각 몬스터,플레이어 객체가 가지는 것으로 한다.ex) 체력50%이하일때 30%확률로 팔다리가 절단되었다는 텍스트를 출력한다

//2021.3.8
//화살표 입력이 없어도 첫화면에서 화살표가 뜨게 함
//choice클래스 안에 nextChoice맴버를 통해 다음 선택지를 저장해 놓을 수 있게 함
//nextChoice ChoiceNext 헷갈리니까 좀더 덜 헷갈리게 바꿀것
//DTG안에 Init메소드 추가
//선택지를 선택하면 다음 선택지로 넘어가도록 메인에 로직 짬. Awesome!
//다음은 전투 시스템과 전투중 캐릭터의 상태에따라 다른 메세지가 출력되도록 하는 것 구현하자
//먼저 CharacterList 클래스를 따로 만들어 캐릭터 객체들을 관리
//전투 시스템을 구현하려다 먼저 문장을 한자씩 출력하는 기능을 구현함
//한글자씩 출력하는 부분 좀더 손보기, 한글자씩은 TextAndPosition에 설정된 딜레이대로 출력되고 한문장씩은 DTG에 설정된 딜레이대로 출력되게

//2021.3.9
//한글자씩출력되는 것 해결, TextAndPosition에 textFrontDelay속성을 추가해 한문장씩 딜레이도 가능하게 함
//선택지를 선택했을때 지금은 다음 선택지로 넘어가는 기능밖에 구현 못함. 선택지로 넘어가는 거 외의 전투,대화 등등을 구현하기위해 TextAndPosition안에 그에 대한 정보를 담을 열거형 변수를 선언해야 할 것 같음
//열거형 변수 선언해 놨고 전투시스템을 구현하기 위해선 더 고민해봐야 할 듯, 현재까지 나온 아이디어로는 아예 배틀 페이즈를 따로 만든다. 일정 확률로 초이스 안에서 발생하게 한다.

//2021.3.10
//각 초이스가 다른 역활을 수행 할 수 있도록 하기로 결정. 구현시작
//고심끝에 DisplayTextGame클래스에서 수행하던 기능중 정렬해서 출력하는 부분을 Choice에서 자체적으로 가능하도록 만들기로 함 
//근데 그렇게 하면 각 Choice의 객체를 만들때 너무 많은 데이터가 낭비될거 같아서 그냥 원래 방식대로 해보기로 함
//오늘도 전투시스템은 구현 못하고 ChoiceType중 QUICK,QUICKNEXT를 구현, 코드를 정리할 필요가 있어보임, 다음엔 컬러를 써보자

//2021.3.11
//컬러써서 글자색 물들이기됨
//STREAM 타입을 추가해서 다른 텍스트들을 유지하면서 한위치에 여러텍스트들을 순차적으로 출력하는 기능을 구현함. 다음시간에 최적화 시도,여러가지 테스트 해보기

//2021.3.13
//UI를 만들어본다. Background를 구현해본다
//Background위에 다른 텍스트를 중첩 시키는건 좀더 생각해본다
//테두리만 구현해본다
//Backgrounds클래스를 구현해 한꺼번에 Background들을 관리하려고함
//지금 오류가 있는데 잘보고 수정하기

//2021.3.14
//오류 두가지 수정,
//첫번째 NullReferenceException은 Backgrounds안에 필드를 초기화하지 않아서 발생한 오류였다...
//두번째 ArgumentOutOfRangeException: Value must be positive and below the buffer height.는 SetCursorPosition 메소드를 호출할때 터미널창의 크기가 충분히 크지 않아서 발생한것이엿다...
//DTG안에 Background메소드를 따로 만들어 항상 전부 출력될 수 있도록 함
//구현했지만 하나하나 출력하는 텍스트에 의해 밀려나는 현상이 있음
//x값을 -1해서 출력해보고 시도해봤지만 안됨. 코드상의 문제가 아니라 콘솔 출력상에 문제인가 싶음

//2021.3.19
//오랜만에왔다
//밀리는 현상 수정하기전에 코드 정리를 함. PrintAlgorithm과 StreamPrint를 만들어서 묶어 놓음
//드디어 해결했다!!! 커서 위치를 옮기고 한글자씩 출력하는 방식에서 커서 위치는 그대로 두고 스트링을 더해가며 출력하는 방식으로 바꾸니 됨! 굳ㄱ두굳굳 
//이제 UI,전투시스템,인벤토리,장비 등등을 구현해보자
//그전에 Choice 작성법 적어놓고 Stream기능을 좀더 손봐 보자

//2021.3.20
//대입하지 않으면 null로 남아있던 choice의 필드들을 모두 빈 인스턴스를 생성하도록 바꿨다.
//한번 선택한 초이스를 다시 선택하면 다시 실행되게 하려고 한다. -> 초기화 하고 싶은 초이스만 그렇게 되도록
//딮 카피한 초이스만 수정하여 다시 그 초이스를 불러올때 초기화된 상태로 불러오려 했으나. Clone메소드를 호출했을때 드는 시간이 너무 많이 늘어나 순차적으로 출력됨
//출력 시스템의 전체적인 개편이 필요할듯..
//아님 처음에 시나리오만 카피해놓고 초기화가 필요한 초이스들만 다시 불러오는 방식은 어떨까?
//생각해보니 그 방식도 시나리오를 지속적으로 카피하지 않으면 똑같을것 같다.
//출력시스템 최적화및 개편을 시도해보자(불필요한 출력 제거,Stream 출력 손보기)

//2021.3.23
//오늘은 대대적인 수정이 이루어질 것이다.
//출력시스템을 만들다보니 굳이 위에서부터 아래로 출력할 필요가 없다는 사실을 깨달았다.
//position만 잘 입력해주면 그 위치에 텍스트가 잘 출력된다.
//그래서 유니티에서도 사용하는 방식인 우선순위 레이어 방식으로 출력하려고 한다.

//우선순위레이어 방식으로 변경완료
//Stream타입을 없에고 TextAndPosition속성에 isStream을 추가하여 구현
//이전 Stream의 길이만큼 공백문자를 채워 지워지게 했지만. 한글과 영어의 글자수가 달라서 한글다음 영어가 오면 안지워질때가 있음 수정요함
//한글자씩 출력되는 텍스트 앞이나 뒤에 딜레이가 추가되면 좋을듯

//2021.3.24
//오늘은 멀티 쓰레딩으로 출력을 구현해볼것이다
//은 다음 기회에.. delegate,task,yield,callback 등등의 개념 이해도를 더 쌓고 오자
//한글과 영어의 글자수가 달라서 한글다음 영어가 오면 안지워질때 GetKoreanCount메소드 만들어서 해결

//2021.3.25
//오늘은 드디어 전투시스템을 구현해볼 것이다.
//몬스터 플래이어 NPC를 나눠서 관리한다.
//choice내부에 각각 객체 리스트 추가
//몬스터나 플래이어의 상태메세지는 콜백함수로 구현하면 좋을 것 같다.
//몬스터 클래스에 스폰확률 추가

//2021.3.27
//전투 데미지 계산은 비동기 콜백 함수로 만들면 좋을거 같다는 생각이 들었다.
//static 선언을 한 DamageSystem클래스를 using static으로 불러와 맴버 Attacker,Defender를 별도의 선언없이 쓰게 했다.
//Attacker,Defender가 null이 아닐때 수시로 데미지를 계산하도록 했다.
//Monster클래스에 4가지 메세지를 추가했다.
//selectList에 요소들에 자동으로 번호를 매기는 기능 추가
//한번 지나갔던 초이스로 돌아가면 텍스트가 바뀌는 기능 LeaveChoice메소드로 구현하려는데 안됨, main에 서순을 잘 바꿔보자

//2021.3.28
//LeaveChoice가 메인에 절대 실행 안되는 if문 안에 있어서 안되는거 였음.. 그런게 왜 있지..?
//이제 몬스터 스폰 시스템을 구현해보자. 초이스를 불러올때마다 일정확률로 스폰되게
//계속 DTG에만 기능을 추가하려니 DTG가 너무 무거워지는 거 같다. 게임 매니저 클래스를 만들어보자
//spawn시스템을 구현하려는중. 신경써야 되는 부분들이 좀 많아서 좀더 생각해 봐야 할것 같다.

//2021.3.29
//choice 안에 있는 몬스터 객체의 정보를 받아 BattleSystem에 있는 메소드를 이용해 DTG에서 출력하는 방식으로 해보자
//GameManager에서 SpawnMonster를 통해 선택지와 해당 선택지를 선택했을때 이동할 배틀페이즈의 초이스를 생성
//스폰시스템은 만들어고 테스트만 해보면 됨

//2021.3.30
//테스트 완료. 디버그에 걸리는거 해결하도록

//2021.3.31
//해결. Choice에 있던 SelectMessage,SpawnMessage를 Monster로 옮김
//Random하게 SelectMessage를 가져오는 함수 추가
//번호로 선택지 고르는 기능 추가
//다음시간엔 배틀페이즈를 만들어보자. 비동기로 해야될듯
//그리고 몬스터를 잡지않고 다른 초이스로 이동했다가 돌아오면 계속 같은 몬스터가 중복되서 추가되는 것 수정하자

//2021.4.1
//Monster클래스에 IsSpawn필드를 추가해서 중복되지 않도록 함

//2021.5.11
//휴가 갖다와서 오랜만에 다시함
//BattleSystem부터 손보기로 함
//선택지에서 몬스터를 선택했을때 String으로 몬스터의 이름이 반환되고 그 이름이 MonsterList에 있으면 BattlePhase가 시작되도록함
//BattlePhase를 다른 메소드로 떼어 Main에서 돌아가는 것과 동일한 사이클로 새로운 시나리오로 진행하려고 하는중
//하나하나 선택지를 짜야한다.
//자동으로 텍스트의 길이에 따라 가운데 정렬되는 기능을 만들면 좋을듯

//2021.5.12
//자동으로 텍스트를 정렬하는 기능을 구현해보자
//TextAndPosition클래스에 AlignH맴버를 추가 하여 true일 경우 가운데 정렬이 되는 위치의 x포지션값으로 바꿔주는 AlignX메소드를 DTG안에 구현하여 List를 초기화할때 검사후 호출되게 함
//그다음 몬스터의 체력에 따라 다른 상태메세지를 출력하는 기능을 만드는중
//ICharacterState를 구현하고있는 클래스에서 CurrentState메소드를 호출하면 현재 체력에 따른 상태 메세지를 string형태로 반환하게 만듦
//CurrentState메소드는 정상적으로 작동. 이제 공격할때마다 데미지 계산을 하고 CurrentState를 다시 초기화시켜서 화면에 출력하도록 해야함
//Characters.User 에 스태틱 맴버 CurrentPlayer를 추가해 전체에서 사용할 수 있도록 함
//데미지 판정은 공격을 하는 순간 Characters.DamageSystem 의 스태틱 맴버인 Attacker,Defender가 null이 아니게 되는 순간 Attacking메소드에서 Defender의 Damage메소드를 호출하여 데미지 계산을 한 후
//Attacker,Defender를 다시 null로 만든다.
//QUICKNEXT타입의 초이스를 사용하여 몬스터 공격,반응 메세지 출력
//QuickNext타입의 초이스에 불필요한 selectList를 없에고 나서 다시 실행했을때 SelectingText()에서 ArgumentOutOfRangeException발생하는 문제가 있음. 해결하기 바람

//2021.8.22
//너무 오랜만에와서 어디까지 짯었는지 확인이 필요했다.
//당일날 수정한 내용을 적어놓아서 다행인것 같다.
//ChoiceControler의 AddChoice메소드에 중복되는 키값을 넣을경우 Value를 수정하도록 함
//왜 reactionPhase에서 movePhase의 상태메세지가 바뀌지 않는지 봤더니, attackPhase에서 바로 QuickChoice로 넘어가기 때문이였음.
//QuickChoice구현 조건문안에 넣으니 정상적으로 작동. 하지만 hp가 일정이하로 내려가면 ArgumentOutOfRangeException 발생
//죽었을때의 상태메세지 추가. 죽었을때에 BattlePhase종료 및 이전 화면으로 돌아가는 기능 추가해야함

//2021.8.23
//몬스터가 죽고 배틀 페이즈를 끝내고 나서 KeyNotFoundException 발생. 원인은 배틀페이즈를 들어갈때 currentChoice에 초이스 이름이 아닌 몬스터 이름을 넣어놓아서
//CC에서 몬스터 이름을 Dictionary의 key값으로 넣어서 발생한 예외. currentChoice를 다시 초기화해 줌으로써 해결 완료
//죽인 몬스터를 선택지에서 없에는 기능 필요. 몬스터가 죽었는지 확인하는 bool 변수를 추가해서 죽었으면 selectList에서 해당 이름의 몬스터가 포함된 선택지를 없에려 했으나.. 
//그냥 선택하는 순간 선택지를 없에는 것이 좋을듯
//여기서 총체적인 문제 발생. 선택지를 없에려면 SelectText,selectList,IndicateChoice를 모두 없에고 IndicateChoice의 키값을 다시 설정해 줘야 되는 문제 발생
//그냥 MonsterList에 몬스터를 없에고 선택지 객체를 새로 만들면 쉽게 해결되지만, 현재 deapcopy가 구현되어 있지 않고 데이터를 따로 다루고 있지 않으므로
//새로운 객체를 다시 할당하려면 데이터를모두 일일이 넣어 새로 객체를 생성해야됨. 해결하려면 Clone함수를 구현하거나 데이터를 따로 관리 해야됨
//DB를 한번 공부해보자

//2021.8.24
//QuickNext타입의 초이스 실행구문에서 바로 다음 초이스로 넘어가지 않게 수정 함
//QuickNext타입의 초이스를 실행했을때 배경이 뜨지 않는 버그 발생
//QuickNext에만 Console.Write(TAndP.text)가 생략되 있어서 Stream에 해당하는 글자만 출력되고 배경은 출력이 안된거였음..
//DB를 따로 쓰지말고 파일로 데이터를 저장해보자
//방법 1. 프로토콜을 만들어 특정 규칙으로 텍스트 or 이진 파일로 저장
//방법 2. C#의 Serialization을 이용하여 데이터를 담은 Dictionary,List등을 이진 파일로 저장

//2021.8.26
//어젠 초번이라 못왔다.
//오늘은 DeepCopy를 위한 Clone메소드를 구현해보자
//찾아보니 그냥 Clone메소드를 구현하는것보단, 복사생성자를 활용하는것이 좋다고 나온다. 이유는 상속관계인 경우 자식클래스에서 복사를 하지 못할 수 있기 때문이다.
//TextAndPosition,Choice,Characters,Monster,NPC,Player클래스에 복사생성자를 이용한 Clone메소드를 구현하였다.
//그런데 List<TextAndPosition>같은 사용자 지정 제네릭을 가진 리스트를 복사하려면 따로 메소드를 만들어야 할것같다.
//List.ConvertAll()메소드를 이용해보려 했는데 안되는듯. 좀더 시도해보고 안되면 메소드를 따로 만들자.

//2021.8.27
//this.MonsterList = that.MonsterList.ConvertAll(o=>o.Clone());에서
//this.MonsterList = that.MonsterList.ConvertAll(new Converter<Monster, Monster>(o => (Monster)o.Clone())); 로 바꿧더니 됫다
//앞에 null이 아닐때 실행시키는 if문 추가
//근데 코드를 보니 그냥 List를 카피해서 반환하는 메소드를 만드는게 더 나을것 같기도..
//Choice를 복사했을때 의도한 대로라면 초이스를 오갈때마다 처음처럼 작동해야하는데 한번갓던 초이스를 다시가면 작동한 후의 상태로 유지되있음. 뭐가 문제지
//Choice를 복사했다면 textDelay값이 계속 동일해야 되는데 0으로 초기화됨, PrintPieceOfText에서 textDelay를 0으로 초기화시키는데 이게 만약 카피된 값이라면 원본값에 영향을 줘서는 안됨.
//TextAndPosition이 제데로 딮카피가 되지 않은듯.
//보니까 Choice클래스의 복사생성자의 TextAndPosition를 담은 List들은 복사를 안햇음.. 멍청.. ConvertAll을 사용해서 딮카피 하니 잘됨
//근데 몬스터 스폰이 안됨..
//원래 얕은 복사로 Choice를 카피하면 스폰 됫엇는데 그것도 안됨.. 몬스터 리스트가 제데로 값이 들어가있는지 비어있는지 확인해 봐야할듯.

//2021.8.28
//코드가 너무 길어져서 소스파일을 여러군데로 나눠서 관리하는것이 편할것 같음.
//소스파일 나눔. 훨씬 깔끔해보인다.
//몬스터 스폰시점을 Choice를 불러온 직후로 해야겠음
//MonsterList에 Monster객체가 제데로 저장이 안되는듯
//MonsterList를 그냥 String형식으로 바꾸고 choice를 불러올때마다 Monster객체를 생성하는 식으로 해야겟..다 라고 생각했는데
//그렇게 하면 각 choice에 몬스터객체를 저장하지 않으므로 해당 choice에서 몬스터의 정보를 저장할 수 없음..
//SpawnMonster메소드에서 Monster객체의 Name필드는 존재하는데 GetRandomSelectMessage를 가져올때 NullReferenceException이 뜸.. 내일 해결해야겟다
	

//2021.8.30
//github에 나눠진 소스파일 올림
//분명 프로퍼티를 통해서 값이 들어갈땐 있는데 get해올땐 NullReferenceException이 뜸.. 왜일까?
//ConvertAll을 사용한 복사가 문제일까싶어 따로 메소드를 만들어서 Monster의 Clone에 SelectMessage의 복사를해봣는데 안됨.

//2021.8.31
//데이터를 소스코드에서 객체화 시켰기 때문에 객체화된 순서가 문제가 아닐까 생각해봄. 함 보자.
//첫번째문제를 찾았음. 복사생성자를 구현할때 복사생성자의 this가 새로 생성된 객체여서 따로 객체 초기화를 해주지 않아 null인 상태에서 reference값이 들어가지 않았던거 였음.
//근데 또 MonsterList를 복사하던중에 NullReferenceException가 뜸.. 내일 해결해야겟음

//2021.9.02
//원인 발견. MonsterList는 null이 아니지만 List의 각 요소가 null이여서 발생한 것이였음.. 왜 null이 됫는지는..
//Choice의 복사생성자에서 각 요소도 null인지 검사하게 함
//근데 이번엔 또 ArgumentOutOfRangeException가 뜸.. 하 디버그 지원 안되니까 너모 힘들다..
//그냥 그 부분을 예외처리함 그럼 작동은 잘 되지만 몬스터가 스폰이 안됨..
//MonsterSpawn하는 부분을 좀더 다듬어야 할듯
//예외처리 대신 isEmptyList()메서드를 만들어서 null이거나 요소 갯수가 0인 list제외시킴. 역시 스폰은 안됨.

//2021.9.03
//오늘은 마치 예초기에 엉킨 잡초와 같은 이 코드들을 정리해 보기로 했음
//choice.cs정리함, 모호한 필드 이름 변경, 불필요한 메서드,생성자 제거
//그리고 노트에 핵심 기능과 필드와 메서드 정리

//2021.9.06
//한 3일동안 플로우차트를 그려봄
//확실히 한장의 플로우차트로 나타내보니 필요한부분과 필요없어보이는 부분이 한눈에 들어옴
//그리고 여전히 몬스터 스폰이 안되서 원인 분석중
//여러가지로 실험해본 결과 Choice의 Clone()메소드에서 that의 MonsterList가 비어있는것이 원인

//2021.9.08
//드디어 해결.. 결과는 굉장히 허무..
//choice의 clone메소드를 실행할때 조건중에 내가만든 isEmptyList()메소드가 쓰이는 구문이 있는데
//여기서 isEmptyList()안에 list.Any()가 !list.Any()로 되어있어야 했던 것이였다.
//이제 다행히 스폰은 잘 되지만 들어가는 선택지 보기와 다른 몬스터 객체가 들어가 있는 경우가 생긴다.
//문제. 처음으로 스폰된 몬스터만 계속 스폰됨. 특이한점은 선택지가 하나만 추가되는데 선택 메세지는 모든 몬스터 객체중에 하나가 랜덤하게 출력됨.
//원인을 알았음. IndicateChoice목록이 초기화가 안됫음.
//IndicateChoice만 deepCopy되지 않고 얕은 복사가 되게 되있었음..
//그리고 예외처리를 대충 해놔서 이런 문제가 발생한것이였음. 다음부턴 예외처리 제데로 하도록
//해결! 이제 Clone도 잘되고 스폰도 잘된다!!! 짝짝짝

//2021.9.10
//goorm의 문제인지 싸지방 컴터문제인지는 모르겟지만 어제 프로젝트 데이터가 싹다 안보였었음
//

//2021.10.23
//파견지에 싸지방이 없어져서 약 한달간 코딩을 할 수 없었다.
//오늘은 간단하게 Item클래스와 inventory클래스를 구현해 보았다.
//단순히 Item을 Inventory안에 List에 추가시키는 방식으로 구현했다.
//이제 더 어려운 문제는 이것을 콘솔 UI로 만드는 것이다.
//유니티같은 강력한 툴이 얼마나 큰 도움을 주는 것인지 온몸으로 느끼는 중

//2021.10.24
//오늘은 인벤토리를 열고 목록을 불러오는 기능을 넣어보았다.
//이제 아이템을 선택할때 각 아이템 객체의 정보를 불러오고 사용할 수 있게 하는 기능을 추가해야 한다.
//좀 걸릴거 같다. 일단 inventory클래스에 openInventory()메소드를 추가해서 main에서 I키를 누르면 열게 해놓았다.
//BattlePhase처럼 따로 Inventory Choice를 따로 분리해서 사용했다. 

//2021.10.26
//백신맞고 하루 못했다
//오늘은 인벤토리의 아이템 사용기능을 구현하기위해 확인창을 띄우는 confirm_window 클래스를 만들었다.
//Choice로 더 다양한 기능을 사용하고자 현재 선택문장에 해당된 String값을 가져오는 GetChoiceOn()메소드를
//GetValueOn()메소드로 변경하여 Object형식으로 가져오게 만들었다.
//그리고 확인창이 기존창 위에 뜬것처럼 보이기 위해 DisplayTextGame클래스에 isClear변수를 추가해 false일 경우 콘솔을 지우지 않고 그위에 그대로 출력하도록 하였다.
//확인누르면 해당 아이템 객체의 Use()메소드를 호출하도록 함. Use()메소드는 오버라이딩을 통해 무기와 소모품의 사용 방식을 다르게 할 예정

//2021.10.27
//오늘은 인벤토리의 아이템을 사용또는 장비를 착용하는 기능을 구현해 보았다.
//inventory에서 아이템 목록 선택 -> 아이템 사용 확인 메세지 -> 아이템 사용 or 착용
//inventory에서 아이템을 선택하면 invenListObject안에 있는 아이템 객체를 가져옴
//사용 확인 메세지를 띄우고 확인을 누른다면 Item객체안에 있는 Use메소드를 호출
//Use메소드 안에서는 static 클래스인 GameManager안에 Equip또는 Eat 호출(장비냐 소모품이냐에 따라 달라짐)
//Equip메소드 안에서 main의 static 맴버 player를 가져와서 player의 Weapon맴버에 장비 장착
//만약 이미 같은 장비가 장착되어 있다면 아이템 해제 확인메세지 띄운후 확인 누르면 Weapon에 null값을 넣음
//여기서 장비인지 소모품인지는 if문과 GetType()을 이용해서 구분하게함
//이제 인벤토리에서 버리는 기능과 아이템 설명을 보여주는 기능을 추가해야함
//현재 아이템 코드가 따로 없어서 같은 이름을 가진 템은 무조건 같은 템으로 구분됨

//2021.10.28
//오늘은 아이템을 버리는 기능을 추가하였다.
//D키를 누르면 확인 메세지가 뜨고 확인을 누른다면 인벤토리 목록에서 제외하고 invenCho를 초기화한후 출력하여 화면에 바로 반영되도록 하였다.
//Equipment클래스를 만들어서 Weapon,Armor가 상속하게 만들고 Equipment에 isEquip를 추가해 장비장착여부를 확인할 수 있게 하였다.
//장착된 장비는 버릴 수 없게 하였다. 
//AlentWindow를 추가하였다.
//selectList가 없다면 화면이 출력이 안되는 문제가 있었는데 PrintAlgorithm()에서 selectList를 출력하는 부분에 Empty여부를 확인해주는 코드를 넣어 해결하였다.

//2021.10.29
//싸지방 연등을 하는 사람이 나밖에 없다.
//오늘은 인벤토리의 아이템 설명을 보여주는 창을 추가 하였다.
//방향키로 아이템 목록을 가리킬때마다 설명을 바로 보여주도록 구현 하였다.
//DisplayTextGame에 PrintWriteLine()메소드를 추가해서 개행 문자가 있는 문장에서 개행되어 출력되도록 하였다.
//gamewindows(confirm_window의 이름을 바꿈)에 ExplanWindow를 추가하고 ChoiceType에 EXPLAN을 추가하여
//ExplanWindow를 호출했을때 ChoiceType이 EXPLAN인 Choice를 DisplayTextGame에 넣고 PrintWriteLine을 실행하게 하였다.
//이때 ExplanWindow안에서 아이템 분류에 따른 구분을 하여 무기일경우 하단에 공격력,공격속도 등이 출력되도록 하였다.
//상당히 만족스럽다.
//이제 배틀시스템을 가다듬어야겠다.
//무기를 이미 장착한 상태에서 다른 무기를 장착하려할때 장착이 되진 않지만 따로 메세지가 뜨지 않는다 수정요함

//2021.10.30
//장착 메세지 문제 수정 완료
//GlobalPosition을 건드려서 전체적인 화면 위치를 옮겼다.

//2021.10.31
//GameSystem의 Attacking(Attacker,Defender)메소드를 호출하고 
//Attacker의 Attack()메소드를 호출해 공격자의 공격 정보를 AttackInfo객체에 담아 반환하고
//Defender의 Damage(AttackInfo)를 호출하여 공격정보를 넘겨주어 각 객체의 계산식으로 데미지를 계산하여 
//Defender의 필드 값(HP등)에 적용하고
//Player의 AttackCry()메소드를 호출해 attackPhase초이스의 텍스트 값을 변경하여 화면에 띄운다.
//그다음 reactionPhase로 넘어가 monster.Reaction()을 호출해 
//플레이어의 공격력과 몬스터의 HP최대값에 따라 반응을 달라지게 구현하려 하였다.
//Character에 AttackInfo맴버를 추가해 저장할 수 있도록 하였다.
//Weapon클래스에 AttackMessage 맴버를 추가해 무기를 장착했을때 해당 무기에 맞는 공격 메세지가 출력되도록 하였다.
//이제 몬스터가 공격할 타이밍과 공격하는 AI를 추가해야겠다.
//몬스터가 공격하는 턴을 어떻게 표현할까 생각하다가
//전투가 시작되면 비동기 방식으로 초를 세서 일정 초가 지나면 
//플레이어의 선택지가 막히고 몬스터의 턴이 진행되었다가 다시 돌아오는 방식으로 생각하였다.
//하지만 비동기방식으로 몬스터의 턴을 만드니 계속 몬스터의 턴만 진행되게 되었다.
//어떻게 해결하여야 할까?

//2021.11.02
//어제 갑자기 급똥이 마려워서 정리를 제데로 못하고 갔다.
//어제는 전투시스템 구현을 위해 Task,await,async등 비동기 코딩에 대해 공부했다.
//그리고 불침번할때 고민해본 결과 Monster의 턴과 Player의 턴을 각각 비동기 메소드로 동시에 실행하고
//getTurn 변수를 선언해서 getTurn변수를 먼저 참조하는 메소드가 턴을 진행하고 그 턴이 끝날때까지 다른 메소드는 진행하지 못하도록 하는 것을 생각했다.
/*BattlePhase메소드를 async메소드로 만들고
playerTurn메소드와
monsterTurn메소드를
비동기로 동작시켜 전투시스템을 구현하려 했는데.. 잘 안된다.
현재는 playerTurn메소드를 비동기로 실행하고
playerTurn메소드 안에서 MonsterTurn메소드를 비동기로 실행해서 5초의 시간을 재고 5초안에 선택지를 선택할 경우 5초후에  MonsterTurn메소드를 리턴하고 playerTurn메소드는 루틴을 끝까지 돌게 했는데..
잘 안된다. 내가 원하는건 선택을 하는 순간 MonsterTurn메소드가 종료되는것.
Token을 활용해 봐야겠다.
*/

//2021.11.03
//비동기 전투시스템을 구현하려 하였다.
//BattleTimer메소드를 추가하여 timeStart가 true가 되면 작동하고 특정 초 만큼의 시간이 지나면 timeOut을 true로 바꾸고 true가 되면 조건문을 활용해
//monsterTurn을 실행하고 timeOut이 true가 되기 전에 선택지를 선택하면 playerTurn이 실행되도록 했다.
//둘중 하나라도 실행이되면 timeStart가 false가 되고 타이머가 멈추고 초기화 된다.
//턴이 끝나면 다시 타이머가 작동한다.
//이렇게 구현하려 했으나.. 여러가지 문제가 있다.
//먼저 코드를 너무 한메소드에 한정해서 작동하도록 코딩해서 구현한 코드 대부분을 수정해야 할 것 같다.
//그리고 timeOut이 되자마자 MonsterTurn을 실행하고 싶은데 ReadKey는 키 입력이 있을때까지 기다리기 때문에
//timeOut이 되어도 키입력이 있어야 MonsterTurn이 실행되는 문제가 있다.
//해결할 수 있도록 하자.

//2021.11.08
//며칠간 머리도 식힐겸 그림연습을 했다.
//그러다 오늘 아이디어가 떠올랐다.
//몬스터를 완전 비동기로 구현하기는 아직 어려우니 
//비동기로 타이머를 재서 timeOut하면 몬스터의 턴을 가져오고 
//몬스터의 턴이 있을경우 플래이어가 행동 선택지를 선택했을때
//몬스터의 턴이 먼저 실행되도록 짜는 것이다.
//DisplayTextGame의 Init(),Cho초기화,Show()를 한데 묶어 Display(Choice choice)메소드 만듦
//BattleSystem을 구현했지만 SelectMessage를 선택하는 과정에서 동작하지 않음. 수정요함

//2021.11.09
//선택하는 과정이 동작하지 않는 이유를 발견
//이유는 Choice를 DisplayTextGame로 Show하기 전에 이전 Choice의 잔재를 지우기 위해 Init()을 호출하는데
//선택하는 과정에서도 Init()이 호출되어 계속 초기값만 가리키는 것.
//SelectingText에 관련된 필드만 따로 초기화 해주는 InitSelect()메소드 정의
//그래도 몇가지 문제 발생
//1.한번 전투를 하고 나서 다시 BattlePhase로 들어가면 타이머 작동 안함
//2.                 "               로 들어가면 선택지가 선택이 안되어 있음(원래는 1번을 화살표가 가리키고 있어야함)
//3.몬스터 턴이 끝나고 나서 movePhase로 넘어가야 하는데 바로 playerTurn의 어택 페이즈로 넘어감

//2021.11.10
//화면에 Display하는 과정을 단순화 하는것이 좋을것 같음
//오늘은 코드정리가 필요
//루프 패턴을 변경했다.
//어떤 키를 눌러도 마지막에 아래 두줄의 코드가 들어가게 했다.
//BDTG.Display(BCC.GetChoiceClone(currentChoice));
//					keyInfo = Console.ReadKey()
//만족스럽게 배틀시스템이 작동한다!
//수정해야할 부분 조금이 있다.
//1.타이머가 다 지나고 나서 몬스터턴일때 timer.Wait()하는 텀이 길다.
//2.한번 몬스터에게 공격받고나서 다시 타이머가 작동하지 않는다. 

//2021.11.11
//두가지를 한번에 해결했다.
//위 두가지의 문제점은 바로 timer를 시작하는 타이밍에 있었다.
//키를 입력받고 화면에 출력하는 과정에서
//movePhase의 화면에 들어섯을때 타이머가 시작되어야 하는데
//movePhase다음인 attackPhase에서 타이머가 작동되어 발생하는 문제였다.
//그래서 movePhase를 출력하는 모든 BDTG.Display()메소드 바로 뒤에TimerStart()메소드를 호출함으로써 해결했다.
//시간을 재는 BattleTimer메소드와 그 시간을 출력하는 TimerStart메소드를 둘다 비동기로 실행했다.
//전투가 정상적으로 작동된다! 굳ㄱ두
//이제 플레이어가 죽었을때에 상황과 몬스터가 죽었을때의 아이템 드랍을 추가하면 된다.
//그리고 플레이어의 현재 체력에 따른 반응도 추가하면 좋을것 같다. 지금은 몬스터의 한방딜에 따른 반응 밖에 없다.
//그리고 몬스터의 공격 시간을 몬스터의 스피드에 따라 다르게 하는 것도 추가 바람

//2021.12.07
//휴가 갖다가 격리 풀리고 오랜만에 하는 코딩
//몬스터 메세지에 PreAttackMessage를 추가해 줬다.
//플레이어가 movePhase에서 선택에 실패하고 시간 초과시 몬스터가 공격할때 공격메세지 앞에 나오는 공격 실패를 알리는 메세지 이다.
//그리고 초기화줘야할 필드가 늘어나는 만큼 각 클래스 앞에 객체 초기화시 반드시 초기화가 필요한 필드등을 알리는 주석을 다는 것이 좋을것 같아 주석을 달아주었다.
//이런식으로 개발을 하면 할수록 게임엔진의 소중함을 깨달아가는 중이다. 간단하게라도 만들까..
//PlayerReactionMessage를 몬스터 객체에 추가해 몬스터 별로 플레이어의 반응을 다르게 했다.
//배틀페이즈에 들어가면 플레이어의 Reaction을 초기화시킨다.

//2021.12.08
//오늘은 몬스터 처치시 템 드랍, 플레이어의 인벤토리에 추가되는 기능을 추가해보았다.
//Monster클래스에 ItemDrop메소드를 추가하여
//BattlePhase에서 플레이어가 몬스터를 처치시 호출하여 플레이어의 인벤토리에 추가되게 하였다.
//아이템 드랍 확률은 Item객체 안에 저장하였다.

//2021.12.09
//오늘은 방향키로 선택 텍스트를 선택하는 기능에
//좌우 방향으로 움직일수 있도록 ArrowMoveRight()와 ArrowMoveLeft()를 추가하였다.
//원리는 오른쪽 화살표를 눌렀을때에는 선택지 리스트중에 현재 선택된 텍스트 이후에 있는 텍스트의 y값이 현재 텍스트의 y값과 같을경우
//그 텍스트를 가리키게 하였고, y값이 같은 텍스트가 없을경우에는 현제 텍스트보다 x값이 큰 텍스트중 아무거나 선택하게 하였다.
//그마저도 없을경우엔 그냥 다음 인덱스에 있는 텍스트를 선택하게 하였다.
//Item이 Stackable일 경우 item의 Amount를 증가시키게 했음. Stackable이 아니면 중복 획득이 불가하게 했음
//Item객체를 복사할 수 있도록 복사생성자를 추가해야함

//전투시스템에서 선택지를 빠르게 선택해야 공격을 받지 않게 되어있는데, 항상 공격이 1번에 있다보니 그냥 엔터만 누르면 공격을 모두 성공하게 되어있다.
//선택지를 매번 랜덤하게 섞도록 해야겠다.
//Display를 담당하는 클래스 안에서 바뀌도록 해야겠다.

//2021.12.10
//어떤 방식으로 처리할까 생각하다가
//가장 먼저 떠오른 가장 간단한 방식으로 구현하였다.
//DisplayTextGame안에 출력을 담당하는 PrintAlgorithm안에 선택지의 초기값을 설정해주는 코드가 있었는데
//그 앞에 if문으로 movePhase일 경우 랜덤으로 인덱스를 받아 출력하게 하도록 했다.
//과연 바로 떠오른 이 방식이 효율적일지는 모르겠지만.. 평소에 고민하는 시간을 생각하면 효율적인거 같기도 하다.
//화면에 표시되는 선택지의 포지션과 실제 리스트에 들어가있는 인덱스 순서가 달라 상하좌우 원하는데로 되지 않는 문제가 있었는데 해결했다.
//movePhase에서는 번호로 선택지를 선택할 수 없게 만들었다.

//현재 배틀 시스템은 플레이어가 순발력있고 실수만 안하면 한대도 맞지 않을 수 있다.
//일정 횟수이상 연속으로 공격에 성공하면 몬스터에게 확정적으로 공격기회를 주는것은 어떤가? 더 재미있을까?
//movePhase에서 선택지 위치도 랜덤으로 바꿔버리면 난이도를 더 올릴 수 있다.

//2021.12.11
//오늘은 플레이어가 죽었을때 어떻게 처리할지에 대해 고민해보았다.
//여러가지 방식이 있는데, 나는 특정 세이브 포인트에서 시작하는 방식으로 하기로 했다.
//Choice에 isSavePoint라는 해당 Choice가 세이브 포인트인지 확인하는 변수를 만들고
//DisplayTextGame에서 Choice를 할당받을때 IsSavePoint가 true라면 savePoint에 해당 Choice의 이름을 저장하고
//죽었을시에 마지막에 저장되었던 savePoint로 가도록 구현했다.
//savePoint는 정적 클래스 PlayData를 만들어 그 안에 넣어놨다.

//파일 입출력을 통해 배경을 디테일하게 넣어보자
//DisplayTextGame에서 출력할때 원래는 integratedList에 배경까지 모두 넣어놓고 배경의 우선순위만 앞에 두고 출력했었는데
//DisplayBackground 메소드를 따로 만들어 배경만 먼저 따로 출력하도록 하였다.
//모든 배경을 미리 저장되어있는 text파일에서 불러와 출력하게 하였다.
//글자 중앙정렬의 기준을 개행문자로 나눠진 문자열의 배열중 첫번째 요소의 길이로 했다.
//이제 더 멋진 배경을 넣을 수 있을것이다.
//배경에 글씨가 묻히지 않도록 특정 범위에 공백문자를 배경위에 깔아주는 기능을 추가해야겠다.

//2021.12.12
//앞으로 데이터를 자주 다룰것 같아 DataManager클래스를 만들어
//데이터를 관리하기로 했다.
//그리고 경로는 DataManager.cs안에 정적 클래스 DataPath를 만들어 일괄적으로 관리 할 수 있도록 하였다.
//여러가지 생각해보니 구현하는데에 시간이 꽤 걸릴 것 같다.
//하지만 내일 모래 사향산 파견을 가기 때문에 약 한달간 코딩을 할 수 없다..
//오늘은 최대한 파견 이후에 구현해야할 기능에 대한 방향을 써놓고 구현은 파견 갖다와서 해야겠다.

//구현해야할 것
//Choice,Player,Monster,Item,게임 진행도 저장

//Player는 여러 플레이어가 정보를 저장할 수 있도록 한다.
//Player안에는 Inventory안에 Item의 정보도 저장되어 있어야 한다.

//게임 진행 정도를 어떻게 저장할 지 생각해 본다.(진행한 맵,진행한 이벤트,Npc등)

//아 NPC도 구현해야 한다.

//Item은 분류별로 다른 정보를 저장해야한다.(무기,방어구,소모품 등)

//현재 Background에 대한 데이터는 각각 배경마다 각각의 파일로 저장되어 있는데 
//이것도 한 파일로 묶어서 저장을 해야 할지 고민해보자.

//플레이어가 죽었을때의 패널티는 아직 넣지 않았다.
//죽었다가 살아날때 체력풀피

//오늘은 선택지를 선택할때 커서 이동을 좀더 직관적으로 할 수 있게 바꾸어 보았다.
//movePhase에서 한쪽 방향으로 이동시키면 무조건 빠르게 회피또는 공격을 할 수 있게 되있었는데, 커서가 끝에 다다르면 처음 또는 마지막으로 이동하도록 구현해 막았다.

//방어와 회피 실패시 플레이어의 리액션과 죽었을 경우 추가

//2021.12.13
//원래 내일 파견이 예정되어 있었으나..
//부스터 샷을 맞지 않으면 파견갈 수 없다고 해서 못가게 되었다.. 코딩을 할 수 있게 되었으니 좋은건가..

//SpawnMonster메소드에서 몬스터 선택지를 기존 선택지중 중앙에 있는 선택지를 기준으로 추가 되도록 되어있는데
//해당 choice 중앙에 선택지가 없을 경우 제데로 된 위치에 선택지가 추가되지 않는 버그가 있다.

//현재는 Choice를 이동할때 계속 새로운 Choice를 복제해 출력하는데. 새로운 Choice가 아닌기존 Choice의 상태를 유지하는 것도 필요할 것 같다.
//기존 상태를 유지하도록 했다. LeaveChoice() 메소드를 통해 한번 들렸던 초이스를 다시 방문하면 다른 메세지가 출력되도록 하였다.
//지금은 몬스터가 한번 스폰되면 계속 남아있는 문제가 있는데.
//계속 남아있는 몬스터와 맵을 이동할때마다 없어졌다 스폰됫다를 반복하는 몬스터를 나누고 구현해봐야 할것 같다.

//2021.12.14
//오늘은 배경을 출력하는 기능을 손봣다.
//background에 두가지 이상의 배경을 넣으면 중첩시킬 수 있다.
//첫번째 배경 위에 두번째 배경을 중첩시킬때 첫번째배경의 글자 크기보다 두번째 배경의 글자 크기가 작으면 그 차이만큼 그 줄의 길이가 더 짧아지는 에로사항이 있다..
//그래서 그냥 배경위에 글씨만 출력하도록 해보려고 한다.
//글씨만 출력되있으면 가독성이 떨어지므로 현재 선택된 선택지가 잘보이도록 색을 바꾸게 했다.
//아이템 획득도 색을 바꾸도록 했다. (아이템 획득 메세지를 띄우는 AlertWindow매개변수에 ConsoleColor를 추가해 TextAndPosition 생성자의 color에 넣게함)

//2021.12.15
//현재 전투를 생각해보니 회피나 방어를 했을때의 메리트가 없다.
//회피나 방어를 했을때 무조건 공격기회를 준다던지 공격 기회를 잡을 시간을 늘려준다던지 하는 메리트를 추가해야겠다.

//배경을 좀더 정교하게 출력되도록 했다. (한칸짜리 공백문자를 두칸짜리 공백문자로 교체)

//Choice가 실행될때 또는 선택지를 선택했을때 delegate를 실행시키려고 한다.
//DisplayTextGame에 Choice를 초기화시킬때 실행시키는 방법과 
//ChoiceType이 Quick일때 Main에서 따로 처리하는 방벙이 있다. 뭘로 할까?
//두번째 방법으로 구현했다.
//하지만 실제로 Quick을 통해 데이터값을 변경하거나 하려면 데이터를 소스코드에 놓는게 아닌
//따로 파일에 저장해 놔야 가능할 것 같다.
//지금은 경민이 집에서 경민이 방에 들어갔다 나오면 경민이가 스폰되는 기능을 넣으려 하는데
//Quick타입의 선택지도 독립적으로 작동하는 선택지여서 전역변수가 아닌이상 접근을 할 수가 없다.
//해결방법은 위에 말한데로 따로 파일에 저장해놓고 불러오기를 하던지
//아님 전역변수로 모든 데이터를 올려놓으면 된다.
//하지만 전역변수로 모든 데이터를 올려놓으면 메모리가 많이 사용될 것이다.(static으로 선언하기 때문)

//전역변수로 Choice를 올렸다. 나중에 잘 보고 왠만하면 삭제하고 저장방식으로 하는것이 좋을듯하다.
/* 주의해서 판단하고 결정할 것
	위치 main.cs
	
	public static Choice currentOpenChoice; //현재 Display되고 있는 Choice를 담는변수. 접근에 주의!!
	public static Choice accessAbleChoice; //접근할 Choice를 담는다
	
	
	일단 구현해보려고 막짠 코드. QuickRun을 실행하면 accessAbleChoice에 MonsterList에 접근해서 스폰확률을 100으로 하게 햇는데 결과적으로 몬스터 스폰이 안된다.
	public static void runQuick()
		{
			DTG.Cho = CC.GetChoiceClone(currentChoice);
			
			currentChoice = (String)CC.GetChoiceClone(currentChoice).QuickNext();
			Choice tcho = CC.GetChoiceClone(currentChoice);
			accessAbleChoice = tcho;
			DTG.Cho.QuickRun();
			DTG.Display(GameManager.SpawnMonster(tcho));
			//keyInfo = Console.ReadKey();
		}
	
	*/
//2021.12.18
//delegate실행을 성공했다.
/*
QuickDelegate = ()=>{
					PlayData.accessAbleChoice.MonsterList.Add(PlayData.CList.GetMonster("야생의 경민이",100));
				}
				*/
//결국 전역변수에 접근하는 방식으로 실행했다.
//앞으로 더 다양한 상호작용을 할 수 있을 것이다.

//지금은 몬스터를 어떤 방식으로 필드에 소환할지 고민중이다. 그 내용은
//1.한번만 스폰되는 몬스터

//2.무한 스폰되는 몬스터
//2-1.무한 스폰되지만 맵에 랜덤으로 스폰
//2-2.무한 스폰되지만 맵에 한번 스폰되면 계속 머물러있음
//Main에 있는 SpawnMonster_Stay로는 2-2만 구현이 된다.
//2-2가 되는 방법은
//SpawnMonster_Stay(Choice) 메소드를 호출하면
//SpawnMonster_Stay에서 Choice.MonsterList에 있는 몬스터의 스폰 확률을 가져와 계산하여 Choice의 SelectingText에 추가시킨다.
//그리고 그것을 출력하면 자연스럽게 SelectList에 들어가 있는 Monster를 출력하게 된다.

//1,2번을 나누기 위해서는 먼저 몬스터 클래스에 구분할 수 있는 필드를 추가해야 할듯
//그리고 DTG내부에서 MonsterList를 따로 관리해야 할 듯 하다.

//Console.KeyAvailable라는 변수로 키가 버퍼에 입력 되었는지 확인할 수 있다는 정보를 얻음. 이게 된다면 플레이어가 따로 입력하지 않아도 몬스터가 공격하게됨. 실험해봐야겠음.

//2021.12.19
//오늘은 배틀시스템을 좀더 자연스러운 방식으로 손봤다.
//Console.KeyAvailable을 통해 플레이어의 입력이 없어도 몬스터의 공격이 실행되도록 했다.
//그러기 위해서 BattlePhase의 루프를 거의 완전히 바꿧다.

//2021.12.20
//오늘은 몬스터 스폰을 구현해 봐야겠다.
//더 생각을 해보니 사실상 1번과 2-1번만 있으면 될 것 같다.
//한번만 소환되는 몬스터는 보스등 유니크한 몬스터일 것이고
//무한 스폰되는 몬스터는 잡몹일 것이기 때문이다.
//SpawnMonster_Stay를 손봐서 한번만 스폰되고 그 장소에 머무르는 몬스터를 스폰시키고
//DisplayTextGame내부에서 무한 스폰 되는 몬스터를 관리하는 List를 따로 만들어 스폰시켜야 겠다.
//전역 변수로 alreadySpawnedMonsterList를 만들어 한번이라도 스폰된 몬스터를 여기 담으려고 한다.

//SpawnMonster_Stay를 SpawnUniqueMonster_Stay로 바꾸어 Monster 클래스에 추가한 isSpawnOnce가 true인 몬스터 객체만 한번만 스폰되도록 하였고
//DisplayTextGame안에 selectList와 indicateList를 따로 만들어 choice의 리스트를 딥 카피 한 후 
//각각 리스트에 DisplayTextGame내부에서 따로 spawn한 몬스터 리스트를 추가해서 Choice가 DTG에 초기화 될때마다 새로 스폰하도록 했다.
//현재 플레이어가 맞아 죽을때 Battle내부에서 초기화 해야될 값과
//유일 몬스터를 잡았을때 없에는 기능을 추가해야 한다.

//2021.12.22
//오늘은 몬스터를 없에는 기능과
//몬스터를 잡았을때 특정 이벤트 또는 원하는 장소로 돌아가는 기능을 추가해 보도록 하겠다.
//유일 몬스터를 스폰하기 위해 정보가 추가되는 곳은
//1.Choice.SelectList(List<TextAndPosition>)
//2.Choice.indicateList(List<Object>)
//위 두 리스트 이다.
//Choice를 SpawnUniqueMonster_Stayd에 넣으면 Choice.MonsterList에서 몬스터 목록을 가져와
//SelectList와 indicateList에 추가 시킨다.
//그럼 몬스터를 없에기 위해서는 위 두 리스트에서 해당 몬스터를 제거 시켜야한다.
//indicateList에 몬스터 이름이 값으로 들어가므로 indicateList의 이름과 battlePhase에 있는 몬스터의 이름이 일치하는 index를 구해서 없에면 될것 같다.

//그대로 구현에 성공했다.
//PlayData에 WorldMap을 추가해 게임 진행내내 진행되는 Choice를 한꺼번에 담아 관리하기로 했다.

//다음은 특정 이벤트 발생이다.
//이건 몬스터 객체에 delegate를 추가해야 할것 같다.
//어떤 방식으로 구현할지 고민해보았다.
//몬스터 객체에 delegate를 넣어도 다양한 이벤트를 발생시키려면
//메소드 밖에 있는 여러 변수에 접글이 가능하게 해야 할텐데.
//그럼 코드의 은닉성이 훼손될 우려가 있다.
//스스로 규칙을 정할 필요가 있다.
//그래서 생각한 것이 어짜피 monster객체는 battlePhase내에서만 생성되고 소멸하므로
//battlePhase내부의 변수에 접근하는 코드로만 delegate를 만들고
//다음에 갈 choice를 바꿀 수 있도록 하는 코드로만 짜기로 하였다.
//battlePhase에 backField 변수만 바꿔주는 식으로 짜면 될 것이다.
//추가로 choice의 값도 건드려야 할것 같다.
//Define에 delegate void Event()를 추가해 범용적으로 쓸수 있게 하였다.
//Monster객체 안에 Event타입 delegate DeathEvent를 추가해 구현했다.
//DeathEvent에서 바꿀 수 있는 값
//1.backField 값
//2.WorldMap안에 있는 Choice의 값

//간과한 점이 하나 있었다.
//indicateList는 key와 value로 값을 저장하는 dictionary인데 
//여기서 중간값을빼면 키값이 1,3처럼 중간이 비게되어 선택지를 추가하는 기능이 제데로 작동되지 않는다.
//일단 runtime으로 indicateList의 값을 바꾸는 SpawnMonster_Random안에 indicateList의 키값 중복체크해서 인덱스를 따로 결정하는 기능을 넣어야겠다.
//오늘은 시간이 다되서 이만

//2021.12.23
//중복체크 기능을 추가했다.
//값을 하나씩 증가시키면서 indicateList안에 키값이 중복되는지 체크하고
//중복되지 않는 값이 나오면 그 값으로 selectList와 indicateList에 insert해서 추가하도록 만들었다.
//그런데 여기서 문제가 하나더 생겼다.
// 0,1,2,3,4에 맞춰 selectList와 indicateList에 인덱스와 키값에 따른 value가 들어가 있었는데
//만약 3이 삭제 되고 그 자리에 아무것도 추가되지 않는다면
//selectList의 index는 0,1,2,3 이 되지만 indicateList의 키값은 0,1,2,4 가 되어서 3을 선택했을때 KeyNotFoundException이 뜨게 된다.
//해결해보자.
//DisplayTextGame에서 List들을 초기화할때 indicateList를 재정렬 해주는 메소드를 추가해 해결했다.
//0,1,2,4이런식으로 키값이 들어있는 Dictionary를 넣으면 0,1,2,3처럼 만들어 준다.
//결과적으로 키값의 중복체크기능은 무용지물이 됬다. 하지만 없에진 않으려고 한다.

//2021.12.25
//오늘은 크리스마스다.
//그런데 어제 당직을 서서 굉장히 짧은 하루였다.
//그렇다.
//오늘은 엔피씨를 만들어보겟다.
//Monster클래스의 이름을 Enemy로 바꿧다.
//그로 인해 관련 메소드,필드의 이름도 모두 바꿧다.
//엔피씨의 가장 중요한 역활중 하나인 퀘스틀를 구현해보려고 하였다.
//Quest클래스를 만들었다.
//퀘스트를 주면 Quest객체를 Player에게 주는 방식으로 구현하려고 한다.
//해당 퀘스트 조건을 완료해서 다시 그 NPC에게 가면
//NPC는 준 퀘스트 목록 중 해당 퀘스트가 있는지 확인하고 있으면 
//조건을 확인하고 보상을 주도록 구현할 것이다.

//2021.12.27
//어제도 당직을 섯다
//퀘스트를 구현하는데 플레이어의 행동이 퀘스트 조건과 맞는지는 어떻게 확인해야 하는 걸까?
//퀘스트의 목표는 게임에 따라 각양 각색이다.
//단순 몬스터의 사냥,아이템 습득 부터
//특정 장소에 도달, 특정 NPC접촉, 특정 행동
//등등 단순한 수치계산으로만은 그 조건을 확인하기 힘든 퀘스트들도 있다.
//이걸 어떻게 하는게 좋을까?

//퀘스트 종류별로 클래스를 만들어 구현하기로 했다
//Player에 QuestList를 추가했다.
//퀘스트 종류는 사냥,아이템 획득,장소방문,인물접촉 이 네가지로 하기로 했다.
//오늘은 먼저 사냥 퀘스트를 구현했다.
//CheckTarget메소드는 Target의 정보를 Object의 형태로 받을 수 있게 해 퀘스트 목표를 확인 할 수 있게 했다.

//2021.12.28
//어제 사냥 퀘스트를 구현하면서 간과 한 점이 있었다.
//사냥 퀘스트 클래스에 잡아야할 몬스터 목록을 List로 만들어 여러 몬스터를 퀘스트 목표로 할 수 있도록 했는데
//각각의 목표치를 따로 카운트하는 기능을 넣지 않았다. 넣자.
//오늘은 장소방문과 아아템 획득 퀘스트를 추가했다.
//그런데 Item획득 퀘스트 성공 여부를 구현하는 도중
//Item 검색과 포함여부를 확인하는 부분이 완전하지 않다 내일 해결바람

//2021.12.29
//해결했다. List의 Find를 쓰니까 그냥 해결이 됫다.. 어젠 왜 안됫지..
//오늘은 4개의 퀘스트 클래스를 모두 만들고
//Reward클래스도 보상 종류별로 나눠 따로 만들었다.
//그리고 한번 Quest를 만들어서 실행시켜보려고
//QuestData클래스를 만들어 QuestControler안에 QuestData.QuestDatas를 넣고(이름이 모호해 바꿀생각이다)
//QuestControler.GetQuestByName()을 통해 퀘스트를 불러와 NPC객체에 넣으려고 했는데
//안된다.System.TypeInitializationException: The type initializer for 'PlayData' threw an exception. ---> System.NullReferenceException
//이 떠서 안되는데 시간이 다되서 내일 해결해야겠다
//그리고 퀘스트 NPC도 BattleSystem처럼 따로 Display루틴을 만들생각이다

//2021.12.30
//Main에서 QuestControler.AllQuestList = new QuestData().QuestDatas;
//이렇게 초기화 했었는데, 이걸 Characters의 NPC추가 직전에 초기화 하니까 됫다.
//TalkToNPC클래스를 추가해서 엔피씨 화면 출력을 따로 하려 한다.
//NPC객체에 또 추가되야하는 필드는 대화목록 인데 그 종류를 약 6가지로 나눴다.
//1.인사
//2.퀘스트 수락
//3.퀘스트 거절
//4.퀘스트 완료
//5.대화
//6.재방문(퀘스트 수락,퀘스트 거절,인사,대화)
//그에 따라 TalkToNPC에서 사용할 Choice의 종류도 대략 4가지로 나눠질것 같다.(인사,대화,퀘스트 거절수락,퀘스트 완료)

//한가지 더 추가.
//7.퀘스트 전조

//Data에 인사,대화,퀘스트 수락,퀘스트 거절,퀘스트 완료 Choice를 추가 했다.
//각 초이스 중에서 대화를 구현할때에는
//NPC객체에 있는 ConversationMessage를 Choice에 StreamMessage에 하나씩 추가해주는 기능을 구현해야 한다.
//그리고 NPC에 따라 인사 페이즈에서 선택지를 더 추가할 수 도 있게 해야되며
//퀘스트 완료에서는 퀘스트 보상 페이즈로 연결되어야 하며 받은 보상의 목록을 띄어주어야 한다.
//NPC에 따라서 플레이어와 전투 페이즈로 넘어갈 수 도 있다.
//모두 Runtime중에 메세지가 바뀔 수 있도록 TalkToNPC에서 구현한다.

//Item Clone하는 부분에서 무기clone이 제데로 안되는 부분을 수정하는데 시간이 없어서 제데로 못햇다 수정요함

//2021.12.31
//오늘은 올해의 마지막 날이다
//이글을 읽는 당신 복 많이 받길
//Item Clone문제는 해결했다.
//원인은 Item부터 이어지는 복사생성자를 이용한 Clone메소드를 virtual,override 시키지 않아서 부모클래스의 Clone이 호출된 것이다.

//Main에서 TalkToNPC.Accost()를 호출해서 NPC대화 창까지 띄우는데에 성공했다.
//그런데 그 대화창 Choice의 onlyShowText를 변경하려하니 ChangeChoiceText에서 GetChoice를 해올때 KeyNotFoundException이 뜨는 오류가 있다 수정하자.-> ChoiceControler의 객체를 생성할때 Scenario를 안넣어서 발생한 문제였다.

//2021.01.02
//어제 코딩을 하다가 시간이 부족해서 주석은 못적었다
//어제는 퀘스트를 구현하였다.
//Npc클래스
//QuestIntoductionMessage를 추가 : Data에 IndicatePhase를 추가하고 그 Choice의 OnlyShowText를 대체하는 변수
//GetComversationMessageList()->List<TextAndPosition> 로 변경 :ConversationPhase Choice의 StreamText를 대체하기 위한 메소드
//Choice(ChoiceControler)
//초이스의 내용을 변경하는 기능을 수행하는 메소드들을 추가했다.
//ChangeChoiceText, AddChoiceSelectText, ChangeChoiceSelectText 이다
//RemoveChoiceSelectList를 RemoveChoiceSelectText로 변경: 좀더 직관적으로 맞는 이름으로 변경
//QuestExplan Choice 만듬: 해당 Choice에는 Quest객체에 QuestContents를 넣어 퀘스트 내용을 출력하게 함
//TalkToNPC
//QuestIntoduction에 Quest들의 선택지를 추가하기 위한 코드를 넣었다.
//퀘스트 기능의 작동 순서는
//Choice 선택 화면 에서
//NPC선택 -> NPC불러와서 TalkToNPC.Accost()실행 -> Accost에서 첫화면 출력 -> NPC.QuestList에 퀘스트가 하나이상 존재하면 QuestIntoduction로 갈 수 있는 선택지 추가 -> QuestIntoduction에 퀘스트 선택하는 선택지 추가 -> 퀘스트 선택시 QuestExplan으로 넘어가 Quest.QuestContents를 출력 -> 수락시 QuestAccept로 넘어감(거절시 QuestReject)

//오늘은 TextAndPosition에 Layout변수를 만들고
//ONLY_SHOW_DEFAULT, SELECT_DEFAULT, CROSSROAD_DEFAULT (streamText,returnText는 ONLY_SHOW_DEFAULT에 속하게 했다.)
//이렇게 세가지 레이아웃 enum을 만들어 각각의 옵션마다 자동으로 text의 포지션을 잡아주는 기능을 넣었다.
//Choice.Arrangement() 메소드가 그것인데, Choice의 Text를 바꿀때나 ChoiceControler에 Scenario를 초기화 할때 호출되어 레이아웃대로 재정렬 할 수 있도록 하였다.

//레이아웃은 기존에 있는 텍스트의 위치를 기준으로 결정되게 했는데, 아무 텍스트도 없을경우 기본값을 넣어주게 했다.
//StreamText의 경우 레이아웃을 설정하면 지금은 하나의 TextAndPosition만 넣을 수 있게 되어있다. 나중에 수정요함

//현재 퀘스트는 제데로 뜨지만
//퀘스트를 수락했을때 받을 수 있는 퀘스트 목록에서 사라지는 기능과
//퀘스트가 진행중임을 나타낼 수 있는 기능과
//실질적으로 플레이어가 퀘스트 객체를 가져오는 기능이 아직 구현되지 않았다.
//물론 퀘스트 완료도 구현되지 않았다.
//다 구현해보자. 물론 오늘은 말고...

//2022.01.03
//오늘은 퀘스트를 거절했다가 다시 받았을때의 NPC반응을 다르게 하기 위해 코드를 손봤다.
//Quest클래스에 isReject를 추가해 거절했는지 여부를 판단하게 하고 거절한 적이 있으면 QuestAccept,QuestReject Choice에서 처음과 다른 메세지가 출력되도록 하려고 한다.
//처음에는 Choice의 LeaveChoice()메소드를 사용해서 ReturnText안에 있는 메세지를 OnlyShowText에 넣도록 하였는데.
//Quest는 여러게인데 그중 하나만 거절했어도 다음 퀘스트를 수락할때 ReturnText안에 메세지를 출력하게 되어 별도로 Quest클래스에 거절전과 후의 메세지를 저장하는 변수를 만들어야겠다고 생각했다.
//그런데 만들다 또 생각해보니 이미 NPC객체에 거절전과 후의 반응에 따른 메세지가 저장되어있다는 사실을 깨달았다.
//그래서 그냥 isReject값에 따라 그 NPC의 전후 메세지를 Choice에 초기화 시켜주면 될것 같았다.
//결과는 성공. 비록 퀘스트에 따라 다른 반응을 출력 할 순 없었지만 그렇게 하려면 각 퀘스트가 NPC맞춤이 아닌이상은 NPC말투와 어울리지 않을 수 있다는 점 때문에 이렇게 했다.

//퀘스트를 수락하는 순간 해당 Quest객체를 Player객체의 QuestList로 옮겨서 한번 테스트 해봤다.
// HuntQuest에 속하는 슬라임 사냥 퀘스트로 실험해봤다.
//Player객체에 QuestCheck()메소드를 만들어 플레이어가 갖고있는 모든 퀘스트의 CheckTarget(Object info)메소드를 호출해 각 퀘스트 종류마다 각각 퀘스트의 완료 조건을 체크한다.
//info에는 몬스터 객체, Choice객체,NPC객체 등의 정보가 들어갈 수 있다.
//몬스터를 체크하기 위해 BattleSystem에 몬스터를 잡는 구간에서 Player의 QuestCheck()메소드를 호출한다.
//그럼 플레이어는 현재 자신이 가지고 있는 모든 퀘스트의 CheckTarget(Object info)를 호출하면서 info에 몬스터 객체를 넣는다.
//해당하는 퀘스트가 있을경우 그 퀘스트객체 안에 관련 정보를 저장한다.
