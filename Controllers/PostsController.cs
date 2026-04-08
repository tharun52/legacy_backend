using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly BlogContext _context;

        public PostsController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        // GET: api/posts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return post;
        }

        // POST: api/posts
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        // PUT: api/posts/5
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePost(int id, Post post)
        {
            if (id != post.Id) return BadRequest();

            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = post.Title;
            existing.Content = post.Content;
            existing.Author = post.Author;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/posts/5
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
