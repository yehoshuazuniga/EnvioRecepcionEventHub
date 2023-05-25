namespace ConsoleApp1
{
    public class Program
    {

        private const string ehubNamespaceConnectionString = "";
        private const string eventHubName = "";
        private const string blobStorageConnectionString = "";
        private const string blobContainerName = "";

        public static void Main()
        {

            bool continuar = true;

            do
            {
                Console.WriteLine(" ----------- Elige una opcion ----------- ");
                Console.WriteLine("Preciona el \"1\"para enviar eventos \n"+
                    "Preciona el \"2\"para recepcion de eventos \n"+
                    "Preciona cualquier tecla para salir\n"
                    ); 
                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        Console.Clear();
                        EventSender envio = new EventSender();
                        envio.envioEventos(ehubNamespaceConnectionString,eventHubName);
                        break;
                    case "2":
                        Console.Clear();
                       EventReceptor receptor= new EventReceptor();
                        receptor.recepcionEvento(ehubNamespaceConnectionString,eventHubName,blobStorageConnectionString,blobContainerName);
                        break;
                    default: 
                        continuar = false;
                        break;
                }


            } while (continuar);

            
        }
    }
}