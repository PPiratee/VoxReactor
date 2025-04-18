using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class AtomUtils
    {
        public static JSONStorable GetReciever(Atom atom, string recieverName) {
            List<string> storables = atom.GetStorableIDs();
            foreach (string id in storables)
            {
                if (id.Contains(recieverName)) {
                    return atom.GetStorableByID(id);
                }
            }
            throw new Exception("unable to get storable with name " + recieverName);
        }
        public static List<JSONStorable> GetRecievers(Atom atom, string recieverName)
        {
            List<JSONStorable> returnValues = new List<JSONStorable>();
            List<string> storables = atom.GetStorableIDs();
            foreach (string id in storables)
            {
                if (id.Contains(recieverName))
                {
                    returnValues.Add(atom.GetStorableByID(id));
                }
            }
            return returnValues;
        }
        public static Atom getAtomByUid(String id) { 
            List<Atom> atoms = SuperController.singleton.GetAtoms();
            foreach (Atom atom in atoms)
            {
                if (atom.uid == id) {
                    return atom;
                }
            }
            throw new Exception("Unable to get atom with uid: " + id);
        }
    }
}
