using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOCHelpers;

namespace AdventCalendar2020.Day21
{
    public class DupdobDay21 : DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            var match = _allergenMatcher.Match(line);
            var details = match.Groups[1].Value.Split(' ');
            var ingredients = details.Take(details.Length - 1).ToList();

            var allergens = match.Groups[2].Value.Split(',').Select(t => t.Trim()).ToList();
            _food.Add((ingredients, allergens));
        }

        public override object GiveAnswer1()
        {
            var allergens = _food.SelectMany(t => t.allergens).Distinct().ToList();
            var ingredients = _food.SelectMany(t => t.ingredients).Distinct().ToList();
            var allergicIngredientCandidates = new Dictionary<string, List<string>>();
            foreach (var tuple in _food)
            {
                foreach (var allergen in tuple.allergens)
                {
                    if (!allergicIngredientCandidates.ContainsKey(allergen))
                    {
                        allergicIngredientCandidates[allergen] = tuple.ingredients;
                    }
                    else
                    {
                        allergicIngredientCandidates[allergen] = allergicIngredientCandidates[allergen].Intersect(tuple.ingredients).ToList();
                    }
                }
            }
            
            var todo = allergens.ToList();
            _mapping = new Dictionary<string, string>();
            while (todo.Count > 0)
            {
                var (allergen, ingredientList) = allergicIngredientCandidates.First(t
                    => t.Value.Count( i => !_mapping.ContainsKey(i))==1);
                var ingredient = ingredientList.First(i => !_mapping.ContainsKey(i));
                _mapping[ingredient] = allergen;
                todo.Remove(allergen);
            }

            return (long)_food.SelectMany(t => t.ingredients).Count(i => !_mapping.ContainsKey(i));
        }

        public override object GiveAnswer2()
        {
            var list = _mapping.OrderBy(t => t.Value).Select(t => t.Key).ToList();
            return string.Join(',',list);
        }

        protected override void SetupTestData()
        {
            TestData = @"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)";
            ExpectedResult1 = 5L;
            ExpectedResult2 = "mxmxvkd,sqjhc,fvjkl";
        }

        protected override void CleanUp()
        {
            _food.Clear();
        }

        private readonly Regex _allergenMatcher = new Regex("^(.*)\\(contains (.*)\\)$", RegexOptions.Compiled);
        private readonly List<(List<string> ingredients, List<string> allergens)> _food = new();
        private Dictionary<string, string> _mapping;

        protected override string Input => @"xfvxzl nfcrh kscdhn khnqq jkvcbf jhlvrg chc fk zbh hgrptqb zrvtg tdhv jhqpdf jvjhx rrmlg mxsklfx vzn tmp hrhg fpcstf zgpv pdpgm cbsvx ttkn bkmgx xknj bxfvt bdccss tqjg mkpmkx fhpm kcgtlb ktrkfn schkf gcbbnqb sfb rjnx hcms cjt qhhsj qddlrq flnhl vxzpfp tplrt bnl ddmg mxbbst (contains shellfish, sesame)
