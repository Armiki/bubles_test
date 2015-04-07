using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client : MonoBehaviour {

	private string ip;
	public void ConnectToServer(string _ip){
		ip = _ip;
		StartCoroutine("SendMessageFromSocket");
	}

	IEnumerator SendMessageFromSocket(){
		// Буфер для входящих данных
		byte[] bytes = new byte[1024];
		
		// Соединяемся с удаленным устройством
		
		// Устанавливаем удаленную точку для сокета
		IPHostEntry ipHost = Dns.GetHostEntry(ip);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 6666);

		Debug.Log("пингуем точку");
		if (!Security.PrefetchSocketPolicy(ip, 6666, 100)){
			yield return null;
			Destroy(gameObject);
		}
		
		Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		Debug.Log("Соединяем сокет с удаленной точкой");
		sender.Connect(ipEndPoint);
		yield return sender.Connected;

		string message = "lool";

		byte[] msg = Encoding.UTF8.GetBytes(message);
		
		// Отправляем данные через сокет
		int bytesSent = sender.Send(msg);
		
		// Получаем ответ от сервера
		int bytesRec = sender.Receive(bytes);
		
		//Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));
		
		// Используем рекурсию для неоднократного вызова SendMessageFromSocket()
//		if (message.IndexOf("<TheEnd>") == -1)
//			SendMessageFromSocket();
		
		// Освобождаем сокет
		sender.Shutdown(SocketShutdown.Both);
		sender.Close();
	}
}
