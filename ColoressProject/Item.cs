using System;
using System.Collections.Generic;
using static Convenience;
using Game;
using MyJson;


public class Item : IEquatable<Item>,ICloneable,ISaveToJson{
	public String Name{get;set;}
	
	private bool useable = false;
	public bool IsStackable{get;set;} = false;
	
	private int amount = 1; //stackable이 true일때 사용
	
	public String ItemExplan{get;set;} = "아이템 설명";
	
	public double DropChance{get;set;} = 0;
	
	public int Amount{
		get{
			return amount;
		}
		set{
			amount = value;
		}
	}
	
	public Item(){
		Name = "unknown";
	}
	
	public virtual void Use(){
		testLog("아이템 사용!: "+Name);
	}
	
	public virtual String Explan(){
		return ItemExplan;
	}
	
	public override bool Equals(Object obj){
		if(obj == null) return false;
		Item objAsItem = obj as Item;
		if(objAsItem == null) return false;
		else return Equals(objAsItem);
	}
	
	public bool Equals(Item other){
		if(other==null) return false;
		return (this.Name.Equals(other.Name));
	}
	
	protected Item(Item that){
		this.Name = that.Name;
		this.useable = that.useable;
		this.IsStackable = that.IsStackable;
		this.Amount = that.Amount;
		
		this.ItemExplan = that.ItemExplan;
		this.DropChance = that.DropChance;
	}
	
	public virtual Object Clone(){
		return new Item(this);
	}
	// public String Name{get;set;}
	
	// bool useable = false;
	// public bool IsStackable{get;set;} = false;
	
	// int amount = 1; //stackable이 true일때 사용
	
	// public String ItemExplan{get;set;} = "아이템 설명";
	
	// public double DropChance{get;set;} = 0;
	public virtual String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Item");
		json.AddItem("Name",Name);
		json.AddItem("IsStackable",IsStackable);
		json.AddItem("ItemExplan",ItemExplan);
		json.AddItem("DropChance",DropChance);
		json.CloseObject();
		return json.JsonString;
	}
	public virtual void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.Name = json.GetItem("Name");
		this.IsStackable = bool.Parse(json.GetItem("IsStackable"));
		this.ItemExplan = json.GetItem("ItemExplan");
		this.DropChance = double.Parse(json.GetItem("DropChance"));
	}
}

public class Equipment : Item{
	 public bool IsEquip {get;set;} = false;
	
	public Equipment(){
		
	}
	
	protected Equipment(Equipment that) : base(that){
		this.IsEquip = that.IsEquip;
	}
	
	public override Object Clone(){
		return new Equipment(this);
	}
	
	public override String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Item");
		json.AddItem("Name",Name);
		json.AddItem("IsStackable",IsStackable);
		json.AddItem("ItemExplan",ItemExplan);
		json.AddItem("DropChance",DropChance);
		json.AddItem("IsEquip",IsEquip);
		json.CloseObject();
		return json.JsonString;
	}
	public override void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.Name = json.GetItem("Name");
		this.IsStackable = bool.Parse(json.GetItem("IsStackable"));
		this.ItemExplan = json.GetItem("ItemExplan");
		this.DropChance = double.Parse(json.GetItem("DropChance"));
		this.IsEquip = bool.Parse(json.GetItem("IsEquip"));
	}
}

public class Weapon : Equipment{
	public int AttackPower{get;set;} = 0;
	public int AttackSpeed{get;set;} = 0;
	
	public List<TextAndPosition> AttackMessage{get;set;} = new List<TextAndPosition>(){ new TextAndPosition("나는 무기를 휘둘렀다.",10)};
	
	public Weapon(){
		
	}
	
	public override void Use(){
		GameManager.Equip(this);
	}
	
	protected Weapon(Weapon that) : base(that){
		this.AttackPower = that.AttackPower;
		this.AttackSpeed = that.AttackSpeed;
		
		if(that.AttackMessage != null)
			this.AttackMessage = that.AttackMessage.ConvertAll(new Converter<TextAndPosition, TextAndPosition>(o => (TextAndPosition)o.Clone()));
	}
	
	public override Object Clone(){
		return new Weapon(this);
	}
	
