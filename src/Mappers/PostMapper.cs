using Catedra3IDWMBackend.src.DTOs;
using Catedra3IDWMBackend.src.Models;

namespace Catedra3IDWMBackend.src.Mappers
{
    public static class PostMapper
    {
        public static GetPostDto ToGetPostDto(this Post post)
        {
            return new GetPostDto
            {
                Title = post.Title,
                PublicationDate = post.PublicationDate,
                ImageUrl = post.ImageUrl
            };
        }

        public static Post ToPost(this CreatePostDto createPostDto)
        {
            return new Post
            {
                Title = createPostDto.Title,
                PublicationDate = createPostDto.PublicationDate,
            };
        }
    }
}