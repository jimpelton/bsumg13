import java.awt.BorderLayout;
import java.awt.GridLayout;
import java.awt.Panel;

import javax.swing.JLabel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;


public class DebugConsoleGUI extends Panel 
{
	JTextArea textArea = null;
	
	
	public void appendMessage(String s)
	{
		if(textArea!=null)
			textArea.append(s+"\n");
	}
	
	public DebugConsoleGUI()
	{
	
		this.setLayout(new BorderLayout(4, 4));
		JLabel l = new JLabel("Debug Console");
		textArea = new JTextArea(38,80);
		textArea.setLineWrap(false);
		JScrollPane scrollPane = new JScrollPane(textArea); 
		textArea.setEditable(false);
		add(l, BorderLayout.NORTH);
		add(scrollPane, BorderLayout.CENTER);
		scrollPane.setAutoscrolls(true);
	}
	
}
