using Blog.Models.Domain;
using Blog.Models.ViewModels;
using Blog.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest);
            if (!ModelState.IsValid)
            {
                return View();
            }
            // comming tag is of type view model, map it to domain model before sends to DB
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName,
            };

            await _tagRepository.AddAsync(tag);
            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            var tag = await _tagRepository.GetAllAsync();
            return View(tag);
        }

        [HttpGet]
        [ActionName("Edit")]
        // input parameter has to match with the route parameter => id, form list.cshtml
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);

            if(tag != null) 
            {
                var existingTag = new EditTagRequest
                {
                    Id= tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName,
                };
                return View(existingTag);
            }
            return View(null);
        }

        [HttpPost]
        // this method is used to update the data into DB in a edit action
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName,
            };

            var updatedTag = await _tagRepository.UpdateAsync(tag);

            if(updatedTag != null)
            {
                // show sucess notification
            }
            else
            {
                // show error notifiation
            }

            // if tag was not found we will redirect to the Edit View - it is taking parameter of id, 
            // so with object format we can send the Id to the edit method 

            // show error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });        
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await _tagRepository.GetAsync(editTagRequest.Id);

            if(deletedTag != null)
            {
                // show sucess notification
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }
        
        private void ValidateAddTagRequest(AddTagRequest addTagRequest)
        {
            if(addTagRequest.Name is not null && addTagRequest.DisplayName is not null)
            {
                if(addTagRequest.Name == addTagRequest.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name can not be same as display name");
                }
            }
        }
    }
}
