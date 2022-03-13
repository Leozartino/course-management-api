﻿using GerenciadorDeCursos.Dtos;
using GerenciadorDeCursos.Enums;
using GerenciadorDeCursos.Interfaces;
using GerenciadorDeCursos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GerenciadorDeCursos.Controllers
{   
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ICourseRepository courseReposiotry)
        {
            _courseRepository = courseReposiotry;

        }

        //Qualquer pessoa
        [AllowAnonymous]
        [HttpGet]
        [Route("index")]
        public async Task<ActionResult<IEnumerable<Course>>> IndexCourses()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return Ok(courses);
        }

        //Qualquer pessoa
        [AllowAnonymous]
        [HttpGet]
        [Route("show/search/{status}")]
        public ActionResult<Course> ShowCourseByStatus(StatusCourseEnum status)
        {
            var courses = _courseRepository.GetCourseByStatus(status);

            if (courses == null)
            {
                return NotFound(new { message = "Nenhum curso foi pode ser encontrado!" });
            }

            return Ok(courses);

        }

        //Qualquer pessoa
        [AllowAnonymous]
        [HttpGet]
        [Route("show/{id}")]
        public async Task<ActionResult<Course>> ShowCourse(Guid id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);

            if(course == null)
            {
                return NotFound(new { message = "curso não pode ser encontrado!" });
            }

            return Ok(course);

        }

        //Gerente e Secretario
        [HttpPost]
        [Route("post")]
        [Authorize(Roles = "Gerente, Secretario")]
        public async Task<ActionResult<Course>> CreateCourse(CourseCreateDTO courseDTO)
        {

            Guid id = Guid.NewGuid();

            Course course = new()
            {
                Id = id,
                Title = courseDTO.Title,
                StartDate = courseDTO.StartDate,
                EndDate = courseDTO.EndDate,
                Status = courseDTO.Status,
                
            };

            var course_created = await _courseRepository.CreateCourseAsync(course);
            // var days = (courseDTO.EndDate - courseDTO.StartDate).Days;

            return Created("~api/courses/post", course_created);
        }


        //Gerente e Secretario
        [HttpPut]
        [Route("put/status/{id}")]
        [Authorize(Roles = "Gerente, Secretario")]
        public async Task<ActionResult<Course>> UpdateStatusCourse(Guid id, StatusCourseEnum statusCourse)
        {

            var course = await _courseRepository.GetCourseByIdAsync(id);

            
            if(course == null)
            {
                return NotFound(new { message = "Course não encontrado!" });
            }

            var course_updated = await _courseRepository.UpdateCourseStatusAsync(course, statusCourse);


            return Ok(course_updated);
        }

        //Gerente
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<Course>> DeleteCourse(Guid id)
        {

            var course = await _courseRepository.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound(new { message = "Curso não encontrado!" });
            }

            bool result = await _courseRepository.DeleteCourseAsync(course);

            return Ok(new { message = "Curso removido com sucesso!", result });
        }

        //TODO uodate Course



    }
}
