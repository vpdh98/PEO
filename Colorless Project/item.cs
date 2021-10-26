using System;
using System.Collections.Generic;
using static Convenience;

public class Item{
	public String Name{get;set;}
	
	bool useable = false;
	bool stackable = false;
	
	int amount; //stackable이 true일때 사용
	
	public Item(){
		Name = "unknown";
	}
	
	public virtual void Use(){
		testLog("아이템 사용!");
	}
}

public class Weapon : Item{
	int attackPower = 0;
	int durability = 1;
	
}

public class Potion : Item{
	
}
