using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/posts/{postId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly BlogContext _context;

        public CommentsController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/posts/5/comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        // POST: api/posts/5/comments
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(int postId, Comment comment)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound("Post not found");

            comment.PostId = postId;
            comment.CreatedAt = DateTime.UtcNow;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetComments), new { postId }, comment);
        }

        // DELETE: api/posts/5/comments/3
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int postId, int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.PostId == postId);
            if (comment == null) return NotFound();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
