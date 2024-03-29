using System;
using System.Collections.Generic;
using Game;
using System.IO;
using MyJson;
using static Convenience;
using static GameWindows;
using static ItemData;


namespace Characters
{
	interface IDamageable
	{
		AttackInfo Attack();
		AttackInfo Damage(AttackInfo attack_info);
	}
	
	interface ICharacterState
	{
		string CurrentState();
	}
	
	
	
	
	
	public static class User
	{
		public static Player CurrentPlayer;
	}
	
	public class Character:IDamageable,ICloneable
	{
		public string Name{get;set;}
		public int Hp{get;set;}
		public int Mp{get;set;}
		public int MaxHp{get;set;}
		public int MaxMp{get;set;}
		public int AttackPower{get;set;}
		public int Defense{get;set;}
		public int AttackSpeed{get;set;}
		
		public AttackInfo attackInfo;
		
		public const int HIGH = 0;
		public const int MIDDLE = 1;
		public const int LOW = 2;
		public const int DIED = 3;
		public const int WEAK_POWER = 0;
		public const int NOMAL_POWER = 1;
		public const int SUPER_POWER = 2;
		
		
		public Character()
		{
			Name = "noNamed";
			Hp = 0;
			Mp = 0;
			AttackPower = 0;
			Defense = 0;
			AttackSpeed = 0;
		}
		public Character(string name,int hp,int mp,int attack_power,int defense,int attack_speed)
		{
			Name = name;
			Hp = hp;
			MaxHp = hp;
			Mp = mp;
			MaxMp = mp;
			AttackPower = attack_power;
			Defense = defense;
			AttackSpeed = attack_speed;
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
		
		protected Character(Character that){
			this.Name = that.Name ;
			this.Hp = that.Hp ;
			this.MaxHp = that.MaxHp ;
			this.Mp = that.Mp ;
			this.MaxMp = that.MaxMp ;
			this.AttackPower = that.AttackPower ;
			this.Defense = that.Defense ;
			this.AttackSpeed = that.AttackSpeed;
		}
		
		virtual public Object Clone(){
			return new Character(this);
		}
		
	}
	
	public class Player : Character,IDamageable,ICharacterState,ISaveToJson
	{
		public Inventory inven = new Inventory();
		
		public List<Quest> QuestList{get;set;} = new List<Quest>();
	
		Weapon weapon;
		public Weapon Weapon
		{
			get
			{
				return weapon;
			}
			set
			{
				if(weapon == null)
				{
					weapon = value;
					weapon.IsEquip = true;
					AttackMessage = weapon.AttackMessage;
					AttackSpeed += weapon.AttackSpeed;
				}
				else
				{
					if(weapon == value)
					{
						if(ConfirmWindow("이미 장착된 무기 입니다.\n해제하시겠습니까?",24,7))
						{
							AttackSpeed -= weapon.AttackSpeed;
							weapon.IsEquip = false;
							weapon = null;
							AttackMessage = null;
						}
					}
					else
					{
						AlertWindow("이미 무기가 존재합니다.",textXPos:24,textYPos:5);
					}
				}
			}
		}
		Armor armor;
		public Armor Armor
		{
			get
			{
				return armor;
			}
			set
			{
				if(armor == null)
				{
					armor = value;
					armor.IsEquip = true;
					Defense += armor.Defense;
				}
				else
				{
					if(armor == value){
						if(ConfirmWindow("이미 장착된 방어구 입니다.\n해제하시겠습니까?",24,7))
						{
							armor.IsEquip = false;
							Defense -= armor.Defense;
							armor = null;
						}
						else
						{
							AlertWindow("이미 방어구가 존재합니다.",textXPos:24,textYPos:5);
						}
					}
				}
			}
		}
		
		List<TextAndPosition> attackMessage= new List<TextAndPosition>(){
						new TextAndPosition("나는 오른쪽 주먹을 힘껏 내질렀다! ",10),
						new TextAndPosition("나는 날라차기를 시도했다!",10),
						new TextAndPosition("나는 마구잡이로 팔을 마구마구 돌렸다!",10)
						};
						
		public List<TextAndPosition> AttackMessage{
			get{
				return attackMessage;
			}
			set{
				attackMessage = value;
				if(attackMessage == null){
					attackMessage = new List<TextAndPosition>(){
						new TextAndPosition("나는 오른쪽 주먹을 힘껏 내질렀다! ",10),
						new TextAndPosition("나는 날라차기를 시도했다!",10),
						new TextAndPosition("나는 마구잡이로 팔을 마구마구 돌렸다!",10)
					};
				}
			}
		}
		
		List<TextAndPosition> reactionMessage= new List<TextAndPosition>(){
						new TextAndPosition("  ",10),
						new TextAndPosition(" ",10),
						new TextAndPosition(" ",10)
						};
						
		public List<TextAndPosition> ReactionMessage{
			get{
				return reactionMessage;
			}
			set{
				reactionMessage = value;
				if(reactionMessage == null){
					reactionMessage = new List<TextAndPosition>(){
						new TextAndPosition("맞은..건가..?",10),
						new TextAndPosition("맞은데가 얼얼하다.",10),
						new TextAndPosition("한참을 날아가 구르다가 겨우 멈춰섰다.",10),
						new TextAndPosition("따로 노는 내 몸을 보며 그대로 의식을 잃었다.",10)
					};
				}
			}
		} 
		
		public String AttackCry(){
			Random random = new Random();
			return AttackMessage[random.Next(0,AttackMessage.Count)].text;
		}
		
		public string Reaction(){
			Random rand = new Random();
			return ReactionMessage[PowerGap()].text;
		}
		
		public int PowerGap(){
			double finalDamage = attackInfo.FinalDamage;
			double hp = attackInfo.HpBeforeAttack;
		
			double powerGap = (finalDamage / hp)*100;
			
			
			//testLog("finalDamage: "+finalDamage);
			//testLog("hp: "+hp);
			//testLog("powerGap: "+powerGap);
			
			if(100 <= powerGap || powerGap < 0)
				return DIED;
			else if(50 <= powerGap)
				return SUPER_POWER;
			else if(20 <= powerGap)
				return NOMAL_POWER;
			else
				return WEAK_POWER;
		}
		
