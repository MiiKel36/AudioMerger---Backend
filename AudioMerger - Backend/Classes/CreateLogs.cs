using AudioMerger___Backend.Model;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AudioMerger___Backend.Classes
{
    public class CreateLogs
    {
        //Usuario que aconteceu o erro
        public string User { get; set; }
        //Em qual arquivo o erro aconteceu
        public string ErrorFile { get; set; }
        //Mensaggem de erro
        public string ErrorMsg { get; set; }
        //Quando aconteceu
        public string Date { get; set; }


        //Server path variables
        public string serverDirectory = Environment.CurrentDirectory;
        public string logFileName = "Logs\\Logs.json";
        public string fullPath;

        public void CreateLog(LogModel logModel)
        {
            fullPath = Path.Combine(serverDirectory, logFileName);

            string strJsonFile = File.ReadAllText(fullPath);//read json file
            var jsonList = JsonConvert.DeserializeObject <List<LogModel>> (strJsonFile);//trasforf into a list

            jsonList.Add(logModel);//add new log
            strJsonFile = JsonConvert.SerializeObject(jsonList);//trasform into a json

            File.WriteAllText(fullPath, strJsonFile);//rewrite the Logs.json file

            
        }

    }
}
