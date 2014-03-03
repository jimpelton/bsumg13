// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-21                                                                      
// ******************************************************************************

namespace uGCapture
{
    class EmptyCycleMessage : Message
    {
        public EmptyCycleMessage(Receiver sender=null) : base(sender)
        {}

        public override void execute(Receiver r)
        {
            
        }
    }
}
