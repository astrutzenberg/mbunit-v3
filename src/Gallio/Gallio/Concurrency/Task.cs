// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Threading;
using Gallio.Utilities;

namespace Gallio.Concurrency
{
    /// <summary>
    /// <para>
    /// A task represents a concurrently executing operation.
    /// </para>
    /// <para>
    /// A <see cref="Task" /> might not necessarily represent an operation that is
    /// executing in a local <see cref="Thread" />.  It can represent other processes
    /// that execute remotely or that are represented by some other mechanism.
    /// </para>
    /// </summary>
    /// <seealso cref="TaskContainer"/>
    public abstract class Task
    {
        private readonly string name;
        private EventHandler<TaskEventArgs> started;
        private EventHandler<TaskEventArgs> aborted;
        private EventHandler<TaskEventArgs> terminated;

        private bool isStarted;
        private bool isAborted;
        private TaskResult result;

        /// <summary>
        /// Creates a task.
        /// </summary>
        /// <param name="name">The name of the task</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public Task(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Returns true if the task has not been started or aborted yet.
        /// </summary>
        public bool IsPending
        {
            get { return ! isStarted && ! isAborted; }
        }

        /// <summary>
        /// Returns true if the task is running.
        /// </summary>
        public bool IsRunning
        {
            get { return isStarted && result == null; }
        }

        /// <summary>
        /// Returns true if the task has been aborted.
        /// </summary>
        public bool IsAborted
        {
            get { return isAborted; }
        }

        /// <summary>
        /// Gets the task result, or null if the task has not terminated or was aborted before starting.
        /// </summary>
        public TaskResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// Adds or removes an event handler that is signaled when the task is started.
        /// If a handler is being added and the task has already started, it is immediately invoked.
        /// </summary>
        public event EventHandler<TaskEventArgs> Started
        {
            add { AddHandler(ref started, value); }
            remove { RemoveHandler(ref started, value); }
        }

        /// <summary>
        /// Adds or removes an event handler that is signaled when the task is aborted.
        /// If a handler is being added and the task has already aborted, it is immediately invoked.
        /// </summary>
        public event EventHandler<TaskEventArgs> Aborted
        {
            add { AddHandler(ref aborted, value); }
            remove { RemoveHandler(ref aborted, value); }
        }

        /// <summary>
        /// Adds or removes an event handler that is signaled when the task is terminated.
        /// If a handler is being added and the task has already terminated, it is immediately invoked.
        /// </summary>
        public event EventHandler<TaskEventArgs> Terminated
        {
            add { AddHandler(ref terminated, value); }
            remove { RemoveHandler(ref terminated, value); }
        }
        
        /// <summary>
        /// <para>
        /// Starts running the task.
        /// </para>
        /// <para>
        /// Does nothing if the task has already been started or has been aborted.
        /// </para>
        /// </summary>
        /// <seealso cref="IsPending"/>
        public void Start()
        {
            lock (this)
            {
                if (! IsPending)
                    return;
                isStarted = true;
            }

            try
            {
                try
                {
                    StartImpl();
                }
                finally
                {
                    Notify(ref started);
                }
            }
            catch (Exception ex)
            {
                NotifyTerminated(TaskResult.CreateFromException(ex));
            }
        }

        /// <summary>
        /// <para>
        /// Asynchronously aborts the task.
        /// </para>
        /// <para>
        /// If the task has not been started, then the task will be forbidden from starting later
        /// and its <see cref="IsAborted" /> property will be set.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// A typical implementation of this method may cause a <see cref="ThreadAbortException" />
        /// to be thrown non-deterministically within the task's thread.  Normally such an exception should
        /// cause the thread to abort abruptly but it may have no effect in certain conditions.
        /// Also, aborting a task in this manner may cause the application to enter an invalid state
        /// because exception handlers are often not designed to recover from <see cref="ThreadAbortException" />
        /// correctly.
        /// </para>
        /// </remarks>
        /// <seealso cref="System.Threading.Thread.Abort()"/>
        /// <seealso cref="IsAborted"/>
        public void Abort()
        {
            lock (this)
            {
                if (isAborted)
                    return;

                isAborted = true;
            }

            try
            {
                AbortImpl();
            }
            finally
            {
                Notify(ref aborted);
            }
        }

        /// <summary>
        /// <para>
        /// Waits for the task to terminate.
        /// </para>
        /// <para>
        /// Does nothing if the task has not been started or is not running.
        /// </para>
        /// </summary>
        /// <param name="timeout">The timeout</param>
        /// <returns>True if the task is not running as of the time this method exits,
        /// false if a timeout occurred while waiting</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/>
        /// represents a negative time span</exception>
        /// <seealso cref="System.Threading.Thread.Join(TimeSpan)"/>
        /// <seealso cref="IsRunning"/>
        public bool Join(TimeSpan timeout)
        {
            if (timeout.Ticks < 0)
                throw new ArgumentOutOfRangeException("timeout");

            return JoinImpl(timeout);
        }

        /// <summary>
        /// Starts the task and waits for it to complete until the timeout expires.
        /// If the timeout expires, aborts the task and returns <c>false</c>.
        /// </summary>
        /// <param name="timeout">The timeout</param>
        /// <returns>True if the task ran to completion within the specified time span,
        /// false if the task was aborted</returns>
        public bool Run(TimeSpan timeout)
        {
            Start();

            if (Join(timeout))
                return true;

            Abort();
            return false;
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <seealso cref="Start"/>
        protected abstract void StartImpl();

        /// <summary>
        /// Aborts the task.
        /// </summary>
        /// <seealso cref="Abort"/>
        protected abstract void AbortImpl();

        /// <summary>
        /// Waits for the task to terminate.
        /// </summary>
        /// <param name="timeout">The timeout</param>
        /// <returns>True if the task is not running as of the time this method exits,
        /// false if a timeout occurred while waiting</returns>
        protected abstract bool JoinImpl(TimeSpan timeout);

        /// <summary>
        /// Dispatches notification that the task has terminated and provides its result.
        /// </summary>
        /// <param name="result">The task result</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is not currently running</exception>
        protected void NotifyTerminated(TaskResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            lock (this)
            {
                if (!IsRunning)
                    throw new InvalidOperationException("The task is not currently running.");

                this.result = result;
            }

            Notify(ref terminated);
        }

        private void Notify(ref EventHandler<TaskEventArgs> chain)
        {
            EventHandler<TaskEventArgs> cachedChain;
            lock (this)
            {
                cachedChain = chain;
                chain = Signal;
            }

            EventHandlerUtils.SafeInvoke(cachedChain, this, new TaskEventArgs(this));
        }

        private void AddHandler(ref EventHandler<TaskEventArgs> chain, EventHandler<TaskEventArgs> handler)
        {
            lock (this)
            {
                if (chain != (EventHandler<TaskEventArgs>)Signal)
                {
                    chain = (EventHandler<TaskEventArgs>)Delegate.Combine(chain, handler);
                    return;
                }
            }

            handler(this, new TaskEventArgs(this));
        }

        private void RemoveHandler(ref EventHandler<TaskEventArgs> chain, EventHandler<TaskEventArgs> handler)
        {
            lock (this)
            {
                chain = (EventHandler<TaskEventArgs>)Delegate.Remove(chain, handler);
            }
        }

        private static void Signal(object sender, TaskEventArgs e)
        {
        }
    }
}
