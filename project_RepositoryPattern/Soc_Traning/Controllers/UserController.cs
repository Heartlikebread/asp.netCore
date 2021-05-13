using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soc_Traning.Models;
using Soc_Traning.Models.Service;

namespace Soc_Traning.Controllers
{
    [Route("api/Users")]
    public class UserController : Controller
    {
        private readonly IRepository<UserModel, int> _repository;

        public UserController(IRepository<UserModel,int>repository)
        {
            _repository = repository;
        }

        public IActionResult Index() {

            return View();
        }
        [HttpGet]
        public ResultModel GetAll()
        {
            var result = new ResultModel();
            result.Data = _repository.FindAll();
            result.IsSuccess = true;
            result.Message = "";
            return result;
        }
        [HttpGet("{id}")]
        public ResultModel FindById(int id)
        {
            var result = new ResultModel();

            result.Data = _repository.FindById(id);
            result.IsSuccess = true;
            result.Message = "";
            return result;
        }

        [HttpPost]
        public ResultModel Post([FromBody]UserModel user) {
            var result = new ResultModel();
            _repository.Create(user);
            result.Data = user.Id;
            result.IsSuccess = true;
            result.Message = "";
            return result;

        }
        [HttpDelete]
        public ResultModel Delete(int id) {
            var result = new ResultModel();
            try {
                _repository.Delete(id);
                result.IsSuccess = true;
            }
            catch (Exception ex) {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}