vxzpfp shlg zrvtg ghq vzn ppqs mqlnbjq cltpv pvxpgp psbnqz nqcmm sqjcsxjq xftxgfj glxxp rrmlg fczfjr phsr dccscx mxlzjfr skrrb flnhl rzlkxt rdrzjqh vjbgmpr tfgf pvx jbcjk chc pqnx pdpgm dlzpmb mkpmkx sbhdb szdxjlb ttkn ljpzx kvrtf ccvzk fjsq sfb xflfx qgs gcsjjf xfpc gkfzqvc hgrptqb cdslv rzlbs tzrp rjnx fk phfmh kbgx brdx qhhsj qtgv ktrkfn fhpm msrnhf cbhfpzp khnqq dhqbf (contains fish)
fzhmtds zbclt pbln qtgv bnl xqcb vxzpfp jkvcbf nnst tplrt xknj hzmvzrr xfvxzl gkddl dqz brdx tqjg hgrptqb pvxpgp cdslv kgrld mdqlbx nv lsngc phfmh ttkn bxfvt ftkfc njtfs tlgchbh xhb vbvfr flnhl qrnxd tmp djldkx kkcm xfpc ghq ktrkfn lvdcdg strd mkpmkx vtdbl pdpgm xknh kbgx hctgkpd (contains sesame, fish, shellfish)
ghq vfqh vtdbl bkmgx bxlc sbhdb xfvxzl lvdcdg nsktq hrjrm dbr prrfm dbqbv jbcjk mkpmkx njtfs flnhl nfsrv lfmng fhznhmx rmxzzf msrnhf nqcmm ghzx pdpgm dzqs dhqbf mqlnbjq ktrkfn mxsklfx gbplfj gcsjjf ttkn dvzqx vxpbn jrpx cbsvx kbgx sfpc cpp jvjhx tzrp qhhsj cdslv zrvtg gmtkh txr rzlbs sqjcsxjq dlzpmb bxfvt tmp qddlrq fjsq hzmvzrr xsnrd cmqrqcf dqz qgs nhjbd jgpgql dpkkzdgs cjds (contains nuts)
xknj bbbzcmh tzrp xkqgc ktfvb tplrt dqz ntq jbcjk pcnmbt nqcmm tlgchbh bxfvt nfsrv kcgtlb ckpjgb rzlkxt tgcvz fczfjr frv dzqs sqjcsxjq jhqpdf schkf mmdfrfcp phfmh bxlc kscdhn rjnx cdslv vbvfr bkhjxl cjdnjf ccvzk cjds gcbbnqb lgtvqf ttkn vfqh pvx cljvdxp gbplfj mkpmkx tqjg lfmng xfpc cbhfpzp nv jgpgql tmp gkgzn cltpv skrrb gcsjjf frgm vzn kbgx xsnrd szdxjlb pqnx pdpgm rmxzzf xflfx qddlrq hzmvzrr sfpc znkr cpp jrpx prrfm cmqrqcf zrvtg lnmv xftxgfj vxzpfp ksrh xknh ssbldv bkmgx lsngc glxxp jhlvrg fhznhmx (contains dairy, soy)
strd dpdgc cjdnjf flmssjn xfpc mmdfrfcp gpp tqjg znkr mxbbst bbbzcmh jbvrzn cdslv mxlzjfr txr cbsvx dvzqx vtdbl xflfx fczfjr tgcvz cjt nfcrh bkmgx prrfm tjj lgtvqf lfmng rrmlg vzn pbln rdrzjqh kkcm qqbfj kgrld cbhfpzp tdsvx hcms phsr vxpbn gkgzn sfb jsdgn szdxjlb flnhl vfqh vnxgpb dpkkzdgs bxfvt xfvxzl pdpgm xknj qhhsj tmp zrvtg xttdqc khnm ddmg fzhmtds hzpsn zhpv cfbgb vxzpfp jvjhx kcgtlb nnst ttkn sfpc jhqpdf gkddl xhb ghq fhznhmx xknh schkf (contains soy, wheat)
gkgzn bndz dpkkzdgs nv nhjbd mqlnbjq khnqq ljpzx cfbgb xknj ddmg mkpmkx tgcvz nnst szdxjlb sqjcsxjq tmp bnl znkr mxbbst nfcrh mxsklfx qqlm ttkn vxzpfp gmtkh pqnx kpxcvj gpp qhhsj ftkfc glxxp vzn dccscx jrj xqcb zrvtg hhvh phfmh cjdnjf vtdbl hctgkpd phsr rjnx cjds qddlrq pvrhpt vjbgmpr vbvfr gcsjjf kbgx hrhg kgrld ksrh nfsrv qqmjlr tjj ccvzk xsnrd hzmvzrr hrjrm pbln qgs zbh bxfvt dqxjx cdslv kkcm fjsq xfpc ktfvb cbsvx rzlkxt ckpjgb bkhjxl pdpgm strd tdhv fk tdsvx (contains sesame)
gnfjcpc ddmg tpbtpd cltpv xfvxzl txr rmxzzf tgcvz jsdgn pgnk jrpx vbvfr vzn gbplfj xqcb bxfvt fk hzpsn qtgv xbksdv cljvdxp dpdgc lnmv rjnx ttkn prrfm ccvzk hctgkpd dlzpmb dhqbf zgpv ghzx gkddl dqrvvk qrnxd flnhl gkgzn vnxgpb pvrhpt lfmng mkpmkx pqnx cjdnjf gmtkh sfpc mxbbst fpcstf dnbhhv cjds dqz tzrp vtdbl jhlvrg kvrtf pdpgm cjt tmp qddlrq fhznhmx xsnrd flmssjn brdx sfb dpkkzdgs djldkx gpp nhjbd nv psbnqz cdslv frv dbr bxlc xfpc dvzqx hcms zrvtg (contains dairy)
pdpgm dlzpmb flnhl drgdhp jbvrzn xssjz njtfs phsr spclmfb tqjg nhjbd ssbldv prrfm xflfx tjj ljpzx msrnhf bxfvt kbgx xfvxzl fjsq khnqq dpkkzdgs vjbgmpr cjds fhznhmx ppqs vxpbn qhhsj bdccss bnl ttkn fzhmtds bxlc nqcmm hzpsn vtdbl dqrvvk gcbz psbnqz dbr cbsvx tmp pcnmbt lffhf xbksdv sfpc djldkx gpp xknh rdrzjqh skrrb kpxcvj flmssjn zrvtg gkddl cljvdxp pqnx bbbzcmh xfpc lvdcdg vxzpfp ntq cdslv jrj hrjrm fpcstf qqlm sfb pvxpgp (contains sesame, soy)
txr ghq rmxzzf tmp ttkn vxpbn brdx gcbz zbclt hctgkpd khnqq dlzpmb cljvdxp nfsrv kkcm mkpmkx nv tgcvz xbksdv ntq rdrzjqh jbvrzn flmssjn qhhsj bbbzcmh sbhdb tplrt hzpsn ghzx qtgv gcbbnqb flnhl rrmlg vjbgmpr ktfvb dhqbf xfpc cdslv cbhfpzp xhb xttdqc dvzqx hrjrm qgs glxxp ldggb bdccss pdpgm cjds xknh frgm tfgf fzhmtds sqjcsxjq fk ncqd ksrh zrvtg (contains peanuts, wheat)
psbnqz hcms gkddl kgrld dccscx txr tjj ddmg vxzpfp jvjhx qqmjlr bndz mkpmkx brdx gpp jbvrzn gnfjcpc bxlc qqlm flnhl gcsjjf sfb nsktq pbln xttdqc qtgv ccvzk kkcm jkvcbf ttkn vnxgpb ktrkfn dbqbv msrnhf rjnx qqbfj bbbzcmh cljvdxp zrvtg dqz cbsvx xhb xflfx shlg ktfvb nnst ghzx mxsklfx khnqq sqjcsxjq prrfm rdrzjqh xkqgc cbhfpzp mxlzjfr vfqh fjsq dnbhhv rzlbs tmp cdslv jhlvrg fzhmtds qddlrq dpdgc ldggb bkmgx bdccss ghq rrmlg frgm fczfjr pcnmbt lvdcdg qhhsj ftkfc tgcvz mqlnbjq nqcmm cmqrqcf phfmh xqcb drgdhp nv mxbbst znkr (contains peanuts)
hrhg dbqbv mkpmkx gnfjcpc drgdhp cpp nfcrh rjnx cdslv ttkn xssjz khnqq ftkfc qqbfj hcms strd gcbz pvx cfbgb dqz hctgkpd kpxcvj jrpx txr vzn ghzx rzlbs pvxpgp xknh nv xknj pdpgm dqxjx gmtkh vjbgmpr skrrb mxbbst pvrhpt tlgchbh bkhjxl sbhdb frv ssbldv fhznhmx tdhv xbksdv tjj dzqs zrvtg nqcmm ghq dpkkzdgs ntq cbsvx glxxp mxsklfx pqnx vbvfr tplrt tmp vxpbn pgnk xfpc xhb cbhfpzp prrfm flnhl mxlzjfr phsr gkddl cjt gbplfj gkgzn sfpc (contains shellfish)
jhlvrg xflfx bxlc hrhg dvzqx zrvtg ghq pdpgm jgpgql ppqs cfbgb rzlbs ccvzk fhpm ttkn xsnrd gkddl mkpmkx jkvcbf vxzpfp bxfvt tfgf lsngc vtdbl mpc sfb jhqpdf dnbhhv cjdnjf vjbgmpr xhb txr hzpsn gpp cjds fk qhhsj fjsq sfpc cpp cdslv pbln khnqq psbnqz lfmng phfmh dccscx nfsrv jbcjk sbhdb hrjrm flnhl fpcstf hgrptqb qqmjlr rdrzjqh cbhfpzp gcbbnqb (contains wheat)
bdccss txr flmssjn fk ccvzk jbvrzn nqcmm vjbgmpr lnmv tmp dccscx xbksdv vzn dvzqx ghzx jrj xfvxzl dbqbv jhlvrg fczfjr rmxzzf vxzpfp fhznhmx hrhg kcgtlb dzqs kkcm psbnqz jbcjk bxfvt skrrb phfmh mxbbst ncqd shlg khnqq zrvtg hzmvzrr jsdgn kgrld kscdhn vxpbn pdpgm ftkfc vtdbl mkpmkx ttkn zbh cdslv cbhfpzp qgs sbhdb ddmg tpbtpd nfsrv mpc brdx njtfs (contains peanuts, soy)
jbcjk dqz schkf znkr tfgf pcnmbt lnmv kkcm rzlbs lgtvqf kvrtf bxlc zrvtg cltpv shlg ccvzk spclmfb pvxpgp dzqs ghq cjds flnhl xssjz xfvxzl kscdhn vxzpfp psbnqz pdpgm khnm nsktq tmp bdccss hgrptqb dnbhhv vtdbl msrnhf kbgx pvx mkpmkx cmqrqcf lvdcdg cdslv qddlrq qrnxd xhb rdrzjqh jgpgql fhznhmx (contains shellfish, nuts, peanuts)
bxlc kgrld bkhjxl pgnk flmssjn rzlkxt khnqq tplrt phsr prrfm lgtvqf tzrp zbh frv tmp dqz cfbgb skrrb mxsklfx txr mkpmkx jkvcbf xsnrd vxpbn xkqgc ssbldv nsktq ntq mdqlbx vxzpfp pdpgm ghzx hgrptqb gpp flnhl mpc shlg dbqbv cdslv nnst zrvtg jhqpdf xttdqc mmdfrfcp pqnx zbclt sbhdb rmxzzf mqlnbjq gkfzqvc (contains shellfish)
ddmg ksrh jsdgn hrhg djldkx mkpmkx kcgtlb cmqrqcf cbhfpzp qtgv kvrtf shlg frv vzn glxxp sfb ccvzk zbclt bxfvt gnfjcpc dqxjx cdslv xftxgfj vbvfr pvrhpt jbvrzn xkqgc mpc dhqbf spclmfb vjbgmpr fhpm xssjz drgdhp jhlvrg cfbgb nv qhhsj flnhl vxzpfp ttkn ldggb psbnqz ntq lffhf gkfzqvc cbsvx tmp prrfm njtfs hhvh xqcb ssbldv pcnmbt pdpgm (contains fish, shellfish)
rzlbs gbplfj cbsvx sfpc cljvdxp xftxgfj xhb jbcjk tgcvz gcbz bxlc txr ksrh ttkn fhpm nfsrv qhhsj ssbldv cjdnjf bbbzcmh kgrld nv ktrkfn skrrb pvxpgp mqlnbjq sfb ddmg dqxjx dqz vxpbn mdqlbx jbvrzn ktfvb dnbhhv glxxp bkmgx tfgf dvzqx tdsvx cdslv kcgtlb hrhg frgm tmp ghq pdpgm hctgkpd vfqh brdx rjnx znkr kscdhn vxzpfp xknh fhznhmx jrj lsngc mkpmkx cjds gkddl tzrp sbhdb dpdgc xttdqc xflfx zrvtg ppqs flmssjn (contains nuts)
qhhsj nqcmm chc rrmlg cjds bndz bdccss ljpzx vxzpfp glxxp bxlc fk rzlkxt mmdfrfcp ntq flnhl rjnx tmp lnmv nfcrh ttkn dqxjx hrhg cmqrqcf lvdcdg phsr zrvtg gpqdxvv cdslv qgs vtdbl khnm jkvcbf gkgzn phfmh dpdgc gkfzqvc mpc ftkfc txr xfpc tdhv kkcm xqcb fpcstf gcsjjf hzmvzrr gcbz fhpm dvzqx msrnhf vzn fzhmtds pdpgm mqlnbjq sfpc cfbgb qqmjlr strd nsktq nv zgpv (contains dairy, sesame, soy)
ktrkfn rrmlg nfcrh gbplfj rjnx ssbldv nv rzlkxt hrhg tplrt xttdqc ghzx sqjcsxjq ckpjgb lvdcdg msrnhf pqnx bbbzcmh mxsklfx gkfzqvc gnfjcpc kcgtlb cdslv jhqpdf lsngc bdccss qhhsj xflfx tmp jsdgn dpdgc vbvfr qrnxd dzqs dbqbv zrvtg jrpx zhpv sfpc hhvh ncqd sbhdb tdsvx dlzpmb phfmh ccvzk xbksdv nnst pdpgm ttkn flnhl mkpmkx dpkkzdgs (contains dairy, shellfish)
hcms lfmng ljpzx vxzpfp pbln pvrhpt phfmh qgs skrrb vzn mkpmkx bndz hzmvzrr tmp cbsvx gnfjcpc hzpsn jhqpdf gkfzqvc lgtvqf pdpgm hrjrm bxlc ttkn cbhfpzp cljvdxp jvjhx tlgchbh zhpv zgpv xbksdv dnbhhv txr cmqrqcf tpbtpd tzrp glxxp cdslv xhb jgpgql nhjbd lnmv dlzpmb prrfm dhqbf flnhl tdsvx (contains soy, sesame, shellfish)
qqbfj ccvzk cjds tdhv fhznhmx xftxgfj xknj kscdhn ktfvb kvrtf prrfm gkddl hzpsn gmtkh lgtvqf xfvxzl xttdqc hrhg bnl ljpzx dqxjx njtfs zbclt dqz ntq dccscx pdpgm nsktq gkgzn bdccss cfbgb pvrhpt tplrt ssbldv vtdbl zrvtg txr chc bkhjxl drgdhp ddmg mxlzjfr glxxp fk xfpc dhqbf gcbbnqb mxbbst vxzpfp cdslv dlzpmb bbbzcmh gcbz skrrb hrjrm zhpv qtgv hcms nhjbd qqlm cmqrqcf hgrptqb qgs ftkfc psbnqz xkqgc tmp gnfjcpc mpc mkpmkx ttkn mdqlbx tlgchbh ldggb brdx (contains dairy)
cjds mxsklfx frv qtgv phsr dpkkzdgs ddmg hrjrm hzmvzrr shlg dhqbf ftkfc nfsrv dlzpmb fhpm tdhv xsnrd tdsvx lgtvqf rzlbs nnst msrnhf cljvdxp djldkx xfvxzl kkcm xknj xhb tplrt vxzpfp dnbhhv qqlm rzlkxt hcms ttkn ppqs khnm bbbzcmh tmp pbln qrnxd phfmh gcbbnqb zrvtg ksrh pcnmbt nsktq flnhl lnmv tpbtpd gmtkh ldggb gkgzn ckpjgb pvx cmqrqcf jrpx szdxjlb pdpgm zbh jbcjk dpdgc nv khnqq dbqbv cdslv (contains nuts, peanuts)
kgrld zrvtg rjnx jrj mxlzjfr xftxgfj gkfzqvc nnst gnfjcpc fczfjr pvrhpt xhb fzhmtds frv tgcvz tlgchbh xqcb xbksdv dpkkzdgs tmp cjdnjf mdqlbx bbbzcmh pvx ljpzx tjj gcbbnqb hrjrm bkmgx sbhdb glxxp cfbgb cdslv xknh pbln ckpjgb dqz rmxzzf flnhl dlzpmb vbvfr mkpmkx pdpgm cjt vxpbn ttkn kpxcvj znkr qhhsj kscdhn dqrvvk xsnrd sfb (contains shellfish, sesame)
mxbbst tgcvz xbksdv chc gkfzqvc dzqs vxzpfp ksrh qddlrq vtdbl gpp vfqh mxsklfx znkr mkpmkx kpxcvj rdrzjqh ddmg glxxp hhvh rmxzzf dpkkzdgs qtgv xhb zbclt xsnrd pvx cmqrqcf skrrb ghzx bkhjxl tplrt dqrvvk cltpv kgrld cdslv qqmjlr pbln hzpsn gpqdxvv nv dlzpmb kvrtf dvzqx tfgf hctgkpd ttkn jgpgql dbqbv phfmh bkmgx dccscx jvjhx tmp ktrkfn qqlm jrpx gcsjjf qqbfj bndz nfsrv pdpgm ssbldv cjds gcbbnqb xfvxzl vbvfr ljpzx xknh lgtvqf zbh tdhv kkcm frv shlg nqcmm tlgchbh djldkx lvdcdg zrvtg gnfjcpc vzn cfbgb qhhsj zhpv fzhmtds fk xqcb tqjg nnst (contains soy)
jrj rjnx vbvfr nv vnxgpb rmxzzf qgs mkpmkx bbbzcmh xftxgfj gpp hgrptqb kcgtlb qqmjlr vzn zbclt jhqpdf hrjrm vjbgmpr tgcvz ldggb ssbldv ttkn cjdnjf kbgx kscdhn jgpgql jrpx xknh pdpgm bkmgx dbqbv frv schkf hhvh qrnxd nfcrh tplrt glxxp skrrb vxzpfp pvxpgp fhpm tdsvx bxlc cpp rzlbs ckpjgb tmp cjt pvrhpt nhjbd xsnrd bkhjxl strd dqrvvk gcbbnqb gkgzn xfvxzl nqcmm bxfvt tqjg zrvtg cmqrqcf ntq zgpv frgm tfgf fjsq xhb lsngc qqlm xknj fzhmtds khnm cjds flnhl kgrld (contains fish, shellfish, dairy)
lfmng psbnqz xknj gkddl cmqrqcf mkpmkx mxlzjfr bkmgx tgcvz pcnmbt vxzpfp tplrt mmdfrfcp khnqq vfqh cjdnjf gpqdxvv ttkn lvdcdg bdccss zbclt tmp kscdhn sqjcsxjq hzpsn flnhl flmssjn cjds xftxgfj xssjz brdx xkqgc pdpgm cfbgb qhhsj skrrb phfmh pvrhpt cdslv tlgchbh fk dnbhhv tzrp mqlnbjq jgpgql nqcmm (contains wheat, fish, nuts)
ddmg djldkx pdpgm txr vzn dnbhhv sfpc zrvtg fhznhmx lfmng hrjrm ntq zbclt bdccss xfpc ttkn tjj pcnmbt cmqrqcf fzhmtds qhhsj xkqgc xssjz cdslv nv flnhl mxlzjfr ckpjgb cltpv kpxcvj jrpx hgrptqb xhb sqjcsxjq ktfvb xqcb zhpv kbgx kcgtlb cjds rdrzjqh jgpgql lnmv mkpmkx lvdcdg cfbgb lsngc nfcrh rzlkxt ktrkfn tmp tfgf (contains wheat, nuts, fish)
fczfjr dvzqx zgpv dqxjx hhvh bnl glxxp tlgchbh qrnxd ghq tfgf kscdhn gmtkh flnhl msrnhf tqjg qddlrq qqmjlr rzlkxt cpp schkf tdhv xqcb vzn khnqq fk xttdqc tmp nsktq hzpsn kvrtf frv hcms fpcstf szdxjlb qqbfj tjj dlzpmb dbqbv qtgv cdslv jhlvrg hgrptqb flmssjn rdrzjqh zrvtg njtfs xknj cjds mxsklfx ncqd pqnx gpqdxvv shlg jbcjk mkpmkx xhb vxzpfp ttkn jvjhx kpxcvj mxbbst fzhmtds jrj bxfvt vfqh (contains shellfish, wheat)
nsktq gkgzn mmdfrfcp ttkn zhpv gkfzqvc rzlbs znkr sbhdb nqcmm dqxjx bndz xflfx frgm ncqd lvdcdg jbvrzn dbqbv zrvtg lsngc qgs lgtvqf cdslv khnm tmp qddlrq xbksdv jkvcbf strd bbbzcmh kscdhn spclmfb ljpzx pqnx hzpsn cltpv mkpmkx skrrb pbln vxzpfp hgrptqb tgcvz ntq mxbbst ckpjgb bdccss cmqrqcf tzrp fk gbplfj tpbtpd pvxpgp hhvh qqmjlr tplrt qrnxd mxsklfx hzmvzrr pdpgm dhqbf xftxgfj khnqq ccvzk bkmgx nv (contains peanuts, dairy)
vfqh mkpmkx vxzpfp hrjrm kpxcvj ghzx dqz lfmng zrvtg pvxpgp flnhl gpqdxvv dpkkzdgs tdsvx dbr cbsvx kcgtlb xfpc mmdfrfcp jvjhx fhznhmx tmp msrnhf xflfx mxsklfx gbplfj tqjg tdhv vtdbl nv drgdhp bndz znkr xttdqc pdpgm ttkn mxbbst rmxzzf pbln lffhf lsngc tgcvz qgs fpcstf szdxjlb sfpc xsnrd hctgkpd cljvdxp rrmlg cmqrqcf ntq sbhdb qrnxd djldkx dvzqx tjj kbgx hgrptqb rzlkxt nfsrv xkqgc lgtvqf jsdgn vbvfr (contains soy)
szdxjlb zrvtg qqlm kcgtlb lgtvqf gkgzn jvjhx xkqgc ssbldv flmssjn xttdqc ktfvb nqcmm cbhfpzp qrnxd cdslv sfpc gkddl chc jsdgn dlzpmb rzlkxt tqjg cjt vxzpfp tfgf xknj msrnhf tzrp kkcm ttkn flnhl pvrhpt mxsklfx skrrb nv tmp frgm hrhg hrjrm qtgv dpkkzdgs xftxgfj fjsq gcsjjf mkpmkx vnxgpb (contains sesame, dairy, nuts)
vbvfr psbnqz jgpgql ktrkfn nsktq dpkkzdgs lffhf kvrtf nv gkgzn gcbz bbbzcmh ktfvb pgnk mmdfrfcp vfqh mqlnbjq ttkn cljvdxp fhpm bxfvt sbhdb hrjrm jbvrzn qtgv vxpbn qrnxd tdhv cpp dnbhhv hhvh zrvtg xbksdv tgcvz xsnrd gnfjcpc frgm cltpv kkcm cdslv kscdhn dqz bkmgx tjj rjnx fk pdpgm mdqlbx drgdhp dpdgc jrj hcms dbr jsdgn strd qqlm lsngc tmp bxlc sfpc chc jvjhx jrpx nhjbd flnhl fjsq mkpmkx gkddl tqjg xknh (contains peanuts)
tpbtpd psbnqz mxbbst bndz vxzpfp pbln qqbfj chc cfbgb dpkkzdgs qtgv gmtkh mkpmkx lffhf strd ghq rzlkxt vfqh mqlnbjq cjt flnhl ntq ppqs drgdhp mmdfrfcp gpqdxvv bkhjxl zgpv ftkfc jrj tjj cljvdxp gcbbnqb zrvtg sfpc ttkn kcgtlb tdsvx tqjg ktfvb tgcvz dbqbv cdslv xkqgc nsktq skrrb tzrp sqjcsxjq xftxgfj dqrvvk znkr dnbhhv pdpgm (contains sesame, shellfish)
lffhf cljvdxp hzmvzrr mdqlbx hrjrm nfcrh njtfs bnl tgcvz xqcb cbhfpzp ktfvb brdx khnqq mpc zrvtg lgtvqf qhhsj mqlnbjq jbvrzn bdccss nhjbd dnbhhv txr msrnhf cjds kvrtf gmtkh dhqbf qqlm xttdqc ftkfc cmqrqcf mxsklfx gnfjcpc xsnrd sfpc dbqbv qrnxd pgnk flnhl jgpgql znkr tpbtpd jrj dqrvvk bxlc zbh mxlzjfr skrrb vnxgpb bkmgx kcgtlb vxzpfp zhpv dccscx drgdhp ljpzx pdpgm gbplfj ccvzk tmp gcbbnqb ssbldv tplrt lnmv vzn dqxjx mxbbst ghq hrhg mkpmkx vxpbn dlzpmb nnst ttkn pvrhpt gcsjjf ddmg vfqh (contains shellfish)
dqrvvk mxbbst ftkfc vjbgmpr dnbhhv fjsq pgnk xkqgc ccvzk fhznhmx cltpv gkddl jsdgn zbh vxzpfp fhpm gmtkh sbhdb kgrld lgtvqf kkcm tlgchbh qqbfj tmp zrvtg lfmng ssbldv chc xftxgfj bdccss tdsvx znkr ddmg fczfjr mdqlbx vzn dccscx xsnrd bbbzcmh cdslv xfvxzl szdxjlb bndz hrjrm drgdhp ncqd rdrzjqh mkpmkx kscdhn glxxp dbr xfpc pdpgm fpcstf hcms flnhl qtgv cpp gnfjcpc nhjbd qqmjlr jgpgql ldggb sfb bxlc (contains nuts)
xknj psbnqz tzrp vxzpfp gmtkh mxlzjfr jsdgn lgtvqf kpxcvj glxxp tplrt ttkn djldkx dqxjx pvx gpp vbvfr dccscx cdslv hcms sqjcsxjq ktfvb dbqbv dpkkzdgs kgrld dqrvvk vzn flnhl bdccss mqlnbjq pbln lffhf tmp mxsklfx frv sfb xhb mkpmkx shlg brdx pdpgm lfmng dlzpmb tqjg ldggb hzmvzrr (contains dairy, wheat)
tmp cdslv cljvdxp rrmlg lffhf hgrptqb pgnk brdx dqrvvk dqz schkf szdxjlb bxfvt pbln vxzpfp dpkkzdgs mqlnbjq xssjz sbhdb msrnhf pvrhpt xsnrd cmqrqcf nfcrh sfb cbhfpzp phfmh tqjg chc jbcjk tplrt qhhsj ttkn bndz pdpgm rmxzzf ldggb mkpmkx ghzx khnqq vzn xkqgc kkcm zrvtg ckpjgb dnbhhv dzqs ghq hzpsn jkvcbf tpbtpd bkhjxl strd rzlbs hcms tjj fhznhmx jbvrzn xbksdv jgpgql phsr (contains fish, soy, nuts)
nsktq ftkfc mdqlbx bxfvt vtdbl djldkx cljvdxp cpp ghzx sbhdb ppqs dvzqx zhpv kgrld hcms jsdgn cjdnjf flnhl dzqs dqz nfcrh mkpmkx sfpc dpkkzdgs xttdqc xftxgfj frv qgs hgrptqb vxzpfp ghq zrvtg bndz cdslv gkfzqvc sfb kscdhn pbln tmp kcgtlb gnfjcpc cfbgb ckpjgb tgcvz nhjbd pgnk xknj skrrb jkvcbf pcnmbt hrjrm glxxp cbhfpzp sqjcsxjq hzmvzrr xhb mxlzjfr bxlc qrnxd qqmjlr jhlvrg khnqq drgdhp pvrhpt gmtkh qqbfj psbnqz tpbtpd vjbgmpr ttkn shlg pvx fzhmtds fk mxbbst rmxzzf rrmlg pvxpgp tjj zbclt kpxcvj kvrtf nnst dqrvvk (contains peanuts)
fhznhmx khnm skrrb ckpjgb glxxp rzlkxt jrj mxsklfx mdqlbx mkpmkx xhb bkhjxl ttkn flnhl njtfs vfqh dqxjx pqnx xfpc cdslv sfb xbksdv zrvtg jvjhx zbclt pvx xsnrd lgtvqf kbgx ntq pbln xttdqc sfpc lffhf spclmfb gpqdxvv qqlm jbcjk rdrzjqh dlzpmb mqlnbjq kkcm tdsvx tmp nv vtdbl gkddl znkr xknh fczfjr djldkx brdx tfgf gmtkh pdpgm dpdgc fpcstf bndz kgrld tdhv frv pcnmbt drgdhp dpkkzdgs fk jbvrzn xfvxzl gnfjcpc pvrhpt ktrkfn cmqrqcf vzn phfmh gkgzn kscdhn txr zgpv (contains fish, shellfish)
cbhfpzp tmp rjnx dpdgc frv xflfx dbr fzhmtds kcgtlb msrnhf khnm hgrptqb tfgf xssjz hhvh bnl ncqd qgs vxzpfp xfpc zrvtg mqlnbjq sfpc ppqs cdslv phsr vnxgpb fhznhmx bbbzcmh pvxpgp tzrp ttkn xqcb dhqbf dzqs ljpzx bkhjxl djldkx gkddl lsngc cljvdxp hctgkpd dccscx xfvxzl vbvfr tjj lnmv ssbldv tgcvz kgrld ccvzk cbsvx xkqgc qddlrq qtgv nqcmm mxsklfx pgnk dpkkzdgs jgpgql lvdcdg drgdhp tdsvx dbqbv gnfjcpc xftxgfj qrnxd ckpjgb ftkfc fhpm flnhl nv cjdnjf sfb rrmlg qqmjlr pdpgm nfsrv dvzqx cjds jhlvrg spclmfb qhhsj (contains sesame, nuts, soy)
flnhl gkddl jhqpdf jkvcbf xqcb gcbbnqb ddmg ppqs bnl jrpx cdslv bxlc tfgf msrnhf strd vxzpfp jsdgn kcgtlb cljvdxp glxxp pvx prrfm qqmjlr drgdhp tmp jrj zrvtg rmxzzf zhpv vnxgpb ldggb ksrh rzlkxt qqbfj lnmv njtfs vjbgmpr pdpgm tpbtpd vxpbn dzqs dnbhhv dpkkzdgs dqz cbhfpzp djldkx mqlnbjq gkfzqvc lfmng pqnx jvjhx gmtkh jbcjk dvzqx pgnk szdxjlb ttkn dqrvvk cltpv (contains wheat, nuts)";
        public override int Day => 21;
    }
}