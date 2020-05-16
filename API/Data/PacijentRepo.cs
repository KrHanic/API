﻿using API.EFModels;
using API.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class PacijentRepo : IPacijentRepo
    {
        private readonly KV_TESTContext _context;
        private readonly Mapper _mapper;

        public PacijentRepo(KV_TESTContext ctx, Mapper mapper)
        {
            _context = ctx;
            _mapper = mapper;
        }

        public void CreatePacijent(Pacijent pacijent)
        {
            if (pacijent == null)
            {
                throw new ArgumentNullException(nameof(pacijent));
            }

            _context._02Pacijent.Add(_mapper.Map<_02Pacijent>(pacijent));
        }

        public void DeletePacijent(Pacijent pacijent)
        {
            if (pacijent == null)
            {
                throw new ArgumentNullException(nameof(pacijent));
            }

            _context._02Pacijent.Remove(_mapper.Map<_02Pacijent>(pacijent));
        }

        public IEnumerable<Pacijent> GetAllPacijenti()
        {
            return _mapper.Map<List<Pacijent>>(_context._02Pacijent.ToList());
        }

        public Pacijent GetPacijentByOIB(long OIB)
        {
            return _mapper.Map<Pacijent>(_context._02Pacijent.Where(x => x.Oib == OIB).FirstOrDefault());
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdatePacijent(Pacijent pacijent)
        {
            //nothing
        }
    }
}