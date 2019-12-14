using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2019.Day14
{
    public class DupdobDay14
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay14();
            runner.Parse();
            Console.WriteLine("Answer 1: {0}", runner.ComputeNeededOre());
            Console.WriteLine("Answer 2: {0}", runner.ComputeFuelWithALotOfLore());
        }

        private void Parse(string input=Input)
        {
            var ingredientMatcher = new Regex(" *(\\d+) *(\\w+) *", RegexOptions.Compiled);
            foreach (var line in input.Split('\n'))
            {
                var bloc = line.Split("=>");
                var result = ingredientMatcher.Match(bloc[1]);
                if (!result.Success)
                {
                    throw new ApplicationException("Failed to parse a line");
                }

                var component = result.Groups[2].Value;
                var qty = int.Parse(result.Groups[1].Value);
                var resultEntry = new List<(string component, long qty)>();
                foreach (var entry in bloc[0].Split(','))
                {
                    result = ingredientMatcher.Match(entry);
                    if (!result.Success)
                    {
                        throw new ApplicationException("Failed to parse a line");
                    }
                    resultEntry.Add((result.Groups[2].Value, int.Parse(result.Groups[1].Value)));
                }

                recipes[component] = (qty, resultEntry);
            }
        }

        private long ComputeNeededOre(long amount = 1)
        {
            var stock = new Dictionary<string, long>();
            var result = Build("FUEL", amount, stock);
            return result["ORE"];
        }
        
        private long ComputeFuelWithALotOfLore()
        {
            var stock = 1000000000000L;
            var oreForOne = ComputeNeededOre();
            var fuel = 1000000000000L/oreForOne;
            var lastFuel = 0L;
            var wasAbvove = false;
            for (;;)
            {
                var neededOre = ComputeNeededOre(fuel);
                if (Math.Abs(lastFuel - fuel) == 1)
                {
                    if (wasAbvove == (neededOre < stock))
                    {
                        fuel = Math.Min(fuel, lastFuel);
                        break;
                    }

                }
                if (neededOre == stock)
                {
                    break;
                }

                if (neededOre < stock)
                {
                    lastFuel = fuel;
                    var step = (stock - neededOre)/oreForOne;
                    if (step < 1)
                    {
                        step = 1;
                    }
                    fuel += step;
                    wasAbvove = false;
                }
                else
                {
                    lastFuel = fuel;
                    var step = (stock - neededOre)/oreForOne;
                    if (step > -1)
                    {
                        step = -1;
                    }
                    fuel += step;
                    wasAbvove = true;
                }
            }
            return fuel;
        }

        private IDictionary<string, long> Build(string component, long quantity, IDictionary<string, long> stock)
        {
            var needed = new Dictionary<string, long>();
            if (!recipes.ContainsKey(component))
            {
                needed.Add(component, quantity);
                return needed;
            }

            if (stock.ContainsKey(component))
            {
                // do we have stock?
                if (stock[component] >= quantity)
                {
                    stock[component] -= quantity;
                    // we have not spend anything
                    return needed;
                }
                // not enough stock
                quantity -= stock[component];
                stock.Remove(component);
            }

            var (qty, ingredients) = recipes[component];
            var factor = (quantity + qty-1) / qty;
            foreach ((string comp, long amount ) in ingredients)
            {
                var need= Build(comp, amount*factor, stock);
                foreach (var entry in need)
                {
                    if (!needed.ContainsKey(entry.Key))
                    {
                        needed.Add(entry.Key, entry.Value);
                    }
                    else
                    {
                        needed[entry.Key] += entry.Value;
                    }
                }
            }
            stock[component] = factor * qty - quantity;
            return needed;
        }
        
  

        private readonly Dictionary<string, (long qty, List<(string comp, long qty)>)> recipes = new Dictionary<string, (long qty, List<(string comp, long qty)>)>();
        private const string Input =
@"11 RVCS => 8 CBMDT
29 QXPB, 8 QRGRH => 8 LGMKD
3 VPRVD => 6 PMFZG
1 CNWNQ, 11 MJVXS => 6 SPLM
13 SPDRZ, 13 PMFZG => 2 BLFM
8 QWPFN => 7 LWVB
1 SPLM => 8 TKWQ
2 QRGRH, 6 CNWNQ => 7 DTZW
2 DMLT, 1 SPLM, 1 TMDK => 9 NKNS
1 MJVXS, 1 HLBV => 7 PQCQH
1 JZHZP, 9 LWVB => 7 MJSCQ
29 DGFR => 7 QRGRH
14 XFLKQ, 2 NKNS, 4 KMNJF, 3 MLZGQ, 7 TKWQ, 24 WTDW, 11 CBMDT => 4 GJKX
4 TKWQ, 1 WLCFR => 4 PDKGT
2 NKNS => 4 GDKL
4 WRZST => 9 XFLKQ
19 DGFR => 4 VPRVD
10 MJSCQ, 4 QWPFN, 4 QXPB => 2 MLZGQ
1 JZHZP => 7 QWPFN
1 XFLKQ => 9 FQGVL
3 GQGXC => 9 VHGP
3 NQZTV, 1 JZHZP => 2 NVZWL
38 WLCFR, 15 GJKX, 44 LGMKD, 2 CBVXG, 2 GDKL, 77 FQGVL, 10 MKRCZ, 29 WJQD, 33 BWXGC, 19 PQCQH, 24 BKXD => 1 FUEL
102 ORE => 5 DGFR
17 NWKLB, 1 SBPLK => 5 HRQM
3 BWXGC => 8 TQDP
1 TQDP => 2 PSZDZ
2 MJVXS => 9 WNXG
2 NBTW, 1 HRQM => 2 SVHBH
8 CNWNQ, 1 DTZW => 4 RVCS
4 VHGP, 20 WNXG, 2 SVHBH => 3 SPDRZ
110 ORE => 5 TXMC
10 QRGRH => 5 NWKLB
1 SBPLK => 3 MJVXS
9 DGFR => 5 RFSRL
5 LBTV => 3 DMLT
1 NWKLB, 1 KMNJF, 1 HDQXB, 6 LBTV, 2 PSZDZ, 34 PMFZG, 2 SVHBH => 2 WJQD
1 RVCS => 5 MKRCZ
14 NQZTV, 3 FPLT, 1 SJMS => 2 GQGXC
18 RFSRL, 13 VHGP, 23 NBTW => 5 WTDW
1 VHGP, 6 TKWQ => 7 QXPB
1 JZHZP, 1 CNWNQ => 5 KMNJF
109 ORE => 9 BWXGC
2 CNWNQ, 1 PDKGT, 2 KMNJF => 5 HDQXB
1 PDKGT, 18 WRZST, 9 MJSCQ, 3 VHGP, 1 BLFM, 1 LGMKD, 7 WLCFR => 2 BKXD
11 MLJK => 6 FPLT
8 DGFR, 2 TXMC, 3 WJRC => 9 SJMS
2 SBPLK => 1 LBTV
22 QWPFN => 4 WRZST
5 WRZST, 22 WNXG, 1 VHGP => 7 NBTW
7 RVCS => 9 TMDK
1 DGFR, 14 TXMC => 5 JZHZP
2 JZHZP => 3 SBPLK
19 PDKGT => 8 HLBV
195 ORE => 6 WJRC
6 GQGXC => 8 CNWNQ
1 NVZWL, 4 GQGXC => 2 CBVXG
1 NVZWL, 1 KMNJF => 8 WLCFR
153 ORE => 4 MLJK
1 BWXGC => 6 NQZTV";
    }
}