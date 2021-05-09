using Modules.Resources;
using Type = Yarn.Type;

public class ResourceCondition
{
    public string resource;
    public Operator op;
    public int value;
    public ResourceCondition(string type, Operator op, int value)
    {
        resource = type;
        this.op = op;
        this.value = value;
    }
}

public enum Operator
{
    None,Eq, Lt, Gt   
}
public static class OperatorMethod
{
    
    public static Operator FromText(this string self) => self switch {
                                                             "=" => Operator.Eq,
                                                             "<" => Operator.Lt,
                                                             ">" => Operator.Gt,
                                                            "?" => Operator.None
                                                        };
    
    public static string ToString(this Operator self) => self switch {
                                                            Operator.Eq => "=",
                                                            Operator.Lt => "<",
                                                            Operator.Gt => ">",
                                                            _ => "?"
                                                        };
    public static string AsText(this Operator self) => self switch {
                                                            Operator.Eq => "=",
                                                            Operator.Lt => "<",
                                                            Operator.Gt => ">",
                                                            _ => "?"
                                                        };
}