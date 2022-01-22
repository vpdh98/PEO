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
		using(StreamWriter sw = new StreamWriter(new FileStream(filePath,mode)))
		{
			sw.Write(data);
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