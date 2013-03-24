﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Timers;
using Phidgets;
using Phidgets.Events;

namespace uGCapture 
{
    public class CaptureClass : Receiver
    {
        private const int FRAME_TIME = 500;
        
        private Queue<Message> messages;
        private Dispatch dispatch;
        private Receiver[] controllers;

        private BufferPool<Byte> StagingBuffer; 


        private Timer ticker = null;

        public CaptureClass()
        {
            StagingBuffer = new BufferPool<byte>(100);
            Receiver.StagingBuffer = StagingBuffer;
            messages = new Queue<Message>();

            dispatch = Dispatch.Instance();
            controllers = new Receiver[7];
            controllers[0] = new NIController();
            controllers[1] = new UPSController();
            controllers[2] = new VCommController();
            controllers[3] = new AptinaController();
            controllers[4] = new PhidgetsController();
            controllers[5] = new AccelerometerController();
            controllers[6] = new AccelerometerController();
            
            ticker = new Timer(FRAME_TIME);
            ticker.Elapsed += new ElapsedEventHandler(DoFrame);
            ticker.Enabled = true;
        }

        public void DoFrame(object source, ElapsedEventArgs e)
        {
           
        }


        public void LogDebugMessage(String s)
        {
            LogDebugMessage(s, 0);
        }

        public void LogDebugMessage(String s, int severtity)
        {
            Dispatch.Instance().Broadcast
            (
                new LogMessage(this, s, severtity)
            );
        }

        public Buffer<Byte> GetEmptyByteBuffer()
        {
            return StagingBuffer.PopEmpty();
        }

        public void submitFilledByteBuffer(Buffer<Byte> fullbuf)
        {
            StagingBuffer.PostFull(fullbuf);
        }
    }
}