		public Player():base(){}
		public Player(string name,int hp,int mp,int attack_power,int defense,int attack_speed):base(name,hp,mp,attack_power,defense,attack_speed){}
		
		override public AttackInfo Attack(){
			AttackInfo aInfo = new AttackInfo();
			aInfo.FinalDamage = Weapon == null ? AttackPower : (AttackPower + Weapon.AttackPower);
		 	return aInfo;
		}
		
		override public AttackInfo Damage(AttackInfo attackInfo){
			AttackInfo aInfo = attackInfo;
			aInfo.CalDamage(Armor == null ? Defense : (Defense + Armor.Defense));
			aInfo.HpBeforeAttack = Hp; //데미지 계산전의 hp정보를 담는다
			Hp -= aInfo.FinalDamage;
			this.attackInfo = aInfo;
			return aInfo;
		}
		
		public void PlayerQuestCheck(Object info){
			for(int i = 0;i<QuestList.Count;i++){
				QuestList[i].CheckTarget(info);
			}
		}
		
		public string CurrentState(){
			return "";
		}
		
		public String ToJsonString(){
			Json json = new Json();
			json.OpenObject(Name);
			json.AddItem("Name",Name);
			json.AddItem("Hp",Hp);
			json.AddItem("Mp",Mp);
			json.AddItem("MaxHp",MaxHp);
			json.AddItem("MaxMp",MaxMp);
			json.AddItem("AttackPower",AttackPower);
			json.AddItem("Defense",Defense);
			json.AddItem("AttackSpeed",AttackSpeed);
			
			json.AddJsonAbleList<TextAndPosition>("ReactionMessage",ReactionMessage,true);
			json.AddJsonAbleList<TextAndPosition>("AttackMessage",AttackMessage,true);
			
			json.AddJsonAbleList("QuestList",QuestList,true);
			json.AddJsonAbleObject(inven);
			json.AddJsonAbleObject(Weapon);
			json.AddJsonAbleObject(Armor);

			json.CloseObject();
			return json.JsonString;
		}
		
		public void JsonToObject(String jsonString){
			Json json = new Json();
			json.JsonString = jsonString;
			Console.WriteLine(jsonString);
			this.Name = json.GetItem("Name");
			this.Hp = int.Parse(json.GetItem("Hp"));
			this.Mp = int.Parse(json.GetItem("Mp"));
			this.MaxHp = int.Parse(json.GetItem("MaxHp"));
			this.MaxMp = int.Parse(json.GetItem("MaxMp"));
			this.AttackPower = int.Parse(json.GetItem("AttackPower"));
			this.Defense = int.Parse(json.GetItem("Defense"));
			this.AttackSpeed = int.Parse(json.GetItem("AttackSpeed"));
		
			this.ReactionMessage = json.GetJsonAbleList<TextAndPosition>("ReactionMessage");
			this.AttackMessage = json.GetJsonAbleList<TextAndPosition>("AttackMessage");
			
			this.QuestList = json.GetJsonAbleList<Quest>("QuestList");
			
			this.inven = json.GetJsonAbleObject<Inventory>(inven.ToString());
			this.Weapon = json.GetJsonAbleObject<Weapon>("Weapon");
			this.Armor = json.GetJsonAbleObject<Armor>("Armor");
		}
		
		//protected Player(Player that):base(that){}
		
		/*public Object Clone(){
			return new Player(this);
		}*/
	}
	
	//*Enemy클래스 수정시 주의사항*
	//몬스터와 상호작용으로 발생하는 메세지들은 ~~Message로 명명한다
	//Message류의 필드를 추가할때는 
	//1.getter,setter 만들기 2.복사생성자 안에서 초기화해준후 copy구문 추가시키기
	//를 반드시 해준다.
	public class Enemy : Character,IDamageable,ICharacterState,ISaveToJson
	{
		public int SpawnChance{get;set;}
		public bool IsSpawn{get;set;} = false; 			//필드에 몬스터가 이미 있는지 확인하기 위한 변수
		public bool IsSpawnOnce{get;set;} = false;		//이 몬스터가 한번만 스폰되는 몬스터인지를 나타내는 변수
		
		
		public String DeathEvent{get;set;} = null;
		
		
		public List<TextAndPosition> SelectMessage{get;set;}
		public List<TextAndPosition> SpawnMessage{get;set;}
		public List<TextAndPosition> StateMessage{get;set;}
		public List<TextAndPosition> BlockMessage{get;set;}
		public List<TextAndPosition> DodgeMessage{get;set;}
		public List<TextAndPosition> ReactionMessage{get;set;}
		public List<TextAndPosition> PlayerReactionMessage{get;set;}
		public List<TextAndPosition> PreAttackMessage{get;set;}
		public List<TextAndPosition> AttackMessage{get;set;}
		
		
		public List<Item> DropItems{get;set;}
	
		public Enemy(){
			SelectMessage = new List<TextAndPosition>(){ new TextAndPosition(Name,10)};
			SpawnMessage = new List<TextAndPosition>(){new TextAndPosition(Name+"이다.",10)};
			BlockMessage = new List<TextAndPosition>(){ new TextAndPosition("막기성공",10),new TextAndPosition("막기실패",10)};
			DodgeMessage = new List<TextAndPosition>(){new TextAndPosition("회피성공",10),new TextAndPosition("회피실패",10)};
			PlayerReactionMessage = new List<TextAndPosition>(){new TextAndPosition("1",10),new TextAndPosition("2",10)};
			ReactionMessage = new List<TextAndPosition>(){new TextAndPosition(Name+" 팅겨나갔다.",10)};
			PreAttackMessage = new List<TextAndPosition>(){new TextAndPosition(Name+"은 빨랐다.",10)};
			AttackMessage = new List<TextAndPosition>(){new TextAndPosition(Name+"공격!",10)};
			DropItems = new List<Item>();
		}
		
		public Enemy(string name,int hp,int mp,int attack_power,int defense,int attack_speed):base(name,hp,mp,attack_power,defense,attack_speed){}
		
		override public AttackInfo Attack(){
			AttackInfo aInfo = new AttackInfo();
			aInfo.FinalDamage = AttackPower;
		 	return aInfo;
		}
		
