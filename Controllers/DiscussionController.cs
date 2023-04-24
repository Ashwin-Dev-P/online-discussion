using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Online_Discussion.Data;
using Online_Discussion.Models;

using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Online_Discussion.Controllers
{
    public class DiscussionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration Configuration;
        
        public DiscussionController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        // GET: Discussion
        public async Task<IActionResult> Index()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["loggedUserId"] = userId;

            var applicationDbContext = _context.Discussion.Include(d => d.User);
            return View(await applicationDbContext.ToListAsync());
        }


        public List<DiscussionDataModel> getAnswers(int? discussionId)
        {

            List<DiscussionDataModel> answers = new List<DiscussionDataModel>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT * FROM DiscussionData where ( DiscussionId = {discussionId} )";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DiscussionDataModel dd = new DiscussionDataModel();
                            dd.Id = (int)reader["Id"];
                            dd.DiscussionId = (int)reader["DiscussionId"];
                            dd.UserId = (string)reader["UserId"];
                            dd.Answer =(string) reader["Answer"];

                            answers.Add(dd);
                            
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return answers;
        }

        // GET: Discussion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Discussion == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussionModel == null)
            {
                return NotFound();
            }
           
            ViewBag.answers =  getAnswers(id);
            return View(discussionModel);
        }

        public void PostAnswer(string answer, string userId, int discussionId)
        {


            DiscussionDataModel discussionData = new DiscussionDataModel();
            discussionData.Answer = answer;
            discussionData.UserId = userId;
            discussionData.DiscussionId = discussionId;
            

                
               
            using (SqlConnection sqlConnection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                
                using (SqlCommand cmd = sqlConnection.CreateCommand())
                {
                    string query = $"INSERT INTO DiscussionData VALUES({discussionId},'{answer}','{userId}')";
                    Console.WriteLine(query);
                    cmd.CommandText = query;

                    sqlConnection.Open();
                    cmd.ExecuteNonQuery();
                    sqlConnection.Close();
                }
                
            }
                
                

            
            
        }

        // POST: Discussion/Details/5
        [HttpPost]
        public async Task<IActionResult> Details(IFormCollection form)
        {
            Console.WriteLine("Posting details...");
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            PostAnswer(form["answer"], userId,Convert.ToInt32( form["discussionId"]));

            return RedirectToAction(nameof(Index));
        }

        // GET: Discussion/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Discussion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] DiscussionModel discussionModel)
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            discussionModel.UserId = userId;

            try
            {
                _context.Add(discussionModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", discussionModel.UserId);
                return View(discussionModel);
            }
            
            
        }

        // GET: Discussion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Discussion == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", discussionModel.UserId);
            return View(discussionModel);
        }

        // POST: Discussion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] DiscussionModel discussionModel)
        {
            if (id != discussionModel.Id)
            {
                return NotFound();
            }
            try
            {
                string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                discussionModel.UserId = userId;
                try
                {
                    _context.Update(discussionModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionModelExists(discussionModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", discussionModel.UserId);
                return View(discussionModel);
            }
            
            
        }

        // GET: Discussion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Discussion == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            return View(discussionModel);
        }

        // POST: Discussion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Discussion == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Discussion'  is null.");
            }
            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel != null)
            {
                _context.Discussion.Remove(discussionModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionModelExists(int id)
        {
          return (_context.Discussion?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
