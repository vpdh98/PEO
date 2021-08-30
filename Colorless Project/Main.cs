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
//using Battle;







			

public static class Define{
		public const int ERROR = 99999;
		public const String BATTLEPHASE = "BattlePhase";
		public const int SCREEN_WIDTH = 64;
		public const int SCREEN_HEIGHT = 20;
}

namespace Game
{
	public class main{
		public delegate void AttackMethod(Character Attacker,Character Defender);
		public static AttackMethod attack;
		public static void Main()
		{
			String currentChoice = "t0828-1"; //첫 화면
			DisplayTextGame DTG = new DisplayTextGame();
			ChoiceControler CC = new ChoiceControler();
			CharacterList CList = new CharacterList();
			/* //파일 생성하는 법
			DirectoryInfo dr = new DirectoryInfo("Data");
			dr.Create();
			FileStream fsa = File.Create("Data/a.txt");
			fsa.Close();
			*/
			
			Player player = CList.GetPlayer("용사");
			
			CC.AddScenario(new Scenario());
			
			Task.Factory.StartNew(BattleCal);
			
			while(true){
			DTG.Cho = GameManager.SpawnMonster(CC.SetChoiceClone(currentChoice)); //초기 화면
				
			//DTG.PrintListInfo();
			DTG.Show();
			DTG.delay = 0;
			
			ConsoleKeyInfo c = Console.ReadKey();
				while(c.Key != ConsoleKey.Escape){
					/*Choice choice = CC.SetChoice(currentChoice);
					if(DTG.Cho != choice){
						DTG.Cho = choice;
					}*/
					DTG.SelectingText(c);
					if(c.Key == ConsoleKey.Enter){
						currentChoice = DTG.Cho.ChoiceNext(DTG.currentSelectNum);
						
						
						if(CList.GetMonster(currentChoice) != null){ //배틀페이즈
							for(int i = 0;i<DTG.Cho.MonsterList.Count;i++){//몬스터 List에서 제거함으로써 spawn이 안되게 함으로 선택지에서 제거.. 하려는 의도였음 8.23
								if(DTG.Cho.MonsterList[i].Name == currentChoice){
									DTG.Cho.MonsterList.RemoveAt(i);
									break;
								}
							}
							DTG.Init();
							Console.Clear();
							currentChoice = BattlePhase(player,CList.GetMonster(currentChoice),DTG.Cho.Name); //currentChoice에 현제 선택된 몬스터 이름이 들어가 있음 //8.23
							
							DTG.Cho = CC.SetChoice(currentChoice);
							
							//Console.WriteLine(DTG.Cho.MonsterList[0].GetRandomSpawnMessage().text);
						}
						Console.WriteLine(currentChoice);
						DTG.Cho.LeaveChoice();
						if(CC.SetChoice(currentChoice).ChoiceType == ChoiceType.QUICKNEXT){//QUICKNEXT구현을 위해 추가된 if문
							DTG.Init();
							DTG.Cho = CC.SetChoice(currentChoice);
							DTG.Show();
							currentChoice = CC.SetChoice(currentChoice).QuickNext();
							c = Console.ReadKey();
						}
						
						DTG.Init();
						break;
					}
					DTG.Show();	
					c = Console.ReadKey();
				}
			}
			
		}
		
		
	}	
	
	
	public static class GameManager{
		public static Choice SpawnMonster(Choice choice){ //몬스터 선택지가 중앙에 있는 기존 선택지 마지막 열 다음으로 들어가도록 해주는 메소드
			Console.WriteLine("in SpawnMonster");
			Console.WriteLine(choice.MonsterList.Count);
			if(choice.MonsterList == null){
				return choice;
			}
			Random random = new Random();
			List<Monster> monsterList = choice.MonsterList;
			List<TextAndPosition> selectList = choice.ChoiceText;
			int selectListCount = choice.CTNum;
			int monsterListCount = monsterList.Count;
			int spawnChance;
			int firstX = selectListFirststPositionX(selectList);
			int lastY = selectListLastPositionY(selectList);
			int mPositionX = firstX;
			int mPositionY = lastY+1;
			for(int i= 0;i<monsterListCount;i++){
				if(!monsterList[i].IsSpawn){
					spawnChance = monsterList[i].SpawnChance;
					if(random.Next(1,101) < spawnChance){
						//monsterList[i].MonsterInfo();
						Console.WriteLine(monsterList[i].Name);
						selectList.Add(new TextAndPosition(monsterList[i].GetRandomSelectMessage().text,mPositionX,mPositionY++,true));
						choice.IndicateChoice.Add(selectList.Count-1,monsterList[i].Name);                            //배틀페이즈로 들어가는 선택지로 추가
						monsterList[i].IsSpawn = true;
					}
				}
			}
			return choice;
		}
		
		static int selectListLastPositionX(List<TextAndPosition> sellist){
			int lastNum = 0;
			foreach(TextAndPosition tap in sellist){
				if(lastNum < tap.x && tap.x<50){//좌우로 이동하는 선택지의 포지선 값을 제외
					lastNum = tap.x;
				}
			}
			return lastNum;
		}
		static int selectListLastPositionY(List<TextAndPosition> sellist){
			int lastNum = 0;
			foreach(TextAndPosition tap in sellist){
				if(lastNum < tap.y && tap.y < 19){ //좌우로 이동하는 선택지의 포지선 값을 제외
					lastNum = tap.y;
				}
			}
			return lastNum;
		}
		static int selectListFirststPositionX(List<TextAndPosition> sellist){
			int firstNum = 9999;
			foreach(TextAndPosition tap in sellist){
				if(firstNum > tap.x && tap.x > 1){
					firstNum = tap.x;
				}
			}
			return firstNum;
		}
		static int selectListFirstPositionY(List<TextAndPosition> sellist){
			int firstNum = 9999;
			foreach(TextAndPosition tap in sellist){
				if(firstNum > tap.y && tap.y > 1){
					firstNum = tap.y;
				}
			}
			return firstNum;
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
//여기서 총체적인 문제 발생. 선택지를 없에려면 ChoiceText,selectList,IndicateChoice를 모두 없에고 IndicateChoice의 키값을 다시 설정해 줘야 되는 문제 발생
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
	



