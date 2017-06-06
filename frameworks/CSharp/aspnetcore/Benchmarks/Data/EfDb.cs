﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Benchmarks.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Benchmarks.Data
{
    public class EfDb : IDb
    {
        private readonly IRandom _random;
        private readonly ApplicationDbContext _dbContext;
        private readonly bool _useBatchUpdate;

        public EfDb(IRandom random, ApplicationDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _random = random;
            _dbContext = dbContext;
            _useBatchUpdate = appSettings.Value.Database != DatabaseServer.PostgreSql;
        }

        public Task<World> LoadSingleQueryRow()
        {
            var id = _random.Next(1, 10001);
            return _dbContext.World.FirstAsync(w => w.Id == id);
        }

        public async Task<World[]> LoadMultipleQueriesRows(int count)
        {
            var result = new World[count];

            for (var i = 0; i < count; i++)
            {
                var id = _random.Next(1, 10001);
                result[i] = await _dbContext.World.FirstAsync(w => w.Id == id);
            }

            return result;
        }

        public async Task<World[]> LoadMultipleUpdatesRows(int count)
        {
            var results = new World[count];

            for (var i = 0; i < count; i++)
            {
                var id = _random.Next(1, 10001);
                var result = await _dbContext.World.AsTracking().FirstAsync(w => w.Id == id);

                result.RandomNumber = _random.Next(1, 10001);
                results[i] = result;
                if (!_useBatchUpdate)
                {
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (_useBatchUpdate)
            {
                await _dbContext.SaveChangesAsync();
            }

            return results;
        }

        public async Task<IEnumerable<Fortune>> LoadFortunesRows()
        {
            var result = await _dbContext.Fortune.ToListAsync();

            result.Add(new Fortune { Message = "Additional fortune added at request time." });
            result.Sort();

            return result;
        }
    }
}