	public override String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Item");
		json.AddItem("Name",Name);
		json.AddItem("IsStackable",IsStackable);
		json.AddItem("ItemExplan",ItemExplan);
		json.AddItem("DropChance",DropChance);
		json.AddItem("IsEquip",IsEquip);
		json.AddItem("AttackPower",AttackPower);
		json.AddItem("AttackSpeed",AttackSpeed);
		json.AddItem("AttackMessage",AttackMessage);
		json.CloseObject();
		return json.JsonString;
	}
	public override void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.Name = json.GetItem("Name");
		this.IsStackable = bool.Parse(json.GetItem("IsStackable"));
		this.ItemExplan = json.GetItem("ItemExplan");
		this.DropChance = double.Parse(json.GetItem("DropChance"));
		this.IsEquip = bool.Parse(json.GetItem("IsEquip"));
		this.AttackPower = int.Parse(json.GetItem("AttackPower"));
		this.AttackSpeed = int.Parse(json.GetItem("AttackSpeed"));
		this.AttackMessage = json.GetJsonAbleList<TextAndPosition>("AttackMessage");
	}
}

public class Armor : Equipment{
	public int Defense{get;set;} = 0;
	
	public Armor(){
		
	}
	
	public override void Use(){
		GameManager.Equip(this);
	}
	
	protected Armor(Armor that) : base(that){
		this.Defense = that.Defense;
	}
	
	public override Object Clone(){
		return new Armor(this);
	}
	
	public override String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Item");
		json.AddItem("Name",Name);
		json.AddItem("IsStackable",IsStackable);
		json.AddItem("ItemExplan",ItemExplan);
		json.AddItem("DropChance",DropChance);
		json.AddItem("IsEquip",IsEquip);
		json.AddItem("Defense",Defense);
		json.CloseObject();
		return json.JsonString;
	}
	public override void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.Name = json.GetItem("Name");
		this.IsStackable = bool.Parse(json.GetItem("IsStackable"));
		this.ItemExplan = json.GetItem("ItemExplan");
		this.DropChance = double.Parse(json.GetItem("DropChance"));
		this.IsEquip = bool.Parse(json.GetItem("IsEquip"));
		this.Defense = int.Parse(json.GetItem("Defense"));
	}
}

public class Potion : Item{
	public int Hp{get;set;}
	
	public override void Use(){
		GameManager.Eat(this);
	}
}
//아이템 데이터를 모아놓은 클래스
/*
	아이템 추가시 초기화 해 주어야 할 필드
	0.공통
	Name,ItemExplan
	1.무기
	AttackPower,AttackSpeed
	2.방어구
	Defense
*/
public class ItemList{
	Dictionary<String,Item> itemList = new Dictionary<String,Item>();
	Item item;
	
	public ItemList(){
		item = new Weapon(){
			Name = "전설의검",
			ItemExplan="이것은 전설의 괴물\n헐크를 죽일 수 있다는\n전설의 검이다",
			AttackPower = 9999,
			AttackSpeed = 99,
			AttackMessage = new List<TextAndPosition>(){
				 new TextAndPosition("전설의 검은 마치 유성우와 같은 궤적을 남겼다.",10),
				 new TextAndPosition("산을 가를듯한 기세로 검을 휘둘렀다.",10),
				 new TextAndPosition("일섬",10)
			}
		};
		itemList.Add(item.Name,item);
		item = new Item(){
			Name = "슬라임 젤",
			ItemExplan="슬라임의 일부이다.\n잘못 만지면 피부가 녹을 수 있다.",
			IsStackable = true
		};
		itemList.Add(item.Name,item);
		
		item = new Armor(){
			Name = "황금갑옷",
			ItemExplan="전설의 황금갑옷\n전설적인 갑옷이다.\n전설적인 갑옷이여서 그런지\n순도 100%의 황금임에도\n꽤 높은 방어력을 자랑한다.",
			Defense = 10
		};
		itemList.Add(item.Name,item);
		
		item = new Item(){
			Name = "촌장의 일기",
			ItemExplan="xxxx.xx.xx\n곧 있으면 아내의 생일이다.\n요즘 벌레가 많아져\n아내의 피부건강이 걱정된다.\n슬라임 젤이면 아내의 피부\n건강을 지킬 수 있을 것이다."
		};
		itemList.Add(item.Name,item);
	}
	
	public Item GetItem(String Name,double DropChance = 0){
		Item i = (Item)itemList[Name].Clone();
		i.DropChance = DropChance;
		return i;
	}
}