using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceProcess;

namespace Interrupt.IrcBot
{
    /// <summary>
    /// Provides common functionality needed by the external services.
    /// </summary>
    public abstract class WorkerServiceBase : ServiceBase
    {
        private List<Thread> m_Threads = new List<Thread>();
        private volatile bool m_IsQuitting = false;

        /// <summary>
        /// Returns whether the service is being stopped.
        /// </summary>
        protected bool IsQuitting
        {
            get { return m_IsQuitting; }
        }

        /// <summary>
        /// Returns all the threads.
        /// </summary>
        protected Thread[] Threads
        {
            get { return m_Threads.ToArray(); }
        }

        /// <summary>
        /// Creates the defined number of threads.
        /// </summary>
        /// <param name="count">The number of threads to create.</param>
        /// <param name="function">The function each thread will be executing.</param>
        protected void CreateThreads(int count, Action function)
        {
            CreateThreads(count, function, null);
        }

        /// <summary>
        /// Creates the defined number of threads.
        /// </summary>
        /// <param name="count">The number of threads to create.</param>
        /// <param name="function">The function each thread will be executing.</param>
        /// <param name="name">The name to assign to the threads.</param>
        protected void CreateThreads(int count, Action function, string name)
        {

            // check for invalid parameters
            if (count > 10) throw new ArgumentOutOfRangeException("count");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (count == 0) return;

            // create the desired number of threads
            for (int i = 0; i < count; i++)
            {
                Thread thread = new Thread(ThreadFunction);
                thread.Name = name;
                m_Threads.Add(thread);
                thread.Start(function);
            }

        }

        /// <summary>
        /// Stops the execution of the threads.
        /// </summary>
        protected void StopThreads()
        {
            StopThreads(null);
        }

        /// <summary>
        /// Stops the execution of the threads.
        /// </summary>
        /// <param name="function">The function to call for waking threads from their wait state.</param>
        protected void StopThreads(Action function)
        {

            // indicate shutdown status
            m_IsQuitting = true;

            // interrupt each thread if possible
            foreach (Thread thread in Threads)
            {
                thread.Interrupt();
            }

            // handle any other operation needed to unlock the threads
            if (function != null) function();

            // wait for each thread to complete
            foreach (Thread thread in Threads)
            {
                thread.Join();
            }

        }

        /// <summary>
        /// Executes the reader and writer functions.
        /// </summary>
        /// <param name="functionPointer">The function delegate pointer.</param>
        private void ThreadFunction(object functionPointer)
        {

            // initialize the variables and pointers
            Action function = functionPointer as Action;

            // process until asked to stop
            while (IsQuitting == false)
            {
                try
                {
                    function();

                }
                catch (ThreadInterruptedException)
                {

                }
                catch (ThreadAbortException)
                {

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }
}
