﻿using System;
using System.Collections.Generic;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class LeafDictionary
    {
        private Dictionary<PhysicalObject, Leaf> leafDictionary = new Dictionary<PhysicalObject, Leaf>();

        public void SetLeaf(PhysicalObject obj, Leaf leaf)
        {
            //this.Invariant();
            if (leaf != null)
            {
                if (!leafDictionary.ContainsKey(obj))
                {
                    leafDictionary.Add(obj, leaf);
                }
                leafDictionary[obj] = leaf;
            }
            else
            {
                leafDictionary.Remove(obj);
            }
            //this.Invariant();
        }
        public Leaf GetLeaf(PhysicalObject obj)
        {
            //this.Invariant();
            if (leafDictionary.ContainsKey(obj))
            {
                if (!leafDictionary[obj].Contains(obj))
                {
                    throw new Exception("Incorrect leaf");
                }

                return leafDictionary[obj];
            }
            throw new Exception("object does not have leaf");
        }

        public void DestroyLeaf(Leaf l)
        {
            Dictionary<PhysicalObject, Leaf> copy = new Dictionary<PhysicalObject, Leaf>(leafDictionary);
            foreach (PhysicalObject obj in copy.Keys)
            {
                if (copy[obj] == l)
                {
                    leafDictionary.Remove(obj);
                }
            }
        }
    }
}
