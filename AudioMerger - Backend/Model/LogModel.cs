namespace AudioMerger___Backend.Model
{
    public class LogModel
    {
        //Usuario que aconteceu o erro
        public string User { get; set; }
        //Em qual arquivo o erro aconteceu
        public string ErrorFile { get; set; }
        //Mensaggem de erro
        public string ErrorMsg { get; set; }
        //Quando aconteceu
        public string Date { get; set; }

    }
}
