using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Costants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService;
        public ProductManager(IProductDal productDal,ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }
        [SecuredOperation("product.add,admin")]
        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
        [TransactionScopeAspect]
        public IResult Add(Product product)
        {
          IResult result= BusinessRules.Run(CheckIfProductNameExists(product.ProductName)
                ,CheckIfProductCountOfCategoryCorrect(product.CategoryId)
                ,CheckCategoryLimit());
            if (result!=null)
            {
                return result;
            }
            _productDal.Add(product);
            return new SuccessResult();
        }

        [CacheAspect]
        public IDataResult<List<Product>> GetAll()
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(),"ürünler getirildi.");
        }

        [CacheAspect]
        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.CategoryId==id));
        }
        [CacheAspect]
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }
        [CacheAspect]
        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.UnitPrice<=max && p.UnitPrice>min));
        }

        [SecuredOperation("product.add,admin")]
        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
        [TransactionScopeAspect]
        public IResult Update(Product product)
        {
            return new SuccessResult();
        }
        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId) //bir kategoride en fazla 10 adet ürün olabilir.
        {
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }
        private IResult CheckIfProductNameExists(string productName)  //Aynı isimde olanlar eklenemez
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any(); //Any() var mı demek
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }
        private IResult CheckCategoryLimit() // mevcut kategori sayısı 15 i geçtiyse yeni ürün eklenemez.
        {
            var result = _categoryService.GetAll().Data.Count;
            if (result>=15)
            {
                return new ErrorResult(Messages.CategoryLimitExceded);
            }
            return new SuccessResult();
        }
    }
}
