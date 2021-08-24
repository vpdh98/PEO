using System;
using System.Collections.Generic;
using Characters;
using static Characters.DamageSystem;
using static Characters.User;
using Game;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;





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


namespace Game
{
	public static class Define{
		public const int ERROR = 99999;
		public const String BATTLEPHASE = "BattlePhase";
		public const int SCREEN_WIDTH = 64;
		public const int SCREEN_HEIGHT = 20;
	}
	
	public enum ChoiceType{
			CHOICE,		//선택지
			BATTLE,		//전투
			TALK,		//대화
			GET,		//획득
			SET,		//장착
			QUICK,		//즉시 실행
			QUICKNEXT	//즉시 다음 선택지
	}
	
	public class main{
		public delegate void AttackMethod(Character Attacker,Character Defender);
		public static AttackMethod attack;
		public static void Main()
		{
			String currentChoice = "c1"; //첫 화면
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
			DTG.Cho = CC.SetChoice(currentChoice); //초기 화면
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
	
	public class TextAndPosition{
		public String text{set;get;}
		public int x{set;get;}
		public int y{set;get;}
		public bool isSelect;
		//public bool isBackground;
		public int textDelay = 0;
		public int textFrontDelay = 0;
		public ConsoleColor color;
		public int PriorityLayer{get;set;}
		public bool isStream;
		public bool AlignH = false;
		public String Highlight{get;set;}
		public ConsoleColor HighlightColor{get;set;}
		
	
		
		public TextAndPosition(){
			text = "";
		}
		
		public TextAndPosition(String text,int x,int y):this(){
			this.text = text;
			this.x = x;
			this.y = y;
			isSelect = false;
			isStream = false;
		}
		
		public TextAndPosition(String text,int textDelay){
			this.text = text;
			this.textDelay = textDelay;
		}
		
		public TextAndPosition(String text,ConsoleColor color,int textDelay):this(text,textDelay){
			this.color = color;
		}
		
		public TextAndPosition(String text,int x,int y,bool isSelect):this(text,x,y){this.isSelect = isSelect;}
		
		public TextAndPosition(int textFrontDelay,String text,int x,int y):this(text,x,y){this.textFrontDelay = textFrontDelay;	}
		
		public TextAndPosition(String text,int x,int y,bool isSelect,ConsoleColor color):this(text,x,y,isSelect){this.color = color;}
		
		public TextAndPosition(String text,int x,int y,int textDelay):this(text,x,y){this.textDelay = textDelay;}
		
		public TextAndPosition(String text,int x,int y,int textDelay,ConsoleColor color):this(text,x,y,textDelay){this.color = color;}
		
		public TextAndPosition(int textFrontDelay,String text,int x,int y,int textDelay):this(text,x,y,textDelay){this.textFrontDelay = textFrontDelay;}
		
		public TextAndPosition(int textFrontDelay,String text,int x,int y,int textDelay,ConsoleColor color):this(text,x,y,textDelay,color){this.textFrontDelay = textFrontDelay;}
		
		public TextAndPosition(int textFrontDelay,String text,int x,int y,bool isSelect):this(textFrontDelay,text,x,y){this.isSelect = isSelect;}
		public TextAndPosition(int textFrontDelay,String text,int x,int y,bool isSelect,ConsoleColor color):this(textFrontDelay,text,x,y,isSelect){this.color = color;}
		
		public TextAndPosition(String text,int x,int y,bool isSelect,ConsoleColor color,int textDelay):this(text,x,y,isSelect,color){this.textDelay = textDelay;}
		
		public TextAndPosition Clone(){//추가된게 많아서 쓰려면 업데이트 하고 쓰기
			TextAndPosition clone = new TextAndPosition();
			clone.text = this.text;
			clone.x = this.x;
			clone.y = this.y;
			clone.isSelect = this.isSelect;
			clone.textDelay = this.textDelay;
			clone.color = this.color;
			clone.textFrontDelay = this.textFrontDelay;
			clone.PriorityLayer = this.PriorityLayer;
			return clone;
		}
		
	
		
		
		
		public int this[int index]{
			get{
				switch(index){
					case 0: return x;
					case 1: return y;
					default: return Define.ERROR;
				}
			}
			set{
				switch(index){
					case 0: x = value;
						break;
					case 1: y = value;
						break;
					default: throw new Exception();
				}
			}
		}
		
	}
	//////////////////////////////////////////////////////////////////////////////////////
	
	public static class GameManager{
		public static void SpawnMonster(Choice choice){ //몬스터 선택지가 중앙에 있는 기존 선택지 마지막 열 다음으로 들어가도록 해주는 메소드
			if(choice.MonsterList == null)
				return;
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
						selectList.Add(new TextAndPosition(monsterList[i].GetRandomSelectMessage().text,mPositionX,mPositionY++,true));
						choice.IndicateChoice.Add(selectList.Count-1,monsterList[i].Name);                            //배틀페이즈로 들어가는 선택지로 추가
						monsterList[i].IsSpawn = true;
					}
				}
			}
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
	
