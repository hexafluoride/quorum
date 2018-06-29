﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface IBoardProvider
    {
        Board GetBoard(long id);
        Board[] GetBoardsUnderBoardGroup(long group_id);
        Board[] GetBoardsUnderBoard(long board_id);
        Board[] GetAllBoards();

        BoardGroup GetBoardGroup(long id);
        BoardGroup[] GetAllBoardGroups();
        BoardGroup[] GetBoardGroupsUnderBoardGroup(long group_id);
    }
}
