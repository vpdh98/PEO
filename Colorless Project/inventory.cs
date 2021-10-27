using System;
using System.Collections.Generic;
using static Define;
using static GameWindows;


public class Inventory{
	List<Item> inventory;
	
	int maximumCount = 20;
	
	public Inventory(){
		inventory = new List<Item>();
	}
	
	public bool AddItem(Item item){
		if(item != null && inventory.Count<maximumCount){
			inventory.Add(item);
			return true;
		}
		else
			return false;
	}
	
	public Item GetItem(int num){
		return inventory[num];
	}
	
	public void OpenInventory(){
			bool OutInven = false;
		Backgrounds backgrounds = new Backgrounds();
		
		Dictionary<int,Object> invenListName = new Dictionary<int,Object>();
		for(int i = 0;i<inventory.Count;i++){
			invenListName.Add(i,inventory[i].Name);
		}
		List<TextAndPosition> itemList = new List<TextAndPosition>();
		for(int i = 0;i<inventory.Count;i++){
			if(i < 10)
				itemList.Add(new TextAndPosition(inventory[i].Name,18,i+2,true));
			else
				itemList.Add(new TextAndPosition(inventory[i].Name,36,i-8,true));
		}
		Dictionary<String,Item> invenListObject = new Dictionary<String,Item>();
		for(int i = 0;i<inventory.Count;i++){
			invenListObject.Add(inventory[i].Name,inventory[i]);
		}
	
			Choice invenCho = new Choice(){
				Name = INVENTORY,
				SelectText = itemList,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("인벤토리",28,1)},
				IndicateChoice = invenListName,
				BackgroundText = backgrounds.GetBackground(2)
			};
		
		DisplayTextGame IDTG = new DisplayTextGame();
		
		while(!OutInven){
				IDTG.Cho = invenCho; //초기 화면
				//DTG.PrintListInfo();
				IDTG.Show();
				IDTG.delay = 0;
				ConsoleKeyInfo c = Console.ReadKey();

				while(c.Key != ConsoleKey.Escape){

					IDTG.SelectingText(c);

					if(c.Key == ConsoleKey.Enter){
						Item i = invenListObject[(String)IDTG.Cho.GetValueOn(IDTG.currentSelectNum)];
						if(i.GetType().Name == "Weapon"){
							if(ConfirmWindow("장비를 착용 하시겠습니까?",24,7)){
								i.Use();
							}
						}
						else{
							if(ConfirmWindow("아이템을 사용하시겠습니까?",24,7)){
								i.Use();
							}
						}
					}
					/*if(c.Key == ConsoleKey.Escape){
						OutInven = true;
					}*/
					//방향키나 숫자를 누르면 여기로 넘어옴
					IDTG.Show();	
					c = Console.ReadKey();
				}
				OutInven = true;			
			}
		return;
	}
	
}