	public class DisplayTextGame{
		public List<TextAndPosition> selectList;       	//한 Choice의 선택지 리스트
		public List<TextAndPosition> onlyShowList;		//    "      읽기전용 리스트
		public List<TextAndPosition> integratedList;	//    "      선택지 선택 값 리스트(다음 choice)
		public List<TextAndPosition> streamList;		//    "      같은줄에 순차적으로 출력하는 텍스트 리스트
		public List<TextAndPosition> backgroundList;	//	  " 	 뒷배경 리스트
		public ChoiceType choiceType;					//선택지 타입
		
		String currentArrowingText;						//현재 선택된 선택지
		
		public int currentSelectNum;					//현재 선택된 선택지 번호
		public int delay = 0;							//문자출력딜레이
		public int delayBackup = 0;						//딜레이 변경후 이전 딜레이로 다시 돌아오기 위해 선언한 변수
		const String ARROW ="=>";						//선택 문자열 앞에 생성할 문자
		String voidARROW="  ";							//Arrow크기만큼 앞을 비운다
		
		public int globalPositionX = 20;				//화면 전체의 위치 x값
		public int globalPositionY = 3;					//y값
		
		TextAndPosition previousStream; 				//이전 스트림 텍스트
		int streamCount= 0;								//이전 문자를 지우기 위한 이전 문자열의 길이 값
		
		int countPoint = 0;								//한글자씩 출력하는 기능을 위한 count
		bool stopStart = false;							//   		"			변수
		TextAndPosition stopText = new TextAndPosition();//			"			변수
		
		public void Init(){
			selectList = new List<TextAndPosition>();
			onlyShowList = new List<TextAndPosition>();
			integratedList = new List<TextAndPosition>();
			streamList = new List<TextAndPosition>();
			backgroundList = new List<TextAndPosition>();
			currentArrowingText = null;
			countPoint = 0;
			currentSelectNum = 0;
			stopText = new TextAndPosition();
			streamCount = 0;
		}
		
		public DisplayTextGame(){
			selectList = new List<TextAndPosition>();
			onlyShowList = new List<TextAndPosition>();
			integratedList = new List<TextAndPosition>();
			streamList = new List<TextAndPosition>();
			backgroundList = new List<TextAndPosition>();
			currentSelectNum = 0;
			
		}
		
		Choice cho;
		public Choice Cho{
			get{
				return cho;
			}
			set{
				cho = value;
				GameManager.SpawnMonster(cho);
				selectList = cho.ChoiceText;
				onlyShowList = cho.OnlyShowText;
				choiceType = cho.ChoiceType;
				backgroundList = cho.BackgroundText;
				streamList = cho.StreamText;
				ListCombiner();
				BackgroundOverlap();
			}
		}
		
		public int SearchSamePosition(List<TextAndPosition> list,TextAndPosition text){
			for(int i = 0;i<list.Count;i++){
				if(list[i].y == text.y){
					if(list[i].x == text.x){
						return i;
					}
					else
						return Define.ERROR;
				}
				return Define.ERROR;
			}
			return Define.ERROR;
		}
		
		public void BackgroundOverlap(){ //나중에 제데로 중첩되게 구현 해보기로
			for(int i = 0;i<backgroundList.Count;i++){
				int temp = SearchSamePosition(integratedList,backgroundList[i]); //현재 리스트와 BackgroundText의 포지션이 겹치지 않으면 
				if(temp == Define.ERROR){
					integratedList.Add(backgroundList[i]); //BackgroundText추가
				}
			}
			integratedList = integratedList.OrderBy(x => x.PriorityLayer).ToList();
		}
		
		public void ListCombiner(){ //y값을 입력받아 출력되야하는 순서대로 통합하여 정렬한 리스트를 만든다
			if(selectList != null){
			foreach(TextAndPosition tp in selectList){
				if(tp.AlignH)
					AlignX(tp);
				integratedList.Add(tp);
			}
			}
			if(onlyShowList != null){
			foreach(TextAndPosition tp in onlyShowList){
				if(tp.AlignH)
					AlignX(tp);
				integratedList.Add(tp);
			}
			}
			if(streamList != null){
			foreach(TextAndPosition tp in streamList){
				if(tp.AlignH)
					AlignX(tp);
				integratedList.Add(tp);
			}
			}
			integratedList = integratedList.OrderBy(x => x.PriorityLayer).ToList();

		}
		
		public void AlignX(TextAndPosition tap){
			int length = tap.text.Length;
			int frontPadding = 0;
			int backPadding = Define.SCREEN_WIDTH;
			while(frontPadding < backPadding){
				backPadding = Define.SCREEN_WIDTH - (frontPadding++ + length);
			}
			
			tap.x = frontPadding - 5;
		}
		
		public void Show()
		{
			Console.Clear();
			ShowAllList();
		}
			
		public void ShowAllList(){
			if(choiceType == ChoiceType.CHOICE)
				PrintAlgorithm();
			if(choiceType == ChoiceType.BATTLE)
				Battle();
			if(choiceType == ChoiceType.QUICK)
				Quick();
			if(choiceType == ChoiceType.QUICKNEXT)
				QuickChoice();
		}
		
		public void Battle(){
				
		
		}
		
		public void Quick(){
			cho.QuickRun();
		}
		
