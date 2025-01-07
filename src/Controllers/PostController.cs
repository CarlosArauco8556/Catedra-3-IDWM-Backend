using Catedra3IDWMBackend.src.DTOs;
using Catedra3IDWMBackend.src.Helpers;
using Catedra3IDWMBackend.src.Interfaces;
using Catedra3IDWMBackend.src.Mappers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Catedra3IDWMBackend.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPost _postRepository;
        private readonly Cloudinary _cloudinary;

        public PostController(IPost postRepository, Cloudinary cloudinary)
        {
            _postRepository = postRepository;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try{
                var posts = await _postRepository.GetPostsAsync(query);
                return Ok(posts);
            }
            catch(Exception ex){
                return StatusCode(500, new { message = ex.Message});
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPostDto){
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try{
                if (createPostDto.Image == null || createPostDto.Image.Length == 0)
                {
                    return BadRequest( new { message = "La imagen es requerida"});
                }

                if (createPostDto.Image.ContentType != "image/jpeg" && createPostDto.Image.ContentType != "image/png")
                {
                    return BadRequest( new { message = "La imagen debe ser un archivo jpeg o png"});
                }

                if (createPostDto.Image.Length > 5 * 1024 * 1024)
                {
                    return BadRequest( new { message = "La imagen debe tener menos de 5 MB"});
                }

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(createPostDto.Image.FileName, createPostDto.Image.OpenReadStream()),
                    Folder = "Catedra3IDWM-Posts"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return BadRequest( new { message = uploadResult.Error.Message});
                }

                var post = createPostDto.ToPost();
                var postAdded = await _postRepository.CreatePostAsync(post, uploadResult);
                return Ok(postAdded.ToGetPostDto());
            }
            catch(Exception ex){
                return StatusCode(500, new { message = ex.Message});
            }
        }
    }
}