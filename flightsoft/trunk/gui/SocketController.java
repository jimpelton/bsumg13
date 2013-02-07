import java.io.IOException;
import java.io.InputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;




public class SocketController implements Runnable {


	Socket client = null;
	private int port = Control.TEST_PORT;
	private boolean connected = false;
	
	@Override
	public void run() 
	{
		while(!connected)
		{
			try {
				client = new Socket("127.0.0.1", port);
				Control.Out("Socket connected successfully");
				connected=true;
			} catch (UnknownHostException e) {
				Control.Out("Socket UnknownHostException");
				e.printStackTrace();
			} catch (IOException e) {
				try {
					Thread.sleep(5000);
					Control.Out("Waiting for sockets connection.");
				} catch (InterruptedException e1) {
					// TODO Auto-generated catch block
					e1.printStackTrace();
				}
			}
		}
		
		InputStream in = null;
		try {
			in = client.getInputStream();
			
		} catch (IOException e) {
			Control.Out("Failed to get input stream.");
			e.printStackTrace();
		}
		
		while(connected)
		{
			try
			{
				if(in.available()>0)
				{
					//first we figure out what type of message we are getting.
					//then we allocate an object to store the message.
					//then we pass the object to our control class for storage and processing.
					
					//debug
					Control.Out(""+in.available()+" bytes available to be read.");
					try {
						Thread.sleep(1000);						
					} catch (InterruptedException e1){}
				}
			}catch (IOException e)
			{
				
			}
		}
	}
	
	public void disconnect()
	{
		try {
			connected=false;
			client.close();
			Control.Out("Socket closed");
		} catch (IOException e) {
			Control.Out("Socket IOException");
			e.printStackTrace();
		}
	}
	
	public void setPort(int p)
	{
		port=p;
	}

}
