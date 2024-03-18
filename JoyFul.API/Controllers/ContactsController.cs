using JoyFul.API.Data;
using JoyFul.API.Models;
using JoyFul.API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JoyFul.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

      
        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("subjects")]

        public IActionResult GetSubjects()
        {
            var  listSubjects = _context.Subjects.ToList();
            return Ok(listSubjects);
        }

        [HttpGet]

        public IActionResult GetContacts(int? page)
        {

            //Add Pagination
            if (page == null || page<1)
            {
                page = 1;
            }
            int pageSize = 5;
            int totalPages = 0;

            decimal count = _context.Contacts.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            var contacts = _context.Contacts
                .Include(c=>c.Subject)
                .OrderByDescending(c=>c.Id)
                .Skip((int)(page-1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Contacts = contacts,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);



            //read contacts from databse 
            //We have to Include the navigation property we want to use,
            //so in the parenthesis, we have to provide an arrow function, so for every contact,
            //let's call it c and we want to include contact. subject
            //var contacts =  _context.Contacts.Include(c=> c.Subject).ToList();
            //return Ok(contacts);
        }

        [HttpGet("{id}")]

        public IActionResult GetContact(int id)
        {
            //read contacts from database

            var contact = _context.Contacts.Include(c=>c.Subject).FirstOrDefault(x => x.Id == id);

            if (contact == null)
            {
                return BadRequest();
            }
            return Ok(contact);
        }

        [HttpPost]

        public IActionResult CreateContact(ContactDto contactDto)
        {
            var subject = _context.Subjects.Find(contactDto.SubjectId);

            //Check that the subjects that we are received belongs to the authorized list of subjects
            if (subject == null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }

            //Convert  contactDto into a Contact object that we can add to the database
            Contact contact = new Contact()
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                PhoneNumber = contactDto.PhoneNumber ?? "",
                Subject = subject,
                Message = contactDto.Message,
                CreatedAt = DateTime.Now,
            };

            //Add this contact to the Database
            _context.Contacts.Add(contact);

            //Save modification
            _context.SaveChanges();

            //return to the client
            return Ok(contact);
        }

        [HttpPut("{id}")]

        public IActionResult UpdateContact(int id, ContactDto contactDto)
        {
            var subject = _context.Subjects.Find(contactDto.SubjectId);

            // Check that the subjects that we are received belongs to the authorized list of subjects
            if (subject == null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }


            //check  if the ID is valid
            var contact = _context.Contacts.FirstOrDefault(x=> x.Id==id);
            if (contact == null)
            {
                return NotFound();
            }
            //UPDATE the data of this contact using the data that we have inside this contactDto
            contact.FirstName = contactDto.FirstName;
            contact.LastName = contactDto.LastName;
            contact.Email = contactDto.Email;
            contact.PhoneNumber = contactDto.PhoneNumber ?? "";
            contact.Subject = subject;
            contact.Message = contactDto.Message;
            contact.CreatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            //read the contact that we want to delete on the database/ Find the contact that we want to delete 
            var contact = _context.Contacts.FirstOrDefault(_x => _x.Id==id);

            //LET'S check if the contact is null or not 
            if (contact == null)
            {
                return NotFound();
            }
            //Otherwise we need to delete this contact
            _context.Contacts.Remove(contact);
            _context.SaveChanges();

            return Ok(contact);
        }
    }
}