		public void QuickChoice(){
			if(selectList != null || onlyShowList != null){
				ConsoleColor tempC;
			if(integratedList != null){
					for(int i = 0;i<integratedList.Count;i++){
						TextAndPosition TAndP = integratedList[i];
						
						if(TAndP.color!=null){
							tempC = Console.ForegroundColor;
							Console.ForegroundColor = TAndP.color;
						}
						if(TAndP.isStream){
							StreamPrint(TAndP);
						}
						else if(TAndP.textDelay > 0){ //텍스트의 딜레이 속성이 0보다 크면 한글자씩출력하게 함
							FrontDelay(TAndP);
							delay = TAndP.textDelay;
							PrintPieceOfText(TAndP);
							delay = 0;
						}
						else{ //stopPoint == 0조건 추가하면 순차적으로 출력된다음 다음 내용 출력. 없에면 한꺼번에 출력
							FrontDelay(TAndP);
							//Console.WriteLine(integratedList[i][0]+globalPositionX+":"+(integratedList[i][1]+globalPositionY));
							Console.SetCursorPosition(TAndP[0]+globalPositionX,TAndP[1]+globalPositionY);
							Console.Write(TAndP.text);//8.24 이게 생략되 있엇음.. 왜지..
						}
						Console.ForegroundColor = tempC;
						Thread.Sleep(delay);
					}
				delay = 0;
			}
			}
		}
		
		public void PrintAlgorithm(){
			ConsoleColor tempC;
			if(currentArrowingText == null && selectList != null) //choice 첫 호출때 화살표가 보이게 하기 위함
				currentArrowingText = selectList[0].text;
			if(integratedList != null){
					for(int i = 0;i<integratedList.Count;i++){
						TextAndPosition TAndP = integratedList[i];
						if(TAndP.color!=null){
							tempC = Console.ForegroundColor;
							Console.ForegroundColor = TAndP.color;
						}
						if(TAndP.isStream){
							StreamPrint(TAndP);
						}
						else if(TAndP.textDelay > 0){ //텍스트의 딜레이 속성이 0보다 크면 한글자씩출력하게 함
							FrontDelay(TAndP);
							delay = TAndP.textDelay;
							PrintPieceOfText(TAndP);
							delay = 0;
						}
						else{ //stopPoint == 0조건 추가하면 순차적으로 출력된다음 다음 내용 출력. 없에면 한꺼번에 출력
							FrontDelay(TAndP);//해당 텍스트를 딜레이시키고 딜레이 0으로 만듦
							//Console.WriteLine(integratedList[i][0]+globalPositionX+":"+(integratedList[i][1]+globalPositionY));
							Console.SetCursorPosition(TAndP[0]+globalPositionX,TAndP[1]+globalPositionY);
							if(TAndP.isSelect){
								int temp = selectList.FindIndex(x => x.text == TAndP.text);
								voidARROW = temp+1+".";
							}
							Console.Write(TextArrower(TAndP.text,TAndP.isSelect));
						}
						Console.ForegroundColor = tempC;
						Thread.Sleep(delay);
					}
				
				delay = 0;
			}
		}
		
		public void StreamPrint(TextAndPosition TAndP){
			FrontDelay(TAndP);
			delay = TAndP.textDelay;
			if(streamCount != 0){
				Console.SetCursorPosition(previousStream[0]+globalPositionX,previousStream[1]+globalPositionY);
				Console.Write(ReturnSpace(streamCount));
			}
			streamCount = TAndP.text.Length+GetKoreanCount(TAndP.text);
			PrintPieceOfText(TAndP);
			delay = 0;
			previousStream = TAndP;
		}
		
		public int GetKoreanCount(String text){
			int count = 0;
			for(int i = 0;i<text.Length;i++){
				if(char.GetUnicodeCategory(text[i])==System.Globalization.UnicodeCategory.OtherLetter){
					count++;
				}
			}
			return count;
		}
		
		public String ReturnSpace(int count){
			String temp = "";
			for(int i = 0;i<count;i++){
				temp += " ";
			}
			return temp;
		}
		
		/*public void PiecesAndChoice(){ //타자기 효과 출력과 일반 선택지 출력 메소드
			PrintAlgorithm();
		}*/
		
		
		public void PrintPieceOfText(TextAndPosition text){ //한글자씩 출력하게 하는 메소드
			if(countPoint == 0){
				stopText = text.Clone();  //출력할 텍스트의 위치등의 값 복사
				stopText.text ="";			//텍스트만 비우기
				delayBackup = delay;
			}
			
			for(countPoint = 0;countPoint < text.text.Length;countPoint++){
				stopText.text += text.text[countPoint];
				Console.SetCursorPosition(stopText[0]+globalPositionX,stopText[1]+globalPositionY);
				Console.Write(stopText.text);
				Thread.Sleep(delay);
			}
			
			text.textDelay = 0;
			countPoint = 0;
			delay = delayBackup;
		}
		
		public void FrontDelay(TextAndPosition TAndP){ 
			if(TAndP.textFrontDelay > 0){
				Thread.Sleep(TAndP.textFrontDelay);
				TAndP.textFrontDelay = 0;//한번 출력한 문자는 딜레이를 없엔다
			}
		}
		
