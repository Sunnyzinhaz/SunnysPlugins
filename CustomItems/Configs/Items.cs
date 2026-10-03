using System.Collections.Generic;
using CustomItems.Items;

namespace CustomItems.Configs
{
    public class Items
    {
        public List<Scp035Item> Scp035Items { get; private set; } =
        [
            new Scp035Item(),
        ];
        
        public List<Scp500A> Scp500As { get; private set; } =
        [
            new Scp500A(),
        ];

        public List<Scp500B> Scp500Bs { get; private set; } =
        [
            new Scp500B(),
        ];

        public List<Scp500H> Scp500Hs { get; private set; } =
        [
            new Scp500H(),
        ];

        public List<Scp500D> Scp500Ds { get; private set; } =
        [
            new Scp500D(),
        ];

        public List<Scp500S1> Scp500S1s { get; private set; } =
        [
            new Scp500S1(),
        ];

        public List<Scp500S2> Scp500S2s { get; private set; } =
        [
            new Scp500S2(),
        ];

        public List<Scp500T> Scp500Ts { get; private set; } =
        [
            new Scp500T(),
        ];
        
        public List<Scp500X> Scp500Xs { get; private set; } =
        [
            new Scp500X(),
        ];
    }
}
