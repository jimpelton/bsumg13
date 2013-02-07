import java.awt.BorderLayout;
import java.awt.Button;
import java.awt.Color;

import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.util.Date;

import javax.swing.BorderFactory;
import javax.swing.JPanel;





/*
 
goo·ey  
/ˈgo͞oē/
Adjective

    Soft and sticky.
    Mawkishly sentimental.

*/


public class Control extends java.awt.Frame
{
	//some defines
	public static final int FRAME_WIDTH = 1200;
	public static final int FRAME_HEIGHT = 700;
	public static final int TEST_PORT = 2560;
	public static final int CAPTURE_PORT = 2561;
	
	
	
	
	private static BITE gBITE = null;
	private static Comm gComm = null;
	private static Control gControl = null;
	
	private static BITEGUI gBITEGUI = null;
	private static CaptureGUI gCaptureGUI = null;
	private static WellStatusGUI gWellStatusGUI = null;
	private static DebugConsoleGUI gDebugConsoleGUI = null;
	private static GraphRenderingGUI gGraphRenderingGUI = null;

	private JPanel centerPanel = null;
	private JPanel topPanel = null;
	
	private Button BITEGUIButton = null;
	private Button CaptureGUIButton = null;
	private Button WellStatusGUIButton = null;
	private Button DebugConsoleGUIButton = null;
	private Button GraphRenderingGUIButton = null;
	
	
	
	//start our program.
	public static void main(String[] args) 
	{			
		gControl  = new Control();
	}
	
	public Control()
	{
		
		super();
		this.setVisible(true);
		
		//handle attempts to close us.
		addWindowListener(new WindowAdapter()
	      {
	         public void windowClosing(WindowEvent e)
	         {
	           dispose();
	           System.exit(0); //calling the method is a must
	         }
	      });
		
		//Create our debug console first.
		gDebugConsoleGUI = new DebugConsoleGUI();
		
		//dump our date and time into our output.
		
		String scratchString = new Date().toString();
		Out(scratchString);
		
		//create our objects.
		
		
		
		gBITEGUI = new BITEGUI();
		gCaptureGUI = new CaptureGUI();
		gWellStatusGUI = new WellStatusGUI();
		gGraphRenderingGUI = new GraphRenderingGUI();
		
		SetUpGUI();
		
		
		//Things to do before we run BITE
		
		//initialize our communications interface.
		gComm = new Comm();
		
		
	}
	
	public void SetUpGUI()
	{
		//set up our buttons for switching between guis.
		SetUpTopBar();
		
		//now the rest of the startup goodness.
		
		//we start with the debug console loaded so that 
		//we can see things that are going on
		centerPanel.add(gDebugConsoleGUI);
		
		
		this.setSize(FRAME_WIDTH, FRAME_HEIGHT);//default size
		this.validate();
	}
	
	public void SetUpTopBar()
	{
		
		this.setLayout(new BorderLayout(4, 4));
		topPanel = new JPanel();
		centerPanel = new JPanel();
		add(topPanel, BorderLayout.NORTH);
		add(centerPanel, BorderLayout.CENTER);
		
		BITEGUIButton = new Button("BITE");
		CaptureGUIButton = new Button("Capture");
		WellStatusGUIButton = new Button("Well Status");
		DebugConsoleGUIButton = new Button("Debug Console");
		GraphRenderingGUIButton = new Button("Graphs");
				
		topPanel.setBorder(BorderFactory.createLineBorder(Color.black));
		centerPanel.setBorder(BorderFactory.createLineBorder(Color.black));
		topPanel.add(BITEGUIButton);
		topPanel.add(CaptureGUIButton);
		topPanel.add(WellStatusGUIButton);
		topPanel.add(DebugConsoleGUIButton);
		topPanel.add(GraphRenderingGUIButton);
		
	}

	//dumps to our debug output
	public synchronized static void Out(String s)
	{
		if(gDebugConsoleGUI!=null)
			gDebugConsoleGUI.appendMessage(s);
	}
}
