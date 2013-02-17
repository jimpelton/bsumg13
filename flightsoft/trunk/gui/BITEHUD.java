import java.awt.Color;
import java.awt.Dimension;
import java.awt.GridLayout;


import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;


public class BITEHUD extends JPanel
{
	private static final long serialVersionUID = -5249518219779454833L;
	private static final int HUDWIDTH = 800;
	private static final int HUDHEIGHT = 80;
	private JButton bNI = null;
	private JButton bPhidgets = null;
	private JButton bAccelVib = null;
	private JButton bAccelCap = null;
	private JButton bWeather = null;
	private JButton bPower = null;
	private JButton bTemp = null;
	private JButton bCam405 = null;
	private JButton bCam485 = null;
	
	public BITEHUD()
	{
		super();
		this.setVisible(true);		
		this.setSize(HUDWIDTH, HUDHEIGHT);
		this.setLayout(new GridLayout(2,5));
		super.setBorder(BorderFactory.createLineBorder(Color.black));
		
		bNI= new JButton("NI DAQ");
		bPhidgets= new JButton("Phidgets 8/8/8");
		bAccelVib= new JButton("Accel-Vibration");
		bAccelCap= new JButton("Accel-Gravity");
		bWeather= new JButton("Barometer");
		bPower= new JButton("Power Supply");
		bTemp= new JButton("Temperature");
		bCam405= new JButton("Camera-405nm");
		bCam485= new JButton("Camera-485nm");
		
		
		add(bNI);
		add(bPhidgets);
		add(bAccelVib);
		add(bAccelCap);
		add(bWeather);
		add(bPower);
		add(bTemp);
		add(bCam405);
		add(bCam485);
		
		/*
		bNI.setEnabled(false);
		bPhidgets.setEnabled(false);
		bAccelVib.setEnabled(false);
		bAccelCap.setEnabled(false);
		bWeather.setEnabled(false);
		bPower.setEnabled(false);
		bTemp.setEnabled(false);
		bCam405.setEnabled(false);
		bCam485.setEnabled(false);
		*/
		
		bNI.setSize(200, 40);
		bPhidgets.setSize(200, 40);
		bAccelVib.setSize(200, 40);
		bAccelCap.setSize(200, 40);
		bWeather.setSize(200, 40);
		bPower.setSize(200, 40);
		bTemp.setSize(200, 40);
		bCam405.setSize(200, 40);
		bCam485.setSize(200, 40);
		
		this.setMaximumSize(new Dimension(HUDWIDTH, HUDHEIGHT));
		this.setPreferredSize(new Dimension(HUDWIDTH, HUDHEIGHT));
		this.setSize(HUDWIDTH, HUDHEIGHT);
	}
	
	public void updateColoration(int[] warns, boolean critical)
	{
		if(warns.length==9)
		{
			if(!critical)//If we are not in a critical section then the coloration isn't all reds.
			{
				if(warns[0]==0)
					bNI.setBackground(Color.LIGHT_GRAY);
				else if(warns[0]==1)
					bNI.setBackground(Color.YELLOW);
				else 
					bNI.setBackground(Color.RED);
				
				if(warns[1]==0)
					bPhidgets.setBackground(Color.LIGHT_GRAY);
				else if(warns[1]==1)
					bPhidgets.setBackground(Color.YELLOW);
				else 
					bPhidgets.setBackground(Color.RED);
				
				if(warns[2]==0)
					bAccelVib.setBackground(Color.LIGHT_GRAY);
				else if(warns[2]==1)
					bAccelVib.setBackground(Color.YELLOW);
				else 
					bAccelVib.setBackground(Color.RED);
				
				if(warns[3]==0)
					bAccelCap.setBackground(Color.LIGHT_GRAY);
				else if(warns[3]==1)
					bAccelCap.setBackground(Color.YELLOW);
				else 
					bAccelCap.setBackground(Color.RED);
				
				if(warns[4]==0)
					bWeather.setBackground(Color.LIGHT_GRAY);
				else if(warns[4]==1)
					bWeather.setBackground(Color.YELLOW);
				else 
					bWeather.setBackground(Color.RED);
							
				if(warns[5]==0)
					bPower.setBackground(Color.LIGHT_GRAY);
				else if(warns[5]==1)
					bPower.setBackground(Color.YELLOW);
				else 
					bPower.setBackground(Color.RED);
				
				
				if(warns[6]==0)
					bTemp.setBackground(Color.LIGHT_GRAY);
				else if(warns[6]==1)
					bTemp.setBackground(Color.YELLOW);
				else 
					bTemp.setBackground(Color.RED);
				
				
				if(warns[7]==0)
					bCam405.setBackground(Color.LIGHT_GRAY);
				else if(warns[7]==1)
					bCam405.setBackground(Color.YELLOW);
				else 
					bCam405.setBackground(Color.RED);
				
				
				if(warns[8]==0)
					bCam485.setBackground(Color.LIGHT_GRAY);
				else if(warns[8]==1)
					bCam485.setBackground(Color.YELLOW);
				else 
					bCam485.setBackground(Color.RED);
			}
			else
			{
				if(warns[0]==0)
					bNI.setBackground(Color.GRAY);
				else 
					bNI.setBackground(Color.RED);
				
				if(warns[1]==0)
					bPhidgets.setBackground(Color.GRAY);
				else 
					bPhidgets.setBackground(Color.RED);
				
				if(warns[2]==0)
					bAccelVib.setBackground(Color.GRAY);
				else 
					bAccelVib.setBackground(Color.RED);
				
				if(warns[3]==0)
					bAccelCap.setBackground(Color.GRAY);
				else 
					bAccelCap.setBackground(Color.RED);
				
				if(warns[4]==0)
					bWeather.setBackground(Color.GRAY);
				else 
					bWeather.setBackground(Color.RED);
							
				if(warns[5]==0)
					bPower.setBackground(Color.GRAY);
				else 
					bPower.setBackground(Color.RED);
				
				
				if(warns[6]==0)
					bTemp.setBackground(Color.GRAY);
				else 
					bTemp.setBackground(Color.RED);
				
				
				if(warns[7]==0)
					bCam405.setBackground(Color.GRAY);
				else 
					bCam405.setBackground(Color.RED);
				
				
				if(warns[8]==0)
					bCam485.setBackground(Color.GRAY);
				else 
					bCam485.setBackground(Color.RED);
			}
		}
	}
}
