using Catedra3IDWMBackend.src.Data;
using Catedra3IDWMBackend.src.DTOs;
using Catedra3IDWMBackend.src.Helpers;
using Catedra3IDWMBackend.src.Interfaces;
using Catedra3IDWMBackend.src.Mappers;
using Catedra3IDWMBackend.src.Models;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace Catedra3IDWMBackend.src.Repositories
{
    public class PostRepository : IPost
    {
        private readonly ApplicationDBContext _context;

        public PostRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Post> CreatePostAsync(Post post, ImageUploadResult imageUploadResult)
        {
            if (post == null || imageUploadResult == null)
            {
                throw new ArgumentException("El post o la imagen no pueden ser nulos");
            }

            post.ImageUrl = imageUploadResult.SecureUrl.AbsoluteUri;

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<List<GetPostDto>> GetPostsAsync(QueryObject query)
        {
            var posts = _context.Posts.AsQueryable();

            if(!string.IsNullOrEmpty(query.TextFilter))
            {
                posts = posts.Where(p => p.Title.ToUpper().Contains(query.TextFilter.ToUpper()) || 
                                         p.PublicationDate.ToString().Contains(query.TextFilter));
            }

            if(query.IsDescendingDate)
            {
                posts = posts.OrderByDescending(p => p.PublicationDate);
            }
            else
            {
                posts = posts.OrderBy(p => p.PublicationDate);
            }

            if(!posts.Any())
            {
                throw new ArgumentException("No se encontraron posts");
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await posts.Skip(skipNumber).Take(query.PageSize)
                .Select(p => p.ToGetPostDto()).ToListAsync();
        }
    }
}