		public void PrintListInfo(){
			for(int i = 0;i<integratedList.Count;i++){
				Console.WriteLine(i+":"+integratedList[i].text+" x:"+integratedList[i].x+" y:"+integratedList[i].y);
			}
		}
		/*
		public void SSList(){
			if(selectList != null){
				Console.WriteLine("call SSList");
				for(int i = 0;i<selectList.Count;i++){
						Console.SetCursorPosition(selectList[i][0],selectList[i][1]);
						Console.WriteLine(TextArrower(selectList[i].text));
				}
			}
		}
			
		public void SOSList(){
			if(onlyShowList != null){
				Console.WriteLine("call SOSList");
				for(int i = 0;i<onlyShowList.Count;i++){
						Console.SetCursorPosition(onlyShowList[i][0],onlyShowList[i][1]);
				}
			}
		}*/
		
		public void PrintLog(int x,int y,int text){
			Console.SetCursorPosition(x,y);
			Console.WriteLine(text);
		}
		
		public void PrintLog(int x,int y,String text){
			Console.SetCursorPosition(x,y);
			Console.WriteLine(text);
		}
			
		public String TextArrower(String s,bool isSelect){ //항상 currentArrowingText에 할당된 Text에 화살표를 붙인다
			if(isSelect){
				if(s.Equals(currentArrowingText))
					return ARROW+currentArrowingText;
				else
					return voidARROW+s;
			}
			return s;
		}
		
		public void SelectingText(ConsoleKeyInfo key){ //방향키를 누르면 currentArrowingText에 할당되는 Text를 순차적으로 바꾼다.
			
			int keyInt = 0;
			try{
				keyInt = int.Parse(key.KeyChar.ToString());
			}catch(Exception e){}
			
			if(currentSelectNum == null)	//한번만 초기화되게
				currentSelectNum = 0;
			if((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow) && currentSelectNum != 0){
				currentSelectNum--;
			}
			else if((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && currentSelectNum != selectList.Count-1){
				currentSelectNum++;
			}
			else if(keyInt <= selectList.Count && keyInt > 0){
				currentSelectNum = keyInt - 1;
			}
			if(selectList != null)
				currentArrowingText = selectList[currentSelectNum].text;
		}
		
		
	}
	///////////////////////////////////////////////////////////////////////////
	/*class TextSerialization{
		String TextSerialize(ShowInfo sif);
		ShowInfo TextDeserialize(String);
	}*/
	
	public class BattleSystem{
		
	}
	
	public class Choice //선택지 부여 하는 클래스
	{
		public int CTNum;      //ChoiceTextNumber
		public int OSTNum;		//OnlyShowTextNumber
		public int STNum;
		public int BGTNum;
		public String Name {get;set;}
		public delegate void Quick();
		public Quick QuickDelegate{get;set;} 
		
		
		
		public Choice(){
			CTNum = 0;
			OSTNum = 0;
			STNum = 0;
			BGTNum = 0;
			indicateChoice = new Dictionary<int,String>();
			backgroundText = new List<TextAndPosition>();
			choiceText = new List<TextAndPosition>();
			onlyShowText = new List<TextAndPosition>();
			streamText = new List<TextAndPosition>();
			choiceType = ChoiceType.CHOICE;
		}
		
		List<TextAndPosition> choiceText;
		public List<TextAndPosition> ChoiceText{
			get{
				return choiceText;
			}
			
			set{
				choiceText = value;
				SetCTNum();
			}
		}
		
		List<TextAndPosition> onlyShowText;
		public List<TextAndPosition> OnlyShowText{
			get{
				return onlyShowText;
			}
			set{
				onlyShowText = value;
				SetOSTNum();
			}
		}
		
		List<TextAndPosition> streamText;
		public List<TextAndPosition> StreamText{
			get{
				return streamText;
			}
			set{
				streamText = value;
				Streaming();
				SetSTNum();
			}
		}
		
		public void Streaming(){
				foreach(TextAndPosition text in streamText){
						text.isStream = true;
				}
			}
		
		List<TextAndPosition> backgroundText;
		public List<TextAndPosition> BackgroundText{
			get{
				return backgroundText;
			}
			set{
				backgroundText = value;
				SetBGTNum();
			}
		}
		
		List<TextAndPosition> returnText;
		public List<TextAndPosition> ReturnText{
			get{
				return returnText;
			}
			set{
				returnText = value;
			}
		}
		
		Dictionary<int,String> indicateChoice;
		public Dictionary<int,String> IndicateChoice{
			get{
				return indicateChoice;
			}
			set{
				indicateChoice = value;
			}
		}
		
		List<Monster> monsterList;
		public List<Monster> MonsterList{
			get{
				return monsterList;
			}
			set{
				monsterList = value;
			}
		}
		
		List<NPC> npcList;
		public List<NPC> NPCList{
			get{
				return npcList;
			}
			set{
				npcList = value;
			}
		}
		
		ChoiceType choiceType;
		public ChoiceType ChoiceType{
			get;
			set;
		}
		
