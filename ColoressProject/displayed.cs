using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game;
using static Convenience;



public class TextAndPosition : ICloneable{
		public String text{set;get;}	//나타낼 텍스트
		public int x{set;get;}			//위치 좌표
		public int y{set;get;}			
		public bool isSelect;			//이게 선택지인지
		//public bool isBackground;
		public int textDelay = 0;		//각 글자 사이의 텀
		public int textFrontDelay = 0;	//문장 앞에 텀
		public ConsoleColor color;		//문장색
		public int PriorityLayer{get;set;}//화면에 나타낼 우선순위, 낮을수록 더 높은 우선순위이다
		public bool isStream;			//이 텍스트가 Stream인지 
		public bool AlignH = false;		//텍스트 중앙 정렬 여부
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
		
		protected TextAndPosition(TextAndPosition that){//추가된게 많아서 쓰려면 업데이트 하고 쓰기 //2021.8.26 업뎃 완료
			this.text = that.text;
			this.x = that.x;
			this.y = that.y;
			this.isSelect = that.isSelect;
			this.textDelay = that.textDelay;
			this.color = that.color;
			this.textFrontDelay = that.textFrontDelay;
			this.PriorityLayer = that.PriorityLayer;
			this.isStream = that.isStream;
			this.AlignH = that.AlignH;
			this.Highlight = that.Highlight;
			this.HighlightColor = that.HighlightColor;
		}
		
		public Object Clone(){
			return new TextAndPosition(this);
		}
		
		public override string ToString(){
			return text;
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

public class DisplayTextGame{
		public List<TextAndPosition> selectList;       	//한 Choice의 선택지 리스트
		public List<TextAndPosition> onlyShowList;		//    "      읽기전용 리스트
		public List<TextAndPosition> integratedList;	//    "      선택지 선택 값 리스트(다음 choice)
		public List<TextAndPosition> streamList;		//    "      같은줄에 순차적으로 출력하는 텍스트 리스트
		public List<TextAndPosition> backgroundList;	//	  " 	 뒷배경 리스트
		public ChoiceType choiceType;					//선택지 타입
		
		String currentArrowingText;						//현재 선택된 선택지
		
		public int currentSelectNum = 0;					//현재 선택된 선택지 번호
		public int delay = 0;							//문자출력딜레이
		public int delayBackup = 0;						//딜레이 변경후 이전 딜레이로 다시 돌아오기 위해 선언한 변수
		const String ARROW ="=>";						//선택 문자열 앞에 생성할 문자
		String voidARROW="  ";							//Arrow크기만큼 앞을 비운다
		
		public int GlobalPositionX{get;set;} = 60;				//화면 전체의 위치 x값
		public int GlobalPositionY{get;set;} = 3;					//y값
		
		TextAndPosition previousStream; 				//이전 스트림 텍스트
		int streamCount= 0;								//이전 문자를 지우기 위한 이전 문자열의 길이 값
		
		int countPoint = 0;								//한글자씩 출력하는 기능을 위한 count
		bool stopStart = false;							//   		"			변수
		TextAndPosition stopText = new TextAndPosition();//			"			변수
	
		bool isClear = true;							//화면을 클리어하고 다음 화면을 출력하는지에 대한 변수
		
		public void Init(){
			selectList = new List<TextAndPosition>();
			onlyShowList = new List<TextAndPosition>();
			integratedList = new List<TextAndPosition>();
			streamList = new List<TextAndPosition>();
			backgroundList = new List<TextAndPosition>();
			currentArrowingText = null;
			
			currentSelectNum = 0;
			delay = 0;							//문자출력딜레이
			delayBackup = 0;
			
			previousStream = new TextAndPosition(); 				
			streamCount = 0;
			
			countPoint = 0;
			stopStart = false;	
			stopText = new TextAndPosition();
		}
		
		public DisplayTextGame(){
			selectList = new List<TextAndPosition>();
			onlyShowList = new List<TextAndPosition>();
			integratedList = new List<TextAndPosition>();
			streamList = new List<TextAndPosition>();
			backgroundList = new List<TextAndPosition>();
		}
	
		public DisplayTextGame(bool isClear):this(){
			this.isClear = isClear;
		}
		
		Choice cho;
		public Choice Cho{
			get{
				return cho;
			}
			set{
				cho = value;
				choiceType = cho.ChoiceType;
				InputLists();
				ListCombiner();
				BackgroundOverlap();
			}
		}
		
		public void InputLists(){
			selectList = cho.SelectText;
			onlyShowList = cho.OnlyShowText;
			backgroundList = cho.BackgroundText;
			streamList = cho.StreamText;
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
					if(tp.AlignH){ AlignX(tp); }
					integratedList.Add(tp);
				}
			}
			if(onlyShowList != null){
				foreach(TextAndPosition tp in onlyShowList){
					if(tp.AlignH){ AlignX(tp); }
					integratedList.Add(tp);
				}
			}
			if(streamList != null){
				foreach(TextAndPosition tp in streamList){
					if(tp.AlignH){ AlignX(tp); }
					integratedList.Add(tp);
				}
			}
			integratedList = integratedList.OrderBy(x => x.PriorityLayer).ToList();
		}
		
