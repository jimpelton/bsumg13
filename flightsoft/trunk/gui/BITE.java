
public class BITE 
{


	private int[] BITECodes;

	
	public BITE()
	{
		BITECodes = new int[9];
		
		StartTest();
	}
	
	public void StartTest()
	{
		BITESocketServer SS = new BITESocketServer();
		
		Thread SSTestThread = new Thread(SS);
		SSTestThread.start();

	}
	
	public void SetBITECodes(int[] codes)
	{
		if(codes.length==9)
		{
			BITECodes = codes;
		}
	}
}
