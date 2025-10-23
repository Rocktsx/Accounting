using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Web.ViewModels
{
    public class EditAccountingPeriodViewModel: CreateAccountingPeriodViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public string? ConcurrencyStamp { get; set; }
    }
}
