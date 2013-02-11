import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;
import java.sql.Date;
import java.util.Scanner;




public class SocketController implements Runnable, PacketDefines {


	Socket client = null;
	private int port = Control.TEST_PORT;
	private boolean connected = false;
	
	@Override
	public void run() 
	{
		
		//wait to connect.
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
					e1.printStackTrace();
				}
			}
		}
		
		
		//set up our streams
		DataInputStream in = null;
		InputStream in2 = null;
		
		try {
			in2 = client.getInputStream();			
		} catch (IOException e) {
			Control.Out("Failed to get input stream.");
			connected=false;			
			e.printStackTrace();
			return;
		}
		
		in=new DataInputStream(in2);
		
		//handle incoming data.
		while(connected)
		{
			try
			{
				if(in.available()>0)
				{
					ProcessInput(in);					
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
		
		//close the socket when we are done.
		try {
			client.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	
	public void ProcessInput(DataInputStream in)
	{
		//first we figure out what type of message we are getting.
		//then we allocate an object to store the message.
		//then we pass the object to our control class for storage and processing.


		//Start reading a packet.
		try{
		
			int type = in.readInt();
			long time = in.readLong();
			
			switch(type)
			{		
			case (PACKET_TYPE_REQUEST):
				Control.Out("The client received a request (wtf?)");
				break;
			case (PACKET_TYPE_BITE):
				HandleBITEPacket(in);
				break;
			case (PACKET_TYPE_WELLDATA):
				break;
			case (PACKET_TYPE_UTILITY):
				break;
			case (PACKET_TYPE_405):
				break;
			case (PACKET_TYPE_485):
				break;
			default:
				Control.Out("Malformed Data Recieved.");
			}
		
		

		} catch (IOException e)
		{
			Control.Out("IOException while reading a packet.");
			e.printStackTrace();
		}
		
	}
	
	public void HandleBITEPacket(DataInputStream in) throws IOException
	{
		int[] codes = new int[9];
		for(int i=0;i<9;i++)
		{
			codes[i]=in.readInt();
		}
		Control.SetBITECodes(codes);		
	}
	
	public boolean DoUnitTest()
	{	
		if(client!=null&&client.isConnected())
		{
			try {
				OutputStream o = client.getOutputStream();
				DataOutputStream out = new DataOutputStream(o);
				
				for(int i =0;i<100;i++)
				{
					//debug.
					try {
						Thread.sleep(100);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
					
					out.writeInt(PACKET_TYPE_REQUEST);//request packet
					out.writeLong(System.currentTimeMillis());// current time
					out.writeInt(PACKET_TYPE_BITE);// what type of packet we want back
					out.flush();			
				}	
			} catch (IOException e) {
				Control.Out("Comm Unit Test Failed at DoUnitTest.");
				e.printStackTrace();
			}	
		}
		return false;
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