		public String ChoiceNext(int num){ //다음 선택지를 indicateChoice에서 찾아 반환
			return indicateChoice[num];
		}
		
		
		void SetCTNum(){
			CTNum = ChoiceText.Count;
		}
		void SetOSTNum(){
			OSTNum = OnlyShowText.Count;
		}
		void SetSTNum(){
			STNum = StreamText.Count;
		}
		void SetBGTNum(){
			STNum = BackgroundText.Count;
		}
		
		
		public void Print(){
			Console.WriteLine(choiceText[0].text+CTNum);
		}
		
		public void QuickRun(){
			QuickDelegate();
		}
		
		public String QuickNext(){  //choiceType이 QUICKNEXT일때 빠르게 다음 선택지로 넘어갈때 DTG또는 Main에서 호출하는 함수
			//Console.ReadKey();
			return ChoiceNext(0);
		}
		
		public List<TextAndPosition> CopyList(List<TextAndPosition> list){
			List<TextAndPosition> copy = new List<TextAndPosition>();
			for(int i = 0;i<list.Count;i++){
				copy.Add(list[i].Clone());
			}
			return copy;
		}
		
		public void LeaveChoice(){
			if(ReturnText != null){
				OnlyShowText = ReturnText;
			}
		}
		
		public Choice Clone(){
			Choice clone = new Choice();
			clone.QuickDelegate = this.QuickDelegate;
			clone.ChoiceText = CopyList(ChoiceText);
			clone.OnlyShowText = CopyList(OnlyShowText);
			clone.StreamText = CopyList(StreamText);
			clone.BackgroundText = CopyList(BackgroundText);
			clone.IndicateChoice = this.IndicateChoice;
			clone.CTNum = this.CTNum;
			clone.OSTNum = this.OSTNum;
			clone.STNum = this.STNum;
			clone.BGTNum = this.BGTNum;
			clone.Name = this.Name;
			clone.ChoiceType = this.ChoiceType;
			return clone;
		}
	}
	
	////////////////////////////////////////////////////////////////
	
	public class ChoiceControler{
		public Dictionary<String,Choice> choiceDictionary; //선택지들을 담을 딕셔너리 클래스
		
		public ChoiceControler(){
			choiceDictionary = new Dictionary<String,Choice>();
		}
		
		public Choice this[String index]{
			get{
				return choiceDictionary[index];
			}
			set{
				choiceDictionary[index] = value;
			}
		}
		
		public Choice ChooseChoice(string choiceName){ //선택지를 골라낼 메소드
			return choiceDictionary[choiceName];
		}
		
		public void AddChoice(Choice choice){ //선택지를 추가할 메소드
			if(choiceDictionary.ContainsKey(choice.Name)){
				choiceDictionary[choice.Name] = choice;
			}else{
				choiceDictionary.Add(choice.Name,choice);
			}
		}
		
		public void AddScenario(Scenario scenario){ //선택지들의 뭉치인 시나리오를 통체로 추가할 메소드
			for(int i = 0;i<scenario.Count;i++){
				choiceDictionary.Add(scenario[i].Name,scenario[i]);
			}
		}
		
		public Choice SetChoice(String cName){
			return choiceDictionary[cName];
		}
		
		public Choice SetChoiceClone(String cName){
			return choiceDictionary[cName].Clone();
		}
		
	}
	
	public class Scenario{
		int clikX;
		int clikY;
		
		List<Choice> choices = new List<Choice>();
		public Choice this[int index]{
			get{
				return choices[index];
			}
		}
		public int Count{get;set;}
		
		public Scenario(){ //지금은 choice생성할때 ChoiceText,OnlyShowText중 하나라도 없으면 실행 불가
			Backgrounds backgrounds = new Backgrounds();
			CharacterList characterList = new CharacterList();
			
			clikX = 15;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "c1",
				ChoiceText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",13+clikX,5+clikY,true){PriorityLayer = 3},
							  new TextAndPosition(200,"Exit",13+clikX,6+clikY,true,ConsoleColor.Yellow){PriorityLayer = 4},
								new TextAndPosition(200,"옵션",13+clikX,7+clikY,true){PriorityLayer = 5}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("The Colorless",10+clikX,3+clikY,10,ConsoleColor.Green){PriorityLayer = 1},
							new TextAndPosition("개발:Peo",1,1,100){PriorityLayer = 2}},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"},{1,"test"},{2,"option"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			clikX = 20;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "testStream",
				ChoiceText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",10+clikX,5+clikY,true){PriorityLayer = 4},
							  new TextAndPosition(200,"Exit",10+clikX,6+clikY,true){PriorityLayer = 5},
								new TextAndPosition(200,"옵션",10+clikX,7+clikY,true){PriorityLayer = 6}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition(1000,"개발:Peo",1,1){PriorityLayer = 3}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("한 용사가 있었다.",5+clikX,3+clikY,30){PriorityLayer = 1},
							new TextAndPosition(1000,"하지만 그는 죽었다.",5+clikX,3+clikY,30,ConsoleColor.Red){PriorityLayer = 2}},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"},{1,"test"},{2,"option"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "option",
				ChoiceText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition(200,"Start",10,5,true){PriorityLayer = 2},
							  new TextAndPosition(200,"Exit",10,6,true){PriorityLayer = 3}},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("옵션은 없다.",5,3,30){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"},{1,"exit"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "exit",
				ChoiceType = ChoiceType.QUICK,
				QuickDelegate = ()=>{Environment.Exit(0);}
			});
			
