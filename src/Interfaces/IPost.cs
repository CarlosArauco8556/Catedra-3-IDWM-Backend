using Catedra3IDWMBackend.src.DTOs;
using Catedra3IDWMBackend.src.Helpers;
using Catedra3IDWMBackend.src.Models;
using CloudinaryDotNet.Actions;

namespace Catedra3IDWMBackend.src.Interfaces
{
    public interface IPost
    {
        Task<List<GetPostDto>> GetPostsAsync(QueryObject query);
        Task<Post> CreatePostAsync(Post post, ImageUploadResult imageUploadResult);
    }
}