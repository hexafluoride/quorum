using Nancy;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            AuthenticationManager.EnableAuthentication(this);
            
            AddGetHandler("/", "index");
            AddGetHandler("/login", "login");
            AddGetHandler("/register", "register");

            Get("/boards", _ => {
                var board_provider = AuthenticationManager.GetProvider<IBoardProvider>();

                var ctx = new
                {
                    Navbar = Navbar.Build(Context),
                    User = ((User)Context.CurrentUser),
                    Groups = board_provider.GetAllBoardGroups(),
                    Boards = board_provider.GetAllBoards()
                };
                return View["boards", ctx];
            });

            Get("/group/{id}", _ => {
                var board_provider = AuthenticationManager.GetProvider<IBoardProvider>();
                var id = (long)_["id"];

                var ctx = new
                {
                    Navbar = Navbar.Build(Context),
                    User = ((User)Context.CurrentUser),
                    Groups = board_provider.GetBoardGroupsUnderBoardGroup(id).Concat(new[] { board_provider.GetBoardGroup(id) }),
                    Boards = board_provider.GetBoardsUnderBoardGroup(id)
                };
                return View["boards", ctx];
            });

            Get("/boards/{id}", _ => {
                var board_provider = AuthenticationManager.GetProvider<IBoardProvider>();
                var thread_provider = AuthenticationManager.GetProvider<IThreadProvider>();

                int page = Request.Query["page"] ?? 0;
                int per_page = 25;

                int offset = page * per_page;
                int count = per_page;

                List<KeyValuePair<string, string>> breadcrumbs = new List<KeyValuePair<string, string>>();

                Board current_board = board_provider.GetBoard(_["id"]);

                breadcrumbs.Add(new KeyValuePair<string, string>(current_board.Name, current_board.GetPath()));
                var last_location = current_board.ParentId;
                var last_type = current_board.ParentType;

                while(true)
                {
                    if(last_type == BoardParentType.Board)
                    {
                        var parent = board_provider.GetBoard(last_location);
                        breadcrumbs.Insert(0, new KeyValuePair<string, string>(parent.Name, parent.GetPath()));

                        last_location = parent.ParentId;
                        last_type = parent.ParentType;
                    }
                    else
                    {
                        var parent = board_provider.GetBoardGroup(last_location);
                        breadcrumbs.Insert(0, new KeyValuePair<string, string>(parent.Name, "/group/" + parent.Id));

                        last_location = parent.ParentId;
                        last_type = BoardParentType.Group;

                        if (last_location == -1)
                            break;
                    }
                }

                var ctx = new
                {
                    Navbar = Navbar.Build(Context),
                    User = ((User)Context.CurrentUser),
                    Board = board_provider.GetBoard(_.id),
                    Boards = board_provider.GetBoardsUnderBoard((long)_.id),
                    Threads = thread_provider.GetThreadsByBoardBumpOrdered((long)_.id, (long)offset, (long)count),
                    Crumbs = breadcrumbs
                    //Groups = AuthenticationManager.GetProvider<IBoardProvider>().GetAllBoardGroups(),
                    //Boards = AuthenticationManager.GetProvider<IBoardProvider>().GetAllBoards()

                };
                return View["board", ctx];
            });
        }

        public void AddGetHandler(string path, string view)
        {
            Get(path, _ => 
            {
                return View[view, new { Navbar = Navbar.Build(Context), User = ((User)Context.CurrentUser) }];
            });
        }
    }
}