import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;


public class BITESocketServer implements Runnable, PacketDefines{
	
	public final int PORT = Control.TEST_PORT;
	private ServerSocket server = null;
	private Socket client = null;
	
	private InputStream istream = null;
	private OutputStream ostream = null;	
	private DataInputStream in = null;
	private DataOutputStream out = null;
	
	
	@Override
	public void run() {
		
		
		try {
			server = new ServerSocket(PORT);
			Control.Out("Test Server Started.");
			client = server.accept();
			Control.Out("Test Server Connected.");
		} catch (IOException e) {
			Control.Out("Test Server Failed.");
			e.printStackTrace();
			return;
		}
		
	
		try{
			ostream = client.getOutputStream();
			istream = client.getInputStream();
		} catch (IOException e1) {
			Control.Out("Test Server Failed.");
			e1.printStackTrace();
		}
		
		
		in = new DataInputStream(istream);
		out = new DataOutputStream(ostream);
		
		
		Thread commUnitTest = new Thread(new BITESocketCommUnitTestThread());
		commUnitTest.start();

		
		//This method returns when the socket is no-longer connected.
		InputLoop();
		

	
		
		try {
			client.close();
		} catch (IOException e) {
			Control.Out("Test Server Failed.");
			e.printStackTrace();
		}
		try {
			server.close();
		} catch (IOException e) {
			Control.Out("Test Server Failed.");
			e.printStackTrace();
		}
		
	}

	private void InputLoop()
	{
		while(client.isConnected())
		{			
			try
			{
				if(istream.available()>0)
				{
					//Control.Out("Server Accepting Data.");
					//Start reading a packet.
					int type = in.readInt();
					long time = in.readLong();
					
					//since we are the server
					//check to see if this
					//is a request before
					//doing anything
					
					if(type==PACKET_TYPE_REQUEST)
					{
						int req = in.readInt();
						
						
						if(req==PACKET_TYPE_BITE)//BITE Request
						{
							out.writeInt(PACKET_TYPE_BITE);
							out.writeLong(System.currentTimeMillis());
							for(int i=0;i<9;i++)
								out.writeInt(1);
						}						
						if(req==PACKET_TYPE_WELLDATA)//Well Data Request
						{
							out.writeInt(PACKET_TYPE_WELLDATA);
							out.writeLong(System.currentTimeMillis());
						}						
						if(req==PACKET_TYPE_UTILITY)//Utility Data Request
						{
							out.writeInt(PACKET_TYPE_UTILITY);
							out.writeLong(System.currentTimeMillis());
						}						
						if(req==PACKET_TYPE_405)//485 image data request
						{
							out.writeInt(PACKET_TYPE_405);
							out.writeLong(System.currentTimeMillis());
						}						
						if(req==PACKET_TYPE_485)//405 image data request
						{
							out.writeInt(PACKET_TYPE_485);
							out.writeLong(System.currentTimeMillis());
						}
						
						out.flush();
						
					}
				}	
			} catch (IOException e1) {
				Control.Out("Test Server Failed.");
				e1.printStackTrace();
			}
		}
		
	}
	
}
