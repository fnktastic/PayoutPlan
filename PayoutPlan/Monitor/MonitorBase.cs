﻿using PayoutPlan.Interfaces;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Monitor
{
    public interface IMonitor
    {
        void Invoke();
    }

    public abstract class MonitorBase : IMonitor
    {
        protected readonly IProduct _productBase;

        protected readonly IDateTimeNow _dateTime;
        protected IProduct ProductBase => _productBase;

        public MonitorBase(IProduct productBase, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _dateTime = dateTime;
        }
        public abstract void Invoke();
    }




}
