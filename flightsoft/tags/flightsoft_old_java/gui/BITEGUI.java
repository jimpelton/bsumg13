import java.awt.BorderLayout;
import java.awt.Panel;


public class BITEGUI extends Panel {


	private static final long serialVersionUID = -5558501933449528808L;

	public BITEGUI()
	{
		super();
		this.setLayout(new BorderLayout());
		BITEHUD hud = Control.getBITEHUD();
		if(hud!=null)
		{
			add(hud,BorderLayout.SOUTH);
		}
		
	}
}
