using System;
using System.IO;
using static DataPath;
using Characters;
using System.Collections.Generic;
using MyJson;


static class DataPath{
	public const String BACKGROUND_PATH = "Save/Backgrounds/";
	public const String ENEMY_PATH = "Save/Characters/Enemys.txt";
	public const String NPC_PATH = "Save/Characters/NPCs.txt";
	public const String PLAYER_PATH = "Save/Characters/Players.txt";
}

public static class DataManager{
	
	
	public static String LoadBackground(String backgroundName){
		String temp = "";
		StreamReader sr = new StreamReader(BACKGROUND_PATH+backgroundName+".txt");
		while(sr.Peek() >= 0){
			temp += (Char)sr.Read();
		}
		sr.Close();
		return temp;
	}
	
	public static CharacterListControler LoadCharacters(){
		CharacterListControler characters = new CharacterListControler();
		characters.SetEnemyDictionaryAsList(LoadEnemy());
		characters.SetNpcDictionaryAsList(LoadNPC());
		characters.SetPlayerDictionaryAsList(LoadPlayer());
		return characters;
	}
	
	public static List<Enemy> LoadEnemy(){
		List<Enemy> list = new List<Enemy>();
		String jsonString = ReadFile(DataPath.ENEMY_PATH);
		Json json = new Json();
		json.JsonString = jsonString;
		list = json.GetJsonAbleList<Enemy>("Enemys");
		return list;
	}
	
	public static List<NPC> LoadNPC(){
		List<NPC> list = new List<NPC>();
		String jsonString = ReadFile(DataPath.NPC_PATH);
		Json json = new Json();
		json.JsonString = jsonString;
		list = json.GetJsonAbleList<NPC>("NPCs");
		return list;
	}
	
	public static List<Player> LoadPlayer(){
		List<Player> list = new List<Player>();
		String jsonString = ReadFile(DataPath.PLAYER_PATH);
		Json json = new Json();
		json.JsonString = jsonString;
		list = json.GetJsonAbleList<Player>("Players");
		return list;
	}
	
	///<summary>
	///매개변수로 전달받은 경로에 디렉토리와 파일을 생성한다.
	///해당 경로에 해당 파일이 이미 존재한다면 생성하지 않는다.
	///</summary>
	///<param name = filePath>'/'로 구분된 생성할 파일의 경로와 파일 이름을 전달</param>
	public static void CreateDirectoryAndFile(String filePath){
		String directoryPath = filePath.Substring(0,filePath.LastIndexOf("/")+1);
		String fileName = filePath.Substring(filePath.LastIndexOf("/")+1);
		
		if(!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}
		if(File.Exists(filePath)) return;//throw new Exception("이미 존재하는 파일");
		using(File.Create(filePath)){}
	}
	
	public static String ReadFile(String filePath)
	{
		if(!File.Exists(filePath)) throw new Exception("파일이 없습니다.");
		
		String data;
		using(StreamReader sr = new StreamReader(filePath))
		{
			data = sr.ReadToEnd();
		}
		return data;
	}
	
	public static void WriteFile(String filePath,String data,FileMode mode = FileMode.Append)
	{
		try{
			using(StreamWriter sw = new StreamWriter(new FileStream(filePath,mode)))
			{

				sw.Write(data);

			}
		}catch(IOException e){
			//이미 있는 파일이면 아무것도 안함
		}
	}
	/*void DeleteAll()
	void DeleteKey(string name)
	bool HasKey(string name)
	Item GetItem(string name)
	int GetInt(string name)
	string GetString(string name)
	void SetFloat(string name, float value)
	void SetInt(string name, int value)
	void SetString(string name)*/
}