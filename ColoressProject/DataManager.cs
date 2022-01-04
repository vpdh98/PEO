using System;
using System.IO;
using static DataPath;


static class DataPath{
	public const String BACKGROUND_PATH = "Save/Backgrounds/";
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