﻿using ASPNET_WebAPI_Testing.Models;
using ASPNET_WebAPI_Testing.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_WebAPI_Testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository, IConfiguration configuration)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _bookRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBooks(int id)
        {
            return await _bookRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBooks([FromBody] Book book)
        {
            var newBook = await _bookRepository.Create(book);
            return CreatedAtAction(nameof(GetBooks), new { id = newBook.Id }, newBook);
        }

        [HttpPut]
        public async Task<ActionResult> PutBooks(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            await _bookRepository.Update(book);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete (int id)
        {
            var bookToDelete = await _bookRepository.Get(id);
            if (bookToDelete == null)
            {
                return NotFound();
            }
                
            await _bookRepository.Delete(bookToDelete.Id);
            return NoContent();
        }
    }
}
