using Microsoft.AspNetCore.Mvc;
using System;

namespace Accounting.Web.ViewModels
{
    public class EditSubjectCategoryViewModel : CreateSubjectCategoryViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
    }
}
