﻿using Switchr.Models;
using Switchr.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Switchr.Controllers
{
    public class GamesController : Controller
    {
        private ApplicationDbContext _context;

        public GamesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Games
        public ActionResult Index()
        {
            //var games = _context.Games.Include(g => g.GameType).ToList();

            //return View(games);

            if (User.IsInRole(RoleName.CanManageGames))
                return View("List");
            else
                return View("ReadOnlyList");
        }

        public ActionResult Details(int id)
        {
            var game = _context.Games.Include(g => g.GameType).SingleOrDefault(g => g.Id == id);

            if (game == null)
                return HttpNotFound();

            return View(game);
        }

        // GET: Games/New
        [Authorize(Roles = RoleName.CanManageGames)]
        public ActionResult New()
        {
            var gameTypes = _context.GameTypes.ToList();
            var viewModel = new GameFormViewModel
            {
                Game = new Game(),
                GameTypes = gameTypes
            };

            return View("GameForm", viewModel);
        }

        // GET: Games/Edit/#
        [Authorize(Roles = RoleName.CanManageGames)]
        public ActionResult Edit(int id)
        {
            var game = _context.Games.SingleOrDefault(g => g.Id == id);

            if (game == null)
                return HttpNotFound();

            var viewModel = new GameFormViewModel
            {
                Game = game,
                GameTypes = _context.GameTypes.ToList()
            };

            return View("GameForm", viewModel);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.CanManageGames)]
        public ActionResult Save(Game game)
        {
            if(!ModelState.IsValid)
            {
                var viewModel = new GameFormViewModel
                {
                    Game = game,
                    GameTypes = _context.GameTypes.ToList()
                };

                return View("GameForm", viewModel);
            }

            if (game.Id == 0)
                _context.Games.Add(game);
            else
            {
                var gameInDb = _context.Games.Single(c => c.Id == game.Id);

                gameInDb.Name = game.Name;
                gameInDb.ReleaseDate = game.ReleaseDate;
                gameInDb.GameTypeId = game.GameTypeId;
                gameInDb.NumberInStock = game.NumberInStock;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Games");
        }

        /*
        // demo code

        // GET: Games/Random
        public ActionResult Random()
        {
            var game = new Game() { Name = "Super Smash Bros. Ultimate" };
            var customers = new List<Customer>
            {
                new Customer { Name = "Customer 1", Id = 1 },
                new Customer { Name = "Customer 2", Id = 2 }
            };

            var viewModel = new RandomGameViewModel
            {
                Game = game,
                Customers = customers
            };

            return View(viewModel);
        }
        */
    }
}