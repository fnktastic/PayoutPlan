﻿using PayoutPlan.Enum;
using PayoutPlan.Extensions;
using PayoutPlan.Factories;
using PayoutPlan.Interfaces;
using PayoutPlan.Interfaces.Common;
using System;
using System.Collections.Generic;

namespace PayoutPlan.Model
{
    public abstract class ProductBase : IProduct
    {
        private readonly IDateTimeNow _dateTimeNow;
        public ProductBase(IDateTimeNow dateTimeNow)
        {
            Created = DateTime.UtcNow;
            _dateTimeNow = dateTimeNow;
        }

        public ProductType ProductType { get; set; }
        public double Investment { get; set; }
        public bool FinalDerisking { get; set; }
        public bool AnnualDerisking { get; set; }
        public double Balance { get; set; }
        public int InvestmentLength { get; set; }
        public DateTime Created { get; set; }
        public IModelPortfolio ModelPortfolio { get; set; }
        public int InvestmentYear => _dateTimeNow.Now.Year - Created.Year;
        public bool LastTwoYearsPeriod => (InvestmentLength - InvestmentYear) <= 2;
        public IDateTimeNow DateTimeNow => _dateTimeNow;
        public IBehaviour Withdrawal { get; set; }
    }
}
