﻿using Course.Domain.Entitites;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IVideoWriteOnly
    {
        public Task DeleteVideo(string id);
        public Task AddVideo(Video video);
        public Task DeleteRangeVideos(List<string> ids);
    }
}
