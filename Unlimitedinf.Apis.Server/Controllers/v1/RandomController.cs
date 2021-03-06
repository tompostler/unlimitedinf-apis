﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;

namespace Unlimitedinf.Apis.Server.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("random")]
    public class RandomController : Controller
    {
        private static RNGCryptoServiceProvider _rngCsp = new RNGCryptoServiceProvider();

        [HttpGet]
        public IActionResult Default(ulong min = 0, ulong max = 101)
        {
            // Standard inclusive lower bound, exclusive upper bound. Defaults to [0,100]
            if (min >= max)
                return BadRequest("max must be greater than min");

            ulong val = Generate(64);

            val %= max - min;
            val += min;

            return Ok(val);
        }

        [HttpGet("bit")]
        public IActionResult Bit()
        {
            return Ok(Generate(1));
        }

        [HttpGet("crumb")]
        public IActionResult Crumb()
        {
            return Ok(Generate(2));
        }

        [HttpGet("nibble")]
        public IActionResult Nibble()
        {
            return Ok(Generate(4));
        }

        [HttpGet("byte")]
        public IActionResult Byte()
        {
            return Ok(Generate(8));
        }

        [HttpGet("short")]
        public IActionResult Short()
        {
            return Ok(Generate(16));
        }

        [HttpGet("int")]
        public IActionResult Int()
        {
            return Ok(Generate(32));
        }

        [HttpGet("long")]
        public IActionResult Long()
        {
            return Ok(Generate(64));
        }

        [HttpGet("bits/{num}")]
        public IActionResult Bits(byte num)
        {
            if (num <= 0 || num > 64)
                return BadRequest("That is not a valid value for 'num'.");

            return Ok(Generate(num));
        }

        [HttpGet("guid")]
        public IActionResult GuidMethod()
        {
            return Ok(Guid.NewGuid().ToString("D"));
        }

        private static readonly ulong[] bitmaps = new ulong[]
        {
            0x0000000000000000,
            0x0000000000000001, 0x0000000000000003, 0x0000000000000007, 0x000000000000000F,
            0x000000000000001F, 0x000000000000003F, 0x000000000000007F, 0x00000000000000FF,
            0x00000000000001FF, 0x00000000000003FF, 0x00000000000007FF, 0x0000000000000FFF,
            0x0000000000001FFF, 0x0000000000003FFF, 0x0000000000007FFF, 0x000000000000FFFF,
            0x000000000001FFFF, 0x000000000003FFFF, 0x000000000007FFFF, 0x00000000000FFFFF,
            0x00000000001FFFFF, 0x00000000003FFFFF, 0x00000000007FFFFF, 0x0000000000FFFFFF,
            0x0000000001FFFFFF, 0x0000000003FFFFFF, 0x0000000007FFFFFF, 0x000000000FFFFFFF,
            0x000000001FFFFFFF, 0x000000003FFFFFFF, 0x000000007FFFFFFF, 0x00000000FFFFFFFF,
            0x00000001FFFFFFFF, 0x00000003FFFFFFFF, 0x00000007FFFFFFFF, 0x0000000FFFFFFFFF,
            0x0000001FFFFFFFFF, 0x0000003FFFFFFFFF, 0x0000007FFFFFFFFF, 0x000000FFFFFFFFFF,
            0x000001FFFFFFFFFF, 0x000003FFFFFFFFFF, 0x000007FFFFFFFFFF, 0x00000FFFFFFFFFFF,
            0x00001FFFFFFFFFFF, 0x00003FFFFFFFFFFF, 0x00007FFFFFFFFFFF, 0x0000FFFFFFFFFFFF,
            0x0001FFFFFFFFFFFF, 0x0003FFFFFFFFFFFF, 0x0007FFFFFFFFFFFF, 0x000FFFFFFFFFFFFF,
            0x001FFFFFFFFFFFFF, 0x003FFFFFFFFFFFFF, 0x007FFFFFFFFFFFFF, 0x00FFFFFFFFFFFFFF,
            0x01FFFFFFFFFFFFFF, 0x03FFFFFFFFFFFFFF, 0x07FFFFFFFFFFFFFF, 0x0FFFFFFFFFFFFFFF,
            0x1FFFFFFFFFFFFFFF, 0x3FFFFFFFFFFFFFFF, 0x7FFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF
        };

        private static ulong Generate(byte bits)
        {
            var bytes = new byte[8];
            _rngCsp.GetBytes(bytes);
            ulong val = 0;
            for (int i = 0; i < 8; i++)
                val = val + (ulong)(bytes[i] << i * 8);

            return val & bitmaps[bits];
        }
    }
}