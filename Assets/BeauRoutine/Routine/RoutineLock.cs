/*
 * Copyright (C) 2016-2018. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    30 Apr 2018
 * 
 * File:    RoutineLock.cs
 * Purpose: Handle that represents a lock on a Fiber.
            Disposing or releasing will unlock on the Fiber.
*/

using System;
using System.Collections;
using BeauRoutine.Internal;

namespace BeauRoutine
{
    /// <summary>
    /// Represents a lock on a Routine.
    /// Locked Routines will suspend execution
    /// until all locks are removed.
    /// </summary>
    public struct RoutineLock : IDisposable
    {
        private Routine m_Locked;

        public RoutineLock(Routine inRoutine)
        {
            m_Locked = inRoutine;
        }

        /// <summary>
        /// Releases the lock on the Routine.
        /// </summary>
        public void Release()
        {
            if (m_Locked == Routine.Null)
                return;

            Manager m = Manager.Get();
            if (m != null)
            {
                Fiber f = m.Fibers[m_Locked];
                if (f != null)
                    f.ReleaseLock();
            }

            m_Locked = Routine.Null;
        }

        /// <summary>
        /// Identical to Release.
        /// </summary>
        public void Dispose()
        {
            Release();
        }

        static public implicit operator bool(RoutineLock inLock)
        {
            return inLock.m_Locked;
        }

        static public implicit operator Routine(RoutineLock inLock)
        {
            return inLock.m_Locked;
        }

        public override string ToString()
        {
            return "RoutineLock";
        }
    }
}
