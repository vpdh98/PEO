using System;
using System.Collections.Generic;
using static Convenience;
using Game;


public class Item{
	public String Name{get;set;}
	
	bool useable = false;
	bool stackable = false;
	
	int amount; //stackable이 true일때 사용
	
	public Item(){
		Name = "unknown";
	}
	
	public virtual void Use(){
		testLog("아이템 사용!: "+Name);
	}
	
	public virtual String Explan(){
		return "아이템 설명";
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
