using System;
namespace uGCapture
{

    //class Sender
    //{
    //    Sender() { /* empty (for now) */ }
    //}

    public abstract class Receiver
    {

        /// <summary>
        /// Any receiver that should respond to the Bite test message should
        /// override this method.
        /// This method should generate a BiteTestResultMessage, however the default behavior 
        /// is to do nothing.
        /// </summary>
        /// <param name="r"></param>
        public virtual void exBiteTest(Receiver r) { ; }

        /// <summary>
        /// Generate a PhidgetsStatusMessage
        /// Default behavior is to do nothing.
        /// </summary>
        /// <param name="r"></param>
        public virtual void exPhidgetsStatus(Receiver r) { ; }

        /// <summary>
        /// Generate an AptinaStatusMessage
        /// Default behavior is to do nothing.
        /// </summary>
        /// <param name="r"></param>
        public virtual void exAptinaStatus(Receiver r) { ; }

        public override string ToString()
        {
            return "Base Receiver";
        }
    }

    //class PhidgetsAccelAccessor : Receiver
    //{

    //    public override void exPhidgetsStatus(Receiver r)
    //    {
    //        Console.WriteLine("status: " + this);
    //        //base.exPhidgetsStatus(r);
    //    }

    //    public override void exBiteTest(Receiver r)
    //    {
    //        //respond in some way to bite test command.
    //        Console.WriteLine("BiteTest: " + this);
    //    }

    //    public override string ToString()
    //    {
    //        return "PhidgetsAccelAccessor";
    //    }

    //}

    //class AptinaAccessor : Receiver
    //{

    //    public override void exAptinaStatus(Receiver r)
    //    {
    //        Console.WriteLine("status: " + this);   
    //    }

    //    public override void exBiteTest(Receiver r)
    //    {
    //        //respond in some way to bite test command.
    //        Console.WriteLine("BiteTest: " + this);
    //    }

    //    public override string ToString()
    //    {
    //        return "AptinaAccessor";
    //    }
    //}




}