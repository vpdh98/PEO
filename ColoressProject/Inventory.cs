using System;
using System.Collections.Generic;
using static Define;
using static GameWindows;
using MyJson;


public class Inventory : ISaveToJson{
	public List<Item> InventoryList{get;set;}
	Choice invenCho;
	Dictionary<String,Item> invenListObject;
	Backgrounds backgrounds = new Backgrounds();
	DisplayTextGame IDTG = new DisplayTextGame(){GlobalPositionX=40,GlobalPositionY=5};
	
	int maximumCount = 20;
	
	public Inventory(){
		InventoryList = new List<Item>();
	}
	
	void Init(){
		
		Dictionary<int,Object> invenListName = new Dictionary<int,Object>();
		for(int i = 0;i<InventoryList.Count;i++){
			invenListName.Add(i,InventoryList[i].Name);
		}
		List<TextAndPosition> itemList = new List<TextAndPosition>();
		for(int i = 0;i<InventoryList.Count;i++){
			if(i < 10)
				itemList.Add(new TextAndPosition(InventoryList[i].Name,18,i+2,true));
			else
				itemList.Add(new TextAndPosition(InventoryList[i].Name,36,i-8,true));
		}
		invenListObject = new Dictionary<String,Item>();
		for(int i = 0;i<InventoryList.Count;i++){
			invenListObject.Add(InventoryList[i].Name,InventoryList[i]);
		}
	
			invenCho = new Choice(){
				Name = INVENTORY,
				SelectText = itemList,
				OnlyShowText = new List<TextAndPosition>()
							{new TextAndPosition("인벤토리",28,1),
							new TextAndPosition("버리기: D",52,1)},
				IndicateChoice = invenListName,
				BackgroundTextName = "inven"
			};
		IDTG.Init();
		IDTG.Cho = invenCho; //초기 화면
		IDTG.Show();
	}
	
	public bool AddItem(Item item){
		Item tItem;
		if(item != null && InventoryList.Count<maximumCount){
			tItem = InventoryList.Find(i => i.Name == item.Name);
			if(tItem == null)
				InventoryList.Add(item);
			else if(tItem.IsStackable){
				tItem.Amount += 1;
			}
			PlayData.player.PlayerQuestCheck(null);
			return true;
		}
		else if(item != null && item.IsStackable && (InventoryList.Find(i => i.Name == item.Name) != null)){
			tItem = InventoryList.Find(i => i.Name == item.Name);
			tItem.Amount += 1;
			return true;
		}
		else{
			return false;
		}
	}
	
	public Item GetItemIndex(int index){
		return InventoryList[index];
	}
	
	public Item GetItemName(String name){
		return InventoryList.Find(x => x.Name == name);
	}
	
	public void OpenInventory(){
		bool OutInven = false;
		Init();
		
		while(!OutInven){
				IDTG.Cho = invenCho; //초기 화면
				IDTG.Show();
				ExplanWindow(InventoryList[0],89,3,20,3);
				ConsoleKeyInfo c = Console.ReadKey();

				while(c.Key != ConsoleKey.Escape && c.Key != ConsoleKey.I && InventoryList.Count != 0)
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
								InventoryList.Remove(i);
								Init();
							}
						}
						else if(ConfirmWindow("버리시겠습니까?",24,7)){
							InventoryList.Remove(i);
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
	
	public String ToJsonString(){
		Json json = new Json();
		json.OpenObject("Inventory");
		json.AddJsonAbleList("InventoryList",InventoryList);
		json.CloseObject();
		return json.JsonString;
		
	}
	public void JsonToObject(String jsonString){
		Json json = new Json();
		json.JsonString = jsonString;
		this.InventoryList = json.GetJsonAbleList<Item>("InventoryList");
	}
	
}