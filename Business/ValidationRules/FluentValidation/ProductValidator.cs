using Entity.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class ProductValidator :AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).NotEmpty().WithMessage("Ürün Adı boş olamaz.");
            RuleFor(p=>p.ProductName).MinimumLength(2);
            RuleFor(p => p.UnitPrice).NotEmpty();
            RuleFor(p => p.UnitPrice).GreaterThan(0); // 0 dan büyük olacak
         //   RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(10).When(p => p.CategoryId == 1);//kategori id si 1 olanın
                                                                                             //değeri minimum 10 olabilir                                                                               
            // RuleFor(p=>p.ProductName).Must(StartsWithA); prouctName A ile başlamalıdır.
            // WithMessage(); ile istenilen mesaj verilebilir.
       
        }
        private bool StartsWithA(string word)
        {
            return word.StartsWith("A");
        }
    }
}
