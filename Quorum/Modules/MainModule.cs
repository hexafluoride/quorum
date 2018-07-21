using Nancy;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Modules
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
                var board_provider = ProviderStore.GetProvider<IBoardProvider>();

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
                var board_provider = ProviderStore.GetProvider<IBoardProvider>();
                var id = (long)_.id;

                var ctx = new
                {
                    Navbar = Navbar.Build(Context),
                    User = ((User)Context.CurrentUser),
                    Groups = board_provider.GetBoardGroupsUnderBoardGroup(id)/*.Concat(new[] { board_provider.GetBoardGroup(id) })*/,
                    Boards = board_provider.GetBoardsUnderBoardGroup(id),
                    Group = board_provider.GetBoardGroup(id),
                    Crumbs = GetBreadcrumbs(id, BoardParentType.Group)
                };
                return View["group", ctx];
            });

            Get("/boards/{id}", _ => {
                var board_provider = ProviderStore.GetProvider<IBoardProvider>();
                var thread_provider = ProviderStore.GetProvider<IThreadProvider>();

                var id = (long)_.id;

                int page = Request.Query["page"] ?? 0;
                int per_page = 25;

                int offset = page * per_page;
                int count = per_page;

                var ctx = new
                {
                    Navbar = Navbar.Build(Context),
                    User = ((User)Context.CurrentUser),
                    Board = board_provider.GetBoard(id),
                    Boards = board_provider.GetBoardsUnderBoard(id),
                    Threads = thread_provider.GetThreadsByBoardBumpOrdered(id, (long)offset, (long)count),
                    Crumbs = GetBreadcrumbs(id, BoardParentType.Board)
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

        public List<KeyValuePair<string, string>> GetBreadcrumbs(long id, BoardParentType type)
        {
            List<KeyValuePair<string, string>> breadcrumbs = new List<KeyValuePair<string, string>>();

            var board_provider = ProviderStore.GetProvider<IBoardProvider>();

            //Board current_board = board_provider.GetBoard(_["id"]);

            //breadcrumbs.Add(new KeyValuePair<string, string>(current_board.Name, current_board.GetPath()));

            var last_location = id;
            var last_type = type;

            while (true)
            {
                if (last_type == BoardParentType.Board)
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

            return breadcrumbs;
        }
    }
}