		public void AlignX(TextAndPosition TAndP){ // [frontPadding[----text----]backPadding] 중앙정렬
			int length = TAndP.text.Length;
			int frontPadding = 0;
			int backPadding = Define.SCREEN_WIDTH;
			while(frontPadding < backPadding){
				backPadding = Define.SCREEN_WIDTH - (frontPadding++ + length);
			}
			TAndP.x = frontPadding - 5;
		}
		
		public void Show()
		{
			if(isClear) {Console.Clear();}
			ShowAllList();
		}
			
		public void ShowAllList(){
			if(choiceType == ChoiceType.NORMAL)
				Normal();
			if(choiceType == ChoiceType.BATTLE)
				Battle();
			if(choiceType == ChoiceType.QUICK)
				Quick();
			if(choiceType == ChoiceType.QUICKNEXT)
				QuickChoice();
			if(choiceType == ChoiceType.EXPLAN)
				PrintWriteLine();
		}
	
		public void Normal(){
			PrintAlgorithm();
		}
		
		public void Battle(){
		}
		
		public void Quick(){
			cho.QuickRun();
		}
		
		public void QuickChoice(){
			if(onlyShowList != null){
				PrintAlgorithm();
			}
		}
	
		
		
		public void PrintAlgorithm(){
			ConsoleColor tempC;
			if(currentArrowingText == null && selectList != null && choiceType != ChoiceType.QUICKNEXT) //choice 첫 호출때 화살표가 보이게 하기 위함
			if(!isEmptyList<TextAndPosition>(selectList))
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
								Console.SetCursorPosition(TAndP[0]+GlobalPositionX,TAndP[1]+GlobalPositionY);
								if(!isEmptyList<TextAndPosition>(selectList)){
									if(TAndP.isSelect && choiceType != ChoiceType.QUICKNEXT){
										int temp = selectList.FindIndex(x => x.text == TAndP.text);
										voidARROW = temp+1+".";
									}
								}
								Console.Write(TextArrower(TAndP.text,TAndP.isSelect));
							}
							Console.ForegroundColor = tempC;
							Thread.Sleep(delay);
						}
					delay = 0;
				}
		}
	
		public void PrintWriteLine(){
			ConsoleColor tempC;
				if(integratedList != null){
						for(int i = 0;i<integratedList.Count;i++){
							TextAndPosition TAndP = integratedList[i];
							if(TAndP.color!=null){
								tempC = Console.ForegroundColor;
								Console.ForegroundColor = TAndP.color;
							}
							 //stopPoint == 0조건 추가하면 순차적으로 출력된다음 다음 내용 출력. 없에면 한꺼번에 출력
								FrontDelay(TAndP);//해당 텍스트를 딜레이시키고 딜레이 0으로 만듦
								int y = 0;
								int x = 0;
								Console.SetCursorPosition(TAndP[0]+GlobalPositionX,TAndP[1]+GlobalPositionY);
								String ts = "";
								for(int j = 0;j<TAndP.text.Length;j++){
									char c = TAndP.text[j];
									if(c == '\n'){
										y++;
										ts="";
									}else{
										Console.SetCursorPosition(TAndP[0]+GlobalPositionX+x,TAndP[1]+GlobalPositionY+y);
										ts+=c;
										Console.Write(ts);
										
									}
								}
								
							
						}
					delay = 0;
				}
		}
		
		public void StreamPrint(TextAndPosition TAndP){
			FrontDelay(TAndP);
			delay = TAndP.textDelay;
			if(streamCount != 0){
				Console.SetCursorPosition(previousStream[0]+GlobalPositionX,previousStream[1]+GlobalPositionY);
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
				stopText = (TextAndPosition)text.Clone();  //출력할 텍스트의 위치등의 값 복사
				stopText.text ="";			//텍스트만 비우기
				delayBackup = delay;
			}
			
			for(countPoint = 0;countPoint < text.text.Length;countPoint++){
				stopText.text += text.text[countPoint];
				Console.SetCursorPosition(stopText[0]+GlobalPositionX,stopText[1]+GlobalPositionY);
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
			
		public String TextArrower(String s,bool isSelect){ //항상 currentArrowingText에 할당된 Text에 화살표를 붙인다
			if(isSelect && choiceType != ChoiceType.QUICKNEXT){
				if(s.Equals(currentArrowingText))			//매개변수 s가 현재 선택된 Text면 화살표를 붙이고 아니면 화살표 없이 반환
					return ARROW+currentArrowingText;
				else
					return voidARROW+s;
			}
			return s;										//선택 가능한 text가 아니면 그냥 바로 반환
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