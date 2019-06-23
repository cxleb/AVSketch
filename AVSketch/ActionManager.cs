using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AVSketch.VectorModel;

namespace AVSketch
{
    enum ActionType
    {
        add,
        delete,
        modify
    }

    class Action
    {
        public string uid;
        public ActionType type;
        public VectorObject obj;

        public Action(string uid, ActionType type, VectorObject obj)
        {
            this.uid = uid;
            this.type = type;
            this.obj = obj;
        }
    }

    class ActionManager
    {
        List<Action> stack;
        int pointer;

        public ActionManager()
        {
            stack = new List<Action>();
        }

        public void push(Action action)
        {
            stack.RemoveRange(pointer, stack.Count - pointer);
            stack.Add(action);
            pointer++;
        }

        public Action undo()
        {
            if (pointer > 0)
            {
                pointer--;
                //print();
                return stack[pointer];
            }
            return null;
        }

        public Action redo()
        {
            if(pointer >= stack.Count)
                return null;
            pointer++;
            return stack[pointer-1];
        }
    }
}