			choices.Add(new Choice(){
				Name = "test",
				ChoiceType = ChoiceType.QUICKNEXT,
				ChoiceText = new List<TextAndPosition>()         
							{new TextAndPosition()},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("나 갈 수 없 다",5,3,100,ConsoleColor.Red)},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"}}
			});
			
			clikX = 15;
			clikY = 5;
			choices.Add(new Choice(){
				Name = "c2",
				ChoiceText = new List<TextAndPosition>()         //프로퍼티를 통한 초기화 생성자가 먼저 호출된다, 이게되네
							{new TextAndPosition("오른쪽",1,19,true),
							new TextAndPosition("가만히 있는다",7+clikX,8+clikY,true),
							new TextAndPosition("아앗.",7+clikX,9+clikY,true),
							new TextAndPosition("왼쪽",56,19,true)},
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("용사는 길을 나섰다.",7+clikX,3+clikY,1),
							new TextAndPosition("울창한 숲.",7+clikX,2+clikY,1)},
				ReturnText = new List<TextAndPosition>()
							{new TextAndPosition("여전히 울창한 숲.",7+clikX,2+clikY,1)},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2-right"},{1,"testStream"}},
				
				MonsterList = new List<Monster>()
							{characterList.GetMonster("슬라임"),
							characterList.GetMonster("뒤틀린 망자")},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			clikX = 23;
			clikY = 7;
			choices.Add(new Choice(){
				Name = "c2-right",
				ChoiceText = new List<TextAndPosition>()
							{new TextAndPosition("다시시작",12+clikX,7+clikY,true){PriorityLayer = 2}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("함정이였다.",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"You Died",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"QuickNext-test"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			choices.Add(new Choice(){
				Name = "QuickNext-test",
				ChoiceType = ChoiceType.QUICKNEXT,
				ChoiceText = new List<TextAndPosition>()
							{new TextAndPosition("다시시작",12+clikX,7+clikY,true){PriorityLayer = 2}},
				StreamText = new List<TextAndPosition>()
							{new TextAndPosition("함정이였다.",5+clikX,3+clikY,10){PriorityLayer = 1},
							new TextAndPosition(1000,"You Died",5+clikX,3+clikY,10,ConsoleColor.Red){PriorityLayer = 1}},
				IndicateChoice = new Dictionary<int,String>(){{0,"c2"}},
				BackgroundText = backgrounds.GetBackground(0)
			});
			
			Count = choices.Count;
		}
	}
	
	//┏ ┓ ━  ┛┗ ┃┣ ┳ ┫┻
		public class Backgrounds{
			public List<List<TextAndPosition>> background;
			public ConsoleColor color;
			public int width = Define.SCREEN_WIDTH;
			public int height = Define.SCREEN_HEIGHT;
			public Backgrounds(){
				background = new List<List<TextAndPosition>>();
				background.Add(new List<TextAndPosition>(new TextAndPosition[]{
					new TextAndPosition("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓",0,0),
					new TextAndPosition("┃",0,1),new TextAndPosition("┃",width,1),
					new TextAndPosition("┃",0,2),new TextAndPosition("┃",width,2),
					new TextAndPosition("┃",0,3),new TextAndPosition("┃",width,3),
					new TextAndPosition("┃",0,4),new TextAndPosition("┃",width,4),
					new TextAndPosition("┃",0,5),new TextAndPosition("┃",width,5),
					new TextAndPosition("┃",0,6),new TextAndPosition("┃",width,6),
					new TextAndPosition("┃",0,7),new TextAndPosition("┃",width,7),
					new TextAndPosition("┃",0,8),new TextAndPosition("┃",width,8),
					new TextAndPosition("┃",0,9),new TextAndPosition("┃",width,9),
					new TextAndPosition("┃",0,10),new TextAndPosition("┃",width,10),
					new TextAndPosition("┃",0,11),new TextAndPosition("┃",width,11),
					new TextAndPosition("┃",0,12),new TextAndPosition("┃",width,12),
					new TextAndPosition("┃",0,13),new TextAndPosition("┃",width,13),
					new TextAndPosition("┃",0,14),new TextAndPosition("┃",width,14),
					new TextAndPosition("┃",0,15),new TextAndPosition("┃",width,15),
					new TextAndPosition("┃",0,16),new TextAndPosition("┃",width,16),
					new TextAndPosition("┃",0,17),new TextAndPosition("┃",width,17),
					new TextAndPosition("┃",0,18),new TextAndPosition("┃",width,18),
					new TextAndPosition("┃",0,19),new TextAndPosition("┃",width,19),
					new TextAndPosition("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛",0,20)
				}));
				background.Add(new List<TextAndPosition>(new TextAndPosition[]{
					new TextAndPosition("┏━┳━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━┓",0,0),
					new TextAndPosition("┣",0,1),new TextAndPosition("━",1,1),new TextAndPosition("┛",2,1),new TextAndPosition("━",width-1,1),new TextAndPosition("┗",width-2,1),new TextAndPosition("┫",width,1),
					new TextAndPosition("┃",0,2),new TextAndPosition("┃",width,2),
					new TextAndPosition("┃",0,3),new TextAndPosition("┃",width,3),
					new TextAndPosition("┃",0,4),new TextAndPosition("┃",width,4),
					new TextAndPosition("┃",0,5),new TextAndPosition("┃",width,5),
					new TextAndPosition("┃",0,6),new TextAndPosition("┃",width,6),
					new TextAndPosition("┃",0,7),new TextAndPosition("┃",width,7),
					new TextAndPosition("┃",0,8),new TextAndPosition("┃",width,8),
					new TextAndPosition("┃",0,9),new TextAndPosition("┃",width,9),
					new TextAndPosition("┃",0,10),new TextAndPosition("┃",width,10),
					new TextAndPosition("┃",0,11),new TextAndPosition("┃",width,11),
					new TextAndPosition("┃",0,12),new TextAndPosition("┃",width,12),
					new TextAndPosition("┃",0,13),new TextAndPosition("┃",width,13),
					new TextAndPosition("┃",0,14),new TextAndPosition("┃",width,14),
					new TextAndPosition("┃",0,15),new TextAndPosition("┃",width,15),
					new TextAndPosition("┃",0,16),new TextAndPosition("┃",width,16),
					new TextAndPosition("┃",0,17),new TextAndPosition("┃",width,17),
					new TextAndPosition("┃",0,18),new TextAndPosition("┃",width,18),
					new TextAndPosition("┣",0,19),new TextAndPosition("━",1,19),new TextAndPosition("┓",2,19),new TextAndPosition("━",width-1,19),new TextAndPosition("┏",width-2,19),new TextAndPosition("┫",width,19),
					new TextAndPosition("┗━┻━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━┛",0,20)
				}));
				
				Coloring();
			}
			
			public List<TextAndPosition> this[int index]{
				get{
					return background[index];
				}
				set{
					background[index] = value;
				}
			}
			
			public List<TextAndPosition> GetBackground(int index){
				return background[index];
			}
			
			public void Backgrounding(){
				foreach(List<TextAndPosition> list in background){
					for(int i = 0;i<list.Count;i++){
						list[i].PriorityLayer = 0;
					}
				}
			}
			
			public void Coloring(){
				if(color != null)
				foreach(List<TextAndPosition> list in background){
					for(int i = 0;i<list.Count;i++){
						list[i].color = color;
					}
				}
			}
		}
}
	


/////////////////////////////////////////////////////////////////////

namespace Characters
{
	interface IDamageable{
		AttackInfo Attack();
		AttackInfo Damage(AttackInfo attack_info);
	}
	
	interface ICharacterState{
		string CurrentState();
	}
	
	public  class AttackInfo{
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
	
	public static class User
	{
		public static Player CurrentPlayer;
	}
	
	public class Character:IDamageable
	{
		public string Name{get;set;}
		public int Hp{get;set;}
		public int Mp{get;set;}
		public int MaxHp{get;set;}
		public int MaxMp{get;set;}
		public int AttackPower{get;set;}
		public int Defense{get;set;}
		
		public const int HIGH = 0;
		public const int MIDDLE = 1;
		public const int LOW = 2;
		public const int DIED = 3;
		
		public Character()
		{
			Name = "noNamed";
			Hp = 0;
			Mp = 0;
			AttackPower = 0;
			Defense = 0;
		}
		public Character(string name,int hp,int mp,int attack_power,int defense)
		{
			Name = name;
			Hp = hp;
			MaxHp = hp;
			Mp = mp;
			MaxMp = mp;
			AttackPower = attack_power;
			Defense = defense;
		}
		public void info()
		{
			Console.WriteLine("name:"+Name+"\nhp:"+Hp+"\nmp:"+Mp+"\nattack_power:"+AttackPower+"\ndefense:"+Defense);
		}
		virtual public AttackInfo Attack(){
		 	return new AttackInfo();
		}
		
		virtual public AttackInfo Damage(AttackInfo attack_info){
			return new AttackInfo();
		}
		
	}
	
	public class Player : Character,IDamageable,ICharacterState
	{
		
		public Player():base(){}
		public Player(string name,int hp,int mp,int attack_power,int defense):base(name,hp,mp,attack_power,defense){}
		
		override public AttackInfo Attack(){
			AttackInfo aInfo = new AttackInfo();
			aInfo.Final_damage = AttackPower;
		 	return aInfo;
		}
		
		override public AttackInfo Damage(AttackInfo attack_info){
			AttackInfo aInfo = attack_info;
			aInfo.CalDamage(Defense);
			Hp -= aInfo.Final_damage;
			return aInfo;
		}
		
		public string CurrentState(){
			return "";
		}
	}
	
	public class Monster : Character,IDamageable,ICharacterState
	{
		public int SpawnChance{get;set;}
		public bool IsSpawn{get;set;} = false;
		
		public List<TextAndPosition> SelectMessage{get;set;}
		public List<TextAndPosition> SpawnMessage{get;set;}
		public List<TextAndPosition> StateMessage{get;set;}
		public TextAndPosition CriticalMessage{get;set;}
	
		public Monster(){
			SelectMessage = new List<TextAndPosition>(){ new TextAndPosition(Name,10)};
			SpawnMessage = new List<TextAndPosition>(){new TextAndPosition(Name+"이다.",10)};
		}
		public Monster(string name,int hp,int mp,int attack_power,int defense):base(name,hp,mp,attack_power,defense){}
		
		override public AttackInfo Attack(){
			AttackInfo aInfo = new AttackInfo();
			aInfo.Final_damage = AttackPower;
		 	return aInfo;
		}
		
		override public AttackInfo Damage(AttackInfo attack_info){
			AttackInfo aInfo = attack_info;
			aInfo.CalDamage(Defense);
			Hp -= aInfo.Final_damage;
			return aInfo;
		}
		
		public TextAndPosition GetRandomSelectMessage(){
			Random rand = new Random();
			return SelectMessage[rand.Next(0,SelectMessage.Count)];
		}
		
		public TextAndPosition GetRandomSpawnMessage(){
			Random rand = new Random();
			return SpawnMessage[rand.Next(0,SpawnMessage.Count)];
		}
		
		public string CurrentState(){
			return StateMessage[HpState()].text;
		}
		
		public int HpState(){
			double hpPer = (Hp * 1.0 / MaxHp)*100;
			if(70 < hpPer)
				return HIGH;
			else if(30 < hpPer)
				return MIDDLE;
			else if(0 < hpPer)
				return LOW;
			else
				return DIED;
		}
	
	}
	
	public class NPC : Character,IDamageable,ICharacterState
	{
		public NPC(){}
		public NPC(string name,int hp,int mp,int attack_power,int defense):base(name,hp,mp,attack_power,defense){}
		
		override public AttackInfo Attack(){
			AttackInfo aInfo = new AttackInfo();
			aInfo.Final_damage = AttackPower;
		 	return aInfo;
		}
		
		override public AttackInfo Damage(AttackInfo attack_info){
			AttackInfo aInfo = attack_info;
			aInfo.CalDamage(Defense);
			Hp -= aInfo.Final_damage;
			return aInfo;
		}
		
		public string CurrentState(){
			return "";
		}
	
	}
	
	public class CharacterList{
		Dictionary<String,Player> PlayerList{get;set;}
		Dictionary<String,Monster> MonsterList{get;set;}
		Dictionary<String,NPC> NPCList{get;set;}
		Player player;
		Monster monster;
		NPC NPC;
		
		public CharacterList(){
			PlayerList = new Dictionary<String,Player>();
			MonsterList = new Dictionary<String,Monster>();
			NPCList = new Dictionary<String,NPC>();
			///////////////////Player/////////////////////////////
			player = new Player(){
				Name = "용사",
				Hp = 10,
				Mp = 10,
				AttackPower = 5,
				Defense = 0
			};
			PlayerList.Add(player.Name,player);
			
			
			///////////////////Monster/////////////////////////////
			monster = new Monster(){
				Name = "슬라임",
				MaxHp = 10,
				Hp = 10,
				Mp = 0,
				AttackPower = 1,
				Defense = 0,
				SpawnChance = 70,
				SelectMessage = new List<TextAndPosition> 
				{new TextAndPosition("어 슬라임이다.",10),
				new TextAndPosition("어 저건 뭐지?",10),
				new TextAndPosition("슬라임.",10)},
				SpawnMessage = new List<TextAndPosition>()
				{new TextAndPosition("반투명한 초록빛깔, 바닥에 눌러붙은 듯한 탱글탱글한 점액질",10),						   
				 new TextAndPosition("그렇다, 슬라임이다.",10)},
				StateMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("탱글 탱글 함을 과시하고 있다.",10),
					new TextAndPosition("흐믈 흐믈 거리기 시작했다.",10),
					new TextAndPosition("형체를 유지하기 힘들어 보인다.",10),
					new TextAndPosition("슬라임이 형체를 잃고 땅속으로 스며들었다.",10)
				}
				
			};
			MonsterList.Add(monster.Name,monster);
			
			monster = new Monster(){
				Name = "뒤틀린 망자",
				MaxHp = 10,
				Hp = 10,
				Mp = 0,
				AttackPower = 3,
				Defense = 2,
				SpawnChance = 50,
				SelectMessage = new List<TextAndPosition> 
				{new TextAndPosition("사람인가?",10),
				new TextAndPosition("망자다.",10),
				new TextAndPosition("뭐지?",10)},
				SpawnMessage = new List<TextAndPosition>()
				{new TextAndPosition("초점 없는 눈동자,",10),						   
				 new TextAndPosition("색을 잃고 영혼마저 잃어버린 망자다.",10)},
				StateMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("나를 향해 미친듯이 달려온다.",10),
					new TextAndPosition("절뚝거리며 달려오는 망자.",10),
					new TextAndPosition("곧 쓰러질것 같다.",10),
					new TextAndPosition("망자는 사자가 되었다.",10)
				}
				
			};
			MonsterList.Add(monster.Name,monster);
			
			///////////////////NPC/////////////////////////////
		}
		
		public Monster GetMonster(String name){
			try{
				return MonsterList[name];
			}catch(Exception e){
				return null;
			}
		}
		
		public Player GetPlayer(String name){
			return PlayerList[name];
		}
		
		/*public Character GetCharacter(String Name){
			return characterList[Name];
		}*/
	}
}
