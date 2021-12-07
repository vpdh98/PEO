using System;
using System.Collections.Generic;
using static Convenience;
using Game;


public class Item{
	public String Name{get;set;}
	
	bool useable = false;
	bool stackable = false;
	
	int amount; //stackable이 true일때 사용
	
	public String ItemExplan{get;set;} = "아이템 설명";
	
	public Item(){
		Name = "unknown";
	}
	
	public virtual void Use(){
		testLog("아이템 사용!: "+Name);
	}
	
	public virtual String Explan(){
		return ItemExplan;
	}
	
	public bool Equals(Item other){
		if(other==null) return false;
		return (this.Name.Equals(other.Name));
	}
}

public class Equipment : Item{
	 public bool IsEquip {get;set;} = false;
}

public class Weapon : Equipment{
	public int AttackPower{get;set;} = 0;
	public int AttackSpeed{get;set;} = 0;
	
	public List<TextAndPosition> AttackMessage{get;set;} = new List<TextAndPosition>(){ new TextAndPosition("나는 무기를 휘둘렀다.",10)};
	
	public override void Use(){
		GameManager.Equip(this);
	}
	
}

public class Armor : Equipment{
	public int Defense{get;set;} = 0;
	
	public override void Use(){
		GameManager.Equip(this);
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
	1.무기
	Name,ItemExplan,AttackPower,AttackSpeed
*/
public class ItemList{
	Dictionary<String,Item> itemList = new Dictionary<String,Item>();
	Item item;
	
	public ItemList(){
		item = new Weapon(){
			Name = "전설의검",
			ItemExplan="이것은 전설의 괴물\n헐크를 죽일 수 있다는\n전설의 검이다",
			AttackPower = 9999,
			AttackSpeed = 99
		};
		itemList.Add(item.Name,item);
	}
	
	public Item GetItem(String Name){
		return itemList[Name];
	}
}