		override public AttackInfo Damage(AttackInfo attackInfo){
			AttackInfo aInfo = attackInfo;
			aInfo.CalDamage(Defense);
			aInfo.HpBeforeAttack = Hp; //데미지 계산전의 hp정보를 담는다
			Hp -= aInfo.FinalDamage;
			this.attackInfo = aInfo;
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
		
		public string BlockSuccess(){
			return BlockMessage[0].text;
		}
		
		public string BlockFail(){
			return BlockMessage[1].text;
		}
		public string DodgeSuccess(){
			return DodgeMessage[0].text;
		}
		public string DodgeFail(){
			return DodgeMessage[1].text;
		}
		
		public String PreAttackSymptom(){
			Random random = new Random();
			return PreAttackMessage[random.Next(0,PreAttackMessage.Count)].text;
		}
		
		public String AttackCry(){
			Random random = new Random();
			return AttackMessage[random.Next(0,AttackMessage.Count)].text;
		}
		
		public string Reaction(){
			Random rand = new Random();
			return ReactionMessage[PowerGap()].text;
		}
		
		public int PowerGap(){
			double finalDamage = attackInfo.FinalDamage;
			double hp = attackInfo.HpBeforeAttack;
			
			double powerGap = (finalDamage / hp)*100;
			if(100 <= powerGap)
				return DIED;
			else if(50 <= powerGap)
				return SUPER_POWER;
			else if(20 <= powerGap)
				return NOMAL_POWER;
			else
				return WEAK_POWER;
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
		
		public List<Item> ItemDrop(){
			Random rand = new Random();
			List<Item> tList = new List<Item>();
			foreach(Item i in DropItems){
				if(rand.Next(1,101) < i.DropChance){
					tList.Add(i);
				}
			}
			return tList;
		}
		
		protected Enemy(Enemy that):base(that){
			this.SpawnChance = that.SpawnChance;
			this.IsSpawn = that.IsSpawn;
			this.IsSpawnOnce = that.IsSpawnOnce;
			this.SelectMessage = new List<TextAndPosition>();
			this.SpawnMessage = new List<TextAndPosition>();
			this.StateMessage = new List<TextAndPosition>();
			this.BlockMessage = new List<TextAndPosition>();
			this.DodgeMessage = new List<TextAndPosition>();
			this.ReactionMessage = new List<TextAndPosition>();
			this.PlayerReactionMessage = new List<TextAndPosition>();
			this.PreAttackMessage = new List<TextAndPosition>();
			this.AttackMessage = new List<TextAndPosition>();
			
			this.DeathEvent = that.DeathEvent;
			
			this.DropItems = that.DropItems;
			
			if(SelectMessage != null)
				this.SelectMessage = that.SelectMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(SpawnMessage != null)
				this.SpawnMessage = that.SpawnMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(StateMessage != null)
				this.StateMessage = that.StateMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(BlockMessage != null)
				this.BlockMessage = that.BlockMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(DodgeMessage != null)
				this.DodgeMessage = that.DodgeMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(ReactionMessage != null)
				this.ReactionMessage = that.ReactionMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(PlayerReactionMessage != null)
				this.PlayerReactionMessage = that.PlayerReactionMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(PreAttackMessage != null)
				this.PreAttackMessage = that.PreAttackMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(AttackMessage != null)
				this.AttackMessage = that.AttackMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
		}
		
		override public Object Clone(){
			return new Enemy(this);
		}
		
		public void EnemyInfo(){
			foreach(TextAndPosition m in SelectMessage){
				Console.WriteLine(m.text);
			}
			foreach(TextAndPosition m in SpawnMessage){
				Console.WriteLine(m.text);
			}
			foreach(TextAndPosition m in StateMessage){
				Console.WriteLine(m.text);
			}
		}
		// public string Name{get;set;}
		// public int Hp{get;set;}
		// public int Mp{get;set;}
		// public int MaxHp{get;set;}
		// public int MaxMp{get;set;}
		// public int AttackPower{get;set;}
		// public int Defense{get;set;}
		// public int AttackSpeed{get;set;}
		// public int SpawnChance{get;set;}
		// public bool IsSpawn{get;set;} = false; 			//필드에 몬스터가 이미 있는지 확인하기 위한 변수
		// public bool IsSpawnOnce{get;set;} = false;		//이 몬스터가 한번만 스폰되는 몬스터인지를 나타내는 변수
		
		
		// public Action DeathEvent{get;set;} = null;
		
		
		// public List<TextAndPosition> SelectMessage{get;set;}
		// public List<TextAndPosition> SpawnMessage{get;set;}
		// public List<TextAndPosition> StateMessage{get;set;}
		// public List<TextAndPosition> BlockMessage{get;set;}
		// public List<TextAndPosition> DodgeMessage{get;set;}
		// public List<TextAndPosition> ReactionMessage{get;set;}
		// public List<TextAndPosition> PlayerReactionMessage{get;set;}
		// public List<TextAndPosition> PreAttackMessage{get;set;}
		// public List<TextAndPosition> AttackMessage{get;set;}
		
		public String ToJsonString(){
			Json json = new Json();
			json.OpenObject(Name);
			json.AddItem("Name",Name);
			json.AddItem("Hp",Hp);
			json.AddItem("Mp",Mp);
			json.AddItem("MaxHp",MaxHp);
			json.AddItem("MaxMp",MaxMp);
			json.AddItem("AttackPower",AttackPower);
			json.AddItem("Defense",Defense);
			json.AddItem("AttackSpeed",AttackSpeed);
			json.AddItem("SpawnChance",SpawnChance);
			json.AddItem("IsSpawn",IsSpawn);
			json.AddItem("IsSpawnOnce",IsSpawnOnce);
			
			json.AddItem("DeathEvent",DeathEvent);
			
			json.AddJsonAbleList<TextAndPosition>("SelectMessage",SelectMessage,true);
			json.AddJsonAbleList<TextAndPosition>("SpawnMessage",SpawnMessage,true);
			json.AddJsonAbleList<TextAndPosition>("StateMessage",StateMessage,true);
			json.AddJsonAbleList<TextAndPosition>("BlockMessage",BlockMessage,true);
			json.AddJsonAbleList<TextAndPosition>("DodgeMessage",DodgeMessage,true);
			json.AddJsonAbleList<TextAndPosition>("ReactionMessage",ReactionMessage,true);
			json.AddJsonAbleList<TextAndPosition>("PlayerReactionMessage",PlayerReactionMessage,true);
			json.AddJsonAbleList<TextAndPosition>("PreAttackMessage",PreAttackMessage,true);
			json.AddJsonAbleList<TextAndPosition>("AttackMessage",AttackMessage,true);

			json.CloseObject();
			return json.JsonString;
		}
		
		public void JsonToObject(String jsonString){
			Json json = new Json();
			json.JsonString = jsonString;
			Console.WriteLine(jsonString);
			this.Name = json.GetItem("Name");
			this.Hp = int.Parse(json.GetItem("Hp"));
			this.Mp = int.Parse(json.GetItem("Mp"));
			this.MaxHp = int.Parse(json.GetItem("MaxHp"));
			this.MaxMp = int.Parse(json.GetItem("MaxMp"));
			this.AttackPower = int.Parse(json.GetItem("AttackPower"));
			this.Defense = int.Parse(json.GetItem("Defense"));
			this.AttackSpeed = int.Parse(json.GetItem("AttackSpeed"));
			this.SpawnChance = int.Parse(json.GetItem("SpawnChance"));
			this.IsSpawn = bool.Parse(json.GetItem("IsSpawn"));
			this.IsSpawnOnce = bool.Parse(json.GetItem("IsSpawnOnce"));
			
			this.DeathEvent = json.GetItem("DeathEvent");
			
			this.SelectMessage = json.GetJsonAbleList<TextAndPosition>("SelectMessage");
			this.SpawnMessage = json.GetJsonAbleList<TextAndPosition>("SpawnMessage");
			this.StateMessage = json.GetJsonAbleList<TextAndPosition>("StateMessage");
			this.BlockMessage = json.GetJsonAbleList<TextAndPosition>("BlockMessage");
			this.DodgeMessage = json.GetJsonAbleList<TextAndPosition>("DodgeMessage");
			this.ReactionMessage = json.GetJsonAbleList<TextAndPosition>("ReactionMessage");
			this.PlayerReactionMessage = json.GetJsonAbleList<TextAndPosition>("PlayerReactionMessage");
			this.PreAttackMessage = json.GetJsonAbleList<TextAndPosition>("PreAttackMessage");
			this.AttackMessage = json.GetJsonAbleList<TextAndPosition>("AttackMessage");
		}
		
		public override string ToString(){
			return Name;
		}
	}
	
	public class NPC : Enemy,IDamageable,ICharacterState,ICloneable,ISaveToJson
	{
		public List<Quest> QuestList{get;set;}
		public List<TextAndPosition> GreetMessage{get;set;}
		public List<TextAndPosition> ConversationMessage{get;set;}
		public List<TextAndPosition> QuestAcceptMessage{get;set;}
		public List<TextAndPosition> QuestRejectMessage{get;set;}
		public List<TextAndPosition> QuestCompleteMassage{get;set;}
		public List<TextAndPosition> RevisitGreetMessage{get;set;}
		public List<TextAndPosition> RevisitConversationMessage{get;set;}
		public List<TextAndPosition> RevisitQuestAcceptMessage{get;set;}
		public List<TextAndPosition> RevisitQuestRejectMessage{get;set;}
		public List<TextAndPosition> PreQuestMessage{get;set;}
		public List<TextAndPosition> RevisitPreQuestMessage{get;set;}
		public List<TextAndPosition> QuestIntroductionMessage{get;set;}
		public List<TextAndPosition> CompleteQuestListMessage{get;set;}
		public List<TextAndPosition> PreCompleteQuestListMessage{get;set;}
	
	
		public NPC() : base()
		{
		
		}
		public NPC(string name,int hp,int mp,int attack_power,int defense,int attack_speed):base(name,hp,mp,attack_power,defense,attack_speed){}
		
		
		public TextAndPosition GetGreetMessage(){
			return TakeRandomMessage(GreetMessage);
		}
		public List<TextAndPosition> GetConversationMessageList(){ //대화는 StreamText를 통해 할 것임으로 List<TextAndPosition>로 통체로 보넴
			return ConversationMessage;
		}
		public TextAndPosition GetQuestAcceptMessage(){
			return TakeRandomMessage(QuestAcceptMessage);
		}
		public TextAndPosition GetQuestRejectMessage(){
			return TakeRandomMessage(QuestRejectMessage);
		}
		public TextAndPosition GetQuestCompleteMassage(){
			return TakeRandomMessage(QuestCompleteMassage);
		}
		public TextAndPosition GetRevisitGreetMessage(){
			return TakeRandomMessage(RevisitGreetMessage);
		}
		public TextAndPosition GetRevisitConversationMessage(){
			return TakeRandomMessage(RevisitConversationMessage);
		}
		public TextAndPosition GetRevisitQuestAcceptMessage(){
			return TakeRandomMessage(RevisitQuestAcceptMessage);
		}
		public TextAndPosition GetRevisitQuestRejectMessage(){
			return TakeRandomMessage(RevisitQuestRejectMessage);
		}
		public TextAndPosition GetPreQuestMessage(){
			return TakeRandomMessage(PreQuestMessage);
		}
		public TextAndPosition GetRevisitPreQuestMessage(){
			return TakeRandomMessage(RevisitPreQuestMessage);
		}
		public TextAndPosition GetQuestIntroductionMessage(){
			return TakeRandomMessage(QuestIntroductionMessage);
		}
		public TextAndPosition GetCompleteQuestListMessage(){
			return TakeRandomMessage(CompleteQuestListMessage);
		}
		public TextAndPosition GetPreCompleteQuestListMessage(){
			return TakeRandomMessage(PreCompleteQuestListMessage);
		}
		
		
		
		public string CurrentState(){
			return "";
		}
		//Npc를 클론할 일이 있을까
		protected NPC(NPC that):base(that){
				this.QuestList = (List<Quest>)ListClone<Quest>(that.QuestList);
				
		}
		
		public Object Clone(){
			return new NPC(this);
		}
		
		// public List<Quest> QuestList{get;set;}
		// public List<TextAndPosition> GreetMessage{get;set;}
		// public List<TextAndPosition> ConversationMessage{get;set;}
		// public List<TextAndPosition> QuestAcceptMessage{get;set;}
		// public List<TextAndPosition> QuestRejectMessage{get;set;}
		// public List<TextAndPosition> QuestCompleteMassage{get;set;}
		// public List<TextAndPosition> RevisitGreetMessage{get;set;}
		// public List<TextAndPosition> RevisitConversationMessage{get;set;}
		// public List<TextAndPosition> RevisitQuestAcceptMessage{get;set;}
		// public List<TextAndPosition> RevisitQuestRejectMessage{get;set;}
		// public List<TextAndPosition> PreQuestMessage{get;set;}
		// public List<TextAndPosition> RevisitPreQuestMessage{get;set;}
		// public List<TextAndPosition> QuestIntroductionMessage{get;set;}
		// public List<TextAndPosition> CompleteQuestListMessage{get;set;}
		// public List<TextAndPosition> PreCompleteQuestListMessage{get;set;}
		
		public String ToJsonString(){
			Json json = new Json();
			json.OpenObject(Name);
			json.AddItem("Name",Name);
			json.AddItem("Hp",Hp);
			json.AddItem("Mp",Mp);
			json.AddItem("MaxHp",MaxHp);
			json.AddItem("MaxMp",MaxMp);
			json.AddItem("AttackPower",AttackPower);
			json.AddItem("Defense",Defense);
			json.AddItem("AttackSpeed",AttackSpeed);
			json.AddItem("SpawnChance",SpawnChance);
			json.AddItem("IsSpawn",IsSpawn);
			json.AddItem("IsSpawnOnce",IsSpawnOnce);
			
			json.AddItem("DeathEvent",DeathEvent);
			
			//적대적 NPC설정시 필요한 정보 Json저장 테스트를위해 일단 주석처리했다.
			 // json.AddJsonAbleList<TextAndPosition>("SelectMessage",SelectMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("SpawnMessage",SpawnMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("StateMessage",StateMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("BlockMessage",BlockMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("DodgeMessage",DodgeMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("ReactionMessage",ReactionMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("PlayerReactionMessage",PlayerReactionMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("PreAttackMessage",PreAttackMessage,true);
			 // json.AddJsonAbleList<TextAndPosition>("AttackMessage",AttackMessage,true);
			
			json.AddJsonAbleList<Quest>("QuestList",QuestList,true);
			
			json.AddJsonAbleList<TextAndPosition>("GreetMessage",GreetMessage,true);
			json.AddJsonAbleList<TextAndPosition>("ConversationMessage",ConversationMessage,true);
			json.AddJsonAbleList<TextAndPosition>("QuestAcceptMessage",QuestAcceptMessage,true);
			json.AddJsonAbleList<TextAndPosition>("QuestRejectMessage",QuestRejectMessage,true);
			json.AddJsonAbleList<TextAndPosition>("QuestCompleteMassage",QuestCompleteMassage,true);
			json.AddJsonAbleList<TextAndPosition>("RevisitGreetMessage",RevisitGreetMessage,true);
			json.AddJsonAbleList<TextAndPosition>("RevisitConversationMessage",RevisitConversationMessage,true);
			json.AddJsonAbleList<TextAndPosition>("RevisitQuestAcceptMessage",RevisitQuestAcceptMessage,true);
			json.AddJsonAbleList<TextAndPosition>("RevisitQuestRejectMessage",RevisitQuestRejectMessage,true);
			json.AddJsonAbleList<TextAndPosition>("PreQuestMessage",PreQuestMessage,true);
			json.AddJsonAbleList<TextAndPosition>("RevisitPreQuestMessage",RevisitPreQuestMessage,true);
			json.AddJsonAbleList<TextAndPosition>("QuestIntroductionMessage",QuestIntroductionMessage,true);
			json.AddJsonAbleList<TextAndPosition>("CompleteQuestListMessage",CompleteQuestListMessage,true);
			json.AddJsonAbleList<TextAndPosition>("PreCompleteQuestListMessage",PreCompleteQuestListMessage,true);

			json.CloseObject();
			return json.JsonString;
		}
		
		public void JsonToObject(String jsonString){
			Json json = new Json();
			json.JsonString = jsonString;
			Console.WriteLine(jsonString);
			this.Name = json.GetItem("Name");
			this.Hp = int.Parse(json.GetItem("Hp"));
			this.Mp = int.Parse(json.GetItem("Mp"));
			this.MaxHp = int.Parse(json.GetItem("MaxHp"));
			this.MaxMp = int.Parse(json.GetItem("MaxMp"));
			this.AttackPower = int.Parse(json.GetItem("AttackPower"));
			this.Defense = int.Parse(json.GetItem("Defense"));
			this.AttackSpeed = int.Parse(json.GetItem("AttackSpeed"));
			this.SpawnChance = int.Parse(json.GetItem("SpawnChance"));
			this.IsSpawn = bool.Parse(json.GetItem("IsSpawn"));
			this.IsSpawnOnce = bool.Parse(json.GetItem("IsSpawnOnce"));
			
			this.DeathEvent = json.GetItem("DeathEvent");
			
			//적대적 NPC설정시 필요한 정보 Json저장 테스트를위해 일단 주석처리했다.
			 // this.SelectMessage = json.GetJsonAbleList<TextAndPosition>("SelectMessage");
			 // this.SpawnMessage = json.GetJsonAbleList<TextAndPosition>("SpawnMessage");
			 // this.StateMessage = json.GetJsonAbleList<TextAndPosition>("StateMessage");
			 // this.BlockMessage = json.GetJsonAbleList<TextAndPosition>("BlockMessage");
			 // this.DodgeMessage = json.GetJsonAbleList<TextAndPosition>("DodgeMessage");
			 // this.ReactionMessage = json.GetJsonAbleList<TextAndPosition>("ReactionMessage");
			 // this.PlayerReactionMessage = json.GetJsonAbleList<TextAndPosition>("PlayerReactionMessage");
			 // this.PreAttackMessage = json.GetJsonAbleList<TextAndPosition>("PreAttackMessage");
			 // this.AttackMessage = json.GetJsonAbleList<TextAndPosition>("AttackMessage");
			
			this.QuestList = json.GetJsonAbleList<Quest>("QuestList");
			
			this.GreetMessage = json.GetJsonAbleList<TextAndPosition>("GreetMessage");
			this.ConversationMessage = json.GetJsonAbleList<TextAndPosition>("ConversationMessage");
			this.QuestAcceptMessage = json.GetJsonAbleList<TextAndPosition>("QuestAcceptMessage");
			this.QuestRejectMessage = json.GetJsonAbleList<TextAndPosition>("QuestRejectMessage");
			this.QuestCompleteMassage = json.GetJsonAbleList<TextAndPosition>("QuestCompleteMassage");
			this.RevisitGreetMessage = json.GetJsonAbleList<TextAndPosition>("RevisitGreetMessage");
			this.RevisitConversationMessage = json.GetJsonAbleList<TextAndPosition>("RevisitConversationMessage");
			this.RevisitQuestAcceptMessage = json.GetJsonAbleList<TextAndPosition>("RevisitQuestAcceptMessage");
			this.RevisitQuestRejectMessage = json.GetJsonAbleList<TextAndPosition>("RevisitQuestRejectMessage");
			this.PreQuestMessage = json.GetJsonAbleList<TextAndPosition>("PreQuestMessage");
			this.RevisitPreQuestMessage = json.GetJsonAbleList<TextAndPosition>("RevisitPreQuestMessage");
			this.QuestIntroductionMessage = json.GetJsonAbleList<TextAndPosition>("QuestIntroductionMessage");
			this.CompleteQuestListMessage = json.GetJsonAbleList<TextAndPosition>("CompleteQuestListMessage");
			this.PreCompleteQuestListMessage = json.GetJsonAbleList<TextAndPosition>("PreCompleteQuestListMessage");
		}
	
	}
	
	public class CharacterListControler{
		Dictionary<String,Player> PlayerDictionary{get;set;}
		Dictionary<String,Enemy> EnemyDictionary{get;set;}
		Dictionary<String,NPC> NpcDictionary{get;set;}
		
		List<Player> PlayerList{get;set;}
		List<Enemy> EnemyList{get;set;}
		List<NPC> NpcList{get;set;}
		
		Player player;
		Enemy enemy;
		NPC npc;
		
		public CharacterListControler(){
			PlayerDictionary = new Dictionary<String,Player>();
			EnemyDictionary = new Dictionary<String,Enemy>();
			NpcDictionary = new Dictionary<String,NPC>();
			///////////////////Player/////////////////////////////
			player = new Player(){
				Name = "용사",
				Hp = 10,
				Mp = 10,
				MaxHp = 10,
				AttackPower = 5,
				Defense = 1,
				AttackSpeed = 2,
				ReactionMessage = null
			};
			PlayerDictionary.Add(player.Name,player);
			
			
			///////////////////Enemy/////////////////////////////
			/*
				몬스터 객체 추가시 필수로 초기화 해 주어야 할 것
				1.능력치
				Name,MaxHp,Hp,Mp,AttackPower,Defense,SpawnChance,AttackSpeed
				2.메세지
				SelectMessage,SpawnMessage,BlockMessage,DodgeMessage,ReactionMessage,PreAttackMessage,AttackMessage,
			*/
			enemy = new Enemy(){
				Name = "슬라임",
				MaxHp = 10,
				Hp = 10,
				Mp = 0,
				AttackPower = 1,
				Defense = 0,
				SpawnChance = 70,
				AttackSpeed = 1,
				
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
				},
				BlockMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("슬라임이 맥없이 팅겨나갔다.",10),
					new TextAndPosition("슬라임은 생각보다 묵직했다.",10)
				},
				DodgeMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("점액질 덩어리를 가볍게 피해냈다.",10),
					new TextAndPosition("슬라임은 내 생각보다 빨랐다.",10)
				},
				ReactionMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("슬라임의 점액질이 아주 조금 튀었다.",10),
					new TextAndPosition("슬라임은 팅겨나갔다.",10),
					new TextAndPosition("슬라임은 잠시 형체를 잃었다!!",10),
					new TextAndPosition("슬라임은 그대로 터져버렸다..",10)
				},
				//플레이어가 몬스터에게 맞았을때의 깍인 HP의 비율에 따라 출력하는 플레이어의 반응 메세지
				//정도에 따라 4단계로 구분된다. 인덱스 순서로 약한 강도에서 강한 강도이다.
				PlayerReactionMessage = new List<TextAndPosition>(){
					new TextAndPosition("말랑말랑 기분이 좋다.",10),
					new TextAndPosition("돌덩이에 맞은것 같다. 맞은데가 얼얼하다. ",10),
					new TextAndPosition("마치 바위에 부딪친듯 하다. 정신이 아득하다.",10),
					new TextAndPosition("슬라임은 내몸을 관통했다. 슬라임에게 지다니 믿을 수 없다.. 나는 그대로 쓰러졌다.",10)
				},
				
				PreAttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("내가 머뭇거리는 사이 슬라임이 튀어올랐다.",10)
				},
				
				AttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("슬라임의 몸통박치기!!",10)
				},
				
				DropItems = new List<Item>(){
					itemList.GetItem("슬라임 젤",100),
					itemList.GetItem("슬라임 젤",50),
					itemList.GetItem("슬라임 젤",100),
					itemList.GetItem("슬라임 젤",50),
					itemList.GetItem("전설의검",100)
				}
			};
			EnemyDictionary.Add(enemy.Name,enemy);
			
			enemy = new Enemy(){
				Name = "뒤틀린 망자",
				MaxHp = 10,
				Hp = 10,
				Mp = 0,
				AttackPower = 3,
				Defense = 2,
				SpawnChance = 50,
				AttackSpeed = 2,
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
				},
				BlockMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("망자는 뒤로 넘어졌다.",10),
					new TextAndPosition("하지만 망자의 공격은 생각보다 매서웠다.",10)
				},
				DodgeMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("망자의 손톱이 코앞을 스친다.",10),
					new TextAndPosition("이미 손톱은 내 몸에 박혀있었다.",10)
				},
				PlayerReactionMessage = new List<TextAndPosition>(){
					new TextAndPosition("망자의 손톱이 힘없이 부딪쳤다. 간지럽지도 않다.",10),
					new TextAndPosition("망자의 손톱이 내 살을 파고들었다. 쓰라리다.",10),
					new TextAndPosition("망자의 손톱이 내살을 두부처럼 파고들었다. 너무 큰 고통에 비명조차 나오지 않는다.",10),
					new TextAndPosition("손톱이 내몸을 훑고 지나간ㄷ....",10)
				},
				ReactionMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("효과가 없는듯 그대로 달려든다.",10),
					new TextAndPosition("망자가 비틀거린다.",10),
					new TextAndPosition("고통스러운듯 비명을 지른다!",10),
					new TextAndPosition("망자는 그대로 조각났다.",10)
				},
				
				PreAttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("나보다 망자가 더 빠르게 움직였다.",10)
				},
				
				AttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("망자의 손톱 할퀴기!",10)
				}
			};
			EnemyDictionary.Add(enemy.Name,enemy);
			
			enemy = new Enemy(){
				Name = "헐크",
				MaxHp = 10000,
				Hp = 10000,
				Mp = 0,
				AttackPower = 100,
				Defense = 100,
				SpawnChance = 10,
				AttackSpeed = 100,
				SelectMessage = new List<TextAndPosition> 
				{new TextAndPosition("저게 뭐야!!!",10),
				new TextAndPosition("헐..크..?",10),
				new TextAndPosition("!!!!!!",10)},
				SpawnMessage = new List<TextAndPosition>()
				{new TextAndPosition("온몸이 초록색인 이성잃은 괴물..",10),						   
				 new TextAndPosition("헐크다!!!",10)},
				StateMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("멀뚱히 서있다.",10),
					new TextAndPosition("화가나 보인다.",10),
					new TextAndPosition("움직임이 둔해졌다.",10),
					new TextAndPosition("헐크를.. 죽였다.",10)
				},
				BlockMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("달려들던 헐크는 그대로 막혔다.",10),
					new TextAndPosition("가드한 모습 그대로 날아갔다.",10)
				},
				DodgeMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("헐크가 내가 있던 자리를 지난다.",10),
					new TextAndPosition("정신을 차려보니 벽에 박혀 있었다.",10)
				},
				ReactionMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("간지러운듯 콧웃음을 친다.",10),
					new TextAndPosition("때려서 화가난듯 하다.",10),
					new TextAndPosition("헐크가 비틀거린다!",10),
					new TextAndPosition("벽에 박힌 헐크는 미동도 하지 않는다.",10)
				},
				PlayerReactionMessage = new List<TextAndPosition>(){
					new TextAndPosition("헐크의 거대한 주먹이 솜방망이 처럼 느껴진다.",10),
					new TextAndPosition("맞은 곳이 얼얼하다.",10),
					new TextAndPosition("피를 토하며 한참을 날아가 굴렀다.",10),
					new TextAndPosition("내몸은 사방으로 흩어졌다.",10)
				},
				PreAttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("헐크에게 눈을 돌린 순간 눈앞에서 헐크는 사라졌다.",10)
				},
				
				AttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("헐크의 주먹 내려치기!",10)
				}
			};
			EnemyDictionary.Add(enemy.Name,enemy);
			
			enemy = new Enemy(){
				Name = "야생의 경민이",
				MaxHp = 100,
				Hp = 100,
				Mp = 0,
				AttackPower = 5,
				Defense = 5,
				SpawnChance = 0,
				AttackSpeed = 3,
				IsSpawnOnce = true,
				SelectMessage = new List<TextAndPosition> 
				{new TextAndPosition("야생의 경민이!!!!",10),
				new TextAndPosition("뭐지..? 경민..?",10),
				new TextAndPosition("박.경.민",10)},
				SpawnMessage = new List<TextAndPosition>()
				{new TextAndPosition("야생의 경민이가 출현했다.",10),						   
				 new TextAndPosition("말로만 듣던 냉장고?!",10)},
				StateMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("곧장이라도 날아올듯 팔팔하다.",10),
					new TextAndPosition("수많은 공격에도 불구하고 피한방울 뿐이다.",10),
					new TextAndPosition("경민이가 헐떡인다.",10),
					new TextAndPosition("경민이가 쓰러졌다!",10)
				},
				BlockMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("경민이를 막았다.",10),
					new TextAndPosition("팔이 막은 그대로 으스러졌다.",10)
				},
				DodgeMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("냉장고를 피했다.",10),
					new TextAndPosition("정신을 차려보니 냉장고에 깔려있었다.",10)
				},
				ReactionMessage = new List<TextAndPosition>()
				{
					new TextAndPosition("간지러운듯 콧웃음을 친다.",10),
					new TextAndPosition("움찔거린다.",10),
					new TextAndPosition("경민이가 신음소리를 낸다.",10),
					new TextAndPosition("경민이가 쓰러졌다!",10)
				},
				PlayerReactionMessage = new List<TextAndPosition>(){
					new TextAndPosition("간지러운데..?",10),
					new TextAndPosition("아야,아야! 아파용~",10),
					new TextAndPosition("우리집 냉장고가 기억이 안 난다..\n 왜 냉장고를 생각하고 있지?",10),
					new TextAndPosition("경민이에게 깔려 의식을 잃어간다...",10)
				},
				PreAttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("눈깜짝할 사이 경민이가 배치기를 시도한다.",10)
				},
				
				AttackMessage = new List<TextAndPosition>(){
					new TextAndPosition("경민이의 배치기!!!",10)
				},
				
				DeathEvent = "spawnGyeonmin"
			};
			EnemyDictionary.Add(enemy.Name,enemy);
			
			
			///////////////////NPC/////////////////////////////
			/*
			public List<Quest> QuestList{get;set;}
			public List<TextAndPosition> GreetMessage{get;set;}
			public List<TextAndPosition> ConversationMessage{get;set;}
			public List<TextAndPosition> QuestAcceptMessage{get;set;}
			public List<TextAndPosition> QuestRejectMessage{get;set;}
			public List<TextAndPosition> QuestCompleteMassage{get;set;}
			public List<TextAndPosition> RevisitGreetMessage{get;set;}
			public List<TextAndPosition> RevisitConversationMessage{get;set;}
			public List<TextAndPosition> RevisitQuestAcceptMessage{get;set;}
			public List<TextAndPosition> RevisitQuestRejectMessage{get;set;}
			public List<TextAndPosition> PreQuestMessage{get;set;}
			public List<TextAndPosition> RevisitPreQuestMessage{get;set;}
			*/
			//QuestControler.AllQuestList = DataManager.LoadQuest();
			//testLog(QuestControler.GetQuestByName("슬라임 사냥").QuestName);
			
			npc = new NPC(){
				Name = "촌장",
				Hp = 10,
				Mp = 10,
				MaxHp = 10,
				AttackPower = 1,
				Defense = 1,
				AttackSpeed = 2,
				GreetMessage = new List<TextAndPosition>(){
					new TextAndPosition("어서오시게 낮선/ 이여\\.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				ConversationMessage = new List<TextAndPosition>(){
					new TextAndPosition("나는 마을 촌장이라네.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT},
					new TextAndPosition("외부인은 정말 오랜만에 보는구먼.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT},
					new TextAndPosition("편히 있다 가게나.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				QuestAcceptMessage = new List<TextAndPosition>(){
					new TextAndPosition("오 정말 고맙네. 잘부탁하네.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				QuestRejectMessage = new List<TextAndPosition>(){
					new TextAndPosition("...정 그렇다면 어쩔 수 없지.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				QuestCompleteMassage = new List<TextAndPosition>(){
					new TextAndPosition("대단하구먼! 고맙네.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				RevisitGreetMessage = new List<TextAndPosition>(){
					new TextAndPosition("또 왔구먼. 편히 있다 가게나.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				RevisitConversationMessage = new List<TextAndPosition>(){
					new TextAndPosition("슬라임은 마을 앞 초원에서 많이 나온다네.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				RevisitQuestAcceptMessage = new List<TextAndPosition>(){
					new TextAndPosition("어짜피 해줄 거면서... 아.. 아무것도 아니네. 고맙네",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				RevisitQuestRejectMessage = new List<TextAndPosition>(){
					new TextAndPosition("이 늙은이를 놀리는겐가!",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				PreQuestMessage = new List<TextAndPosition>(){
					new TextAndPosition("제가 도울 일이 있습니까?",40,13,true)
				},
				RevisitPreQuestMessage = new List<TextAndPosition>(){
					new TextAndPosition("그 일 아직 있습니까?",10)
				},
				QuestIntroductionMessage = new List<TextAndPosition>(){
					new TextAndPosition("일은 많지. 자 골라보게.",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				CompleteQuestListMessage = new List<TextAndPosition>(){
					new TextAndPosition("오. 해결했다고?",10){AlignH = true,Layout = TextLayout.ONLY_SHOW_DEFAULT}
				},
				PreCompleteQuestListMessage = new List<TextAndPosition>(){
					new TextAndPosition("해결했습니다.",40,13,true)
				},
				
				QuestList = new List<Quest>(){
					QuestControler.GetQuestByName("슬라임 사냥"),
					QuestControler.GetQuestByName("망자 사냥"),
					QuestControler.GetQuestByName("폐가 방문"),
					QuestControler.GetQuestByName("피부에 좋은 슬라임 젤"),
					QuestControler.GetQuestByName("testMeeting")
				}
			};
			NpcDictionary.Add(npc.Name,npc);
			
			PlayerList = new List<Player>(PlayerDictionary.Values);
			EnemyList = new List<Enemy>(EnemyDictionary.Values);
			NpcList = new List<NPC>(NpcDictionary.Values);
		}
		
		public void SetEnemyDictionaryAsList(List<Enemy> list){
			EnemyList = list;
			EnemyDictionary = new Dictionary<String,Enemy>();
			foreach(Enemy e in list){
				EnemyDictionary.Add(e.Name,e);
			}
		}
		public void SetNpcDictionaryAsList(List<NPC> list){
			NpcList = list;
			NpcDictionary = new Dictionary<String,NPC>();
			foreach(NPC n in list){
				NpcDictionary.Add(n.Name,n);
			}
		}
		public void SetPlayerDictionaryAsList(List<Player> list){
			PlayerList = list;
			PlayerDictionary = new Dictionary<String,Player>();
			foreach(Player p in list){
				PlayerDictionary.Add(p.Name,p);
			}
		}
		
		public NPC GetNpcOriginal(String name){ //원본임. 복사할 필요가 없다고 판단 2021.12.31
			try{
				return NpcDictionary[name];
			}catch(Exception e){
				//Console.WriteLine(e);
				return null;
			}
		}
		
		public Enemy GetEnemyOriginal(String name){
			try{
				return EnemyDictionary[name];
			}catch(Exception e){
				//Console.WriteLine(e);
				return null;
			}
		}
		
		public Enemy GetEnemy(String name){
			try{
				return (Enemy)EnemyDictionary[name].Clone();
			}catch(Exception e){
				//Console.WriteLine(e);
				return null;
			}
		}
		
		public Enemy GetEnemy(String name,int spawn){
			try{
				Enemy mons = (Enemy)EnemyDictionary[name].Clone();
				mons.SpawnChance = spawn;
				return mons;
			}catch(Exception e){
				//Console.WriteLine(e);
				return null;
			}
		}
		
		public Player GetPlayer(String name){
			return PlayerDictionary[name];
		}
		
		public String EnemyListToJsonString()
		{
			Json json = new Json();
			json.AddJsonAbleList<Enemy>("Enemys",EnemyList);
			return json.JsonString;
		}
		
		public String NpcListToJsonString()
		{
			Json json = new Json();
			json.AddJsonAbleList<NPC>("NPCs",NpcList);
			return json.JsonString;
		}
		
		public String PlayerListToJsonString()
		{
			Json json = new Json();
			json.AddJsonAbleList<Player>("Players",PlayerList);
			return json.JsonString;
		}
		
		
		/*public Character GetCharacter(String Name){
			return characterList[Name];
		}*/
	}
}