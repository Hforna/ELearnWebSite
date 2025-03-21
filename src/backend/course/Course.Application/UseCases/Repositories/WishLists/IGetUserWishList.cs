﻿using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.WishLists
{
    public interface IGetUserWishList
    {
        public Task<WishListResponse> Execute(string sessionId);
    }
}
