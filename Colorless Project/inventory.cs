using System;
using System.Collections.Generic;

public class Inventory{
	List<Item> inventory;
	
	int maximumCount = 10;
	
	public Inventory(){
		inventory = new List<Item>();
	}
	
	public void AddItem(Item item){
		if(item != null && inventory.Count<maximumCount)
			inventory.Add(item);
	}
	
	public Item GetItem(int num){
		return inventory[num];
	}
	
}
