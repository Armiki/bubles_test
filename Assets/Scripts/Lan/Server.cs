using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server : MonoBehaviour {

	Socket sListener;

	public void Init(){
		IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 6666);
		sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		sListener.Bind(ipEndPoint);
		sListener.Listen(10);
		Debug.Log("Ожидаем соединение через порт " + ipEndPoint);
	}

	public void FixedUpdate(){
		//if (sListener.){ //ждем соединения
			Socket handler = sListener.Accept(); 
			string data = null;
			
			// Мы дождались клиента, пытающегося с нами соединиться
			
			byte[] bytes = new byte[1024];
			int bytesRec = handler.Receive(bytes);
			
			data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
			
			// Показываем данные на консоли
			Debug.Log("Полученный текст: " + data + "\n\n");
			
			// Отправляем ответ клиенту\
			string reply = "Спасибо за запрос в " + data.Length.ToString()
				+ " символов";
			byte[] msg = Encoding.UTF8.GetBytes(reply);
			handler.Send(msg);
			
			if (data.IndexOf("<TheEnd>") > -1){
				Debug.Log("Сервер завершил соединение с клиентом.");
			}
			
			//handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		//}
	}

	public void Close(){
		sListener.Shutdown(SocketShutdown.Both);
	}
}
