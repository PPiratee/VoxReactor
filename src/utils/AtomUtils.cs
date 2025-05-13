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
        //todo move to a different util
       

        public static void RunAfterDelay(float delay, Action actionToCall)
        {
            Main.singleton.RunCoroutine(RunAfterDelayEnumerator(delay, actionToCall));
        }

        private static IEnumerator RunAfterDelayEnumerator(float delay, Action actionToCall)
        {
            // Wait for a random delay between minDelay and maxDelay
            yield return new WaitForSeconds(delay);

            // Call the provided action
            actionToCall();
        }

        public static List<string> ConcatList(List<string> list, List<string>  other) {
            foreach (var item in other)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
