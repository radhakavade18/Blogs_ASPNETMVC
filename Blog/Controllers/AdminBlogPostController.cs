using Blog.Models.Domain;
using Blog.Models.ViewModels;
using Blog.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ITagRepository _tagRepository;

        public AdminBlogPostController(IBlogPostRepository blogPostRepository, ITagRepository tagRepository)
        {
            _blogPostRepository = blogPostRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await _tagRepository.GetAllAsync();
            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name,Value = x.Id.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            // map view mode to domain model
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible
            };

            var selectedTags = new List<Tag>();
            // map tags from selectedTags 
            // we are getting ids of tags so we need to get the data from database using selectedTags Id
            foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selecteTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await _tagRepository.GetAsync(selecteTagIdAsGuid);

                if(existingTag != null)
                {
                   selectedTags.Add(existingTag);
                }

            }
            // mapping tags to domain model
            blogPost.Tags = selectedTags;

            await _blogPostRepository.AddAsync(blogPost);
            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            // call the repository
            var blogPost = await _blogPostRepository.GetAllAsync();
            return View(blogPost);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // retrive the result from repository
            var existingBlogPost = await _blogPostRepository.GetAsync(id);

            // get all tags
            var tagsDomainModel = await _tagRepository.GetAllAsync();

            if (existingBlogPost != null)
            {
                // map domain model into view model
                var model = new EditBlogPostRequest
                {
                    Id = existingBlogPost.Id,
                    Heading = existingBlogPost.Heading,
                    PageTitle = existingBlogPost.PageTitle,
                    PublishedDate = existingBlogPost.PublishedDate,
                    Content = existingBlogPost.Content,
                    Author = existingBlogPost.Author,
                    ShortDescription = existingBlogPost.ShortDescription,
                    FeaturedImageUrl = existingBlogPost.FeaturedImageUrl,
                    UrlHandle = existingBlogPost.UrlHandle,
                    Visible = existingBlogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                    }),
                    SelectedTags = existingBlogPost.Tags.Select(x => x.Id.ToString()).ToArray(),
                };
                return View(model);
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            // map blog post to doamin model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                Content = editBlogPostRequest.Content,
                PageTitle = editBlogPostRequest.PageTitle,
                Author = editBlogPostRequest.Author,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                PublishedDate = editBlogPostRequest.PublishedDate,
                UrlHandle = editBlogPostRequest.UrlHandle,
                Visible = editBlogPostRequest.Visible,
            };

            // map tags to domain model
            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if(Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await _tagRepository.GetAsync(tag);
                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            // submit edited information to the repository
            var updatedBlog = await _blogPostRepository.UpdateAsync(blogPostDomainModel);

            if(updatedBlog != null)
            {
                // show success notification
                return RedirectToAction("List");
            }
                // show error notification
                return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            var deleteBlogPost = await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deleteBlogPost != null)
            {
                // Show success notification
                return RedirectToAction("List");
            }
            else
            {
                // show error notification
                return RedirectToAction("Edit");
            }
        }
    }
}
