using System;
using System.Collections.Generic;
using Game;
using System.IO;
using static Convenience;
using static GameWindows;


namespace Characters
{
	interface IDamageable{
		AttackInfo Attack();
		AttackInfo Damage(AttackInfo attack_info);
	}
	
	interface ICharacterState{
		string CurrentState();
	}
	
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
		
		protected Character(Character that){
			this.Name = that.Name ;
			this.Hp = that.Hp ;
			this.MaxHp = that.MaxHp ;
			this.Mp = that.Mp ;
			this.MaxMp = that.MaxMp ;
			this.AttackPower = that.AttackPower ;
			this.Defense = that.Defense ;
		}
		
		virtual public Object Clone(){
			return new Character(this);
		}
		
	}
	
	public class Player : Character,IDamageable,ICharacterState
	{
		Weapon weapon;
		public Weapon Weapon{
			get{
				return weapon;
			}
			set{
				if(weapon == null){
					weapon = value;
					weapon.IsEquip = true;
				}
				else{
					if(weapon == value){
						if(ConfirmWindow("이미 장착된 무기 입니다.해제하시겠습니까?",24,7)){
							weapon.IsEquip = false;
							weapon = null;
						}
					}
					else{
							AlertWindow("이미 무기가 존재합니다.",24,9);
					}
				}
			}
		}
		Armor armor;
		public Armor Armor{
			get{
				return armor;
			}
			set{
				if(armor == null){
					armor = value;
					armor.IsEquip = true;
				}
				else{
					if(armor == value){
						if(ConfirmWindow("이미 장착된 방어구 입니다.해제하시겠습니까?",24,7)){
							armor.IsEquip = false;
							armor = null;
						}
						else{
							AlertWindow("이미 방어구가 존재합니다.",24,9);
						}
					}
				}
			}
		}
		
		public Player():base(){}
		public Player(string name,int hp,int mp,int attack_power,int defense):base(name,hp,mp,attack_power,defense){}
		
		override public AttackInfo Attack(){
			AttackInfo aInfo = new AttackInfo();
			aInfo.Final_damage = Weapon == null ? AttackPower : (AttackPower + Weapon.AttackPower);
		 	return aInfo;
		}
		
		override public AttackInfo Damage(AttackInfo attack_info){
			AttackInfo aInfo = attack_info;
			aInfo.CalDamage(Weapon == null ? Defense : (Defense + Armor.Defense));
			Hp -= aInfo.Final_damage;
			return aInfo;
		}
		
		public string CurrentState(){
			return "";
		}
		
		protected Player(Player that):base(that){
						
		}
		
		public Object Clone(){
			return new Player(this);
		}
	}
	
	public class Monster : Character,IDamageable,ICharacterState
	{
		public int SpawnChance{get;set;}
		public bool IsSpawn{get;set;} = false;
		
		public List<TextAndPosition> selectMessage;
		public List<TextAndPosition> SelectMessage{
			get{
				return selectMessage;
			}
			set{
				selectMessage = value;
			}
		}
		
		public List<TextAndPosition> SpawnMessage{get;set;}
		public List<TextAndPosition> StateMessage{get;set;}
	
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
		
		protected Monster(Monster that):base(that){
			this.SpawnChance = that.SpawnChance;
			this.IsSpawn = that.IsSpawn;
			this.SelectMessage = new List<TextAndPosition>();
			this.SpawnMessage = new List<TextAndPosition>();
			this.StateMessage = new List<TextAndPosition>();
			
			if(SelectMessage != null)
				this.SelectMessage = that.SelectMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(SpawnMessage != null)
				this.SpawnMessage = that.SpawnMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
			if(StateMessage != null)
				this.StateMessage = that.StateMessage.ConvertAll(new Converter<TextAndPosition,TextAndPosition>(o => (TextAndPosition)o.Clone()));
		}
		
		override public Object Clone(){
			return new Monster(this);
		}
		
		public void MonsterInfo(){
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
		
		public override string ToString(){
			return Name;
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
		
		protected NPC(NPC that):base(that){
			
		}
		
		public Object Clone(){
			return new NPC(this);
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
			
			monster = new Monster(){
				Name = "헐크",
				MaxHp = 10000,
				Hp = 10000,
				Mp = 0,
				AttackPower = 100,
				Defense = 100,
				SpawnChance = 10,
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
				}
				
			};
			MonsterList.Add(monster.Name,monster);
			
			///////////////////NPC/////////////////////////////
		}
		
		public Monster GetMonster(String name){
			try{
				return (Monster)MonsterList[name].Clone();
			}catch(Exception e){
				Console.WriteLine(e);
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