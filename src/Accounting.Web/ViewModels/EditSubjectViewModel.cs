using Microsoft.AspNetCore.Mvc;
using System;

namespace Accounting.Web.ViewModels
{
    public class EditSubjectViewModel : CreateSubjectViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public string? ConcurrencyStamp { get; set; }
    }
}
