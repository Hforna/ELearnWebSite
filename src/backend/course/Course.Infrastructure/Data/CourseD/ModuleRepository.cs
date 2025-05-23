﻿using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.Course
{
    public class ModuleRepository : IModuleReadOnly, IModuleWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public ModuleRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public void AddModule(Module module)
        {
            _dbContext.Modules.Add(module);
        }

        public void DeactiveModule(Module module)
        {
            module.Active = false;
            _dbContext.Modules.Update(module);
        }

        public void DeleteModule(Module module)
        {
            _dbContext.Modules.Remove(module);
        }

        public async Task<List<Module>?> GetAllCourseModules(long courseId)
        {
            return await _dbContext.Modules.Where(d => d.CourseId == courseId).ToListAsync();
        }

        public async Task<IList<Module>> GetNotActiveModules()
        {
            return await _dbContext.Modules.Include(d => d.Course).Include(d => d.Lessons).Where(d => d.Active == false).ToListAsync();
        }

        public async Task<Module?> ModuleByCourseAndModuleIds(long courseId, long moduleId)
        {
            return await _dbContext.Modules.SingleOrDefaultAsync(d => d.CourseId == courseId && d.Id == moduleId && d.Active);
        }

        public async Task<List<Module>?> ModuleByCourseId(long courseId)
        {
            return await _dbContext.Modules.Include(d => d.Course).Where(m => m.CourseId == courseId && m.Active).ToListAsync();
        }

        public async Task<Module?> ModuleById(long id)
        {
            return await _dbContext.Modules.Include(d => d.Course).SingleOrDefaultAsync(d => d.Id == id && d.Active);
        }

        public async Task<List<Module>?> ModulesByCourseId(long courseId)
        {
            return await _dbContext.Modules.Where(d => d.Active && d.CourseId == courseId).ToListAsync();
        }

        public void UpdateModule(Module module)
        {
            _dbContext.Modules.Update(module);
        }

        public void UpdateModules(List<Module> modules)
        {
            _dbContext.Modules.UpdateRange(modules);
        }
    }
}
