using System;
using System.Collections.Generic;
using static Define;
using static GameWindows;


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
	
	public void OpenInventory(){
			bool OutInven = false;
		Backgrounds backgrounds = new Backgrounds();
		
		Dictionary<int,Object> invenListName = new Dictionary<int,Object>();
		for(int i = 0;i<inventory.Count;i++){
			invenListName.Add(i,inventory[i].Name);
		}
		List<TextAndPosition> itemList = new List<TextAndPosition>();
		for(int i = 0;i<inventory.Count;i++){
			itemList.Add(new TextAndPosition(inventory[i].Name,28,i+2,true));
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
						if(ConfirmWindow("아이템을 사용하시겠습니까?",24,7)){
							i.Use();
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
