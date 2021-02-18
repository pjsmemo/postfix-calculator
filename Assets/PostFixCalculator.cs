using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PostFixCalculator : MonoBehaviour
{


    // Start is called before the first frame update
    void Start ()
    {
        var infix = "(2*(3+6/2)+2)/4+3";
        Debug.Log("infix : " + infix);

        var stackPostfix = InFixToPostFix(infix);
        PrintPostfix(stackPostfix);

        float r = CalculatePostFix(stackPostfix);
        Debug.Log("result : " + r);
    }

    private float CalculatePostFix (Stack<string> stackPostfix)
    {
        // 1. pop해서 숫자면 출력.
        // 2. pop해서 연산자이면 출력에서 숫자 두개 꺼내서 연산 후 다시 출력.
        Stack<string> t = new Stack<string>();

        while(stackPostfix.Count > 0)
        {            
            var pop = stackPostfix.Pop();
            //Debug.Log("? : " + pop);
            //Debug.Log("first : " + pop);
            if (IsOperator(pop))
            {
                var b = t.Pop();
                var a = t.Pop();
                var c = Calculate(a,b,pop);
                //Debug.LogFormat("{0}, {1}, {2}, {3}", a, b, pop, c);
                t.Push(c);
            }
            else
            {
                t.Push(pop);
            }
        }

        return float.Parse(t.Pop());
    }

    private string Calculate (string a, string b, object op)
    {
        float floatA = float.Parse(a);
        float floatB = float.Parse(b);
        float r = 0f;

        switch (op)
        {
            case "+":
                r = floatA + floatB;
                break;

            case "-":
                r = floatA - floatB;
                break;

            case "*":
                r = floatA * floatB;
                break;

            case "/":
                r = floatA / floatB;
                break;
        }

        return r.ToString();
    }

    private void PrintPostfix (Stack<string> stack)
    {
        var tmp = new Stack<string>(stack);

        // reverse
        tmp = new Stack<string>(tmp);

        StringBuilder sb = new StringBuilder();
        while(tmp.Count > 0)
        {
            var p = tmp.Pop();
            sb.Append(p + ", ");
        }
        Debug.Log("postfix : " + sb.ToString());
    }



    // C로 배우는 알고리즘 1권 내용 중..
    //
    // 1. '(' 를 만나면 스택에 푸시 한다.
    // 2. ')' 를 만나면 스택에서 '(' 가 나올 때까지 팝하여 출력하고 '('는 팝하여 버린다.
    // 3. 연산자를 만나면 스택에서 그 연산자보다 낮은 우선순위의 연산자를 만날 때까지 팝하여 출력한 뒤에 자신을 푸시한다.
    // 4. 피연산자는 그냥 출력한다.
    // 5. 모든 입력이 끝나면 스택에 있는 연산자들은 모두 팝하여 출력한다.

    // 예시. (3 + 4 ) * 2;
    //            -출력-   -스택-
    // ( 처리      []      [(]
    // 3 처리      [3]     [(]
    // + 처리      [3]     [(+]
    // ) 처리      [34+]   []
    // * 처리      [34+]   [*]
    // 2 처리      [34+2]  [*]
    // 스택처리     [34+2*] []
    private Stack<string> InFixToPostFix (string infix)
    {
        if (string.IsNullOrEmpty(infix))
        {
            Debug.LogError("no data");
            return null;
        };

        Stack<string> stackInfix = StringToStack(infix);
        Stack<string> stack_tmp = new Stack<string>();
        Stack<string> stackPostfix = new Stack<string>();


        string item = null;
        while (GetNextItem(stackInfix, out item))
        {
            //Debug.Log("item : " + item);

            if (item == "(")
            {
                stack_tmp.Push(item);
            }
            else if (item == ")")
            {
                MoveItemUntil(stack_tmp, stackPostfix, "(");
            }
            else if (IsOperator(item))
            {
                MoveItemLowerThan(stack_tmp, stackPostfix, item);
                stack_tmp.Push(item);
            }
            else
            {
                stackPostfix.Push(item);
            }
        }

        // flush all
        MoveItemUntil(stack_tmp, stackPostfix, "");

        // reverse
        stackPostfix = new Stack<string>(stackPostfix);

        return stackPostfix;
    }


    private void MoveItemLowerThan (Stack<string> from, Stack<string> to, string item)
    {
        int itemLevel = OperatorLevel(item);


        while (from.Count > 0)
        {
            var peek = from.Peek();

            if (IsOperator(peek))
            {
                int peekLevel = OperatorLevel(peek);


                if (itemLevel > peekLevel)
                {
                    break;                    
                }
                else
                {
                    to.Push(from.Pop());
                }
            }
            else
            {
                break;
            }
        }
    }

    private int OperatorLevel (string item)
    {
        if ("()".Contains(item))
        {
            return 0;
        }
        else if ("+-".Contains(item))
        {
            return 1;
        }
        else if ("*/".Contains(item))
        {
            return 2;
        }
        else
        {
            Debug.LogError("잘못된 연산자 : " + item);
            return 3;
        }
    }

    private bool IsOperator (string item)
    {
        return "+-*/()".Contains(item);
    }

    private Stack<string> StringToStack (string s)
    {
        Stack<string> stack = new Stack<string>();

        for (int i = s.Length-1; i >= 0; i--)
        {
            stack.Push(s[i]+"");
        }

        return stack;
    }

    private bool GetNextItem (Stack<string> stackInfix, out string item)
    {
        item = null;

        if (stackInfix.Count == 0)
            return false;


        StringBuilder sb = new StringBuilder();
        bool someCharPushed = false;

        while(stackInfix.Count > 0)
        {
            var peek = stackInfix.Peek();
            int n;
            bool isNumber = int.TryParse(peek, out n);

            if (isNumber || !someCharPushed)
            {
                var pop = stackInfix.Pop();
                sb.Append(pop);
                someCharPushed = true;
            }

            if (!isNumber)
                break;
        }

        item = sb.ToString();

        return true;
    }

    private void MoveItemUntil (Stack<string> from, Stack<string> to, string compareString)
    {
        while (from.Count > 0)
        {
            var s = from.Pop();
            if (s == compareString)
            {
                return;
            }
            else
            {
                to.Push(s);
            }
        }
    }
}
