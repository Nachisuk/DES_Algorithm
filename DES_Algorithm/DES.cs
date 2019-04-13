using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DES_Algorithm
{
    class DES
    {
        BitArray bitArray = new BitArray(64);

        public DES(BitArray array)
        {
            this.bitArray = array;
        }

        public void DES_Cipher()
        {
            InitialPermutation();
        }

        private void InitialPermutation()
        {

        }
        private void InverseInitialPermutation()
        {

        }
    }
}
