using System;
using System.Collections.Generic;

public class Item{
	public string Name{get;set;}
	
	bool useable = false;
	bool stackable = false;
	
	int amount; //stackable이 true일때 사용
	
	public Item(){
		Name = "unknown";
	}
}

public class Weapon : Item{
	int attackPower = 0;
	int durability = 1;
	
}

public class Potion : Item{
	
}
