﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectMongoDBReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectMongoDBReact.Services
{
    public class RezerwacjaService
    {
        private readonly IMongoCollection<Rezerwacja> _rezerwacja;
        private readonly IMongoCollection<Klient> _klient;
        private readonly IMongoCollection<Pracownik> _pracownik;
        private readonly IMongoCollection<Pokoj> _pokoj;

        public RezerwacjaService(IMongoDBDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _rezerwacja = database.GetCollection<Rezerwacja>(settings.RezerwacjaCollectionName);
            _klient = database.GetCollection<Klient>(settings.KlientCollectionName);
            _pracownik = database.GetCollection<Pracownik>(settings.PracownikCollectionName);
            _pokoj = database.GetCollection<Pokoj>(settings.PokojCollectionName);
        }

        [HttpGet]
        public IEnumerable<Rezerwacja> Get(string name)
        {
            Pracownik pracownik = _pracownik.Find<Pracownik>(f => f.email == name).FirstOrDefault();

            if (pracownik == null) 
            { 
                return null; 
            }
            else { 
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);

            var db = client.GetDatabase("bazaDanych");
            var collection = db.GetCollection<Rezerwacja>("rezerwacje").Find(new BsonDocument()).ToList();

            return collection.Select(s => new Rezerwacja
            {
                Id = s.Id,
                id_pokoju = s.id_pokoju,
                id_klienta = s.id_klienta,
                id_rezerwujacego = s.id_rezerwujacego,
                poczatek = s.poczatek,
                koniec = s.koniec,
                koszt = s.koszt,
                czyEdytowana = s.czyEdytowana,
                dataEdycji = s.dataEdycji,
                czyAnulowana = s.czyAnulowana,
                czyOplacona = s.czyOplacona
            }).ToArray();
            }
        }

        [HttpGet]
        public Rezerwacja GetOne(string id)
        {
            return _rezerwacja.Find<Rezerwacja>(w => w.Id == id).FirstOrDefault();
        }

        public IEnumerable<Rezerwacja> GetRezerwacje(string name)
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);

            var db = client.GetDatabase("bazaDanych");
            var collection = db.GetCollection<Rezerwacja>("rezerwacje").Find(new BsonDocument()).ToList();

            Klient klient = KlientCheck(name);

            return collection.Where(w => w.id_klienta == klient.Id).Select(s => new Rezerwacja
            {
                Id = s.Id,
                id_pokoju = s.id_pokoju,
                id_klienta = s.id_klienta,
                id_rezerwujacego = s.id_rezerwujacego,
                poczatek = s.poczatek,
                koniec = s.koniec,
                koszt = s.koszt,
                czyEdytowana = s.czyEdytowana,
                dataEdycji = s.dataEdycji,
                czyAnulowana = s.czyAnulowana,
                czyOplacona = s.czyOplacona
            }).ToArray();
        }

        public IEnumerable<Rezerwacja> GetRezerwacjeDetails(string name, string email)
        {
            var pr = _pracownik.Find<Pracownik>(f => f.email == name).FirstOrDefault();

            if (pr != null)
            {
                var connectionString = "mongodb://localhost";
                var client = new MongoClient(connectionString);

                var db = client.GetDatabase("bazaDanych");
                var collection = db.GetCollection<Rezerwacja>("rezerwacje").Find(new BsonDocument()).ToList();

                Klient klient = KlientCheck(email);

                return collection.Where(w => w.id_klienta == klient.Id).Select(s => new Rezerwacja
                {
                    Id = s.Id,
                    id_pokoju = s.id_pokoju,
                    id_klienta = s.id_klienta,
                    id_rezerwujacego = s.id_rezerwujacego,
                    poczatek = s.poczatek,
                    koniec = s.koniec,
                    koszt = s.koszt,
                    czyEdytowana = s.czyEdytowana,
                    dataEdycji = s.dataEdycji,
                    czyAnulowana = s.czyAnulowana,
                    czyOplacona = s.czyOplacona
                }).ToArray();
            }
            return null;
        }


        public Rezerwacja Create(Rezerwacja r)
        {
            var klient = KlientCheck(r.id_rezerwujacego);

            var pokoj = _pokoj.Find<Pokoj>(f => f.Id == r.id_pokoju).FirstOrDefault();

            if (pokoj == null) 
            { return null; }

            bool zajety = czyZajety(pokoj, r.poczatek, r.koniec);

            if (zajety == false)
            {
                String diff = (r.koniec - r.poczatek).TotalDays.ToString();

                int ileDni = Int32.Parse(diff);

                if (ileDni < 0)
                { return null; }

                Rezerwacja nowa = new Rezerwacja { id_klienta = klient.Id, id_rezerwujacego = klient.Id, id_pokoju = r.id_pokoju, poczatek = r.poczatek, koniec = r.koniec, koszt = r.koszt * ileDni, dataEdycji = r.dataEdycji, czyAnulowana = r.czyAnulowana, czyEdytowana = r.czyEdytowana, czyOplacona = false };
                _rezerwacja.InsertOne(nowa);
                return nowa;
            }
            else 
                return null;
        }

        public bool czyZajety(Pokoj p, DateTime begin, DateTime end) {

            var result0 = _rezerwacja.Find<Rezerwacja>(f => f.id_pokoju == p.Id &&
            f.czyAnulowana == false &&
            ((begin >= f.poczatek && begin <= f.koniec) ||
            (end >= f.poczatek && end <= f.koniec))).Any();

            var result1 = _rezerwacja.Find<Rezerwacja>(f => f.id_pokoju == p.Id &&
            f.czyAnulowana == false &&
            begin < f.poczatek &&
            end >= f.koniec).Any();

            if (result0 == false && result1 == false) { return false; }
            else 
                return true;
        }

        public Klient KlientCheck(string email) {
            var klient = _klient.Find<Klient>(f => f.email == email).FirstOrDefault();
            var pracownik = _pracownik.Find<Pracownik>(f => f.email == email).FirstOrDefault();

            if (klient == null && pracownik == null)
            {
                _klient.InsertOne(new Klient { email = email, imie = "Joe/Jane", nazwisko = "Doe", nr_tel = 000000000 });
                klient = _klient.Find<Klient>(f => f.email == email).FirstOrDefault();
                return klient;
            }
            else if (klient != null) {
                return klient;
            }
            
            return new Klient { Id = pracownik.Id, imie = pracownik.imie, nazwisko = pracownik.nazwisko, email = pracownik.email, nr_tel = pracownik.nr_tel };
        }

        public Rezerwacja Cancel(string id) {
            Rezerwacja cancel = _rezerwacja.Find<Rezerwacja>(p => p.Id == id).FirstOrDefault();
            cancel.czyAnulowana = true;
            _rezerwacja.ReplaceOne(p => p.Id == id, cancel);
            return cancel;
        }

        public Rezerwacja Edit(string id, Rezerwacja rezerwacja)
        {
            Rezerwacja edit = _rezerwacja.Find<Rezerwacja>(p => p.Id == id).FirstOrDefault();

            var pokoj = _pokoj.Find<Pokoj>(f => f.Id == edit.id_pokoju).FirstOrDefault();

            if (pokoj == null)
            { return null; }

            bool zajety = czyZajety(pokoj, rezerwacja.poczatek, rezerwacja.koniec);


            if (zajety == false)
            {
                String diff = (rezerwacja.koniec - rezerwacja.poczatek).TotalDays.ToString();
                int ileDni = Int32.Parse(diff);

                if (ileDni < 0) { return null; }

                edit.poczatek = rezerwacja.poczatek;
                edit.koniec = rezerwacja.koniec;
                edit.dataEdycji = rezerwacja.dataEdycji;
                
                edit.koszt = pokoj.cena * ileDni;
                edit.czyEdytowana = true;
                _rezerwacja.ReplaceOne(p => p.Id == id, edit);
                return edit;
            }
            else 
                return null;
        }

    }
}
