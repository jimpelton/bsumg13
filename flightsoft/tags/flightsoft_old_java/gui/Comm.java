
public class Comm {

	private static SocketController sock = null;
	
	public Comm()
	{		
		
		Thread t = new Thread(sock = new SocketController());
		Control.Out("Socket Client Thread Created.");
		t.start();
	}
	
	public static void DoUnitTest()
	{
		Control.Out("Starting Comm Unit Test");
		sock.DoUnitTest();
	}
}
