using Catedra3IDWMBackend.src.Models;
using CloudinaryDotNet.Actions;

namespace Catedra3IDWMBackend.src.Interfaces
{
    public interface IPost
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> CreatePostAsync(Post post, ImageUploadResult imageUploadResult);
    }
}