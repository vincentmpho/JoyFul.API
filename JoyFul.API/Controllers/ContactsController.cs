using JoyFul.API.Data;
using JoyFul.API.Models;
using JoyFul.API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]

        public IActionResult GetContacts()
        {
            //read contacts from databse
            var contacts =  _context.Contacts.ToList();
            return Ok(contacts);
        }

        [HttpGet("{id}")]

        public IActionResult GetContact(int id)
        {
            //read contacts from database

            var contact = _context.Contacts.FirstOrDefault(x => x.Id == id);

            if (contact == null)
            {
                return BadRequest();
            }
            return Ok(contact);
        }

        [HttpPost]

        public IActionResult CreateContact(ContactDto contactDto)
        {
            //Convert  contactDto into a Contact object that we can add to the database

            Contact contact = new Contact()
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                PhoneNumber = contactDto.PhoneNumber ?? "",
                Subject = contactDto.Subject,
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
            contact.Subject = contactDto.Subject;
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
