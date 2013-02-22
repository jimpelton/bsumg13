
#ifndef _WORKERTHREAD_H
#define _WORKERTHREAD_H


#ifdef _WIN32
#include <Windows.h>
#endif

#include "Export.h"
#include <assert.h>
#include <boost/thread.hpp>




namespace uG
{

    class  WorkerThread
    {
    public:

        /**
         *  \fn Worker(DWORD (WINAPI *pfun)(void*), void *parg);
         *  \brief yar! Thready!!!
         *  \param pfun The worker function.
         *  \param parg Pointer to arguments for pfun.
         */
        WorkerThread(DWORD (WINAPI *pfun)(void*), void *parg)
            : m_stoprequested(false)
            , m_running(false)
            , m_tid()
        {
            InitializeCriticalSection(&m_criticalSection);
            m_thread = CreateThread(
                        0,
                        0,
                        pfun,
                        parg,
                        CREATE_SUSPENDED,
                        m_tid);
        }

        virtual ~WorkerThread()
        {
            DeleteCriticalSection(&m_criticalSection);
            CloseHandle(m_thread);
        }

        /**
         * \fn virtual void join();
         * \brief Set running status as stopped and Wait for this Worker's
         *        thread to be finished. Then join the thread.
         *        Calling join() on a Worker that is not running has no effect.
         */
        virtual void join()
        {
            EnterCriticalSection(&m_criticalSection);
            if (!m_running)
            {
                LeaveCriticalSection(&m_criticalSection);
                return;
            }
            m_running = false;
            m_stoprequested = true;
            LeaveCriticalSection(&m_criticalSection);
            WaitForSingleObject(m_thread, INFINITE);
        }

        /**
         * \fn virtual void go();
         * \brief Set this thread as running and start execution.
         *        Calling go() on a running thread has no effect.
         *       
         */
        virtual void go()
        {
            EnterCriticalSection(&m_criticalSection);
            if (m_running) {
                LeaveCriticalSection(&m_criticalSection);
                return;
            }
            m_running = true;
            m_stoprequested = false;
            LeaveCriticalSection(&m_criticalSection);
            ResumeThread(m_thread);
            std::cout << "Started thread " << 
                GetThreadId(m_thread) << std::endl;
        }

    protected:
        bool m_stoprequested;
        bool m_running;
        boost::thread m_thread;

        
        LPDWORD m_tid;
        HANDLE m_thread;
        CRITICAL_SECTION m_criticalSection;
    };

} /* namespace uG */
#endif



