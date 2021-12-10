using System;
using System.Collections.Generic;
using static Define;
using static GameWindows;


public class Inventory{
	List<Item> inventory;
	Choice invenCho;
	Dictionary<String,Item> invenListObject;
	Backgrounds backgrounds = new Backgrounds();
	DisplayTextGame IDTG = new DisplayTextGame(){GlobalPositionX=40,GlobalPositionY=5};
	
	int maximumCount = 20;
	
	public Inventory(){
		inventory = new List<Item>();
	}
	
	void Init(){
		
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
		invenListObject = new Dictionary<String,Item>();
		for(int i = 0;i<inventory.Count;i++){
			invenListObject.Add(inventory[i].Name,inventory[i]);
		}
	
			invenCho = new Choice(){
				Name = INVENTORY,
				SelectText = itemList,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("인벤토리",28,1),
							new TextAndPosition("버리기: D",52,1)},
				IndicateChoice = invenListName,
				BackgroundText = backgrounds.GetBackground(2)
			};
		IDTG.Init();
		IDTG.Cho = invenCho; //초기 화면
		IDTG.Show();
	}
	
	public bool AddItem(Item item){
		if(item != null && inventory.Count<maximumCount){
			Item tItem = inventory.Find(i => i.Name == item.Name);
			if(tItem == null)
				inventory.Add(item);
			else if(tItem.Stackable){
				tItem.Amount += 1;
			}
			return true;
		}
		else
			return false;
	}
	
	public Item GetItem(int num){
		return inventory[num];
	}
	
	public Item GetItem(String name){
		return inventory.Find(x => x.Name == name);
	}
	
	public void OpenInventory(){
		bool OutInven = false;
		Init();
		
		while(!OutInven){
				IDTG.Cho = invenCho; //초기 화면
				IDTG.Show();
				ExplanWindow(inventory[0],89,3,20,3);
				ConsoleKeyInfo c = Console.ReadKey();

				while(c.Key != ConsoleKey.Escape && c.Key != ConsoleKey.I && inventory.Count != 0)
				{

					IDTG.SelectingText(c);
					Item i = invenListObject[(String)IDTG.Cho.GetValueOn(IDTG.currentSelectNum)];
					
					if(c.Key == ConsoleKey.Enter){
						
						
						if(i is Equipment){
							if(((Equipment)i).IsEquip){
								i.Use();
							}
							else if(ConfirmWindow("장비를 착용 하시겠습니까?",24,7)){
								i.Use();
							}
						}
						else{
							if(ConfirmWindow("아이템을 사용하시겠습니까?",24,7)){
								i.Use();
							}
						}
					}
					else if(c.Key == ConsoleKey.D){
						//Item i = invenListObject[(String)IDTG.Cho.GetValueOn(IDTG.currentSelectNum)];
						if(i is Equipment){
							if(((Equipment)i).IsEquip)
								AlertWindow("장착된 장비는 버리실 수 없습니다.",textXPos:24,textYPos:9);
							else if(ConfirmWindow("버리시겠습니까?",24,7)){
								inventory.Remove(i);
								Init();
							}
						}
						else if(ConfirmWindow("버리시겠습니까?",24,7)){
							inventory.Remove(i);
							Init();
						}
					}
					/*if(c.Key == ConsoleKey.Escape){
						OutInven = true;
					}*/
					//방향키나 숫자를 누르면 여기로 넘어옴
					IDTG.Show();	
					ExplanWindow(i,89,3,20,3);
					c = Console.ReadKey();
				}
				OutInven = true;			
			}
		return;
	}
	
}