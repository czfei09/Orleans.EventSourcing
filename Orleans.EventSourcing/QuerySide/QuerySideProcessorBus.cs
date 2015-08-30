﻿using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Orleans.EventSourcing.QuerySide
{
    public class QuerySideProcessorBus
    {
        private static QuerySideProcessorContainer _handlerContainer;

        public static void Initialize(QuerySideProcessorContainer handlerContainer)
        {
            _handlerContainer = handlerContainer;
        }

        public static WriteResult Send<TEvent>(TEvent @event) where TEvent : GrainEvent
        { 
            try
            {
                var executor = _handlerContainer.FindHandler(typeof(TEvent));

                if (executor == null)
                    throw new Exception("Faile to find " + typeof(TEvent).Name + "'s executor.");

                var writeResult = executor.Item1.Invoke(executor.Item2, @event);
            
                return writeResult;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
